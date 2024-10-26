using DAL.Entities;
using DAL.UnitOfWork;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AirportService : IAirportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AirportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Airport> GetAllAirports()
        {
            var list = _unitOfWork.Airport.GetAll().ToList();
            return list;
        }
    }
}
