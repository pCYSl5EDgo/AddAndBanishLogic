using System;
using Unity.Collections.LowLevel.Unsafe;

namespace AddAndBanish.LowLevel.Unsafe
{
    public unsafe struct Board8x8 : IBoard, IEquatable<Board8x8>
    {
        internal readonly int hashCode;
        internal readonly int height;
        internal readonly int width;
        internal fixed sbyte Values[64];

        public Board8x8(sbyte[] values, int height)
        {
            this.height = height;
            this.width = values.Length / height;
            fixed (sbyte* dest = Values)
            fixed (sbyte* src = &values[0])
            {
                CalcIndexHelper.MemClear(dest, 64);
                UnsafeUtility.MemCpyStride(dest, 8, src, height, height, width);
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

        int IBoard.Height => height;

        int IBoard.Width => width;

        int IBoard.Length => height * width;

        sbyte IBoard.this[int x, int y] => Values[(x << 3) + y];

        public Board8x8 Clone() => this;
        IBoard IBoard.Clone() => Clone();

        public bool DoesExist(int x, int y) => Values[(x << 3) + y] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER;

        bool IBoard.IsMultipleOfArgument(int goal)
        {
            var sum = 0;
            for (int i = 0; i < 64; i++)
                if (Values[i] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER)
                    sum += Values[i];
            return sum % goal == 0;
        }

        bool IEquatable<IBoard>.Equals(IBoard other)
        {
            if (other is null) return false;
            if (other is Board8x8 board) return Equals(board);
            if (this.height != other.Height || this.width != other.Width) return false;
            for (int x = width; --x >= 0;)
                for (int y = height; --y >= 0;)
                    if (Values[(x << 3) + y] != other[x, y])
                        return false;
            return true;
        }

        bool IEquatable<Board8x8>.Equals(Board8x8 other) => Equals(other);

        public bool Equals(in Board8x8 other)
        {
            if (hashCode != other.hashCode || this.height != other.height || this.width != other.width)
                return false;
            fixed (sbyte* ptr0 = Values)
            fixed (Board8x8* ptr1 = &other)
            {
                return UnsafeUtility.MemCmp(ptr0, ptr1->Values, 64) == 0;
            }
        }

        public override int GetHashCode() => hashCode;
    }
}