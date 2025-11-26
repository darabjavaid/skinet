using System;
using System.Reflection.Metadata;
using API.Extensions;
using API.SignalR;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers;

public class PaymentController
    (IPaymentService paymentService, IUnitOfWork unit, ILogger<PaymentController> logger, IConfiguration config, IHubContext<NotificationHub> hubContext) 
    : BaseApiController
{
    private readonly string _whSecret = config["StripeSettings:WhSecret"]!;
    [Authorize]
    [HttpPost("{cartId}")]
    public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
    {
        var cart = await paymentService.CreateOrUpdatePaymentIntent(cartId);

        // if (cart == null) return BadRequest(new ProblemDetails { Title = "Problem creating payment intent" });

        if (cart == null) return BadRequest("Problem with your cart, can't create payment intent");

        return Ok(cart);
    }

    [HttpGet("delivery-methods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
        return Ok(await unit.Repository<DeliveryMethod>().ListAllAsync());
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        logger.LogInformation("**************Received Stripe webhook**************");
        logger.LogInformation($"Stripe Signature: {Request.Headers["Stripe-Signature"]}");

        var json = await new StreamReader(Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = ConstructStripeEvent(json);
            if (stripeEvent.Data.Object is not PaymentIntent intent) return BadRequest("Invalid PaymentIntent object in event data");

            await HandlePaymentIntentEvent(intent);
            return Ok();
        }
        catch (StripeException strEx)
        {
            logger.LogError(strEx, "Error processing Stripe webhook");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error processing webhook");

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "standard error processing webhook");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
       
    }

    private async Task HandlePaymentIntentEvent(PaymentIntent intent)
    {
        if(intent.Status == "succeeded")
        {
            logger.LogInformation($"Payment Succeeded: {intent.Id}");

            var spec = new OrderSpecification(intent.Id, true);
            var order = await unit.Repository<Core.Entities.OrderAggregate.Order>().GetEntityWithSpec(spec);
            if(order == null)
            {
                logger.LogError($"Order not found for PaymentIntent: {intent.Id}");
                throw new Exception($"Order not found for PaymentIntent: {intent.Id}");
            }

            if((long)order.GetTotal() * 100 != intent.AmountReceived)
            {
                order.Status = OrderStatus.PaymentMismatch;
                logger.LogWarning($"Payment amount mismatch for Order Id: {order.Id}, PaymentIntent Id: {intent.Id}");
            }
            else
            {
                order.Status = OrderStatus.PaymentReceived;
            }
            await unit.Complete();


            // todo: send email/notification using signalR etc
            var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);
            if (!string.IsNullOrEmpty(connectionId))
            {
                await hubContext.Clients.Client(connectionId)
                    .SendAsync("OrderCompleteNotification", order.ToDto());
                logger.LogInformation($"Sent payment notification to user {order.BuyerEmail} via SignalR.");
            }

        }
        else if (intent.Status == "payment_failed")
        {
            logger.LogInformation($"Payment Failed: {intent.Id}");
        }
    }

    private Event ConstructStripeEvent(string json)
    {
        try
        {
            return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);// 
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error constructing Stripe event");
            throw new StripeException("Invalid Stripe event - invalid signature", ex);
        }
    }
}