namespace DemoApi.Controllers;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using DemoApi.Models;
using DemoApi.Models.Request;
using DemoApi.Service;
using DemoSharedLib.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[Route("document")]
[ApiController]
[IgnoreAntiforgeryToken]
[ApiVersion("1")]
[Authorize]
[SwaggerTag("Document management endpoint")]
public class DocumentController(IDocumentService documentService) : Controller
{
    /// <summary>
    /// Retrieves a list of documents with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="accessLevel">Optional access level to filter documents by.</param>
    /// <param name="search">Optional search term to filter documents by name.</param>
    /// <param name="sort">Sort order for the documents. Default is ascending.</param>
    /// <param name="page">Page number for pagination. Default is 1.</param>
    /// <param name="pageSize">Number of documents per page. Default is 10.</param>
    /// <returns>A JSON list of documents.</returns>
    [HttpGet]
    public IActionResult List(
        AccessLevel? accessLevel = null,
        string? search = null,
        PageSort sort = PageSort.Asc,
        int page = 1,
        [Range(1, 20)] int pageSize = 10)
    {
        var items = documentService.List(accessLevel, search, sort, page, pageSize);

        return this.Json(items);
    }

    /// <summary>
    /// Retrieves a document by its ID.
    /// </summary>
    /// <param name="id">The ID of the document to retrieve.</param>
    /// <returns>A JSON representation of the document if found, otherwise a 404 Not Found status.</returns>
    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var document = documentService.Get(id);

        if (document == null)
        {
            return this.NotFound();
        }

        return this.Json(document);
    }

    /// <summary>
    /// Creates a new document. (It creates the document in async queue as we offload the document creation to a background process)
    /// </summary>
    /// <param name="request">The request object containing document details.</param>
    /// <returns>A JSON representation of the created document.</returns>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [HttpPost]
    public IActionResult Create([FromBody] DocumentRequest request)
    {
        var document = documentService.Create(request);

        return this.StatusCode((int)HttpStatusCode.Created, document);
    }

    /// <summary>
    /// Updates an existing document by its ID.
    /// </summary>
    /// <param name="id">The ID of the document to update.</param>
    /// <param name="request">The request object containing updated document details.</param>
    /// <returns>A JSON representation of the updated document if found, otherwise a 404 Not Found status.</returns>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] DocumentRequest request)
    {
        var document = documentService.Update(id, request);

        if (document == null)
        {
            return this.NotFound();
        }

        return this.Json(document);
    }

    /// <summary>
    /// Deletes a document by its ID. (ADMIN ONLY, use 'admin' as username to generate token to access this endpoint)
    /// </summary>
    /// <param name="id">The ID of the document to delete.</param>
    /// <returns>A 204 No Content status if the document is deleted, otherwise a 404 Not Found status.</returns>
    [HttpDelete("{id:int}")]
    [ApiVersion("1-beta")]
    [ApiVersion("1")]
    [Authorize(Policy = AdminRolePolicy.PolicyName)]
    [Description("Accessible only by admin")]
    public IActionResult Delete(int id)
    {
        var document = documentService.Delete(id);

        if (document == null)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }
}
