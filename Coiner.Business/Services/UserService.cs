using Coiner.Business.Models;
using Coiner.Business.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Coiner.Business.Heplers;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using System.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Coiner.Business.Models.Enums;
using System.Threading.Tasks;

namespace Coiner.Business.Services
{
    public class UserService
    {
        private CoinerContext _context;
        private EmailService _emailService;
        private MangoPayService _mangoPayService;
        private NotificationsService _notificationsService = new NotificationsService();
        private IConfiguration config;

        public UserService()
        {
            _context = new CoinerContext();
            _emailService = new EmailService();
            _mangoPayService = new MangoPayService();
        }

        public UserService(CoinerContext contex, EmailService emailService)
        {
            _context = contex;
            _emailService = emailService;
        }

        public async Task<bool> CreateUser(User user)
        {
            user.ActivationToken = Guid.NewGuid().ToString();  //generate a unique random token to verify user        
            CheckParameters(user);
            if (CheckExistingLogin(user.Login))
            {
                return false;
            }
            user.UserCoinsNumber = Constants.DefaultUserCoinsNmber;
            var userWallet = new UserWallet()
            {
                UnusedCoinsNumber = Constants.DefaultUserCoinsNmber
            };

            user.UserWallet = userWallet;
            AddImage(user);

            if (user.UserType == UserTypeEnum.Particulier)
            {
                var walletId = await _mangoPayService.CreateUser(user);
                user.WalletId = walletId;
            }

            _context.Users.Add(user);
            _context.SaveChanges();
            _emailService.SendUserCreationEmail(user);

            return true;
        }

        public async Task<bool> CreateUserWithGoogle(User user)
        {
            if (CheckExistingLogin(user.Email))
            {
                return false;
            }
            user.UserCoinsNumber = Constants.DefaultUserCoinsNmber;
            var userWallet = new UserWallet()
            {
                UnusedCoinsNumber = Constants.DefaultUserCoinsNmber
            };

            user.UserWallet = userWallet;
            AddImage(user);
            user.IsActive = AccountVerificationEnum.Verified;

            if (user.UserType == UserTypeEnum.Particulier)
            {
                var walletId = await _mangoPayService.CreateUser(user);
                user.WalletId = walletId;
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            _emailService.SendUserCreationEmail(user);

            return true;
        }

        public object GetUser(int userId)
        {
            var adminLogin = GetLoginFromConfig();

            var currentUser = _context.Users.Where(u => u.Id == userId)
                // .Include(u => u.Address)
                .Include(u => u.UserImage)
                .FirstOrDefault();
            return new
            {
                currentUser,
                adminLogin,
            };

        }

        private void AddImage(User user)
        {
            if (user.UserImage != null)
            {
                if (user.Provider == ProvidersEnum.Google)
                    return;
                var imageGuid = Guid.NewGuid();
                var imageName = $"{imageGuid}.Jpeg";

                user.UserImage.Path = Path.GetFileName(imageName);
                SaveImage(user.UserImage.Content, imageName);
            }
        }

        private void SaveImage(string ImgStr, string ImgName)
        {
            var path = HostingEnvironment.WebRootPath + Constants.UserImagesSharedPath;
            //Check if directory exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path); //Create directory if it doesn't exist
            }

            //set the image path
            var imgPath = Path.Combine(path, ImgName);

            byte[] imageBytes = Convert.FromBase64String(ImgStr);

            using (MagickImage image = new MagickImage(imageBytes))
            {
                image.Quality = 60;
                image.Write(imgPath);
            }
        }

        public string GetLoginFromConfig()
        {
            config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", true, true)
                                  .Build();
            var adminConfig = config.GetSection("AdminConfig");
            return Convert.ToString(adminConfig["AdminLogin"]);
        }

        public User loginUser(string login, string pw)
        {
            var myuser = _context.Users
                         .Include(p => p.UserImage)
                         //   .Include(a => a.Address)
                         .FirstOrDefault(u => u.Login == login && u.Password == pw);
            return myuser;
        }

        public User LoginUserWithGogle(string email)
        {
            var myuser = _context.Users
                         .Include(p => p.UserImage)
                         //       .Include(a => a.Address)
                         .FirstOrDefault(u => u.Login == email);
            return myuser;
        }

        public Boolean IsUserActive(int id)
        {
            var myuser = _context.Users
                         .FirstOrDefault(u => u.Id == id);
            if (myuser.IsActive == AccountVerificationEnum.Verified)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public User UpdateUser(User user)
        {
            CheckParameters(user);
            if (CheckExistingLogin(user.Login, user.Id))
            {
                return null;
            }
            if (user.Provider == ProvidersEnum.Coiner)
            {
                var oldUserImage = _context.UserImage.Where(i => i.UserId == user.Id).FirstOrDefault();
                UpdateUserImage(user, oldUserImage);
            }
            _context.Users.Update(user);
            _context.SaveChanges();
            return user;
        }

        public bool SendEmailToUpdatePassword(HttpContext httpContext, string email)
        {
            var decodedEmail = ConvertString.Base64Decode(email);
            var currentUrl = httpContext.Request.Scheme + "://" + httpContext.Request.Host.Value;
            var client = GetUser(decodedEmail);
            if (client != null)
            {
                var userId = ConvertString.MD5Encode(client.Id.ToString());

                var redirectUrl = $"{currentUrl}/modifier-password/{userId}";

                var content = EmailTemplate.EmailContent(DemandUpdatePWEmailTemplate.Path)
                                           .Replace(EmailTemplate.UserName, client.FirstName)
                                           .Replace(EmailTemplate.ResetLink, redirectUrl);

                _emailService.SendEmail(decodedEmail, DemandUpdatePWEmailTemplate.Subject, content);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateUserPassword(string id, string newPassword)
        {
            if (CheckUserNewPassword(id, newPassword))
            {
                int mod4 = id.Length % 4;
                if (mod4 > 0)
                {
                    id += new string('=', 4 - mod4);
                }
                var userId = Convert.ToInt32(ConvertString.MD5Decode(id));
                var currentClient = _context.Users.Find(userId);
                if (currentClient.Id == userId)
                {
                    currentClient.Password = newPassword;
                    _context.SaveChanges();
                    _notificationsService.SendPasswordChangeNotification(currentClient);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool VerifyAccount(int id, string token)
        {

            User user = _context.Users.Where(u => u.Id == id)
                        .FirstOrDefault();
            if (user.ActivationToken == token)
            {
                user.IsActive = AccountVerificationEnum.Verified;
                _context.Users.Update(user);
                _context.SaveChanges();
                _notificationsService.SendWelcomeNotification(user);
                return true;
            }
            else
            {
                return false;
            }
        }

        // Check variables of user form

        private static void CheckParameters(User user)
        {
            if (!CheckEmail(user.Email) ||
                //!CheckPhoneNumber(user.PhoneNumber) ||
                //!CheckAdress(user.Address.Address1) ||
                !CheckCredentials(user.Login, user.Password, user.Provider) ||
                !CheckFields(user.FirstName)
            //|| !CheckFields(user.LastName),
            //!CheckFields(user.Address.ZipCode)
            )
            {
                throw new ArgumentException("not all fields are valid");
            }
        }

        private static bool CheckEmail(string email)
        {
            return (!string.IsNullOrWhiteSpace(email) &&
                    Regex.IsMatch(email, Constants.UserEmailRegex) &&
                    email.Length <= Constants.EmailMaxLength);
        }

        private static bool CheckPhoneNumber(string phonenumber)
        {
            return (!string.IsNullOrWhiteSpace(phonenumber) &&
                    Regex.IsMatch(phonenumber, Constants.PhoneNumberRegEx) &&
                    phonenumber.Length <= Constants.PhoneNumberMaxLength);
        }

        private static bool CheckAdress(string adress)
        {
            return (!string.IsNullOrWhiteSpace(adress) &&
                    adress.Length <= Constants.AddressMaxLength);
        }

        private static bool CheckFields(string field)
        {
            return (!string.IsNullOrWhiteSpace(field) &&
                    field.Length <= Constants.userNameMaxLength);
        }

        private static bool CheckCredentials(string login, string password, ProvidersEnum provider)
        {
            if (provider == ProvidersEnum.Google)
            {
                return (CheckEmail(login));
            }
            else
            {
                return (CheckEmail(login) &&
                    !string.IsNullOrWhiteSpace(password) &&
                    password.Length >= Constants.userPasswordMinLength &&
                    password.Length <= Constants.userPasswordMaxLength);
            }
        }

        private bool CheckExistingLogin(string login, int id = 0)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentException("not all fields are valid", nameof(login));
            }

            User user = _context.Users.AsNoTracking().FirstOrDefault(c => c.Login == login);

            if (user == null)
            {
                return false;
            }
            else if (user != null && user.Id == id)
            {
                return false;
            }

            return true;
        }

        private static bool CheckUserNewPassword(string clientId, string newPassword)
        {
            return (!string.IsNullOrWhiteSpace(newPassword) &&
                    !string.IsNullOrWhiteSpace(clientId) &&
                    newPassword.Length >= Constants.userPasswordMinLength &&
                    newPassword.Length <= Constants.userPasswordMaxLength);
        }

        public string FetchImageContent(int userId)
        {
            User user = _context.Users.Where(u => u.Id == userId)
                .Include(u => u.UserImage)
                .FirstOrDefault();
            string content;
            if (user != null && user.UserImage != null)
            {
                try
                {
                    var userImagePath = "wwwroot\\" + Constants.UserImagesSharedPath + user.UserImage.Path;
                    if (File.Exists(userImagePath))
                    {
                        content = Convert.ToBase64String(File.ReadAllBytes(userImagePath));
                        return content;

                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return null;
        }

        private void DeleteUserImage(User user, UserImage oldUserImage)
        {
            if (oldUserImage != null)
            {
                var path = HostingEnvironment.WebRootPath + Constants.UserImagesSharedPath + oldUserImage.Path;
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        private void UpdateUserImage(User user, UserImage oldUserImage)
        {
            DeleteUserImage(user, oldUserImage);
            UpdateImage(user, oldUserImage);
        }

        private void UpdateImage(User user, UserImage oldUserImage)
        {
            if (user.UserImage != null)
            {
                var imageGuid = Guid.NewGuid();
                var imageName = $"{imageGuid}.Jpeg";

                user.UserImage.Path = Path.GetFileName(imageName);
                SaveImage(user.UserImage.Content, imageName);
                if (oldUserImage != null)
                {
                    oldUserImage.UpdateDate = DateTime.Now;
                    oldUserImage.Path = imageName;
                }
            }
            else
            {
                if (oldUserImage != null)
                {
                    _context.UserImage.Remove(oldUserImage);
                }
            }
        }

        private User GetUser(string email)
        {
            return _context.Users.Where(c => c.Email == email).FirstOrDefault();
        }

        public bool CheckCaptcha(string captchaResponse)
        {
            using (var client = new WebClient())
            {
                var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                   .AddJsonFile("appsettings.json", true, true)
                                   .Build();

                var privateKey = Convert.ToString(config["GoogleRecaptchaWepApiKey"]);

                var response = client.DownloadString(
                    $"{Constants.GoogleRecaptchaUrl}" +
                    $"{privateKey}" +
                    $"&response={ captchaResponse }");
                return (bool)JObject.Parse(response)["success"];
            }
        }

        public void SendMessageFromContactUs(string name, string email, string message)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", true, true)
                   .Build();

            var adminEmail = Convert.ToString(config["AdminEmail"]);

            var content = EmailTemplate.EmailContent(ContactUsEmail.Path)
                                           .Replace(EmailTemplate.UserName, name)
                                           .Replace(EmailTemplate.UserEmail, email)
                                           .Replace(EmailTemplate.UserMessage, message);

            _emailService.SendEmail(adminEmail, ContactUsEmail.Subject, content);
        }

        public void SendEditProjectRequest(string name, string email, string message, string projectName)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", true, true)
                   .Build();

            var adminEmail = Convert.ToString(config["AdminEmail"]);
            //pour le moment on utilise le template de contact us 
            var content = EmailTemplate.EmailContent(ContactUsEmail.Path)
                                           .Replace(EmailTemplate.UserName, name)
                                           .Replace(EmailTemplate.UserEmail, email)
                                           .Replace(EmailTemplate.UserMessage, message);

            _emailService.SendEmail(adminEmail, EditProjectRequestEmailTemplate.Subject + " " + projectName, content);
        }

        public void SendAddNewsRequest(string name, string email, string message, string projectName)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", true, true)
                   .Build();

            var adminEmail = Convert.ToString(config["AdminEmail"]);
            //pour le moment on utilise le template de contact us 
            var content = EmailTemplate.EmailContent(ContactUsEmail.Path)
                                           .Replace(EmailTemplate.UserName, name)
                                           .Replace(EmailTemplate.UserEmail, email)
                                           .Replace(EmailTemplate.UserMessage, message);

            _emailService.SendEmail(adminEmail, AddNewsRequestEmailTemplate.Subject + " " + projectName, content);
        }

        public IEnumerable<NotificationProduced> GetUserNotifications(int userId)
        {
            return _context.NotificationProduced.Where(u => u.UserId == userId && !(u.AppReadStatus == true && u.NotificationTemplateId == Constants.privateKeyConfigurationId))
                   .OrderByDescending(p => p.CreateDate)
                   .ToList();

        }

        public int GetUserNotificationsCount(int userId)
        {
            return _context.NotificationProduced.Where(u => u.UserId == userId && u.AppReadStatus == false)
                   .OrderByDescending(p => p.CreateDate)
                   .Count();
        }

        public bool updateNotificationReadStatus(int notificationId)
        {
            var notif = _context.NotificationProduced.Where(u => u.Id == notificationId)
                   .OrderByDescending(p => p.CreateDate)
                   .FirstOrDefault();
            notif.AppReadStatus = true;
            _context.NotificationProduced.Update(notif);
            _context.SaveChanges();
            return true;
        }

        public void updateKycNotification(int id)
        {
            var myuser = _context.Users
                         .FirstOrDefault(u => u.Id == id);
            myuser.KycNotificationSent = true;
            _context.Users.Update(myuser);
            _context.SaveChanges();
        }

        public User addCoinsToUser(int id, int CoinNumber)
        {
            var myuser = _context.Users
                         .FirstOrDefault(u => u.Id == id);
            myuser.UserCoinsNumber += CoinNumber;
            _context.Users.Update(myuser);
            _context.SaveChanges();
            return(myuser);
        }



    }
}
