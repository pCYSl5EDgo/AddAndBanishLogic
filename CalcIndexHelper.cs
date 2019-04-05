using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace AddAndBanish
{
    public static class CalcIndexHelper
    {
        public const sbyte NOT_REMOVE_CARD_NUMBER = sbyte.MaxValue;

        public static unsafe void MemClear(this sbyte[] array)
        {
            fixed (sbyte* ptr = &array[0])
            {
                MemClear(ptr, array.Length);
            }
        }

        public static unsafe void MemClear(sbyte* ptr, int length)
        {
            var value = NOT_REMOVE_CARD_NUMBER;
            UnsafeUtility.MemCpyReplicate(ptr, &value, 1, length);
        }
    }
}