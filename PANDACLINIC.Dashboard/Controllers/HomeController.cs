using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PANDACLINIC.Application.InterfacesService.AnimalService;
using PANDACLINIC.Application.InterfacesService.AppointmentService;
using PANDACLINIC.Application.InterfacesService.HostingService;
using PANDACLINIC.Application.InterfacesService.OrderService;
using PANDACLINIC.Application.InterfacesService.ProductService;
using PANDACLINIC.Dashboard.Models;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Shared.Enums;
using System.Diagnostics;

namespace PANDACLINIC.Dashboard.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAnimalService _animalService;
        private readonly IAppointmentService _appointmentService;
        private readonly IHostingService _hostingService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            IAnimalService animalService,
            IAppointmentService appointmentService,
            IHostingService hostingService,
            IProductService productService,
            IOrderService orderService,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _animalService = animalService;
            _appointmentService = appointmentService;
            _hostingService = hostingService;
            _productService = productService;
            _orderService = orderService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardOverviewViewModel
            {
                GreetingName = User.Identity?.Name ?? "Admin",
                GeneratedAt = DateTime.Now
            };

            var isAdmin = User.IsInRole("Admin");

            var animalsResult = await _animalService.GetAllAsync();
            var animals = animalsResult.IsSuccess ? animalsResult.Data ?? Enumerable.Empty<PANDACLINIC.Application.DTOS.Animal.AnimalSummaryDto>() : Enumerable.Empty<PANDACLINIC.Application.DTOS.Animal.AnimalSummaryDto>();
            vm.TotalAnimals = animals.Count();

            var appointmentsResult = await _appointmentService.GetAllAsync();
            var appointments = appointmentsResult.IsSuccess ? appointmentsResult.Data ?? Enumerable.Empty<PANDACLINIC.Application.DTOS.Appointment.AppointmentSummaryDto>() : Enumerable.Empty<PANDACLINIC.Application.DTOS.Appointment.AppointmentSummaryDto>();
            vm.TotalAppointments = appointments.Count();
            vm.TodayAppointments = appointments.Count(a => a.AppointmentDate.Date == DateTime.Today);
            vm.ScheduledAppointments = appointments.Count(a => a.Status == AppointmentStatus.Scheduled);

            var hostingsResult = await _hostingService.GetAllAsync();
            var hostings = hostingsResult.IsSuccess ? hostingsResult.Data ?? Enumerable.Empty<PANDACLINIC.Application.DTOS.Hosting.HostingSummaryDto>() : Enumerable.Empty<PANDACLINIC.Application.DTOS.Hosting.HostingSummaryDto>();
            vm.TotalHostings = hostings.Count();
            vm.OngoingHostings = hostings.Count(h => h.Status == HostingStayStatus.Ongoing);

            var productsResult = await _productService.GetAllAsync();
            var products = productsResult.IsSuccess ? productsResult.Data ?? Enumerable.Empty<PANDACLINIC.Application.DTOS.Product.ProductSummaryDto>() : Enumerable.Empty<PANDACLINIC.Application.DTOS.Product.ProductSummaryDto>();
            vm.TotalProducts = products.Count();
            vm.ActiveProducts = products.Count(p => p.IsActive);
            vm.LowStockProducts = products.Count(p => p.Stock <= 5);
            vm.TotalUsers = await _userManager.Users.CountAsync();

            if (isAdmin)
            {
                var ordersResult = await _orderService.GetAllAsync();
                var orders = ordersResult.IsSuccess ? ordersResult.Data ?? Enumerable.Empty<PANDACLINIC.Application.DTOS.Order.OrderSummaryDto>() : Enumerable.Empty<PANDACLINIC.Application.DTOS.Order.OrderSummaryDto>();
                vm.TotalOrders = orders.Count();
                vm.PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending);
                vm.NewPayments = orders.Count(o => o.PaymentStatus == PaymentStatus.New || o.PaymentStatus == PaymentStatus.InProgress);
                vm.CompletedOrders = orders.Count(o => o.Status == OrderStatus.Completed);
                vm.TotalRevenue = orders.Where(o => o.Status == OrderStatus.Completed).Sum(o => o.TotalAmount);
            }
            else
            {
                vm.TotalOrders = 0;
                vm.PendingOrders = 0;
                vm.NewPayments = 0;
                vm.CompletedOrders = 0;
                vm.TotalRevenue = 0;
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> NavbarKpis()
        {
            if (!(User.Identity?.IsAuthenticated == true) || !User.IsInRole("Admin"))
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Unauthorized"
                });
            }

            var todayAppointmentsCount = 0;
            var ongoingHostingsCount = 0;
            var pendingOrdersCount = 0;
            var paymentFollowUpCount = 0;

            var appointmentsResult = await _appointmentService.GetAllAsync();
            var appointments = appointmentsResult.IsSuccess
                ? appointmentsResult.Data ?? Enumerable.Empty<PANDACLINIC.Application.DTOS.Appointment.AppointmentSummaryDto>()
                : Enumerable.Empty<PANDACLINIC.Application.DTOS.Appointment.AppointmentSummaryDto>();
            todayAppointmentsCount = appointments.Count(a => a.AppointmentDate.Date == DateTime.Today);

            var hostingsResult = await _hostingService.GetAllAsync();
            var hostings = hostingsResult.IsSuccess
                ? hostingsResult.Data ?? Enumerable.Empty<PANDACLINIC.Application.DTOS.Hosting.HostingSummaryDto>()
                : Enumerable.Empty<PANDACLINIC.Application.DTOS.Hosting.HostingSummaryDto>();
            ongoingHostingsCount = hostings.Count(h => h.Status == HostingStayStatus.Ongoing);

            var ordersResult = await _orderService.GetAllAsync();
            var orders = ordersResult.IsSuccess
                ? ordersResult.Data ?? Enumerable.Empty<PANDACLINIC.Application.DTOS.Order.OrderSummaryDto>()
                : Enumerable.Empty<PANDACLINIC.Application.DTOS.Order.OrderSummaryDto>();
            pendingOrdersCount = orders.Count(o => o.Status == OrderStatus.Pending);
            paymentFollowUpCount = orders.Count(o => o.PaymentStatus == PaymentStatus.New || o.PaymentStatus == PaymentStatus.InProgress);

            return Json(new
            {
                isSuccess = true,
                todayAppointmentsCount,
                ongoingHostingsCount,
                pendingOrdersCount,
                paymentFollowUpCount,
                generatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
