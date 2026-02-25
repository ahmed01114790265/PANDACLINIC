using AutoMapper;
using PANDACLINIC.Domain.Comman.BaseEntity;
using PANDACLINIC.Domain.Comman.GenericRepository;
using PANDACLINIC.Domain.InterfaceRepository;
using PANDACLINIC.Shared.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Application.BaseService
{
    public abstract class BaseService<TEntity, TSummaryDto, TDetailDto, TCreateDto>
    : IBaseService<TEntity, TSummaryDto, TDetailDto, TCreateDto>
    where TEntity : BaseEntity
    {
        protected readonly IGenericRepository<TEntity> _repository;
        protected readonly IUnitOfWork _uow;
        protected readonly IMapper _mapper;

        protected BaseService(IGenericRepository<TEntity> repository, IUnitOfWork uow, IMapper mapper)
        {
            _repository = repository;
            _uow = uow;
            _mapper = mapper;
        }
        public virtual async Task<Result<TDetailDto>> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
                return Result<TDetailDto>.Failure("Record not found.");

            return Result<TDetailDto>.Success(_mapper.Map<TDetailDto>(entity));
        }

        public virtual async Task<Result<IEnumerable<TSummaryDto>>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return Result<IEnumerable<TSummaryDto>>.Success(_mapper.Map<IEnumerable<TSummaryDto>>(entities));
        }

        public virtual async Task<Result<TDetailDto>> CreateAsync(TCreateDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);

            await _repository.AddAsync(entity);
            await _uow.CompleteAsync();

            return Result<TDetailDto>.Success(_mapper.Map<TDetailDto>(entity));
        }

        public virtual async Task<Result> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
                return Result.Failure("Record not found.");

            _repository.Delete(entity);

            await _uow.CompleteAsync();

            return Result.Success("Record deleted successfully.");
        }
    }
}
