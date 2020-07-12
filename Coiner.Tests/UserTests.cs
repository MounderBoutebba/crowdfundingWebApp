using Coiner.Business.Context;
using Coiner.Business.Heplers;
using Coiner.Business.Models;
using Coiner.Business.Models.Enums;
using Coiner.Business.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Coiner.Tests
{
    [TestClass]
    public class UserTests
    {
        //[TestMethod]
        //public void CreateUser()
        //{
        //    using (var context = new CoinerContext())
        //    {
        //        var count = context.Users.Local.Count;

        //        var user = new User
        //        {
        //            FirstName = "CoinerUser",
        //            LastName = "CoinerUser",
        //            Job = "Full-Stack Web Developer",
        //            Email = "coiner.user@outlook.com",
        //            Login = "coiner.user@outlook.com",
        //            Password = "coineruser",
        //            PhoneNumber = "07779167967",
        //            Gender = GenderEnum.Male,
        //            BirthDay = new DateTime(1992, 10, 18),
        //            Address = new Address
        //            {
        //                Address1 = "Skikda",
        //                Address2 = "Constatnine",
        //                Country = "Algeria",
        //                Town = "Skikda",
        //                ZipCode = "21000"
        //            }
        //        };

        //        context.Users.Add(user);
        //        context.SaveChanges();
        //        Assert.IsTrue(context.Users.Local.Count == (count + 1));
        //    }
        //}

        //[TestMethod]
        //public void GetUsers()
        //{
        //    using (var context = new CoinerContext())
        //    {
        //        var users = context.Users.Include(p => p.UserImage) 
        //                                 .ToList();
        //    }
        //}

        [TestMethod]
        public async Task CreateUser_WhenCall_MockCreateUser()
        {
            User user1 = new User()
            {
                FirstName = "mustafa",
                LastName = "derouaz",
                Email = "aaa@gmailyyy.com",
                Password = "azerty",
                Login = "aaa@gmail.com"
            };

            var mockEmail = new Mock<EmailService>();
            mockEmail.Setup(e => e.SendUserCreationEmail(user1)).Verifiable();

            var options = new DbContextOptionsBuilder<CoinerContext>()
                .UseInMemoryDatabase(databaseName: "CreateUser_WhenCall_MockCreateUser")
                .Options;

            using (var context = new CoinerContext(options))
            {
                var service = new UserService(context, mockEmail.Object);
                await service.CreateUser(user1);
            }
            using (var context = new CoinerContext(options))
            {
                Assert.AreEqual(1, context.Users.Count());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateUser_WhenCall_CheckEmail()
        {
            User user1 = new User()
            {
                FirstName = "mustafa",
                LastName = "derouaz",
                Email = "aaagmailyyy.com",
                Password = "azerty",
                Login = "aaa@gmail.com"
            };

            var mockEmail = new Mock<EmailService>();
            mockEmail.Setup(e => e.SendUserCreationEmail(user1)).Verifiable();

            var options = new DbContextOptionsBuilder<CoinerContext>()
                .UseInMemoryDatabase(databaseName: "CreateUser_WhenCall_MockCreateUser")
                .Options;

            using (var context = new CoinerContext(options))
            {
                var service = new UserService(context, mockEmail.Object);
                await service.CreateUser(user1);
            }
            using (var context = new CoinerContext(options))
            {
                Assert.AreEqual(2, context.Users.Count());
            }
        }

        [TestMethod]
        public async Task CreateUser_WhenCall_CheckExistLogin()
        {
            bool Response;
            User user1 = new User()
            {
                FirstName = "mustafa",
                LastName = "derouaz",
                Email = "aaa@gmailyyy.com",
                Password = "azerty",
                Login = "aaa@gmail.com"
            };

            User user2 = new User()
            {
                FirstName = "mustafa",
                LastName = "derouaz",
                Email = "aaa@gmailyyy.com",
                Password = "azerty",
                Login = "aaa@gmail.com"
            };

            var mockEmail = new Mock<EmailService>();
            mockEmail.Setup(e => e.SendUserCreationEmail(user1)).Verifiable();

            var options = new DbContextOptionsBuilder<CoinerContext>()
                .UseInMemoryDatabase(databaseName: "CreateUser_WhenCall_MockCreateUser")
                .Options;

            using (var context = new CoinerContext(options))
            {
                var service = new UserService(context, mockEmail.Object);
                Response = await service.CreateUser(user1);
                Response = await service.CreateUser(user2);
                Assert.IsFalse(Response);
            }

        }
    }
}
