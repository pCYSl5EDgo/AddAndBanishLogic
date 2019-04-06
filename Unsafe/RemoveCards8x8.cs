using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

namespace AddAndBanish.LowLevel.Unsafe
{
    public unsafe struct RemoveCards8x8 : IRemoveCards
    {
        internal int Width, Height, Sum, Count;
        internal fixed sbyte Values[64];
        internal ref sbyte this[int x, int y] => ref Values[(x << 3) + y];
        sbyte IRemoveCards.this[int x, int y] => Values[(x << 3) + y];

        public int Length => Width * Height;
        int IRemoveCards.Width => Width;
        int IRemoveCards.Height => Height;
        int IRemoveCards.Count => Count;
        int IRemoveCards.Sum => Sum;

        public bool IsHamilton => throw new NotImplementedException();
        public bool IsValid(int goal) => Sum == goal && IsHamilton;

        IRemoveCards IRemoveCards.Add(int x, int y, sbyte number) => Add(x, y, number);
        public RemoveCards8x8 Add(int x, int y, sbyte number)
        {
            var answer = this;
            if (x >= answer.Width)
                answer.Width = x + 1;
            if (y >= answer.Height)
                answer.Height = y + 1;
            answer.Sum += (answer[x, y] = number);
            answer.Count++;
            return answer;
        }

        public bool DoesExist(int x, int y) => this[x, y] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER;

        public bool Equals(in RemoveCards8x8 other)
        {
            if (Count != other.Count || Sum != other.Count || Width != other.Width || Height != other.Height) return false;
            fixed (sbyte* thisPtr = this.Values)
            fixed (sbyte* otherPtr = other.Values)
            {
                return UnsafeUtility.MemCmp(thisPtr, otherPtr, Width << 3) == 0;
            }
        }

        bool IEquatable<IRemoveCards>.Equals(IRemoveCards other)
        {
            if (other is null) return false;
            if (other is RemoveCards8x8 other8x8) return Equals(other8x8);
            if (Count != other.Count || Sum != other.Count || Width != other.Width || Height != other.Height) return false;
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (this[x, y] != other[x, y])
                        return false;
            return true;
        }

        public int GetConnection(int x, int y) => (x > 0 && DoesExist(x - 1, y) ? 1 : 0) + (x < Width - 1 && DoesExist(x + 1, y) ? 1 : 0) + (y > 0 && DoesExist(x, y - 1) ? 1 : 0) + (y < Height - 1 && DoesExist(x, y + 1) ? 1 : 0);

        public Generics.RemoveCardsEnumerator<RemoveCards8x8> GetEnumerator() => new Generics.RemoveCardsEnumerator<RemoveCards8x8>(this);
        IEnumerator<(int x, int y)> IEnumerable<(int x, int y)>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        Generics.NeighborEnumerable<Board8x8, RemoveCards8x8> GetNeighborEnumerable(in Board8x8 board, int goal) => new Generics.NeighborEnumerable<Board8x8, RemoveCards8x8>(board, this, goal);
        IEnumerable<Scard> IRemoveCards.GetNeighborEnumerable(IBoard board, int goal) => board is Board8x8 board8x8 ? (IEnumerable<Scard>)GetNeighborEnumerable(board8x8, goal) : (IEnumerable<Scard>)new Generics.NeighborEnumerable<IBoard, RemoveCards8x8>(board, this, goal);
    }
}