using System;
using System.Collections.Generic;
using System.Text;

namespace Coiner.Business
{
    public class Constants
    {
        // Images Folder

        public const string UserImagesSharedPath = @"\Content\userImages\";
        public const string ProjectImagesSharedPath = @"\Content\images\";
        public const string froalaImagesSharedPath = @"\Content\froalaImages\";
        public const string froalaVideosSharedPath = @"\Content\froalaVideos\";
        public const string froalaFilesSharedPath = @"\Content\froalaFiles\";
        public const string BillsSharedPath = @"\Content\bills\"; 

        public const string BaseUrl = @"http://localhost:63730";
        public const string GoogleRecaptchaUrl = @"https://www.google.com/recaptcha/api/siteverify?secret=";
        
        public const string ProjectDocumentsSharedPath = @"\Content\documents\";

        public const string hash = "Beitcan";

        // Constants for User validation inputs
        public const int AddressMaxLength = 100;
        public const int EmailMaxLength = 50;
        public const int PhoneNumberMaxLength = 25;
        public const int userPasswordMinLength = 6;
        public const int userPasswordMaxLength = 30;
        public const int userNameMaxLength = 30; 
        public const string PhoneNumberRegEx = @"((?:\+|00)[17](?: |\-)?|(?:\+|00)[1-9]\d{0,2}(?: |\-)?|(?:\+|00)1\-\d{3}(?: |\-)?)?(0\d|\([0-9]{3}\)|[1-9]{0,3})(?:((?: |\-)[0-9]{2}){4}|((?:[0-9]{2}){4})|((?: |\-)[0-9]{3}(?: |\-)[0-9]{4})|([0-9]{7}))";
        public const string UserEmailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                               @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
        // Constants for Project validation inputs
        public const int ProjectName = 30;
        public const int PresentationTeam = 1000;
        public const int ProjectAdress = 100;
        public const int FundingGoal = 12;
        public const int PercentageMaxLength = 3;
        public const string PercentageAsset = @"/^(100([\.][0]{1,})?$|[0-9]{1,2}([\.][0 - 9]{1,})?)$/";
        public const int ProjectDescreption = 300;

        public const int DefaultUserCoinsNmber = 100;
        public const int coinValue = 1;

        public const string CurrencyAssetName = "EUR";
    
        //------------ notifications configuration id --------------------
        public const int projectCreationConfigurationId = 1;
        public const int projectBakingConfigurationId = 2;
        public const int projectFundingEndBakerConfigurationId = 3;
        public const int projectFundingEndCoinerConfigurationId = 4;
        public const int projectConfirmationConfigurationId = 5;
        public const int privateKeyConfigurationId = 6;
        public const int welcomeyConfigurationId = 7;
        public const int passwordChangeConfigurationId = 8;
        //------------ notifications configuration id --------------------
        public const string checkYourEmail = "Consulter votre email";

        //----- mango pay constants
        //-- wallet
        public const string walletDescription = "My wallet";

    }
}
