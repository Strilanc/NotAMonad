using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ParserTest {
    [TestMethod]
    public void TestWrap() {
        var s = ParserMonad.Wrap("test");
        s("whatever").AssertEquals(Tuple.Create("test", "whatever"));
    }
    
    [TestMethod]
    public void TestTransform() {
        ParserMonad<int> p = text => Tuple.Create(int.Parse(text.Substring(0, 5)), text.Substring(5));
        var q = p.Transform(e => e*e);
        q("00005wonder").AssertEquals(Tuple.Create(25, "wonder"));
    }
    
    [TestMethod]
    public void TestFlatten() {
        ParserMonad<ParserMonad<int>> p = text => {
            var first = text[0];
            if (first == 'a') {
                return Tuple.Create<ParserMonad<int>, string>(
                    rest => Tuple.Create(int.Parse(rest.Substring(0, 5)), rest.Substring(5)),
                    text.Substring(1));
            }
            return Tuple.Create(ParserMonad.Wrap(-1), text.Substring(1));
        };
        var q = p.Flatten();

        q("a00004x").AssertEquals(Tuple.Create(4, "x"));
        q("b00004x").AssertEquals(Tuple.Create(-1, "00004x"));
    }
}
