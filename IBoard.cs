using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

namespace AddAndBanish
{
    public interface IBoard : IEquatable<IBoard>
    {
        int Height { get; }
        int Width { get; }
        int Length { get; }
        sbyte this[int x, int y] { get; }
        IBoard Clone();
        bool IsMultipleOfArgument(int goal);
        bool DoesExist(int x, int y);
    }
}