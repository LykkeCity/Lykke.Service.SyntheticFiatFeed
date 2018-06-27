using System;
using Autofac;
using Autofac.Core;
using JetBrains.Annotations;
using Lykke.Common.Log;

namespace Lykke.Service.SyntheticFiatFeed.Client
{
    [PublicAPI]
    public static class AutofacExtension
    {
        public static void RegisterSyntheticFiatFeedClient(this ContainerBuilder builder, string serviceUrl)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrWhiteSpace(serviceUrl))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));
            }

            builder.RegisterType<SyntheticFiatFeedClient>()
                .WithParameter("serviceUrl", serviceUrl)
                .As<ISyntheticFiatFeedClient>()
                .SingleInstance();
        }

        public static void RegisterSyntheticFiatFeedClient(this ContainerBuilder builder, SyntheticFiatFeedServiceClientSettings settings)
        {
            builder.RegisterSyntheticFiatFeedClient(settings?.ServiceUrl);
        }
    }
}
