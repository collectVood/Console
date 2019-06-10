using System.Data.Linq;
using Console.Security;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Tests
{
    [TestFixture]
    public class Encryption
    {
        private Randomizer _randomizer;
        
        [SetUp]
        public void Setup()
        {
            _randomizer = Randomizer.CreateRandomizer();
        }
        
        [Test]
        [Repeat(10)]
        public void EncryptTest()
        {
            var buffer = new byte[8192];
            _randomizer.NextBytes(buffer);
            
            var bufferBinary = new Binary(buffer);
            
            var password = new byte[512];
            _randomizer.NextBytes(password);

            var encrypted = Encryptor.Encrypt(buffer, password);
            var encryptedBinary = new Binary(encrypted);
            Assert.IsFalse(bufferBinary.Equals(encryptedBinary));

            var decrypted = Encryptor.Encrypt(encrypted, password);
            var decryptedBinary = new Binary(decrypted);
            Assert.IsTrue(bufferBinary.Equals(decryptedBinary));
        }
    }
}