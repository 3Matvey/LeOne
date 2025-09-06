namespace LeOne.Application.SpaServices.Commands.UpdateSpaService
{
    public sealed record ChangePriceCommand(Guid Id, long NewPriceInCents);
}
