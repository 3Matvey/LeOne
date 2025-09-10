using FluentValidation;
using LeOne.Application.Common.Mappings;
using LeOne.Application.Products.Commands.CreateProduct;
using LeOne.Application.Products.Commands.DeleteProduct;
using LeOne.Application.Products.Commands.UpdateProduct;
using LeOne.Application.Products.Queries.GetProductById;
using LeOne.Application.Products.Queries.ListProduct;
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

            // Spa Service
            // Validators
            services.AddScoped<IValidator<CreateSpaServiceCommand>, CreateSpaServiceValidator>();
            services.AddScoped<IValidator<ChangeSpaServicePriceCommand>, ChangeSpaServicePriceValidator>();

            // Handlers
            services.AddScoped<ICreateSpaService, CreateSpaServiceHandler>();
            services.AddScoped<IChangeSpaServicePrice, ChangeSpaServicePriceHandler>();
            services.AddScoped<IDeleteSpaService, DeleteSpaServiceHandler>();
            services.AddScoped<GetSpaServiceByIdHandler>();
            services.AddScoped<ListSpaServiceHandler>();


            // Product
            // Validators
            services.AddScoped<IValidator<CreateProductCommand>, CreateProductValidator>();
            services.AddScoped<IValidator<ChangeProductPriceCommand>, ChangeProductPriceValidator>();

            // Handlers
            services.AddScoped<ICreateProduct, CreateProductHandler>();
            services.AddScoped<IChangeProductPrice, ChangeProductPriceHandler>();
            services.AddScoped<IDeleteProduct, DeleteProductHandler>();
            services.AddScoped<GetProductByIdHandler>();
            services.AddScoped<ListProductHandler>();

            return services;
        }
    }
}
