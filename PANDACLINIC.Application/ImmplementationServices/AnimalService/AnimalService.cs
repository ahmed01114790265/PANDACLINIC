using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PANDACLINIC.Application.BaseService;
using PANDACLINIC.Application.DTOS.Animal;
using PANDACLINIC.Application.FileService;
using PANDACLINIC.Application.InterfacesService.AnimalService;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Domain.InterfaceRepository;
using PANDACLINIC.Shared.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Application.ImmplementationServices.AnimalService
{
    public class AnimalService : BaseService<Animal, AnimalSummaryDto, AnimalDetailDto, AnimalRequestDto>, IAnimalService
    {
        private readonly IFileService _fileService;
        public AnimalService(IUnitOfWork uow, IMapper mapper, IFileService fileService)
            : base(uow.Animals, uow, mapper)
        {
            _fileService = fileService;
        }



        public override async Task<Result<AnimalDetailDto>> CreateAsync(AnimalRequestDto dto)
        {
            if (dto.ImageFile != null)
            {
               
                dto.Imgageurl = await _fileService.UploadFileAsync(dto.ImageFile, "animals");
            }

            var entity = _mapper.Map<Animal>(dto);

            await _repository.AddAsync(entity);
            await _uow.CompleteAsync();

            return Result<AnimalDetailDto>.Success(_mapper.Map<AnimalDetailDto>(entity));
        }


        public override async Task<Result> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null && !string.IsNullOrEmpty(entity.Imgageurl))
            {
                _fileService.DeleteFile(entity.Imgageurl);
            }
            return await base.DeleteAsync(id);
        }

        public async Task<Result<IEnumerable<AnimalSummaryDto>>> GetByOwnerAsync(Guid ownerId)
        {
            var animals = await _uow.Animals.GetAnimalsByOwnerIdAsync(ownerId);
            return Result<IEnumerable<AnimalSummaryDto>>.Success(_mapper.Map<IEnumerable<AnimalSummaryDto>>(animals));
        }

        public async Task<Result<AnimalDetailDto>> GetDetailsAsync(Guid animalId)
        {
            var animal = await _uow.Animals.GetAnimalWithDetailsAsync(animalId);
            if (animal == null) return Result<AnimalDetailDto>.Failure("Animal not found.");
            return Result<AnimalDetailDto>.Success(_mapper.Map<AnimalDetailDto>(animal));
        }

        public async Task<Result<PagedResponseDto<AnimalSummaryDto>>> GetPagedAsync(int page, int size, string? search)
        {
            var (items, total) = await _uow.Animals.GetPagedAnimalsAsync(page, size, search);
            var response = new PagedResponseDto<AnimalSummaryDto>
            {
                Items = _mapper.Map<IEnumerable<AnimalSummaryDto>>(items),
                TotalCount = total,
                PageNumber = page,
                PageSize = size
            };
            return Result<PagedResponseDto<AnimalSummaryDto>>.Success(response);
        }

        public async Task<Result<IEnumerable<AnimalSummaryDto>>> GetCurrentlyHostedAsync()
        {
            var animals = await _uow.Animals.GetAnimalsCurrentlyInHostingAsync();
            return Result<IEnumerable<AnimalSummaryDto>>.Success(_mapper.Map<IEnumerable<AnimalSummaryDto>>(animals));
        }

        public async Task<Result<bool>> CheckNameAvailabilityAsync(string name, Guid ownerId)
        {
            bool exists = await _uow.Animals.IsNameExistsForOwnerAsync(name, ownerId);
            return Result<bool>.Success(!exists);
        }

        public async Task<Result<IEnumerable<AnimalSummaryDto>>> GetDeletedAnimalsAsync()
        {
            // 1. Call the specialized repository method that uses .IgnoreQueryFilters()
            var deletedAnimals = await _uow.Animals.GetDeletedAnimalsAsync();

            // 2. Map the entities to DTOs
            var dtos = _mapper.Map<IEnumerable<AnimalSummaryDto>>(deletedAnimals);

            // 3. Return a successful Result
            return Result<IEnumerable<AnimalSummaryDto>>.Success(dtos);
        }

        
        public async Task<Result> RestoreAsync(Guid id)
        {
          
            var animal = await _uow.Animals.GetByIdDeletedAsync(id);

            if (animal == null)
                return Result.Failure("Animal not found in Recycle Bin.");

            animal.IsDeleted = false;
            animal.DeletedAt = null;

            // Use the Unit of Work to persist changes
            await _uow.CompleteAsync();


            return Result.Success("Animal restored successfully.");
        }
    }
}
