namespace u4.Core;

public class FastList<T>
{
    private uint _newArrayLength;
    
    public T[] Array;

    public uint Length;

    public FastList(uint capacity = 0)
    {
        Length = capacity;

        if (capacity == 0)
        {
            Array = [];
            _newArrayLength = 1;
        }
        else
        {
            Array = new T[capacity];
            _newArrayLength = capacity << 1;
        }
    }

    public void Add(T item)
    {
        if (Length + 1 >= Array.Length)
        {
            System.Array.Resize(ref Array, (int) _newArrayLength);
            _newArrayLength <<= 1;
        }

        Array[Length++] = item;
    }
}