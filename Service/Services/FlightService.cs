using AutoMapper;
using DAL.Entities;
using DAL.UnitOfWork;
using Service.Interfaces;

namespace Service.Services
{
    public class FlightService : IFlightService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public FlightService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> DeleteFlight(Guid flightId)
        {
            // Check if any orders are using this flight
            Order order = await _unitOfWork.Order
                .GetByCondition(o => o.FlightId == flightId);

            if (order == null)
            {
                Flight flight = await _unitOfWork.Flight.GetByCondition(o => o.FlightId == flightId);
                _unitOfWork.Flight.Delete(flight);
                await _unitOfWork.SaveChangeAsync();
                return true;
            }

            return false;
        }
    }
}