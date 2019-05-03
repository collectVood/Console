using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Tests
{
    [TestFixture]
    public class DataFileSystem
    {
        private Console.Files.DataFileSystem _dataFileSystem;

        private Randomizer _randomizer;
        
        [SetUp]
        public void Setup()
        {
            _dataFileSystem = new Console.Files.DataFileSystem(Directory.GetCurrentDirectory());
            _randomizer = Randomizer.CreateRandomizer();
        }
        
        [Test]
        public void GetPathTest()
        {
            var examples = new Dictionary<string, bool> // PATH - EXPECTED RESULT
            {
                {"C:", true},
                {"MyFolder", false},
                {"MyFile.json", false},
                {"MyFolder/MyFolder", false},
                {"MyFolder/MyFile.data", false}
            };

            foreach (var kvp in examples)
            {
                var result = _dataFileSystem.GetPath(kvp.Key);
                Assert.IsTrue(kvp.Value == (result == kvp.Key), $"{kvp.Key} -> {result}");
            }
        }

        [Test]
        public void ReadWriteBinaryTest()
        {
            var file = _randomizer.GetString(10) + ".data";
            var content = _randomizer.GetString(100);

            _dataFileSystem.Write(content, file);
            var result = _dataFileSystem.Read<string>(file);
            
            Assert.IsTrue(result == content);
        }

        [Test]
        public void ReadWriteJsonTest()
        {
            var file = _randomizer.GetString(10) + ".json";
            var content = _randomizer.GetString(100);

            _dataFileSystem.Write(content, file);
            var result = _dataFileSystem.Read<string>(file);
            
            Assert.IsTrue(result == content);
        }

        [Test]
        public void ExistsTest()
        {
            var file = _randomizer.GetString(10) + ".data";
            Assert.IsFalse(_dataFileSystem.Exists(file));
            Assert.IsTrue(_dataFileSystem.Exists(file, true));
            Assert.IsTrue(_dataFileSystem.Exists(file));
        }
    }
}