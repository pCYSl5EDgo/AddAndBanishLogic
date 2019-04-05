using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace AddAndBanish
{
    public unsafe ref struct BoardBuilder_MUST_BE_DISPOSED
    {
        internal sbyte* cards;
        internal int length, height, width;
        internal const Allocator allocator = Allocator.Temp;

        public int Length => length;
        public int Height => height;
        public int Width => width;

        public BoardBuilder_MUST_BE_DISPOSED(int length, int height)
        {
            this.length = length;
            this.height = height;
            this.width = length / height;
            cards = (sbyte*)UnsafeUtility.Malloc(length, 1, allocator);
            CalcIndexHelper.MemClear(cards, length);
        }

        public Board ToBoard()
        {
            var answer = new Board(length, height);
            fixed (sbyte* dest = &answer.Cards[0])
            {
                UnsafeUtility.MemCpy(dest, cards, length);
            }
            return answer;
        }

        public sbyte this[int x, int y]
        {
            get => cards[x * height + y];
            internal set
            {
                EnlargeIfNeeded(x, y);
                cards[x * height + y] = value;
            }
        }

        public bool DoesExist(int x, int y) => x >= 0 && x < width && y >= 0 && y < height && cards[x * height + y] != CalcIndexHelper.NOT_REMOVE_CARD_NUMBER;

        internal void EnlargeIfNeeded(int x, int y)
        {
            if (x >= width)
            {
                if (y >= height)
                    EnlargeXY(x, y);
                else
                    EnlargeX(x);
            }
            else
            {
                if (y >= height)
                    EnlargeY(y);
            }
        }

        internal void EnlargeY(int y) => EnlargeWidthHeight(width, y + 1);
        internal void EnlargeX(int x) => EnlargeWidthHeight(x + 1, height);
        internal void EnlargeXY(int x, int y) => EnlargeWidthHeight(x + 1, y + 1);

        internal void EnlargeWidthHeight(int newWidth, int newHeight)
        {
            if (newWidth == width && newHeight == height) return;
            var newLength = newWidth * newHeight;
            var newCards = (sbyte*)UnsafeUtility.Malloc(newLength, 1, allocator);
            CalcIndexHelper.MemClear(newCards, newLength);
            if (cards != null)
            {
                UnsafeUtility.MemCpyStride(newCards, newHeight, cards, height, height, width);
                UnsafeUtility.Free(cards, allocator);
            }
            this.width = newWidth;
            this.height = newHeight;
            this.length = newLength;
            this.cards = newCards;
        }

        public void Dispose()
        {
            if (cards != null)
                UnsafeUtility.Free(cards, allocator);
            this = default;
        }
    }
}
