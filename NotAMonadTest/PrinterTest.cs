using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

[TestClass]
public class PrinterTest {
    [TestMethod]
    public void TestWrap() {
        var s = PrinterMonad.Wrap("test");

        // note: AWKWARD
        // run test and watch debug prompt to see if working
        // should print 'test' three times
        s("input");
        s("test");
        s("a");
    }
    
    [TestMethod]
    public void TestTransform() {
        var r = new List<string>();
        PrinterMonad<string> p = r.Add;

        // note: BROKEN, goes backwards from string to int, instead of forward from int to string
        var q = p.Transform<int, string>(e => e+"");
        r.SequenceEqual(new string[0]).AssertTrue();
        q(5);
        r.SequenceEqual(new[] {"5"}).AssertTrue();
        q(8);
        r.SequenceEqual(new[] { "5", "8" }).AssertTrue();
    }
    
    [TestMethod]
    public void TestFlatten() {
        var r = new List<PrinterMonad<int>>();
        PrinterMonad<PrinterMonad<int>> p = r.Add;
        var q = p.Flatten();

        // note: AWKWARD
        // can't really verify what the printers are, just that they were made and inserted
        r.Count.AssertEquals(0);
        q(5);
        r.Count.AssertEquals(1);
        q(6);
        r.Count.AssertEquals(2);
    }
}
