using System.Collections;

namespace CoreElements;

public class ElementList<T> : IEnumerable<T>, IList<T>, ICollection<T>
{
    private T[] items;
    private int count;
    private int currentSize;
    private const int DefaultInitialSize = 16;

    public ElementList(T[] values)
    {
        count = values.Length-1;
        currentSize = values.Length;
        // settings the items value to values is much faster
        // (about 10 times faster for a 100 element array), but then the
        // original array is referenced. By copying the values, the original
        // array is preserved.
        items = new T[currentSize];
        Array.Copy(values, 0, items,0, currentSize);
    }

    public ElementList(int capacity)
    {
        count = -1;
        currentSize = capacity;
        items = new T[currentSize];
    }

    public ElementList()
    {
        count = -1;
        currentSize = DefaultInitialSize;
        items = new T[currentSize];
    }

    public T[] Items => items[..(count+1)];

    public void Add(T item)
    {
        var writePosition = ++count;
        if (writePosition >= currentSize)
        {
            var newDataLength = (currentSize + 1)<<1;

            var newData = new T[newDataLength];
            Array.Copy(items, 0, newData, 0, currentSize);
            items = newData;
            currentSize = newDataLength-1;
            
        }
        items[writePosition] = item;
    }

    public void Clear()
    {
        count = -1;
    }
    public bool Contains(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        for (var i = 0; i < count; i++)
        {
            if (item.Equals(items[i])) return true;
        }
        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        var targetPosition = arrayIndex;
        if (targetPosition > count) throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Starting position is out of bounds.");
        var endPosition = targetPosition + array.Length;
        if (endPosition > count) endPosition = count;
        var index = 0;
        for (var i = targetPosition; i <= endPosition; i++)
        {
            items[i] = array[index++];
        }
    }

    public bool Remove(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        var index = -1;
        for (var i = 0; i < count; i++)
        {
            if (item.Equals(items[i]))
            {
                index = i;
                break;
            };
        }
        if (index < 0)
        {
            return false;
        }
        var newCount = count - 1;
        for (var i = index; i < newCount; i++)
        {
            items[i] = items[i + 1];
        }
        count = newCount;
        return true;
    }

    public IEnumerator<T> GetEnumerator() => new ElementEnumerator<T>(items, count);
    IEnumerator IEnumerable.GetEnumerator() => new ElementEnumerator<T>(items, count);

    public int IndexOf(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        for (var i = 0; i < count; i++)
        {
            if (item.Equals(items[i]))
            {
                return i;
            };
        }
        return -1;
    }

    public void Insert(int index, T item)
    {
        if (index < 0 || index > count) throw new ArgumentOutOfRangeException(nameof(index));
        items[index] = item;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index > count) throw new ArgumentOutOfRangeException(nameof(index));
        var newCount = count -1;
        for (var i = index; i <= newCount; i++)
        {
            items[i] = items[i + 1];
        }
        count = newCount;
    }

    public int Count => count+1;
    public bool IsReadOnly => false;

    public T this[int index]
    {
        get
        {
            if (index < 0 || index > count) return default!;
            return items[index];
        }
        set
        {
            if (index < 0 || index > count) return;
            items[index] = value;
        }
    }
}