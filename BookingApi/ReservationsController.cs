using System;
using System.Web.Http;

namespace Ploeh.Samples.BookingApi
{
    public class ReservationsController : ApiController
    {
        public IHttpActionResult Post(ReservationDto reservationDto)
        {
            return this.BadRequest("Invalid date.");
        }
    }
}