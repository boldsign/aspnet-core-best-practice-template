namespace DemoApi.Queue;

using DemoApi.Models;
using MediatR;

public class DocumentQueueHandler : IRequestHandler<DocumentQueueMessage>
{
    public async Task Handle(DocumentQueueMessage item, CancellationToken cancellationToken)
    {
        Document.Items.Add(item.Request);
    }
}
