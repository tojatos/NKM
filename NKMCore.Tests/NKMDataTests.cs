using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mono.Data.Sqlite;
using Xunit;

namespace NKMCore.Tests
{
    public class NKMDataTests
    {
        public NKMDataTests()
        {
            string assemblyDirPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            string dbPath = Path.Combine(assemblyDirPath, "NKMData", "database.db");
            NKMData.Connection = new SqliteConnection($"Data source={dbPath}");
        }

        [Fact]
        public void CharacterNamesGotCorrectly()
        {
            List<string> characterNames = NKMData.GetCharacterNames();
            Assert.NotNull(characterNames);
            Assert.NotEmpty(characterNames);
        }

        [Fact]
        public void AbilityClassNamesGotCorrectly()
        {
            List<string> characterNames = NKMData.GetCharacterNames();
            characterNames.ForEach(c =>
            {
                IEnumerable<string> abilityClassNames = NKMData.GetAbilityClassNames(c);
                Assert.NotNull(abilityClassNames);
                Assert.NotEmpty(abilityClassNames);
            });
        }
    }
}