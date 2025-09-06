using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;

namespace LeOne.Application.SpaServices.Commands.DeleteSpaService
{
    public sealed class DeleteSpaServiceHandler(IUnitOfWork uow) : IDeleteSpaService
    {
        public async Task<Result> HandleAsync(DeleteSpaServiceCommand cmd, CancellationToken ct = default)
        {
            var entity = await uow.SpaService.FirstOrDefaultAsync(x => x.Id == cmd.Id, ct);
            if (entity is null)
                return Error.NotFound("SpaService.NotFound", $"SpaService {cmd.Id}");

            try
            {
                await uow.ExecuteInTransactionAsync(async innerCt =>
                {
                    await uow.SpaService.DeleteAsync(entity, innerCt);
                }, ct);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Error.Failure("SpaService.Delete.Failure", ex.Message);
            }
        }
    }
}
