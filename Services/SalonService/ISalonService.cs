﻿using TestApiSalon.Dtos.Other;
using TestApiSalon.Dtos.Salon;
using TestApiSalon.Dtos.Schedule;
using TestApiSalon.Models;

namespace TestApiSalon.Services.SalonService
{
    public interface ISalonService
    {
        Task<Result<IEnumerable<Salon>>> GetAllSalons(Paging paging);
        Task<Result<IEnumerable<Salon>>> GetSalonsInCity(int cityId, Paging paging);
        Task<Result<SalonWithAddress>> GetSalonWithAddressById(int salonId);
        Task<Result<Salon>> GetSalonById(int salonId);
        Task<Result<string>> CreateSalon(SalonCreateDto request);
        Task<Result<string>> UpdateSalon(int salonId, SalonChangeDto request);
        Task<Result<string>> DeleteSalon(int salonId);
        Task<Result<decimal>> GetSalonIncome(int salonId, DateRangeDto dateRange);
    }
}
