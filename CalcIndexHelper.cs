using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace AddAndBanish
{
    internal static class CalcIndexHelper
    {
        internal const sbyte NOT_REMOVE_CARD_NUMBER = sbyte.MaxValue;

        internal static unsafe void MemClear(this sbyte[] array)
        {
            fixed (sbyte* ptr = &array[0])
            {
                var value = NOT_REMOVE_CARD_NUMBER;
                UnsafeUtility.MemCpyReplicate(ptr, &value, 1, array.Length);
            }
        }
    }
}