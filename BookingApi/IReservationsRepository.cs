using System;

namespace Ploeh.Samples.BookingApi
{
    public interface IReservationsRepository
    {
        int ReadReservedSeats(DateTime date);
        void SaveReservation(DateTime dateTime, ReservationDto reservationDto);
    }
}