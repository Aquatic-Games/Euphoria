namespace Tests.Math;

public class Vector2Tests
{
    [Test]
    public void TestConstruct()
    {
        Vector2i vec1 = new Vector2i(1, 2);
        
        Assert.Multiple(() =>
        {
            Assert.That(vec1.X, Is.EqualTo(1));
            Assert.That(vec1.Y, Is.EqualTo(2));
        });
    }

    [Test]
    public void TestConstructWithScalar()
    {
        Vector2i vec1 = new Vector2i(4);
        
        Assert.Multiple(() =>
        {
            Assert.That(vec1.X, Is.EqualTo(4));
            Assert.That(vec1.Y, Is.EqualTo(4));
        });
    }

    [Test]
    public void TestZero()
    {
        Vector2i zero = Vector2i.Zero;
        
        Assert.Multiple(() =>
        {
            Assert.That(zero.X, Is.EqualTo(0));
            Assert.That(zero.Y, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void TestOne()
    {
        Vector2i one = Vector2i.One;
        
        Assert.Multiple(() =>
        {
            Assert.That(one.X, Is.EqualTo(1));
            Assert.That(one.Y, Is.EqualTo(1));
        });
    }
    
    [Test]
    public void TestUnitX()
    {
        Vector2i unitX = Vector2i.UnitX;
        
        Assert.Multiple(() =>
        {
            Assert.That(unitX.X, Is.EqualTo(1));
            Assert.That(unitX.Y, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void TestUnitY()
    {
        Vector2i unitY = Vector2i.UnitY;
        
        Assert.Multiple(() =>
        {
            Assert.That(unitY.X, Is.EqualTo(0));
            Assert.That(unitY.Y, Is.EqualTo(1));
        });
    }

    [Test]
    public void TestAddition()
    {
        Vector2f vec1 = new Vector2f(0.5f, 2);
        Vector2f vec2 = new Vector2f(-2, 1.5f);

        Vector2f vec3 = vec1 + vec2;
        
        Assert.Multiple(() =>
        {
            Assert.That(vec3.X, Is.EqualTo(-1.5f));
            Assert.That(vec3.Y, Is.EqualTo(3.5f));
        });
    }
    
    [Test]
    public void TestSubtraction()
    {
        Vector2f vec1 = new Vector2f(0.5f, 2);
        Vector2f vec2 = new Vector2f(-2, 1);

        Vector2f vec3 = vec1 - vec2;
        
        Assert.Multiple(() =>
        {
            Assert.That(vec3.X, Is.EqualTo(2.5f));
            Assert.That(vec3.Y, Is.EqualTo(1));
        });
    }
    
    [Test]
    public void TestMultiplicationWith2Vectors()
    {
        Vector2f vec1 = new Vector2f(-3, 0.2f);
        Vector2f vec2 = new Vector2f(2);

        Vector2f vec3 = vec1 * vec2;
        
        Assert.Multiple(() =>
        {
            Assert.That(vec3.X, Is.EqualTo(-6));
            Assert.That(vec3.Y, Is.EqualTo(0.4f));
        });
    }
    
    [Test]
    public void TestMultiplicationWithScalar()
    {
        Vector2f vec1 = new Vector2f(-3, 0.2f);
        float scalar = 2;

        Vector2f vec3 = vec1 * scalar;
        
        Assert.Multiple(() =>
        {
            Assert.That(vec3.X, Is.EqualTo(-3 * 2f));
            Assert.That(vec3.Y, Is.EqualTo(0.2f * 2f));
        });
    }
    
    [Test]
    public void TestDivisionWith2Vectors()
    {
        Vector2f vec1 = new Vector2f(-3, 0.2f);
        Vector2f vec2 = new Vector2f(2);

        Vector2f vec3 = vec1 / vec2;
        
        Assert.Multiple(() =>
        {
            Assert.That(vec3.X, Is.EqualTo(-3 / 2f));
            Assert.That(vec3.Y, Is.EqualTo(0.2f / 2f));
        });
    }
    
    [Test]
    public void TestDivisionWithScalar()
    {
        Vector2f vec1 = new Vector2f(-3, 0.2f);
        float scalar = 2;

        Vector2f vec3 = vec1 / scalar;
        
        Assert.Multiple(() =>
        {
            Assert.That(vec3.X, Is.EqualTo(-3 / 2f));
            Assert.That(vec3.Y, Is.EqualTo(0.2f / 2f));
        });
    }
}