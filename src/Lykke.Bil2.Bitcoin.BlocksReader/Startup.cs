using System.ComponentModel;
using System;
using System.Net;
using JetBrains.Annotations;
using Lykke.Bil2.Bitcoin.BlocksReader.Services;
using Lykke.Bil2.Bitcoin.BlocksReader.Settings;
using Lykke.Bil2.Sdk.BlocksReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Lykke.Bil2.Sdk.BlocksReader.Services;
using NBitcoin;
using NBitcoin.RPC;

namespace Lykke.Bil2.Bitcoin.BlocksReader
{
    [UsedImplicitly]
    public class Startup
    {
        private const string IntegrationName = "Bitcoin";

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildBlockchainBlocksReaderServiceProvider<AppSettings>(options =>
            {
                options.IntegrationName = IntegrationName;

                // Register required service implementations:

                options.BlockReaderFactory = ctx =>
                    new BlockReader
                    (
                        new RPCClient(
                            new NetworkCredential(ctx.Settings.CurrentValue.Rpc.UserName, ctx.Settings.CurrentValue.Rpc.Password),
                            new Uri(ctx.Settings.CurrentValue.Rpc.Host),
                            Network.GetNetwork(ctx.Settings.CurrentValue.Network)),
                        Network.GetNetwork(ctx.Settings.CurrentValue.Network)
                    );
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseBlockchainBlocksReader(options =>
            {
                options.IntegrationName = IntegrationName;
            });
        }
    }
}