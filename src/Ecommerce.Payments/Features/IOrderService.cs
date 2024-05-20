using Ecommerce.Payments.Features.Responses;

namespace Ecommerce.Payments.Features
{
    public interface IOrderService
    {
        Task<CheckoutOrderResponse> CreateCheckout(OrderRequest request);
    }
}
