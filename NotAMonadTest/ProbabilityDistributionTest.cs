using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Numerics;

[TestClass]
public class ProbabilityDistributionTest {
    [TestMethod]
    public void TestWrap() {
        var s = ProbabilityDistributionMonad.Wrap("test");
        s.Dictionary.Single().Key.AssertEquals("test");
        s.Dictionary.Single().Value.AssertEquals(BigRational.One);
    }
    
    [TestMethod]
    public void TestTransform() {
        var s = new ProbabilityDistributionMonad<string>(new Dictionary<string, BigRational> {
            {"hey", BigRational.One/2},
            {"listen", BigRational.One/4},
            {"die", BigRational.One/4}
        }).Transform(e => e.Length);

        s.Dictionary.Count.AssertEquals(2);
        s.Dictionary.ContainsKey(3).AssertTrue();
        s.Dictionary.ContainsKey(6).AssertTrue();

        s.Dictionary[3].AssertEquals(BigRational.One / 4 * 3);
        s.Dictionary[6].AssertEquals(BigRational.One / 4);
    }
    
    [TestMethod]
    public void TestFlatten() {
        var s = ProbabilityDistributionMonad.Wrap(ProbabilityDistributionMonad.Wrap("test")).Flatten();
        s.Dictionary.Single().Key.AssertEquals("test");
        s.Dictionary.Single().Value.AssertEquals(BigRational.One);

        var a1 = new ProbabilityDistributionMonad<string>(new Dictionary<string, BigRational> {
            {"a", BigRational.One/7},
            {"bra", BigRational.One/7*2},
            {"ca", BigRational.One/7*4}
        });
        var a2 = new ProbabilityDistributionMonad<string>(new Dictionary<string, BigRational> {
            {"da", BigRational.One/3},
            {"bra", BigRational.One/3*2},
        });
        var s2 = new ProbabilityDistributionMonad<ProbabilityDistributionMonad<string>>(new Dictionary<ProbabilityDistributionMonad<string>, BigRational> {
            {a1, BigRational.One/5},
            {a2, BigRational.One/5*4}
        }).Flatten();

        s2.Dictionary.Count.AssertEquals(4);
        s2.Dictionary["a"].AssertEquals(BigRational.One / 105 * 3);
        s2.Dictionary["bra"].AssertEquals(BigRational.One / 105 * 62);
        s2.Dictionary["ca"].AssertEquals(BigRational.One / 105 * 12);
        s2.Dictionary["da"].AssertEquals(BigRational.One / 105 * 28);
    }
}
