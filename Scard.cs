using System;

namespace AddAndBanish
{
    public readonly struct Scard : System.IEquatable<Scard>, System.IComparable<Scard>
    {
        public readonly int X, Y;
        public readonly sbyte Number;

        public Scard(int x, int y, sbyte number)
        {
            X = x;
            Y = y;
            Number = number;
        }

        public int CompareTo(Scard other)
        {
            if (X == other.X)
                return Y - other.Y;
            return X - other.X;
        }

        public bool Equals(Scard other) => X == other.X && Y == other.Y;
        public override int GetHashCode() => (X << 16) | Y;
        public override string ToString() => $"({X}, {Y}) : {Number}";
    }
}