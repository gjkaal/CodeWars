using System.Collections;

namespace CoreElements;

public class ElementEnumerator<T> : IEnumerator<T>
{
    private readonly T[] items;
    private int position;
    private readonly int count;
    public ElementEnumerator(T[] items, int count)
    {
        this.count = count;
        this.items = items;
    }

    public void Dispose() => throw new NotImplementedException();
    public bool MoveNext()
    {
        position++;
        return (position < count);
    }

    public void Reset()
    {
        position = -1;
    }

    object IEnumerator.Current
    {
        get
        {
            return Current!;
        }
    }

    public T Current
    {
        get
        {
            try
            {
                return items[position];
            }
            catch (IndexOutOfRangeException)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
