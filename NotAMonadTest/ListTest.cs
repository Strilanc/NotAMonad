using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

[TestClass]
public class ListTest {
    [TestMethod]
    public void TestWrap() {
        var s = ListMonad.Wrap("test");
        s.Single().AssertEquals("test");
    }
    
    [TestMethod]
    public void TestTransform() {
        var s = new List<int> {1, 2, 3}.Transform(e => e*e);
        s.SequenceEqual(new[] {1,4,9}).AssertTrue();
    }
    
    [TestMethod]
    public void TestFlatten() {
        new List<List<int>> {
            new List<int>(),
            new List<int> {2, 3, 5, 7},
            new List<int> {1}
        }.Flatten()
         .SequenceEqual(new[] {2, 3, 5, 7, 1})
         .AssertTrue();

        new List<List<int>> {
            new List<int>(),
        }.Flatten()
         .SequenceEqual(new int[0])
         .AssertTrue();

        new List<List<int>>()
            .Flatten()
            .SequenceEqual(new int[0])
            .AssertTrue();
    }
}
