using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

namespace AddAndBanish
{
    public readonly struct Board : IEquatable<Board>, IBoard
    {
        internal readonly sbyte[] Cards;
        public readonly int Height;
        public int Width => Cards.Length / Height;
        public int Length => Cards.Length;

        int IBoard.Height => Height;

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

        public sbyte this[int x, int y] => Cards[x * Height + y];

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

        IBoard IBoard.Clone() => Clone();

        public bool DoesExist(int x, int y) => this[x, y] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER;

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
                    buffer.AppendCard(card);
                buffer.Append('\n');
            }
            return buffer.ToString();
        }

        public override string ToString()
        {
            var buffer = new System.Text.StringBuilder(5 * Length);
            buffer.Append("{ Width : ").Append(Width).Append(", Height : ").Append(Height).Append("}\n");
            buffer.AppendBoard_GameView(this);
            return buffer.ToString();
        }

        public bool Equals(IBoard other)
        {
            if (other is Board board) return Equals(board);
            if (Length != other.Length || Height != other.Height) return false;
            for (int x = 0, end = Width; x < end; x++)
                for (int y = 0; y < Height; y++)
                    if (this[x, y] != other[x, y])
                        return false;
            return true;
        }
    }
}