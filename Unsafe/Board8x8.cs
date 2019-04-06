using System;
using Unity.Collections.LowLevel.Unsafe;

namespace AddAndBanish.LowLevel.Unsafe
{
    public unsafe struct Board8x8 : IBoard, IEquatable<Board8x8>
    {
        internal readonly int hashCode;
        internal readonly int height;
        internal readonly int width;
        internal fixed sbyte Cards[64];
        public Board8x8(int height, sbyte[] copySource)
        {
            this.height = height;
            this.width = copySource.Length / height;
            fixed (sbyte* dest = Cards)
            fixed (sbyte* src = &copySource[0])
            {
                CalcIndexHelper.MemClear(dest, 64);
                UnsafeUtility.MemCpyStride(dest, 8, src, height, height, width);
                hashCode = CalcHashCode(width, dest);
            }
        }

        internal Board8x8(sbyte* copySource, int width, int height)
        {
            this.width = width;
            this.height = height;
            fixed (sbyte* dest = Cards)
            {
                CalcIndexHelper.MemClear(dest, 64);
                if (copySource != null)
                    UnsafeUtility.MemCpyStride(dest, 8, copySource, height, height, width);
                hashCode = CalcHashCode(width, dest);
            }
        }

        private static int CalcHashCode(int width, sbyte* values)
        {
            switch (width)
            {
                case 0: return 0;
                case 8: return CalcHashCodeEqualTo8(values);
                default: return CalcHashCodeLessThan8(width, values);
            }
        }

        private static int CalcHashCodeLessThan8(int width, sbyte* values)
        {
            // 先頭4bit消費　最上位bitは常に0
            var answer = (uint)width << 28;
            var power2Sum = 0u;
            for (int x = 0; x < width - 1; x++)
            {
                int sum = 0;
                for (uint y = 0; y < 8; y++)
                {
                    var card = values[(x << 3) + y];
                    // 4bit消費
                    if (card == CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                        answer |= (((y - 1) << 1) | (uint)(sum & 1)) << (24 - (x << 2));
                    sum += card;
                    power2Sum += (uint)(card * card);
                }
            }
            // 残り32-(width << 2)bit
            power2Sum <<= width << 2;
            power2Sum >>= width << 2;
            answer |= power2Sum;
            return (int)answer;
        }

        private static int CalcHashCodeEqualTo8(sbyte* values)
        {
            // 先頭1bit消費
            var answer = 0x80_00_00_00u;
            var power2Sum = 0u;
            for (int x = 0; x < 7; x++)
            {
                int sum = 0;
                for (uint y = 0; y < 8; y++)
                {
                    var card = values[(x << 3) + y];
                    // 4bit消費
                    if (card == CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                        answer |= (((y - 1) << 1) | (uint)(sum & 1)) << (27 - (x << 2));
                    sum += card;
                    power2Sum += (uint)(card * card);
                }
            }
            // 残り3bit 0-7
            answer |= power2Sum & 0b111;
            return (int)answer;
        }

        public int Height => height;

        public int Width => width;

        public int Length => height * width;

        public sbyte this[int x, int y] => Cards[(x << 3) + y];

        public Board8x8 Clone() => this;
        IBoard IBoard.Clone() => Clone();

        public int GetHeight(int x)
        {
            fixed(sbyte* ptr = Cards)
            {
                return BoardHelper.CalcHeight(ptr, 8, x);
            }
        }

        public bool DoesExist(int x, int y) => Cards[(x << 3) + y] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER;

        public bool IsMultipleOfArgument(int goal)
        {
            var sum = 0;
            for (int i = 0; i < 64; i++)
                if (Cards[i] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                    sum += Cards[i];
            return sum % goal == 0;
        }

        bool IEquatable<IBoard>.Equals(IBoard other)
        {
            if (other is null) return false;
            if (other is Board8x8 board) return Equals(board);
            if (this.height != other.Height || this.width != other.Width) return false;
            for (int x = width; --x >= 0;)
                for (int y = height; --y >= 0;)
                    if (Cards[(x << 3) + y] != other[x, y])
                        return false;
            return true;
        }

        bool IEquatable<Board8x8>.Equals(Board8x8 other) => Equals(other);

        public bool Equals(in Board8x8 other)
        {
            if (hashCode != other.hashCode || this.height != other.height || this.width != other.width)
                return false;
            fixed (sbyte* ptr0 = Cards)
            fixed (Board8x8* ptr1 = &other)
            {
                return UnsafeUtility.MemCmp(ptr0, ptr1->Cards, 64) == 0;
            }
        }

        public override int GetHashCode() => hashCode;
        public override string ToString()
        {
            var buffer = new System.Text.StringBuilder(5 * Length);
            buffer.Append("{ Width : ").Append(Width).Append(", Height : ").Append(Height).Append("}\n");
            buffer.AppendBoard_GameView(ref this);
            return buffer.ToString();
        }
    }
}