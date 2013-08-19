using System;

// NOT a monad
public sealed class SquareMatrix<T> {
    public readonly T[,] Items;
    public int Span { get { return Items.GetLength(0); } }
    public SquareMatrix(T[,] items) {
        if (items.GetLength(0) != items.GetLength(1)) throw new ArgumentException("Not square");
        Items = items;
    }
}

public static class SquareMatrix {
    /// <summary>
    /// Returns a square matrix containing a single value: the given value.
    /// </summary>
    public static SquareMatrix<T> Wrap<T>(T value) {
        var result = new T[1,1];
        result[0, 0] = value;
        return new SquareMatrix<T>(result);
    }

    /// <summary>
    /// Returns a square matrix created by running all of the items in the given square matrix through a transformation function.
    /// </summary>
    public static SquareMatrix<TOut> Transform<TIn, TOut>(this SquareMatrix<TIn> squareMatrix, Func<TIn, TOut> transformation) {
        var result = new TOut[squareMatrix.Span,squareMatrix.Span];
        
        for (var i = 0; i < squareMatrix.Span; i++) {
            for (var j = 0; j < squareMatrix.Span; j++) {
                result[i, j] = transformation(squareMatrix.Items[i, j]);
            }
        }

        return new SquareMatrix<TOut>(result);
    }

    /// <summary>
    /// Returns a square matrix containing the items inside the square matrices inside a square matrix.
    /// The items are ordered like you'd expect: by outer position then by inner position.
    /// 
    /// BROKEN - When inner matrices are of different sizes, there may not be a square number of items.
    /// </summary>
    public static SquareMatrix<T> Flatten<T>(this SquareMatrix<SquareMatrix<T>> squareMatrixOfSquareMatrices) {
        var outerSpan = squareMatrixOfSquareMatrices.Span;
        if (outerSpan == 0) return new SquareMatrix<T>(new T[0,0]);
        if (outerSpan == 1) return squareMatrixOfSquareMatrices.Items[0, 0];
        var innerSpan = squareMatrixOfSquareMatrices.Items[0, 0].Span;
        var result = new T[outerSpan*innerSpan,outerSpan*innerSpan];

        for (var outerI = 0; outerI < outerSpan; outerI++) {
            for (var outerJ = 0; outerJ < outerSpan; outerJ++) {
                var innerMatrix = squareMatrixOfSquareMatrices.Items[outerI, outerJ];
                if (innerMatrix.Span != innerSpan) {
                    throw new InvalidOperationException("Don't know how to handle cases involving differing inner spans.");
                }
                var x = outerI*innerSpan;
                var y = outerJ*innerSpan;
                for (var innerI = 0; innerI < innerSpan; innerI++) {
                    for (var innerJ = 0; innerJ < innerSpan; innerJ++) {
                        result[outerI*innerSpan + innerI, outerJ*innerSpan + innerJ] = squareMatrixOfSquareMatrices.Items[outerI, outerJ].Items[innerI, innerJ];
                    }
                }
            }
        }

        return new SquareMatrix<T>(result);
    }
}
