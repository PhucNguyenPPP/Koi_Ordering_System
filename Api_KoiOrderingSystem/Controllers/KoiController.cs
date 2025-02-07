﻿using Common.DTO.General;
using Microsoft.AspNetCore.Http;    
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Common.DTO.KoiFish;
using Microsoft.AspNetCore.OData.Query;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.Authorization;
using Common.Enum;

namespace Api_KoiOrderingSystem.Controllers
{
    [Route("odata")]
    [ApiController]
    public class KoiController : ODataController
    {
        private readonly IKoiService _koiService;
        public KoiController(IKoiService koiService)
        {
            _koiService = koiService;
        }

        [HttpGet("all-koi")]
        [EnableQuery]
        public async Task<IActionResult> GetAllKoi()
        {
            ResponseDTO responseDTO = await _koiService.GetAll();
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                return BadRequest(responseDTO);

            }
            return Ok(responseDTO.Result);
        }

        [HttpPost("koi")]
        [Authorize(Roles = "KoiFarmManager")]
        public async Task<IActionResult> AddKoi([FromForm] KoiDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDTO(ModelState.ToString() ?? "Unknow error", 400, false, null));
            }

            var checkValid = await _koiService.CheckValidationCreateKoi(model);
            if (!checkValid.IsSuccess)
            {
                return BadRequest(checkValid);
            }

            var signUpResult = await _koiService.AddKoi(model);
            if (signUpResult)
            {
                return Created("Success", new ResponseDTO("Thêm koi thành công", 201, true, null));
            }
            else
            {
                return BadRequest(new ResponseDTO("Thêm koi không thành công", 400, true, null));
            }
        }

        [HttpDelete("koi")]
        [Authorize(Roles = "KoiFarmManager")]
        public async Task<IActionResult> DeleteKoi(Guid koiId)
        {

            ResponseDTO responseDTO = await _koiService.DeleteKoi(koiId);
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                return BadRequest(responseDTO);

            }
            return Ok(responseDTO);
        }

        [HttpPut("koi")]
        [Authorize(Roles = "KoiFarmManager")]
        public async Task<IActionResult> UpdateKoi([FromForm] UpdateKoiDTO updateKoiDTO)
        {
            var checkValid = await _koiService.CheckValidationUpdateKoi(updateKoiDTO);
            if (!checkValid.IsSuccess)
            {
                return BadRequest(checkValid);
            }
            ResponseDTO responseDTO = await _koiService.UpdateKoi(updateKoiDTO);
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                if (responseDTO.StatusCode == 500)
                {
                    return BadRequest(responseDTO);
                }
            }

            return Ok(responseDTO);
        }

        [HttpGet("koi")]
        public async Task<IActionResult> GetKoiByKoiId(Guid koiId)
        {
            ResponseDTO responseDTO = await _koiService.GetKoiByKoiId(koiId);
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                return BadRequest(responseDTO);

            }
            return Ok(responseDTO);
        }

        [HttpGet("all-koi-koifarm")]
        [Authorize(Roles = "KoiFarmManager")]
        [EnableQuery]
        public async Task<IActionResult> GetAllKoiOfFarm([Required] Guid farmId)
        {
            ResponseDTO responseDTO = await _koiService.GetAllKoiOfFarm(farmId);
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                return BadRequest(responseDTO);

            }
            return Ok(responseDTO.Result);
        }
    }
}