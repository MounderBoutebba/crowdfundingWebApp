using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Coiner.Business.Models;
using Coiner.Business.Services;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using log4net;
using System.Reflection;
using Coiner.Business.LoggerService;
using Coiner.Controllers.ModelsDto;
using Coiner.Business;

namespace Coiner.Controllers
{
    [Produces("application/json")]
    [Route("api/Users")]
    public class UserController : Controller
    {
        UserService _userService = new UserService();
        private static readonly ILog Log = LogManager.GetLogger(typeof(UserService));

        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                if (await _userService.CreateUser(user))
                {
                    return Ok();
                }
                else
                {
                    return new StatusCodeResult(403);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("CreateUserWithGoogle")]
        public async Task<IActionResult> CreateUserWithGoogle([FromBody] User user)
        {
            try
            {
                if (await _userService.CreateUserWithGoogle(user))
                {
                    return Ok();
                }
                else
                {
                    return new StatusCodeResult(403);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("VerifyUser/{id}/{token}")]
        public IActionResult VerifyAccount(string id, string token)
        {
            //decode the user Id frome base64
            try
            {
                id = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(id));
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());

            }

                var url = Constants.BaseUrl + "/activation-compte/";
            try
            {
                if (_userService.VerifyAccount(Int32.Parse(id), token))
                {
                    

                    return Redirect(url+ "changePasswordSuccess");
                }
                else
                {
                    return Redirect(url + "changePasswordFail");
                }


            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return Redirect(url + "changePasswordError");
            }
        }

        [HttpGet]
        [Route("LoginUserWithGogle/{email}")]
        public IActionResult LoginUserWithGogle(string email)
        {
            try
            {

                var coiner = _userService.LoginUserWithGogle(email);
                var active = (coiner != null) ? _userService.IsUserActive(coiner.Id) : false;
                var token = (coiner != null) ? RequestToken(email, email) : null;
                var adminLgin = _userService.GetLoginFromConfig();
                return Ok(new object[] {
                    coiner,
                    token,
                    adminLgin,
                    active
                });
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("LoginUser/{login}/{password}")]
        public IActionResult LoginUser(string login, string password)
        {
            try
            {

                var coiner = _userService.loginUser(login, password);
                var active = (coiner != null) ? _userService.IsUserActive(coiner.Id) : false;
                var token = (coiner != null) ? RequestToken(login, password) : null;
                var adminLgin = _userService.GetLoginFromConfig();
                return Ok(new object[] {
                    coiner,
                    token,
                    adminLgin,
                    active
                });
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("GetUser")]
        public IActionResult GetUser([FromBody] UserCheckTokenDto userDto)
        {
            try
            {
                if (userDto.Token != null)
                {
                    if (VerifyToken(userDto.Token))
                    {
                        var user = _userService.GetUser(userDto.UserId);
                        return Ok(user);
                    }
                    else
                    {
                        return Ok(null);
                    }
                }
                else
                {
                    return Ok(null);
                }

            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("FetchImageContent/{userId}")]
        public IActionResult FetchImageContent(int userId)
        {
            try
            {
                var getImageContent = _userService.FetchImageContent(userId);
                return Ok(getImageContent);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("UpdateUser")]
        public IActionResult UpdateUser([FromBody] User user)
        {
            try
            {
                var modifiedUser = _userService.UpdateUser(user);
                if (modifiedUser != null)
                {
                    return Ok(modifiedUser);
                }
                else
                {
                    return new StatusCodeResult(403);
                }

            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("SendEmailToUpdatePassword/{email}")]
        public IActionResult SendEmailToUpdatePassword(string email)
        {
            try
            {
                if (_userService.SendEmailToUpdatePassword(HttpContext, email))
                {
                    return Ok();
                }
                else
                {
                    return new StatusCodeResult(403);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("UpdateUserPassword/{id}/{pw}")]
        public IActionResult UpdateUserPassword(string id, string pw)
        {
            try
            {
                if (_userService.UpdateUserPassword(id, pw))
                {
                    return Ok();

                }
                else
                {
                    return new StatusCodeResult(404);

                }

            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(404);
            }
        }

        [HttpPost]
        [Route("GetRecaptchaResponse")]
        public IActionResult GetRecaptchaResponse([FromBody] string captchaResponseToken)
        {
            try
            {
                var response = _userService.CheckCaptcha(captchaResponseToken);
                return Ok(response);

            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("SendMessageFromContactUs")]
        public IActionResult SendMessageFromContactUs([FromBody] ContactUsDto contactUsDto)
        {
            try
            {
               _userService.SendMessageFromContactUs(contactUsDto.Name, contactUsDto.Email, contactUsDto.Message);
                return Ok();

            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }
        [HttpPost]
        [Route("SendAddNewsRequest/{projectName}")]
        public IActionResult SendAddNewsRequest([FromBody] ContactUsDto contactUsDto, string projectName)
        {
            try
            {
                _userService.SendAddNewsRequest(contactUsDto.Name, contactUsDto.Email, contactUsDto.Message, projectName);
                return Ok();

            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }
        [HttpPost]
        [Route("SendEditProjectRequest/{projectName}")]
        public IActionResult SendEditProjectRequest([FromBody] ContactUsDto contactUsDto, string projectName)
        {
            try
            {
                _userService.SendEditProjectRequest(contactUsDto.Name, contactUsDto.Email, contactUsDto.Message, projectName);
                return Ok();

            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }
        private object RequestToken([FromBody] string login, string password)
        {
            var claims = new[] {
                 new Claim(ClaimTypes.Name, login),
                 new Claim(ClaimTypes.Name, password)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("My_Keydqsdsqdqsdqsdqsdqs"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                audience: "yourdomain.com",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }

        private bool VerifyToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "yourdomain.com",
                ValidAudience = "yourdomain.com",
                IssuerSigningKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes("My_Keydqsdsqdqsdqsdqsdqs"))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken = null;
            try
            {
                tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
            }
            catch (SecurityTokenException)
            {
                return false;
            }
            catch (Exception ex)
            {
                //something else happened
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                throw;
            }
            //... manual validations return false if anything untoward is discovered
            return validatedToken != null;
        }

        private string GetTokenFromHeaders()
        {
            return HttpContext.Request.Headers.GetCommaSeparatedValues("Authorization").FirstOrDefault().Split(" ")[1];
        }


        [Authorize]
        [HttpGet]
        [Route("GetUserNotifications/{userId}")]
        public IActionResult GetUserNotifications(int userId)
        {
            try
            {
                var notifications = _userService.GetUserNotifications(userId);
                var notificationsCount = _userService.GetUserNotificationsCount(userId);
                return Ok(new { notifications, notificationsCount }) ;

            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }


        [Authorize]
        [HttpGet]
        [Route("UpdateNotificationStatus/{notifId}/{userId}")]
        public IActionResult UpdateNotificationStatus(int notifId, int userId)
        {
            try
            {
                var status = _userService.updateNotificationReadStatus(notifId);
                var notificationsCount = _userService.GetUserNotificationsCount(userId);
                return Ok(new { status, notificationsCount });

            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }



        [HttpGet]
        [Route("updateKycNotification/{id}")]
        public IActionResult updateKycNotification(int id)
        {
            try
            {
                _userService.updateKycNotification(id);         
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(404);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("addCoinsToUser")]
        public  IActionResult addCoinsToUser([FromBody] CreditCurrencyDto creditCurrencyDto)
        {
            User user;
            try
            {
                user =  _userService.addCoinsToUser(creditCurrencyDto.UserId, creditCurrencyDto.CurrencyQuantity);
          
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
            return Ok(user);
        }


    }
}