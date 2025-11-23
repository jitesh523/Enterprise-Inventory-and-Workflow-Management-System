using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace Inventory.Tests.Domain;

public class OrderTests
{
    [Fact]
    public void Confirm_DraftOrder_ShouldSucceed()
    {
        // Arrange
        var order = new Order
        {
            OrderNumber = "ORD-001",
            CustomerId = 1,
            Status = OrderStatus.Draft
        };
        order.AddLine(1, 10, 100);

        // Act
        order.Confirm();

        // Assert
        order.Status.Should().Be(OrderStatus.Confirmed);
        order.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void Confirm_EmptyOrder_ShouldThrowException()
    {
        // Arrange
        var order = new Order
        {
            OrderNumber = "ORD-002",
            CustomerId = 1,
            Status = OrderStatus.Draft
        };

        // Act & Assert
        Assert.Throws<DomainException>(() => order.Confirm());
    }

    [Fact]
    public void Confirm_NonDraftOrder_ShouldThrowException()
    {
        // Arrange
        var order = new Order
        {
            OrderNumber = "ORD-003",
            CustomerId = 1,
            Status = OrderStatus.Confirmed
        };

        // Act & Assert
        Assert.Throws<DomainException>(() => order.Confirm());
    }

    [Fact]
    public void Ship_PackedOrder_ShouldSucceed()
    {
        // Arrange
        var order = new Order
        {
            OrderNumber = "ORD-004",
            CustomerId = 1,
            Status = OrderStatus.Packed
        };

        // Act
        order.Ship();

        // Assert
        order.Status.Should().Be(OrderStatus.Shipped);
        order.DomainEvents.Should().ContainSingle(e => e is Events.OrderShippedEvent);
    }

    [Fact]
    public void Ship_UnpackedOrder_ShouldThrowException()
    {
        // Arrange
        var order = new Order
        {
            OrderNumber = "ORD-005",
            CustomerId = 1,
            Status = OrderStatus.Confirmed
        };

        // Act & Assert
        Assert.Throws<DomainException>(() => order.Ship());
    }

    [Fact]
    public void AddLine_ToDraftOrder_ShouldSucceed()
    {
        // Arrange
        var order = new Order
        {
            OrderNumber = "ORD-006",
            CustomerId = 1,
            Status = OrderStatus.Draft
        };

        // Act
        order.AddLine(1, 5, 50);

        // Assert
        order.Lines.Should().HaveCount(1);
        order.TotalAmount.Should().Be(250);
    }

    [Fact]
    public void AddLine_ToConfirmedOrder_ShouldThrowException()
    {
        // Arrange
        var order = new Order
        {
            OrderNumber = "ORD-007",
            CustomerId = 1,
            Status = OrderStatus.Confirmed
        };

        // Act & Assert
        Assert.Throws<DomainException>(() => order.AddLine(1, 5, 50));
    }

    [Fact]
    public void Cancel_ShippedOrder_ShouldThrowException()
    {
        // Arrange
        var order = new Order
        {
            OrderNumber = "ORD-008",
            CustomerId = 1,
            Status = OrderStatus.Shipped
        };

        // Act & Assert
        Assert.Throws<DomainException>(() => order.Cancel());
    }

    [Fact]
    public void Cancel_DraftOrder_ShouldSucceed()
    {
        // Arrange
        var order = new Order
        {
            OrderNumber = "ORD-009",
            CustomerId = 1,
            Status = OrderStatus.Draft
        };

        // Act
        order.Cancel();

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
    }
}
