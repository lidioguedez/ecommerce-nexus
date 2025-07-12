using OrderService.Domain.Common;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Entities;

/// <summary>
/// Represents an item within an order.
/// </summary>
public class OrderItem : Entity
{
    /// <summary>
    /// Gets the product identifier.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Gets the unit price of the product.
    /// </summary>
    public Money UnitPrice { get; private set; }

    /// <summary>
    /// Gets the quantity of the product ordered.
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Gets the total price for this order item (UnitPrice * Quantity).
    /// </summary>
    public Money TotalPrice { get; private set; }

    /// <summary>
    /// Initializes a new instance of the OrderItem class.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="unitPrice">The unit price of the product.</param>
    /// <param name="quantity">The quantity of the product.</param>
    private OrderItem(Guid productId, Money unitPrice, int quantity) : base()
    {
        ProductId = productId;
        UnitPrice = unitPrice;
        Quantity = quantity;
        TotalPrice = unitPrice.Multiply(quantity);
    }

    /// <summary>
    /// Creates a new order item.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="unitPrice">The unit price of the product.</param>
    /// <param name="quantity">The quantity of the product.</param>
    /// <returns>A new order item instance.</returns>
    public static OrderItem Create(Guid productId, Money unitPrice, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        return new OrderItem(productId, unitPrice, quantity);
    }

    /// <summary>
    /// Updates the quantity of this order item.
    /// </summary>
    /// <param name="newQuantity">The new quantity.</param>
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(newQuantity));

        Quantity = newQuantity;
        TotalPrice = UnitPrice.Multiply(newQuantity);
        UpdateTimestamp();
    }
}