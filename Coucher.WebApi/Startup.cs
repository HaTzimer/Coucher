using Augustus.Infra.Core.Shared.Extensions;
using Coucher.Lib.DAL;
using Coucher.Lib.DAL.Providers;
using Coucher.Lib.Gql;
using Coucher.Lib.Repositories;
using Coucher.Lib.Services;
using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Interfaces.Services;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;

namespace Coucher.WebApi;

public sealed class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGenericAugustusServices();
        services.AddPooledDbContextFactory<CoucherDbContext>(Configuration);

        services.AddScoped<IExerciseProvider, ExerciseProvider>();
        services.AddScoped<IExerciseTaskProvider, ExerciseTaskProvider>();
        services.AddScoped<IUserProfileProvider, UserProfileProvider>();
        services.AddScoped<IClosedListItemProvider, ClosedListItemProvider>();
        services.AddScoped<IFixedTaskTemplateProvider, FixedTaskTemplateProvider>();
        services.AddScoped<IUserNotificationProvider, UserNotificationProvider>();

        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IExerciseTaskRepository, ExerciseTaskRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IClosedListItemRepository, ClosedListItemRepository>();
        services.AddScoped<IFixedTaskTemplateRepository, FixedTaskTemplateRepository>();
        services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();

        services.AddScoped<IExerciseService, ExerciseService>();
        services.AddScoped<IExerciseTaskService, ExerciseTaskService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IClosedListItemService, ClosedListItemService>();
        services.AddScoped<IFixedTaskTemplateService, FixedTaskTemplateService>();
        services.AddScoped<IUserNotificationService, UserNotificationService>();

        var graphQlBuilder = services.AddGraphQLServer();
        graphQlBuilder
            .RegisterDbContext<CoucherDbContext>(DbContextKind.Pooled)
            .AddQueryType<CoucherQuery>()
            .AddProjections()
            .AddFiltering()
            .AddSorting();

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGraphQL("/graphql");
        });
    }
}
