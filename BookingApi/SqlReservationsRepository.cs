using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ploeh.Samples.BookingApi
{
    public class SqlReservationsRepository :
        IReservationsRepository, IDisposable
    {
        private readonly Lazy<SqlConnection> lazyConn;

        public SqlReservationsRepository()
        {
            this.lazyConn = new Lazy<SqlConnection>(this.OpenSqlConnection);
        }

        private SqlConnection OpenSqlConnection()
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

        public virtual int ReadReservedSeats(DateTime date)
        {
            const string sql = @"
                SELECT COALESCE(SUM([Quantity]), 0) FROM [dbo].[Reservations]
                WHERE YEAR([Date]) = YEAR(@Date)
                AND MONTH([Date]) = MONTH(@Date)
                AND DAY([Date]) = DAY(@Date)";

            using (var cmd = new SqlCommand(sql, this.lazyConn.Value))
            {
                cmd.Parameters.Add(new SqlParameter("@Date", date));

                return (int)cmd.ExecuteScalar();
            }
        }

        public virtual void SaveReservation(
            DateTime dateTime,
            ReservationDto reservationDto)
        {
            const string sql = @"
                INSERT INTO [dbo].[Reservations] ([Date], [Name], [Email], [Quantity])
                VALUES (@Date, @Name, @Email, @Quantity)";

            using (var cmd = new SqlCommand(sql, this.lazyConn.Value))
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

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                this.lazyConn.Value.Dispose();
        }
    }
}