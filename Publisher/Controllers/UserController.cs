using Dto;
using Microsoft.AspNetCore.Mvc;
using SQSLib.Publish;

namespace Publisher.Controllers;
[Route("[controller]")]
[ApiController]
public class UserController: Controller
{

    private readonly ISqsPublisher _sqsPublisher;

    public UserController(ISqsPublisher sqsPublisher)
    {
        _sqsPublisher = sqsPublisher;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        await _sqsPublisher.PublishAsync("sample-queue",new User
        {
            Id = Guid.NewGuid(),
            Name = "Test"
        }, cancellationToken);

        return Ok();
    }
}