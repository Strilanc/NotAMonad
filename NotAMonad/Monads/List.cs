using System;
using System.Collections.Generic;

public static class ListMonad {
    /// <summary>
    /// Returns a list containing a single value: the given value.
    /// </summary>
    public static List<T> Wrap<T>(T value) {
        var result = new List<T>();
        result.Add(value);
        return result;
    }

    /// <summary>
    /// Returns a list created by running all of the items in the given list through a transformation function.
    /// </summary>
    public static List<TOut> Transform<TIn, TOut>(this List<TIn> list, Func<TIn, TOut> transformation) {
        var result = new List<TOut>();
        foreach (var item in list) {
            result.Add(transformation(item));
        }
        return result;
    }

    /// <summary>
    /// Returns a list created by appending together all of the items in all of the lists in the given list.
    /// </summary>
    public static List<T> Flatten<T>(this List<List<T>> listOfLists) {
        var result = new List<T>();
        foreach (var list in listOfLists) {
            foreach (var item in list) {
                result.Add(item);
            }
        }
        return result;
    }
}
