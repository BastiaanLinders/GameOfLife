using FluentAssertions;

namespace Game.Services.Tests;

public class SquareTests
{
    [Fact]
    public void Reset_Should_OnlyResetSquare()
    {
        // Arrange
        const int x = 2;
        const int y = 3;
        var sut = new Square
        {
            Location = new Location { X = x, Y = y },
            IsAlive = true
        };

        // Act
        sut.Reset();
        sut.Advance();

        // Assert
        sut.IsAlive.Should().BeFalse();
        sut.Location.X.Should().Be(x);
        sut.Location.Y.Should().Be(y);
    }
}
