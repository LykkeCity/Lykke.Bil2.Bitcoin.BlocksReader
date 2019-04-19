using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Bil2.Bitcoin.BlocksReader.Services.Helpers;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Bil2.Sdk.BlocksReader.Services;
using Lykke.Bil2.SharedDomain;
using Lykke.Numerics;
using NBitcoin;
using NBitcoin.DataEncoders;
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
            Block block;
            
            try
            {
                block = await _rpcClient.GetBlockAsync((int)blockNumber);
            }
            catch (RPCException e) when(e.RPCCode == RPCErrorCode.RPC_INVALID_PARAMETER)
            {
                await listener.HandleBlockNotFoundAsync(new BlockNotFoundEvent(blockNumber));

                return;
            }

            var blockHash = block.Header.GetHash().ToString();

            await listener.HandleRawBlockAsync(Encoders.Hex.EncodeData(block.ToBytes()).ToBase58(), new BlockId(blockHash));
            
            for (int i = 0; i < block.Transactions.Count; i++)
            {
                var tx = block.Transactions[i];

                await listener.HandleExecutedTransactionAsync(
                    tx.ToHex().ToBase58(),
                    new TransferCoinsTransactionExecutedEvent(
                        blockHash,
                        i,
                        tx.GetHash().ToString(),
                        tx.Outputs.AsIndexedOutputs()
                            .Select(vout =>
                            {
                                var addr = vout.TxOut.ScriptPubKey.ExtractAddress(_network);

                                return new ReceivedCoin(
                                    (int) vout.N,
                                    new Asset(new AssetId("BTC")),
                                    new UMoney(new BigInteger(vout.TxOut.Value.ToUnit(MoneyUnit.Satoshi)), 8),
                                    addr != null ? new Address(vout.TxOut.ScriptPubKey.ExtractAddress(_network)) : null);
                            })
                            .ToList(),
                        tx.Inputs.AsIndexedInputs()
                            .Where(p => !p.PrevOut.IsNull)
                            .Select(vin => new CoinId(vin.PrevOut.Hash.ToString(), (int) vin.PrevOut.N))
                            .ToList(),
                        isIrreversible: false
                    )
                );
            }

            await listener.HandleHeaderAsync(new BlockHeaderReadEvent(
                blockNumber,
                blockHash,
                block.Header.BlockTime.DateTime,
                block.GetSerializedSize(),
                block.Transactions.Count,
                block.Header.HashPrevBlock.ToString()
            ));
        }
    }
}