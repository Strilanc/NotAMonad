using System;
using System.Collections.Generic;
using Numerics;
using System.Linq;

// NOT a monad
public sealed class QuantumSuperPosition<T> {
    public readonly IReadOnlyDictionary<T, ComplexRational> Dictionary;
    public QuantumSuperPosition(IReadOnlyDictionary<T, ComplexRational> dictionary) {
        this.Dictionary = dictionary;

        // well-formed superpositions must add up to 100%:
        var totalProbability = dictionary.Values.Aggregate(BigRational.Zero, (a, e) => a + e.SquaredMagnitude);
        if (totalProbability != 1) throw new ArgumentOutOfRangeException("dictionary", "Squared magnitudes must add up to 1.");
    }
}

public static class QuantumSuperPosition {
    /// <summary>
    /// Returns a superposition where the given value is the single state and has an amplitude of 1.
    /// </summary>
    public static QuantumSuperPosition<T> Wrap<T>(T value) {
        var result = new Dictionary<T, ComplexRational>();
        result.Add(value, 1);
        return new QuantumSuperPosition<T>(result);
    }

    /// <summary>
    /// Returns a superposition over the result of drawing an input from the given superposition, 
    /// then running that input through the given transformation.
    /// 
    /// BROKEN - When distinct inputs are merged, they interfere. The interference breaks the squared magnitude constraint.
    /// </summary>
    public static QuantumSuperPosition<TOut> Transform<TIn, TOut>(this QuantumSuperPosition<TIn> superposition, Func<TIn, TOut> transformation) {
        var result = new Dictionary<TOut, ComplexRational>();
        foreach (var item in superposition.Dictionary) {
            var transformed = transformation(item.Key);
            if (!result.ContainsKey(transformed)) {
                result.Add(transformed, 0);
            }
            result[transformed] += item.Value;
        }
        return new QuantumSuperPosition<TOut>(result);
    }

    /// <summary>
    /// Returns a superposition over the result of drawing an intermediate superposition from the given superposition of superpositions,
    /// then drawing an item from that intermediate superposition.
    /// 
    /// BROKEN - When an item appears in multiple intermediate superpositions, the squared magnitude constraint may be violated.
    /// </summary>
    public static QuantumSuperPosition<T> Flatten<T>(this QuantumSuperPosition<QuantumSuperPosition<T>> superpositionOfSuperpositions) {
        var result = new Dictionary<T, ComplexRational>();
        foreach (var superposition in superpositionOfSuperpositions.Dictionary) {
            foreach (var item in superposition.Key.Dictionary) {
                if (!result.ContainsKey(item.Key)) {
                    result.Add(item.Key, 0);
                }
                result[item.Key] += item.Value * superposition.Value;
            }
        }
        return new QuantumSuperPosition<T>(result);
    }
}

public struct ComplexRational {
    public readonly BigRational RealPart;
    public readonly BigRational ImaginaryPart;
    public BigRational SquaredMagnitude { get { return RealPart*RealPart + ImaginaryPart*ImaginaryPart; } }
    public ComplexRational(BigRational realPart, BigRational imaginaryPart) : this() {
        RealPart = realPart;
        ImaginaryPart = imaginaryPart;
    }
    public static ComplexRational operator +(ComplexRational v1, ComplexRational v2) {
        return new ComplexRational(
            v1.RealPart + v2.RealPart, 
            v1.ImaginaryPart + v2.ImaginaryPart);
    }
    public static ComplexRational operator *(ComplexRational v1, ComplexRational v2) {
        return new ComplexRational(
            v1.RealPart*v2.RealPart - v1.ImaginaryPart*v2.ImaginaryPart,
            v1.RealPart*v2.ImaginaryPart + v1.ImaginaryPart*v2.RealPart);
    }
    public static implicit operator ComplexRational(int value) {
        return new ComplexRational(value, 0);
    }
    public static implicit operator ComplexRational(BigRational value) {
        return new ComplexRational(value, 0);
    }
    public override string ToString() {
        return string.Format("{0} + {1}i", RealPart, ImaginaryPart);
    }
}
