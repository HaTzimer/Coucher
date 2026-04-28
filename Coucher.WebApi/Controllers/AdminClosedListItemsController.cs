using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.WebApi.Requests.Admin;
using Coucher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Coucher.WebApi.Controllers;

[ApiController]
[Route("api/admin/closed-list-items")]
[ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
public sealed class AdminClosedListItemsController : ControllerBase
{
    private readonly IClosedListItemService _closedListItemService;

    public AdminClosedListItemsController(IClosedListItemService closedListItemService)
    {
        _closedListItemService = closedListItemService;
    }

    [HttpPost]
    public async Task<ActionResult<ClosedListItem>> CreateAsync(
        [FromBody] CreateClosedListItemRequest request,
        CancellationToken cancellationToken
    )
    {
        var closedListItem = await _closedListItemService.CreateClosedListItemAsync(request, cancellationToken);

        return Ok(closedListItem);
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<List<ClosedListItem>>> BulkCreateAsync(
        [FromBody] List<CreateClosedListItemRequest> requests,
        CancellationToken cancellationToken
    )
    {
        var closedListItems = await _closedListItemService.CreateClosedListItemsAsync(requests, cancellationToken);

        return Ok(closedListItems);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClosedListItem>> UpdateAsync(
        Guid id,
        [FromBody] UpdateClosedListItemRequest request,
        CancellationToken cancellationToken
    )
    {
        var closedListItem = await _closedListItemService.UpdateClosedListItemAsync(id, request, cancellationToken);

        return Ok(closedListItem);
    }

    [HttpPut("display-orders")]
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

    [HttpPost("{id:guid}/archive")]
    public async Task<ActionResult<ClosedListItem>> ArchiveAsync(Guid id, CancellationToken cancellationToken)
    {
        var closedListItem = await _closedListItemService.ArchiveClosedListItemAsync(id, cancellationToken);

        return Ok(closedListItem);
    }

    [HttpPost("{id:guid}/unarchive")]
    public async Task<ActionResult<ClosedListItem>> UnarchiveAsync(Guid id, CancellationToken cancellationToken)
    {
        var closedListItem = await _closedListItemService.UnarchiveClosedListItemAsync(id, cancellationToken);

        return Ok(closedListItem);
    }
}
