using NUnit.Framework;

namespace AddAndBanish.Tests
{
    public class BoardTest
    {
        [Test]
        public void SByteMaxClearTest()
        {
            Board board = new Board(4, 2);
            for (int i = 0; i < board.Cards.Length; i++)
            {
                Assert.AreEqual(CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, board.Cards[i]);
            }
            board = new Board(2, new sbyte[] { 2, 2, 2, 4 });
            Assert.IsTrue(board.IsMultipleOfArgument(10));
        }

        [Test]
        public void ShrinkTest()
        {
            var builder = new BoardBuilder_MUST_BE_DISPOSED();
            try
            {
                var board = new Board(4, new sbyte[] {
                3, 2, 1, 4,
                2, 3, 2, 2,
                2, 2, 2, 4,
                2, 2, 2, 5});
                Assert.IsTrue(board.IsMultipleOfArgument(10));
                board = builder.FromBoard(board).Shrink(2, 2).ToBoard();
                Assert.IsTrue(board.IsMultipleOfArgument(10));
                Assert.AreEqual(4, board.Length);
                Assert.AreEqual(3, board[0, 0]);
                Assert.AreEqual(2, board[0, 1]);
                Assert.AreEqual(2, board[1, 0]);
                Assert.AreEqual(3, board[1, 1]);
            }
            finally
            {
                builder.Dispose();
            }
        }

        [Test]
        public void GetHighestTest()
        {
            var board = new Board(4, new sbyte[] {
                3, 2, 2, CalcIndexHelper.NOT_REMOVE_CARD_NUMBER,
                2, 3, 2, CalcIndexHelper.NOT_REMOVE_CARD_NUMBER,
                2, 2, 2, CalcIndexHelper.NOT_REMOVE_CARD_NUMBER,
                CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, CalcIndexHelper.NOT_REMOVE_CARD_NUMBER});
            Assert.IsTrue(board.IsMultipleOfArgument(10));
            var highest = board.GetHeighestColumnHeight(board.Width);
            Assert.AreEqual(3, highest);
        }

        [Test]
        public void RemoveEmptyColumnTest()
        {
            var board = new Board(4, new sbyte[] {
                3, 2, 1, 4,
                2, 3, 4, 1,
                CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, CalcIndexHelper.NOT_REMOVE_CARD_NUMBER, CalcIndexHelper.NOT_REMOVE_CARD_NUMBER,
                2, 2, 2, 4,});
            Assert.IsTrue(board.IsMultipleOfArgument(10));
            Assert.IsTrue(board.IsEmptyColumn(2));
            var builder = new BoardBuilder_MUST_BE_DISPOSED();
            try
            {
                builder.FromBoard(ref board);
                builder.RemoveColumn(2);
                board = builder.ToBoard();
                Assert.AreEqual(3, board.Width);
                Assert.AreEqual(2, board[2, 0]);
                Assert.AreEqual(2, board[2, 1]);
                Assert.AreEqual(2, board[2, 2]);
                Assert.AreEqual(4, board[2, 3]);
            }
            finally { builder.Dispose(); }
        }
    }
}
