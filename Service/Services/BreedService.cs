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
    public class BreedService : IBreedService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BreedService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddBreed(string breedName)
        {
            Breed breed = new Breed()
            {
                BreedId = Guid.NewGuid(),
                Name = breedName,
            };

            await _unitOfWork.Breed.AddAsync(breed);
            return await _unitOfWork.SaveChangeAsync();
        }

        public List<Breed>? GetAllBreeds()
        {
            return _unitOfWork.Breed.GetAll().ToList();
        }
    }
}
