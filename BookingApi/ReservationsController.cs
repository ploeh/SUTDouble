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

            using (var conn = this.OpenDbConnection())
            {
                var reservedSeats = this.ReadReservedSeats(conn, requestedDate);
                if (this.Capacity < reservationDto.Quantity + reservedSeats)
                    return this.StatusCode(HttpStatusCode.Forbidden);

                this.SaveReservation(conn, requestedDate, reservationDto);

                return this.Ok();
            }
        }

        [AcceptVerbs("")] // Hack to prevent routing from thinking this is an action method
        public virtual SqlConnection OpenDbConnection()
        {
            var connStr = ConfigurationManager.ConnectionStrings["booking"]
                .ConnectionString;

            var conn = new SqlConnection(connStr);
            try
            {
                conn.Open();
            }
            catch
            {
                conn.Dispose();
                throw;
            }
            return conn;
        }

        public virtual int ReadReservedSeats(SqlConnection conn, DateTime date)
        {
            const string sql = @"
                SELECT COALESCE(SUM([Quantity]), 0) FROM [dbo].[Reservations]
                WHERE YEAR([Date]) = YEAR(@Date)
                AND MONTH([Date]) = MONTH(@Date)
                AND DAY([Date]) = DAY(@Date)";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Date", date));

                return (int)cmd.ExecuteScalar();
            }
        }

        public virtual void SaveReservation(
            SqlConnection conn,
            DateTime dateTime,
            ReservationDto reservationDto)
        {
            const string sql = @"
                INSERT INTO [dbo].[Reservations] ([Date], [Name], [Email], [Quantity])
                VALUES (@Date, @Name, @Email, @Quantity)";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(
                    new SqlParameter("@Date", reservationDto.Date));
                cmd.Parameters.Add(
                    new SqlParameter("@Name", reservationDto.Name));
                cmd.Parameters.Add(
                    new SqlParameter("@Email", reservationDto.Email));
                cmd.Parameters.Add(
                    new SqlParameter("@Quantity", reservationDto.Quantity));

                cmd.ExecuteNonQuery();
            }
        }
    }
}