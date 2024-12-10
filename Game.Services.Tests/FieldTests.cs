using System.Text;
using FluentAssertions;

namespace Game.Services.Tests;

public class FieldTests
{
    [Fact]
    public void Advance_Should_DieOfLoneliness()
    {
        // Arrange
        const string before = """
                              - + - - -
                              - + - - +
                              - - - - -
                              + - - - -
                              - - + + -
                              """;

        const string expected = """
                                - - - - -
                                - - - - -
                                - - - - -
                                - - - - -
                                - - - - -
                                """;

        var sut = ParseField(before);

        // Act
        sut.Advance();

        // Assert
        FormatField(sut).Should().Be(expected);
    }

    [Fact]
    public void Advance_Should_DieOfStarvation()
    {
        // Arrange
        const string before = """
                              + + + - -
                              + + + - -
                              - - - - -
                              + + + + +
                              + + + + +
                              """;

        const string expected = """
                                + - + - -
                                + - + - -
                                - - - - -
                                + - - - +
                                + - - - +
                                """;

        var sut = ParseField(before);

        // Act
        sut.Advance();

        // Assert
        FormatField(sut).Should().Be(expected);
    }

    [Fact]
    public void Advance_Should_FlipBar()
    {
        // Arrange
        const string before = """
                              - - - - -
                              - - - - -
                              - + + + -
                              - - - - -
                              - - - - -
                              """;

        const string expected = """
                                - - - - -
                                - - + - -
                                - - + - -
                                - - + - -
                                - - - - -
                                """;

        var sut = ParseField(before);

        // Act
        sut.Advance();

        // Assert
        FormatField(sut).Should().Be(expected);
    }

    [Fact]
    public void Advance_Should_KeepSquare()
    {
        // Arrange
        const string before = """
                              - - - - -
                              - + + - -
                              - + + - -
                              - - - - -
                              - - - - -
                              """;

        var sut = ParseField(before);

        // Act
        sut.Advance();

        // Assert
        FormatField(sut).Should().Be(before);
    }

    [Fact]
    public void Advance_Should_KeepCircle()
    {
        // Arrange
        const string before = """
                              - - + + -
                              - + - - +
                              - + - - +
                              - - + + -
                              - - - - -
                              """;

        var sut = ParseField(before);

        // Act
        sut.Advance();

        // Assert
        FormatField(sut).Should().Be(before);
    }

    private static Field ParseField(string fieldString)
    {
        var lines = fieldString.Replace(" ", "").Trim().Split(Environment.NewLine);
        var height = lines.Length;
        var width = lines[0].Length;

        var field = new Field(width, height, new ParallelOptions());
        var grid = field.Grid;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                grid[y][x].IsAlive = lines[y][x] == '+';
            }
        }

        return field;
    }

    private static string FormatField(Field field)
    {
        var grid = field.Grid;
        var height = grid.Length;
        var width = grid[0].Length;

        var fieldString = new StringBuilder();
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                fieldString.Append(grid[y][x].IsAlive ? "+ " : "- ");
            }

            // Remove last space
            fieldString.Remove(fieldString.Length - 1, 1);
            fieldString.AppendLine();
        }

        return fieldString.ToString().Trim();
    }
}
