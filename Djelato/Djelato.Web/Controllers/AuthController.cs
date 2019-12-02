using System;
using System.Threading.Tasks;
using AutoMapper;
using Djelato.Services.Models;
using Djelato.Services.Services.Interfaces;
using Djelato.Web.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Djelato.Web.Controllers
{
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class AuthController : BaseWebApiController
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, IMapper mapper, ILogger<AuthController> logger)
        {
            _authService = authService;
            _mapper = mapper;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthDTO authDTO)
        {
            try
            {
                bool isExist = await _authService.CheckUserAsync(authDTO.Email.ToLower());
                if (!isExist)
                {
                    _logger.LogWarning("User didn't exist in database");
                    _logger.LogTrace("Djelato.Web.Controllers.AuthController.AuthenticationAsync([FromBody] AuthDTO authDTO)");

                    IActionResult result = ClientError("Check email and password please");
                    return result;
                }

                AuthModel model = _mapper.Map<AuthModel>(authDTO);

                var authorizeResult = await _authService.AuthorizeAsync(model);
                if (!authorizeResult.IsSuccessful)
                {
                    IActionResult result = ClientError("Check email and password please");
                    return result;
                }

                var tokenResult = await _authService.GetTokenAsync(model.Email);
                if (tokenResult.IsSuccessful)
                {
                    IActionResult result = Success(tokenResult.Obj, message: tokenResult.Message);
                    return result;
                }
                else
                {
                    IActionResult result = ServerError();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"When user: tried to authenticate, appeared the exception: {ex.Source}");
                _logger.LogError($"Code message for this error: {ex.Message}");
                _logger.LogTrace($"Trace for error: {ex.StackTrace}");

                IActionResult result = ServerError();
                return result;
            }        
        }
    }
}