using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PANDACLINIC.Application.DTOS.Animal;
using PANDACLINIC.Application.DTOS.Appointment;
using PANDACLINIC.Application.InterfacesService.AnimalService;
using PANDACLINIC.Application.InterfacesService.AppointmentService;
using System.Security.Claims;

namespace PANDACLINIC.Web.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IAnimalService _animalService;

        public AppointmentController(IAppointmentService appointmentService, IAnimalService animalService)
        {
            _appointmentService = appointmentService;
            _animalService = animalService;
        }

        [HttpGet]
        public async Task<IActionResult> MyAppointments()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var result = await _appointmentService.GetByCreatorAsync(userId.Value);
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Message;
                return View(Enumerable.Empty<AppointmentSummaryDto>());
            }

            var appointments = (result.Data ?? Enumerable.Empty<AppointmentSummaryDto>())
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();

            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid? animalId = null)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            await LoadOwnerAnimalsAsync(userId.Value);

            var dto = new AppointmentRequestDto();
            if (animalId.HasValue && await UserOwnsAnimalAsync(userId.Value, animalId.Value))
            {
                dto.AnimalId = animalId.Value;
            }

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentRequestDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var ownsAnimal = await UserOwnsAnimalAsync(userId.Value, dto.AnimalId);
            if (!ownsAnimal)
            {
                ModelState.AddModelError(nameof(dto.AnimalId), "Selected animal does not belong to your account.");
            }

            dto.CreatedBy = userId.Value.ToString();

            if (!ModelState.IsValid)
            {
                await LoadOwnerAnimalsAsync(userId.Value);
                return View(dto);
            }

            var result = await _appointmentService.CreateAsync(dto);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Appointment created successfully.";
                return RedirectToAction(nameof(MyAppointments));
            }

            ModelState.AddModelError(string.Empty, result.Message);
            await LoadOwnerAnimalsAsync(userId.Value);
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var createdByUser = await UserCreatedAppointmentAsync(userId.Value, id);
            if (!createdByUser)
            {
                return Forbid();
            }

            var result = await _appointmentService.GetDetailsAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(MyAppointments));
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var createdByUser = await UserCreatedAppointmentAsync(userId.Value, id);
            if (!createdByUser)
            {
                return Forbid();
            }

            var result = await _appointmentService.DeleteAsync(id);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;

            return RedirectToAction(nameof(MyAppointments));
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdClaim)) return null;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private async Task<bool> UserCreatedAppointmentAsync(Guid userId, Guid appointmentId)
        {
            var result = await _appointmentService.GetByCreatorAsync(userId);
            var userAppointments = result.IsSuccess
                ? result.Data ?? Enumerable.Empty<AppointmentSummaryDto>()
                : Enumerable.Empty<AppointmentSummaryDto>();

            return userAppointments.Any(a => a.Id == appointmentId);
        }

        private async Task<bool> UserOwnsAnimalAsync(Guid ownerId, Guid animalId)
        {
            var animalsResult = await _animalService.GetByOwnerAsync(ownerId);
            var ownerAnimals = animalsResult.IsSuccess
                ? animalsResult.Data ?? Enumerable.Empty<AnimalSummaryDto>()
                : Enumerable.Empty<AnimalSummaryDto>();

            return ownerAnimals.Any(a => a.Id == animalId);
        }

        private async Task LoadOwnerAnimalsAsync(Guid ownerId)
        {
            var animalsResult = await _animalService.GetByOwnerAsync(ownerId);
            var ownerAnimals = animalsResult.IsSuccess
                ? animalsResult.Data ?? Enumerable.Empty<AnimalSummaryDto>()
                : Enumerable.Empty<AnimalSummaryDto>();

            ViewBag.Animals = ownerAnimals
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                })
                .ToList();
        }
    }
}
