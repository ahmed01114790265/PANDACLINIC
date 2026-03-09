using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PANDACLINIC.Application.DTOS.Animal;
using PANDACLINIC.Application.DTOS.Hosting;
using PANDACLINIC.Application.InterfacesService.AnimalService;
using PANDACLINIC.Application.InterfacesService.HostingService;
using System.Security.Claims;

namespace PANDACLINIC.Web.Controllers
{
    [Authorize]
    public class HostingController : Controller
    {
        private readonly IHostingService _hostingService;
        private readonly IAnimalService _animalService;

        public HostingController(IHostingService hostingService, IAnimalService animalService)
        {
            _hostingService = hostingService;
            _animalService = animalService;
        }

        [HttpGet]
        public async Task<IActionResult> MyHostings()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var result = await _hostingService.GetByCreatorAsync(userId.Value);
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Message;
                return View(Enumerable.Empty<HostingSummaryDto>());
            }

            var hostings = (result.Data ?? Enumerable.Empty<HostingSummaryDto>())
                .OrderByDescending(h => h.CheckInDate)
                .ToList();

            return View(hostings);
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid? animalId = null)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            await LoadOwnerAnimalsAsync(userId.Value);

            var dto = new HostingRequestDto
            {
                CheckInDate = DateTime.Now,
                CheckOutDate = DateTime.Now.AddDays(1)
            };

            if (animalId.HasValue && await UserOwnsAnimalAsync(userId.Value, animalId.Value))
            {
                dto.AnimalId = animalId.Value;
            }

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HostingRequestDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var ownsAnimal = await UserOwnsAnimalAsync(userId.Value, dto.AnimalId);
            if (!ownsAnimal)
            {
                ModelState.AddModelError(nameof(dto.AnimalId), "Selected animal does not belong to your account.");
            }

            dto.CreatedBy = userId.Value.ToString();

            if (string.IsNullOrWhiteSpace(dto.ClientFullName))
            {
                ModelState.AddModelError(nameof(dto.ClientFullName), "Please enter client full name.");
            }

            if (string.IsNullOrWhiteSpace(dto.ClientPhone))
            {
                ModelState.AddModelError(nameof(dto.ClientPhone), "Please enter client phone number.");
            }

            if (!dto.IsClientConfirmed)
            {
                ModelState.AddModelError(nameof(dto.IsClientConfirmed), "Please confirm your information before reservation.");
            }

            if (!ModelState.IsValid)
            {
                await LoadOwnerAnimalsAsync(userId.Value);
                return View(dto);
            }

            var result = await _hostingService.CreateAsync(dto);
            if (result.IsSuccess)
            {
                TempData["Success"] = $"Reservation confirmed for {dto.ClientFullName} ({dto.ClientPhone}).";
                return RedirectToAction(nameof(MyHostings));
            }

            ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault() ?? result.Message);
            await LoadOwnerAnimalsAsync(userId.Value);
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var createdByUser = await UserCreatedHostingAsync(userId.Value, id);
            if (!createdByUser)
            {
                return Forbid();
            }

            var result = await _hostingService.GetDetailsAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(MyHostings));
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var createdByUser = await UserCreatedHostingAsync(userId.Value, id);
            if (!createdByUser)
            {
                return Forbid();
            }

            var result = await _hostingService.DeleteAsync(id);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;

            return RedirectToAction(nameof(MyHostings));
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdClaim)) return null;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private async Task<bool> UserCreatedHostingAsync(Guid userId, Guid hostingId)
        {
            var result = await _hostingService.GetByCreatorAsync(userId);
            var userHostings = result.IsSuccess
                ? result.Data ?? Enumerable.Empty<HostingSummaryDto>()
                : Enumerable.Empty<HostingSummaryDto>();

            return userHostings.Any(h => h.Id == hostingId);
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



