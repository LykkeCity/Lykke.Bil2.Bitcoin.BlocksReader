using System.Linq;
using IdentityModel;
using Lykke.Bil2.SharedDomain;
using NBitcoin;

namespace Lykke.Bil2.Bitcoin.BlocksReader.Services.Helpers
{
    public static class AddressExtractorExtensions
    {

        public static bool IsUnrecognizedAddress(this Script script)
        {
            return script.ToOps().First().Code == OpcodeType.OP_RETURN;  //https://en.bitcoin.it/wiki/OP_RETURN data transaction without address
        }

        public static Address ExtractAddress(this Script script, Network network)
        {
            var common = script.GetDestinationAddress(network);
            if (common != null)
            {
                return common.ToString();
            }

            if (PayToPubkeyTemplate.Instance.CheckScriptPubKey(script))
            {
                return PayToPubkeyTemplate.Instance.ExtractScriptPubKeyParameters(script).GetAddress(network)
                    .ToString();
            }

            if (PayToPubkeyHashTemplate.Instance.CheckScriptPubKey(script))
            {
                return PayToPubkeyHashTemplate.Instance.ExtractScriptPubKeyParameters(script).GetAddress(network)
                    .ToString();
            }

            if (PayToScriptHashTemplate.Instance.CheckScriptPubKey(script))
            {
                return PayToScriptHashTemplate.Instance.ExtractScriptPubKeyParameters(script).GetAddress(network)
                    .ToString();
            }
            
            if (PayToWitPubKeyHashTemplate.Instance.CheckScriptPubKey(script))
            {
                return PayToWitPubKeyHashTemplate.Instance.ExtractScriptPubKeyParameters(script).GetAddress(network)
                    .ToString();
            }

            if (PayToMultiSigTemplate.Instance.CheckScriptPubKey(script))
            {
                var pubKeys = PayToMultiSigTemplate.Instance.ExtractScriptPubKeyParameters(script);

                var addresses = pubKeys.PubKeys.Select(p => p.GetAddress(network).ToString()).OrderBy(p => p).ToList();

                if (addresses.Count == 1)
                {
                    return addresses.Single();
                }

                return $"msig-{string.Join("_", addresses).ToSha256()}";
            }

            if (IsUnrecognizedAddress(script))
            {
                return null;
            }

            return Address.Unrecognized;
        }
    }
}
