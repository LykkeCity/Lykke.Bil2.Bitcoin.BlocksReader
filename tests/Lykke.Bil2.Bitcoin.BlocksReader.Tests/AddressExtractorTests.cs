﻿using Lykke.Bil2.Bitcoin.BlocksReader.Services.Helpers;
using NBitcoin;
using NUnit.Framework;

namespace Lykke.Bil2.Bitcoin.BlocksReader.Tests
{
    [TestFixture]
    public class AddressExtractorTests
    {
        [TestCase("01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff0704ffff001d0104ffffffff0100f2052a0100000043410496b538e853519c726a2c91e61ec11600ae1390813a627c66fb8be7947be63c52da7589379515d4e0a604f8141781e62294721166bf621e73a82cbf2342c858eeac00000000",
            0, "12c6DSiU4Rq3P4ZxziKxzrL5LmMBrzjrJX", "mainnet")]
        [TestCase("0200000001d97a4c5251a10ac0c0fb2943abb82ee991923b4f72b204a7f05f797fe2067c0a000000006a47304402206fd9c2847a82eb9753c1b426f75b4e379c36e89715af836cf6e6d0e780d93655022002cee7480102a4ccf1cc4a23fdae6a70606f53e1f4a527afa84627776b237996012102cfd0c98eb906bfdb6ea92c0269875097a0c0a4d4bff582735c2daeb3eb501227ffffffff020cf236020000000017a914420deacbb951130095ef959192a819b7fa88ab3687d40042af040000001976a914b039c7919c0177476499bc77ddfa80133bd4c9e288ac00000000",
            0, "37iHCBmqqvEGpn1PMnG89HxHrx7iiB3uoz", "mainnet")]
        [TestCase("01000000028ac984a22c17ce27acd4811feb6b1cdd06d7261c5a98d7dff58196c1260face2000000006f0047304402202bc0f89bcae72a821f2b9fc5b59c7e439c03a7c55f968657ff7f0b26fe9e535302205089810ec9c446a8e0ac6b91fc05219b4a3de4ba19a16a1181e644a8f4014119012551210261afd7a2796e9bb83896089293f11edd2987ebbd67ed2075f5e45b7c4bb7edcb51aeffffffffff95ca47921426fb3a6e5c6d41f7c0a9fc5b1105b504506a87420bc54073ccac000000007100493046022100a90f89fcdf86d4edbc5ba02b26426ad842a5a5cc03f22e652b7656a68e627fce022100a3d50139af2027e8c3ea830f9e4e6253204aa1b8dc65cd5af5ed77aed6fef22f012551210261afd7a2796e9bb83896089293f11edd2987ebbd67ed2075f5e45b7c4bb7edcb51aeffffffff0110270000000000001976a9140549d0007c94150b63c7249cd2c3531c2bf56af388ac00000000",
            0, "1UxoYSYwqtqgzn4Pm5VTMMJumNHwY71fz", "mainnet")] // multisig
        [Test]
        public void Can_Extract_Address_From_RawTx(string rawTx,int outputNum,  string address, string network)
        {
            var tx = Transaction.Parse(rawTx, Network.GetNetwork(network));
            var addr = tx.Outputs[outputNum].ScriptPubKey.ExtractAddress(Network.GetNetwork(network));

            Assert.AreEqual(addr, address);
        }

        [TestCase("0100000001d0e67013c4c8512cb7e4cfbb64bcea38d854cab6db36b6af0113109ce489ee7201000000fdfd000048304502210096df80136e578ce721589d61cb2efcf5e4748c6a3a6ab7e34d0fc12e3e748e2c02201049e22331794a5d99856105b096e3adb60b053562ad40ef6fe28b8bb70ca8f40147304402203f49e5198e7b14aeb59c26dc42a4207ede9d0d4a291e2ae6f2eb1809fbdba21a022030e74e63e752296499e18f772e00d26c206972029f3b8c0b7d50298a1734241f014c69522103459d20315debcb8b4c47c5f0ff356c7764ea3b103487487a1ed2bbcac3f18bc221023b0fd344dbd13d25663adc5a31d269ceac90b6dfc3ac8af8d5b31aa10ba366fc21032233fc2b5916568cd5177e9b88feda049195418cbadb2c6741e8df8967ec84ab53aeffffffff030000000000000000106a0e69643a64616e6f6d6172722e69647c150000000000001976a9146ada8b2f3ce136abedd949e749ccf5574d867d5b88ac557d0c000000000017a9148e1719fb937c598ddd0760118b5455fc4f31891b8700000000",
            0, "mainnet")]
        [Test]
        public void Can_Detect_Unrecognized_Address(string rawtx, int outputNum,  string network)
        {
            var tx = Transaction.Parse(rawtx, Network.GetNetwork(network));

            Assert.True(tx.Outputs[outputNum].ScriptPubKey.IsUnrecognizedAddress());
        }
    }
}
