using GM.Mediator.Sample.API.Base;
using GM.Mediator.Sample.Application.Samples.Commands.CreateSample;
using GM.Mediator.Sample.Application.Samples.Commands.UpdateSample;
using Microsoft.AspNetCore.Mvc;

namespace GM.Mediator.Sample.API.Samples;

[ApiController]
[Route("[controller]")]
public class SamplesController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateSampleCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateSampleCommand command, CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);
        return Ok();
    }
}