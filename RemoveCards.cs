using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace AddAndBanish
{
    public readonly struct RemoveCards : IEquatable<RemoveCards>
    {
        internal readonly sbyte[] Cards;
        public readonly int Height;
        public readonly int Sum;
        public readonly int Count;
        public int Width => Cards.Length / Height;
        public int Length => Cards.Length;

        public RemoveCards(in Board board)
        {
            Cards = new sbyte[board.Length];
            Height = board.Height;
            Cards.MemClear();
            Sum = 0;
            Count = 0;
        }

        private RemoveCards(in RemoveCards removeCards, int index, sbyte number)
        {
            var remCards = removeCards.Cards;
            Cards = new sbyte[remCards.Length];
            Height = removeCards.Height;
            unsafe
            {
                fixed (sbyte* dest = &Cards[0])
                fixed (sbyte* src = &remCards[0])
                {
                    UnsafeUtility.MemCpy(dest, src, Cards.Length);
                }
            }
            Cards[index] = number;
            Sum = removeCards.Sum + number;
            Count = removeCards.Count + 1;
        }

        public RemoveCards Add(int x, int y, sbyte number) => new RemoveCards(this, x * Height + y, number);

        public int GetConnection(int x, int y)
        {
            int answer = 0;
            if (x > 0 && Cards[(x - 1) * Height + y] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                ++answer;
            if (x < Width - 1 && Cards[(x + 1) * Height + y] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                ++answer;
            if (y > 0 && Cards[x * Height + y - 1] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                ++answer;
            if (y < Height - 1 && Cards[x * Height + y + 1] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                ++answer;
            return answer;
        }

        public bool IsValid(int goal) => Sum == goal && IsHamilton;

        public unsafe bool IsHamilton
        {
            get
            {
                int oneConnection = 0;
                for (int x = 0, end = Width; x < end; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        if (Cards[x * Height + y] == CalcIndexHelper.NOT_REMOVE_CARD_NUMBER) continue;
                        switch (GetConnection(x, y))
                        {
                            case 0:
                                return false;
                            case 1:
                                if (++oneConnection > 2)
                                    return false;
                                break;
                            default:
                                break;
                        }
                    }
                }
                return true;
            }
        }

        public Enumerable GetNeighborEnumerable(in Board board, int goal) => new Enumerable(this, board, goal);

        public unsafe bool Equals(in RemoveCards other)
        {
            if (this.Sum != other.Sum || this.Height != other.Height || this.Count != other.Count) return false;
            if (object.ReferenceEquals(this.Cards, other.Cards)) return true;
            if (this.Length != other.Length) return false;
            fixed (sbyte* thisPtr = &this.Cards[0])
            fixed (sbyte* otherPtr = &other.Cards[0])
            {
                return UnsafeUtility.MemCmp(thisPtr, otherPtr, Length) == 0;
            }
        }

        bool IEquatable<RemoveCards>.Equals(RemoveCards other) => Equals(other);

        public struct Enumerable : IEnumerable<Scard>
        {
            private readonly sbyte[] BoardCards;
            private readonly sbyte[] Cards;
            private readonly int Height;
            private readonly int Goal;
            private readonly int RemoveSum;
            public Enumerable(in RemoveCards parent, in Board board, int goal)
            {
                BoardCards = board.Cards;
                Cards = parent.Cards;
                Height = parent.Height;
                RemoveSum = parent.Sum;
                Goal = goal;
            }

            public Enumerator GetEnumerator() => new Enumerator(this);

            IEnumerator<Scard> IEnumerable<Scard>.GetEnumerator() => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public struct Enumerator : IEnumerator<Scard>
            {
                private readonly sbyte[] BoardCards;
                private readonly sbyte[] Cards;
                private readonly int Height;
                private readonly int Goal;
                private readonly int RemoveSum;
                private int x, y;
                private int Length => BoardCards.Length;

                public Enumerator(in Enumerable enumerable)
                {
                    BoardCards = enumerable.BoardCards;
                    Cards = enumerable.Cards;
                    Height = enumerable.Height;
                    RemoveSum = enumerable.RemoveSum;
                    Goal = enumerable.Goal;
                    x = enumerable.Cards.Length / Height;
                    y = 0;
                }

                public bool MoveNext()
                {
                    while (true)
                    {
                        if (y-- == 0)
                        {
                            y = Height - 1;
                            --x;
                        }
                        var index = x * Height + y;
                        if (index < 0) return false;
                        if (IsInvalidRemoveCandidate(index))
                            continue;
                        if ((x > 0 && IsRemoveCardMember(index - Height)) ||
                            (index + Height < Length && IsRemoveCardMember(index + Height)) ||
                            (y > 0 && IsRemoveCardMember(index - 1)) ||
                            (y + 1 < Height && IsRemoveCardMember(index + 1))) return true;
                    }
                }

                public Scard Current => new Scard(x, y, BoardCards[x * Height + y]);

                object IEnumerator.Current => Current;

                private bool IsRemoveCardMember(int index) => Cards[index] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER;
                private bool IsInvalidRemoveCandidate(int index) => IsRemoveCardMember(index) || BoardCards[index] + RemoveSum > Goal;

                public void Reset()
                {
                    x = Length / Height;
                    y = 0;
                }

                public void Dispose() { }
            }
        }
    }


    public struct RemoveCards_UNSAFE_MUTABLE
    {
        public sbyte[] Cards;
        public int Height;
        public int Sum;
        public int Count;

        public void Add(int x, int y, sbyte card)
        {
            var index = x * Height + y;
            if (Cards[index] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER) return;
            Cards[index] = card;
            Sum += card;
            ++Count;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct RemoveCards_UNSAFE_UNION : IEquatable<RemoveCards>, IEquatable<RemoveCards_UNSAFE_UNION>
    {
        [FieldOffset(0)]
        public RemoveCards_UNSAFE_MUTABLE Mutable;
        [FieldOffset(0)]
        public RemoveCards Immutable;

        public bool Equals(RemoveCards other) => Immutable.Equals(other);

        bool IEquatable<RemoveCards_UNSAFE_UNION>.Equals(RemoveCards_UNSAFE_UNION other) => Equals(other.Immutable);
    }
}