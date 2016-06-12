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
            var sut = new ReservationsController(
                new Mock<IReservationsRepository>().Object);

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
            var repo = new Mock<IReservationsRepository>();
            var sut = new ReservationsController(repo.Object);
            repo
                .Setup(r => r.ReadReservedSeats(It.IsAny<DateTime>()))
                .Returns(sut.Capacity);

            var actual =
                sut.Post(
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
            var repo = new Mock<IReservationsRepository>();
            repo
                .Setup(r => r.ReadReservedSeats(new DateTime(2016, 5, 31)))
                .Returns(0);
            var sut = new ReservationsController(repo.Object);

            var actual = sut.Post(json);

            Assert.IsAssignableFrom<OkResult>(actual);
            repo.Verify(
                r => r.SaveReservation(new DateTime(2016, 5, 31), json));
        }
    }
}
