namespace DemoApi.Service;

using System.Linq;
using DemoApi.Models;
using DemoApi.Models.Request;
using DemoApi.Queue;
using MediatR;

public class DocumentService(
    IMediator queue
) : IDocumentService
{
    public List<Document> List(AccessLevel? accessLevel = null, string? search = null, PageSort sort = PageSort.Asc, int page = 1, int pageSize = 10)
    {
        var query = Document.Items.AsQueryable();

        if (accessLevel != null)
        {
            query = query.Where(x => x.AccessLevel == accessLevel);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        query = sort switch
        {
            PageSort.Desc => query.OrderByDescending(x => x.AccessLevel),
            _ => query.OrderBy(x => x.CreatedDate),
        };

        query = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return query.ToList();
    }

    public Document? Get(int id)
    {
        var document = Document.Items.FirstOrDefault(x => x.Id == id);

        if (document == null)
        {
            return null;
        }

        return document;
    }

    public Document Create(DocumentRequest request)
    {
        var document = new Document
        {
            Id = Document.Items.Count + 1,
            Name = request.Name,
            AccessLevel = request.AccessLevel!.Value,
        };

        queue.Send(
            new DocumentQueueMessage()
            {
                Request = document,
            });

        return document;
    }

    public Document? Update(int id, DocumentRequest request)
    {
        var document = Document.Items.FirstOrDefault(x => x.Id == id);

        if (document == null)
        {
            return null;
        }

        document.Name = request.Name;
        document.AccessLevel = request.AccessLevel!.Value;

        return document;
    }

    public bool Delete(int id)
    {
        var document = Document.Items.FirstOrDefault(x => x.Id == id);

        if (document == null)
        {
            return false;
        }

        Document.Items.Remove(document);

        return true;
    }
}
