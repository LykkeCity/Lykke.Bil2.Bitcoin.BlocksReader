using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Bil2.Sdk.BlocksReader.Services;
using NBitcoin;
using NBitcoin.RPC;

namespace Lykke.Bil2.Bitcoin.BlocksReader.Services
{
    public class BlockReader : IBlockReader
    {
        private readonly RPCClient _rpcClient;
        private readonly Network _network;

        public BlockReader(RPCClient rpcClient, Network network)
        {
            _rpcClient = rpcClient;
            _network = network;
        }


        public async Task ReadBlockAsync(long blockNumber, IBlockListener listener)
        {
            var block = await _rpcClient.GetBlockAsync((int) blockNumber);

            //TODO handle Invalid block number

            await listener.HandleHeaderAsync(new BlockHeaderReadEvent(
                blockNumber,
                block.Header.GetHash().ToString(),
                block.Header.BlockTime.DateTime,
                block.GetSerializedSize(),
                block.Transactions.Count,
                block.Header.HashPrevBlock.ToString()
            ));

            for (int i = 0; i < block.Transactions.Count; i++)
            {
                var tx = block.Transactions[i];

                await listener.HandleExecutedTransactionAsync(
                    tx.ToHex().ToBase58(),
                    new TransferCoinsTransactionExecutedEvent(
                        block.Header.GetHash().ToString(),
                        i,
                        tx.GetHash().ToString(),
                        tx.Outputs.AsIndexedOutputs()
                            .Where(vout => vout.TxOut.ScriptPubKey?.GetDestinationAddress(_network)?.ToString() != null)
                            .Select(vout => new ReceivedCoin(
                                (int)vout.N,
                                "BTC",
                                CoinsAmount.FromDecimal(vout.TxOut.Value.ToUnit(MoneyUnit.BTC), 8),
                                new Address(vout.TxOut.ScriptPubKey?.GetDestinationAddress(_network).ToString())))
                            .ToList(),
                        tx.Inputs.AsIndexedInputs()
                            .Where(p => !p.PrevOut.IsNull)
                            .Select(vin => new CoinReference(vin.PrevOut.Hash.ToString(), (int) vin.PrevOut.N))
                            .ToList(),
                        isIrreversible: false
                    )
                );
            }
        }
    }
}