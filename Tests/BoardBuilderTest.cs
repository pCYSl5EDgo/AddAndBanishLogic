using System.Collections.Generic;
using NUnit.Framework;

namespace AddAndBanish.Tests
{
    public class BoardBuilderTest
    {
        IReadOnlyList<IReadOnlyList<Scard>> cards;
        [SetUp]
        public void SetUp()
        {
            cards = new List<List<Scard>>(new[]{
                new List<Scard>(){
                    new Scard(0, 0, 1),
                    new Scard(0, 1, 9),
                },
                new List<Scard>(){
                    new Scard(1, 0, 7),
                    new Scard(1, 1, 3),
                },
            });
        }
        [Test]
        public void AddTest()
        {
            var builder = new BoardBuilder_MUST_BE_DISPOSED();
            var board = builder.AddOrSet(new Scard(0, 0, 1)).ToBoard();
            Assert.AreEqual(1, board.Length);
            Assert.AreEqual(1, board.Width);
            Assert.AreEqual(1, board.Height);
            Assert.AreEqual(1, board.Cards[0]);
            builder.Dispose();
        }

        [Test]
        public void AddRangeTest()
        {
            var builder = new BoardBuilder_MUST_BE_DISPOSED();
            builder.AddOrSetRange(cards);
            var board = builder.ToBoard();
            Assert.AreEqual(4, board.Length);
            Assert.AreEqual(2, board.Width);
            Assert.AreEqual(2, board.Height);
            Assert.AreEqual(1, board.Cards[0]);
            Assert.AreEqual(9, board.Cards[1]);
            Assert.AreEqual(7, board.Cards[2]);
            Assert.AreEqual(3, board.Cards[3]);
            builder.Dispose();
        }

        [Test]
        public void RemoveColumnTest()
        {
            var builder = new BoardBuilder_MUST_BE_DISPOSED();
            builder.AddOrSetRange(cards).RemoveColumn(3);
            var board = builder.ToBoard();
            Assert.AreEqual(4, board.Length);
            Assert.AreEqual(2, board.Width);
            Assert.AreEqual(2, board.Height);
            Assert.AreEqual(1, board.Cards[0]);
            Assert.AreEqual(9, board.Cards[1]);
            Assert.AreEqual(7, board.Cards[2]);
            Assert.AreEqual(3, board.Cards[3]);
            board = builder.Clear().AddOrSetRange(cards).RemoveColumn(1).ToBoard();
            Assert.AreEqual(2, board.Length);
            Assert.AreEqual(1, board.Width);
            Assert.AreEqual(2, board.Height);
            Assert.AreEqual(1, board.Cards[0]);
            Assert.AreEqual(9, board.Cards[1]);
            builder.Dispose();
        }

        [Test]
        public void GetMaxHeightTest()
        {
            var board = new Board(7, new sbyte[]{
                1,1,1,1,1,1,1,
                2,2,2,2,2,CalcIndexHelper.NOT_REMOVE_CARD_NUMBER,CalcIndexHelper.NOT_REMOVE_CARD_NUMBER,
                3,CalcIndexHelper.NOT_REMOVE_CARD_NUMBER,CalcIndexHelper.NOT_REMOVE_CARD_NUMBER,CalcIndexHelper.NOT_REMOVE_CARD_NUMBER,CalcIndexHelper.NOT_REMOVE_CARD_NUMBER,CalcIndexHelper.NOT_REMOVE_CARD_NUMBER,CalcIndexHelper.NOT_REMOVE_CARD_NUMBER,
                4,4,4,4,4,CalcIndexHelper.NOT_REMOVE_CARD_NUMBER,CalcIndexHelper.NOT_REMOVE_CARD_NUMBER,
            });
            var builder = new BoardBuilder_MUST_BE_DISPOSED();
            builder.FromBoard(board);
            Assert.AreEqual(7, builder.GetHeight(0));
            Assert.AreEqual(5, builder.GetHeight(1));
            Assert.AreEqual(1, builder.GetHeight(2));
            Assert.AreEqual(5, builder.GetHeight(3));
            builder.RemoveColumn(0).RemoveColumn(1);
            Assert.AreEqual(5, builder.GetMaxHeight());
            board = builder.Shrink().ToBoard();
            Assert.AreEqual(10, board.Length);
            Assert.AreEqual(2, board.Width);
            Assert.AreEqual(5, board.Height);
        }

        [Test]
        public void ClearTest()
        {
            var builder = new BoardBuilder_MUST_BE_DISPOSED();
            Assert.AreEqual(0, builder.Length);
            Assert.AreEqual(0, builder.Width);
            Assert.AreEqual(0, builder.Height);
            builder.AddOrSetRange(cards).Clear();
            Assert.AreEqual(0, builder.Length);
            Assert.AreEqual(0, builder.Width);
            Assert.AreEqual(0, builder.Height);
            builder.Dispose();
        }
    }
}
