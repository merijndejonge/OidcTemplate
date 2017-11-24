using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace OpenSoftware.OidcTemplate.Api
{
    /// <inheritdoc />
    /// <summary>
    /// http://benjii.me/2016/08/global-routes-for-asp-net-core-mvc/
    /// </summary>
    public class ModeRouteConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            var centralPrefix = new AttributeRouteModel(new RouteAttribute("api/{mode}"));
            foreach (var controller in application.Controllers)
            {
                var routeSelector = controller.Selectors.FirstOrDefault(x => x.AttributeRouteModel != null);

                if (routeSelector != null)
                {
                    routeSelector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(centralPrefix,
                        routeSelector.AttributeRouteModel);
                }
                else
                {
                    controller.Selectors.Add(new SelectorModel() { AttributeRouteModel = centralPrefix });
                }
            }
        }
    }
}