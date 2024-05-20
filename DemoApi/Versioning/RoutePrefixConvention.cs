namespace DemoApi.Versioning;

using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

public class RoutePrefixConvention : IApplicationModelConvention
{
    private readonly AttributeRouteModel routePrefix;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutePrefixConvention"/> class.
    /// </summary>
    /// <param name="route">Route.</param>
    public RoutePrefixConvention(IRouteTemplateProvider route)
    {
        this.routePrefix = new AttributeRouteModel(route);
    }

    /// <summary>
    /// Apply.
    /// </summary>
    /// <param name="application">ApplicationModel.</param>
    public void Apply(ApplicationModel application)
    {
        if (application == null)
        {
            throw new ArgumentException(null);
        }

        foreach (var selector in application.Controllers.SelectMany(c => c.Selectors))
        {
            if (selector.AttributeRouteModel != null)
            {
                selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(this.routePrefix, selector.AttributeRouteModel);
            }
            else
            {
                selector.AttributeRouteModel = this.routePrefix;
            }
        }
    }
}
