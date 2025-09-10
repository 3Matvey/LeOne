namespace LeOne.Application.SpaServices.Commands.UpdateSpaService
{
    public sealed record ChangeSpaServicePriceCommand(Guid Id, long NewPriceInCents);
}
