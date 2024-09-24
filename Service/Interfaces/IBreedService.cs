using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IBreedService
    {
        List<Breed>? GetAllBreeds();
        Task<bool> AddBreed(string breedName);
    }
}
