﻿using Lykke.Bil2.Bitcoin.BlocksReader.Services.Helpers;
using NBitcoin;
using NUnit.Framework;

namespace Lykke.Bil2.Bitcoin.BlocksReader.Tests
{
    [TestFixture]
    public class HashExtractorTests
    {
        [TestCase(
            "010000000f362bdc16f2f880097c71fd3296c01b835c8b034e4d2939e8af02000000000065a62f2f6b9102d6eb5eee95be5ec3fcdfa27cf2117deeebefc6be53761d99499423e04c56720e1bb4518a450201000000010000000000000000000000000000000000000000000000000000000000000000ffffffff060456720e1b00ffffffff0100f2052a010000004341046896ecfc449cb8560594eb7f413f199deb9b4e5d947a142e7dc7d2de0b811b8e204833ea2a2fd9d4c7b153a8ca7661d0a0b7fc981df1f42f55d64b26b3da1e9cac000000000100000002f11414b9f94e659d9523fa15c8d29bc0d4b52b0965c431acc3081df1a5118bf9000000008a473044022068b2a587ec9374325c17c79f9102358fdd0574eeb3bebea08affab3e07afd93c0220248ac15de4e2b6391ff12c348ba6cc024448dba20611a40a35f8b67b6f966cd50141046d337916441e20c469623303b47fd0c23326956a183e3274801f1a863e04b7f1dc2bee3388292571955955f8a24445ae35781dd9e39e8472bff623d3375f490effffffffb37c2f3a28b138a8ff4f04037e83f1e5d2af023fee64c5ecd21dbd9f69264d0c000000008a47304402206ccc7e10f33ed2e73c000780e972c479cf0a74d4a5825d74eebb8ec87d31da3202207a945ff0062c9df5d7a9c51483f0d8fc9d0cad1670319c8fbebde2198f8eea2201410450a6524d0f7519571e8fb761ac8285f88c5ad9976454ee2148db5e0d13caa534f6bc56678a328dab8319fd62feabc977e478776c9cf0e705575be61dccdf8383ffffffff0100943577000000001976a914dda4521a9cd99e92323ae4762467dab928b65f4588ac00000000",
            "01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff060456720e1b00ffffffff0100f2052a010000004341046896ecfc449cb8560594eb7f413f199deb9b4e5d947a142e7dc7d2de0b811b8e204833ea2a2fd9d4c7b153a8ca7661d0a0b7fc981df1f42f55d64b26b3da1e9cac00000000",
            ExpectedResult = "d5d27987d2a3dfc724e359870c6644b40e497bdc0589a033220fe15429d88599"
        )]
        [Test]
        [TestCase(
            "0100000020114beba985ecbf1351bf5f4d5495105dc2ca94f5bdbcac921e0a00000000009985d82954e10f2233a08905dc7b490eb444660c8759e324c7dfa3d28779d2d5f34ee04c56720e1b021838e10101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff060456720e1b00ffffffff0100f2052a010000004341046896ecfc449cb8560594eb7f413f199deb9b4e5d947a142e7dc7d2de0b811b8e204833ea2a2fd9d4c7b153a8ca7661d0a0b7fc981df1f42f55d64b26b3da1e9cac00000000",
            "01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff060456720e1b00ffffffff0100f2052a010000004341046896ecfc449cb8560594eb7f413f199deb9b4e5d947a142e7dc7d2de0b811b8e204833ea2a2fd9d4c7b153a8ca7661d0a0b7fc981df1f42f55d64b26b3da1e9cac00000000",
            ExpectedResult = "d5d27987d2a3dfc724e359870c6644b40e497bdc0589a033220fe15429d88599_0"
        )]
        [TestCase(
            "0100000042ba7629c32525ff7c74ca323fdc4c6d5b5c4410901aeb4f04300a000000000068b45f58b674e94eb881cd67b04c2cba07fe5552dbf1d5385637b0d4073dbfe3c89fdf4c56720e1ba67373ee0101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff060456720e1b00ffffffff0100f2052a01000000434104124b212f5416598a92ccec88819105179dcb2550d571842601492718273fe0f2179a9695096bff94cd99dcccdea7cd9bd943bfca8fea649cac963411979a33e9ac00000000",
            "01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff060456720e1b00ffffffff0100f2052a01000000434104124b212f5416598a92ccec88819105179dcb2550d571842601492718273fe0f2179a9695096bff94cd99dcccdea7cd9bd943bfca8fea649cac963411979a33e9ac00000000",
            ExpectedResult = "e3bf3d07d4b0375638d5f1db5255fe07ba2c4cb067cd81b84ee974b6585fb468"
        )]
        [TestCase(
            "01000000f3f32d77f94d5e971e202afa4a633056b2b14dda3f4abf569609040000000000e90f74856c0ed42ab7eea359487079e3f76e4076f91250fab8431f6241f56b2f8380e04c56720e1b1c467e890201000000010000000000000000000000000000000000000000000000000000000000000000ffffffff060456720e1b00ffffffff0100f2052a01000000434104124b212f5416598a92ccec88819105179dcb2550d571842601492718273fe0f2179a9695096bff94cd99dcccdea7cd9bd943bfca8fea649cac963411979a33e9ac000000000100000001e347459d80c56ae0bc0bfdd3dbc252c2019114419ac945deb11954a19746e2ca010000008b48304502204b26cc103a2c92295c217848b8425a8d9612d1d2e98133f5b5a5268a108069ab022100f449acb15a3cfc83657a66cd49e5d8fc6c46b2a52ed899f94f4e4da9ad8ca93a0141046554f9d515c996b209e48ae07bec74b424cab1fc1e658c1e87d0169628a6ae45abcbc1c7fb5f14f0f6e8aa3b3fd386735fc76eef8c447c044abdcafba95e8b7affffffff02c0475269000000001976a9142dd5dd5d96006ff246bfc169b8bfcf6f76935d2088ac4018b5a1000000001976a914d7895813489a508c334e881546e919b1cbd80b0888ac00000000",
            "01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff060456720e1b00ffffffff0100f2052a01000000434104124b212f5416598a92ccec88819105179dcb2550d571842601492718273fe0f2179a9695096bff94cd99dcccdea7cd9bd943bfca8fea649cac963411979a33e9ac00000000",
            ExpectedResult = "e3bf3d07d4b0375638d5f1db5255fe07ba2c4cb067cd81b84ee974b6585fb468_0"
        )]
        public string Test_hash_violations(string rawBlock, string rawTransaction)
        {
            var blockHeader = BlockHeader.Parse(rawBlock, Network.Main);
            var transaction = Transaction.Parse(rawTransaction, Network.Main);

            return transaction.ExtractHash(blockHeader);
        }
    }

    [TestFixture]
    public class AddressExtractorTests
    {
        [TestCase("01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff0704ffff001d0104ffffffff0100f2052a0100000043410496b538e853519c726a2c91e61ec11600ae1390813a627c66fb8be7947be63c52da7589379515d4e0a604f8141781e62294721166bf621e73a82cbf2342c858eeac00000000",
            0, "12c6DSiU4Rq3P4ZxziKxzrL5LmMBrzjrJX", "mainnet")]
        [TestCase("0200000001d97a4c5251a10ac0c0fb2943abb82ee991923b4f72b204a7f05f797fe2067c0a000000006a47304402206fd9c2847a82eb9753c1b426f75b4e379c36e89715af836cf6e6d0e780d93655022002cee7480102a4ccf1cc4a23fdae6a70606f53e1f4a527afa84627776b237996012102cfd0c98eb906bfdb6ea92c0269875097a0c0a4d4bff582735c2daeb3eb501227ffffffff020cf236020000000017a914420deacbb951130095ef959192a819b7fa88ab3687d40042af040000001976a914b039c7919c0177476499bc77ddfa80133bd4c9e288ac00000000",
            0, "37iHCBmqqvEGpn1PMnG89HxHrx7iiB3uoz", "mainnet")]
        [TestCase("01000000028ac984a22c17ce27acd4811feb6b1cdd06d7261c5a98d7dff58196c1260face2000000006f0047304402202bc0f89bcae72a821f2b9fc5b59c7e439c03a7c55f968657ff7f0b26fe9e535302205089810ec9c446a8e0ac6b91fc05219b4a3de4ba19a16a1181e644a8f4014119012551210261afd7a2796e9bb83896089293f11edd2987ebbd67ed2075f5e45b7c4bb7edcb51aeffffffffff95ca47921426fb3a6e5c6d41f7c0a9fc5b1105b504506a87420bc54073ccac000000007100493046022100a90f89fcdf86d4edbc5ba02b26426ad842a5a5cc03f22e652b7656a68e627fce022100a3d50139af2027e8c3ea830f9e4e6253204aa1b8dc65cd5af5ed77aed6fef22f012551210261afd7a2796e9bb83896089293f11edd2987ebbd67ed2075f5e45b7c4bb7edcb51aeffffffff0110270000000000001976a9140549d0007c94150b63c7249cd2c3531c2bf56af388ac00000000",
            0, "1UxoYSYwqtqgzn4Pm5VTMMJumNHwY71fz", "mainnet")]
        [TestCase("010000000104fd15da4b97565f7ba6de05843f080699ca6ac92162b42b05eafb1e5364eca5000000004a00483045022100918a9636e3bd713f8e248d4c672fedf6a9b11135fa9d1e201fe1fb3c35f8910b02206724d5def40ff38363b9123a1c66db61c14ee9a2ad9060ecb565974113a78f8e01ffffffff0127c31c0500000000475121037953dbf08030f67352134992643d033417eaa6fcfb770c038f364ff40d76158821009e9d91f4b4d06659d277be861ad9829d90ec792098542d05af6864342b1cd8f652ae90a90300",
            0, "13MH4zmU4UT4Ct6BhoRFGjigC8gN9a9FNn", "mainnet")] 
        [Test]
        public void Can_Extract_Address_From_RawTx(string rawTx,int outputNum,  string address, string network)
        {
            var tx = Transaction.Parse(rawTx, Network.GetNetwork(network));
            var extractAddress = tx.Outputs[outputNum].ScriptPubKey.ExtractAddress(Network.GetNetwork(network));

            Assert.AreEqual(address, extractAddress.ToString());
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
