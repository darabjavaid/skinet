using System;
using API.DTOs;
using API.Extensions;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class OrdersController(ICartService cartService, IUnitOfWork unit) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto orderDto)
    {
        var email = User.GetUserEmail();

        var cart = await cartService.GetCartAsync(orderDto.CartId);
        if (cart == null) return BadRequest("Cart not found");
        if (cart.PaymentIntentId == null) return BadRequest("No payment intent associated with the order");
        
        var items = new List<OrderItem>();
        foreach (var item in cart.Items)
        {
            var productItem = await unit.Repository<Product>().GetByIdAsync(item.ProductId);
            if (productItem == null) return BadRequest($"Problem with order. Product with id {item.ProductId} not found");

            var itemOrdered = new ProductItemOrdered
            {
                ProductId = productItem.Id,
                ProductName = productItem.Name,
                PictureUrl = productItem.PictureUrl
            };

            var orderItem = new OrderItem
            {
                ItemOrdered = itemOrdered,
                Price = productItem.Price,
                Quantity = item.Quantity
            };

            items.Add(orderItem);
        }

        var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync(orderDto.DeliveryMethodId);
        if (deliveryMethod == null) return BadRequest("Delivery method not found");

        var order = new Order
        {
            OrderItems = items,
            ShippingAddress = orderDto.ShippingAddress,
            DeliveryMethod = deliveryMethod,
            Subtotal = items.Sum(item => item.Price * item.Quantity),
            PaymentSummary = orderDto.PaymentSummary,
            PaymentIntentId = cart.PaymentIntentId,
            BuyerEmail = email
        };

        unit.Repository<Order>().Add(order);
        if(await unit.Complete())
        {
            return order;
        }

        return BadRequest($"Problem creating order. kindly contact customer service support with your payment intent id: {cart.PaymentIntentId}");
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Order>>> GetOrdersForUser()
    {
        var spec = new OrderSpecification(User.GetUserEmail());
        var orders = await unit.Repository<Order>().ListAsync(spec);
        if (orders == null) return NotFound();
        var ordersDto = orders.Select(order => order.ToDto()).ToList();
        return Ok(ordersDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrderById(int id)
    {
        var spec = new OrderSpecification(User.GetUserEmail(), id);
        var order = await unit.Repository<Order>().GetEntityWithSpec(spec);
        if (order == null) return NotFound();        
        return Ok(order.ToDto());
    }


}
