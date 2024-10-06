﻿using AutoMapper;
using Common.DTO.General;
using Common.DTO.KoiFish;
using Common.Enum;
using DAL.Entities;
using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;

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
            var koi = _unitOfWork.Koi
                .GetAllByCondition(c => c.Status == true)
                .Include(c => c.KoiBreeds).ThenInclude(c=> c.Breed)
                .Include(c => c.Farm)
                .ToList();
            if (koi == null)
            {
                return new ResponseDTO("Danh sách trống!", 400, false);
            }
            var list = _mapper.Map<List<GetAllKoiDTO>>(koi);
            return new ResponseDTO("Hiển thị danh sách thành công", 200, true, list);
        }

        public async Task<bool> AddKoi(KoiDTO koiDTO)
        {
            var koi = _mapper.Map<Koi>(koiDTO);
            var certificationLink = await _imageService.StoreImageAndGetLink(koiDTO.CertificationLink, "koiCertificate_img");
            var avatarLink = await _imageService.StoreImageAndGetLink(koiDTO.AvatarLink, "koiAvatar_img");
            var id = Guid.NewGuid();
            koi.KoiId = id;
            koi.CertificationLink = certificationLink;
            koi.AvatarLink = avatarLink;
            koi.FarmId = koiDTO.FarmId;
            koi.Status = true;
            await _unitOfWork.Koi.AddAsync(koi);
            KoiBreed koiBreed = new KoiBreed();
            for (int i = 0; i < koiDTO.BreedId.Count; i++)
            {
                koiBreed.KoiBreedId = Guid.NewGuid();
                koiBreed.KoiId = id;
                koiBreed.BreedId = koiDTO.BreedId[i];
                await _unitOfWork.KoiBreed.AddAsync(koiBreed);
                
                if (i == koiDTO.BreedId.Count - 1)
                {
                    return await _unitOfWork.SaveChangeAsync();
                }
                await _unitOfWork.SaveChangeAsync();
            }

            
            return await _unitOfWork.SaveChangeAsync();
        }

        public async Task<ResponseDTO> CheckValidationCreateKoi(KoiDTO koiDTO)
        {
            for (int i = 0; i < koiDTO.BreedId.Count; i++)
            {
                var breed = _unitOfWork.Breed.GetAllByCondition(c => c.BreedId == koiDTO.BreedId[i]);
                if (breed.IsNullOrEmpty())
                {
                    return new ResponseDTO("Giống cá không hợp lệ", 400, false);
                }
            }

            if (koiDTO.Gender.ToUpper() != GenderEnum.Male.ToString().ToUpper() && koiDTO.Gender.ToUpper() != GenderEnum.Female.ToString().ToUpper())
            {
                return new ResponseDTO("Vui lòng nhập giới tính hợp lệ", 400, false);
            }
            var farm = _unitOfWork.KoiFarm.GetAllByCondition(c => c.KoiFarmId == koiDTO.FarmId);
            if (farm.IsNullOrEmpty())
            {
                return new ResponseDTO("Farm không hợp lệ", 400, false);
            }

            var existedName = _unitOfWork.Koi.GetAll();
            if (existedName.Any(c=> c.Name == koiDTO.Name))
            {
                return new ResponseDTO("Tên koi đã tồn tại!", 400, false);
            }

            return new ResponseDTO("Check thành công", 200, true);
        }

        public async Task<ResponseDTO> DeleteKoi(Guid koiId)
        {
            var koi = await _unitOfWork.Koi.GetByCondition(c => c.KoiId == koiId && c.Status == true);
            if(koi == null)
            {
                return new ResponseDTO("Koi không tồn tại!", 400, false);
            }
            koi.Status = false;
            _unitOfWork.Koi.Update(koi);
            List<KoiBreed> koiBreed =  _unitOfWork.KoiBreed.GetAllByCondition(c => c.KoiId == koiId).ToList();
            
            for(int i = 0; i< koiBreed.Count(); i++)
            {
                _unitOfWork.KoiBreed.Delete(koiBreed[i]);
                if(i == koiBreed.Count() - 1)
                {
                    var result2 = await _unitOfWork.SaveChangeAsync();
                    if (result2)
                    {
                        return new ResponseDTO("Xóa koi thành công", 200, true, koi.KoiId);
                    }
                    else
                    {
                        return new ResponseDTO("Xóa koi không thành công", 500, false, null);
                    }
                }
            }

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
            if (koi == null)
            {
                return new ResponseDTO("Koi không tồn tại!", 400, false);
            }
            var certificationLink = await _imageService.StoreImageAndGetLink(updateKoiDTO.CertificationLink, "koiCertificate_img");
            var avatarLink = await _imageService.StoreImageAndGetLink(updateKoiDTO.AvatarLink, "koiAvatar_img");

            koi.Name = updateKoiDTO.Name;
            koi.CertificationLink = certificationLink;
            koi.AvatarLink = avatarLink;
            koi.Description = updateKoiDTO.Description;
            koi.Dob = updateKoiDTO.Dob;
            koi.Gender = updateKoiDTO.Gender;
            koi.Price = updateKoiDTO.Price;
            
            //koi.BreedId = updateKoiDTO.BreedId;
            await DeleteKoi(updateKoiDTO.KoiId);
            koi.Status = true;
            _unitOfWork.Koi.Update(koi);
            KoiBreed koiBreed = new KoiBreed();
            for (int i = 0; i < updateKoiDTO.BreedId.Count; i++)
            {
                koiBreed.KoiBreedId = Guid.NewGuid();
                koiBreed.KoiId = updateKoiDTO.KoiId;
                koiBreed.BreedId = updateKoiDTO.BreedId[i];
                await _unitOfWork.KoiBreed.AddAsync(koiBreed);

                if (i == updateKoiDTO.BreedId.Count - 1)
                {
                    var update2 = await _unitOfWork.SaveChangeAsync();
                    if (update2)
                    {
                        return new ResponseDTO("Chỉnh sửa thông tin thành công", 200, true);
                    }
                    return new ResponseDTO("Chỉnh sửa thông tin thất bại", 500, true);
                }
                await _unitOfWork.SaveChangeAsync();
            }
            
           
            
            return new ResponseDTO("Chỉnh sửa thông tin thất bại", 500, true);

        }

        public async Task<ResponseDTO> CheckValidationUpdateKoi(UpdateKoiDTO koiDTO)
        {
            for(int i = 0; i < koiDTO.BreedId.Count; i++)
            {
                var breed = _unitOfWork.Breed.GetAllByCondition(c => c.BreedId == koiDTO.BreedId[i]);
                if (breed.IsNullOrEmpty())
                {
                    return new ResponseDTO("Giống cá không hợp lệ", 400, false);
                }
            }

            if (koiDTO.Gender.ToUpper() != GenderEnum.Male.ToString().ToUpper() && koiDTO.Gender.ToUpper() != GenderEnum.Female.ToString().ToUpper())
            {
                return new ResponseDTO("Vui lòng nhập giới tính hợp lệ", 400, false);
            }
           

            var existedName = _unitOfWork.Koi.GetAllByCondition(c => c.KoiId != koiDTO.KoiId);
            if (existedName.Any(c=> c.Name == koiDTO.Name))
            {
                return new ResponseDTO("Tên koi đã tồn tại!", 400, false);
            }

            return new ResponseDTO("Check thành công", 200, true);
        }

        public async Task<bool> CheckKoiExist(Guid koiId)
        {
            var koi = await _unitOfWork.Koi.GetByCondition(c => c.KoiId == koiId);
            if (koi != null)
            {
                return true;
            }
            return false;
        }

        public async Task<ResponseDTO> GetKoiByKoiId(Guid koiId)
        {
            var koi = _unitOfWork.Koi
               .GetAllByCondition(c => c.Status == true && c.KoiId == koiId)
               .Include(c => c.KoiBreeds.FirstOrDefault().Breed)
               .Include(c => c.Farm)
               .FirstOrDefault();

            if (koi == null)
            {
                return new ResponseDTO("Koi does not exist!", 404, false);
            }

            var mapKoi = _mapper.Map<KoiDetailDTO>(koi);
            return new ResponseDTO("Get koi successfully", 200, true, mapKoi);
        }
    }
}