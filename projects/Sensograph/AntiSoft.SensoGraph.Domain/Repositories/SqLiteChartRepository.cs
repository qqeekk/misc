using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using AntiSoft.SensoGraph.Domain.Abstractions;
using AntiSoft.SensoGraph.Domain.Entities;

namespace AntiSoft.SensoGraph.Domain.Repositories
{
    /// <summary>
    /// SQLite repository for charts.
    /// </summary>
    public sealed class SqLiteChartRepository : IChartRepository
    {
        private const string TableName = "metrics";

        private readonly string sqliteConnectionString;

        private static int? DecimalToInt(decimal? val) => val.HasValue ? (int)(val * 100) : null;

        private static decimal IntToDecimal(int val) => (decimal)(val / 100d);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sqliteConnectionString">SQLite connection string.</param>
        public SqLiteChartRepository(string sqliteConnectionString)
        {
            this.sqliteConnectionString = sqliteConnectionString;
        }

        /// <inheritdoc />
        public async Task<IList<ChartItem>> GetForPeriod(DateTime start, DateTime end, CancellationToken cancellationToken = default)
        {
            await using var connection = new SqliteConnection(connectionString: sqliteConnectionString);
            await connection.OpenAsync(cancellationToken);

            await using var selectCommand = connection.CreateCommand();
            selectCommand.CommandText =
                $"SELECT * FROM {TableName} WHERE timestamp BETWEEN @start AND @end ORDER BY timestamp DESC;";
            selectCommand.Parameters.AddWithValue("@start", start)
                .SqliteType = SqliteType.Real;
            selectCommand.Parameters.AddWithValue("@end", end)
                .SqliteType = SqliteType.Real;
            await using var reader =  await selectCommand.ExecuteReaderAsync(cancellationToken);
            var graphEntries = new List<ChartItem>();
            while (await reader.ReadAsync(cancellationToken))
            {
                var graphEntry = new ChartItem();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    switch (name)
                    {
                        case "timestamp": graphEntry.Timestamp = reader.GetDateTime(i); break;
                        default:
                            if (!reader.IsDBNull(i))
                            {
                                graphEntry.CodeValues[name] = IntToDecimal(reader.GetInt32(i));
                            }
                            break;
                    }
                }
                graphEntries.Add(graphEntry);
            }
            return graphEntries;
        }

        /// <inheritdoc />
        public async Task Add(ChartItem item, CancellationToken cancellationToken = default)
        {
            var codeValues = item.CodeValues.ToArray();
            var sb = new StringBuilder()
                .AppendLine($"INSERT INTO {TableName}")
                .AppendLine("(")
                .AppendLine("timestamp,")
                .AppendLine(string.Join(", ", codeValues.Select(cv => cv.Key)))
                .AppendLine(") VALUES (")
                .AppendLine($"@timestamp, ")
                .AppendLine(string.Join(", ", codeValues.Select(cv => "@" + cv.Key)))
                .AppendLine(");");

            await using var connection = new SqliteConnection(connectionString: sqliteConnectionString);
            await connection.OpenAsync(cancellationToken);
            await using var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = sb.ToString();
            insertCommand.Parameters.AddWithValue("@timestamp", item.Timestamp)
                .SqliteType = SqliteType.Real;
            foreach (var keyValuePair in codeValues)
            {
                insertCommand.Parameters.AddWithValue("@" + keyValuePair.Key, DecimalToInt(keyValuePair.Value))
                    .SqliteType = SqliteType.Integer;
            }
            await insertCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int> RemoveOldRecords(DateTime minDateToKeep, CancellationToken cancellationToken = default)
        {
            await using var connection = new SqliteConnection(connectionString: sqliteConnectionString);
            await connection.OpenAsync(cancellationToken);

            await using var deleteCommand = connection.CreateCommand();
            deleteCommand.CommandText = $"DELETE FROM {TableName} WHERE timestamp < @date;";
            deleteCommand.Parameters.AddWithValue("@date", minDateToKeep)
                .SqliteType = SqliteType.Real;
            var removedRecords = await deleteCommand.ExecuteNonQueryAsync(cancellationToken);

            await using var vacuumCommand = connection.CreateCommand();
            vacuumCommand.CommandText = "VACUUM;";
            await vacuumCommand.ExecuteNonQueryAsync(cancellationToken);

            return removedRecords;
        }

        /// <inheritdoc />
        public async Task Initialize(string[] metricCodes, CancellationToken cancellationToken = default)
        {
            var existingColumns = new List<string>();
            await using var connection = new SqliteConnection(connectionString: sqliteConnectionString);
            await connection.OpenAsync(cancellationToken);

            // Create table.
            var sb = new StringBuilder()
                .AppendLine($"CREATE TABLE IF NOT EXISTS {TableName}")
                .AppendLine("(")
                .AppendLine("  [id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,")
                .AppendLine("  [timestamp] DATETIME NOT NULL")
                .AppendLine(");")
                .AppendLine($"CREATE INDEX IF NOT EXISTS ix_{TableName}_timestamp ON {TableName} (timestamp);");
            await using var addTableCommand = connection.CreateCommand();
            addTableCommand.CommandText = sb.ToString();
            await addTableCommand.ExecuteNonQueryAsync(cancellationToken);

            // Get columns.
            await using var getColumnsCommand = connection.CreateCommand();
            getColumnsCommand.CommandText = $"PRAGMA table_info({TableName});";
            await using var reader = await getColumnsCommand.ExecuteReaderAsync(cancellationToken);
            for (int i = 0; await reader.ReadAsync(cancellationToken); i++)
            {
                if (i < 2)
                {
                    continue;
                }
                existingColumns.Add((string)reader["name"]);
            }

            // Add missed columns.
            foreach (var metricCode in metricCodes.Where(mc => !existingColumns.Contains(mc)))
            {
                await using var addColumnsCommand = connection.CreateCommand();
                addColumnsCommand.CommandText = $"ALTER TABLE {TableName} ADD COLUMN {metricCode} INTEGER;";
                await addColumnsCommand.ExecuteNonQueryAsync(cancellationToken);
            }
        }
    }
}
