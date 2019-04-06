using Unity.Collections.LowLevel.Unsafe;

namespace AddAndBanish
{
    internal unsafe static class BoardHelper
    {
        internal static bool IsEmptyColumn(this in Board board, int x) => board.Cards.IsEmptyColumn(x, board.Height);
        internal static bool IsEmptyColumn<T>(this ref T board, int x) where T : struct, IBoard => board.GetHeight(x) == 0;
        internal static bool IsEmptyColumn(this sbyte[] cards, int x, int height)
        {
            // その列について
            for (int index = x * height, end = (x + 1) * height; index < end; index++)
                // 1手進めた盤面のその列のあるマスが空欄であるか調べる
                if (cards[index] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                    return false;
            return true;
        }
        internal static bool IsEmptyColumn(sbyte* cards, int x, int height)
        {
            // その列について
            for (int index = x * height, end = (x + 1) * height; index < end; index++)
                // 1手進めた盤面のその列のあるマスが空欄であるか調べる
                if (cards[index] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                    return false;
            return true;
        }

        internal static int GetHeighestColumnHeight(sbyte* cards, int width, int height)
        {
            if (width == 0) return 0;
            int maxHeight = CalcHeight(cards, height, --width);
            for (; --width >= 0;)
            {
                var currentColumnHeight = CalcHeight(cards, height, width);
                // Math.Max is slow.
                if (maxHeight < currentColumnHeight)
                    maxHeight = currentColumnHeight;
            }
            return maxHeight;
        }

        internal static int GetHeighestColumnHeight(this in Board answer, int width)
        {
            fixed (sbyte* cards = &answer.Cards[0])
            {
                return GetHeighestColumnHeight(cards, width, answer.Height);
            }
        }

        internal static int GetHeighestColumnHeight(this in LowLevel.Unsafe.Board8x8 answer, int width)
        {
            fixed (sbyte* cards = answer.Cards)
            {
                return GetHeighestColumnHeight(cards, width, answer.height);
            }
        }

        internal static int GetHeighestColumnHeight<T>(this ref T board, int width) where T : struct, IBoard
        {
            int highestHeight = 0;
            for (int x = 0; x < width; x++)
            {
                var height = board.GetHeight(x);
                if (height > highestHeight)
                    highestHeight = height;
            }
            return highestHeight;
        }

        internal static int CalcHeight(sbyte* cards, int maxHeight, int x)
        {
            for (int start = (x + 1) * maxHeight, temporaryHeight = maxHeight, end = x * maxHeight; --start >= end; temporaryHeight--)
                if (cards[start] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                    return temporaryHeight;
            return 0;
        }

        internal unsafe static void Shrink_UNSAFE_SIDEEFFECT(this ref Board answer, int width, int height)
        {
            if (answer.Length == width * height) return;
            var newAnswer = new Board(width * height, height);
            fixed (sbyte* dest = &newAnswer.Cards[0])
            fixed (sbyte* src = &answer.Cards[0])
            {
                UnsafeUtility.MemCpyStride(dest, height, src, answer.Height, height, width);
            }
            answer = newAnswer;
        }
    }
}