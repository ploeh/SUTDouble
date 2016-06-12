using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Web.Http;

namespace Ploeh.Samples.BookingApi
{
    public class ReservationsController : ApiController
    {
        public ReservationsController(IReservationsRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            this.Capacity = 12;
            this.Repository = repository;
        }

        public int Capacity { get; }

        public IReservationsRepository Repository { get; }

        public IHttpActionResult Post(ReservationDto reservationDto)
        {
            DateTime requestedDate;
            if (!DateTime.TryParse(reservationDto.Date, out requestedDate))
                return this.BadRequest("Invalid date.");

            var reservedSeats =
                this.Repository.ReadReservedSeats(requestedDate);
            if (this.Capacity < reservationDto.Quantity + reservedSeats)
                return this.StatusCode(HttpStatusCode.Forbidden);

            this.Repository.SaveReservation(requestedDate, reservationDto);

            return this.Ok();
        }
    }
}