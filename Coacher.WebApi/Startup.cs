using Augustus.Infra.Core.Logging.CorrelationId;
using Augustus.Infra.Core.Shared.Extensions;
using Augustus.Infra.Core.Shared.Interfaces;
using Augustus.Infra.Core.DAL.Redis;
using Coacher.Lib.DAL;
using Coacher.Lib.DAL.Cache;
using Coacher.Lib.DAL.Providers;
using Coacher.Lib.Gql;
using Coacher.Lib.Repositories;
using Coacher.Lib.Services;
using Coacher.Shared.Interfaces.DAL.Providers;
using Coacher.Shared.Interfaces.Repositories;
using Coacher.Shared.Interfaces.Services;
using Coacher.WebApi.GraphQl;
using Coacher.WebApi.Swagger;
using Coacher.WebApi.Filters;
using HotChocolate.Data;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace Coacher.WebApi;

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
        services.AddPooledDbContextFactory<CoacherDbContext>(Configuration);
        services.AddHttpContextAccessor();

        services.AddScoped<IExerciseProvider, ExerciseProvider>();
        services.AddScoped<IExerciseTaskProvider, ExerciseTaskProvider>();
        services.AddScoped<IUserProfileProvider, UserProfileProvider>();
        services.AddScoped<IClosedListItemProvider, ClosedListItemProvider>();
        services.AddScoped<ITaskTemplateProvider, TaskTemplateProvider>();
        services.AddScoped<IUserNotificationProvider, UserNotificationProvider>();
        services.AddScoped<IUserRoleProvider, UserRoleProvider>();

        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IExerciseTaskRepository, ExerciseTaskRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IClosedListItemRepository, ClosedListItemRepository>();
        services.AddScoped<ITaskTemplateRepository, TaskTemplateRepository>();
        services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();

        services.AddScoped<IExerciseService, ExerciseService>();
        services.AddScoped<IExerciseTaskService, ExerciseTaskService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IClosedListItemService, ClosedListItemService>();
        services.AddScoped<ITaskTemplateService, TaskTemplateService>();
        services.AddScoped<IUserNotificationService, UserNotificationService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<ICoacherAuthorizationService, CoacherAuthorizationService>();
        services.AddScoped<IMockSeedService, MockSeedService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<ICacheProvider, CacheRedisProvider>();
        services.AddScoped<WebApiSessionAuthenticationFilter>();
        services.AddSingleton<RedisCommunicationFactory>();
        services.AddTransient<IRedisConnection, RedisConnection>();

        var graphQlBuilder = services.AddGraphQLServer();
        graphQlBuilder
            .RegisterDbContext<CoacherDbContext>(DbContextKind.Pooled)
            .AddQueryType<CoacherQuery>()
            .AddProjections()
            .AddFiltering()
            .AddSorting()
            .AddErrorFilter<CoacherGraphQlErrorFilter>();

        services.AddControllers(options =>
        {
            options.Filters.AddService<CorrelationIdResourceFilter>();
            options.Filters.Add<CoacherHttpStatusCodeExceptionFilter>();
        })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Coacher Web API",
                Version = "v1"
            });
            options.OperationFilter<CorrelationHeaderFilter>();
            options.SchemaFilter<TaskTemplateCreateRequestSchemaFilter>();
            options.OperationFilter<TaskTemplateCreateRequestOperationFilter>();
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
