using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
namespace SportsStore.Controllers
{
    public class OrderController : Controller
    {
        private IOrderRepository repository;
        private Cart cart;
        private readonly ILogger<OrderController> _logger;
        public OrderController(IOrderRepository repoService, Cart cartService, ILogger<OrderController> logger)
        {
            repository = repoService;
            cart = cartService;
            _logger = logger;
        }
        public ViewResult Checkout() => View(new Order());
        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            try
            {
                if (cart.Lines.Count() == 0)
                {
                    _logger.LogWarning("Checkout attempted with empty cart");
                    ModelState.AddModelError("", "Sorry, your cart is empty!");
                }
                if (ModelState.IsValid)
                {
                    order.Lines = cart.Lines.ToArray();
                    repository.SaveOrder(order);
                    cart.Clear();
                    _logger.LogInformation("Order created: {OrderId} for {CustomerName} in {City}",
                        order.OrderID, order.Name, order.City);
                    return RedirectToPage("/Completed", new { orderId = order.OrderID });
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order for {CustomerName}", order.Name);
                throw;
            }
        }
    }
}