using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;

namespace CoreElements.UnitTests;

[TestClass]
public class ElementsListsTests
{
    private readonly Random random = new Random();
    private readonly ElementList<int> sut = new ElementList<int>();

    [TestMethod]
    public void CreateElementsLists_ShouldBeIList()
    {
        Assert.IsInstanceOfType(sut, typeof(IList<int>));
    }

    [TestMethod]
    public void CreateElementsLists_ShouldBeIEnumerable()
    {
        Assert.IsInstanceOfType(sut, typeof(IEnumerable<int>));
    }

    [TestMethod]
    public void CreateElementsLists_ShouldBeICollection()
    {
        Assert.IsInstanceOfType(sut, typeof(ICollection<int>));
    }

    [TestMethod]
    public void ElementsList_EmptyListAsserts()
    {
        AssertListIsEmpty(sut);
    }

    private void AssertListIsEmpty<T>(ElementList<T> list)
    {
        Assert.AreEqual(0, list.Count);
        T item;
        item = list[0];
        Assert.AreEqual(default, item);
    }

    private void AssertListEquality<T>(ElementList<T> list, T[] values)
    {
        Assert.AreEqual(values.Length, list.Count);
        T item;
        item = list[values.Length+1];
        Assert.AreEqual(default, item);

        for (var i = 0; i < values.Length; i++)
        {
            Assert.AreEqual(values[i], list[i]);
        }
    }

    [TestMethod]
    public void ElementsList_CanAddItem()
    {
        sut.Add(10);
        AssertListEquality(sut, new[] {10});
    }

    [TestMethod]
    public void ElementsList_CanAddItems()
    {
        sut.Add(10);
        sut.Add(11);
        sut.Add(12);
        sut.Add(13);
        AssertListEquality(sut, new[] { 10, 11, 12, 13 });
    }

    [TestMethod]
    public void ElementsList_CanRemoveItems()
    {
        sut.Add(10);
        sut.Add(11);
        sut.Add(12);
        sut.Add(13);
        sut.RemoveAt(2);
        AssertListEquality(sut, new[] { 10, 11, 13 });
    }

    [TestMethod]
    public void ElementsList_InitializeWithArray()
    {
        var item = new ElementList<int>(new[] { 10, 11, 13 });
        AssertListEquality(item, new[] { 10, 11, 13 });
    }

    [TestMethod]
    public void ElementsList_InitializeWithArrayCreatesACopy()
    {
        var original = new[] { 10, 11, 13 };
        var item = new ElementList<int>(original);
        AssertListEquality(item, new[] { 10, 11, 13 });

        item[1] = 23;
        AssertListEquality(item, new[] { 10, 23, 13 });
        Assert.AreEqual(11, original[1]);

    }

    [TestMethod]
    public void ElementsList_HasCount()
    {
        sut.Add(10);
        sut.Add(11);
        sut.Add(12);
        sut.Add(13);
        sut.RemoveAt(2);
        Assert.AreEqual(3, sut.Count);
    }

    [TestMethod]
    public void ElementsList_HasDirctAccessToElements()
    {
        sut.Add(10);
        sut.Add(11);
        sut.Add(12);
        sut.Add(13);
        sut.RemoveAt(2);
        var items = sut.Items;
        Assert.AreEqual(3, items.Length);
        Assert.AreEqual(10, items[0]);
        Assert.AreEqual(11, items[1]);
        Assert.AreEqual(13, items[2]);
    }

    [TestMethod]
    public void ElementsList_Expands()
    {
        for(var i = 0;i<23; i++)
        {
            sut.Add(random.Next(100));
        }
        var items = sut.Items;
        Assert.AreEqual(23, items.Length);
    }
}