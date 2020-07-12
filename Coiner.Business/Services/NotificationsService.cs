using Coiner.Business.Context;
using Coiner.Business.Models;
using System;
using System.Linq;


namespace Coiner.Business.Services
{
    public class NotificationsService
    {
        private CoinerContext _context = new CoinerContext();
        private void SendNotification(int templateId, string title, string textContent, User user = null, Project Project = null)
        {

            var notificationConfiguration = _context.NotificationConfiguration
                                                      .Where(e => e.Id == templateId)
                                                      .Single();
            // We can add other sending methodes later ( mobile, email...)
            if (notificationConfiguration.NotificationOutputType.HasFlag(NotificationOutputTypeEnum.Application))
            {
                SendNotificationApp(templateId, title, textContent, user, Project);
            }
        }

        private void SendNotificationApp(int templateId, string title, string textContent, User user = null, Project project = null)
        {
            var notificationProduced = new NotificationProduced
            {
                NotificationTemplateId = templateId,
                Title = title,
                Content = textContent,
                NotificationOutputType = NotificationOutputTypeEnum.Application,
                CreateDate = DateTimeOffset.Now,
                AppReadStatus = false,
                NotificationSent = new NotificationSent
                {
                    SendStatus = true,
                    SendTime = DateTimeOffset.Now
                }
            };
            if(project != null)  notificationProduced.ProjectId = project.Id;
            if (user != null) notificationProduced.UserId = user.Id;
            _context.NotificationProduced.Add(notificationProduced);
            _context.SaveChanges();
        }


        //Liste of individual notifications services
        public void SendProjectCreartionNotification(User user, Project project)
        {

            // from notification template
            var notificationTemplate = _context.NotificationTemplate
                                              .Where(e => e.NotificationConfigurationId == Constants.projectCreationConfigurationId)
                                              .Single();

            var title = notificationTemplate.Title.Replace("{Project_Name}", project.ProjectName);
            var textContent = notificationTemplate.Content.Replace("{User_Name}", user.FirstName + " " + user.LastName)
                                                          .Replace("{Project_Name}", project.ProjectName);
            SendNotification(Constants.projectCreationConfigurationId, title, textContent, user, project);
        }


        public void SendBakingConfirmationNotification(User user, Project project)
        {


            var notificationTemplate = _context.NotificationTemplate
                                              .Where(e => e.NotificationConfigurationId == Constants.projectBakingConfigurationId)
                                              .Single();

            var title = notificationTemplate.Title.Replace("{Project_Name}", project.ProjectName);
            var textContent = notificationTemplate.Content.Replace("{User_Name}", user.FirstName + " " + user.LastName)
                                                          .Replace("{Project_Name}", project.ProjectName);
            SendNotification(Constants.projectBakingConfigurationId, title, textContent, user, project);
        }

        public void SendFundingEndNotification(User user, Project project, int receiver)
        {


            var notificationTemplate = _context.NotificationTemplate
                                              .Where(e => e.NotificationConfigurationId == receiver)
                                              .Single();

            var title = notificationTemplate.Title.Replace("{Project_Name}", project.ProjectName);
            var textContent = notificationTemplate.Content.Replace("{User_Name}", user.FirstName + " " + user.LastName)
                                                          .Replace("{Project_Name}", project.ProjectName);
            SendNotification(receiver, title, textContent, user, project);
        }

        public void SendProjectConfirmationNotification(User user, Project project)
        {
            var notificationTemplate = _context.NotificationTemplate
                                              .Where(e => e.NotificationConfigurationId == Constants.projectConfirmationConfigurationId)
                                              .Single();

            var title = notificationTemplate.Title.Replace("{Project_Name}", project.ProjectName);
            var textContent = notificationTemplate.Content.Replace("{User_Name}", user.FirstName + " " + user.LastName)
                                                          .Replace("{Project_Name}", project.ProjectName);

            SendNotification(Constants.projectConfirmationConfigurationId, title, textContent, user, project);
        }

        public void SendPivateKeyNotification(User user, string privKey, string publicKey, string Adress)
        {
            var notificationTemplate = _context.NotificationTemplate
                                              .Where(e => e.NotificationConfigurationId == Constants.privateKeyConfigurationId)
                                              .Single();

            var title = notificationTemplate.Title;
            var textContent = notificationTemplate.Content.Replace("{User_Name}", user.FirstName + " " + user.LastName)
                                                          .Replace("{User_PrivKey}", Constants.checkYourEmail)
                                                          .Replace("{User_PubKey}", Constants.checkYourEmail)
                                                          .Replace("{User_Adress}", Constants.checkYourEmail);

            SendNotification(Constants.privateKeyConfigurationId, title, textContent, user);
        }

        public void SendWelcomeNotification(User user)
        {
            var notificationTemplate = _context.NotificationTemplate
                                              .Where(e => e.NotificationConfigurationId == Constants.welcomeyConfigurationId)
                                              .Single();

            var title = notificationTemplate.Title;
            var textContent = notificationTemplate.Content.Replace("{User_Name}", user.FirstName + " " + user.LastName)
                                                          .Replace("{Coin_Number}", Constants.DefaultUserCoinsNmber.ToString());

            SendNotification(Constants.welcomeyConfigurationId, title, textContent, user);
        }

        public void SendPasswordChangeNotification(User user)
        {
            var notificationTemplate = _context.NotificationTemplate
                                              .Where(e => e.NotificationConfigurationId == Constants.passwordChangeConfigurationId)
                                              .Single();

            var title = notificationTemplate.Title;
            var textContent = notificationTemplate.Content.Replace("{User_Name}", user.FirstName + " " + user.LastName);

            SendNotification(Constants.passwordChangeConfigurationId, title, textContent, user);
        }


    }
}
