namespace PeakyFlow.Abstractions.Common.Interfaces
{
    public interface IStringConverter
    {
        T ToValue<T>(string rawVal, T defaultValue = default)
            where T : struct;
    }
}
