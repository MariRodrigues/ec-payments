namespace Ecommerce.Payments.Features
{
    public class OrderRequest
    {
        public List<OrderItemsCheckout> Products { get; set; }
        public string CustomerEmail { get; set; }
        public int ClientReferenceId { get; set; }
        public string PaymentMethodType { get; set; }
        public decimal ShippingCost { get; set; } // Valor do frete
    }

    public class OrderItemsCheckout
    {
        public int ProductId { get; set; }
        public decimal UnitAmount { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public int Quantity { get; set; }
    }
}
