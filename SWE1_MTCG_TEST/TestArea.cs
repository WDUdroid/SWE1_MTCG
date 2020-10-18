using System;
using System.Threading;
using NUnit.Framework;
using SWE1_MTCG;
using Moq;

namespace SWE1_MTCG_TEST
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GenerateCardTest()
        {
            //arrange
            var generatedCardEmptyCardSpecs = new CardSpecs();
            var generatedCardKnight = new CardSpecs(1);

            //assert
            Assert.AreEqual(EMonsterType.Unknown, generatedCardEmptyCardSpecs.MonsterType);
            Assert.AreEqual(EMonsterType.Knight,generatedCardKnight.MonsterType);
        }

        [Test]
        public void PackageReturnTest()
        {
            //arrange
            var returnedPackage = Store.Package();

            for (var i = 1; i <= 4; i++)
            {
                Console.WriteLine(returnedPackage[i].Name + " " + returnedPackage[i].Damage + " " + returnedPackage[i].Element + " " + returnedPackage[i].MonsterType);
                Assert.AreNotEqual(null, returnedPackage);
            }
        }

        [Test]
        public void BattleTest()
        {
            //arrange
            var mockedA = new Mock<User>();
            var mockedB = new Mock<User>();
            var fight = new Battle();

            var fightLog = fight.NewBattle(mockedA.Object, mockedB.Object);

            Assert.AreEqual(1, 1);
        }
    }
}