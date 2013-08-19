using System;

/// <summary>
/// Parses a value out of the given text, and returns the value as well as any remaining text.
/// </summary>
public delegate Tuple<T, string> ParserMonad<T>(string text);

public static class ParserMonad {
    /// <summary>
    /// Returns a parser that outputs the given value and consumes no text.
    /// </summary>
    public static ParserMonad<T> Wrap<T>(T value) {
        return text => Tuple.Create(value, text);
    }

    /// <summary>
    /// Returns a parser that uses the given parser but transforms its resulting value with the given transformation.
    /// </summary>
    public static ParserMonad<TOut> Transform<TIn, TOut>(this ParserMonad<TIn> parser, Func<TIn, TOut> transformation) {
        return text => {
            var mid = parser(text);
            var value = mid.Item1;
            var remainingText = mid.Item2;
            var result = transformation(value);
            return Tuple.Create(result, remainingText);
        };
    }

    /// <summary>
    /// Returns a parser that uses the given parser to parse an intermediate parser, 
    /// then immediately applies that intermediate parser to the remaining text,
    /// then returns the resulting value and final remaining text.
    /// </summary>
    public static ParserMonad<T> Flatten<T>(this ParserMonad<ParserMonad<T>> parserOfParser) {
        return text => {
            var mid = parserOfParser(text);
            var parser = mid.Item1;
            var remainingText = mid.Item2;
            return parser(remainingText);
        };
    }
}
