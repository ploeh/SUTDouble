using System;
using System.Net;
using System.Web.Http;

namespace Ploeh.Samples.BookingApi
{
    public class ReservationsController : ApiController
    {
        public int Capacity { get; }

        public IHttpActionResult Post(ReservationDto reservationDto)
        {
            DateTime requestedDate;
            if(!DateTime.TryParse(reservationDto.Date, out requestedDate))
                return this.BadRequest("Invalid date.");

            return this.StatusCode(HttpStatusCode.Forbidden);
        }

        public virtual int ReadReservedSeats(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}