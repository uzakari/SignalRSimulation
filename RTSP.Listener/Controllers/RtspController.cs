using Application.Features.Commands;
using Domain.Models.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RTSP.Listener.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RtspController : ControllerBase
{
    private readonly IMediator _mediator;

    public RtspController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("listen")]
    public async Task<IActionResult> Listen([FromBody] RstpModel rstpModel)
    {
        return Ok(await _mediator.Send(new CaptureFrameCommand(rstpModel.RstpUrl)));
    }
}