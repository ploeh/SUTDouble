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
            var repo = new Mock<SqlReservationsRepository>();
            var sut = new Mock<ReservationsController> { CallBase = true };
            repo
                .Setup(r => r.ReadReservedSeats(It.IsAny<DateTime>()))
                .Returns(sut.Object.Capacity);
            sut.Setup(s => s.CreateRepository()).Returns(repo.Object);

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
            var repo = new Mock<SqlReservationsRepository>();
            repo
                .Setup(r => r.ReadReservedSeats(new DateTime(2016, 5, 31)))
                .Returns(0);
            repo
                .Setup(r => r.SaveReservation(new DateTime(2016, 5, 31), json))
                .Verifiable();
            var sut = new Mock<ReservationsController> { CallBase = true };
            sut.Setup(s => s.CreateRepository()).Returns(repo.Object);

            var actual = sut.Object.Post(json);

            Assert.IsAssignableFrom<OkResult>(actual);
            repo.Verify();
        }
    }
}
