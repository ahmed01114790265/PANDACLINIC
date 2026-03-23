using PANDACLINIC.Application.Mapping;
using PANDACLINIC.Application.BaseService;
using PANDACLINIC.Application.DTOS.Hosting;
using PANDACLINIC.Application.InterfacesService.HostingService;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Domain.InterfaceRepository;
using PANDACLINIC.Shared.Enums;
using PANDACLINIC.Shared.ResultModel;

namespace PANDACLINIC.Application.ImmplementationServices.HostingService
{
    public class HostingService : BaseService<HostingStay, HostingSummaryDto, HostingDetailDto, HostingRequestDto>, IHostingService
    {
        public HostingService(IUnitOfWork uow, IMapper mapper)
            : base(uow.HostingStays, uow, mapper)
        {
        }

        public override async Task<Result<IEnumerable<HostingSummaryDto>>> GetAllAsync()
        {
            var stays = await _uow.HostingStays.GetAllWithDetailsAsync();
            return Result<IEnumerable<HostingSummaryDto>>.Success(_mapper.Map<IEnumerable<HostingSummaryDto>>(stays));
        }
        public override async Task<Result<HostingDetailDto>> CreateAsync(HostingRequestDto dto)
        {
            var animal = await _uow.Animals.GetByIdAsync(dto.AnimalId);
            if (animal == null)
            {
                return Result<HostingDetailDto>.Failure("Animal not found.");
            }

            if (string.IsNullOrWhiteSpace(dto.CreatedBy))
            {
                return Result<HostingDetailDto>.Failure("Creator is required.");
            }

            if (dto.CheckOutDate.HasValue && dto.CheckOutDate.Value <= dto.CheckInDate)
            {
                return Result<HostingDetailDto>.Failure("Check-out date must be after check-in date.");
            }

            var effectiveCheckout = dto.CheckOutDate ?? dto.CheckInDate.AddDays(1);

            if (dto.RoomNumber <= 0)
            {
                var availableRooms = await _uow.HostingStays.GetAvailableRoomsAsync(dto.CheckInDate, effectiveCheckout, 20);
                var selectedRoom = availableRooms.FirstOrDefault();
                if (selectedRoom <= 0)
                {
                    return Result<HostingDetailDto>.Failure("No rooms are available for the chosen date range.");
                }
                dto.RoomNumber = selectedRoom;
            }
            else
            {
                var roomAvailable = await _uow.HostingStays.IsRoomAvailableAsync(dto.RoomNumber, dto.CheckInDate, effectiveCheckout);
                if (!roomAvailable)
                {
                    return Result<HostingDetailDto>.Failure("Selected room is not available for the chosen date range.");
                }
            }

            var entity = _mapper.Map<HostingStay>(dto);
            entity.CreatedBy = dto.CreatedBy.Trim();

            await _repository.AddAsync(entity);
            await _uow.CompleteAsync();

            var created = await _uow.HostingStays.GetHostingWithDetailsAsync(entity.Id);
            return Result<HostingDetailDto>.Success(_mapper.Map<HostingDetailDto>(created ?? entity));
        }

        public async Task<Result<IEnumerable<HostingSummaryDto>>> GetByAnimalIdAsync(Guid animalId)
        {
            var stays = await _uow.HostingStays.GetHistoryByAnimalIdAsync(animalId);
            return Result<IEnumerable<HostingSummaryDto>>.Success(_mapper.Map<IEnumerable<HostingSummaryDto>>(stays));
        }

        public async Task<Result<IEnumerable<HostingSummaryDto>>> GetByCreatorAsync(Guid creatorId)
        {
            var stays = await _uow.HostingStays.GetByCreatorAsync(creatorId.ToString());
            return Result<IEnumerable<HostingSummaryDto>>.Success(_mapper.Map<IEnumerable<HostingSummaryDto>>(stays));
        }

        public async Task<Result<IEnumerable<HostingSummaryDto>>> GetByDateAsync(DateTime date)
        {
            var stays = await _uow.HostingStays.GetArrivalsByDateAsync(date);
            return Result<IEnumerable<HostingSummaryDto>>.Success(_mapper.Map<IEnumerable<HostingSummaryDto>>(stays));
        }

        public async Task<Result<IEnumerable<HostingSummaryDto>>> GetByStatusAsync(HostingStayStatus status)
        {
            var stays = await _uow.HostingStays.GetStaysByStatusAsync(status);
            return Result<IEnumerable<HostingSummaryDto>>.Success(_mapper.Map<IEnumerable<HostingSummaryDto>>(stays));
        }

        public async Task<Result<HostingDetailDto>> GetDetailsAsync(Guid hostingId)
        {
            var stay = await _uow.HostingStays.GetHostingWithDetailsAsync(hostingId);
            if (stay == null)
            {
                return Result<HostingDetailDto>.Failure("Hosting stay not found.");
            }

            return Result<HostingDetailDto>.Success(_mapper.Map<HostingDetailDto>(stay));
        }

        public async Task<Result> UpdateAsync(HostingUpdateDto dto)
        {
            var stay = await _uow.HostingStays.GetByIdAsync(dto.Id);
            if (stay == null)
            {
                return Result.Failure("Hosting stay not found.");
            }

            if (dto.CheckOutDate.HasValue && dto.CheckOutDate.Value <= dto.CheckInDate)
            {
                return Result.Failure("Check-out date must be after check-in date.");
            }

            var allStays = await _uow.HostingStays.GetAllAsync();
            var effectiveCheckout = dto.CheckOutDate ?? dto.CheckInDate.AddDays(1);
            var roomConflict = allStays.Any(h =>
                h.Id != dto.Id &&
                h.RoomNumber == dto.RoomNumber &&
                (h.Status == HostingStayStatus.Scheduled || h.Status == HostingStayStatus.Ongoing) &&
                dto.CheckInDate < (h.CheckOutDate ?? DateTime.MaxValue) &&
                effectiveCheckout > h.CheckInDate);

            if (roomConflict)
            {
                return Result.Failure("Selected room is not available for the chosen date range.");
            }

            stay.AnimalId = dto.AnimalId;
            stay.CheckInDate = dto.CheckInDate;
            stay.CheckOutDate = dto.CheckOutDate;
            stay.RoomNumber = dto.RoomNumber;
            stay.Status = dto.Status;

            _repository.Update(stay);
            await _uow.CompleteAsync();
            return Result.Success("Hosting stay updated successfully.");
        }

        public async Task<Result> UpdateStatusAsync(Guid hostingId, HostingStayStatus status)
        {
            var stay = await _uow.HostingStays.GetByIdAsync(hostingId);
            if (stay == null)
            {
                return Result.Failure("Hosting stay not found.");
            }

            stay.Status = status;
            _repository.Update(stay);
            await _uow.CompleteAsync();
            return Result.Success("Hosting status updated successfully.");
        }

        public async Task<Result<IEnumerable<HostingSummaryDto>>> GetDeletedHostingsAsync()
        {
            var deleted = await _uow.HostingStays.GetDeletedHostingsAsync();
            return Result<IEnumerable<HostingSummaryDto>>.Success(_mapper.Map<IEnumerable<HostingSummaryDto>>(deleted));
        }

        public async Task<Result> RestoreAsync(Guid id)
        {
            var stay = await _uow.HostingStays.GetByIdDeletedAsync(id);
            if (stay == null)
            {
                return Result.Failure("Hosting stay not found in archive.");
            }

            stay.IsDeleted = false;
            stay.DeletedAt = null;
            await _uow.CompleteAsync();
            return Result.Success("Hosting stay restored successfully.");
        }
    }
}




