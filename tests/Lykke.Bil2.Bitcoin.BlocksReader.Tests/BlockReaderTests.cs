using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Bitcoin.BlocksReader.Services;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Sdk.BlocksReader.Services;
using Moq;
using NBitcoin;
using NBitcoin.RPC;
using NUnit.Framework;
using Xunit;

namespace Lykke.Bil2.Bitcoin.BlocksReader.Tests
{
    public class BlockReaderTests
    {
        private readonly RPCClient _rpcClient;
        private readonly Mock<IBlockListener> _blockListenerMock;

        public BlockReaderTests()
        {
            _rpcClient = RpcClientFactory.Create();
            _blockListenerMock = new Mock<IBlockListener>();
        }

        [TestCase()]
        [Fact]
        public async Task Can_Parse_Block()
        {
            var blReader = new BlockReader(_rpcClient, Network.TestNet);

            await blReader.ReadBlockAsync(225460, _blockListenerMock.Object);

            _blockListenerMock.Verify(x => x.HandleHeaderAsync(
                It.Is<BlockHeaderReadEvent>(ev=> 
                    ev.BlockId == "41bdf1cfd3cc36f3c2f67729fc546d580b668f7ae3c7d190cd4b42496a62106d" && 
                    ev.PreviousBlockId == "2dd02e2e53a6a8906a2dfd9f9bdf72b0c271193a74c6f61a8c5401af8bb3c478" &&
                    ev.BlockTransactionsNumber == 4)), 
                Times.Once);

            _blockListenerMock.Verify(x=>x.HandleExecutedTransactionAsync(
                    It.Is<Base58String>(p => p.DecodeToString() == "020000000001010000000000000000000000000000000000000000000000000000000000000000ffffffff0603b470030101ffffffff02143a00000000000023210224cc6f0baa45afa6738cdd704f9567979689cd403c2261eaf54b53ddca8d35e1ac0000000000000000266a24aa21a9ed8eedbbce7fbe2789aa24d2f0a19a17d17547be8e85a4b1ae8da3cd13974c2a5e0120000000000000000000000000000000000000000000000000000000000000000000000000"), 
                It.Is<TransferCoinsTransactionExecutedEvent>(p=> p.TransactionNumber == 0 & !p.SpentCoins.Any())), 
                Times.Once);

            //https://private-bcn-explorer-test.lykkex.net/transaction/88a5407783a2ca99b0dad75624aaefdd9c1f7a092ea63d5ec93d48fc4d49c4ed
            _blockListenerMock.Verify(x => x.HandleExecutedTransactionAsync(
                    It.Is<Base58String>(p => p.DecodeToString() == "01000000000101a6c097d359de0fe589bb96b09a3a2f52a0320338a90c3cc4f8da2227a1a94633010000001716001457a4720ac6221fe84ca72b672c62ca9073a9595fffffffff012c320f000000000017a9143ae45b5b44661086fa16ece1d3824a032edad042870247304402204e48a044c4b6a0ae31ccac4cbbe2761d14ae9d0e9873a536034659c5521da62502204a3a578975625d8cf4a778f84d4d67cfe3f16b9c289406eea7a24bf97631b2860121031b4dbdf71fe3ab46b82f15871d04c2891e8d75cba81c2b4ae28c57e52554d9c800000000"), 
                It.Is<TransferCoinsTransactionExecutedEvent>(p=> 
                    p.SpentCoins.Single().TransactionId == "3346a9a12722daf8c43c0ca9380332a0522f3a9ab096bb89e50fde59d397c0a6" &&  
                    p.ReceivedCoins.Single().Address == "2MxccjvVEmW2XDLtZVE4nhMijgJi2APMCgf" &&
                    p.ReceivedCoins.Single().Value.ToDecimal() == 0.00995884m)), 
                Times.Once);

            _blockListenerMock.Verify(x => x.HandleExecutedTransactionAsync(
                    It.Is<Base58String>(p => p.DecodeToString() == "0100000001f4fef6f2bba226d6742abc945c5c658fdce49e8a08b980c52e3a04fa52ea9642010000006a47304402204165755bf17d61bef32bbc5d8a91b4b587115d32c8cfd1446f68ff4819cb1fb302207240e405fbe471af0b6819318d50ae1c9f0174da048a5119ded7730db0ad692801210218724079a342c0385cee4944a1e4434aa4a44606520c107ae772839821f8b4fcffffffff024099eb02000000001976a914a7f48354c754828a6b673db5c8340110daa577f288ac40420f000000000017a914e7c08818e2c00a279e3946777b9a76f5fe44bdb48700000000"), 
                It.Is<TransferCoinsTransactionExecutedEvent>(p => p.TransactionNumber == 2 &&
                                                                  p.TransactionId == "561563fb5321d2805540e388b3f41f0067447833fba4311429588987a348ba54")), 
                Times.Once);

            _blockListenerMock.Verify(x => x.HandleExecutedTransactionAsync(
                It.Is<Base58String>(p => p.DecodeToString() == "0100000001a6c097d359de0fe589bb96b09a3a2f52a0320338a90c3cc4f8da2227a1a94633000000006a47304402201e558fa724ba1b38eb14bbd22995ab34b4e87f6391b82c9cee2fd3f29e39ca4a02200bf77f15ea599a840c64a992535e35976a467e6c8ebfca9f789347d0935ce9a001210218724079a342c0385cee4944a1e4434aa4a44606520c107ae772839821f8b4fcffffffff0293a16301000000001976a914a7f48354c754828a6b673db5c8340110daa577f288ac40420f000000000017a914e7c08818e2c00a279e3946777b9a76f5fe44bdb48700000000"), 
                It.Is<TransferCoinsTransactionExecutedEvent>(p => p.TransactionId == "2e9dc9e1e5ac5b8839018b490de942f1992bb8f3dd90eb93c5ecd69cdafbe6f4")), 
                Times.Once);

            _blockListenerMock.VerifyNoOtherCalls();
        }
    }
}