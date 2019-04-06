using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

namespace AddAndBanish.LowLevel.Unsafe
{
    public unsafe struct RemoveCards8x8 : IRemoveCards<Board8x8>
    {
        private const int _Width = 8;
        private const int _Height = 8;
        private const int _Length = 64;
        internal int Sum, Count;
        internal ulong Cards;

        public int Length => _Length;
        public int Width => _Width;
        public int Height => _Height;
        int IRemoveCards.Count => Count;
        int IRemoveCards.Sum => Sum;

        public bool IsHamilton => this.IsHamilton();

        public bool this[int x, int y]
        {
            get => ((Cards >> ((x << 3) + y)) & 1) == 1;
            internal set => Cards ^= (value ? 1u : 0u) << ((x << 3) + y);
        }

        public bool Equals(in RemoveCards8x8 other) => Cards == other.Cards && Sum == other.Sum && Count == other.Count;
        public bool Equals(IRemoveCards other)
        {
            if (other is null) return false;
            if (other is RemoveCards8x8 other8x8) return Equals(other8x8);
            int width = other.Width, height = other.Height;
            if (Count != other.Count || Sum != other.Count || _Width != width || _Height != height) return false;
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (this[x, y] != other[x, y])
                        return false;
            return true;
        }

        public bool Equals(IRemoveCards<Board8x8> other)
        {
            if (other is null) return false;
            if (other is RemoveCards8x8 other8x8) return Equals(other8x8);
            if (Count != other.Count || Sum != other.Count || _Width != other.Width || _Height != other.Height) return false;
            for (int x = 0; x < _Width; x++)
                for (int y = 0; y < _Height; y++)
                    if (this[x, y] != other[x, y])
                        return false;
            return true;
        }

        int IRemoveCards.GetConnection(int x, int y) => this.GetConnection(x, y);

        public Generics.RemoveCardsEnumerator<Board8x8, RemoveCards8x8> GetEnumerator() => new Generics.RemoveCardsEnumerator<Board8x8, RemoveCards8x8>(this);
        IEnumerator<(int x, int y)> IEnumerable<(int x, int y)>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Generics.NeighborEnumerable<Board8x8, RemoveCards8x8> GetNeighborEnumerable(in Board8x8 board, int goal) => new Generics.NeighborEnumerable<Board8x8, RemoveCards8x8>(board, this, goal);
        IEnumerable<Scard> IRemoveCards.GetNeighborEnumerable(IBoard board, int goal)
        => board is null ? throw new ArgumentNullException() : board is Board8x8 board8x8 ? (IEnumerable<Scard>)GetNeighborEnumerable(board8x8, goal) : new Generics.NeighborEnumerable<IBoard, RemoveCards8x8>(board, this, goal);

        public Board8x8 CalcNextStepByRemove(ref Board8x8 board)
        {
            var builder = new BoardBuilder_MUST_BE_DISPOSED();
            try
            {
                builder.FromBoard(ref board);
                foreach (var (x, y) in this)
                {
                    builder.Remove(x, y);
                    if (y == 0 && !builder.DoesExist(x, 0))
                        builder.RemoveColumn(x);
                }
                builder.Shrink();
                return builder.ToBoard8x8();
            }
            finally
            {
                builder.Dispose();
            }
        }
        Board8x8 IRemoveCards<Board8x8>.CalcNextStepByRemove(Board8x8 board) => CalcNextStepByRemove(ref board);

        public RemoveCards8x8 Add(int x, int y, sbyte number)
        {
            var answer = this;
            if (number == CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
            {
                if (answer[x, y])
                {
                    answer[x, y] = false;
                    answer.Count--;
                    answer.Sum -= number;
                }
            }
            else
            {
                if (!answer[x, y])
                {
                    answer[x, y] = true;
                    answer.Count++;
                    answer.Sum += number;
                }
            }
            return answer;
        }

        IRemoveCards<Board8x8> IRemoveCards<Board8x8>.Add(int x, int y, sbyte number) => Add(x, y, number);

        IRemoveCards IRemoveCards.Add(int x, int y, sbyte number) => Add(x, y, number);

        public bool IsValid(int goal) => Sum == goal && IsHamilton;
    }
}