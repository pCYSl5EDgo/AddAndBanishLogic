using System;
using System.Collections.Generic;

namespace AddAndBanish
{
    public interface IRemoveCards : IEquatable<IRemoveCards>, IEnumerable<(int x, int y)>
    {
        int Length { get; }
        int Width { get; }
        int Height { get; }
        int Count { get; }
        int Sum { get; }
        IEnumerable<Scard> GetNeighborEnumerable(IBoard board, int goal);
        IRemoveCards Add(int x, int y, sbyte number);
        int GetConnection(int x, int y);
        bool IsValid(int goal);
        bool IsHamilton { get; }

        bool this[int x, int y] { get; }
    }
    public interface IRemoveCards<TBoard> : IEquatable<IRemoveCards<TBoard>>, IRemoveCards
        where TBoard : struct, IBoard
    {
        TBoard CalcNextStepByRemove(TBoard board);
        new IRemoveCards<TBoard> Add(int x, int y, sbyte number);
    }
}