using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using VStarCameraZone.Commands;

namespace VStarCameraZone.Infrastructure.DependencyInjection;

/// <summary>
/// Resolve CLI commands with DI.
/// </summary>
/// <remarks>
/// Source: https://endjin.com/blog/2020/09/simple-pattern-for-using-system-commandline-with-dependency-injection
/// </remarks>
public static class CliCommandCollectionExtensions
{
    public static IServiceCollection AddCliCommands(this IServiceCollection services)
    {
        Type cameraCommand = typeof(CameraCommand);
        Type commandType = typeof(Command);

        IEnumerable<Type> commands = cameraCommand
            .Assembly
            .GetExportedTypes()
            .Where(x => x.Namespace == cameraCommand.Namespace
                && commandType.IsAssignableFrom(x));

        foreach (var command in commands)
        {
            services.AddSingleton(command);
        }

        return services;
    }
}
