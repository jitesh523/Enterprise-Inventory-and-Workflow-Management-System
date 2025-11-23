using Inventory.Application.Services;
using Inventory.Domain.Entities;
using Inventory.Web.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Web.Controllers;

[AuthorizePermission("Permissions.Orders.View")]
public class OrdersController : Controller
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [AuthorizePermission("Permissions.Orders.Create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [AuthorizePermission("Permissions.Orders.Create")]
    public async Task<IActionResult> Create(Order order)
    {
        if (ModelState.IsValid)
        {
            await _orderService.CreateOrderAsync(order);
            return RedirectToAction(nameof(Index));
        }
        return View(order);
    }

    [AuthorizePermission("Permissions.Orders.Confirm")]
    public async Task<IActionResult> Confirm(int id)
    {
        await _orderService.ConfirmOrderAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [AuthorizePermission("Permissions.Orders.Ship")]
    public async Task<IActionResult> Ship(int id)
    {
        await _orderService.ShipOrderAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
