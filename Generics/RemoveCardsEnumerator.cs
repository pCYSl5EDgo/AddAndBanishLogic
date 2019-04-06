using System;
using System.Collections;
using System.Collections.Generic;

namespace AddAndBanish.Generics
{
    public struct RemoveCardsEnumerator<T> : IEnumerator<(int x, int y)> where T : IRemoveCards
    {
        private T Parent;
        private (int x, int y) xy;

        internal RemoveCardsEnumerator(in T parent)
        {
            Parent = parent;
            xy = (parent.Width, 0);
        }

        public (int x, int y) Current => xy;
        object IEnumerator.Current => Current;
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
                if (xy.x < 0) return false;
                if (Parent.DoesExist(xy.x, xy.y))
                    return true;
            }
        }
        public void Reset() => throw new NotImplementedException();
    }
}