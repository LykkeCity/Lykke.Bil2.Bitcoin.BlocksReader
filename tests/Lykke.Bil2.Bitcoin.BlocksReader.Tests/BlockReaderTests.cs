using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Bil2.Bitcoin.BlocksReader.Services;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.Sdk.BlocksReader.Services;
using Lykke.Bil2.SharedDomain;
using Lykke.Numerics;
using Moq;
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.RPC;
using NUnit.Framework;
using Money = NBitcoin.Money;

namespace Lykke.Bil2.Bitcoin.BlocksReader.Tests
{
    [TestFixture]
    public class BlockReaderTests
    {
        private readonly RPCClient _rpcClient;
        private readonly Mock<IBlockListener> _blockListenerMock;

        public BlockReaderTests()
        {
            _rpcClient = RpcClientFactory.Create();
            _blockListenerMock = new Mock<IBlockListener>();
        }

        [Test]
        public async Task Can_Parse_Block()
        {
            var blReader = new BlockReader(_rpcClient, Network.TestNet);

            await blReader.ReadBlockAsync(240065, _blockListenerMock.Object);

            _blockListenerMock.Verify(x => x.HandleHeaderAsync(
                It.Is<BlockHeaderReadEvent>(ev=> 
                    ev.BlockId == "41bdf1cfd3cc36f3c2f67729fc546d580b668f7ae3c7d190cd4b42496a62106d" && 
                    ev.PreviousBlockId == "2dd02e2e53a6a8906a2dfd9f9bdf72b0c271193a74c6f61a8c5401af8bb3c478" &&
                    ev.BlockTransactionsCount == 4)), 
                Times.Once);

            var blockBytes = Encoders.Hex.DecodeData("0000002078c4b38baf01548c1af6c6743a1971c2b072df9b9ffd2d6a90a8a6532e2ed02d6b95c932c433282d38d657ef406efa5dc93646e18e7e02912efbed8b23a5bd73a516815cffff7f200000000004020000000001010000000000000000000000000000000000000000000000000000000000000000ffffffff0603b470030101ffffffff02143a00000000000023210224cc6f0baa45afa6738cdd704f9567979689cd403c2261eaf54b53ddca8d35e1ac0000000000000000266a24aa21a9ed8eedbbce7fbe2789aa24d2f0a19a17d17547be8e85a4b1ae8da3cd13974c2a5e012000000000000000000000000000000000000000000000000000000000000000000000000001000000000101a6c097d359de0fe589bb96b09a3a2f52a0320338a90c3cc4f8da2227a1a94633010000001716001457a4720ac6221fe84ca72b672c62ca9073a9595fffffffff012c320f000000000017a9143ae45b5b44661086fa16ece1d3824a032edad042870247304402204e48a044c4b6a0ae31ccac4cbbe2761d14ae9d0e9873a536034659c5521da62502204a3a578975625d8cf4a778f84d4d67cfe3f16b9c289406eea7a24bf97631b2860121031b4dbdf71fe3ab46b82f15871d04c2891e8d75cba81c2b4ae28c57e52554d9c8000000000100000001f4fef6f2bba226d6742abc945c5c658fdce49e8a08b980c52e3a04fa52ea9642010000006a47304402204165755bf17d61bef32bbc5d8a91b4b587115d32c8cfd1446f68ff4819cb1fb302207240e405fbe471af0b6819318d50ae1c9f0174da048a5119ded7730db0ad692801210218724079a342c0385cee4944a1e4434aa4a44606520c107ae772839821f8b4fcffffffff024099eb02000000001976a914a7f48354c754828a6b673db5c8340110daa577f288ac40420f000000000017a914e7c08818e2c00a279e3946777b9a76f5fe44bdb487000000000100000001a6c097d359de0fe589bb96b09a3a2f52a0320338a90c3cc4f8da2227a1a94633000000006a47304402201e558fa724ba1b38eb14bbd22995ab34b4e87f6391b82c9cee2fd3f29e39ca4a02200bf77f15ea599a840c64a992535e35976a467e6c8ebfca9f789347d0935ce9a001210218724079a342c0385cee4944a1e4434aa4a44606520c107ae772839821f8b4fcffffffff0293a16301000000001976a914a7f48354c754828a6b673db5c8340110daa577f288ac40420f000000000017a914e7c08818e2c00a279e3946777b9a76f5fe44bdb48700000000");

            _blockListenerMock.Verify(x => x.HandleRawBlockAsync(
                    It.Is<Base64String>(p => p.DecodeToBytes().SequenceEqual(blockBytes) ),
                    It.Is<BlockId>(p => p == "41bdf1cfd3cc36f3c2f67729fc546d580b668f7ae3c7d190cd4b42496a62106d")),
                Times.Once);

            var tx1Bytes = Encoders.Hex.DecodeData("020000000001010000000000000000000000000000000000000000000000000000000000000000ffffffff0603b470030101ffffffff02143a00000000000023210224cc6f0baa45afa6738cdd704f9567979689cd403c2261eaf54b53ddca8d35e1ac0000000000000000266a24aa21a9ed8eedbbce7fbe2789aa24d2f0a19a17d17547be8e85a4b1ae8da3cd13974c2a5e0120000000000000000000000000000000000000000000000000000000000000000000000000");

            _blockListenerMock.Verify(x => x.HandleExecutedTransactionAsync(
                    It.Is<Base64String>(p => p.DecodeToBytes().SequenceEqual(tx1Bytes)),
                It.Is<TransferCoinsTransactionExecutedEvent>(p =>
                    p.TransactionId == "ba938c9f5188956f0a36e01e6878c5bc98c3c2e9dcb2209898bf03115fa72537" &&
                    p.TransactionNumber == 0 & !p.SpentCoins.Any())),
                Times.Once);

            var tx2Bytes = Encoders.Hex.DecodeData("01000000000101a6c097d359de0fe589bb96b09a3a2f52a0320338a90c3cc4f8da2227a1a94633010000001716001457a4720ac6221fe84ca72b672c62ca9073a9595fffffffff012c320f000000000017a9143ae45b5b44661086fa16ece1d3824a032edad042870247304402204e48a044c4b6a0ae31ccac4cbbe2761d14ae9d0e9873a536034659c5521da62502204a3a578975625d8cf4a778f84d4d67cfe3f16b9c289406eea7a24bf97631b2860121031b4dbdf71fe3ab46b82f15871d04c2891e8d75cba81c2b4ae28c57e52554d9c800000000");
            
            //https://private-bcn-explorer-test.lykkex.net/transaction/88a5407783a2ca99b0dad75624aaefdd9c1f7a092ea63d5ec93d48fc4d49c4ed
            _blockListenerMock.Verify(x => x.HandleExecutedTransactionAsync(
                    It.Is<Base64String>(p => p.DecodeToBytes().SequenceEqual(tx2Bytes)),
                It.Is<TransferCoinsTransactionExecutedEvent>(p =>
                    p.TransactionId == "88a5407783a2ca99b0dad75624aaefdd9c1f7a092ea63d5ec93d48fc4d49c4ed" &&
                    p.ReceivedCoins.Single().Address == "2MxccjvVEmW2XDLtZVE4nhMijgJi2APMCgf" &&
                    p.ReceivedCoins.Single().Value == new UMoney(new BigInteger(new Money(0.00995884m, MoneyUnit.BTC ).ToUnit(MoneyUnit.Satoshi)), 8)
                    )),
                Times.Once);

            var tx3Bytes = Encoders.Hex.DecodeData("0100000001f4fef6f2bba226d6742abc945c5c658fdce49e8a08b980c52e3a04fa52ea9642010000006a47304402204165755bf17d61bef32bbc5d8a91b4b587115d32c8cfd1446f68ff4819cb1fb302207240e405fbe471af0b6819318d50ae1c9f0174da048a5119ded7730db0ad692801210218724079a342c0385cee4944a1e4434aa4a44606520c107ae772839821f8b4fcffffffff024099eb02000000001976a914a7f48354c754828a6b673db5c8340110daa577f288ac40420f000000000017a914e7c08818e2c00a279e3946777b9a76f5fe44bdb48700000000");

            _blockListenerMock.Verify(x => x.HandleExecutedTransactionAsync(
                    It.Is<Base64String>(p => p.DecodeToBytes().SequenceEqual(tx3Bytes)),
                It.Is<TransferCoinsTransactionExecutedEvent>(p => p.TransactionNumber == 2 &&
                                                                  p.TransactionId == "561563fb5321d2805540e388b3f41f0067447833fba4311429588987a348ba54"  &&
                                                                  p.ReceivedCoins.First().Address == "mvq249kND1czVbgbNVYbEWszW5PVeXon6u" &&
                                                                  p.ReceivedCoins.First().Value == new UMoney(new BigInteger(new Money(0.48994624m, MoneyUnit.BTC).ToUnit(MoneyUnit.Satoshi)), 8) &&
                                                                  p.ReceivedCoins.Skip(1).Single().Address == "2NENcivqBdw5ovBnCo8ovaTryHKCkoqCuB6" &&
                                                                  p.ReceivedCoins.Skip(1).Single().Value == new UMoney(new BigInteger(new Money(0.01m, MoneyUnit.BTC).ToUnit(MoneyUnit.Satoshi)), 8))),
                Times.Once);

            var tx4Bytes = Encoders.Hex.DecodeData("0100000001a6c097d359de0fe589bb96b09a3a2f52a0320338a90c3cc4f8da2227a1a94633000000006a47304402201e558fa724ba1b38eb14bbd22995ab34b4e87f6391b82c9cee2fd3f29e39ca4a02200bf77f15ea599a840c64a992535e35976a467e6c8ebfca9f789347d0935ce9a001210218724079a342c0385cee4944a1e4434aa4a44606520c107ae772839821f8b4fcffffffff0293a16301000000001976a914a7f48354c754828a6b673db5c8340110daa577f288ac40420f000000000017a914e7c08818e2c00a279e3946777b9a76f5fe44bdb48700000000");

            _blockListenerMock.Verify(x => x.HandleExecutedTransactionAsync(
                It.Is<Base64String>(p => p.DecodeToBytes().SequenceEqual(tx4Bytes)),
                It.Is<TransferCoinsTransactionExecutedEvent>(p => p.TransactionId == "2e9dc9e1e5ac5b8839018b490de942f1992bb8f3dd90eb93c5ecd69cdafbe6f4")),
                Times.Once);

            _blockListenerMock.VerifyNoOtherCalls();
        }


        [Test]
        public async Task Can_Proceed_Invalid_Block_Num()
        {
            var blReader = new BlockReader(_rpcClient, Network.TestNet);

            await blReader.ReadBlockAsync(int.MaxValue, _blockListenerMock.Object);

            _blockListenerMock.Verify(x => x.HandleBlockNotFoundAsync(It.Is<BlockNotFoundEvent>(ev => ev.BlockNumber == int.MaxValue)),
                Times.Once);
        }
    }
}