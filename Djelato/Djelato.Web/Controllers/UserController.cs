using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Djelato.Common.Shared;
using Djelato.DataAccess.Repository.Interfaces;
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
    public class UserController : BaseWebApiController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _service;
        private readonly IEmailService _emailSender;
        private readonly IRedisRepository _redis;

        public UserController(ILogger<UserController> logger, IMapper mapper, IUserService service, IEmailService emailSender, IRedisRepository redis)
        {
            _logger = logger;
            _mapper = mapper;
            _service = service;
            _emailSender = emailSender;
            _redis = redis;
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody]UserDTO dto)
        {
            try
            {
                bool isExist = await _service.CheckByEmailAsync(dto.Email);
                if (isExist)
                {
                    _logger.LogInformation($"User {dto.Name} use email which exist in database");
                    var result = ClientError("This email are exist");
                    return result;
                }

                UserModel model = _mapper.Map<UserModel>(dto);
                ServiceResult addResult = await _service.AddAsync(model);
                if (!addResult.IsSuccessful)
                {
                    var result = ServerError();
                    return result;
                }

                var key = Guid.NewGuid();
                await _emailSender.CreateNotification(dto.Email, key);
                 
                bool isCache = await _redis.SetAsync(key.ToString(), dto.Email);
                if (!isCache)
                {
                    var result = Success(null, "Profile created");
                    return result;
                }
                else
                {
                    var result = ServerError();
                    return result;
                }
            }
            catch (MongoWriteException mwEx)
            {
                if (mwEx.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    _logger.LogInformation($"User: -{dto.Name}- tried to register an email which already exist");
                    var result = ClientError("This email are exist");
                    return result;
                }
                else
                {
                    var result = ServerError();
                    return result;
                }                
            }
            catch (Exception ex)
            {
                _logger.LogError($"When user: -{dto.Name}- tried to create new profile, appeared the exception: {ex.Source}");
                _logger.LogInformation($"Code message for this error: {ex.Message}");
                _logger.LogTrace($"Trace for error: {ex.StackTrace}");

                var result = ServerError();
                return result;
            }
        }

        [HttpPost]
        [Route("confirmEmail/{key}")]
        public async Task<IActionResult> ConfirmEmailAsync(string key)
        {
            try
            {
                Match matchKey = Regex.Match(key, RegexExpressions.GuidKeyRgx, RegexOptions.IgnoreCase);
                if (!matchKey.Success)
                {
                    _logger.LogInformation($"confirm key is not the Guid type");
                    var result = ClientError("Email doesn't confirm wrong link to confirm");
                    return result;
                }

                var email = await _redis.GetAsync(key);
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogError($"Redis return empty or null eamil value");
                    _logger.LogTrace($"Trace for error: Djelato.Web.Controllers.CondirmEmailAsync()");

                    var result = ServerError();
                    return result;
                }

                bool isConfirmed = await _service.ConfirmEmailAsync(email);

                if (isConfirmed)
                {
                    var result = Success(null, "Your email wos confirmed. Thank you for your time!");
                    return result;
                }
                else
                {
                    var result = ServerError();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"When user tried confim an eamil, appeared the exception: {ex.Source}");
                _logger.LogInformation($"Code message for this error: {ex.Message}");
                _logger.LogTrace($"Trace for error: {ex.StackTrace}");

                var result = ServerError();
                return result;
            }
        }
    }
}