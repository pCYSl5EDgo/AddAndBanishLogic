using System;
using System.Runtime.InteropServices;

namespace AddAndBanish.LowLevel.Unsafe
{
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