using FluentValidation;
using LeOne.Application.Common.Mappings;
using LeOne.Application.SpaServices.Commands.CreateSpaService;
using LeOne.Application.SpaServices.Commands.DeleteSpaService;
using LeOne.Application.SpaServices.Commands.UpdateSpaService;
using LeOne.Application.SpaServices.Queries.GetSpaServiceById;
using LeOne.Application.SpaServices.Queries.ListSpaService;
using Microsoft.Extensions.DependencyInjection;

namespace LeOne.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // Validators
            services.AddScoped<IValidator<CreateSpaServiceCommand>, CreateSpaServiceValidator>();
            services.AddScoped<IValidator<ChangePriceCommand>, ChangePriceValidator>();

            // Handlers
            services.AddScoped<ICreateSpaService, CreateSpaServiceHandler>();
            services.AddScoped<IChangePrice, ChangePriceHandler>();
            services.AddScoped<IDeleteSpaService, DeleteSpaServiceHandler>();
            services.AddScoped<GetSpaServiceByIdHandler>();
            services.AddScoped<ListSpaServiceHandler>();

            return services;
        }
    }
}
