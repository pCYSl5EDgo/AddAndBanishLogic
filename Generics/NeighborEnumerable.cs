using System.Collections;
using System.Collections.Generic;

namespace AddAndBanish.Generics
{
    public struct NeighborEnumerable<TBoard, TRemoveCards> : IEnumerable<Scard>
        where TBoard : IBoard
        where TRemoveCards : struct, IRemoveCards
    {
        private readonly TBoard Board;
        private readonly TRemoveCards Parent;
        private readonly int Goal;

        internal NeighborEnumerable(in TBoard board, in TRemoveCards parent, int goal)
        {
            Board = board;
            Parent = parent;
            Goal = goal;
        }

        public NeighborEnumerator<TBoard, TRemoveCards> GetEnumerator() => new NeighborEnumerator<TBoard, TRemoveCards>(this.Board, this.Parent, Goal);
        IEnumerator<Scard> IEnumerable<Scard>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}