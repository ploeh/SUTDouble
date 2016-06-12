using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Ploeh.Samples.BookingApi
{
    public class PureCompositionRoot : IHttpControllerActivator
    {
        public IHttpController Create(
            HttpRequestMessage request,
            HttpControllerDescriptor controllerDescriptor,
            Type controllerType)
        {
            if(controllerType == typeof(ReservationsController))
            {
                var repo = new SqlReservationsRepository();
                request.RegisterForDispose(repo);
                return new ReservationsController(repo);
            }

            throw new ArgumentException(
                "Unexpected controller type: " + controllerType,
                nameof(controllerType));
        }
    }
}