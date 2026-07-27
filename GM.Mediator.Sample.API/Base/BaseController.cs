using GM.Mediator.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GM.Mediator.Sample.API.Base;

public class BaseController : ControllerBase
{
    protected IMediator Mediator =>
        field ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}