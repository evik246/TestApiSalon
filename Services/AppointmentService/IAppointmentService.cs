﻿using TestApiSalon.Dtos;

namespace TestApiSalon.Services.AppointmentService
{
    public interface IAppointmentService
    {
        Task<Result<IEnumerable<CustomerAppointmentDto>>> GetCustomerAppointments(int customerId, int salonId, Paging paging);
        Task<Result<string>> CreateAppointment(AppointmentCreateDto request);
    }
}
