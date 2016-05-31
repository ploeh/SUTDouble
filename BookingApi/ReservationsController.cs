using System;
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
            if(!DateTime.TryParse(reservationDto.Date, out requestedDate))
                return this.BadRequest("Invalid date.");

            var reservedSeats = this.ReadReservedSeats(requestedDate);
            if(this.Capacity < reservationDto.Quantity + reservedSeats)
                return this.StatusCode(HttpStatusCode.Forbidden);

            this.SaveReservation(requestedDate, reservationDto);

            return this.Ok();
        }

        public virtual int ReadReservedSeats(DateTime date)
        {
            throw new NotImplementedException();
        }

        public virtual void SaveReservation(
            DateTime dateTime,
            ReservationDto reservationDto)
        {
        }
    }
}