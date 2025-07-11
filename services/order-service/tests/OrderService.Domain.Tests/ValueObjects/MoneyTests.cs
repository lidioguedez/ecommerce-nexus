using FluentAssertions;
using OrderService.Domain.ValueObjects;
using Xunit;

namespace OrderService.Domain.Tests.ValueObjects;

/// <summary>
/// Unit tests for the Money value object following TDD principles.
/// </summary>
public class MoneyTests
{
    [Fact]
    public void Constructor_WithValidAmountAndCurrency_ShouldCreateMoney()
    {
        // Arrange
        var amount = 100.50m;
        var currency = "USD";

        // Act
        var money = new Money(amount, currency);

        // Assert
        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Constructor_WithDefaultCurrency_ShouldUseUSD()
    {
        // Arrange
        var amount = 50.00m;

        // Act
        var money = new Money(amount);

        // Assert
        money.Amount.Should().Be(50.00m);
        money.Currency.Should().Be("USD");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void Constructor_WithNegativeAmount_ShouldThrowArgumentException(decimal amount)
    {
        // Act & Assert
        var act = () => new Money(amount);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Amount cannot be negative*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidCurrency_ShouldThrowArgumentException(string currency)
    {
        // Act & Assert
        var act = () => new Money(100, currency);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Currency cannot be null or empty*");
    }

    [Fact]
    public void Constructor_ShouldRoundToTwoDecimalPlaces()
    {
        // Arrange
        var amount = 100.999m;

        // Act
        var money = new Money(amount);

        // Assert
        money.Amount.Should().Be(101.00m);
    }

    [Fact]
    public void Constructor_ShouldNormalizeCurrencyToUpperCase()
    {
        // Arrange
        var currency = "eur";

        // Act
        var money = new Money(100, currency);

        // Assert
        money.Currency.Should().Be("EUR");
    }

    [Fact]
    public void Add_WithSameCurrency_ShouldReturnSummedMoney()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(50, "USD");

        // Act
        var result = money1.Add(money2);

        // Assert
        result.Amount.Should().Be(150);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Add_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(50, "EUR");

        // Act & Assert
        var act = () => money1.Add(money2);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Cannot add different currencies: USD and EUR");
    }

    [Fact]
    public void Subtract_WithSameCurrency_ShouldReturnSubtractedMoney()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(30, "USD");

        // Act
        var result = money1.Subtract(money2);

        // Assert
        result.Amount.Should().Be(70);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Subtract_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(50, "EUR");

        // Act & Assert
        var act = () => money1.Subtract(money2);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Cannot subtract different currencies: USD and EUR");
    }

    [Fact]
    public void Multiply_WithPositiveInteger_ShouldReturnMultipliedMoney()
    {
        // Arrange
        var money = new Money(25.50m, "USD");

        // Act
        var result = money.Multiply(3);

        // Assert
        result.Amount.Should().Be(76.50m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Multiply_WithPositiveDecimal_ShouldReturnMultipliedMoney()
    {
        // Arrange
        var money = new Money(100, "USD");

        // Act
        var result = money.Multiply(1.5m);

        // Assert
        result.Amount.Should().Be(150);
        result.Currency.Should().Be("USD");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    public void Multiply_WithNegativeInteger_ShouldThrowArgumentException(int quantity)
    {
        // Arrange
        var money = new Money(100, "USD");

        // Act & Assert
        var act = () => money.Multiply(quantity);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Quantity cannot be negative*");
    }

    [Theory]
    [InlineData(-1.5)]
    [InlineData(-0.1)]
    public void Multiply_WithNegativeDecimal_ShouldThrowArgumentException(decimal multiplier)
    {
        // Arrange
        var money = new Money(100, "USD");

        // Act & Assert
        var act = () => money.Multiply(multiplier);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Multiplier cannot be negative*");
    }

    [Fact]
    public void Zero_ShouldReturnZeroMoney()
    {
        // Act
        var zero = Money.Zero();

        // Assert
        zero.Amount.Should().Be(0);
        zero.Currency.Should().Be("USD");
        zero.IsZero.Should().BeTrue();
    }

    [Fact]
    public void Zero_WithCurrency_ShouldReturnZeroMoneyWithSpecifiedCurrency()
    {
        // Act
        var zero = Money.Zero("EUR");

        // Assert
        zero.Amount.Should().Be(0);
        zero.Currency.Should().Be("EUR");
        zero.IsZero.Should().BeTrue();
    }

    [Fact]
    public void IsZero_WithZeroAmount_ShouldReturnTrue()
    {
        // Arrange
        var money = new Money(0, "USD");

        // Assert
        money.IsZero.Should().BeTrue();
    }

    [Fact]
    public void IsZero_WithNonZeroAmount_ShouldReturnFalse()
    {
        // Arrange
        var money = new Money(10, "USD");

        // Assert
        money.IsZero.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithSameAmountAndCurrency_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(100, "USD");

        // Assert
        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentAmount_ShouldReturnFalse()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(200, "USD");

        // Assert
        money1.Should().NotBe(money2);
        (money1 != money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentCurrency_ShouldReturnFalse()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(100, "EUR");

        // Assert
        money1.Should().NotBe(money2);
        (money1 != money2).Should().BeTrue();
    }

    [Fact]
    public void GreaterThan_WithSameCurrencyAndLargerAmount_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(200, "USD");
        var money2 = new Money(100, "USD");

        // Assert
        (money1 > money2).Should().BeTrue();
    }

    [Fact]
    public void GreaterThan_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var money1 = new Money(200, "USD");
        var money2 = new Money(100, "EUR");

        // Act & Assert
        var act = () => money1 > money2;
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Cannot compare different currencies: USD and EUR");
    }

    [Fact]
    public void LessThan_WithSameCurrencyAndSmallerAmount_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(200, "USD");

        // Assert
        (money1 < money2).Should().BeTrue();
    }

    [Fact]
    public void AdditionOperator_ShouldWorkCorrectly()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(50, "USD");

        // Act
        var result = money1 + money2;

        // Assert
        result.Amount.Should().Be(150);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void SubtractionOperator_ShouldWorkCorrectly()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(30, "USD");

        // Act
        var result = money1 - money2;

        // Assert
        result.Amount.Should().Be(70);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void MultiplicationOperatorWithInt_ShouldWorkCorrectly()
    {
        // Arrange
        var money = new Money(25, "USD");

        // Act
        var result = money * 4;

        // Assert
        result.Amount.Should().Be(100);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void MultiplicationOperatorWithDecimal_ShouldWorkCorrectly()
    {
        // Arrange
        var money = new Money(100, "USD");

        // Act
        var result = money * 0.5m;

        // Assert
        result.Amount.Should().Be(50);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        // Arrange
        var money = new Money(100.50m, "USD");

        // Act
        var result = money.ToString();

        // Assert
        result.Should().Be("100.50 USD");
    }
}