using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.DTO.Flight;
using Common.DTO.General;
using Common.DTO.KoiFish;
using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;

namespace Service.Services
{
    public class FlightService : IFlightService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FlightService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<bool> AddFlight(NewFlightDTO model)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDTO> CheckValidationCreateFlight(NewFlightDTO model)
        {
            if (model.DepartureDate > DateTime.Now || model.ArrivalDate > DateTime.Now)
            {
                return new ResponseDTO("The flight must be in the future", 400, false);
            }
            if (model.DepartureDate >  model.ArrivalDate)
            {
                return new ResponseDTO("The DepartureDate must be sooner than ArrivalDate", 400, false);
            }
            var departureAirport = await _unitOfWork.Airport.GetByCondition(a => a.AirportId.Equals(model.DepartureAirportId));
            return new ResponseDTO("The DepartureDate must be sooner than ArrivalDate", 400, false);

        }

        public async Task<ResponseDTO> GetAll()
        {
            var flights = _unitOfWork.Flight
                .GetAllByCondition(f => f.Status.Equals("true"))
                .Include(c => c.ArrivalAirport)
                .Include(c => c.DepartureAirport)
                .ToList();
            if (flights == null)
            {
                return new ResponseDTO("Flight does not exist", 400, false);
            }
            var list = _mapper.Map<List<GetAllFlightDTO>>(flights);
            return new ResponseDTO("Get Flights Sucessfully", 200, true, list);
        }
    }
}
