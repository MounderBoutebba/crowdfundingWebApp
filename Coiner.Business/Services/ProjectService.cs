using Coiner.Business.Context;
using Coiner.Business.Heplers;
using Coiner.Business.Models;
using Coiner.Business.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageMagick;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Collections;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using SelectPdf;
using Coiner.Business.CustomExceptions;

namespace Coiner.Business.Services
{
    public class ProjectService
    {
        private CoinerContext _context = new CoinerContext();
        private EmailService _emailService = new EmailService();
        private MangoPayService _mangöPayService = new MangoPayService();
        private BlockChainService _blockChainService = new BlockChainService();
        private NotificationsService _notificationsService = new NotificationsService();

        private IConfiguration config;

        public bool CreateProject(Project project)
        {
            CheckParametres(project);
            AddImages(project);
            AddDocuments(project);
            _context.Projects.Add(project);
            _context.SaveChanges();
            SendProjectCreationEmail(project);
            SendProjectCreationEmailToAdmin(project);
            var user = _context.Users.Where(u => u.Id == project.UserId).FirstOrDefault();
            _notificationsService.SendProjectCreartionNotification(user, project);
            return true;
        }

        public bool AddNewsToProject(ProjectUpdate projectUpate)
        {
            _context.ProjectUpdate.Add(projectUpate);
            _context.SaveChanges();
            return true;
        }

        private void SendProjectCreationEmail(Project project)
        {
            var url = Constants.BaseUrl;
            var user = _context.Users.Where(u => u.Id == project.UserId).FirstOrDefault();
            var content = EmailTemplate.EmailContent(ProjectCreationEmailTemplate.Path)
                                           .Replace(EmailTemplate.UserName, user.FirstName)
                                           .Replace(EmailTemplate.BaseUrl, url);

            _emailService.SendEmail(project.User.Email, ProjectCreationEmailTemplate.Subject, content);
        }

        private void SendProjectCreationEmailToAdmin(Project project)
        {
            config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", true, true)
                                  .Build();

            var adminEmail = Convert.ToString(config["AdminEmail"]);

            var projectDetailsTemplate = GenerateProjectDetailsTemplate(project);
            var content = EmailTemplate.EmailContent(ProjectCreationEmailAdminTemplate.Path)
                                           .Replace(EmailTemplate.UserName, adminEmail)
                                           .Replace(EmailTemplate.ProjectDetails, projectDetailsTemplate);

            _emailService.SendEmail(adminEmail, ProjectCreationEmailAdminTemplate.Subject, content);
        }

        private static string GenerateProjectDetailsTemplate(Project project)
        {
            StringBuilder stringBuilder = new StringBuilder();
            switch (project.ProjectType)
            {
                case ProjectTypeEnum.Project:
                    stringBuilder.AppendFormat(
                    $"<div><span> <b>Type : </b></span>{project.ProjectType}</div>" +
                    $"<div><span><b>Dénomination du projet : <b/></span>{ project.ProjectName}</div>" +
                    $"<div><span><b>Presentez - vous et votre équipe : </b></ span >{ project.ProjectPresentation}</div>" +
                    $"<div><span><b>Type d'activité : </b></span>{ project.ActivityType}</div>" +
                    $"<div><span><b>Lieu : </b></ span >{ project.ProjectAddress}</div>" +
                    $"<div><span><b>Business plan : </b></span>{ project.BusinessPlan}</div>" +
                    $"<div><span><b>Lien web : </b> </span>{ project.WebLink}</ div > " +
                    $"<div><span><b>Montant recherché : </b> </ span >{ project.FundingGoal} €</div>" +
                    $"<div><span><b>Date début estimé : </b> </ span >{ project.BeginEstimatedDate}</div>" +
                    $"<div><span><b>Durée de la lévé : </b> </ span >{ project.FundraisingPeriod}mois</div>" +
                    $"<div><span><b>Quel pourcentage de la société souhaitez - vous vendre  : </b> </ span >{ project.PercentageAsset} %</div>" +
                    $"<div><span><b>Description du projet : </b> </ span >{ project.ProjectDescription}</div>"
                    );
                    break;
                case ProjectTypeEnum.Society:
                    stringBuilder.AppendFormat(
                    $"<div><span> <b>Type : </b></span>{project.ProjectType}</div>" +
                    $"<div><span><b>Nom du projet : <b/></span>{ project.ProjectName}</div>" +
                    //$"<div><span><b>Nom du societé : <b/></span>{ project.Society_Name}</div>" +
                    $"<div><span><b>Identification légale : <b/></span>{ project.Society_LegaleIdentification}</div>" +
                    $"<div><span><b>Type de structure : <b/></span>{ project.Society_StructureType}</div>" +
                    $"<div><span><b>Date de création : <b/></span>{ project.Society_CreationDate}</div>" +
                    $"<div><span><b>Type d'activité : </b></span>{ project.ActivityType}</div>" +
                    $"<div><span><b>Addresse : </b></ span >{ project.ProjectAddress}</div>" +
                    $"<div><span><b>Perspective d'évolution : </b></span>{ project.BusinessPlan}</div>" +
                    $"<div><span><b>Lien web : </b> </span>{ project.WebLink}</ div > " +
                    $"<div><span><b>Montant recherché : </b> </ span >{ project.FundingGoal} €</div>" +
                    $"<div><span><b>Date début estimé : </b> </ span >{ project.BeginEstimatedDate}</div>" +
                    $"<div><span><b>Durée de la lévé : </b> </ span >{ project.FundraisingPeriod}mois</div>" +
                    $"<div><span><b>Quel pourcentage de la société souhaitez-vous vendre: </b> </ span >{ project.PercentageAsset} %</div>" +
                    $"<div><span><b>Description du projet : </b> </ span >{ project.ProjectDescription}</div>"
                    );
                    break;
                case ProjectTypeEnum.Career:
                    stringBuilder.AppendFormat(
                    $"<div><span> <b>Type : </b></span>{project.ProjectType}</div>" +
                    $"<div><span><b>Dénomination du profil : <b/></span>{ project.ProjectName}</div>" +
                    $"<div><span><b>Presentez-vous : <b/></span>{ project.ProjectPresentation}</div>" +
                    $"<div><span><b>Type d'activité : </b></span>{ project.ActivityType}</div>" +
                    $"<div><span><b>Lieu : </b></ span >{ project.ProjectAddress}</div>" +
                    $"<div><span><b>Projection des gains suite à votre réalisation : </b></span>{ project.BusinessPlan}</div>" +
                    $"<div><span><b>Lien web : </b> </span>{ project.WebLink}</ div > " +
                    $"<div><span><b>Montant recherché : </b> </ span >{ project.FundingGoal} €</div>" +
                    $"<div><span><b>Date début estimé : </b> </ span >{ project.BeginEstimatedDate}</div>" +
                    $"<div><span><b>Durée de la lévé : </b> </ span >{ project.FundraisingPeriod}mois</div>" +
                    $"<div><span><b>Sur combien d'année vous souhaitez vous engager : </b> </ span >{ project.Career_EngagementYears}mois</div>" +
                    $"<div><span><b>Quel pourcentage du CA ou salaire comptez-vous verser durant votre engagement : </b> </ span >{ project.PercentageAsset} %</div>" +
                    $"<div><span><b>Description de vos objectif : </b> </ span >{ project.ProjectDescription}</div>"
                    );
                    break;
                case ProjectTypeEnum.Product:
                    stringBuilder.AppendFormat(
                    $"<div><span> <b>Type : </b></span>{project.ProjectType}</div>" +
                    $"<div><span><b>Dénomination du bien : <b/></span>{ project.ProjectName}</div>" +
                    $"<div><span><b>Presentez-vous : <b/></span>{ project.ProjectPresentation}</div>" +
                    $"<div><span><b>Type de bien : </b></span>{ project.ActivityType}</div>" +
                    $"<div><span><b>Lieu du bien : </b></ span >{ project.ProjectAddress}</div>" +
                    $"<div><span><b>Percepective de vente : </b></ span >{ project.Product_SalesPercepective}</div>" +
                    $"<div><span><b>Lien web : </b> </span>{ project.WebLink}</ div > " +
                    $"<div><span><b>Montant recherché : </b> </ span >{ project.FundingGoal} €</div>" +
                    $"<div><span><b>Date début estimé : </b> </ span >{ project.BeginEstimatedDate}</div>" +
                    $"<div><span><b>Durée de la lévé : </b> </ span >{ project.FundraisingPeriod}mois</div>" +
                    $"<div><span><b>Quel pourcentage du bien souhaitez-vous vendre : </b> </ span >{ project.PercentageAsset} %</div>" +
                    $"<div><span><b>Description du bien : </b> </ span >{ project.ProjectDescription}</div>"
                    );
                    break;
                default:
                    break;
            }
            return stringBuilder.ToString();
        }

        private void SendMiseAcceptedEmail(User user, Project project)
        {
            var url = Constants.BaseUrl;
            //var user = _context.Users.Where(u => u.Id == project.UserId).FirstOrDefault();
            var content = EmailTemplate.EmailContent(AcceptedMiseEmailTemplate.Path)
                                           .Replace(EmailTemplate.UserName, user.FirstName)
                                           .Replace(EmailTemplate.BaseUrl, url);

            _emailService.SendEmail(user.Email, AcceptedMiseEmailTemplate.Subject, content);
            _notificationsService.SendBakingConfirmationNotification(user, project);
        }

        public IEnumerable<Project> GetProjects()
        {
            return _context.Projects.Where(p => p.ProjectStatus == ProjectStatusEnum.Accepted ||
                                                p.ProjectStatus == ProjectStatusEnum.EndSurvey)
                .Include(p => p.User)
                .Include(p => p.ProjectImages)
                .Include(p => p.Documents)
                .Include(p => p.Coins)
                .Include(p => p.ProjectUpdates)
                .Include(p => p.Discussions).ThenInclude(d => d.User).ThenInclude(d => d.UserImage)
                .OrderByDescending(p => p.CreationDate)
                .ToList();
        }

        public IEnumerable<Project> GetLatestProjects()
        {
            return _context.Projects.OrderByDescending(p => p.CreationDate)
                              .Where(p => p.ProjectStatus == ProjectStatusEnum.Accepted ||
                                          p.ProjectStatus == ProjectStatusEnum.EndSurvey)
                              .Take(9)
                              .Include(p => p.User)
                              .Include(p => p.User.UserImage)
                              .Include(p => p.ProjectImages)
                              .Include(p => p.Coins)
                              .Include(p => p.Documents)
                              .Include(p => p.ProjectUpdates)
                              .Include(p => p.Discussions).ThenInclude(d => d.User).ThenInclude(d => d.UserImage)
                              .ToList();
        }

        public int GetAllProjectsCount()
        {
            return _context.Projects
                 .Where(p => p.ProjectStatus == ProjectStatusEnum.Accepted ||
                             p.ProjectStatus == ProjectStatusEnum.EndSurvey)
                 .Count();
        }

        public IEnumerable<Project> GetUserProjects(int userId, int pageIndex, int pageSize)
        {
            config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", true, true)
                                  .Build();
            var AdminConfig = config.GetSection("AdminConfig");

            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user.Login == Convert.ToString(AdminConfig["AdminLogin"]) && user.Password == Convert.ToString(AdminConfig["AdminPassword"]))
            {
                return _context.Projects
                .Include(p => p.User)
                .Include(p => p.User.UserImage)
                .Include(p => p.ProjectImages)
                .Include(p => p.Coins)
                .Include(p => p.Documents)
                .Include(p => p.ProjectUpdates)
                .Include(p => p.Discussions).ThenInclude(d => d.User).ThenInclude(d => d.UserImage)
                .OrderByDescending(p => p.CreationDate)
                .Skip(pageIndex)
                .Take(pageSize).ToList()
                .ToList();
            }
            else
            {
                return _context.Projects.Where(p => p.UserId == userId)
                .Include(p => p.User)
                .Include(p => p.User.UserImage)
                .Include(p => p.ProjectImages)
                .Include(p => p.Documents)
                .Include(p => p.Coins)
                .Include(p => p.ProjectUpdates)
                .Include(p => p.Discussions).ThenInclude(d => d.User).ThenInclude(d => d.UserImage)
                .OrderByDescending(p => p.CreationDate)
                .Skip(pageIndex)
                .Take(pageSize).ToList()
                .ToList();
            }
        }

        private void AddImages(Project project)
        {
            if (project.ProjectImages != null)
            {
                foreach (var image in project.ProjectImages)
                {
                    var imageGuid = Guid.NewGuid();
                    var imageName = $"{imageGuid}.Jpeg";

                    image.Path = Path.GetFileName(imageName);
                    SaveImage(image.Content, imageName);
                }
            }
        }

        private void SaveImage(string ImgStr, string ImgName)
        {
            var path = HostingEnvironment.WebRootPath + Constants.ProjectImagesSharedPath;
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

        private void AddDocuments(Project project)
        {
            if (project.Documents != null)
            {
                foreach (var document in project.Documents)
                {
                    var documentGuid = Guid.NewGuid();
                    var documentName = $"{documentGuid}.{document.Extention}";

                    document.Path = Path.GetFileName(documentName);
                    SaveDocument(document.Content, documentName);
                }
            }
        }

        private void SaveDocument(string documentStr, string documentName)
        {
            var path = HostingEnvironment.WebRootPath + Constants.ProjectDocumentsSharedPath;
            //Check if directory exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path); //Create directory if it doesn't exist
            }

            //set the image path
            var documentPath = Path.Combine(path, documentName);

            byte[] documentBytes = Convert.FromBase64String(documentStr);

            File.WriteAllBytes(documentPath, documentBytes);
        }

        public IEnumerable<Project> GetFilteredProjects(List<string> projectActivityTypes,
                                                        List<ProjectTypeEnum> projectTypes,
                                                        int pageIndex,
                                                        int pageSize)
        {
            return _context.Projects
                .Where(p => p.ProjectStatus == ProjectStatusEnum.Accepted ||
                            p.ProjectStatus == ProjectStatusEnum.EndSurvey)
                .Where(p => projectActivityTypes.Contains(p.ActivityType))
                .Where(p => projectTypes.Contains(p.ProjectType))
                .Include(p => p.User)
                .Include(p => p.User.UserImage)
                .Include(p => p.ProjectImages)
                .Include(p => p.Coins)
                .Include(p => p.Documents)
                .Include(p => p.ProjectUpdates)
                .Include(p => p.Discussions).ThenInclude(d => d.User).ThenInclude(d => d.UserImage)
                .OrderByDescending(p => p.CreationDate)
                .Skip(pageIndex)
                .Take(pageSize).ToList()
                .ToList();
        }

        public int GetFilteredProjectsCount(List<string> projectActivityTypes,
                                                List<ProjectTypeEnum> projectTypes)
        {
            return _context.Projects
                .Where(p => p.ProjectStatus == ProjectStatusEnum.Accepted ||
                            p.ProjectStatus == ProjectStatusEnum.EndSurvey)
                .Where(p => projectActivityTypes.Contains(p.ActivityType))
                .Where(p => projectTypes.Contains(p.ProjectType)).Count();
        }

        public int getUserProjectsCount(int userId)
        {
            config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", true, true)
                                  .Build();
            var AdminConfig = config.GetSection("AdminConfig");

            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user.Login == Convert.ToString(AdminConfig["AdminLogin"]) && user.Password == Convert.ToString(AdminConfig["AdminPassword"]))
            {
                return _context.Projects.Count();
            }
            else
            {
                return _context.Projects
               .Where(p => p.UserId == userId)
               .Count();
            }
        }

        public IEnumerable<Project> GetFavoriteProjects(List<int> ids)
        {
            return _context.Projects.Where(p => p.ProjectStatus == ProjectStatusEnum.Accepted ||
                                                p.ProjectStatus == ProjectStatusEnum.EndSurvey)
                                    .Where(p => ids.Contains(p.Id))
                                    .Include(p => p.User)
                                    .Include(p => p.User.UserImage)
                                    .Include(p => p.ProjectImages)
                                    .Include(p => p.Coins)
                                    .Include(p => p.Documents)
                                    .Include(p => p.ProjectUpdates)
                                    .Include(p => p.Discussions).ThenInclude(d => d.User).ThenInclude(d => d.UserImage)
                                    .ToList();
        }

        private static bool CheckMaxLength(string field, int maxLength)
        {
            return (!string.IsNullOrWhiteSpace(field) &&
                    field.Length <= maxLength);
        }

        private static bool CheckDate(string field)
        {
            try
            {
                DateTime dt = DateTime.Parse(field);
                return true;
            }
            catch
            {
                return false;
            }

        }

        private static bool CheckWebLink(string weblink, int maxlength)
        {
            if (weblink == null)
            {
                return true;
            }
            else
            {
                return (weblink.Length <= maxlength);
            }
        }

        private static void CheckParametres(Project project)
        {


            switch (project.ProjectType)
            {
                case ProjectTypeEnum.Project:
                    if (!CheckMaxLength(project.ProjectPresentation, Constants.PresentationTeam) ||
                        !CheckMaxLength(project.BusinessPlan, Constants.ProjectAdress))
                    {
                        throw new ArgumentException("not all fields are valid");
                    }
                    break;
                case ProjectTypeEnum.Society:
                    if (//!CheckMaxLength(project.Society_Name, Constants.ProjectName) ||
                        //!CheckMaxLength(project.Society_LegaleIdentification, Constants.ProjectName) ||
                        !CheckWebLink(project.Society_StructureType, Constants.ProjectName)
                        //!CheckDate(project.Society_CreationDate.ToString())
                        )
                    {
                        throw new ArgumentException("not all fields are valid");
                    }
                    break;
                case ProjectTypeEnum.Career:
                    if (!CheckMaxLength(project.Career_EngagementYears.ToString(), Constants.ProjectAdress) ||
                        !CheckWebLink(project.BusinessPlan, Constants.ProjectAdress))
                    {
                        throw new ArgumentException("not all fields are valid");
                    }
                    break;
                case ProjectTypeEnum.Product:
                    //if (!CheckMaxLength(project.Product_SalesPercepective.ToString(), Constants.PercentageMaxLength))
                    //{
                    //    throw new ArgumentException("not all fields are valid");
                    //}
                    break;
                default:
                    break;
            }
            if (!CheckMaxLength(project.ProjectName, Constants.ProjectName) ||
               //!CheckMaxLength(project.ProjectAddress, Constants.ProjectAdress) ||
               !CheckWebLink(project.WebLink, Constants.EmailMaxLength) ||
               !CheckMaxLength(project.FundingGoal.ToString(), Constants.FundingGoal) ||
               !CheckMaxLength(project.PercentageAsset.ToString(), Constants.PercentageMaxLength)
               //!CheckMaxLength(project.ProjectDescription, Constants.ProjectDescreption)
               //!CheckDate(project.BeginEstimatedDate.ToString())
               )
            {
                throw new ArgumentException("not all fields are valid");
            }
        }

        public bool ChekCoinsAdded(Coin currentCoin)
        {
            var coins = _context.Coins
                .Where(c => c.ProjectId == currentCoin.ProjectId && c.UserId == currentCoin.UserId)
                .Where(c => c.CoinValue == currentCoin.CoinValue).FirstOrDefault();
            var coinsDiff = (currentCoin.CoinsNumber - coins.CoinsNumber);
            return coinsDiff == 0 ? false : true;
        }

        public object[] AddCoinsToProject(Coin currentCoin)
        {
            var currentUser = _context.Users.Where(u => u.Id == currentCoin.UserId).Include(u => u.UserWallet)
                .FirstOrDefault();
            if (currentUser == null)
            {
                throw new UserNotFoundException(currentCoin.UserId);
            }
            var currentProject = _context.Projects.Where(p => p.Id == currentCoin.ProjectId)
                .Include(p => p.User)
                .Include(p => p.ProjectImages)
                .Include(p => p.Documents)
                .Include(p => p.Coins)
                .Include(p => p.ProjectUpdates)
                .Include(p => p.Discussions).ThenInclude(d => d.User).ThenInclude(d => d.UserImage)
                .FirstOrDefault();
            var coins = _context.Coins
                .Where(c => c.ProjectId == currentCoin.ProjectId && c.UserId == currentCoin.UserId)
                .Where(c => c.CoinValue == currentCoin.CoinValue).FirstOrDefault();
            if (coins == null)
            {
                _context.Coins.Add(currentCoin);
                currentUser.UserCoinsNumber -= currentCoin.CoinsNumber;
                currentProject.ReceivedFunding += currentCoin.CoinsNumber * currentCoin.CoinValue;

                currentUser.UserWallet.UnusedCoinsNumber -= currentCoin.CoinsNumber;
                currentUser.UserWallet.FirstUsedCoinsNumber += currentCoin.CoinsNumber;

            }
            else
            {
                var coinsDiff = (currentCoin.CoinsNumber - coins.CoinsNumber);

                coins.CoinsNumber = coins.CoinsNumber + coinsDiff;
                currentUser.UserCoinsNumber -= coinsDiff;
                currentProject.ReceivedFunding += coinsDiff * currentCoin.CoinValue;

                currentUser.UserWallet.UnusedCoinsNumber -= coinsDiff;
                currentUser.UserWallet.FirstUsedCoinsNumber += coinsDiff;

                //// check if it is decrease or increase 
                //if (coinsDiff < 0)
                //{
                //    // decrease
                //    var decreasedCoins = -coinsDiff;
                //    if (currentUser.UserWallet.FirstUsedCoinsNumber - decreasedCoins <= 0)
                //    {
                //        currentUser.UserWallet.FirstUsedCoinsNumber -= decreasedCoins;
                //    }
                //    else
                //    {

                //    }
                //    currentUser.UserWallet.FirstUsedCoinsNumber -= decreasedCoins;
                //    currentUser.UserWallet.RemovedCoinsNumber += decreasedCoins;
                //}
                //else
                //{
                //    //increase
                //    var increasedCoins = coinsDiff;

                //    // first time using the coins so we use the unused coins first 
                //    if (currentUser.UserWallet.RemovedCoinsNumber == 0)
                //    {
                //        currentUser.UserWallet.UnusedCoinsNumber -= increasedCoins;
                //        currentUser.UserWallet.FirstUsedCoinsNumber += increasedCoins;
                //    }
                //    // user have removed coins we have to use the removed coins first
                //    else
                //    {
                //        // the user have only used some coins once 
                //        if (currentUser.UserWallet.FirstUsedCoinsNumber != 0 &&
                //            currentUser.UserWallet.ThirdUsedCoinsNumber == 0)
                //        {
                //            //user used second time coins as the same removed coins
                //            if (increasedCoins == currentUser.UserWallet.RemovedCoinsNumber)
                //            {
                //                currentUser.UserWallet.SecondUsedCoinsNumber += increasedCoins;
                //                currentUser.UserWallet.RemovedCoinsNumber -= increasedCoins;
                //            }
                //            //user used second time coins less than removed coins
                //            else if (increasedCoins < currentUser.UserWallet.RemovedCoinsNumber)
                //            {
                //                currentUser.UserWallet.SecondUsedCoinsNumber += increasedCoins;
                //                currentUser.UserWallet.RemovedCoinsNumber -= increasedCoins;
                //            }
                //            //user used second time coins more coins than removed coins
                //            else
                //            {
                //                var decreasedDiff = increasedCoins - currentUser.UserWallet.RemovedCoinsNumber;
                //                if (decreasedDiff == currentUser.UserWallet.FirstUsedCoinsNumber)
                //                {
                //                    currentUser.UserWallet.SecondUsedCoinsNumber = currentUser.UserWallet.RemovedCoinsNumber + decreasedDiff;
                //                    currentUser.UserWallet.RemovedCoinsNumber = 0;
                //                    currentUser.UserWallet.FirstUsedCoinsNumber = 0;
                //                }
                //                else if (decreasedDiff < currentUser.UserWallet.FirstUsedCoinsNumber)
                //                {
                //                    currentUser.UserWallet.FirstUsedCoinsNumber -= decreasedDiff;
                //                    currentUser.UserWallet.SecondUsedCoinsNumber = currentUser.UserWallet.RemovedCoinsNumber + decreasedDiff;
                //                    currentUser.UserWallet.RemovedCoinsNumber = 0;
                //                }
                //                else
                //                {
                //                    currentUser.UserWallet.SecondUsedCoinsNumber = currentUser.UserWallet.RemovedCoinsNumber + currentUser.UserWallet.FirstUsedCoinsNumber;
                //                    var neededCoinsFromUnused = increasedCoins - currentUser.UserWallet.SecondUsedCoinsNumber;
                //                    currentUser.UserWallet.UnusedCoinsNumber -= neededCoinsFromUnused;
                //                    currentUser.UserWallet.RemovedCoinsNumber = 0;
                //                    currentUser.UserWallet.FirstUsedCoinsNumber = neededCoinsFromUnused;
                //                }
                //            }
                //        }
                //        else if (currentUser.UserWallet.FirstUsedCoinsNumber != 0 &&
                //                currentUser.UserWallet.SecondUsedCoinsNumber != 0 &&
                //                currentUser.UserWallet.ThirdUsedCoinsNumber == 0)
                //        {
                //            // the user have used some coins twice 
                //        }

                //    }
                //}
            }

            // test block to set user as a kyc user
            if (currentUser.UserWallet.FirstUsedCoinsNumber > 50)
            {
                currentUser.Kyc = true;
                _context.Users.Update(currentUser);
                _context.SaveChanges();
            }
            //--

            //if (currentProject.ReceivedFunding >= currentProject.FundingGoal)
            //{
            //    currentProject.ProjectStatus = ProjectStatusEnum.EndSurvey;
            //}

            _context.SaveChanges();
            SendMiseAcceptedEmail(currentUser, currentProject);

            //    await CheckFundingSurveyEnd(currentProject);

            object[] data = new object[]
            {
                currentUser.UserCoinsNumber,
                currentProject,
                currentUser.UserWallet
            };
            return data;
        }

        public IEnumerable<Project> SearchForProjects(string searchInput)
        {
            return _context.Projects
                .Where(p => p.ProjectStatus == ProjectStatusEnum.Accepted ||
                            p.ProjectStatus == ProjectStatusEnum.EndSurvey)
                .Where(p => p.ProjectName.ToUpper().Contains(searchInput.ToUpper()) ||
                                     p.User.FirstName.ToUpper().Contains(searchInput.ToUpper()) ||
                                     p.User.LastName.ToUpper().Contains(searchInput.ToUpper()))
                .Include(p => p.User)
                .Include(p => p.User.UserImage)
                .Include(p => p.ProjectImages)
                .Include(p => p.Coins)
                .Include(p => p.Documents)
                .Include(p => p.ProjectUpdates)
                .Include(p => p.Discussions).ThenInclude(d => d.User).ThenInclude(d => d.UserImage)
                .OrderByDescending(p => p.CreationDate)
                .ToList();
        }

        public List<ProjectImage> FetchImagesContent(int projectId)
        {
            var projectImages = new List<ProjectImage>();
            if (projectId != 0)
            {
                projectImages = _context.ProjectImages.Where(i => i.ProjectId == projectId).ToList();
                foreach (var image in projectImages)
                {
                    try
                    {
                        var imageFilePath = "wwwroot\\" + Constants.ProjectImagesSharedPath + Path.GetFileName(image.Path);
                        if (File.Exists(imageFilePath))
                        {
                            image.Content = Convert.ToBase64String(File.ReadAllBytes(imageFilePath));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            return projectImages;
        }

        public List<Document> FetchDocumentsContent(int projectId)
        {
            var projectDocuemts = new List<Document>();
            if (projectId != 0)
            {
                projectDocuemts = _context.Documents.Where(i => i.ProjectId == projectId).ToList();
                foreach (var document in projectDocuemts)
                {
                    try
                    {
                        var documentFilePath = "wwwroot\\" + Constants.ProjectDocumentsSharedPath + Path.GetFileName(document.Path);
                        if (File.Exists(documentFilePath))
                        {
                            document.Content = Convert.ToBase64String(File.ReadAllBytes(documentFilePath));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            return projectDocuemts;
        }

        public bool UpdateProject(Project project)
        {
            CheckParametres(project);
            var projectInDb = _context.Projects.Where(o => o.Id == project.Id).FirstOrDefault();
            if (projectInDb == null)
            {
                return false;
            }
            project.ProjectStatus = ProjectStatusEnum.Initialized;
            project.UpdateDate = DateTime.Now;
            UpdateProjectImages(project);
            UpdateProjectDocuments(project);
            _context.Users.Update(project.User);
            _context.Entry(projectInDb).CurrentValues.SetValues(project);
            _context.SaveChanges();
            return true;
        }

        public int GetUserCoinsForProject(int userId, int projectId)
        {
            var coin = _context.Coins.Where(c => c.UserId == userId && c.ProjectId == projectId).FirstOrDefault();
            if (coin != null)
            {
                return coin.CoinsNumber;
            }
            else
            {
                return 0;
            }
        }

        public List<Discussion> AddQuestionToProject(Discussion discussion)
        {
            if (CheckMaxLength(discussion.QuestionContent, Constants.ProjectDescreption))
            {
                discussion.QuestionCreation = DateTime.Now;
                _context.Discussions.Add(discussion);
                _context.SaveChanges();
                SendAddQuestionToProjectEmail(discussion);
                var discussions = _context.Discussions
                                  .Where(d => d.ProjectId == discussion.ProjectId)
                                  .Include(d => d.User).ThenInclude(d => d.UserImage)
                                  .ToList();
                return discussions;
            }
            else
            {
                return null;
            }
        }

        public List<Discussion> AddAnswerToQuestion(Discussion discussion)
        {
            if (CheckMaxLength(discussion.AnswerContent, Constants.ProjectDescreption))
            {
                var currentDiscussion = _context.Discussions.Where(d => d.Id == discussion.Id).FirstOrDefault();
                if (currentDiscussion != null)
                {
                    currentDiscussion.AnswerContent = discussion.AnswerContent;
                    currentDiscussion.AnswerCreation = DateTime.Now;
                    _context.SaveChanges();

                    SendAddAnswerToProjectEmail(discussion);

                    var discussions = _context.Discussions
                                      .Where(d => d.ProjectId == discussion.ProjectId)
                                      .Include(d => d.User).ThenInclude(d => d.UserImage)
                                      .ToList();
                    return discussions;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private void UpdateProjectImages(Project project)
        {
            DeleteProjectOldImages(project);
            AddImages(project);

            if (project.ProjectImages != null)
            {
                foreach (var image in project.ProjectImages)
                {
                    image.ProjectId = project.Id;
                    _context.ProjectImages.Add(image);
                }
            }
        }

        private void UpdateProjectDocuments(Project project)
        {
            DeleteProjectOldDocuments(project);
            AddDocuments(project);

            if (project.Documents != null)
            {
                foreach (var document in project.Documents)
                {
                    document.ProjectId = project.Id;
                    _context.Documents.Add(document);
                }
            }
        }

        private void DeleteProjectOldImages(Project project)
        {
            var imagesInDb = _context.ProjectImages.Where(i => i.ProjectId == project.Id).ToList();
            foreach (var image in imagesInDb)
            {
                var imageFilePath = "wwwroot\\" + Constants.ProjectImagesSharedPath + Path.GetFileName(image.Path);
                File.Delete(imageFilePath);
                _context.ProjectImages.Remove(image);
            }
        }

        private void DeleteProjectOldDocuments(Project project)
        {
            var documentsInDb = _context.Documents.Where(i => i.ProjectId == project.Id).ToList();
            foreach (var document in documentsInDb)
            {
                var documentFilePath = "wwwroot\\" + Constants.ProjectDocumentsSharedPath + Path.GetFileName(document.Path);
                File.Delete(documentFilePath);
                _context.Documents.Remove(document);
            }
        }

        private void SendAddQuestionToProjectEmail(Discussion discussion)
        {
            var project = _context.Projects.Where(p => p.Id == discussion.ProjectId).Include(p => p.User).FirstOrDefault();
            var user = project.User;

            var content = EmailTemplate.EmailContent(AddQuestionToProjectEmailTemplate.Path)
                                           .Replace(EmailTemplate.UserName, user.FirstName)
                                           .Replace(EmailTemplate.ProjectName, project.ProjectName);


            _emailService.SendEmail(user.Email, AddQuestionToProjectEmailTemplate.Subject, content);
        }

        private void SendAddAnswerToProjectEmail(Discussion discussion)
        {
            var project = _context.Projects.Where(p => p.Id == discussion.ProjectId).Include(p => p.User).FirstOrDefault();
            var user = _context.Users.Where(u => u.Id == discussion.UserId).FirstOrDefault();

            var content = EmailTemplate.EmailContent(AddAnswerToProjectEmailTemplate.Path)
                                           .Replace(EmailTemplate.UserName, user.FirstName)
                                           .Replace(EmailTemplate.ProjectName, project.ProjectName);


            _emailService.SendEmail(user.Email, AddAnswerToProjectEmailTemplate.Subject, content);
        }

        private async Task CheckFundingSurveyEnd(Project project)
        {
            if (project.ReceivedFunding >= project.FundingGoal)
            {
                project.ProjectStatus = ProjectStatusEnum.EndSurvey;
                _context.SaveChanges();
                SendFundingEndCoiner(project);
                SendFundingEndBackers(project);
                CreateBillsForBakers(project);
                await _blockChainService.CreateUsers(project);
                _blockChainService.CreateNewAssetForProject(project);
            }
        }

        public async Task<Project> EndSurvey(int projectId)
        {
            var currentProject = _context.Projects.Where(p => p.Id == projectId)
                .Include(p => p.User)
                .Include(p => p.ProjectImages)
                .Include(p => p.Documents)
                .Include(p => p.Coins)
                .Include(p => p.ProjectUpdates)
                .Include(p => p.Discussions).ThenInclude(d => d.User).ThenInclude(d => d.UserImage)
                .FirstOrDefault();
            await CheckFundingSurveyEnd(currentProject);

            return currentProject;
        }

        private void SendFundingEndCoiner(Project project)
        {
            var url = Constants.BaseUrl;
            var user = _context.Users.Where(u => u.Id == project.UserId).FirstOrDefault();
            var content = EmailTemplate.EmailContent(FundingEndCoinerEmailTemplate.Path)
                                           .Replace(EmailTemplate.UserName, user.FirstName)
                                           .Replace(EmailTemplate.BaseUrl, url);

            _emailService.SendEmail(project.User.Email, FundingEndCoinerEmailTemplate.Subject, content);
            _notificationsService.SendFundingEndNotification(user, project, Constants.projectFundingEndCoinerConfigurationId);
        }

        private void SendFundingEndBackers(Project project)
        {
            var ProjectId = project.Id;
            var url = Constants.BaseUrl;
            var ListOfBakers = _context.Coins.Where(c => c.ProjectId == ProjectId).ToList();
            var ProjectName = _context.Projects.Where(o => o.Id == ProjectId).FirstOrDefault().ProjectName;

            foreach (var IdBaker in ListOfBakers)
            {
                var user = _context.Users.Where(u => u.Id == IdBaker.UserId).FirstOrDefault();
                var content = EmailTemplate.EmailContent(FundingEndBackerEmailTemplate.Path)
                                           .Replace(EmailTemplate.UserName, user.FirstName)
                                           .Replace(EmailTemplate.ProjectName, ProjectName)
                                           .Replace(EmailTemplate.BaseUrl, url)
                                           .Replace(EmailTemplate.ProjectId, ProjectId.ToString());

                _emailService.SendEmail(user.Email, FundingEndBackerEmailTemplate.Subject, content);
                _notificationsService.SendFundingEndNotification(user, project, Constants.projectFundingEndBakerConfigurationId);
            }
        }

        public List<Product> GetDummyProduts(int pageIndex = 0, int pageSize = 0)
        {
            Random random = new Random();
            List<Product> dummyProducts = new List<Product>();
            List<Project> Projects;
            if (pageIndex == 0)
            {
                Projects = _context.Projects
               .Where(p => p.ProjectStatus == ProjectStatusEnum.Accepted ||
                           p.ProjectStatus == ProjectStatusEnum.EndSurvey)
               .Where(p => p.ReceivedFunding >= p.FundingGoal)
               .Include(p => p.User)
               .Include(p => p.ProjectImages)
               .Include(p => p.Documents)
               .Include(p => p.Coins)
               .Include(p => p.ProjectUpdates)
               .Include(p => p.Discussions).ThenInclude(d => d.User).ThenInclude(d => d.UserImage)
               .OrderByDescending(p => p.CreationDate)
               .ToList();
            }
            else
            {
                Projects = _context.Projects
               .Where(p => p.ProjectStatus == ProjectStatusEnum.Accepted ||
                           p.ProjectStatus == ProjectStatusEnum.EndSurvey)
               .Where(p => p.ReceivedFunding >= p.FundingGoal)
               .Include(p => p.User)
               .Include(p => p.ProjectImages)
               .Include(p => p.Documents)
               .Include(p => p.Coins)
               .Include(p => p.ProjectUpdates)
               .Include(p => p.Discussions).ThenInclude(d => d.User).ThenInclude(d => d.UserImage)
               .OrderByDescending(p => p.CreationDate)
               .Skip(pageIndex)
               .Take(pageSize).ToList()
               .ToList();
            }
            foreach (var project in Projects)
            {
                var transactions = new List<int>();
                for (int i = 0; i < 10; i++)
                {
                    transactions.Add(random.Next(10, 100));
                }
                dummyProducts.Add(new Product()
                {
                    Project = project,
                    ProductName = project.ProductName, //$"asset_{project.Id.ToString()}",
                    //TotalCapitalisation = random.Next(100, 300),
                    //Number = random.Next(100, 800),
                    //Buy = random.Next(10, 100),
                    //Sell = random.Next(10, 150),
                    //Transactions = transactions
                });
            }

            return dummyProducts;
        }

        public int GetAllDummyProductsCount()
        {
            return GetDummyProduts().Count;
        }

        public List<Product> GetFilteredDummyProducts(int pageIndex, int pageSize)
        {
            return GetDummyProduts().Skip(pageIndex).Take(pageSize).ToList();
        }

        public Project GetProjectId(int projectId)
        {
            return _context.Projects.Where(p => p.Id == projectId && (p.ProjectStatus == ProjectStatusEnum.Accepted || p.ProjectStatus == ProjectStatusEnum.EndSurvey))
                .Include(p => p.User)
                .Include(p => p.User.UserImage)
                .Include(p => p.ProjectImages)
                .Include(p => p.Coins)
                .Include(p => p.Documents)
                .Include(p => p.ProjectUpdates)
                .Include(p => p.Discussions).ThenInclude(d => d.User).ThenInclude(d => d.UserImage)
                .OrderByDescending(p => p.CreationDate)
                .FirstOrDefault();
        }

        public bool confirmProject(int id)
        {

            Project project = _context.Projects.Where(p => p.Id == id).FirstOrDefault();
            project.ProjectStatus = ProjectStatusEnum.Accepted;
            if (project.CommissionTokenStirblock != 0)
            {
                try
                {
                    config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", true, true)
                                  .Build();
                    var CommissionConfig = config.GetSection("ComissionConfig");
                    var comissionUser = _context.Users.Where(u => u.Login == Convert.ToString(CommissionConfig["ComissionLogin"])).FirstOrDefault();
                    var coin = new Coin();

                    coin.CoinValue = Constants.coinValue;
                    coin.CoinsNumber = project.CommissionTokenStirblock;
                    coin.UserId = comissionUser.Id;
                    coin.ProjectId = id;

                    AddCoinsToProject(coin);
                }
                catch (ArgumentException ex)
                {
                    return false;
                }
            }
            _context.SaveChanges();
            var user = _context.Users.Where(u => u.Id == project.UserId).FirstOrDefault();
            _notificationsService.SendProjectConfirmationNotification(user, project);

            return true;
        }

        public async Task<object> AddFroalaImages(HttpContext context)
        {
            var theFile = context.Request.Form.Files.GetFile("file");

            // Get the server path, wwwroot
            string webRootPath = HostingEnvironment.WebRootPath;

            // Building the path to the uploads directory
            var fileRoute = Path.Combine(webRootPath + Constants.froalaImagesSharedPath);

            // Get the mime type
            var mimeType = context.Request.Form.Files.GetFile("file").ContentType;

            // Get File Extension
            string extension = System.IO.Path.GetExtension(theFile.FileName);

            // Generate Random name.
            string name = Guid.NewGuid().ToString().Substring(0, 8) + extension;

            // Build the full path inclunding the file name
            string link = Path.Combine(fileRoute, name);

            // Create directory if it does not exist.
            FileInfo dir = new FileInfo(fileRoute);
            dir.Directory.Create();

            // Basic validation on mime types and file extension
            string[] imageMimetypes = { "image/gif", "image/jpeg", "image/pjpeg", "image/x-png", "image/png", "image/svg+xml" };
            string[] imageExt = { ".gif", ".jpeg", ".jpg", ".png", ".svg", ".blob" };

            try
            {
                if (Array.IndexOf(imageMimetypes, mimeType) >= 0 && (Array.IndexOf(imageExt, extension) >= 0))
                {
                    // Copy contents to memory stream.
                    Stream stream;
                    stream = new MemoryStream();
                    theFile.CopyTo(stream);
                    stream.Position = 0;
                    String serverPath = link;

                    // Save the file
                    using (FileStream writerFileStream = System.IO.File.Create(serverPath))
                    {
                        await stream.CopyToAsync(writerFileStream);
                        writerFileStream.Dispose();
                    }

                    // Return the file path as json
                    Hashtable imageUrl = new Hashtable();
                    imageUrl.Add("link", Constants.BaseUrl + Constants.froalaImagesSharedPath + name);

                    return imageUrl;
                }
                throw new ArgumentException("The image did not pass the validation");
            }
            catch (ArgumentException ex)
            {
                return ex.Message;
            }
        }

        public bool RemoveFroalaImages(List<string> images)
        {
            string webRootPath = HostingEnvironment.WebRootPath;

            var fileRoute = Path.Combine(webRootPath + Constants.froalaImagesSharedPath);

            var succes = false;

            for (int i = 0; i < images.Count; i++)
            {
                string link = Path.Combine(fileRoute, images[i]);
                if (File.Exists(link))
                {
                    File.Delete(link);
                    succes = true;
                }
                else
                {
                    succes = false;
                }
            }
            if (succes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveFroalaVideos(List<string> videosName)
        {
            string webRootPath = HostingEnvironment.WebRootPath;

            var fileRoute = Path.Combine(webRootPath + Constants.froalaVideosSharedPath);

            var succes = false;

            for (int i = 0; i < videosName.Count; i++)
            {
                string link = Path.Combine(fileRoute, videosName[i]);
                if (File.Exists(link))
                {
                    File.Delete(link);
                    succes = true;
                }
                else
                {
                    succes = false;
                }
            }
            if (succes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<object> AddFroalaVideos(HttpContext context)
        {
            var theFile = context.Request.Form.Files.GetFile("file");

            // Get the server path, wwwroot
            string webRootPath = HostingEnvironment.WebRootPath;

            // Building the path to the uploads directory
            var fileRoute = Path.Combine(webRootPath + Constants.froalaVideosSharedPath);

            // Get the mime type
            var mimeType = context.Request.Form.Files.GetFile("file").ContentType;

            // Get File Extension
            string extension = System.IO.Path.GetExtension(theFile.FileName);

            // Generate Random name.
            string name = Guid.NewGuid().ToString().Substring(0, 8) + extension;

            // Build the full path inclunding the file name
            string link = Path.Combine(fileRoute, name);

            // Create directory if it does not exist.
            FileInfo dir = new FileInfo(fileRoute);
            dir.Directory.Create();

            // Basic validation on mime types and file extension
            string[] videoMimetypes = { "video/mp4", "video/webm", "video/ogg" };
            string[] videoExt = { ".mp4", ".webm", ".ogg" };

            try
            {
                if (Array.IndexOf(videoMimetypes, mimeType) >= 0 && (Array.IndexOf(videoExt, extension) >= 0))
                {
                    // Copy contents to memory stream.
                    Stream stream;
                    stream = new MemoryStream();
                    theFile.CopyTo(stream);
                    stream.Position = 0;
                    String serverPath = link;

                    // Save the file
                    using (FileStream writerFileStream = System.IO.File.Create(serverPath))
                    {
                        await stream.CopyToAsync(writerFileStream);
                        writerFileStream.Dispose();
                    }

                    // Return the file path as json
                    Hashtable videoUrl = new Hashtable();
                    videoUrl.Add("link", Constants.BaseUrl + Constants.froalaVideosSharedPath + name);

                    return videoUrl;
                }
                throw new ArgumentException("The video did not pass the validation");
            }
            catch (ArgumentException ex)
            {
                return ex.Message;
            }
        }

        public async Task<object> AddFroalaFiles(HttpContext context)
        {
            var theFile = context.Request.Form.Files.GetFile("file");

            // Get the server path, wwwroot
            string webRootPath = HostingEnvironment.WebRootPath;

            // Building the path to the uploads directory
            var fileRoute = Path.Combine(webRootPath + Constants.froalaFilesSharedPath);

            // Get the mime type
            var mimeType = context.Request.Form.Files.GetFile("file").ContentType;

            // Get File Extension
            string extension = System.IO.Path.GetExtension(theFile.FileName);

            // Generate Random name.
            string name = Guid.NewGuid().ToString().Substring(0, 8) + extension;

            // Build the full path inclunding the file name
            string link = Path.Combine(fileRoute, name);

            // Create directory if it does not exist.
            FileInfo dir = new FileInfo(fileRoute);
            dir.Directory.Create();

            // Basic validation on mime types and file extension
            string[] fileMimetypes = { "text/plain", "application/msword","application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/x-pdf", "application/pdf","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                       "application/vnd.ms-powerpoint","application/vnd.openxmlformats-officedocument.presentationml.presentation","application/vnd.ms-excel", "application/json", "text/html", "application/docx" };
            string[] fileExt = { ".txt", ".pdf", ".doc", ".json", ".html", ".docx", ".pptx", ".ppt", ".xlsx", ".xla", ".xlt", ".xls" };

            try
            {
                if (Array.IndexOf(fileMimetypes, mimeType) >= 0 && (Array.IndexOf(fileExt, extension) >= 0))
                {
                    // Copy contents to memory stream.
                    Stream stream;
                    stream = new MemoryStream();
                    theFile.CopyTo(stream);
                    stream.Position = 0;
                    String serverPath = link;

                    // Save the file
                    using (FileStream writerFileStream = System.IO.File.Create(serverPath))
                    {
                        await stream.CopyToAsync(writerFileStream);
                        writerFileStream.Dispose();
                    }

                    // Return the file path as json
                    Hashtable fileUrl = new Hashtable();
                    fileUrl.Add("link", Constants.BaseUrl + Constants.froalaFilesSharedPath + name);

                    return fileUrl;
                }
                throw new ArgumentException("The file did not pass the validation");
            }
            catch (ArgumentException ex)
            {
                return ex.Message;
            }
        }

        public bool RemoveFroalaFiles(List<string> filename)
        {
            string webRootPath = HostingEnvironment.WebRootPath;

            var fileRoute = Path.Combine(webRootPath + Constants.froalaFilesSharedPath);

            var succes = false;

            for (int i = 0; i < filename.Count; i++)
            {
                string link = Path.Combine(fileRoute, filename[i]);
                if (File.Exists(link))
                {
                    File.Delete(link);
                    succes = true;
                }
                else
                {
                    succes = false;
                }
            }
            if (succes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CreateBillsForBakers(Project project)
        {
            HtmlToPdf converter = new HtmlToPdf();
            var coiner = project.User;
            foreach (var coin in project.Coins)
            {
                var backer = coin.User;

                var pdfPath = $"{Guid.NewGuid()}.pdf";

                var TTC = Math.Round(Convert.ToDecimal(coin.CoinsNumber), 2);
                var HT = Math.Round(TTC / (1 + (project.Product_TVA / 100)), 2);
                var TVA = Math.Round(TTC - HT, 2);
                var acquisition = Math.Round((TTC / project.FundingGoal) * 100, 2);

                var bill = new Bill
                {
                    UserId = backer.Id,
                    ProjectId = project.Id,
                    PDFPath = pdfPath
                };

                _context.Bills.Add(bill);
                _context.SaveChanges();

                var billContent = BillTemplate.BillContent()
                 .Replace(BillTemplate.CoinerLastName, coiner.LastName)
                 .Replace(BillTemplate.CoinerAdress, coiner.Address)
                 .Replace(BillTemplate.BackerLastName, backer.LastName)
                 .Replace(BillTemplate.BackerFirstName, backer.FirstName)
                 .Replace(BillTemplate.BackerEmail, backer.Email)
                 .Replace(BillTemplate.BillId, bill.Id.ToString())
                 .Replace(BillTemplate.BillCreationDate, bill.CreationDate.ToString("dd/MM/yyyy"))
                 .Replace(BillTemplate.BillDescription, project.Product_BillDescription)
                 .Replace(BillTemplate.Acquisition, acquisition.ToString())
                 .Replace(BillTemplate.ProductTVA, Math.Round(project.Product_TVA, 0).ToString())
                 .Replace(BillTemplate.TVA, TVA.ToString())
                 .Replace(BillTemplate.HT, HT.ToString())
                 .Replace(BillTemplate.TTC, String.Format("{0:.00}", TTC));

                // create a new pdf document converting html string
                var pdfBill = converter.ConvertHtmlString(billContent);

                var billsPath = HostingEnvironment.WebRootPath + Constants.BillsSharedPath;

                //Check if directory exist
                if (!Directory.Exists(billsPath))
                {
                    Directory.CreateDirectory(billsPath); //Create directory if it doesn't exist
                }

                // save pdf document
                pdfBill.Save(Path.Combine(billsPath, pdfPath));

                // close pdf document
                pdfBill.Close();
            }
        }

        public List<Bill> GetUserBills(int userId)
        {
            var bills = _context.Bills.Where(b => b.UserId == userId).OrderByDescending(b => b.CreationDate).ToList();

            if (bills != null)
            {
                FetchPdfContent(bills);
            }
            else
            {
                throw new UserNotFoundException(userId);
            }

            return bills;
        }

        public void FetchPdfContent(List<Bill> bills)
        {
            if (bills.Count != 0)
            {
                foreach (var bill in bills)
                {
                    var pdfFilePath = "wwwroot\\" + Constants.BillsSharedPath + Path.GetFileName(bill.PDFPath);
                    if (File.Exists(pdfFilePath))
                    {
                        bill.Content = Convert.ToBase64String(File.ReadAllBytes(pdfFilePath));
                    }
                    else
                    {
                        throw new FileNotFoundException("pdf file not found", bill.PDFPath);
                    }
                }
            }
        }

        public async Task<Project> RefundUsers(int projectId)
        {
            config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", true, true)
              .Build();
            var CommissionConfig = config.GetSection("ComissionConfig");
            var comissionUser = _context.Users.Where(u => u.Login == Convert.ToString(CommissionConfig["ComissionLogin"])).FirstOrDefault();

            var project = _context.Projects.Where(p => p.Id == projectId).Include(p => p.User).Include(p => p.Coins).ThenInclude(c => c.User)
                                                                         .FirstOrDefault();

            var projectOwnerWalletId = project.User.WalletId;
            var totalAmountToRefund = 0;

            foreach (var baker in project.Coins)
            {
                if (baker.User.Login != comissionUser.Login)
                {
                    await _blockChainService.CreditCurrency(baker.User.Id, baker.CoinsNumber);
                    totalAmountToRefund += baker.CoinsNumber;
                }
            }
            var refundDone = await _mangöPayService.RefundUsers(projectOwnerWalletId, totalAmountToRefund);

            if (refundDone)
            {
                project.ProjectStatus = ProjectStatusEnum.Refused;
                _context.SaveChanges();
                return project;
            }
            else
            {
                return null;
            }
        }

    }
}
