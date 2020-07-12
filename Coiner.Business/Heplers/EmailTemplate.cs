using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Coiner.Business.Heplers
{
    public class EmailTemplate
    {
        public static string UserName = "{User_Name}";
        public static string UserId = "{User_Id}";
        public static string EmailText = "{Email_Text}";
        public static string ResetLink = "{Reset_Link}";
        public static string BaseUrl = "{Base_Url}";
        public static string ProjectName = "{Project_Name}";
        public static string ProjectDetails = "{ProjectDetails}";
        public static string ProjectId = "{Project_Id}";
        //contact us
        public static string UserEmail = "{User_Email}";
        public static string UserMessage = "{User_Message}";

        //blockchain privKey
        public static string UserPrivKey = "{User_PrivKey}";
        public static string UserPubKey = "{User_PubKey}";
        public static string UserAdress = "{User_Adress}";
        public static string ProductName = "{Product_Name}";
        public static string Qte = "{Qte}";
        public static string TotalValue = "{Total_Value}";

        //email verification token
        public static string ActivationToken = "{User_ActivationToken}";

        public static string EmailContent(string path)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), path.Replace("/", "\\"))
                                  .Replace("\\", "/");

            return File.ReadAllText(filePath);
        }
    }

    public class UserCreationEmailTemplate : EmailTemplate
    {
        public static string Path
        {
            get
            {
                return HostingEnvironment.WebRootPath + "/Content/emailPages/welcome.inlined.html";
            }
        }

        public static string Subject
        {
            get
            {
                return "Creation du compte";
            }
        }
    }

    public class UserCreationGoogleEmailTemplate : EmailTemplate
    {
        public static string Path
        {
            get
            {
                return HostingEnvironment.WebRootPath + "/Content/emailPages/welcome-Google.inlined.html";
            }
        }

        public static string Subject
        {
            get
            {
                return "Creation du compte";
            }
        }
    }

    public class ProjectCreationEmailTemplate : EmailTemplate
    {
        public static string Path
        {
            get
            {
                return HostingEnvironment.WebRootPath + "/Content/emailPages/projet-creation.inlined.html";
            }
        }

        public static string Subject
        {
            get
            {
                return "Creation du projet";
            }
        }
    }

    public class ProjectCreationEmailAdminTemplate : EmailTemplate
    {
        public static string Path
        {
            get
            {
                return HostingEnvironment.WebRootPath + "/Content/emailPages/projet-creation-admin.inlined.html";
            }
        }

        public static string Subject
        {
            get
            {
                return "Nouveau projet";
            }
        }
    }

    public class AdminConfirmationEmailTemplate : EmailTemplate
    {
        public static string Path
        {
            get
            {
                return HostingEnvironment.WebRootPath + "/Content/emailPages/projet-confirmation.inlined.html";
            }
        }

        public static string Subject
        {
            get
            {
                return "Confirmation du projet";
            }
        }
    }

    public class AcceptedMiseEmailTemplate : EmailTemplate
    {
        public static string Path
        {
            get
            {
                return HostingEnvironment.WebRootPath + "/Content/emailPages/mise-accepted.inlined.html";
            }
        }

        public static string Subject
        {
            get
            {
                return "Confirmation de la mise";
            }
        }
    }

    public class DemandUpdatePWEmailTemplate : EmailTemplate
    {
        public static string Path
        {
            get
            {
                return HostingEnvironment.WebRootPath + "/Content/emailPages/update-password.inlined.html";
            }
        }

        public static string Subject
        {
            get
            {
                return "Modification de votre mot de passe";
            }
        }
    }

    public class AddQuestionToProjectEmailTemplate : EmailTemplate
    {
        public static string Path
        {
            get
            {
                return HostingEnvironment.WebRootPath + "/Content/emailPages/new-question.inlined.html";
            }
        }

        public static string Subject
        {
            get
            {
                return "Nouvelle question à propos de votre projet";
            }
        }
    }

    public class AddAnswerToProjectEmailTemplate : EmailTemplate
    {
        public static string Path
        {
            get
            {
                return HostingEnvironment.WebRootPath + "/Content/emailPages/new-answer.inlined.html";
            }
        }

        public static string Subject
        {
            get
            {
                return "Nouvelle réponse sur votre question";
            }
        }
    }

    public class FundingEndCoinerEmailTemplate : EmailTemplate
    {
        public static string Path
        {
            get
            {
                return HostingEnvironment.WebRootPath + "/Content/emailPages/funding-end-coiner.inlined.html";
            }
        }

        public static string Subject
        {
            get
            {
                return "Fin de la levé";
            }
        }
    }

    public class FundingEndBackerEmailTemplate : EmailTemplate
    {
        public static string Path
        {
            get
            {
                return HostingEnvironment.WebRootPath + "/Content/emailPages/funding-end-backer.inlined.html";
            }
        }

        public static string Subject
        {
            get
            {
                return "Fin de la levé";
            }
        }
    }

    public class ContactUsEmail : EmailTemplate
    {
        public static string Path
        {
            get
            {
                return HostingEnvironment.WebRootPath + "/Content/emailPages/contact-us.inlined.html";
            }
        }

        public static string Subject
        {
            get
            {
                return "Message de contact";
            }
        }
    }

    public class BlockChainPrivKeyEmailTemplate : EmailTemplate
    {
        public static string Path
        {
            get
            {
                return HostingEnvironment.WebRootPath + "/Content/emailPages/blockchain-privkey.inlined.html";
            }
        }

        public static string Subject
        {
            get
            {
                return "BlockChain Clé Privé ";
            }
        }
    }

    public class BuyEndEmailTemplate : EmailTemplate
    {
        public static string Path
        {
            get
            {
                return HostingEnvironment.WebRootPath + "/Content/emailPages/bying-end.inlined.html";
            }
        }

        public static string Subject
        {
            get
            {
                return "Limite d’achat";
            }
        }
    }

    public class SaleEndEmailTemplate : EmailTemplate
    {
        public static string Path
        {
            get
            {
                return HostingEnvironment.WebRootPath + "/Content/emailPages/sale-end.inlined.html";
            }
        }

        public static string Subject
        {
            get
            {
                return "Limite de vente";
            }
        }
    }
    public class AddNewsRequestEmailTemplate : EmailTemplate
    {
        public static string Subject
        {
            get
            {
                return "Demande de Diffusion d’actualité du projet";
            }
        }
    }
    public class EditProjectRequestEmailTemplate : EmailTemplate
    {
        public static string Subject
        {
            get
            {
                return "Demande de modification du projet";
            }
        }
    }
}
