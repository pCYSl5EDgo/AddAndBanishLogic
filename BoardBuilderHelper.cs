using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace AddAndBanish
{
    public unsafe static class BoardBuilderHelper
    {
        public static ref BoardBuilder_MUST_BE_DISPOSED AddOrSetRange(ref this BoardBuilder_MUST_BE_DISPOSED builder, IEnumerable<Scard> cards)
        {
            foreach (var card in cards)
                builder.AddOrSet(card);
            return ref builder;
        }
        public static ref BoardBuilder_MUST_BE_DISPOSED AddOrSetRange(ref this BoardBuilder_MUST_BE_DISPOSED builder, IEnumerable<IEnumerable<Scard>> cards)
        {
            foreach (var column in cards)
                foreach (var card in column)
                    builder.AddOrSet(card);
            return ref builder;
        }

        public static ref BoardBuilder_MUST_BE_DISPOSED AddOrSet(ref this BoardBuilder_MUST_BE_DISPOSED builder, Scard card)
        {
            builder[card.X, card.Y] = card.Number;
            return ref builder;
        }

        public static ref BoardBuilder_MUST_BE_DISPOSED Clear(this ref BoardBuilder_MUST_BE_DISPOSED builder)
        {
            builder.Dispose();
            return ref builder;
        }

        public static ref BoardBuilder_MUST_BE_DISPOSED RemoveColumn(this ref BoardBuilder_MUST_BE_DISPOSED builder, int x)
        {
            // 削除行が現在の盤面の範囲外なら無視
            if (x < 0 || x >= builder.width) return ref builder;
            builder.length -= builder.height;
            // 盤面の最右端なら盤面の幅を1減らす
            if (x == --builder.width)
                return ref builder;
            var ptr = builder.cards + x * builder.height;
            // 1列左に詰める
            UnsafeUtility.MemMove(ptr, ptr + builder.height, builder.height * (builder.width - x));
            return ref builder;
        }

        public static int GetHeight(this ref BoardBuilder_MUST_BE_DISPOSED builder, int x)
        {
            for (int y = 0; y < builder.height; y++)
                if (!builder.DoesExist(x, y))
                    return y;
            return builder.height;
        }

        public static ref BoardBuilder_MUST_BE_DISPOSED Remove(this ref BoardBuilder_MUST_BE_DISPOSED builder, int x, int y)
        {
            if (x >= builder.width || y >= builder.height) return ref builder;
            int size = builder.height - 1 - y;
            if (size != 0)
            {
                var dest = builder.cards + x * builder.height + y;
                UnsafeUtility.MemMove(dest, dest + 1, size);
            }
            builder.cards[(x + 1) * builder.height - 1] = CalcIndexHelper.NOT_REMOVE_CARD_NUMBER;
            return ref builder;
        }

        public static ref BoardBuilder_MUST_BE_DISPOSED Shrink(this ref BoardBuilder_MUST_BE_DISPOSED builder) => ref builder.Shrink(builder.width, builder.GetMaxHeight());

        public static int GetMaxHeight(this ref BoardBuilder_MUST_BE_DISPOSED builder)
        {
            var maxHeight = 0;
            for (int x = builder.width; --x >= 0;)
            {
                var height = builder.GetHeight(x);
                if (height > maxHeight)
                    maxHeight = height;
            }
            return maxHeight;
        }

        public static ref BoardBuilder_MUST_BE_DISPOSED Shrink(this ref BoardBuilder_MUST_BE_DISPOSED builder, int width, int height)
        {
            if (builder.length == width * height) return ref builder;
            if (width == 0 || height == 0)
            {
                builder.Dispose();
                return ref builder;
            }
            var newBuilder = new BoardBuilder_MUST_BE_DISPOSED(width * height, height);
            UnsafeUtility.MemCpyStride(newBuilder.cards, height, builder.cards, builder.height, height, width);
            builder.Dispose();
            builder = newBuilder;
            return ref builder;
        }

        public static ref BoardBuilder_MUST_BE_DISPOSED FromBoard<T>(this ref BoardBuilder_MUST_BE_DISPOSED builder, T board) where T : IBoard
        {
            int width = builder.width < board.Width ? board.Width : builder.width;
            int height = builder.height < board.Height ? board.Height : builder.height;
            builder.EnlargeWidthHeight(width, height);
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    builder[x, y] = board[x, y];
            return ref builder;
        }
    }
}
