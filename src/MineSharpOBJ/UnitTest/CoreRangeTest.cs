namespace UnitTest;

using binstarjs03.MineSharpOBJ.Core;

[TestClass]
public class CoreRangeTest
{
    private static void WriteException(Exception e)
    {
        Console.WriteLine($"Exception caught: {e.GetType()} {e.Message}");
    }

    [TestMethod]
    public void TestInstantiate()
    {
        Range range = new(0, 5);
        Console.WriteLine(range);
    }

    [TestMethod]
    public void TestConstructorInvalidRange()
    {
        try
        {
            Range range = new(4, 2);
        }
        catch (InvalidRangeException e)
        {
            WriteException(e);
        }
    }

    [TestMethod]
    public void TestSettingInvalidMinValue()
    {
        try
        {
            Range range = new(1, 8);
            range.Min = 10;
        }
        catch (InvalidRangeException e)
        {
            WriteException(e);
        }
    }

    [TestMethod]
    public void TestSettingInvalidMaxValue()
    {
        try
        {
            Range range = new(1, 8);
            range.Max = -3;
        }
        catch (InvalidRangeException e)
        {
            WriteException(e);
        }
    }

    [TestMethod]
    public void TestInside()
    {
        Range range = new(1, 8);
        Console.WriteLine(range.IsInside(3));
    }

    [TestMethod]
    public void TestOutside()
    {
        Range range = new(1, 8);
        Console.WriteLine(range.IsInside(-2));
    }

    [TestMethod]
    public void TestThrowIfOutside()
    {
        try
        {
            Range range = new(1, 8);
            range.ThrowIfOutside(-5);
        }
        catch (ArgumentOutOfRangeException e)
        {
            WriteException(e);
        }
    }
}