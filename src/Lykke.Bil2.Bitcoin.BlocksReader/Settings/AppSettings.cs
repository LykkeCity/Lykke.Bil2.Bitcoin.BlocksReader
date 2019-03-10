using Lykke.Bil2.Sdk.BlocksReader.Settings;

namespace Lykke.Bil2.Bitcoin.BlocksReader.Settings
{
    /// <summary>
    /// Specific blockchain settings
    /// </summary>
    public class AppSettings : BaseBlocksReaderSettings<DbSettings>
    {
        public string Network { get; set; }

        public RpcClientSettings Rpc { get; set; }
    }
}
