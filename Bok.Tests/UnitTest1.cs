using System.Drawing;

namespace Bok.Tests;

//Mock an interface with default values
public abstract class BaseProperties<T> where T : class, IPet
{

    public virtual string ExpectedName { get; set; } = "sdfg";
    public virtual double ExpectedPrice { get; set; } = 735.00;
    public virtual DateTime ExpectedDateOfBirth { get; } = new DateTime(2020, 1, 1);
    public virtual int ExpectedLegs { get; set; } = 5;
    public virtual Color ExpectedColour { get; set; } = Color.Blue;
    public virtual bool ExpectedHairy { get; set; } = true;
    public virtual byte[] ExpectedBarcode { get; set; } = new byte[] { 0, 0, 1, 1, 0 };

    public virtual string ExpectedMethod2 { get; set; } = null;
    public virtual double ExpectedMethod3 { get; set; } = 0;
    public virtual DateTime ExpectedMethod4 { get; } = DateTime.MinValue;
    public virtual int ExpectedMethod5 { get; set; } = 0;
    public virtual Color ExpectedMethod6 { get; set; } = Color.Empty;
    public virtual bool ExpectedMethod1 { get; set; } = false;

    public T Mock;
    public IMock<T> Proxy;
    public BaseProperties()
    {
        Setup();
    }

    protected virtual void Setup()
    {
        Proxy = Mock<T>.Create();
        Mock = Proxy.Instance;
    }

    [Fact]
    public void MethodReturnsDefaultBool()
    {
        var result = Mock.Pat1();
        Assert.Equal(ExpectedMethod1, result);
    }
    [Fact]
    public void MethodReturnsDefaultString()
    {
        var result = Mock.Pat2();
        Assert.Equal(ExpectedMethod2, result);
    }

    [Fact]
    public void MethodReturnsDefaultDouble()
    {
        var result = Mock.Pat3();
        Assert.Equal(ExpectedMethod3, result);
    }
    [Fact]
    public void MethodReturnsDefaultDateTime()
    {
        DateTime result = Mock.Pat4();
        Assert.Equal(ExpectedMethod4, result);
    }
    [Fact]
    public void MethodReturnsDefaultInt32()
    {
        var result = Mock.Pat5();
        Assert.Equal(ExpectedMethod5, result);
    }
    [Fact]
    public void PropertyReturnsDefaultString()
    {
        Proxy.Defaults();
        string input = ExpectedName;
        Assert.Null(Mock.Name);
    }
    [Fact]
    public void PropertyReturnsDefaultInt32()
    {
        Proxy.Defaults();
        int input = ExpectedLegs;
        Assert.True(Mock.Legs == 0);
    }
    [Fact]
    public void PropertyReturnsDefaultColour()
    {
        Proxy.Defaults();
        Color input = ExpectedColour;
        Assert.Equal(Color.Empty, Mock.Colour);
    }
    [Fact]
    public void PropertyReturnsDefaultBool()
    {
        Proxy.Defaults();
        bool input = ExpectedHairy;
        Assert.False(Mock.Hairy);
    }





    [Fact]
    public void PropertyCanSetDouble()
    {
        Mock.Price = ExpectedPrice;
        Assert.Equal(ExpectedPrice, Mock.Price);
    }
    [Fact]
    public void PropertyCanSetByteArray()
    {
        Mock.Barcode = ExpectedBarcode;
        Assert.Equal(ExpectedBarcode, Mock.Barcode);
    }


    [Fact]
    public void PropertyCanSetString()
    {
        Mock.Name = ExpectedName;
        Assert.True(Mock.Name == ExpectedName);
    }
    [Fact]
    public void PropertyCanSetInt32()
    {
        Mock.Legs = ExpectedLegs;
        Assert.Equal(ExpectedLegs, Mock.Legs);
    }
    [Fact]
    public void PropertyCanSetColour()
    {
        Mock.Colour = ExpectedColour;
        Assert.Equal(ExpectedColour, Mock.Colour);
    }
    [Fact]
    public void PropertyCanSetBool()
    {
        Mock.Hairy = ExpectedHairy;
        Assert.Equal(ExpectedHairy, Mock.Hairy);
    }

    [Fact]
    public void SetupPropertyBool()
    {
        bool input = true;
        Proxy.SetupProperty(x => x.Hairy = input);
        Assert.True(Mock.Hairy == input);
    }
    [Fact]
    public void SetupPropertyString()
    {
        string input = "LKHJJL";
        Proxy.SetupProperty(x => x.Name = input);
        Assert.Equal(Mock.Name, input);
    }
    [Fact]
    public void SetupPropertyInt32()
    {
        var input = 7777;
        Proxy.SetupProperty(x => x.Legs = input);
        Assert.Equal(Mock.Legs, input);
    }
    [Fact]
    public void SetupPropertyByteArray()
    {
        byte[] input = new byte[] { 1, 0, 1 };
        Proxy.SetupProperty(x => x.Barcode = input);
        Assert.Equal(Mock.Barcode, input);
    }


    [Fact]
    public void SetupMethodBool()
    {
        var input = true;
        Proxy.Setup(x => x.Pat1(), input);
        var result = Mock.Pat1();
        Assert.Equal(input, result);
    }
    [Fact]
    public void SetupMethodString()
    {
        var input = "yuirtert";
        Proxy.Setup(x => x.Pat2(), input);
        var result = Mock.Pat2();
        Assert.Equal(input, result);
    }

    [Fact]
    public void SetupMethodDouble()
    {
        var input = 888888.00;
        Proxy.Setup(x => x.Pat3(), input);
        var result = Mock.Pat3();
        Assert.Equal(input, result);
    }
    [Fact]
    public void SetupMethodDateTime()
    {
        var input = new DateTime(2040, 5, 5);
        Proxy.Setup(x => x.Pat4(), input);
        DateTime result = Mock.Pat4();
        Assert.Equal(input, result);
    }
    [Fact]
    public void SetupMethodInt32()
    {
        var input = 786687;
        Proxy.Setup(x => x.Pat5(), input);
        var result = Mock.Pat5();
        Assert.Equal(input, result);
    }



}

public class InterfaceProperties1 : BaseProperties<IPet>
{
    protected override void Setup()
    {
        Proxy = Mock<IPet>.Create();
        Mock = Proxy.Instance;
    }
}

public class InheritedProperties1 : BaseProperties<IDog>
{
    protected override void Setup()
    {
        Proxy = Mock<IDog>.Create();
        Mock = Proxy.Instance;
    }
}
public class InheritedProperties2 : BaseProperties<IPoodle>
{
    protected override void Setup()
    {
        Proxy = Mock<IPoodle>.Create();
        Mock = Proxy.Instance;
    }
}
public class InheritedInterfaceProperties3 : BaseProperties<IRat>
{
    protected override void Setup()
    {
        // Test all properties inherited a few levels down
        Proxy = Mock<IRat>.Create();
        Mock = Proxy.Instance;
    }
}

public class InheritedClassProperties1 : BaseProperties<IDog>
{
    protected override void Setup()
    {
        // Wrap + Defaults should replace all methods with default responses
        var dog = new Dog();
        Proxy = Mock<IDog>.Wrap(dog).Defaults();
        Mock = Proxy.Instance;
    }
}


public class InheritedClassProperties2 : BaseProperties<IRat>
{
    protected override void Setup()
    {
        // Wrap + Defaults should replace all methods with default responses
        var rat = new Rat();
        Proxy = Mock<IRat>.Wrap(rat).Defaults();
        Mock = Proxy.Instance;
    }
}

public class WrapClassProperties1 : BaseProperties<IDog>
{
    public override string ExpectedName { get; set; } = "dog";
    public override double ExpectedPrice { get; set; } = 55621;

    public override DateTime ExpectedDateOfBirth { get; } = DateTime.MaxValue;

    public override int ExpectedLegs { get; set; } = 4;
    public override Color ExpectedColour { get; set; } = Color.Brown;
    public override bool ExpectedHairy { get; set; } = true;
    public override byte[] ExpectedBarcode { get; set; } = new byte[] { 0 };

    public override bool ExpectedMethod1 { get; set; } = true;
    public override string ExpectedMethod2 { get; set; } = "zzzz";
    public override double ExpectedMethod3 { get; set; } = 33.00;
    public override DateTime ExpectedMethod4 { get; } = new DateTime(2023, 1, 1);
    public override int ExpectedMethod5 { get; set; } = 999;
    public override Color ExpectedMethod6 { get; set; } = Color.Aqua;


    protected override void Setup()
    {
        // Wrap should return the Dog implementation responses
        var dog = new Dog();
        Proxy = Mock<IDog>.Wrap(dog);
        Mock = Proxy.Instance;
    }

}


public class WrapClassProperties2 : BaseProperties<IRat>
{

    public override string ExpectedName { get; set; } = "rat";
    public override double ExpectedPrice { get; set; } = 6789;
    public override DateTime ExpectedDateOfBirth { get; } = new DateTime(2050, 4, 4);
    public override int ExpectedLegs { get; set; } = 456;
    public override Color ExpectedColour { get; set; } = Color.RebeccaPurple;
    public override bool ExpectedHairy { get; set; } = false;
    public override byte[] ExpectedBarcode { get; set; } = new byte[] { 0, 0, 0, 0 };

    public override bool ExpectedMethod1 { get; set; } = false;
    public override string ExpectedMethod2 { get; set; } = "aaa";
    public override double ExpectedMethod3 { get; set; } = 22.00;
    public override DateTime ExpectedMethod4 { get; } = new DateTime(2026, 1, 1);
    public override int ExpectedMethod5 { get; set; } = 444;
    public override Color ExpectedMethod6 { get; set; } = Color.Green;
    protected override void Setup()
    {
        // Wrap should return the Rat implementation responses
        var rat = new Rat();
        Proxy = Mock<IRat>.Wrap(rat);
        Mock = Proxy.Instance;
    }

}

public interface IPet
{
    string Name { get; set; }
    double Price { get; set; }
    DateTime DateOfBirth { get; }
    int Legs { get; set; }
    Color Colour { get; set; }
    bool Hairy { get; set; }
    byte[] Barcode { get; set; }

    bool Pat1();
    string Pat2();
    double Pat3();
    DateTime Pat4();
    int Pat5();
    Color Pat6();

    void Method2(string newDescription, out double price, ref bool dummy);
    Task<int> Method3Async(string newDescription);
}
public interface IDog : IPet
{
    bool Cranky { get; set; }
}
public interface IPoodle : IDog
{
}
public interface IRat : IPoodle
{
    new string Name { get; set; }
    new double Price { get; set; }
}

public class Dog : IDog
{
    public virtual bool Cranky { get; set; } = true;
    public virtual string Name { get; set; } = "dog";
    public virtual double Price { get; set; } = 55621;

    public virtual DateTime DateOfBirth { get; set; } = DateTime.MaxValue;

    public virtual int Legs { get; set; } = 4;
    public virtual Color Colour { get; set; } = Color.Brown;
    public virtual bool Hairy { get; set; } = true;
    public virtual byte[] Barcode { get; set; } = new byte[] { 0 };

    public void Method2(string newDescription, out double price, ref bool dummy)
    {
        price = 10;
    }

    public Task<int> Method3Async(string newDescription)
    {
        throw new NotImplementedException();
    }

    public virtual bool Pat1()
    {
        return true;
    }

    public virtual string Pat2()
    {
        return "zzzz";
    }

    public virtual double Pat3()
    {
        return 33.00;
    }

    public virtual DateTime Pat4()
    {
        return new DateTime(2023, 1, 1);
    }

    public virtual int Pat5()
    {
        return 999;
    }

    public virtual Color Pat6()
    {
        return Color.Aqua;
    }
}
public class Rat : Dog, IRat
{
    public override string Name { get; set; } = "rat";
    public override double Price { get; set; } = 6789;
    public override DateTime DateOfBirth { get; set; } = new DateTime(2050, 4, 4);
    public override int Legs { get; set; } = 456;
    public override Color Colour { get; set; } = Color.RebeccaPurple;
    public override bool Hairy { get; set; } = false;
    public override byte[] Barcode { get; set; } = new byte[] { 0, 0, 0, 0 };

    public override bool Pat1()
    {
        return false;
    }

    public override string Pat2()
    {
        return "aaa";
    }

    public override double Pat3()
    {
        return 22.00;
    }

    public override DateTime Pat4()
    {
        return new DateTime(2026, 1, 1);
    }

    public override int Pat5()
    {
        return 444;
    }

    public override Color Pat6()
    {
        return Color.Green;
    }
}
