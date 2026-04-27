using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Coucher.WebApi.Swagger;

public sealed class TaskTemplateCreateRequestOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.RequestBody is null)
        {
            return;
        }

        foreach (var content in operation.RequestBody.Content.Values)
        {
            var schemaReferenceId = content.Schema?.Reference?.Id;

            if (schemaReferenceId == "CreateTaskTemplateRequest")
            {
                content.Example = BuildRootExample();

                continue;
            }

            if (schemaReferenceId == "CreateTaskTemplateChildRequest")
            {
                content.Example = BuildChildExample("child-b", "Child B", 1, new[] { "child-a" });
            }
        }
    }

    private static OpenApiObject BuildRootExample()
    {
        return new OpenApiObject
        {
            ["templateKey"] = new OpenApiString("root-template"),
            ["seriesId"] = new OpenApiString("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            ["categoryId"] = new OpenApiString("3fa85f64-5717-4562-b3fc-2c963f66afb0"),
            ["name"] = new OpenApiString("Root Template"),
            ["description"] = new OpenApiString("Top level task template"),
            ["notes"] = new OpenApiString("Optional notes"),
            ["defaultWeeksBeforeExerciseStart"] = new OpenApiInteger(2),
            ["influencerIds"] = new OpenApiArray
            {
                new OpenApiString("3fa85f64-5717-4562-b3fc-2c963f66afc1")
            },
            ["children"] = new OpenApiArray
            {
                BuildChildExample(
                    "child-a",
                    "Child A",
                    1
                ),
                BuildChildExample(
                    "child-b",
                    "Child B",
                    1,
                    new[] { "child-a" }
                )
            }
        };
    }

    private static OpenApiObject BuildChildExample(
        string templateKey,
        string name,
        int defaultWeeksBeforeExerciseStart,
        IEnumerable<string>? dependsOnTemplateKeys = null
    )
    {
        var example = new OpenApiObject
        {
            ["templateKey"] = new OpenApiString(templateKey),
            ["name"] = new OpenApiString(name),
            ["defaultWeeksBeforeExerciseStart"] = new OpenApiInteger(defaultWeeksBeforeExerciseStart)
        };

        if (dependsOnTemplateKeys is not null)
        {
            var dependsOnKeysArray = new OpenApiArray();
            foreach (var dependsOnTemplateKey in dependsOnTemplateKeys)
            {
                dependsOnKeysArray.Add(new OpenApiString(dependsOnTemplateKey));
            }

            example["dependsOnTemplateKeys"] = dependsOnKeysArray;
        }

        return example;
    }
}
