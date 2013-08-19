using System;
using System.Diagnostics;

/// <summary>
/// Prints a value to some place in some way.
/// </summary>
public delegate void PrinterMonad<T>(T value);

public static class PrinterMonad {
    /// <summary>
    /// Returns a printer that always prints the given value to the debug prompt.
    /// 
    /// AWKWARD: The same value is always printed, regardless of what input value is given to the printer at the time.
    /// AWKWARD: Why the debug prompt?
    /// </summary>
    public static PrinterMonad<T> Wrap<T>(T value) {
        return input => Debug.WriteLine(""+value);
    }

    /// <summary>
    /// Returns a parser that uses the given parser but transforms its resulting value with the given transformation.
    /// 
    /// BROKEN: The transformation goes backwards! The resulting printer expects values of type TIn instead of TOut.
    /// </summary>
    public static PrinterMonad<TIn> Transform<TIn, TOut>(this PrinterMonad<TOut> printer, Func<TIn, TOut> transformation) {
        return input => printer(transformation(input));
    }

    /// <summary>
    /// Returns a printer that transforms values into a printer then prints that printer with the given printer printer.
    /// 
    /// AWKWARD: The intermediate printer inherits awkwardness from Wrap's awkwardness.
    /// </summary>
    public static PrinterMonad<T> Flatten<T>(this PrinterMonad<PrinterMonad<T>> printerOfPrinter) {
        return input => printerOfPrinter(Wrap(input));
    }
}
