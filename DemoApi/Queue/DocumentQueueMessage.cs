namespace DemoApi.Queue;

using DemoApi.Models;
using MediatR;

public class DocumentQueueMessage : IRequest
{
    public Document Request { get; set; }
}
