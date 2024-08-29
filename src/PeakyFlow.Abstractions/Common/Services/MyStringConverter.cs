using PeakyFlow.Abstractions.Common.Interfaces;

namespace PeakyFlow.Abstractions.Common.Services
{
    public class MyStringConverter : IStringConverter
    {
        public T ToValue<T>(string rawVal, T defaultValue = default)
            where T : struct
        {
            if (rawVal != null && typeof(T).IsEnum)
            {
                Enum.TryParse<T>(rawVal, true, out var result);
                return result;
            }
            else if (rawVal != null && TryConvertValue(rawVal, out T convertedValue))
            {
                return convertedValue;
            }
            return defaultValue;
        }

        private bool TryConvertValue<T>(string value, out T convertedValue)
            where T : struct
        {
            try
            {
                convertedValue = (T)Convert.ChangeType(value, typeof(T));
                return true;
            }
            catch (Exception)
            {
                convertedValue = default;
                return false;
            }
        }
    }
}
