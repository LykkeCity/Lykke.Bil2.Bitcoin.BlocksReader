using Lykke.Bil2.Sdk.BlocksReader;
using Lykke.Bil2.Sdk.BlocksReader.Settings;

namespace Lykke.Bil2.Bitcoin.BlocksReader.Settings
{
    /// <summary>
    /// Specific blockchain settings
    /// </summary>
    public class AppSettings : BaseBlocksReaderSettings<DbSettings>
    {
        // Implement specific blockchain settings, if necessary.
        // Mark sensitive data with SecureSettingsAttribute to prevent leaks.
        //
        // For example:
        //
        // public string NodeUrl { get; set; }
        //
        // public string NodeRpcUsername { get; set; }
        //
        // [SecureSettings]
        // public string NodeRpcPassword { get; set; }
    }
}
