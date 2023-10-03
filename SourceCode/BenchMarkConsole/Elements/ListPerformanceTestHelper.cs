using System.Linq;
using CoreElements;

namespace BenchMarkConsole.Elements;
internal static class ListPerformanceTestHelper
{
    public static T[] CreateArray<T>(int count, Func<T> item)
    {
        var r = new T[count];
        for(int i = 0; i < count; i++)
        {
            r[i] = item();
        }
        return r;
    }

    public static List<T> CreateList<T>(int count, Func<T> item)
    {
        var r = new List<T>(count);
        for (int i = 0; i < count; i++)
        {
            r[i] = item();
        }
        return r;
    }

    public static List<T> CreateListWithAdd<T>(int count, Func<T> item)
    {
        var r = new List<T>();
        for (int i = 0; i < count; i++)
        {
            r.Add(item());
        }
        return r;
    }

    public static ElementList<T> CreateElementList<T>(int count, Func<T> item)
    {
        var r = new ElementList<T>(count);
        for (int i = 0; i < count; i++)
        {
            r[i] = item();
        }
        return r;
    }

    public static ElementList<T> CreateElementListWithAdd<T>(int count, Func<T> item)
    {
        var r = new ElementList<T>();
        for (int i = 0; i < count; i++)
        {
            r.Add(item());
        }
        return r;
    }
}
