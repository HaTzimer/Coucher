using Augustus.Infra.Core.Shared.Extensions;
using Augustus.Infra.Core.Shared.Interfaces;
using Augustus.Infra.Core.DAL.Redis;
using Coucher.Lib.DAL;
using Coucher.Lib.DAL.Providers;
using Coucher.Lib.Gql;
using Coucher.Lib.Repositories;
using Coucher.Lib.Services;
using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Interfaces.Services;
using Coucher.WebApi.Filters;
using HotChocolate.Data;
using Microsoft.OpenApi.Models;

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
        services.AddHttpContextAccessor();

        services.AddScoped<IExerciseProvider, ExerciseProvider>();
        services.AddScoped<IExerciseTaskProvider, ExerciseTaskProvider>();
        services.AddScoped<IUserProfileProvider, UserProfileProvider>();
        services.AddScoped<IClosedListItemProvider, ClosedListItemProvider>();
        services.AddScoped<ITaskTemplateProvider, TaskTemplateProvider>();
        services.AddScoped<IUserNotificationProvider, UserNotificationProvider>();

        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IExerciseTaskRepository, ExerciseTaskRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IClosedListItemRepository, ClosedListItemRepository>();
        services.AddScoped<ITaskTemplateRepository, TaskTemplateRepository>();
        services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();

        services.AddScoped<IExerciseService, ExerciseService>();
        services.AddScoped<IExerciseTaskService, ExerciseTaskService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IClosedListItemService, ClosedListItemService>();
        services.AddScoped<ITaskTemplateService, TaskTemplateService>();
        services.AddScoped<IUserNotificationService, UserNotificationService>();
        services.AddScoped<IMockSeedService, MockSeedService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IGraphQlCurrentUserService, GraphQlCurrentUserService>();
        services.AddSingleton<IAuthorizationCacheProvider, AuthorizationCacheRedisProvider>();
        services.AddScoped<WebApiSessionAuthenticationFilter>();
        services.AddSingleton<RedisCommunicationFactory>();
        services.AddTransient<IRedisConnection, RedisConnection>();

        var graphQlBuilder = services.AddGraphQLServer();
        graphQlBuilder
            .RegisterDbContext<CoucherDbContext>(DbContextKind.Pooled)
            .AddQueryType<CoucherQuery>()
            .AddProjections()
            .AddFiltering()
            .AddSorting();

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("SessionId", new OpenApiSecurityScheme
            {
                Name = "session-id",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "SessionId"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment() || environment.EnvironmentName.Equals("Local", StringComparison.OrdinalIgnoreCase))
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
