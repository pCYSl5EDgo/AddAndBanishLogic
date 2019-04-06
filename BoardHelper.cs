using Unity.Collections.LowLevel.Unsafe;

namespace AddAndBanish
{
    internal unsafe static class BoardHelper
    {
        internal static void RemoveColumnFromBoard_UNSAFE_UNSAFE_SIDEEFFECT(this in Board board, int x, ref int width)
        {
            fixed (sbyte* ptr = &board.Cards[x * board.Height])
            {
                RemoveColumnFromBoard_UNSAFE_UNSAFE_SIDEEFFECT(ptr, x, ref width, board.Height);
            }
        }

        internal static void RemoveColumnFromBoard_UNSAFE_UNSAFE_SIDEEFFECT(sbyte* ptr, int x, ref int width, int height)
        {
            --width;
            // 1列左に詰める
            UnsafeUtility.MemMove(ptr, ptr + height, height * (width - x));
        }

        internal static bool IsEmptyColumn(this in Board board, int x) => board.Cards.IsEmptyColumn(x, board.Height);
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