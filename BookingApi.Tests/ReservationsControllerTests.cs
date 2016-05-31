using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
