using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.DTO.Flight;
using Common.DTO.General;
using Common.DTO.KoiFish;
using DAL.Entities;
using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;

namespace Service.Services
{
    public class FlightService : IFlightService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public FlightService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> AddFlight(NewFlightDTO model)
        {
            var newFlight = _mapper.Map<Flight>(model);
            newFlight.FlightId = Guid.NewGuid();
            newFlight.Status = true;
            await _unitOfWork.Flight.AddAsync(newFlight);
            var saveChanges = await _unitOfWork.SaveChangeAsync();
            if (saveChanges)
            {
                return true;
            }
            return false;
        }

        public async Task<ResponseDTO> CheckValidationCreateFlight(NewFlightDTO model)
        {
            if (model.DepartureDate < DateTime.Now || model.ArrivalDate < DateTime.Now)
            {
                return new ResponseDTO("The flight must be in the future", 400, false);
            }
            if (model.DepartureDate > model.ArrivalDate)
            {
                return new ResponseDTO("The DepartureDate must be sooner than ArrivalDate", 400, false);
            }
            var departureAirport = await _unitOfWork.Airport.GetByCondition(a => a.AirportId.Equals(model.DepartureAirportId));
            var arrivalAirport = await _unitOfWork.Airport.GetByCondition(a => a.AirportId.Equals(model.ArrivalAirportId));
            if (departureAirport == null)
            {
                return new ResponseDTO("The Departure Airport does not exist", 400, false);
            }
            if (arrivalAirport == null)
            {
                return new ResponseDTO("The Arrival Airport does not exist", 400, false);
            }
            if (arrivalAirport.Country.Equals(departureAirport.Country))
            {
                return new ResponseDTO("2 airport must be in different country", 400, false);
            }
            var checkNameExist = await _unitOfWork.Flight.GetByCondition(f => f.FlightCode.Equals(model.FlightCode));
            if (checkNameExist != null)
            {
                return new ResponseDTO("Flight code is duplicated", 400, false);
            }
            return new ResponseDTO("Validate sucessfully", 200, true);
        }


        public async Task<ResponseDTO> CheckValidationUpdateFlight(UpdateFlightDTO model)
        {
            var checkExist = await CheckFlightExist(model.FlightId);
            if (checkExist == false)
            {
                return new ResponseDTO("Flight does not exist", 400, false);
            }
            var flight = await _unitOfWork.Flight.GetByCondition(f => f.FlightId.Equals(model.FlightId));
            if (model.DepartureDate < DateTime.Now || model.ArrivalDate < DateTime.Now)
            {
                return new ResponseDTO("The flight must be in the future", 400, false);
            }
            if (model.DepartureDate > model.ArrivalDate)
            {
                return new ResponseDTO("The DepartureDate must be sooner than ArrivalDate", 400, false);
            }
            if (model.DepartureDate < flight.DepartureDate || (model.ArrivalDate < flight.ArrivalDate))
            {
                var checkAssign = _unitOfWork.Order.GetAllByCondition(o => o.FlightId.Equals(model.FlightId));
                if (checkAssign.Any())
                {
                    return new ResponseDTO("Cannot update the flight to be sooner because it was assigned", 400, false);
                }
            }
            var departureAirport = await _unitOfWork.Airport.GetByCondition(a => a.AirportId.Equals(model.DepartureAirportId));
            var arrivalAirport = await _unitOfWork.Airport.GetByCondition(a => a.AirportId.Equals(model.ArrivalAirportId));
            if (departureAirport == null)
            {
                return new ResponseDTO("The Departure Airport does not exist", 400, false);
            }
            if (arrivalAirport == null)
            {
                return new ResponseDTO("The Arrival Airport does not exist", 400, false);
            }
            if (arrivalAirport.Country.Equals(departureAirport.Country))
            {
                return new ResponseDTO("2 airport must be in different country", 400, false);
            }
            var checkNameExist = await _unitOfWork.Flight.GetByCondition(f => f.FlightCode.Equals(model.FlightCode) && !f.FlightId.Equals(model.FlightId));
            if (checkNameExist != null)
            {
                return new ResponseDTO("Flight code is duplicated", 400, false);
            }
            return new ResponseDTO("Validate sucessfully", 200, true);
        }

        private async Task<bool> CheckFlightExist(Guid flightId)
        {
            var flight = await _unitOfWork.Flight.GetByCondition(f => f.FlightId.Equals(flightId) && f.Status == true);
            if (flight != null)
            {
                return true;
            }
            return false;
        }

        public async Task<ResponseDTO> GetAll()
        {
            var flights = _unitOfWork.Flight
                .GetAllByCondition(f => f.Status == true)
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

        public async Task<bool> UpdateFlight(UpdateFlightDTO model)
        {
            var flight = await _unitOfWork.Flight.GetByCondition(f => f.FlightId.Equals(model.FlightId));
            flight.ArrivalAirportId = model.ArrivalAirportId;
            flight.DepartureAirportId = model.DepartureAirportId;
            flight.Airline = model.Airline;
            flight.ArrivalDate = model.ArrivalDate;
            flight.DepartureDate = model.DepartureDate;
            flight.Status = true;
            _unitOfWork.Flight.Update(flight);
            var saveChanges = await _unitOfWork.SaveChangeAsync();
            if (saveChanges)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteFlight(Guid flightId)
        {
            // Check if any orders are using this flight
            Order order = await _unitOfWork.Order
                .GetByCondition(o => o.FlightId == flightId);

            if (order == null)
            {
                Flight flight = await _unitOfWork.Flight.GetByCondition(o => o.FlightId == flightId);
                flight.Status = false;
                _unitOfWork.Flight.Update(flight);
                await _unitOfWork.SaveChangeAsync();
                return true;
            }

            return false;
        }

        public async Task<ResponseDTO> GetAllFlightByStorageProvinceId(Guid departureStorageProvinceId, Guid arrivalStorageProvinceId)
        {
            var departureStorageProvince = await _unitOfWork
                .StorageProvince.GetByCondition(c => c.StorageProvinceId == departureStorageProvinceId);

            var arrivalStorageProvince = await _unitOfWork
                .StorageProvince.GetByCondition(c => c.StorageProvinceId == arrivalStorageProvinceId);

            if (departureStorageProvince == null || arrivalStorageProvince == null)
            {
                return new ResponseDTO("Storage does not exist", 400, false);
            }

            var departureAirportId = departureStorageProvince.AirportId;
            var arrivalAirportId = arrivalStorageProvince.AirportId;

            var flights = _unitOfWork.Flight
                .GetAllByCondition(c => c.DepartureAirportId == departureAirportId
                && c.ArrivalAirportId == arrivalAirportId
                && c.DepartureDate > DateTime.Now);

            var list = _mapper.Map<List<GetAllFlightDTO>>(flights);
            return new ResponseDTO("Get Flights Sucessfully", 200, true, list);
        }
    }
}