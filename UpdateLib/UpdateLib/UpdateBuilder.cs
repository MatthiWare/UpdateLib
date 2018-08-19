using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.UpdateLib.Abstractions;
using MatthiWare.UpdateLib.Abstractions.Internal;
using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Core;
using MatthiWare.UpdateLib.Core.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MatthiWare.UpdateLib
{
    public class UpdateBuilder
    {
        private IServiceCollection services;
        private string remoteUrl;

        private UpdateBuilder()
        {
            services = new ServiceCollection();
        }

        public static UpdateBuilder CreateDefaultUpdateBuilder()
        {
            return new UpdateBuilder();
        }

        public IUpdater Build()
        {
            Validate();

            RegisterDefaultServices();

            var svcProvider = services.BuildServiceProvider();

            return svcProvider.GetRequiredService<IUpdater>();
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(remoteUrl))
                throw new ArgumentNullException("Specify the remote url");

        }

        private void RegisterDefaultServices()
        {
            services.AddTransient<IValueResolver<UpdateVersion>, DefaultVersionResolver>();
            services.AddTransient<IUpdateManager, UpdateManager>();
            services.AddTransient<IConfigureOptions<UpdateLibOptions>, UpdateLibOptionsSetup>();
            services.AddSingleton<IUpdater, Updater>();
        }
    }
}
