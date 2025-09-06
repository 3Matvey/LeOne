using AutoMapper;
using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;
using LeOne.Application.SpaServices.Dtos;

namespace LeOne.Application.SpaServices.Queries.GetSpaServiceById
{
    public sealed class GetSpaServiceByIdHandler(
        IUnitOfWork uow,
        IMapper mapper)
    {
        public async Task<Result<SpaServiceDto>> HandleAsync(GetSpaServiceByIdQuery query, CancellationToken ct = default)
        {
            var entity = await uow.SpaService.FirstOrDefaultAsync(x => x.Id == query.Id, ct);
            if (entity is null)
                return Error.NotFound("SpaService.NotFound", $"SpaService {query.Id}");

            var dto = mapper.Map<SpaServiceDto>(entity);

            return Result<SpaServiceDto>.Success(dto);
        }
    }
}
