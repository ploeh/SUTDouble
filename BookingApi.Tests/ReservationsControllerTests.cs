using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Xunit;

namespace Ploeh.Samples.BookingApi.Tests
{
    public class ReservationsControllerTests
    {
        [Fact]
        public void PostReturnsCorrectResultWhenDateisInvalid()
        {
            var sut = new ReservationsController();

            var actual =
                sut.Post(
                    new ReservationDto
                    {
                        Date = "Not a date",
                        Name = "Mark Seemann",
                        Email = "mark@example.com",
                        Quantity = 4
                    });

            var badReq =
                Assert.IsAssignableFrom<BadRequestErrorMessageResult>(actual);
            Assert.Equal("Invalid date.", badReq.Message);
        }

        [Fact]
        public void PostReturnsCorrectResultWhenCapacityIsInsufficient()
        {
            var sut = new Mock<ReservationsController> { CallBase = true };
            sut.Setup(s => s.OpenDbConnection()).Returns((SqlConnection)null);
            sut
                .Setup(s => s.ReadReservedSeats(
                    It.IsAny<SqlConnection>(),
                    It.IsAny<DateTime>()))
                .Returns(sut.Object.Capacity);

            var actual =
                sut.Object.Post(
                    new ReservationDto
                    {
                        Date = "2016-05-31",
                        Name = "Mark Seemann",
                        Email = "mark@example.com",
                        Quantity = 1
                    });

            var statusCode = Assert.IsAssignableFrom<StatusCodeResult>(actual);
            Assert.Equal(HttpStatusCode.Forbidden, statusCode.StatusCode);
        }

        [Fact]
        public void PostReturnsCorrectResultAndHasCorrectStateOnAcceptableRequest()
        {
            var json =
                   new ReservationDto
                   {
                       Date = "2016-05-31",
                       Name = "Mark Seemann",
                       Email = "mark@example.com",
                       Quantity = 1
                   };
            var sut = new Mock<ReservationsController> { CallBase = true };
            sut.Setup(s => s.OpenDbConnection()).Returns((SqlConnection)null);
            sut
                .Setup(s => s.ReadReservedSeats(
                    It.IsAny<SqlConnection>(),
                    new DateTime(2016, 5, 31)))
                .Returns(0);
            sut
                .Setup(s => s.SaveReservation(
                    It.IsAny<SqlConnection>(),
                    new DateTime(2016, 5, 31), json))
                .Verifiable();
            
            var actual = sut.Object.Post(json);

            Assert.IsAssignableFrom<OkResult>(actual);
            sut.Verify();
        }
    }
}
