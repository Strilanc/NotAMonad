using System;
using System.Collections.Generic;
using Numerics;
using System.Linq;

public sealed class ProbabilityDistributionMonad<T> {
    public readonly IReadOnlyDictionary<T, BigRational> Dictionary;
    public ProbabilityDistributionMonad(IReadOnlyDictionary<T, BigRational> dictionary) {
        this.Dictionary = dictionary;

        // well-formed distributions must add up to 100%:
        var totalProbability = dictionary.Values.Aggregate(BigRational.Zero, (a, e) => a + e);
        if (totalProbability != 1) throw new ArgumentOutOfRangeException("dictionary", "Probabilities must add up to 1.");
    }
}

public static class ProbabilityDistributionMonad {
    /// <summary>
    /// Returns a probability distribution where the given value is the single 100% likely possibility.
    /// </summary>
    public static ProbabilityDistributionMonad<T> Wrap<T>(T value) {
        var result = new Dictionary<T, BigRational>();
        result.Add(value, 1);
        return new ProbabilityDistributionMonad<T>(result);
    }

    /// <summary>
    /// Returns a probability distribution over the result of drawing an input from the given distribution, 
    /// then running that input through the given transformation.
    /// 
    /// When two distinct inputs are merged into the same output by the transformation, the probability of the output is the sum of the inputs' probabilities.
    /// </summary>
    public static ProbabilityDistributionMonad<TOut> Transform<TIn, TOut>(this ProbabilityDistributionMonad<TIn> distribution, Func<TIn, TOut> transformation) {
        var result = new Dictionary<TOut, BigRational>();
        foreach (var item in distribution.Dictionary) {
            var transformed = transformation(item.Key);
            if (!result.ContainsKey(transformed)) {
                result.Add(transformed, 0);
            }
            result[transformed] += item.Value;
        }
        return new ProbabilityDistributionMonad<TOut>(result);
    }

    /// <summary>
    /// Returns a probability distribution over the result of drawing an intermediate distribution from the given distribution of distributions,
    /// then drawing an item from that intermediate distribution.
    /// 
    /// The output probability of an item is its probability times the probability of its distribution.
    /// When an item appears in multiple intermediate distributions, the corresponding probabilities are added.
    /// </summary>
    public static ProbabilityDistributionMonad<T> Flatten<T>(this ProbabilityDistributionMonad<ProbabilityDistributionMonad<T>> distributionOfDistributions) {
        var result = new Dictionary<T, BigRational>();
        foreach (var distribution in distributionOfDistributions.Dictionary) {
            foreach (var item in distribution.Key.Dictionary) {
                if (!result.ContainsKey(item.Key)) {
                    result.Add(item.Key, 0);
                }
                result[item.Key] += item.Value * distribution.Value;
            }
        }
        return new ProbabilityDistributionMonad<T>(result);
    }
}
