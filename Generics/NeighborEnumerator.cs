using System;
using System.Collections;
using System.Collections.Generic;

namespace AddAndBanish.Generics
{
    public struct NeighborEnumerator<TBoard, TRemoveCards> : IEnumerator<Scard>
        where TBoard : IBoard
        where TRemoveCards : struct, IRemoveCards
    {
        private readonly TBoard Board;
        private TRemoveCards Parent;
        private readonly int Goal;
        private int x, y;

        internal NeighborEnumerator(in TBoard board, in TRemoveCards parent, int goal)
        {
            Board = board;
            Parent = parent;
            x = Parent.Width;
            y = 0;
            Goal = goal;
        }
        public Scard Current => new Scard(x, y, Board[x, y]);
        object IEnumerator.Current => Current;

        void IDisposable.Dispose() => this = default;

        bool IEnumerator.MoveNext()
        {
            while (true)
            {
                if (--y < 0)
                {
                    y = Parent.Height - 1;
                    --x;
                }
                if (x < 0) return false;
                var card = Board[x, y];
                if (card + Parent.Sum > Goal) continue;
                if (Parent.Is_AnyOf4NeighborCards_RemoveCard(x, y))
                    return true;
            }
        }

        void IEnumerator.Reset() => throw new NotImplementedException();
    }
}
