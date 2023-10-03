namespace CoreElements;

public class InterlockedValue<T>
    where T : struct
{
    public InterlockedValue(T initValue)
    {
        Value = initValue;
    }
    public bool UpdateValue(T oldValue, T value)
    {
        var result = false;
        {
            if (Value.Equals(oldValue))
            {
                Value = value;
                result = true;
            }
        }
        return result;
    }
    public T Value { get; private set; }
}
