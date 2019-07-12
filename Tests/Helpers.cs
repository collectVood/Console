namespace Tests
{
    public class Helpers
    {
        public static bool SequenceEquals<T>(T[] array1, T[] array2)
        {
            if (array1 == array2)
                return true;

            if (array1 == null || array2 == null)
                return false;

            if (array1.Length != array2.Length)
                return false;

            for (var i = 0; i < array1.Length; i++)
            {
                if (!Equals(array1[i], array2[i]))
                    return false;
            }

            return false;
        }
    }
}