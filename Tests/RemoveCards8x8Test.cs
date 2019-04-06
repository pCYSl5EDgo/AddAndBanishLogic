using NUnit.Framework;
using AddAndBanish.LowLevel.Unsafe;

namespace AddAndBanish.Tests
{
    public class RemoveCards8x8Test
    {
        [Test]
        public void MaxClearTest()
        {
            var board = new Board8x8(4, new sbyte[] {
                3, 2, 1, 4,
                2, 3, 2, 2,
                2, 2, 2, 4,
                2, 2, 2, 5});
            var builder = new BoardBuilder_MUST_BE_DISPOSED();
            builder.FromBoard(ref board);
            var rems = new RemoveCards8x8();
            for (int x = 0; x < rems.Width; x++)
                for (int y = 0; y < rems.Height; y++)
                    Assert.IsFalse(rems[x, y]);
        }

        [Test]
        public void AddTest()
        {
            var board = new Board8x8(4, new sbyte[] {
                3, 2, 1, 4,
                2, 3, 2, 2,
                2, 2, 2, 4,
                2, 2, 2, 5});
            var rems = new RemoveCards8x8();
            for (int x = 0; x < rems.Width; x++)
                for (int y = 0; y < rems.Height; y++)
                    Assert.IsFalse(rems[x, y]);
            var rems_00_3 = rems.Add(0, 0, 3);
            Assert.AreEqual(3, rems_00_3.Sum);
            Assert.IsTrue(rems_00_3[0, 0]);
            var rems_01_2 = rems.Add(0, 1, 2);
            Assert.AreEqual(2, rems_01_2.Sum);
            Assert.IsTrue(rems_01_2[0, 1]);
            var rems_02_1 = rems.Add(0, 2, 1);
            Assert.AreEqual(1, rems_02_1.Sum);
            Assert.IsTrue(rems_02_1[0, 2]);
            var rems_00_3_01_2 = rems_00_3.Add(0, 1, 2);
            var rems_01_2_00_3 = rems_01_2.Add(0, 0, 3);
            Assert.AreEqual(5, rems_00_3_01_2.Sum);
            Assert.IsTrue(rems_00_3_01_2[0, 0]);
            Assert.IsTrue(rems_00_3_01_2[0, 1]);
            Assert.IsTrue(rems_01_2_00_3[0, 1]);
            Assert.IsTrue(rems_01_2_00_3[0, 0]);
            Assert.IsTrue(rems_00_3_01_2.Equals(rems_01_2_00_3));
        }

        [Test]
        public void EnumTest()
        {
            var board = new Board8x8(4, new sbyte[] {
                3, 2, 1, 4,
                2, 3, 2, 2,
                2, 2, 2, 4,
                2, 2, 2, 5});
            var rems = new RemoveCards8x8();
            for (int x = 0; x < rems.Width; x++)
                for (int y = 0; y < rems.Height; y++)
                    Assert.IsFalse(rems[x, y]);
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
            var board = new Board8x8(4, new sbyte[] {
                3, 2, 1, 4,
                2, 3, 2, 2,
                2, 2, 2, 4,
                2, 2, 2, 5});
            var rems = new RemoveCards8x8();
            for (int x = 0; x < rems.Width; x++)
                for (int y = 0; y < rems.Height; y++)
                    Assert.IsFalse(rems[x, y]);
            var rems_00_3_01_2_10_2_11_3 = rems.Add(0, 0, 3).Add(0, 1, 2).Add(1, 0, 2).Add(1, 1, 3);
            Assert.IsTrue(rems_00_3_01_2_10_2_11_3.IsValid(10));
            var newBoard = rems_00_3_01_2_10_2_11_3.CalcNextStepByRemove(ref board);
            Assert.AreEqual(1, newBoard[0, 0]);
            Assert.AreEqual(4, newBoard[0, 1]);
            Assert.AreEqual(CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, newBoard[0, 2]);
            Assert.AreEqual(CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, newBoard[0, 3]);
            Assert.AreEqual(2, newBoard[1, 0]);
            Assert.AreEqual(2, newBoard[1, 1]);
            Assert.AreEqual(CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, newBoard[1, 2]);
            Assert.AreEqual(CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, newBoard[1, 3]);
            Assert.AreEqual(2, newBoard[2, 0]);
            Assert.AreEqual(2, newBoard[2, 1]);
            Assert.AreEqual(2, newBoard[2, 2]);
            Assert.AreEqual(4, newBoard[2, 3]);
            Assert.AreEqual(2, newBoard[3, 0]);
            Assert.AreEqual(2, newBoard[3, 1]);
            Assert.AreEqual(2, newBoard[3, 2]);
            Assert.AreEqual(5, newBoard[3, 3]);
        }

        [Test]
        public void InvalidRemoveTest()
        {
            var board = new Board8x8(4, new sbyte[] {
                3, 2, 1, 4,
                2, 3, 2, 2,
                2, 2, 2, 4,
                2, 2, 2, 5});
            var rems = new RemoveCards8x8();
            for (int x = 0; x < rems.Width; x++)
                for (int y = 0; y < rems.Height; y++)
                    Assert.IsFalse(rems[x, y]);
            var rems_00_3 = rems.Add(0, 0, 3);
            Assert.AreEqual(3, rems_00_3.Sum);
            var newBoard = rems_00_3.CalcNextStepByRemove(ref board);
            Assert.AreEqual(2, newBoard[0, 0]);
            Assert.AreEqual(1, newBoard[0, 1]);
            Assert.AreEqual(4, newBoard[0, 2]);
            Assert.AreEqual(CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, newBoard[0, 3]);
            Assert.AreEqual(2, newBoard[1, 0]);
            Assert.AreEqual(3, newBoard[1, 1]);
            Assert.AreEqual(2, newBoard[1, 2]);
            Assert.AreEqual(2, newBoard[1, 3]);
            Assert.AreEqual(2, newBoard[2, 0]);
            Assert.AreEqual(2, newBoard[2, 1]);
            Assert.AreEqual(2, newBoard[2, 2]);
            Assert.AreEqual(4, newBoard[2, 3]);
            Assert.AreEqual(2, newBoard[3, 0]);
            Assert.AreEqual(2, newBoard[3, 1]);
            Assert.AreEqual(2, newBoard[3, 2]);
            Assert.AreEqual(5, newBoard[3, 3]);
            Assert.IsFalse(rems_00_3.IsValid(10));
        }

        [Test]
        public void ConnectionTest()
        {
            var board = new Board8x8(4, new sbyte[] {
                3, 2, 1, 4,
                2, 3, 2, 2,
                2, 2, 2, 4,
                2, 2, 2, 5});
            var rems = new RemoveCards8x8();
            for (int x = 0; x < rems.Width; x++)
                for (int y = 0; y < rems.Height; y++)
                    Assert.IsFalse(rems[x, y]);
            var rems_00_3_01_2_10_2_11_3 = rems.Add(0, 0, 3).Add(0, 1, 2).Add(1, 0, 2).Add(1, 1, 3);
            Assert.IsTrue(rems_00_3_01_2_10_2_11_3.IsValid(10));
            Assert.AreEqual(2, rems_00_3_01_2_10_2_11_3.GetConnection(0, 0));
            Assert.AreEqual(2, rems_00_3_01_2_10_2_11_3.GetConnection(0, 1));
            Assert.AreEqual(1, rems_00_3_01_2_10_2_11_3.GetConnection(0, 2));
            Assert.AreEqual(0, rems_00_3_01_2_10_2_11_3.GetConnection(0, 3));
            Assert.AreEqual(2, rems_00_3_01_2_10_2_11_3.GetConnection(1, 0));
            Assert.AreEqual(2, rems_00_3_01_2_10_2_11_3.GetConnection(1, 1));
            Assert.AreEqual(1, rems_00_3_01_2_10_2_11_3.GetConnection(1, 2));
            Assert.AreEqual(0, rems_00_3_01_2_10_2_11_3.GetConnection(1, 3));
            Assert.AreEqual(1, rems_00_3_01_2_10_2_11_3.GetConnection(2, 0));
            Assert.AreEqual(1, rems_00_3_01_2_10_2_11_3.GetConnection(2, 1));
            Assert.AreEqual(0, rems_00_3_01_2_10_2_11_3.GetConnection(2, 2));
            Assert.AreEqual(0, rems_00_3_01_2_10_2_11_3.GetConnection(2, 3));
            Assert.AreEqual(0, rems_00_3_01_2_10_2_11_3.GetConnection(3, 0));
            Assert.AreEqual(0, rems_00_3_01_2_10_2_11_3.GetConnection(3, 1));
            Assert.AreEqual(0, rems_00_3_01_2_10_2_11_3.GetConnection(3, 2));
            Assert.AreEqual(0, rems_00_3_01_2_10_2_11_3.GetConnection(3, 3));
        }

        [Test]
        public void EnumerableTest()
        {
            
        }
    }
}
