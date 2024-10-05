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
    public interface IKoiService
    {
        Task<ResponseDTO> GetAll();
        Task<bool> AddKoi(KoiDTO koiDTO);
        Task<ResponseDTO> CheckValidationCreateKoi(KoiDTO koiDTO);
        Task<ResponseDTO> DeleteKoi(Guid koiId);
        Task<ResponseDTO> UpdateKoi(UpdateKoiDTO updateKoiDTO);
        Task<ResponseDTO> CheckValidationUpdateKoi(UpdateKoiDTO koiDTO);
        Task<bool> CheckKoiExist (Guid koiId);
        Task<ResponseDTO> GetKoiByKoiId (Guid koiId);
    }
}