using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DTO.General;
using Common.DTO.KoiFish;
using DAL.Entities;

namespace Service.Interfaces
{
    public interface IFlightService
    {
        public Task<bool> DeleteFlight(Guid flightId);
    }
}