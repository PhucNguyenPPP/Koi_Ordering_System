using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DTO.Flight;
using Common.DTO.General;

namespace Service.Interfaces
{
    public interface IFlightService
    {
        Task<bool> AddFlight(NewFlightDTO model);
        Task<ResponseDTO> CheckValidationCreateFlight(NewFlightDTO model);
        Task<ResponseDTO> CheckValidationUpdateFlight(UpdateFlightDTO model);
        Task<ResponseDTO> GetAll();
        Task<bool> UpdateFlight(UpdateFlightDTO model);
    }
}
