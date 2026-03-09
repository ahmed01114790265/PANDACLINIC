using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PANDACLINIC.Application.DTOS.Animal;
using PANDACLINIC.Application.DTOS.Appointment;
using PANDACLINIC.Application.InterfacesService.AnimalService;
using PANDACLINIC.Application.InterfacesService.AppointmentService;
using PANDACLINIC.Shared.Enums;
using System.Security.Claims;

namespace PANDACLINIC.Dashboard.Controllers
{
    public class AppointmentDashboardController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IAnimalService _animalService;

        public AppointmentDashboardController(IAppointmentService appointmentService, IAnimalService animalService)
        {
            _appointmentService = appointmentService;
            _animalService = animalService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? date = null, AppointmentStatus? status = null, bool mine = false)
        {
            var source = mine
                ? await GetMineAppointmentsAsync()
                : date.HasValue
                    ? await _appointmentService.GetByDateAsync(date.Value)
                    : await _appointmentService.GetAllAsync();

            if (!source.IsSuccess)
            {
                TempData["ErrorMessage"] = source.Message;
                return View(Enumerable.Empty<AppointmentSummaryDto>());
            }

            var data = source.Data ?? Enumerable.Empty<AppointmentSummaryDto>();

            if (date.HasValue)
            {
                data = data.Where(a => a.AppointmentDate.Date == date.Value.Date);
            }

            if (status.HasValue)
            {
                data = data.Where(a => a.Status == status.Value);
            }

            ViewBag.SelectedDate = date?.ToString("yyyy-MM-dd");
            ViewBag.SelectedStatus = status;
            ViewBag.Mine = mine;
            return View(data.OrderByDescending(a => a.AppointmentDate));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _appointmentService.GetDetailsAsync(id);
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
            return View(new AppointmentRequestDto
            {
                AppointmentDate = DateTime.Now.AddHours(1)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadAnimalsAsync();
                return View(dto);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            dto.CreatedBy = userId;

            var result = await _appointmentService.CreateAsync(dto);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Appointment created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault() ?? result.Message);
            await LoadAnimalsAsync();
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _appointmentService.GetDetailsAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            await LoadAnimalsAsync();
            return View(new AppointmentUpdateDto
            {
                Id = result.Data.Id,
                AnimalId = result.Data.AnimalId,
                TypeOfAppoinment = result.Data.TypeOfAppoinment,
                AppointmentDate = result.Data.AppointmentDate,
                Status = result.Data.Status
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppointmentUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadAnimalsAsync();
                return View(dto);
            }

            var result = await _appointmentService.UpdateAsync(dto);
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
        public async Task<IActionResult> UpdateStatus(Guid id, AppointmentStatus status)
        {
            var result = await _appointmentService.UpdateStatusAsync(id, status);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? result.Message
                : result.Errors.FirstOrDefault() ?? result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _appointmentService.DeleteAsync(id);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? "Appointment moved to archive successfully."
                : result.Errors.FirstOrDefault() ?? result.Message;
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> RecycleBin()
        {
            var result = await _appointmentService.GetDeletedAppointmentsAsync();
            return View(result.Data ?? Enumerable.Empty<AppointmentSummaryDto>());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            var result = await _appointmentService.RestoreAsync(id);

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
            var result = await _appointmentService.GetDeletedAppointmentsAsync();
            if (!result.IsSuccess)
            {
                return BadRequest(new
                {
                    result.IsSuccess,
                    result.Message,
                    result.Errors
                });
            }

            return Json(new
            {
                result.IsSuccess,
                result.Message,
                Data = result.Data
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreApi(Guid id)
        {
            var result = await _appointmentService.RestoreAsync(id);
            if (!result.IsSuccess)
            {
                return BadRequest(new
                {
                    result.IsSuccess,
                    result.Message,
                    result.Errors
                });
            }

            return Json(new
            {
                result.IsSuccess,
                result.Message
            });
        }

        private async Task<PANDACLINIC.Shared.ResultModel.Result<IEnumerable<AppointmentSummaryDto>>> GetMineAppointmentsAsync()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return PANDACLINIC.Shared.ResultModel.Result<IEnumerable<AppointmentSummaryDto>>.Failure("User not found.");
            }

            return await _appointmentService.GetByCreatorAsync(userId);
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