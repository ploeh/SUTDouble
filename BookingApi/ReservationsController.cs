using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Web.Http;

namespace Ploeh.Samples.BookingApi
{
    public class ReservationsController : ApiController
    {
        public ReservationsController()
        {
            this.Capacity = 12;
        }

        public int Capacity { get; }

        public IHttpActionResult Post(ReservationDto reservationDto)
        {
            DateTime requestedDate;
            if (!DateTime.TryParse(reservationDto.Date, out requestedDate))
                return this.BadRequest("Invalid date.");

            using (var repo = this.CreateRepository())
            {
                var reservedSeats = repo.ReadReservedSeats(requestedDate);
                if (this.Capacity < reservationDto.Quantity + reservedSeats)
                    return this.StatusCode(HttpStatusCode.Forbidden);

                repo.SaveReservation(requestedDate, reservationDto);

                return this.Ok();
            }
        }

        [AcceptVerbs("")] // Hack to prevent routing from thinking this is an action method
        public virtual SqlReservationsRepository CreateRepository()
        {
            return new SqlReservationsRepository();
        }
    }
}