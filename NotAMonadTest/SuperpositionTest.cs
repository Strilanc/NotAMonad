using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Numerics;

[TestClass]
public class SuperpositionTest {
    [TestMethod]
    public void TestWrap() {
        var s = QuantumSuperPosition.Wrap("test");
        s.Dictionary.Single().Key.AssertEquals("test");
        s.Dictionary.Single().Value.AssertEquals((ComplexRational)1);
    }

    [TestMethod]
    public void TestTransform() {
        var s = new QuantumSuperPosition<string>(new Dictionary<string, ComplexRational> {
            {"hey", BigRational.One/5*3},
            {"you", -BigRational.One/5*4}
        });
        
        TestingUtilities.AssertThrows(() => s.Transform(e => e.Length));
    }

    [TestMethod]
    public void TestTransform_Broken() {
        var s = new QuantumSuperPosition<string>(new Dictionary<string, ComplexRational> {
            {"hey", BigRational.One/5*3},
            {"listen", BigRational.One/5*4}
        }).Transform(e => e.Length);

        s.Dictionary.Count.AssertEquals(2);
        s.Dictionary.ContainsKey(3).AssertTrue();
        s.Dictionary.ContainsKey(6).AssertTrue();
        s.Dictionary[3].AssertEquals((ComplexRational)(BigRational.One / 5 * 3));
        s.Dictionary[6].AssertEquals((ComplexRational)(BigRational.One / 5 * 4));
    }
    
    [TestMethod]
    public void TestFlatten() {
        var s = QuantumSuperPosition.Wrap(QuantumSuperPosition.Wrap("test")).Flatten();
        s.Dictionary.Single().Key.AssertEquals("test");
        s.Dictionary.Single().Value.AssertEquals((ComplexRational)1);

        var a1 = new QuantumSuperPosition<string>(new Dictionary<string, ComplexRational> {
            {"hey", BigRational.One/5*3},
            {"listen", -BigRational.One/5*4}
        });
        var a2 = new QuantumSuperPosition<string>(new Dictionary<string, ComplexRational> {
            {"over", BigRational.One/5*3},
            {"there", -BigRational.One/5*4}
        });
        var s2 = new QuantumSuperPosition<QuantumSuperPosition<string>>(new Dictionary<QuantumSuperPosition<string>, ComplexRational> {
            {a1, BigRational.One/5*3},
            {a2, new ComplexRational(0, BigRational.One/5*4)}
        }).Flatten();

        s2.Dictionary.Count.AssertEquals(4);
        s2.Dictionary["hey"].AssertEquals(new ComplexRational(BigRational.One/25*9, 0));
        s2.Dictionary["listen"].AssertEquals(new ComplexRational(-BigRational.One / 25 * 12, 0));
        s2.Dictionary["over"].AssertEquals(new ComplexRational(0, BigRational.One / 25 * 12));
        s2.Dictionary["there"].AssertEquals(new ComplexRational(0, -BigRational.One / 25 * 16));
    }

    [TestMethod]
    public void TestFlattenBroken() {
        var a1 = new QuantumSuperPosition<string>(new Dictionary<string, ComplexRational> {
            {"hey", BigRational.One},
        });
        var a2 = new QuantumSuperPosition<string>(new Dictionary<string, ComplexRational> {
            {"hey", -BigRational.One},
        });
        var s2 = new QuantumSuperPosition<QuantumSuperPosition<string>>(new Dictionary<QuantumSuperPosition<string>, ComplexRational> {
            {a1, BigRational.One/5*3},
            {a2, BigRational.One/5*4}
        });
        
        TestingUtilities.AssertThrows(() => s2.Flatten());
    }
}
