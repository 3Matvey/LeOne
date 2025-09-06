namespace LeOne.Application.SpaServices.Commands.CreateSpaService
{
    public sealed record CreateSpaServiceCommand(
        string Name,
        long PriceInCents,
        int DurationMinutes,
        string? Description);
}