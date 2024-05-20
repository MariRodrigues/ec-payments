using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace Ecommerce.Payments.Features
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateCheckout([FromBody] OrderRequest request)
        {
            var checkout = await _orderService.CreateCheckout(request);

            if (checkout == null)
                return BadRequest("Não foi possível criar a sessão de checkout.");

            return Ok(checkout);
        }

        [HttpGet("success")]
        public async Task<ActionResult> CheckoutSuccess(string sessionId)
        {
            var sessionService = new SessionService();
            var session = sessionService.Get(sessionId);

            // Here you can save order and customer details to your database.
            var total = session.AmountTotal.Value;
            var customerEmail = session.CustomerDetails.Email;

            // Redirect para o front após os dados do pedido terem sidos salvos
            return Ok("success");
        }
    }
}
