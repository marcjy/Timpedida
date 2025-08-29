using System.Collections.Generic;
using System.Linq;

public class RandomPicker<T>
{
    private T[] _items;
    private List<int> _unusedIndices;
    private System.Random _rng = new System.Random();

    public RandomPicker(T[] items)
    {
        _items = items;
        Reset();
    }

    public T GetNext()
    {
        if (_unusedIndices.Count == 0)
            Reset();

        int randIndex = _rng.Next(_unusedIndices.Count);
        int itemIndex = _unusedIndices[randIndex];
        _unusedIndices.RemoveAt(randIndex);

        return _items[itemIndex];
    }

    private void Reset()
    {
        _unusedIndices = Enumerable.Range(0, _items.Length).ToList();
    }
}

