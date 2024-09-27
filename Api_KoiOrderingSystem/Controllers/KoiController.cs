﻿using Common.DTO.General;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Services;
using Common.DTO.KoiFish;

namespace Api_KoiOrderingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KoiController : ControllerBase
    {
        private readonly IKoiService _koiService;
        public KoiController(IKoiService koiService)
        {
            _koiService = koiService;
        }

        [HttpGet("Koi")]
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
            return Ok(responseDTO);
        }

        [HttpPost("new-koi")]
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
                return Ok(new ResponseDTO("Đăng kí thành công", 200, true, null));
            }
            else
            {
                return BadRequest(new ResponseDTO("Đăng kí không thành công", 400, true, null));
            }
        }

        [HttpDelete("delete-koi")]
        public async Task<IActionResult> DeleteKoi([FromBody] Guid koiId)
        {
            ResponseDTO responseDTO = await _koiService.deleteKoi(koiId);
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

    }
}