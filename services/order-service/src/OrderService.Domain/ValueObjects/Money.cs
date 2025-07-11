using OrderService.Domain.Common;

namespace OrderService.Domain.ValueObjects;

/// <summary>
/// Represents a monetary value with amount and currency.
/// This is an immutable value object that encapsulates money-related business logic.
/// </summary>
public class Money : ValueObject
{
    /// <summary>
    /// Gets the monetary amount.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Gets the currency code (e.g., USD, EUR).
    /// </summary>
    public string Currency { get; }

    /// <summary>
    /// Gets a value indicating whether this money represents zero amount.
    /// </summary>
    public bool IsZero => Amount == 0;

    /// <summary>
    /// Initializes a new instance of the Money class.
    /// </summary>
    /// <param name="amount">The monetary amount.</param>
    /// <param name="currency">The currency code. Defaults to USD if not specified.</param>
    /// <exception cref="ArgumentException">Thrown when amount is negative or currency is invalid.</exception>
    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));
        
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

        Amount = Math.Round(amount, 2);
        Currency = currency.ToUpper();
    }

    /// <summary>
    /// Adds another money object to this one.
    /// </summary>
    /// <param name="other">The money to add.</param>
    /// <returns>A new Money object with the sum.</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies don't match.</exception>
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add different currencies: {Currency} and {other.Currency}");
        
        return new Money(Amount + other.Amount, Currency);
    }

    /// <summary>
    /// Subtracts another money object from this one.
    /// </summary>
    /// <param name="other">The money to subtract.</param>
    /// <returns>A new Money object with the difference.</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies don't match.</exception>
    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot subtract different currencies: {Currency} and {other.Currency}");
        
        return new Money(Amount - other.Amount, Currency);
    }

    /// <summary>
    /// Multiplies this money by an integer quantity.
    /// </summary>
    /// <param name="quantity">The quantity to multiply by.</param>
    /// <returns>A new Money object with the multiplied amount.</returns>
    /// <exception cref="ArgumentException">Thrown when quantity is negative.</exception>
    public Money Multiply(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(quantity));
        
        return new Money(Amount * quantity, Currency);
    }

    /// <summary>
    /// Multiplies this money by a decimal multiplier.
    /// </summary>
    /// <param name="multiplier">The multiplier to multiply by.</param>
    /// <returns>A new Money object with the multiplied amount.</returns>
    /// <exception cref="ArgumentException">Thrown when multiplier is negative.</exception>
    public Money Multiply(decimal multiplier)
    {
        if (multiplier < 0)
            throw new ArgumentException("Multiplier cannot be negative", nameof(multiplier));
        
        return new Money(Amount * multiplier, Currency);
    }

    /// <summary>
    /// Creates a zero money object.
    /// </summary>
    /// <param name="currency">The currency for the zero amount. Defaults to USD.</param>
    /// <returns>A Money object representing zero amount.</returns>
    public static Money Zero(string currency = "USD") => new(0, currency);

    /// <summary>
    /// Gets the equality components for value object comparison.
    /// </summary>
    /// <returns>The components that determine equality.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    /// <summary>
    /// Returns a string representation of this money object.
    /// </summary>
    /// <returns>A formatted string showing amount and currency.</returns>
    public override string ToString()
    {
        return $"{Amount:F2} {Currency}";
    }

    // Comparison operators
    /// <summary>
    /// Determines if this money is greater than another.
    /// </summary>
    /// <param name="left">The first money to compare.</param>
    /// <param name="right">The second money to compare.</param>
    /// <returns>True if left is greater than right.</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies don't match.</exception>
    public static bool operator >(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException($"Cannot compare different currencies: {left.Currency} and {right.Currency}");
        
        return left.Amount > right.Amount;
    }

    /// <summary>
    /// Determines if this money is less than another.
    /// </summary>
    /// <param name="left">The first money to compare.</param>
    /// <param name="right">The second money to compare.</param>
    /// <returns>True if left is less than right.</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies don't match.</exception>
    public static bool operator <(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException($"Cannot compare different currencies: {left.Currency} and {right.Currency}");
        
        return left.Amount < right.Amount;
    }

    /// <summary>
    /// Determines if this money is greater than or equal to another.
    /// </summary>
    /// <param name="left">The first money to compare.</param>
    /// <param name="right">The second money to compare.</param>
    /// <returns>True if left is greater than or equal to right.</returns>
    public static bool operator >=(Money left, Money right)
    {
        return left > right || left == right;
    }

    /// <summary>
    /// Determines if this money is less than or equal to another.
    /// </summary>
    /// <param name="left">The first money to compare.</param>
    /// <param name="right">The second money to compare.</param>
    /// <returns>True if left is less than or equal to right.</returns>
    public static bool operator <=(Money left, Money right)
    {
        return left < right || left == right;
    }

    // Arithmetic operators
    /// <summary>
    /// Adds two money objects.
    /// </summary>
    /// <param name="left">The first money to add.</param>
    /// <param name="right">The second money to add.</param>
    /// <returns>The sum of the two money objects.</returns>
    public static Money operator +(Money left, Money right)
    {
        return left.Add(right);
    }

    /// <summary>
    /// Subtracts one money object from another.
    /// </summary>
    /// <param name="left">The money to subtract from.</param>
    /// <param name="right">The money to subtract.</param>
    /// <returns>The difference of the two money objects.</returns>
    public static Money operator -(Money left, Money right)
    {
        return left.Subtract(right);
    }

    /// <summary>
    /// Multiplies money by an integer.
    /// </summary>
    /// <param name="money">The money to multiply.</param>
    /// <param name="quantity">The quantity to multiply by.</param>
    /// <returns>The product of money and quantity.</returns>
    public static Money operator *(Money money, int quantity)
    {
        return money.Multiply(quantity);
    }

    /// <summary>
    /// Multiplies money by a decimal.
    /// </summary>
    /// <param name="money">The money to multiply.</param>
    /// <param name="multiplier">The multiplier to multiply by.</param>
    /// <returns>The product of money and multiplier.</returns>
    public static Money operator *(Money money, decimal multiplier)
    {
        return money.Multiply(multiplier);
    }
}