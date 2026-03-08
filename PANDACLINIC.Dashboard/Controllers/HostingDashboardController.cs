using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PANDACLINIC.Application.DTOS.Animal;
using PANDACLINIC.Application.DTOS.Hosting;
using PANDACLINIC.Application.InterfacesService.AnimalService;
using PANDACLINIC.Application.InterfacesService.HostingService;
using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Dashboard.Controllers
{
    public class HostingDashboardController : Controller
    {
        private readonly IHostingService _hostingService;
        private readonly IAnimalService _animalService;

        public HostingDashboardController(IHostingService hostingService, IAnimalService animalService)
        {
            _hostingService = hostingService;
            _animalService = animalService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? date = null, HostingStayStatus? status = null)
        {
            var source = date.HasValue
                ? await _hostingService.GetByDateAsync(date.Value)
                : await _hostingService.GetAllAsync();

            if (!source.IsSuccess)
            {
                TempData["ErrorMessage"] = source.Message;
                return View(Enumerable.Empty<HostingSummaryDto>());
            }

            var data = source.Data ?? Enumerable.Empty<HostingSummaryDto>();
            if (status.HasValue)
            {
                data = data.Where(h => h.Status == status.Value);
            }

            ViewBag.SelectedDate = date?.ToString("yyyy-MM-dd");
            ViewBag.SelectedStatus = status;
            return View(data.OrderByDescending(h => h.CheckInDate));
        }

        [HttpGet]
        public async Task<IActionResult> CurrentlyHosted()
        {
            var result = await _hostingService.GetByStatusAsync(HostingStayStatus.Ongoing);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return View("Index", Enumerable.Empty<HostingSummaryDto>());
            }

            ViewBag.SelectedDate = null;
            ViewBag.SelectedStatus = HostingStayStatus.Ongoing;
            return View("Index", (result.Data ?? Enumerable.Empty<HostingSummaryDto>())
                .OrderByDescending(h => h.CheckInDate));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _hostingService.GetDetailsAsync(id);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadAnimalsAsync();
            return View(new HostingRequestDto
            {
                CheckInDate = DateTime.Now,
                CheckOutDate = DateTime.Now.AddDays(1),
                Status = HostingStayStatus.Scheduled
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HostingRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadAnimalsAsync();
                return View(dto);
            }

            dto.CreatedBy = User.Identity?.Name ?? "Admin";

            var result = await _hostingService.CreateAsync(dto);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Hosting stay created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault() ?? result.Message);
            await LoadAnimalsAsync();
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _hostingService.GetDetailsAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            await LoadAnimalsAsync();
            return View(new HostingUpdateDto
            {
                Id = result.Data.Id,
                AnimalId = result.Data.AnimalId,
                CheckInDate = result.Data.CheckInDate,
                CheckOutDate = result.Data.CheckOutDate,
                RoomNumber = result.Data.RoomNumber,
                Status = result.Data.Status
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HostingUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadAnimalsAsync();
                return View(dto);
            }

            var result = await _hostingService.UpdateAsync(dto);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault() ?? result.Message);
            await LoadAnimalsAsync();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid id, HostingStayStatus status)
        {
            var result = await _hostingService.UpdateStatusAsync(id, status);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? result.Message
                : result.Errors.FirstOrDefault() ?? result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _hostingService.DeleteAsync(id);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? "Hosting stay moved to archive successfully."
                : result.Errors.FirstOrDefault() ?? result.Message;
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> RecycleBin()
        {
            var result = await _hostingService.GetDeletedHostingsAsync();
            return View(result.Data ?? Enumerable.Empty<HostingSummaryDto>());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            var result = await _hostingService.RestoreAsync(id);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.Errors.FirstOrDefault() ?? result.Message;
            return RedirectToAction(nameof(RecycleBin));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ArchiveApi()
        {
            var result = await _hostingService.GetDeletedHostingsAsync();
            if (!result.IsSuccess)
            {
                return BadRequest(new { result.IsSuccess, result.Message, result.Errors });
            }

            return Json(new { result.IsSuccess, result.Message, Data = result.Data });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreApi(Guid id)
        {
            var result = await _hostingService.RestoreAsync(id);
            if (!result.IsSuccess)
            {
                return BadRequest(new { result.IsSuccess, result.Message, result.Errors });
            }

            return Json(new { result.IsSuccess, result.Message });
        }

        private async Task LoadAnimalsAsync()
        {
            var animalsResult = await _animalService.GetAllAsync();
            var animals = animalsResult.IsSuccess
                ? animalsResult.Data ?? Enumerable.Empty<AnimalSummaryDto>()
                : Enumerable.Empty<AnimalSummaryDto>();

            ViewBag.Animals = animals
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                })
                .ToList();
        }
    }
}

