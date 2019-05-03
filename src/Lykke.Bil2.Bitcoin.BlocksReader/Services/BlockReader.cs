using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Bil2.Bitcoin.BlocksReader.Services.Helpers;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.Sdk.BlocksReader.Services;
using Lykke.Bil2.SharedDomain;
using Lykke.Bil2.SharedDomain.Extensions;
using Lykke.Numerics;
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
            Block block;
            
            try
            {
                block = await _rpcClient.GetBlockAsync((int)blockNumber);
            }
            catch (RPCException e) when(e.RPCCode == RPCErrorCode.RPC_INVALID_PARAMETER)
            {
                listener.HandleBlockNotFound(new BlockNotFoundEvent(blockNumber));

                return;
            }

            var blockHash = block.Header.GetHash().ToString();
            var blockId = new BlockId(blockHash);

            listener.HandleRawBlock(block.ToBytes().EncodeToBase64(), blockId);

            var transactionsListener = listener.StartBlockTransactionsHandling(new BlockHeaderReadEvent(
                blockNumber,
                blockHash,
                block.Header.BlockTime.DateTime,
                block.GetSerializedSize(),
                block.Transactions.Count,
                block.Header.HashPrevBlock.ToString()
            ));
            
            for (var i = 0; i < block.Transactions.Count; i++)
            {
                var tx = block.Transactions[i];
                var txId = tx.GetHash().ToString();

                transactionsListener.HandleExecutedTransaction
                (
                    new TransferCoinsExecutedTransaction
                    (
                        i,
                        txId,
                        tx.Outputs.AsIndexedOutputs()
                            .Select(output =>
                            {
                                var address = output.TxOut.ScriptPubKey.ExtractAddress(_network);

                                return new ReceivedCoin(
                                    (int) output.N,
                                    new Asset(new AssetId("BTC")),
                                    new UMoney(new BigInteger(output.TxOut.Value.ToUnit(MoneyUnit.Satoshi)), 8),
                                    address);
                            })
                            .ToList(),
                        tx.Inputs.AsIndexedInputs()
                            .Where(p => !p.PrevOut.IsNull)
                            .Select(vin => new CoinId(vin.PrevOut.Hash.ToString(), (int) vin.PrevOut.N))
                            .ToList(),
                        isIrreversible: false
                    )
                );

                await transactionsListener.HandleRawTransactionAsync(tx.ToBytes().EncodeToBase64(), txId);
            }
        }
    }
}