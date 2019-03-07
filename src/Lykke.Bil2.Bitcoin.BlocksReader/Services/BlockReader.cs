using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.Sdk.BlocksReader.Services;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.TransactionsExecutor;

namespace Lykke.Bil2.Bitcoin.BlocksReader.Services
{
    public class BlockReader : IBlockReader
    {
        public BlockReader(/* TODO: Provide specific settings and dependencies, if necessary */)
        {
        }

        public Task ReadBlockAsync(long blockNumber, IBlockListener listener)
        {
            // TODO: Process block with specified number, emit header and transaction(s) events
            //
            // For example
            //
            // var block = ...;
            //
            // listener.HandleHeaderAsync(new BlockHeaderReadEvent(
            //     block.Number,
            //     block.Hash,
            //     block.Time,
            //     block.Raw.Length,
            //     block.Transactions.Count,
            //     block.Previous.Hash
            // ));

            // for (int i = 0; i < block.Transactions.Length; i++)
            // {
            //     var tx = block.Transactions[i];

            //     if (tx.State == "SUCCESS")
            //     {
            //
            //         // If blockchain uses amount transfer scheme (is JSON-based) then emit TransferAmountTransactionExecutedEvent
            //
            //         listener.HandleExecutedTransactionAsync(
            //             tx.Raw.ToBase58(),
            //             new TransferAmountTransactionExecutedEvent(
            //                 block.Hash,
            //                 i,
            //                 tx.Hash,
            //                 tx.Actions.SelectMany(act =>
            //                 {
            //                     yield return new BalanceChange("0", act.ActionId, act.Token.Id, CoinsChange.FromDecimal((-1) * act.Amount, act.Token.Accuracy), act.From);
            //                     yield return new BalanceChange("1", act.ActionId, act.Token.Id, CoinsChange.FromDecimal(act.Amount, act.Token.Accuracy), act.To);
            //                 }),
            //                 fee: null,
            //                 isIrreversible: true
            //             )
            //         );
            //
            //         // If blockchain uses coins transfer scheme (is UTXO-based) then emit TransferCoinsTransactionExecutedEvent
            //
            //         listener.HandleExecutedTransactionAsync(
            //             tx.Raw.ToBase58(),
            //             new TransferCoinsTransactionExecutedEvent(
            //                 block.Hash,
            //                 i,
            //                 tx.Hash,
            //                 tx.Outputs.Select(vout => new ReceivedCoin(vout.Number, "BTC", CoinsAmount.FromDecimal(vout.Amount, 8), vout.Address)),
            //                 tx.Inputs.Select(vin => new CoinReference(vin.Hash, vin.Number)),
            //                 new Dictionary<AssetId, CoinsAmount> { { "BTC", CoinsAmount.FromDecimal(tx.Fee) } },
            //                 isIrreversible: null
            //             )
            //         );
            //     }
            //
            //     if (tx.State == "FAILURE")
            //     {
            //         listener.HandleFailedTransactionAsync(
            //             tx.Raw.ToBase58(),
            //             new TransactionFailedEvent(
            //                 block.Hash,
            //                 i,
            //                 tx.Hash,
            //                 TransactionBroadcastingError.RebuildRequired,
            //                 tx.Error,
            //                 fee: null
            //             )
            //         );
            //     }
            // }


            throw new System.NotImplementedException();
        }
    }
}