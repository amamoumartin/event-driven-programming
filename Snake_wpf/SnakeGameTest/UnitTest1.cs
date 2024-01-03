

using SnakeModel.Model;
using SnakeModel.Persistence;

namespace SnakeGame.SnakeGameTest
{
    [TestClass]
    public class UnitTest1
    {
        private SnakeGameModel? testModel;
        [TestInitialize]
        public void Initialize()
        {
            testModel = new SnakeGameModel(new DataAccess());
        }

        [TestMethod]
        public void TenBoardTestMethod()
        {
            testModel!.StartNewGame(10);
            Assert.IsTrue(testModel.Table[9, 0] == Field.Empty);
            Assert.IsTrue(testModel.Table[0, 1] == Field.Empty);
            Assert.IsTrue(testModel.Table[0, 2] == Field.Empty);

            Assert.IsTrue(testModel.Table[0, 0] == Field.Wall);
            Assert.IsTrue(testModel.Table[1, 2] == Field.Wall);
            Assert.IsTrue(testModel.Table[6, 1] == Field.Wall);

            Assert.IsTrue(testModel.Table[5, 5] == Field.Head);
            Assert.IsTrue(testModel.Table[6, 5] == Field.Body);
            Assert.IsTrue(testModel.Table[7, 5] == Field.Body);

            Assert.IsTrue(testModel.Table[1, 5] == Field.Egg);
        }

        [TestMethod]
        public void FifteenBoardTestMethod()
        {
            testModel!.StartNewGame(15);
            Assert.IsTrue(testModel.Table[0, 0] == Field.Empty);
            Assert.IsTrue(testModel.Table[0, 10] == Field.Empty);
            Assert.IsTrue(testModel.Table[1, 8] == Field.Empty);

            Assert.IsTrue(testModel.Table[2, 0] == Field.Wall);
            Assert.IsTrue(testModel.Table[11, 0] == Field.Wall);
            Assert.IsTrue(testModel.Table[10, 1] == Field.Wall);

            Assert.IsTrue(testModel.Table[7, 7] == Field.Head);

            Assert.IsTrue(testModel.Table[8, 7] == Field.Body);
            Assert.IsTrue(testModel.Table[9, 7] == Field.Body);

            Assert.IsTrue(testModel.Table[4, 7] == Field.Egg);
        }
        [TestMethod]
        public void TwentyBoardTestMethod()
        {
            testModel!.StartNewGame(20);
            Assert.IsTrue(testModel.Table[0, 0] == Field.Empty);
            Assert.IsTrue(testModel.Table[0, 10] == Field.Empty);
            Assert.IsTrue(testModel.Table[1, 8] == Field.Empty);

            Assert.IsTrue(testModel.Table[0, 1] == Field.Wall);
            Assert.IsTrue(testModel.Table[6, 1] == Field.Wall);
            Assert.IsTrue(testModel.Table[19, 11] == Field.Wall);

            Assert.IsTrue(testModel.Table[10, 10] == Field.Head);

            Assert.IsTrue(testModel.Table[11, 10] == Field.Body);
            Assert.IsTrue(testModel.Table[12, 10] == Field.Body);

            Assert.IsTrue(testModel.Table[5, 10] == Field.Egg);

        }
        [TestMethod]
        public void EatTest()
        {
            testModel!.StartNewGame(15);
            testModel.State = true;
            testModel.Move();
            testModel.Move();
            testModel.Move();
            testModel.Move();
            Assert.AreNotEqual(5, testModel!.Snake!.Count);
            Assert.AreNotEqual(0, testModel.Score);

        }
        [TestMethod]
        public void WallCollisionTest()
        {
            testModel!.StartNewGame(15);
            testModel.State = true;
            testModel.Move();
            testModel.SetDirection(Direction.Up);
            testModel.Move();

            Assert.IsFalse(testModel.Alive);
        }
        [TestMethod]
        public void BorderCollisionTest()
        {
            testModel!.StartNewGame(15);
            testModel.State = true;
            for (int i = 0; i < 8; i++)
            {
                testModel.Move();
            }
            Assert.IsFalse(testModel.Alive);
        }

        [TestMethod]
        public void SnakeCollisionTest()
        {
            testModel!.StartNewGame(15);
            testModel.State = true;
            testModel.Move();
            testModel.SetDirection(Direction.Up);
            testModel.Move();
            testModel.Move();
            testModel.Move();
            testModel.SetDirection(Direction.Up);
            testModel.Move();
            testModel.SetDirection(Direction.Up);
            testModel.Move();
            testModel.SetDirection(Direction.Up);
            testModel.Move();

            Assert.IsFalse(testModel.Alive);
        }


        private void TestModel_GameAdvanced(object? sender, GameEventArgs e)
        {
            Assert.IsTrue(e.score >= 0);
        }

        private void TestModel_GameOver(object? sender, GameEventArgs e)
        {
            Assert.IsTrue(e.text != "");
        }
    }
}