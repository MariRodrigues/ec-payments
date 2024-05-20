using Ecommerce.Payments.Features.Responses;
using Stripe.Checkout;

namespace Ecommerce.Payments.Features
{
    public class OrderService : IOrderService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CheckoutOrderResponse> CreateCheckout(OrderRequest request)
        {
            // Obter a URL do servidor
            var serverUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host;

            // Construir a URL de sucesso
            var successUrl = serverUrl + "/order/success?sessionId={CHECKOUT_SESSION_ID}";

            // Obter a URL do referenciador (navegador)
            var refererUrl = _httpContextAccessor.HttpContext.Request.Headers["Referer"].ToString();

            // Construir a URL de cancelamento
            var cancelUrl = !string.IsNullOrEmpty(refererUrl) ? refererUrl + "failed" : successUrl;

            var session = await CheckOut(request, successUrl, cancelUrl);
            var pubKey = _configuration["Stripe:PubKey"];

            return new CheckoutOrderResponse()
            {
                SessionId = session.SessionId,
                SessionUrl = session.Url,
                PubKey = pubKey
            };
        }

        private async Task<CheckoutResponse> CheckOut(OrderRequest request, string successUrl, string cancelUrl)
        {
            var options = new SessionCreateOptions
            {
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                PaymentMethodTypes = new List<string>
                {
                    request.PaymentMethodType
                },

                LineItems = CreateItems(request.Products),
                CustomerEmail = request.CustomerEmail,
                ClientReferenceId = request.ClientReferenceId.ToString(),
                Mode = "payment"
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return new CheckoutResponse()
            {
                SessionId = session.Id,
                Url = session.Url
            };
        }

        private List<SessionLineItemOptions> CreateItems(List<OrderItemsCheckout> request)
        {
            var items = new List<SessionLineItemOptions>();

            foreach (var orderItem in request)
            {
                items.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(orderItem.UnitAmount * 100),
                        Currency = "BRL",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = orderItem.Title,
                            Description = orderItem.Description,
                            Images = new List<string> { orderItem.Url }
                        },
                    },
                    Quantity = orderItem.Quantity
                });
            }

            return items;
        }
    }
}
