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
            
            var password = new byte[512];
            _randomizer.NextBytes(password);

            var encrypted = Encryptor.Encrypt(buffer, password);
            Assert.IsFalse(Helpers.SequenceEquals(buffer, encrypted));

            var decrypted = Encryptor.Encrypt(encrypted, password);
            Assert.IsTrue(Helpers.SequenceEquals(buffer, decrypted));
        }
    }
}