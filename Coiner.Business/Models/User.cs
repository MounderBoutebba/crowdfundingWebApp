using System;
using System.Collections.Generic;
using Coiner.Business.Models.Bases;
using Coiner.Business.Models.Enums;

namespace Coiner.Business.Models
{
    public class User : BusinessEntityBase
    {
        public string FirstName { get; set; }

        public UserTypeEnum UserType { get; set; }

        public string LastName { get; set; }

        public GenderEnum Gender { get; set; }

        public  DateTime? BirthDay { get; set; }

        public string Email { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string Job { get; set; }

        // BlockChain Fields

        public string BlockChainAddress { get; set; }
        
        public string BlockChainPublicKey { get; set; }

        // BlockChain Fields

        public int UserCoinsNumber { get; set; }

        // Account Verification Fields

        public string ActivationToken { get; set; }

        public AccountVerificationEnum IsActive { get; set; }

        // Provider fild

        public ProvidersEnum Provider { get; set; }

        // buisness fields

        public string Siren { get; set; }

        public string Tva { get; set; }
               
        //KIC informations

        public bool Kyc { get; set; }

        public bool KycNotificationSent { get; set; }

        public string WalletId { get; set; }

        //Relations

        public string Address { get; set; }

        public UserImage UserImage { get; set; }

        public UserWallet UserWallet { get; set; }

        public ICollection<Project> Projects { get; set; }

        public ICollection<Coin> Coins { get; set; }

        public ICollection<Bill> Bills { get; set; }
    }
}
