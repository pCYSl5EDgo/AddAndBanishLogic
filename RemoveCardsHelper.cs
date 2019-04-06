namespace AddAndBanish
{
    internal static class RemoveCardsHelper
    {
        internal static bool IsInvalidRemoveCandidate(this in RemoveCards removeCards, int x, int y, sbyte[] boardCards, int goal) => removeCards.DoesExist(x, y) || boardCards[x * removeCards.Height + y] + removeCards.Sum > goal;
        internal static bool IsInvalidRemoveCandidate<TBoard>(this IRemoveCards<TBoard> removeCards, int x, int y, sbyte[] boardCards, int goal)
            where TBoard : struct, IBoard
        => removeCards[x, y] || boardCards[x * removeCards.Height + y] + removeCards.Sum > goal;

        internal static bool Is_AnyOf4NeighborCards_RemoveCard(this in RemoveCards removeCards, int x, int y)
        => (x > 0 && removeCards.DoesExist(x - 1, y)) ||
        (x + 1 < removeCards.Width && removeCards.DoesExist(x + 1, y)) ||
        (y > 0 && removeCards.DoesExist(x, y - 1)) ||
        (y + 1 < removeCards.Height && removeCards.DoesExist(x, y + 1));

        internal static bool Is_AnyOf4NeighborCards_RemoveCard<TRemoveCards>(this ref TRemoveCards removeCards, int x, int y)
            where TRemoveCards : struct, IRemoveCards
        => (x > 0 && removeCards[x - 1, y]) ||
        (x + 1 < removeCards.Width && removeCards[x + 1, y]) ||
        (y > 0 && removeCards[x, y - 1]) ||
        (y + 1 < removeCards.Height && removeCards[x, y + 1]);

        public static bool IsHamilton<T>(this ref T removeCards)
            where T : struct, IRemoveCards
        {
            int oneConnection = 0;
            for (int x = 0, width = removeCards.Width; x < width; x++)
            {
                for (int y = 0, height = removeCards.Height; y < height; y++)
                {
                    if (!removeCards[x, y]) continue;
                    switch (removeCards.GetConnection(x, y))
                    {
                        case 0:
                            return false;
                        case 1:
                            if (++oneConnection > 2)
                                return false;
                            break;
                        default:
                            break;
                    }
                }
            }
            return true;
        }

        public static int GetConnection<T>(this ref T removeCards, int x, int y)
            where T : struct, IRemoveCards
        {
            var Width = removeCards.Width;
            var Height = removeCards.Height;
            return (x > 0 && removeCards[x - 1, y] ? 1 : 0) + (x < Width - 1 && removeCards[x + 1, y] ? 1 : 0) + (y > 0 && removeCards[x, y - 1] ? 1 : 0) + (y < Height - 1 && removeCards[x, y + 1] ? 1 : 0);
        }
    }
}