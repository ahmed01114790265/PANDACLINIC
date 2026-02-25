using Microsoft.AspNetCore.Mvc;
using PANDACLINIC.Application.DTOS.Animal;
using PANDACLINIC.Application.InterfacesService.AnimalService;
using System.Security.Claims;

namespace PANDACLINIC.Web.Controllers
{
    public class AnimalController : Controller
    {
        private readonly IAnimalService _animalService;

        public AnimalController(IAnimalService animalService)
        {
            _animalService = animalService;
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
        public async Task<IActionResult> Create(AnimalRequestDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            dto.UserId = Guid.Parse(userIdClaim!);

            if (!ModelState.IsValid) return View(dto);

            var result = await _animalService.CreateAsync(dto);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Your pet has been registered!";
                return RedirectToAction(nameof(MyAnimals));
            }

            ModelState.AddModelError("", result.Message);
            return View(dto);
        }
       
    }
}

