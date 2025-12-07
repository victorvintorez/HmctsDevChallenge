using HmctsDevChallenge.Backend.Helpers.Validation;

namespace HmctsDevChallenge.Backend.Test.Unit.Helpers.Validation;

public class FutureDateTimeAttribute_IsValidShould
{
    [Fact]
    public void IsValid_InputIsFutureDateTime_ReturnTrue()
    {
        // ARRANGE
        var value = DateTime.Now.AddDays(15);
        var attribute = new FutureDateTimeAttribute();

        // ACT
        var result = attribute.IsValid(value);

        // ASSERT
        Assert.True(result);
    }

    [Fact]
    public void IsValid_InputIsPresentDateTime_ReturnFalse()
    {
        // ARRANGE
        var value = DateTime.Now;
        var attribute = new FutureDateTimeAttribute();

        // ACT
        var result = attribute.IsValid(value);

        // ASSERT
        Assert.False(result);
    }

    [Fact]
    public void IsValid_InputIsPastDateTime_ReturnFalse()
    {
        // ARRANGE
        var value = DateTime.Now.AddDays(-15);
        var attribute = new FutureDateTimeAttribute();

        // ACT
        var result = attribute.IsValid(value);

        // ASSERT
        Assert.False(result);
    }

    [Fact]
    public void IsValid_InputIsNull_ReturnFalse()
    {
        // ARRANGE
        DateTime? value = null;
        var attribute = new FutureDateTimeAttribute();

        // ACT
        var result = attribute.IsValid(value);

        // ASSERT
        Assert.False(result);
    }

    [Theory]
    [InlineData("invalid value")]
    [InlineData(15)]
    [InlineData(12.5)]
    [InlineData('c')]
    public void IsValid_InputIsInvalidType_ReturnFalse(object value)
    {
        // ARRANGE
        var attribute = new FutureDateTimeAttribute();

        // ACT
        var result = attribute.IsValid(value);

        // ASSERT
        Assert.False(result);
    }
}