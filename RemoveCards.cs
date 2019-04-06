using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

namespace AddAndBanish
{
    public readonly struct RemoveCards : IEquatable<RemoveCards>, IRemoveCards<Board>
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
        IRemoveCards<Board> IRemoveCards<Board>.Add(int x, int y, sbyte number) => Add(x, y, number);

        public int GetConnection(int x, int y)
        => (x > 0 && DoesExist(x - 1, y) ? 1 : 0) + (x < Width - 1 && DoesExist(x + 1, y) ? 1 : 0) + (y > 0 && DoesExist(x, y - 1) ? 1 : 0) + (y < Height - 1 && DoesExist(x, y + 1) ? 1 : 0);

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

        internal sbyte this[int x, int y] => Cards[x * Height + y];
        bool IRemoveCards.this[int x, int y] => DoesExist(x, y);
        Board IRemoveCards<Board>.CalcNextStepByRemove(Board board) => CalcNextStepByRemove(board);
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
            if (other is null) return false;
            if (other is RemoveCards removeCards) return Equals(removeCards);
            if (Length != other.Length || Height != other.Height) return false;
            for (int x = 0, width = Width; x < width; x++)
                for (int y = 0; y < Height; y++)
                    if (DoesExist(x, y) != other[x, y])
                        return false;
            return true;
        }

        public bool Equals(IRemoveCards<Board> other)
        {
            if (other is null) return false;
            if (other is RemoveCards removeCards) return Equals(removeCards);
            if (Length != other.Length || Height != other.Height) return false;
            for (int x = 0, width = Width; x < width; x++)
                for (int y = 0; y < Height; y++)
                    if (DoesExist(x, y) != other[x, y])
                        return false;
            return true;
        }

        public bool DoesExist(int x, int y) => this[x, y] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER;

        public Generics.NeighborEnumerable<Board, RemoveCards> GetNeighborEnumerable(in Board board, int goal) => new Generics.NeighborEnumerable<Board, RemoveCards>(board, this, goal);
        IEnumerable<Scard> IRemoveCards.GetNeighborEnumerable(IBoard board, int goal)
        => board is null ? throw new ArgumentNullException() : board is Board _board ? (IEnumerable<Scard>)GetNeighborEnumerable(_board, goal) : new Generics.NeighborEnumerable<IBoard, RemoveCards>(board, this, goal);
        public Generics.RemoveCardsEnumerator<Board, RemoveCards> GetEnumerator() => new Generics.RemoveCardsEnumerator<Board, RemoveCards>(this);
        IEnumerator<(int x, int y)> IEnumerable<(int x, int y)>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}