using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.DAL.Admin;
using Coacher.Shared.Models.WebApi.Requests.Admin;
using Coacher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Coacher.WebApi.Controllers;

[ApiController]
[Route("api/admin/closed-list-item")]
[ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[SwaggerTag("Administrative closed-list catalog management endpoints.")]
public sealed class AdminClosedListItemsController : ControllerBase
{
    private readonly IClosedListItemService _closedListItemService;

    public AdminClosedListItemsController(IClosedListItemService closedListItemService)
    {
        _closedListItemService = closedListItemService;
    }

    [HttpPost("create/single")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ClosedListItem), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Create one closed-list item.",
        Description = "Creates a single administrative closed-list entry."
    )]
    public async Task<ActionResult<ClosedListItem>> CreateAsync(
        [FromBody] CreateClosedListItemRequest request,
        CancellationToken cancellationToken
    )
    {
        var closedListItem = await _closedListItemService.CreateClosedListItemAsync(request, cancellationToken);

        return Ok(closedListItem);
    }

    [HttpPost("create/bulk")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(List<ClosedListItem>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Create many closed-list items.",
        Description = "Creates multiple administrative closed-list entries in one request."
    )]
    public async Task<ActionResult<List<ClosedListItem>>> BulkCreateAsync(
        [FromBody] List<CreateClosedListItemRequest> requests,
        CancellationToken cancellationToken
    )
    {
        var closedListItems = await _closedListItemService.CreateClosedListItemsAsync(requests, cancellationToken);

        return Ok(closedListItems);
    }

    [HttpPut("update/{id:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ClosedListItem), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Update root closed-list item fields.",
        Description = "Updates only the non-null request fields of a single closed-list item."
    )]
    public async Task<ActionResult<ClosedListItem>> UpdateAsync(
        Guid id,
        [FromBody] UpdateClosedListItemRequest request,
        CancellationToken cancellationToken
    )
    {
        var closedListItem = await _closedListItemService.UpdateClosedListItemAsync(id, request, cancellationToken);

        return Ok(closedListItem);
    }

    [HttpPut("update/display-orders")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(List<ClosedListItem>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Bulk update display orders.",
        Description = "Updates display ordering for multiple closed-list items in one request."
    )]
    public async Task<ActionResult<List<ClosedListItem>>> BulkUpdateDisplayOrdersAsync(
        [FromBody] BulkUpdateClosedListItemDisplayOrdersRequest request,
        CancellationToken cancellationToken
    )
    {
        var closedListItems = await _closedListItemService.BulkUpdateClosedListItemDisplayOrdersAsync(
            request,
            cancellationToken
        );

        return Ok(closedListItems);
    }

    [HttpPut("archive/{id:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ClosedListItem), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Set closed-list item archive state.",
        Description = "Archives the item when the boolean body is true and restores it when the body is false."
    )]
    public async Task<ActionResult<ClosedListItem>> SetArchiveStateAsync(
        Guid id,
        [FromBody] bool isArchived,
        CancellationToken cancellationToken
    )
    {
        var closedListItem = await _closedListItemService.SetClosedListItemArchiveStateAsync(
            id,
            isArchived,
            cancellationToken
        );

        return Ok(closedListItem);
    }
}
