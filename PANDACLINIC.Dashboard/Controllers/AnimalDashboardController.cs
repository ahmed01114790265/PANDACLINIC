using Microsoft.AspNetCore.Mvc;
using PANDACLINIC.Application.DTOS.Animal;
using PANDACLINIC.Application.InterfacesService.AnimalService;
using System.Security.Claims;

namespace PANDACLINIC.Dashboard.Controllers
{
    public class AnimalDashboardController : Controller
    {
        private readonly IAnimalService _animalService;

        public AnimalDashboardController(IAnimalService animalService)
        {
            _animalService = animalService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int size = 10, string? search = null)
        {
            var result = await _animalService.GetPagedAsync(page, size, search);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(new PagedResponseDto<AnimalSummaryDto>());
            }

            ViewBag.SearchTerm = search;
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _animalService.GetDetailsAsync(id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> CurrentlyHosted()
        {
            var result = await _animalService.GetCurrentlyHostedAsync();
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> RecycleBin()
        {
            var result = await _animalService.GetDeletedAnimalsAsync();
            return View(result.Data);
        }
  
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Restore(Guid id)
        {
            var result = await _animalService.RestoreAsync(id);

            if (result.IsSuccess)
            {
                
                TempData["SuccessMessage"] = result.Message;

                
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(RecycleBin));
        }

        // POST: Dashboard/Animal/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _animalService.DeleteAsync(id);

            if (result.IsSuccess)
                TempData["SuccessMessage"] = "Animal record successfully moved to Recycle Bin.";
            else
                TempData["ErrorMessage"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        // Remote validation for AJAX checks in the Dashboard
        [HttpGet]
        public async Task<IActionResult> IsNameAvailable(string name, Guid ownerId)
        {
            var result = await _animalService.CheckNameAvailabilityAsync(name, ownerId);
            return Json(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> MyAnimals()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Challenge();

            var ownerId = Guid.Parse(userIdClaim);
            var result = await _animalService.GetByOwnerAsync(ownerId);

            return View(result.Data);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] AnimalRequestDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            dto.UserId = Guid.Parse(userIdClaim!);

            if (!ModelState.IsValid) return View(dto);

            var result = await _animalService.CreateAsync(dto);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Your pet has been registered!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", result.Message);
            return View(dto);
        }
    }
}

