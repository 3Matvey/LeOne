using FluentValidation;
using LeOne.Application.Auth.Commands.Login;
using LeOne.Application.Auth.Commands.Refresh;
using LeOne.Application.Auth.Commands.Register;
using LeOne.Application.Common.DomainEvents;
using LeOne.Application.Common.Mappings;
using LeOne.Application.Products.Commands.CreateProduct;
using LeOne.Application.Products.Commands.DeleteProduct;
using LeOne.Application.Products.Commands.UpdateProduct;
using LeOne.Application.Products.Queries.GetProductById;
using LeOne.Application.Products.Queries.ListProduct;
using LeOne.Application.Reviews.Commands.CreateReview;
using LeOne.Application.Reviews.Commands.DeleteReview;
using LeOne.Application.Reviews.Commands.UpdateReview;
using LeOne.Application.Reviews.Queries.GetReviewById;
using LeOne.Application.Reviews.Queries.ListReview;
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

            services.AddSingleton<IDomainEventTypeProvider, HardcodedDomainEventTypeProvider>();

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

            // Reviews
            services.AddScoped<IValidator<CreateReviewCommand>, CreateReviewValidator>();
            services.AddScoped<IValidator<UpdateReviewCommand>, UpdateReviewValidator>();

            services.AddScoped<ICreateReview, CreateReviewHandler>();
            services.AddScoped<IUpdateReview, UpdateReviewHandler>();
            services.AddScoped<IDeleteReview, DeleteReviewHandler>();
            services.AddScoped<GetReviewByIdHandler>();
            services.AddScoped<ListReviewHandler>();


            services.AddScoped<IRegisterUser, RegisterUserHandler>();
            services.AddScoped<ILoginUser, LoginUserHandler>();
            services.AddScoped<IRefreshToken, RefreshTokenHandler>();

            return services;
        }
    }
}
