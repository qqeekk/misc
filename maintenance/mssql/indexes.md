## Currently Running Queries

```sql
select
	sqltext.text as 'SQL',
	requests.session_id,
	requests.status,
	requests.command,
	requests.cpu_time,
	requests.total_elapsed_time,
	sessions.host_name,
	sessions.program_name
from sys.dm_exec_requests requests
left join sys.dm_exec_sessions sessions on requests.session_id = sessions.session_id
cross apply sys.dm_exec_sql_text(sql_handle) AS sqltext
order by requests.total_elapsed_time desc;
```

## Index Fragmentation

```sql
select
	schemas.name as 'schema',
	tables.name as 'table',
	indexes.name as 'index',
	DDIPS.avg_fragmentation_in_percent as 'fragmentation',
	DDIPS.page_count
from sys.dm_db_index_physical_stats(DB_ID(), null, null, null, null) as DDIPS
inner join sys.tables tables on tables.object_id = DDIPS.object_id
inner join sys.schemas schemas on tables.schema_id = schemas.schema_id
inner join sys.indexes indexes ON indexes.object_id = DDIPS.object_id and DDIPS.index_id = indexes.index_id
where DDIPS.database_id = DB_ID() and indexes.name is not null and DDIPS.avg_fragmentation_in_percent > 0
order by DDIPS.avg_fragmentation_in_percent desc;
```

## Rebuild Index

```sql
alter index Index_Name on Table_Name rebuild with (online = off);
```

```
= "alter index " & B1 & " on " & A1 & " rebuild with (online = off);"
```

## Optimize All Indexes

```sql
declare @table_name varchar(255);
declare table_cursor cursor for
    select table_name from information_schema.tables where table_type = 'base table';
open table_cursor;
fetch next from table_cursor into @table_name;
while @@fetch_status = 0
begin
	print @table_name;
	declare @sql nvarchar(500);
	set @sql = N'alter index all on ' + @table_name + N' rebuild with (fillfactor = 80, online = off);';
	execute sp_executesql @sql;
	fetch next from table_cursor into @table_name;
end
close table_cursor;
deallocate table_cursor;
```
