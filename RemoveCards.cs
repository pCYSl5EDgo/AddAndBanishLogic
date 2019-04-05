using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

namespace AddAndBanish
{
    public readonly struct RemoveCards : IEquatable<RemoveCards>, IRemoveCards
    {
        internal readonly sbyte[] Cards;
        public readonly int Height;
        public readonly int Sum;
        public readonly int Count;
        public readonly int Width;
        public int Length => Cards.Length;
        int IRemoveCards.Height => Height;
        int IRemoveCards.Width => Width;
        int IRemoveCards.Count => Count;

        int IRemoveCards.Sum => Sum;

        public RemoveCards(in Board board)
        {
            Cards = new sbyte[board.Length];
            Height = board.Height;
            Width = Cards.Length / Height;
            Cards.MemClear();
            Sum = 0;
            Count = 0;
        }

        private RemoveCards(in RemoveCards removeCards, int index, sbyte number)
        {
            var remCards = removeCards.Cards;
            Cards = new sbyte[remCards.Length];
            Height = removeCards.Height;
            Width = Cards.Length / Height;
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
        IRemoveCards IRemoveCards.Add(int x, int y, sbyte number) => Add(x, y, number);

        public int GetConnection(int x, int y)
        {
            int answer = 0;
            if (x > 0 && this[x - 1, y] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                ++answer;
            if (x < Width - 1 && this[x + 1, y] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                ++answer;
            if (y > 0 && this[x, y - 1] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                ++answer;
            if (y < Height - 1 && this[x, y + 1] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
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
                        if (!DoesExist(x, y)) continue;
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

        public sbyte this[int x, int y] => Cards[x * Height + y];

        public Board CalcNextStepByRemove(in Board board)
        {
            var builder = new BoardBuilder_MUST_BE_DISPOSED(board.Length, board.Height);
            try
            {
                builder.FromBoard(board);
                foreach (var (x, y) in this)
                {
                    builder.Remove(x, y);
                    if (y == 0 && !builder.DoesExist(x, 0))
                        builder.RemoveColumn(x);
                }
                builder.Shrink();
                return builder.ToBoard();
            }
            finally
            {
                builder.Dispose();
            }
        }

        public NeighborEnumerable GetNeighborEnumerable(in Board board, int goal) => new NeighborEnumerable(this, board, goal);
        public IEnumerable<Scard> GetNeighborEnumerable(IBoard board, int goal)
        {
            if (board is Board concreteBoard) return GetNeighborEnumerable(concreteBoard, goal);
            if (board is null) throw new ArgumentNullException();
            return GetNeighborEnumerable_Internal(board, goal);
        }

        private IEnumerable<Scard> GetNeighborEnumerable_Internal(IBoard board, int goal)
        {
            for (int x = board.Width, height = board.Height; --x >= 0;)
            {
                for (int y = height; --y >= 0;)
                {
                    if (DoesExist(x, y)) continue;
                    var currentCard = board[x, y];
                    if (Sum + currentCard > goal) continue;
                    if (this.Is_AnyOf4NeighborCards_RemoveCard(x, y))
                        yield return new Scard(x, y, currentCard);
                }
            }
        }

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

        public bool Equals(IRemoveCards other)
        {
            if (other is RemoveCards removeCards) return Equals(removeCards);
            if (Length != other.Length || Height != other.Height) return false;
            for (int x = 0, width = Width; x < width; x++)
                for (int y = 0; y < Height; y++)
                    if (this[x, y] != other[x, y])
                        return false;
            return true;
        }

        public bool DoesExist(int x, int y) => this[x, y] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER;

        public Enumerator GetEnumerator() => new Enumerator(this);
        IEnumerator<(int x, int y)> IEnumerable<(int x, int y)>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<(int x, int y)>
        {
            private readonly RemoveCards Parent;
            private (int x, int y) xy;
            public (int x, int y) Current => xy;
            (int x, int y) IEnumerator<(int x, int y)>.Current => xy;
            object IEnumerator.Current => Current;

            public Enumerator(in RemoveCards parent)
            {
                Parent = parent;
                xy = (parent.Width, 0);
            }

            public void Dispose() => this = default;

            public bool MoveNext()
            {
                while (true)
                {
                    if (--xy.y < 0)
                    {
                        xy.y = Parent.Height - 1;
                        --xy.x;
                    }
                    if (xy.x < 0)
                        return false;
                    if (Parent.DoesExist(xy.x, xy.y))
                        return true;
                }
            }

            public void Reset() => xy = (Parent.Width, 0);
        }

        public struct NeighborEnumerable : IEnumerable<Scard>
        {
            private readonly sbyte[] BoardCards;
            private readonly RemoveCards Parent;
            private readonly int Goal;

            public NeighborEnumerable(in RemoveCards parent, in Board board, int goal)
            {
                BoardCards = board.Cards;
                Parent = parent;
                Goal = goal;
            }

            public NeighborEnumerator GetEnumerator() => new NeighborEnumerator(this);

            IEnumerator<Scard> IEnumerable<Scard>.GetEnumerator() => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public struct NeighborEnumerator : IEnumerator<Scard>
            {
                private readonly sbyte[] BoardCards;
                private readonly RemoveCards Parent;
                private readonly int Goal;
                private readonly int Width;
                private int x, y;

                public NeighborEnumerator(in NeighborEnumerable enumerable)
                {
                    BoardCards = enumerable.BoardCards;
                    Parent = enumerable.Parent;
                    Goal = enumerable.Goal;
                    Width = x = Parent.Cards.Length / Parent.Height;
                    y = 0;
                }

                public bool MoveNext()
                {
                    while (true)
                    {
                        if (y-- == 0)
                        {
                            y = Parent.Height - 1;
                            --x;
                        }
                        if (x < 0) return false;
                        if (Parent.IsInvalidRemoveCandidate(x, y, this.BoardCards, this.Goal))
                            continue;
                        if (Parent.Is_AnyOf4NeighborCards_RemoveCard(x, y))
                            return true;
                    }
                }

                public Scard Current => new Scard(x, y, BoardCards[x * Parent.Height + y]);

                object IEnumerator.Current => Current;

                public void Reset()
                {
                    x = Width;
                    y = 0;
                }

                public void Dispose() { }
            }
        }
    }
}