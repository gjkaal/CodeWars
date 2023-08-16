namespace CoreElements;

public static class ArrayHelpers
{
    public static ElementList<T> ToElementList<T>(this T[] values)
    {
        return new ElementList<T>(values);
    }

    public static T[] ToArray<T>(this ElementList<T> value)
    {
        return value.Items;
    }
}
