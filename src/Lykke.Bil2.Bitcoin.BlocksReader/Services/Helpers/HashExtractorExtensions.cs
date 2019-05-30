using System.Collections.Generic;
using NBitcoin;

namespace Lykke.Bil2.Bitcoin.BlocksReader.Services.Helpers
{
    public static class HashExtractorExtensions
    {
        //tx hash violations https://github.com/bitcoin/bitcoin/commit/ab91bf39b7c11e9c86bb2043c24f0f377f1cf514#diff-7ec3c68a81efff79b6ca22ac1f1eabbaR1367
        private static readonly IReadOnlyDictionary<(uint256 blockHash, uint256 transactionHash), long>  Violations = new Dictionary<(uint256 blockHash, uint256 transactionHash), long>
        {
            {(uint256.Parse("00000000000a4d0a398161ffc163c503763b1f4360639393e0e4c8e300e0caec"), uint256.Parse("d5d27987d2a3dfc724e359870c6644b40e497bdc0589a033220fe15429d88599")), 0},
            {(uint256.Parse("00000000000743f190a18c5577a3c2d2a1f610ae9601ac046a38084ccb7cd721 "), uint256.Parse("e3bf3d07d4b0375638d5f1db5255fe07ba2c4cb067cd81b84ee974b6585fb468")), 0},
        }; 

        public static string ExtractHash(this Transaction source, BlockHeader header)
        {
            var blockHash = header.GetHash();
            var transactionHash = source.GetHash();

            if (Violations.TryGetValue((blockHash, transactionHash), out var suffix))
            {
                return $"{transactionHash}_{suffix}";
            }

            return transactionHash.ToString();
        }
    }
}
