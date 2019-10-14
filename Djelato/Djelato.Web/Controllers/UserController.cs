using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Djelato.Services.Models;
using Djelato.Services.Services.Interfaces;
using Djelato.Web.ViewModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Djelato.Web.Controllers
{
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _service;

        public UserController(ILogger<UserController> logger, IMapper mapper, IUserService service)
        {
            _logger = logger;
            _mapper = mapper;
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(UserDTO dto)
        {
            try
            {
                ServiceResult checkResult = await _service.CheckByEmailAsync(dto.Email);
                if (checkResult.IsSuccessful)
                {
                    _logger.LogInformation($"User {dto.Name} use email which exist in database");
                    return BadRequest(checkResult.Message);
                }

                UserModel model = _mapper.Map<UserModel>(dto);
                ServiceResult addResult = await _service.AddAsync(model);
                if (addResult.IsSuccessful)
                {
                    return Ok("profile created");
                }
                else
                {
                    return StatusCode(500, addResult.Message);
                }                
            }
            catch (MongoWriteException mwEx)
            {
                if (mwEx.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    _logger.LogInformation($"User: -{dto.Name}- tried to register an email which already exist");
                    return BadRequest("An account with this email and role exist");
                }
                return StatusCode(500, mwEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"User: -{dto.Name}- tried to register a new account, appeared the exception: {ex.Source}");
                _logger.LogInformation($"Code message for this error: {ex.Message}");
                _logger.LogTrace($"Trace for error: {ex.StackTrace}");

                return StatusCode(500, ex.Message);
            }
        }
    }
}