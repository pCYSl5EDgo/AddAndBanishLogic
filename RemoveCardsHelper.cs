namespace AddAndBanish
{
    internal static class RemoveCardsHelper
    {
        internal static bool IsInvalidRemoveCandidate(this in RemoveCards removeCards, int x, int y, sbyte[] boardCards, int goal) => removeCards.DoesExist(x, y) || boardCards[x * removeCards.Height + y] + removeCards.Sum > goal;
        internal static bool IsInvalidRemoveCandidate(this IRemoveCards removeCards, int x, int y, sbyte[] boardCards, int goal) => removeCards.DoesExist(x, y) || boardCards[x * removeCards.Height + y] + removeCards.Sum > goal;

        internal static bool Is_AnyOf4NeighborCards_RemoveCard(this in RemoveCards removeCards, int x, int y)
        => (x > 0 && removeCards.DoesExist(x - 1, y)) ||
        (x + 1 < removeCards.Width && removeCards.DoesExist(x + 1, y)) ||
        (y > 0 && removeCards.DoesExist(x, y - 1)) ||
        (y + 1 < removeCards.Height && removeCards.DoesExist(x, y + 1));

        internal static bool Is_AnyOf4NeighborCards_RemoveCard<T>(this ref T removeCards, int x, int y) where T : struct, IRemoveCards
        => (x > 0 && removeCards.DoesExist(x - 1, y)) ||
        (x + 1 < removeCards.Width && removeCards.DoesExist(x + 1, y)) ||
        (y > 0 && removeCards.DoesExist(x, y - 1)) ||
        (y + 1 < removeCards.Height && removeCards.DoesExist(x, y + 1));
    }
}