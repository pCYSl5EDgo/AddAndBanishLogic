using System.Text;

namespace AddAndBanish
{
    public static class BoardStringBuilderHelper
    {
        public static StringBuilder AppendBoard_GameView(this StringBuilder buffer, in Board board)
        {
            for (int y = board.Height; --y >= 0;)
            {
                for (int x = 0, end = board.Width; x < end; x++)
                    buffer.AppendCard(board[x, y]);
                buffer.Append('\n');
            }
            return buffer;
        }
        public static StringBuilder AppendBoard_GameView<T>(this StringBuilder buffer, ref T board)
            where T : struct, IBoard
        {
            for (int y = board.Height; --y >= 0;)
            {
                for (int x = 0, end = board.Width; x < end; x++)
                    buffer.AppendCard(board[x, y]);
                buffer.Append('\n');
            }
            return buffer;
        }

        public static StringBuilder AppendBoard_ArraySequence<T>(this StringBuilder buffer, ref T board)
            where T : struct, IBoard
        {
            for (int x = 0, end = board.Width; x < end; x++)
            {
                for (int y = 0; y < board.Height; y++)
                    buffer.AppendCard(board[x, y]);
                buffer.Append('\n');
            }
            return buffer;
        }

        public static StringBuilder AppendCard(this StringBuilder buffer, sbyte card)
        {
            if (card == CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                buffer.Append("   ,");
            else buffer.Append($"{card,3}").Append(',');
            return buffer;
        }
    }
}