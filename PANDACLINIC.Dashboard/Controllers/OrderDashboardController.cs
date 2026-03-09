using Microsoft.AspNetCore.Mvc;
using PANDACLINIC.Application.DTOS.Order;
using PANDACLINIC.Application.InterfacesService.OrderService;
using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Dashboard.Controllers
{
    public class OrderDashboardController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderDashboardController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(OrderStatus? status = null, PaymentStatus? paymentStatus = null, bool? clientConfirmed = null)
        {
            var result = await _orderService.GetAllAsync();
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(Enumerable.Empty<OrderSummaryDto>());
            }

            var data = result.Data ?? Enumerable.Empty<OrderSummaryDto>();
            if (status.HasValue)
                data = data.Where(o => o.Status == status.Value);

            if (paymentStatus.HasValue)
                data = data.Where(o => o.PaymentStatus == paymentStatus.Value);

            if (clientConfirmed.HasValue)
                data = data.Where(o => o.IsClientConfirmed == clientConfirmed.Value);

            ViewBag.SelectedStatus = status;
            ViewBag.SelectedPaymentStatus = paymentStatus;
            ViewBag.SelectedClientConfirmed = clientConfirmed;
            return View(data.OrderByDescending(o => o.CreatedAt));
        }

        [HttpGet]
        public async Task<IActionResult> RecycleBin()
        {
            var result = await _orderService.GetDeletedOrdersAsync();
            return View(result.Data ?? Enumerable.Empty<OrderSummaryDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _orderService.GetDetailsAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, OrderStatus status)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, status);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? result.Message
                : result.Errors.FirstOrDefault() ?? result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePaymentStatus(Guid id, PaymentStatus paymentStatus)
        {
            var result = await _orderService.UpdatePaymentStatusAsync(id, paymentStatus);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? result.Message
                : result.Errors.FirstOrDefault() ?? result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _orderService.DeleteAsync(id);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? result.Message
                : result.Errors.FirstOrDefault() ?? result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            var result = await _orderService.RestoreAsync(id);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? result.Message
                : result.Errors.FirstOrDefault() ?? result.Message;
            return RedirectToAction(result.IsSuccess ? nameof(Index) : nameof(RecycleBin));
        }
    }
}
