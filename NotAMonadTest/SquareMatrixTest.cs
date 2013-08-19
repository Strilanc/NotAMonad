using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class SquareMatrixTest {
    [TestMethod]
    public void TestWrap() {
        var s = SquareMatrix.Wrap("test");
        s.Span.AssertEquals(1);
        s.Items[0, 0].AssertEquals("test");
    }
    
    [TestMethod]
    public void TestTransform() {
        var r = new int[2,2];
        r[1, 0] = 2;
        r[0, 0] = 3;
        r[1, 1] = 5;
        r[0, 1] = 7;

        var s = new SquareMatrix<int>(r).Transform(e => e*e);
        s.Items[1, 0].AssertEquals(4);
        s.Items[0, 0].AssertEquals(9);
        s.Items[1, 1].AssertEquals(25);
        s.Items[0, 1].AssertEquals(49);
    }
    
    [TestMethod]
    public void TestFlatten() {
        var s1 = SquareMatrix.Wrap(SquareMatrix.Wrap("test")).Flatten();
        s1.Span.AssertEquals(1);
        s1.Items[0, 0].AssertEquals("test");

        var s2 = new SquareMatrix<SquareMatrix<int>>(new SquareMatrix<int>[0,0]).Flatten();
        s2.Span.AssertEquals(0);

        var r3 = new SquareMatrix<int>[1,1];
        r3[0, 0] = new SquareMatrix<int>(new int[0,0]);
        var s3 = new SquareMatrix<SquareMatrix<int>>(r3).Flatten();
        s3.Span.AssertEquals(0);

        var r4 = new int[2, 2];
        r4[1, 0] = 2;
        r4[0, 0] = 3;
        r4[1, 1] = 5;
        r4[0, 1] = 7;
        var s4 = new SquareMatrix<int>(r4).Transform(SquareMatrix.Wrap).Flatten();
        s4.Span.AssertEquals(2);
        s4.Items[1, 0].AssertEquals(2);
        s4.Items[0, 0].AssertEquals(3);
        s4.Items[1, 1].AssertEquals(5);
        s4.Items[0, 1].AssertEquals(7);

        var s5 = new SquareMatrix<int>(r4).Transform(e => s4).Flatten();
        s5.Span.AssertEquals(4);
        s5.Items[1, 0].AssertEquals(2);
        s5.Items[3, 0].AssertEquals(2);
        s5.Items[1, 2].AssertEquals(2);
        s5.Items[3, 2].AssertEquals(2);
        
        s5.Items[0, 0].AssertEquals(3);
        s5.Items[2, 0].AssertEquals(3);
        s5.Items[0, 2].AssertEquals(3);
        s5.Items[2, 2].AssertEquals(3);

        s5.Items[1, 1].AssertEquals(5);
        s5.Items[3, 1].AssertEquals(5);
        s5.Items[1, 3].AssertEquals(5);
        s5.Items[3, 3].AssertEquals(5);
        
        s5.Items[0, 1].AssertEquals(7);
        s5.Items[2, 1].AssertEquals(7);
        s5.Items[0, 3].AssertEquals(7);
        s5.Items[2, 3].AssertEquals(7);
    }

    [TestMethod]
    public void TestFlatten_Broken() {
        var r = new SquareMatrix<int>[2, 2];
        r[1, 0] = SquareMatrix.Wrap(1);
        r[0, 0] = SquareMatrix.Wrap(1);
        r[1, 1] = SquareMatrix.Wrap(1);
        r[0, 1] = new SquareMatrix<int>(new int[0,0]);

        var s = new SquareMatrix<SquareMatrix<int>>(r);

        TestingUtilities.AssertThrows(() => s.Flatten());
    }
}
