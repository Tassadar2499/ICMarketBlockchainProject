using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using ICMarkets.Blockchains.Api.Models;
using ICMarkets.Blockchains.Application.Abstractions.Messaging;
using ICMarkets.Blockchains.Application.Common;
using ICMarkets.Blockchains.Application.Snapshots.Commands.CreateAllSnapshots;
using ICMarkets.Blockchains.Application.Snapshots.Commands.CreateSnapshot;
using ICMarkets.Blockchains.Application.Snapshots.Queries.GetSnapshotHistory;
using Microsoft.AspNetCore.Mvc;

namespace ICMarkets.Blockchains.Api.Controllers;

[ApiController]
[Route("api/blockchains")]
public sealed class BlockchainsController(
    ICommandHandler<CreateBlockchainSnapshotCommand, Result<BlockchainSnapshotDto>> createSnapshotHandler,
    ICommandHandler<CreateAllBlockchainSnapshotsCommand, CreateAllBlockchainSnapshotsResult> createAllSnapshotsHandler,
    IQueryHandler<GetBlockchainSnapshotHistoryQuery, Result<PagedResult<BlockchainSnapshotDto>>> getHistoryHandler)
    : ControllerBase
{
    [HttpPost("{chain}/{network}/snapshots")]
    [ProducesResponseType(typeof(BlockchainSnapshotResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BlockchainSnapshotResponse>> CreateSnapshotAsync(
        [FromRoute, Required] string chain,
        [FromRoute, Required] string network,
        CancellationToken cancellationToken)
    {
        var result = await createSnapshotHandler.HandleAsync(
            new CreateBlockchainSnapshotCommand(chain, network),
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(CreateProblemDetails(result.Error));
        }

        var response = ToResponse(result.Value);
        return CreatedAtAction(
            nameof(GetHistoryAsync),
            new { chain = response.Chain, network = response.Network },
            response);
    }

    [HttpPost("snapshots")]
    [ProducesResponseType(typeof(CreateAllSnapshotsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CreateAllSnapshotsResponse>> CreateAllSnapshotsAsync(CancellationToken cancellationToken)
    {
        var result = await createAllSnapshotsHandler.HandleAsync(
            new CreateAllBlockchainSnapshotsCommand(),
            cancellationToken);

        return Ok(new CreateAllSnapshotsResponse(
            result.Snapshots.Select(ToResponse).ToArray(),
            result.Failures.Select(ToFailureResponse).ToArray()));
    }

    [HttpGet("{chain}/{network}/snapshots")]
    [ProducesResponseType(typeof(PagedResponse<BlockchainSnapshotResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<BlockchainSnapshotResponse>>> GetHistoryAsync(
        [FromRoute, Required] string chain,
        [FromRoute, Required] string network,
        [FromQuery, Range(1, int.MaxValue)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await getHistoryHandler.HandleAsync(
            new GetBlockchainSnapshotHistoryQuery(chain, network, page, pageSize),
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(CreateProblemDetails(result.Error));
        }

        var pagedResult = result.Value;
        return Ok(new PagedResponse<BlockchainSnapshotResponse>(
            pagedResult.Items.Select(ToResponse).ToArray(),
            pagedResult.Page,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages));
    }

    private static BlockchainSnapshotResponse ToResponse(BlockchainSnapshotDto snapshot)
    {
        using var document = JsonDocument.Parse(snapshot.RawJson);

        return new BlockchainSnapshotResponse(
            snapshot.Id,
            snapshot.Chain,
            snapshot.Network,
            snapshot.SourceUrl,
            snapshot.CreatedAt,
            document.RootElement.Clone());
    }

    private static SnapshotFailureResponse ToFailureResponse(SnapshotFailureDto failure) =>
        new(failure.Chain, failure.Network, failure.SourceUrl, failure.Error);

    private static ProblemDetails CreateProblemDetails(string? detail) =>
        new()
        {
            Title = "Blockchain snapshot request failed.",
            Detail = detail ?? "The request could not be completed.",
            Status = StatusCodes.Status400BadRequest
        };
}
