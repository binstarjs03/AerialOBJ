using System;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.CoordinateSystem;

using Range = binstarjs03.AerialOBJ.Core.Range;

namespace binstarjs03.AerialOBJ.Test;

[TestClass]
public class CoordinateSystemTest
{
    [TestMethod]
    public void TestThrowRange()
    {
        CoordsRange3 sectionBlockRange = new(new Coords3(0,0,0), new Coords3(15,15,15));
        try
        {
            sectionBlockRange.ThrowIfOutside(new Coords3(0, 17, 0));
        }
        catch (Exception e)
        {
            Assert.AreEqual(typeof(ArgumentOutOfRangeException), e.GetType());
            Console.WriteLine(e.Message);
        }
    }

    [TestMethod]
    public void TestThrowInvalidRange()
    {
        Range range = new(-2, 4);
        try
        {
            range.Max = -5;
        }
        catch (Exception e)
        {
            Assert.AreEqual(typeof(InvalidRangeException), e.GetType());
            Console.WriteLine(e.Message);
        }
    }
}