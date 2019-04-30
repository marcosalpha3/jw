namespace SystemNet.Practice.Common.Values
{
    public class IntValues
    {
        public static bool IsNullorDefault(int? value)
        {
            return (value == 0 || value == int.MinValue) ? true : false;
        }
    }
}
