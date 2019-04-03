using NUnit.Framework;

namespace AddAndBanish.Tests
{
    public class RemoveCardsTest
    {
        [Test]
        public void MaxClearTest()
        {
            var board = new Board(4, new sbyte[] {
                3, 2, 1, 4,
                2, 3, 2, 2,
                2, 2, 2, 4,
                2, 2, 2, 5});
            var rems = new RemoveCards(board);
            for (int i = 0; i < rems.Length; i++)
                Assert.AreEqual(CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, rems.Cards[i]);
        }

        [Test]
        public void AddTest()
        {
            var board = new Board(4, new sbyte[] {
                3, 2, 1, 4,
                2, 3, 2, 2,
                2, 2, 2, 4,
                2, 2, 2, 5});
            var rems = new RemoveCards(board);
            for (int i = 0; i < rems.Length; i++)
                Assert.AreEqual(CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, rems.Cards[i]);
            var rems_00_3 = rems.Add(0, 0, 3);
            Assert.AreEqual(3, rems_00_3.Sum);
            var rems_01_2 = rems.Add(0, 1, 2);
            Assert.AreEqual(2, rems_01_2.Sum);
            var rems_02_1 = rems.Add(0, 2, 1);
            Assert.AreEqual(1, rems_02_1.Sum);
            var rems_00_3_01_2 = rems_00_3.Add(0, 1, 2);
            var rems_01_2_00_3 = rems_01_2.Add(0, 0, 3);
            Assert.AreEqual(5, rems_00_3_01_2.Sum);
            Assert.IsTrue(rems_00_3_01_2.Equals(rems_01_2_00_3));
        }

        [Test]
        public void EnumTest()
        {
            var board = new Board(4, new sbyte[] {
                3, 2, 1, 4,
                2, 3, 2, 2,
                2, 2, 2, 4,
                2, 2, 2, 5});
            var rems = new RemoveCards(board);
            for (int i = 0; i < rems.Length; i++)
                Assert.AreEqual(CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, rems.Cards[i]);
            var rems_12_2 = rems.Add(1, 2, 2);
            {
                var enumerator = rems_12_2.GetNeighborEnumerable(board, 10).GetEnumerator();
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(new Scard(2, 2, 2), enumerator.Current);
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(new Scard(1, 3, 2), enumerator.Current);
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(new Scard(1, 1, 3), enumerator.Current);
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(new Scard(0, 2, 1), enumerator.Current);
                Assert.IsFalse(enumerator.MoveNext());
            }
            {
                var enumerator = rems_12_2.GetNeighborEnumerable(board, 4).GetEnumerator();
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(new Scard(2, 2, 2), enumerator.Current);
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(new Scard(1, 3, 2), enumerator.Current);
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(new Scard(0, 2, 1), enumerator.Current);
                Assert.IsFalse(enumerator.MoveNext());
            }
            var rems_12_2_13_2 = rems_12_2.Add(1, 3, 2);
            {
                var enumerator = rems_12_2_13_2.GetNeighborEnumerable(board, 10).GetEnumerator();
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(new Scard(2, 3, 4), enumerator.Current);
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(new Scard(2, 2, 2), enumerator.Current);
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(new Scard(1, 1, 3), enumerator.Current);
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(new Scard(0, 3, 4), enumerator.Current);
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(new Scard(0, 2, 1), enumerator.Current);
            }
        }

        [Test]
        public void ValidRemoveTest()
        {
            var board = new Board(4, new sbyte[] {
                3, 2, 1, 4,
                2, 3, 2, 2,
                2, 2, 2, 4,
                2, 2, 2, 5});
            var rems = new RemoveCards(board);
            for (int i = 0; i < rems.Length; i++)
                Assert.AreEqual(CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, rems.Cards[i]);
            var rems_00_3_01_2_10_2_11_3 = rems.Add(0, 0, 3).Add(0, 1, 2).Add(1, 0, 2).Add(1, 1, 3);
            Assert.IsTrue(rems_00_3_01_2_10_2_11_3.IsValid(10));
            var newBoard = board.CalcNextStepByRemove(rems_00_3_01_2_10_2_11_3);
            Assert.AreEqual(1, newBoard.Cards[0]);
            Assert.AreEqual(4, newBoard.Cards[1]);
            Assert.AreEqual(CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, newBoard.Cards[2]);
            Assert.AreEqual(CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, newBoard.Cards[3]);
            Assert.AreEqual(2, newBoard.Cards[4]);
            Assert.AreEqual(2, newBoard.Cards[5]);
            Assert.AreEqual(CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, newBoard.Cards[6]);
            Assert.AreEqual(CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, newBoard.Cards[7]);
            Assert.AreEqual(2, newBoard.Cards[8]);
            Assert.AreEqual(2, newBoard.Cards[9]);
            Assert.AreEqual(2, newBoard.Cards[10]);
            Assert.AreEqual(4, newBoard.Cards[11]);
            Assert.AreEqual(2, newBoard.Cards[12]);
            Assert.AreEqual(2, newBoard.Cards[13]);
            Assert.AreEqual(2, newBoard.Cards[14]);
            Assert.AreEqual(5, newBoard.Cards[15]);
        }

        [Test]
        public void InvalidRemoveTest()
        {
            var board = new Board(4, new sbyte[] {
                3, 2, 1, 4,
                2, 3, 2, 2,
                2, 2, 2, 4,
                2, 2, 2, 5});
            var rems = new RemoveCards(board);
            for (int i = 0; i < rems.Length; i++)
                Assert.AreEqual(CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, rems.Cards[i]);
            var rems_00_3 = rems.Add(0, 0, 3);
            Assert.AreEqual(3, rems_00_3.Sum);
            var newBoard = board.CalcNextStepByRemove(rems_00_3);
            Assert.AreEqual(2, newBoard.Cards[0]);
            Assert.AreEqual(1, newBoard.Cards[1]);
            Assert.AreEqual(4, newBoard.Cards[2]);
            Assert.AreEqual(CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, newBoard.Cards[3]);
            Assert.AreEqual(2, newBoard.Cards[4]);
            Assert.AreEqual(3, newBoard.Cards[5]);
            Assert.AreEqual(2, newBoard.Cards[6]);
            Assert.AreEqual(2, newBoard.Cards[7]);
            Assert.AreEqual(2, newBoard.Cards[8]);
            Assert.AreEqual(2, newBoard.Cards[9]);
            Assert.AreEqual(2, newBoard.Cards[10]);
            Assert.AreEqual(4, newBoard.Cards[11]);
            Assert.AreEqual(2, newBoard.Cards[12]);
            Assert.AreEqual(2, newBoard.Cards[13]);
            Assert.AreEqual(2, newBoard.Cards[14]);
            Assert.AreEqual(5, newBoard.Cards[15]);
            Assert.IsFalse(rems_00_3.IsValid(10));
        }
    }
}
