using FluentAssertions;
using TourManagement.Domain.Entities;
using TourManagement.Domain.ValueObjects;

namespace TourManagement.Tests.Domain;

public class KeyPointTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateKeyPoint()
    {
        var coord = new Coordinate(45.25, 19.85);

        var kp = new KeyPoint("Muzej", "Opis muzeja", coord, "img.jpg", 1);

        kp.Name.Should().Be("Muzej");
        kp.Description.Should().Be("Opis muzeja");
        kp.Position.Should().Be(coord);
        kp.ImageUrl.Should().Be("img.jpg");
        kp.Order.Should().Be(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldThrow(string? name)
    {
        var act = () => new KeyPoint(name!, "Opis", new Coordinate(45, 19), "img.jpg", 1);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithNullPosition_ShouldThrow()
    {
        var act = () => new KeyPoint("Muzej", "Opis", null!, "img.jpg", 1);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Update_ShouldChangeProperties()
    {
        var kp = new KeyPoint("Stari", "Stari opis", new Coordinate(45, 19), "old.jpg", 1);
        var newCoord = new Coordinate(44.8, 20.4);

        kp.Update("Novi", "Novi opis", newCoord, "new.jpg");

        kp.Name.Should().Be("Novi");
        kp.Description.Should().Be("Novi opis");
        kp.Position.Should().Be(newCoord);
        kp.ImageUrl.Should().Be("new.jpg");
    }

    [Fact]
    public void Update_WithInvalidName_ShouldThrow()
    {
        var kp = new KeyPoint("Muzej", "Opis", new Coordinate(45, 19), "img.jpg", 1);

        var act = () => kp.Update("", "Opis", new Coordinate(45, 19), "img.jpg");

        act.Should().Throw<ArgumentException>();
    }
}

public class CoordinateTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateCoordinate()
    {
        var coord = new Coordinate(45.25, 19.85);

        coord.Latitude.Should().Be(45.25);
        coord.Longitude.Should().Be(19.85);
    }

    [Theory]
    [InlineData(-91, 0)]
    [InlineData(91, 0)]
    public void Create_WithInvalidLatitude_ShouldThrow(double lat, double lon)
    {
        var act = () => new Coordinate(lat, lon);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(0, -181)]
    [InlineData(0, 181)]
    public void Create_WithInvalidLongitude_ShouldThrow(double lat, double lon)
    {
        var act = () => new Coordinate(lat, lon);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void TwoCoordinates_WithSameValues_ShouldBeEqual()
    {
        var a = new Coordinate(45.25, 19.85);
        var b = new Coordinate(45.25, 19.85);

        a.Should().Be(b);
    }
}
