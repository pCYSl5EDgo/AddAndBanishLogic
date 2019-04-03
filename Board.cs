using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace AddAndBanish
{
    public readonly struct Board : IEquatable<Board>
    {
        internal readonly sbyte[] Cards;
        public readonly int Height;
        public int Width => Cards.Length / Height;
        public int Length => Cards.Length;

        public Board(int length, int height)
        {
            Cards = new sbyte[length];
            Height = height;
            Cards.MemClear();
        }

        public Board(int height, sbyte[] cards)
        {
            Cards = cards;
            Height = height;
        }

        public Board CalcNextStepByRemove(in RemoveCards removeCards)
        {
            var answer = Clone();
            var width = Width;
            for (int x = width; --x >= 0;)
            {
                RemoveCardsInColumn_UNSAFE_SIDEEFFECT(removeCards, answer, x);
                if (IsEmptyColumn(answer, x))
                    RemoveColumnFromBoard_UNSAFE_UNSAFE_SIDEEFFECT(answer, x, ref width);
            }
            var height = GetHeighestColumnHeight(answer, width);
            Shrink_UNSAFE_SIDEEFFECT(ref answer, width, height);
            return answer;
        }

        public unsafe Board Clone()
        {
            sbyte[] cards = new sbyte[Length];
            fixed (sbyte* dest = &cards[0])
            fixed (sbyte* src = &this.Cards[0])
            {
                UnsafeUtility.MemCpy(dest, src, Length);
            }
            var answer = new Board(Height, cards);
            return answer;
        }

        public bool IsMultipleOfArgument(int goal)
        {
            int sum = 0;
            for (int i = 0; i < Cards.Length; i++)
            {
                var element = Cards[i];
                if (element == CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                    continue;
                sum += element;
            }
            return sum % goal == 0;
        }

        internal unsafe static void Shrink_UNSAFE_SIDEEFFECT(ref Board answer, int width, int height)
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

        internal static int GetHeighestColumnHeight(in Board answer, int width)
        {
            if (width == 0) return 0;
            var cards = answer.Cards;
            int height = CalcHeight(answer.Height, cards, --width);
            for (; --width >= 0;)
            {
                var currentColumnHeight = CalcHeight(answer.Height, cards, width);
                // Math.Max is slow.
                if (height < currentColumnHeight)
                    height = currentColumnHeight;
            }
            return height;
        }
        internal static int CalcHeight(int Height, sbyte[] cards, int x)
        {
            for (int start = (x + 1) * Height, height = Height; --start >= 0; height--)
            {
                if (cards[start] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                    return height;
            }
            return 0;
        }
        internal unsafe void RemoveCardsInColumn_UNSAFE_SIDEEFFECT(in RemoveCards removeCards, in Board newBoard, int x)
        {
            var newCards = newBoard.Cards;
            fixed (sbyte* ptr = &newCards[x * Height])
            {
                for (int y = Height, i = (x + 1) * Height - 1; --y >= 0; i--)
                {
                    var dontRemove = removeCards.Cards[i] == CalcIndexHelper.NOT_REMOVE_CARD_NUMBER;
                    if (dontRemove)
                        continue;
                    RemoveOneCardInColumn_UNSAFE_SIDEEFFECT(newCards, x, y, ptr);
                }
            }
        }
        internal unsafe void RemoveOneCardInColumn_UNSAFE_SIDEEFFECT(sbyte[] newCards, int x, int y, sbyte* ptr)
        {
            UnsafeUtility.MemMove(ptr + y, ptr + y + 1, Height - y - 1);
            newCards[(x + 1) * Height - 1] = CalcIndexHelper.NOT_REMOVE_CARD_NUMBER;
        }
        internal static bool IsEmptyColumn(in Board newBoard, int x)
        {
            // その列について
            for (int index = x * newBoard.Height, end = (x + 1) * newBoard.Height; index < end; index++)
                // 1手進めた盤面のその列のあるマスが空欄であるか調べる
                if (newBoard.Cards[index] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                    return false;
            return true;
        }
        internal unsafe static void RemoveColumnFromBoard_UNSAFE_UNSAFE_SIDEEFFECT(in Board newBoard, int x, ref int width)
        {
            --width;
            fixed (sbyte* ptr = &newBoard.Cards[x * newBoard.Height])
            {
                // 1列左に詰める
                UnsafeUtility.MemMove(ptr, ptr + newBoard.Height, newBoard.Height * (width - x));
            }
        }

        public unsafe bool Equals(in Board other)
        {
            if (Height != other.Height) return false;
            if (object.ReferenceEquals(Cards, other.Cards)) return true;
            if (Length != other.Length) return false;
            fixed (sbyte* ptr = &Cards[0])
            fixed (sbyte* otherPtr = &other.Cards[0])
            {
                return UnsafeUtility.MemCmp(ptr, otherPtr, Length) == 0;
            }
        }

        bool IEquatable<Board>.Equals(Board other) => Equals(other);

        public string ToString(Func<sbyte[], IEnumerable<IEnumerable<sbyte>>> sorter)
        {
            var buffer = new System.Text.StringBuilder(5 * Length);
            buffer.Append("{ Width : ").Append(Width).Append(", Height : ").Append(Height).Append("}\n");
            foreach (var line in sorter(Cards))
            {
                foreach (var card in line)
                    AppendCard(buffer, card);
                buffer.Append('\n');
            }
            return buffer.ToString();
        }

        public override string ToString()
        {
            var buffer = new System.Text.StringBuilder(5 * Length);
            buffer.Append("{ Width : ").Append(Width).Append(", Height : ").Append(Height).Append("}\n");
            AppendBoard_GameView(buffer);
            return buffer.ToString();
        }

        private void AppendBoard_ArraySequence(System.Text.StringBuilder buffer)
        {
            for (int x = 0, end = Width; x < end; x++)
            {
                for (int y = 0; y < Height; y++)
                    AppendCard(buffer, Cards[x * Height + y]);
                buffer.Append('\n');
            }
        }

        private void AppendBoard_GameView(System.Text.StringBuilder buffer)
        {
            for (int y = Height; --y >= 0;)
            {
                for (int x = 0, end = Width; x < end; x++)
                    AppendCard(buffer, Cards[x * Height + y]);
                buffer.Append('\n');
            }
        }
        private void AppendCard(System.Text.StringBuilder buffer, sbyte card)
        {
            if (card == CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                buffer.Append("   ,");
            else buffer.Append($"{card,3}").Append(',');
        }
    }
}