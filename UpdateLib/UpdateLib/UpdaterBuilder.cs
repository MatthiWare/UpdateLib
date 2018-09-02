using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.UpdateLib.Abstractions;
using MatthiWare.UpdateLib.Abstractions.Internal;
using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Core;
using MatthiWare.UpdateLib.Core.Internal;
using MatthiWare.UpdateLib.Core.Internal.CommandLine;
using MatthiWare.UpdateLib.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MatthiWare.UpdateLib
{
    public class UpdaterBuilder
    {
        private IServiceCollection services;
        private UpdateLibOptions options;

        private UpdaterBuilder()
        {
            services = new ServiceCollection();

            options = new UpdateLibOptions
            {
                CommandLineArgumentPrefix = "--",
                RollbackArgumentName = "rollback",
                UpdateArgumentName = "update",
                WaitArgumentName = "wait"
            };
        }

        public UpdaterBuilder UseServiceCollection(IServiceCollection services)
        {
            services = Guard.NotNull(services, nameof(services));

            return this;
        }

        public UpdaterBuilder UseCommandLineArgumentPrefix(string prefix)
        {
            options.CommandLineArgumentPrefix = Guard.NotNullOrEmpty(prefix, nameof(prefix));

            return this;
        }

        public UpdaterBuilder UseUpdateUrl(string url)
        {
            options.UpdateUrl = Guard.NotNullOrEmpty(url, nameof(url));

            return this;
        }

        public UpdaterBuilder UseWaitArgument(string argument)
        {
            options.WaitArgumentName = Guard.NotNullOrEmpty(argument, nameof(argument));

            return this;
        }

        public UpdaterBuilder UseUpdateArgument(string argument)
        {
            options.UpdateArgumentName = Guard.NotNullOrEmpty(argument, nameof(argument));

            return this;
        }

        public UpdaterBuilder UseRollbackArgument(string argument)
        {
            options.RollbackArgumentName = Guard.NotNullOrEmpty(argument, nameof(argument));

            return this;
        }

        public static UpdaterBuilder CreateDefaultUpdateBuilder() => new UpdaterBuilder();

        public IUpdater Build()
        {
            Validate();

            RegisterDefaultServices();
            RegisterCommandLineServices();

            var svcProvider = services.BuildServiceProvider();

            return svcProvider.GetRequiredService<IUpdater>();
        }

        private void RegisterCommandLineServices()
        {
            services.AddTransient<ICommandLineArgumentResolver<int>, IntArgumentResolver>();
            services.AddTransient<ICommandLineArgumentResolver<int[]>, MultipleIntArgumentResolver>();
            services.AddTransient<ICommandLineArgumentResolver<string>, StringArgumentResolver>();
            services.AddTransient<ICommandLineArgumentResolver<UpdateVersion>, UpdateVersionArgumentResolver>();
        }

        private void RegisterDefaultServices()
        {
            services.AddTransient<IValueResolver<UpdateVersion>, DefaultVersionResolver>();
            services.AddTransient<IUpdateManager, UpdateManager>();

            services.AddOptions();

            services.ConfigureOptions(options);

            services.AddSingleton<IUpdater, Updater>();
        }

        private void Validate()
        {
            options.UpdateUrl.NotNullOrEmpty(nameof(options.UpdateUrl));
        }
    }
}
