using System.Collections;

namespace MarkovNameGenerator.Util;

/// <summary>
/// A collection that contains no duplicate elements. The underlying data structure is a List.
/// Elements are compared using the standard equality operator.
/// </summary>
/// <typeparam name="T">The type of elements in the set.</typeparam>
public class ArraySet<T> : IEnumerable<T>
{
    private readonly List<T> _items;

    /// <summary>
    /// Gets the number of elements in the set.
    /// </summary>
    public int Length => _items.Count;

    /// <summary>
    /// Creates an empty set.
    /// </summary>
    public ArraySet()
    {
        _items = new List<T>();
    }

    /// <summary>
    /// Creates a set from an enumerable collection.
    /// </summary>
    /// <param name="items">The items to add to the set (duplicates will be removed).</param>
    public ArraySet(IEnumerable<T> items)
    {
        _items = new List<T>();
        foreach (var item in items)
        {
            Add(item);
        }
    }

    /// <summary>
    /// Creates a set from an array.
    /// </summary>
    /// <param name="array">The array to convert to a set (duplicates will be removed).</param>
    /// <returns>The new ArraySet.</returns>
    public static ArraySet<T> Create(T[]? array = null)
    {
        if (array == null)
        {
            return new ArraySet<T>();
        }
        return new ArraySet<T>(array);
    }

    /// <summary>
    /// Attempts to add an element to the set.
    /// Succeeds if the element is not already in the set, fails if it was in the set.
    /// </summary>
    /// <param name="element">The element to add to the set.</param>
    /// <returns>True if the element was not present, false if it was already present.</returns>
    public bool Add(T element)
    {
        ArgumentNullException.ThrowIfNull(element);

        if (Contains(element))
        {
            return false;
        }

        _items.Add(element);
        return true;
    }

    /// <summary>
    /// Checks if an element is contained within the set.
    /// </summary>
    /// <param name="element">The element to search the set for.</param>
    /// <returns>True if the element is present, false if it is not present.</returns>
    public bool Contains(T element)
    {
        return _items.Contains(element);
    }

    /// <summary>
    /// Removes an element from the set.
    /// </summary>
    /// <param name="element">The element to remove.</param>
    /// <returns>True if the element was removed, false if it was not present.</returns>
    public bool Remove(T element)
    {
        return _items.Remove(element);
    }

    /// <summary>
    /// Returns the index of the first occurrence of the element in the set.
    /// </summary>
    /// <param name="element">The element to find.</param>
    /// <returns>The zero-based index of the element, or -1 if not found.</returns>
    public int IndexOf(T element)
    {
        return _items.IndexOf(element);
    }

    /// <summary>
    /// Returns the index of the last occurrence of the element in the set.
    /// </summary>
    /// <param name="element">The element to find.</param>
    /// <returns>The zero-based index of the element, or -1 if not found.</returns>
    public int LastIndexOf(T element)
    {
        return _items.LastIndexOf(element);
    }

    /// <summary>
    /// Sorts the elements in the set using the default comparer.
    /// </summary>
    public void Sort()
    {
        _items.Sort();
    }

    /// <summary>
    /// Sorts the elements in the set using the specified comparison.
    /// </summary>
    /// <param name="comparison">The comparison to use when comparing elements.</param>
    public void Sort(Comparison<T> comparison)
    {
        _items.Sort(comparison);
    }

    /// <summary>
    /// Reverses the order of elements in the set.
    /// </summary>
    public void Reverse()
    {
        _items.Reverse();
    }

    /// <summary>
    /// Removes and returns the element at the end of the set.
    /// </summary>
    /// <returns>The element that was removed.</returns>
    public T? Pop()
    {
        if (_items.Count == 0) return default;

        var item = _items[^1];
        _items.RemoveAt(_items.Count - 1);
        return item;
    }

    /// <summary>
    /// Removes and returns the element at the beginning of the set.
    /// </summary>
    /// <returns>The element that was removed.</returns>
    public T? Shift()
    {
        if (_items.Count == 0) return default;

        var item = _items[0];
        _items.RemoveAt(0);
        return item;
    }

    /// <summary>
    /// Returns a new set containing the intersection of two sets.
    /// </summary>
    /// <param name="set">The set to intersect with this set.</param>
    /// <returns>The intersection of this set and the given set.</returns>
    /// <example>Intersection([A, B, C], [B, C, D]) => [B, C]</example>
    public ArraySet<T> Intersection(ArraySet<T> set)
    {
        var result = new ArraySet<T>();
        foreach (var element in _items)
        {
            if (set.Contains(element))
            {
                result.Add(element);
            }
        }
        return result;
    }

    /// <summary>
    /// Returns a new set containing the union of two sets.
    /// </summary>
    /// <param name="set">The set to unify with this set.</param>
    /// <returns>The union of this set and the given set.</returns>
    /// <example>Union([A, B, C], [B, C, D]) => [A, B, C, D]</example>
    public ArraySet<T> Union(ArraySet<T> set)
    {
        var result = new ArraySet<T>(_items);
        foreach (var element in set)
        {
            result.Add(element);
        }
        return result;
    }

    /// <summary>
    /// Returns a new set containing the union of the set and array.
    /// </summary>
    /// <param name="array">The array to unify with this set.</param>
    /// <returns>The union of this set and the given array.</returns>
    /// <example>Union([A, B, C], [B, C, D]) => [A, B, C, D]</example>
    public ArraySet<T> Union(T[] array)
    {
        var result = new ArraySet<T>(_items);
        foreach (var element in array)
        {
            result.Add(element);
        }
        return result;
    }

    /// <summary>
    /// Returns a new set containing the difference of two sets.
    /// </summary>
    /// <param name="set">The set to difference with this set.</param>
    /// <returns>The difference of this set and the given set.</returns>
    /// <example>Difference([A, B, C], [B, C, D]) => [A]</example>
    public ArraySet<T> Difference(ArraySet<T> set)
    {
        var result = Copy();
        foreach (var element in set)
        {
            result.Remove(element);
        }
        return result;
    }

    /// <summary>
    /// Copies the set.
    /// </summary>
    /// <returns>A shallow copy of the original set.</returns>
    public ArraySet<T> Copy()
    {
        return new ArraySet<T>(_items);
    }

    /// <summary>
    /// Creates a slice of the set.
    /// </summary>
    /// <param name="start">The inclusive start index.</param>
    /// <param name="end">The exclusive end index.</param>
    /// <returns>The requested slice of the set.</returns>
    public ArraySet<T> Slice(int start, int? end = null)
    {
        int actualEnd = end ?? _items.Count;
        var sliced = _items.GetRange(start, actualEnd - start);
        return new ArraySet<T>(sliced);
    }

    /// <summary>
    /// Converts the set into an array.
    /// </summary>
    /// <returns>A shallow copy of the set as an array.</returns>
    public T[] ToArray()
    {
        return _items.ToArray();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the set.
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
