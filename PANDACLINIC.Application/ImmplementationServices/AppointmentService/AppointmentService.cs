using AutoMapper;
using PANDACLINIC.Application.BaseService;
using PANDACLINIC.Application.DTOS.Appointment;
using PANDACLINIC.Application.InterfacesService.AppointmentService;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Domain.InterfaceRepository;
using PANDACLINIC.Shared.Enums;
using PANDACLINIC.Shared.ResultModel;

namespace PANDACLINIC.Application.ImmplementationServices.AppointmentService
{
    public class AppointmentService : BaseService<Appointment, AppointmentSummaryDto, AppointmentDetailDto, AppointmentRequestDto>, IAppointmentService
    {
        public AppointmentService(IUnitOfWork uow, IMapper mapper)
            : base(uow.Appointments, uow, mapper)
        {
        }

        public override async Task<Result<IEnumerable<AppointmentSummaryDto>>> GetAllAsync()
        {
            var appointments = await _uow.Appointments.GetAllWithDetailsAsync();
            var dtos = _mapper.Map<IEnumerable<AppointmentSummaryDto>>(appointments);
            return Result<IEnumerable<AppointmentSummaryDto>>.Success(dtos);
        }

        public override async Task<Result<AppointmentDetailDto>> CreateAsync(AppointmentRequestDto dto)
        {
            var animal = await _uow.Animals.GetByIdAsync(dto.AnimalId);
            if (animal == null)
            {
                return Result<AppointmentDetailDto>.Failure("Animal not found.");
            }

            if (string.IsNullOrWhiteSpace(dto.CreatedBy))
            {
                return Result<AppointmentDetailDto>.Failure("Creator is required.");
            }

            var existingAppointments = await _uow.Appointments.GetByAnimalIdAsync(dto.AnimalId);
            var hasConflict = existingAppointments.Any(a =>
                a.AppointmentDate == dto.AppointmentDate &&
                a.Status != AppointmentStatus.Cancelled);

            if (hasConflict)
            {
                return Result<AppointmentDetailDto>.Failure("This animal already has an appointment at the selected time.");
            }

            var entity = _mapper.Map<Appointment>(dto);
            entity.CreatedBy = dto.CreatedBy.Trim();
            entity.Status = AppointmentStatus.Scheduled;

            await _repository.AddAsync(entity);
            await _uow.CompleteAsync();

            var createdWithDetails = await _uow.Appointments.GetAppointmentWithDetailsAsync(entity.Id);
            return Result<AppointmentDetailDto>.Success(_mapper.Map<AppointmentDetailDto>(createdWithDetails ?? entity));
        }

        public async Task<Result<IEnumerable<AppointmentSummaryDto>>> GetByAnimalIdAsync(Guid animalId)
        {
            var appointments = await _uow.Appointments.GetByAnimalIdAsync(animalId);
            var dtos = _mapper.Map<IEnumerable<AppointmentSummaryDto>>(appointments);
            return Result<IEnumerable<AppointmentSummaryDto>>.Success(dtos);
        }

        public async Task<Result<IEnumerable<AppointmentSummaryDto>>> GetByCreatorAsync(Guid creatorId)
        {
            var appointments = await _uow.Appointments.GetByCreatorAsync(creatorId.ToString());
            var dtos = _mapper.Map<IEnumerable<AppointmentSummaryDto>>(appointments);
            return Result<IEnumerable<AppointmentSummaryDto>>.Success(dtos);
        }

        public async Task<Result<IEnumerable<AppointmentSummaryDto>>> GetByDateAsync(DateTime date)
        {
            var appointments = await _uow.Appointments.GetAppointmentsByDateAsync(date);
            var dtos = _mapper.Map<IEnumerable<AppointmentSummaryDto>>(appointments);
            return Result<IEnumerable<AppointmentSummaryDto>>.Success(dtos);
        }

        public async Task<Result<IEnumerable<AppointmentSummaryDto>>> GetByStatusAsync(AppointmentStatus status)
        {
            var appointments = await _uow.Appointments.GetAppointmentsByStatusAsync(status);
            var dtos = _mapper.Map<IEnumerable<AppointmentSummaryDto>>(appointments);
            return Result<IEnumerable<AppointmentSummaryDto>>.Success(dtos);
        }

        public async Task<Result<IEnumerable<AppointmentSummaryDto>>> GetTodaysScheduleAsync()
        {
            var appointments = await _uow.Appointments.GetTodaysScheduleAsync();
            var dtos = _mapper.Map<IEnumerable<AppointmentSummaryDto>>(appointments);
            return Result<IEnumerable<AppointmentSummaryDto>>.Success(dtos);
        }

        public async Task<Result<AppointmentDetailDto>> GetDetailsAsync(Guid appointmentId)
        {
            var appointment = await _uow.Appointments.GetAppointmentWithDetailsAsync(appointmentId);
            if (appointment == null)
            {
                return Result<AppointmentDetailDto>.Failure("Appointment not found.");
            }

            return Result<AppointmentDetailDto>.Success(_mapper.Map<AppointmentDetailDto>(appointment));
        }

        public async Task<Result> UpdateAsync(AppointmentUpdateDto dto)
        {
            var appointment = await _uow.Appointments.GetByIdAsync(dto.Id);
            if (appointment == null)
            {
                return Result.Failure("Appointment not found.");
            }

            var animal = await _uow.Animals.GetByIdAsync(dto.AnimalId);
            if (animal == null)
            {
                return Result.Failure("Animal not found.");
            }

            var existingAppointments = await _uow.Appointments.GetByAnimalIdAsync(dto.AnimalId);
            var hasConflict = existingAppointments.Any(a =>
                a.Id != dto.Id &&
                a.AppointmentDate == dto.AppointmentDate &&
                a.Status != AppointmentStatus.Cancelled);

            if (hasConflict)
            {
                return Result.Failure("This animal already has an appointment at the selected time.");
            }

            appointment.AnimalId = dto.AnimalId;
            appointment.TypeOfAppoinment = dto.TypeOfAppoinment;
            appointment.AppointmentDate = dto.AppointmentDate;
            appointment.Status = dto.Status;

            _repository.Update(appointment);
            await _uow.CompleteAsync();

            return Result.Success("Appointment updated successfully.");
        }

        public async Task<Result> UpdateStatusAsync(Guid appointmentId, AppointmentStatus status)
        {
            var appointment = await _uow.Appointments.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                return Result.Failure("Appointment not found.");
            }

            appointment.Status = status;
            _repository.Update(appointment);
            await _uow.CompleteAsync();

            return Result.Success("Appointment status updated successfully.");
        }

        public async Task<Result<IEnumerable<AppointmentSummaryDto>>> GetDeletedAppointmentsAsync()
        {
            var deleted = await _uow.Appointments.GetDeletedAppointmentsAsync();
            var dtos = _mapper.Map<IEnumerable<AppointmentSummaryDto>>(deleted);
            return Result<IEnumerable<AppointmentSummaryDto>>.Success(dtos);
        }

        public async Task<Result> RestoreAsync(Guid id)
        {
            var appointment = await _uow.Appointments.GetByIdDeletedAsync(id);
            if (appointment == null)
            {
                return Result.Failure("Appointment not found in archive.");
            }

            appointment.IsDeleted = false;
            appointment.DeletedAt = null;

            await _uow.CompleteAsync();
            return Result.Success("Appointment restored successfully.");
        }
    }
}
