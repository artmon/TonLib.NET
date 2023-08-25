using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public static class Program
    {
        // You will need actual mnemonic and address with some coins to run tests like SendTon, SendJetton etc.
        // These tests have safeguards and will not run on mainnet.
        // But anyway, double check that you are using testnet and what tests are uncommented before putting actual seed phrase here!!!
        public const string TestAddress = "EQAgEYgZ0oabZtyby9v_SzfpPEXHLRNi8Hlwnyv7k74dfVq7";
        public const string DestAddress = "EQCb_XPAB_riViDmTb0bUua-dAehlczJDM_VLSXKX2zm70Ib";
        public const string TestMnemonic = "carry october thumb spell state topic cube moral move home physical legend need silly enact loan orient ice travel prepare smooth luggage canyon session"; // put space-delimited mnemonic words here
        public const string DestMnemonic = "slim chair rather lizard strategy winter total vote happy aerobic any tape veteran fashion exact liberty box error bronze relief tragic excess path subject"; // put space-delimited mnemonic words here

        // Some tests need mainnet (e.g. domains), some will run only in testnet (e.g. sending coins).
        public const bool UseMainnet = false;

        private const string DirectoryForKeys = "C:/Temp/keys";

        public static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            builder.UseConsoleLifetime();
            builder.ConfigureServices((context, services) =>
            {
                services.Configure<TonOptions>(o =>
                {
                    o.UseMainnet = UseMainnet;
                    o.LogTextLimit = 500; // Set to 0 to see full requests/responses
                    o.VerbosityLevel = 0;
                    o.Options.KeystoreType = new KeyStoreTypeInMemory(); // KeyStoreTypeDirectory(DirectoryForKeys);
                });

                services.AddHostedService<SamplesRunner>();

                services.AddSingleton<ITonClient, TonClient>();

                var samples = typeof(Program).Assembly.GetTypes().Where(x => x.IsClass && x.IsAssignableTo(typeof(ISample))).ToList();
                foreach (var sample in samples)
                {
                    services.AddTransient(typeof(ISample), sample);
                }
            });

            /// Add types from current assembly (see <see cref="LibraryExtensibility"/> class for more info).
            TonClient.RegisterAssembly(typeof(Program).Assembly);

            var app = builder.Build();
            await app.RunAsync();
        }
    }
}