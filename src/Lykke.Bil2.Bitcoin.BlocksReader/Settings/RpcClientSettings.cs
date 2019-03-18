using Lykke.Bil2.Sdk.Services;

namespace Lykke.Bil2.Bitcoin.BlocksReader.Settings
{
    public class RpcClientSettings
    {
        public string Host { get; set; }

        public string UserName { get; set; }

        [SecureSettings]
        public string Password { get; set; }
    }
}
