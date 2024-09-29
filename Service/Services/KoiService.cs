using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.DTO.General;
using Common.DTO.KoiFish;
using Common.Enum;
using DAL.Entities;
using DAL.Interfaces;
using DAL.UnitOfWork;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Service.Services
{
    public class KoiService : IKoiService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        public KoiService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageService = imageService;
        }
        public async Task<ResponseDTO> GetAll()
        {
            var koi = _unitOfWork.Koi.GetAllByCondition(c=> c.Status == true);
            if (koi.IsNullOrEmpty())
            {
                return new ResponseDTO("Danh sách trống!", 400, false);
            }

            var list = _mapper.Map<List<Koi>>(koi);
            return new ResponseDTO("Hiển thị danh sách thành công", 200, true, list);
        }

        public async Task<bool> AddKoi(KoiDTO koiDTO)
        {
            var koi = _mapper.Map<Koi>(koiDTO);
            var certificationLink = await _imageService.StoreImageAndGetLink(koiDTO.CertificationLink, "koiCertificate_img");
            koi.KoiId = Guid.NewGuid();
            koi.CertificationLink = certificationLink;
            koi.Status = true;

            await _unitOfWork.Koi.AddAsync(koi);
            return await _unitOfWork.SaveChangeAsync();
        }

        public async Task<ResponseDTO> CheckValidationCreateKoi(KoiDTO koiDTO)
        {
            var breed = _unitOfWork.Breed.GetAllByCondition(c => c.BreedId == koiDTO.BreedId);
            if (breed.IsNullOrEmpty())
            {
                return new ResponseDTO("Giống cá không hợp lệ", 400, false);
            }

            if (koiDTO.Gender != GenderEnum.Male.ToString() && koiDTO.Gender != GenderEnum.Female.ToString())
            {
                return new ResponseDTO("Vui lòng nhập giới tính hợp lệ", 400, false);
            }
            //var farm = _unitOfWork.Farm.GetAllByCondition(c => c.FarmId == koiDTO.FarmId);
            //if (farm.IsNullOrEmpty())
            //{
            //    return new ResponseDTO("Farm không hợp lệ", 400, false);
            //}

            var existedName = _unitOfWork.Koi.GetAll();
            if (existedName.Any(c=> c.Name == koiDTO.Name))
            {
                return new ResponseDTO("Tên koi đã tồn tại!", 400, false);
            }
            /*if(!koiDTO.Gender.Equals(GenderEnum.Female)|| !koiDTO.Gender.Equals(GenderEnum.Male))
            {
                return new ResponseDTO("Vui lòng điền giới tính hợp lệ", 400, false);
            }*/

            return new ResponseDTO("Check thành công", 200, true);
        }

        public async Task<ResponseDTO> DeleteKoi(Guid koiId)
        {
            var koi = await _unitOfWork.Koi.GetByCondition(c => c.KoiId == koiId);
            if(koi == null)
            {
                return new ResponseDTO("Koi không tồn tại!", 400, false);
            }
            koi.Status = false;
            _unitOfWork.Koi.Update(koi);
            var result = await _unitOfWork.SaveChangeAsync();
            if (result)
            {
                return new ResponseDTO("Xóa koi thành công", 200, true, koi.KoiId);
            }
            else
            {
                return new ResponseDTO("Xóa koi không thành công", 500, false, null);
            }
        }

        public async Task<ResponseDTO> UpdateKoi(UpdateKoiDTO updateKoiDTO)
        {
            var koi = _unitOfWork.Koi.GetAllByCondition(c => c.KoiId == updateKoiDTO.KoiId).FirstOrDefault();
            if(koi == null)
            {
                return new ResponseDTO("Koi không tồn tại!", 400, false);
            }
            var certificationLink = await _imageService.StoreImageAndGetLink(updateKoiDTO.CertificationLink, "koiCertificate_img");
            koi.Name = updateKoiDTO.Name;
            koi.CertificationLink = certificationLink;
            koi.Description = updateKoiDTO.Description;
            koi.Dob=updateKoiDTO.Dob;
            koi.Gender = updateKoiDTO.Gender;
            koi.BreedId = updateKoiDTO.BreedId;

            _unitOfWork.Koi.Update(koi);
            var update = await _unitOfWork.SaveChangeAsync();
            if (update)
            {
                return new ResponseDTO("Chỉnh sửa thông tin thành công", 200, true);
            }
            return new ResponseDTO("Chỉnh sửa thông tin thất bại", 500, true);

        }

        public async Task<ResponseDTO> CheckValidationUpdateKoi(UpdateKoiDTO koiDTO)
        {
            var breed = _unitOfWork.Breed.GetAllByCondition(c => c.BreedId == koiDTO.BreedId);
            if (breed.IsNullOrEmpty())
            {
                return new ResponseDTO("Giống cá không hợp lệ", 400, false);
            }

            if (koiDTO.Gender != GenderEnum.Male.ToString() && koiDTO.Gender != GenderEnum.Female.ToString())
            {
                return new ResponseDTO("Vui lòng nhập giới tính hợp lệ", 400, false);
            }
            //var farm = _unitOfWork.Farm.GetAllByCondition(c => c.FarmId == koiDTO.FarmId);
            //if (farm.IsNullOrEmpty())
            //{
            //    return new ResponseDTO("Farm không hợp lệ", 400, false);
            //}

            var existedName = _unitOfWork.Koi.GetAllByCondition(c => c.KoiId != koiDTO.KoiId);
            if (existedName.Any(c=> c.Name == koiDTO.Name))
            {
                return new ResponseDTO("Tên koi đã tồn tại!", 400, false);
            }

            return new ResponseDTO("Check thành công", 200, true);
        }
    }
}