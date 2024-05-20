namespace DemoApi.Service;

using DemoApi.Models;
using DemoApi.Models.Request;

public interface IDocumentService
{
    List<Document> List(AccessLevel? accessLevel = null, string? search = null, PageSort sort = PageSort.Asc, int page = 1, int pageSize = 10);

    Document? Get(int id);

    Document Create(DocumentRequest request);

    Document? Update(int id, DocumentRequest request);

    bool Delete(int id);
}
