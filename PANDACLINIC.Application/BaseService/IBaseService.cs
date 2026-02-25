using PANDACLINIC.Domain.Comman.BaseEntity;
using PANDACLINIC.Shared.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Application.BaseService
{
    public interface IBaseService<TEntity, TSummaryDto, TDetailDto, TCreateDto>
    where TEntity : BaseEntity
    {
        Task<Result<TDetailDto>> GetByIdAsync(Guid id);
        Task<Result<IEnumerable<TSummaryDto>>> GetAllAsync();
        Task<Result<TDetailDto>> CreateAsync(TCreateDto dto);
        Task<Result> DeleteAsync(Guid id);
    }
}
