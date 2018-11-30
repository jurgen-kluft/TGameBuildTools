using System.Security.Cryptography;

namespace SkeinFish
{
    public class Threefish : SymmetricAlgorithm
    {
        const int DefaultCipherSize = 256;

        public Threefish()
        {
            // Set up supported key and block sizes for Threefish
            KeySizes[] supportedSizes = 
            {
                new KeySizes(256, 512, 256),
                new KeySizes(1024, 1024, 0)
            };

            base.LegalBlockSizesValue = supportedSizes;
            base.LegalKeySizesValue   = supportedSizes;

            // Set up default sizes
            base.KeySizeValue   = DefaultCipherSize;
            base.BlockSizeValue = DefaultCipherSize;

            // ECB is the default for the other ciphers in
            // the standard library I think
            base.ModeValue = CipherMode.ECB;
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new ThreefishTransform(rgbKey, rgbIV, ThreefishTransformType.Decrypt, ModeValue, PaddingValue);
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new ThreefishTransform(rgbKey, rgbIV, ThreefishTransformType.Encrypt, ModeValue, PaddingValue);
        }

        public override void GenerateIV()
        {
            base.IVValue = GenerateRandomBytes(base.BlockSizeValue / 8);
        }

        public override void GenerateKey()
        {
            base.KeyValue = GenerateRandomBytes(base.KeySizeValue / 8);
        }

        static byte[] GenerateRandomBytes(int amount)
        {
            var rngCrypto = new RNGCryptoServiceProvider();

            var bytes = new byte[amount];
            rngCrypto.GetBytes(bytes);

            return bytes;
        }
    }
}
