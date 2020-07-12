using Coiner.Business;
using Coiner.Business.Context;
using Coiner.Business.Models;
using Coiner.Business.Models.Enums;
using Coiner.Business.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coiner.Tests
{
    [TestClass]
    public class ProjectsTests
    {
        ProjectService _projectService;
        CoinerContext _Context;

        [TestInitialize]
        public void Setup()
        {
            _projectService = new ProjectService();
            _Context = new CoinerContext();
        }

        [TestCleanup]
        public void CleanMethod()
        {
            deleteProject();
        }

        [TestMethod]
        public void GetAllProjectsCount_AcceptedEndsurvey_ReturnInt()
        {
            CreateProject(5);
            var result = _projectService.GetAllProjectsCount();
            Assert.AreEqual(result, 5);
        }

        [TestMethod]
        public void GetLatestProjects_ReturnNineLastPrjects()
        {
            CreateProject(9);
            var result = _projectService.GetLatestProjects();
            Assert.AreEqual(result.Count(), 9);
        }

        [TestMethod]
        public void GetUserProjects_whenCall_ReturnAllProjectInDB()
        {
            CreateProject(3);
            CreateProject(3);
            CreateNewUserWithId(true);
            var result = _projectService.GetUserProjects(GetLastUserId(), 0, 6);
            Assert.AreEqual(result.Count(), 6);
        }

        [TestMethod]
        public void GetUserProjects_whenCall_ReturnAllUserProjects()
        {
            CreateProject(3);
            CreateProject(3);
            var result = _projectService.GetUserProjects(GetLastUserId(), 0, 6);
            Assert.AreEqual(result.Count(), 3);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetUserProjects_InExistentUserId_ReturnException()
        {
            CreateProject(3);
            CreateProject(3);
            var result = _projectService.GetUserProjects(GetLastUserId()+1, 0, 6);
        }

        public void CreateProject(int number)
        {
            User user = CreateNewUser();
            for (int i = 0; i < number; i++)
            {
                var project = new Project
                {
                    ProjectName = "Project" + i,
                    ProjectDescription = "This my test project",
                    FundingGoal = 5000,
                    ProjectStatus = ProjectStatusEnum.EndSurvey,
                    ProjectType = ProjectTypeEnum.Society,
                    FundraisingPeriod = 5,
                    ReceivedFunding = 0,
                    User = user,
                    ActivityType = "Car",
                    Coins = CreateNewCoins()
                };
                _Context.Projects.Add(project);
                _Context.SaveChanges();
            }
        }

        public void deleteProject()
        {
            _Context.Database.ExecuteSqlCommand("TRUNCATE TABLE projects");
            _Context.Database.ExecuteSqlCommand("TRUNCATE TABLE addresses");
            _Context.Database.ExecuteSqlCommand("TRUNCATE TABLE coins");
            _Context.Database.ExecuteSqlCommand("TRUNCATE TABLE users");
        }

        public User CreateNewUser()
        {
            using (var context = new CoinerContext())
            {
                var user = new User
                {
                    FirstName = "Test",
                    LastName = "TestUser",
                    Job = "Full-Stack Web Developer",
                    Email = "TestUser@mail.com",
                    Login = "TestUser@mail.com",
                    Password = "Beitcan2022",
                    PhoneNumber = "07779167967",
                    Gender = GenderEnum.Male,
                    BirthDay = new DateTime(1992, 10, 18),
                    Address = "Skikda",
                    //new Address
                    //{
                    //    Address1 = "Skikda",
                    //    Address2 = "Constatnine",
                    //    Country = "Algeria",
                    //    Town = "Skikda",
                    //    ZipCode = "21000"
                    //},
                    UserImage = new UserImage
                    {
                        Path = "C:\\Documents\\Images\\MyProfileImage.png",
                        IsDefault = true,
                    },
                };
                return user;
            }
        }

        public List<Coin> CreateNewCoins()
        {
            return new List<Coin>
            {
                new Coin
                {
                     CoinStatus = CoinStatusEnum.Unused,
                     CoinValue = 100,
                     UsedNumber = 0,
                     CoinsNumber = 10,
                     CoinsMonetizedNumber = 0,
                     CoinPrice = 1,
                     UserId = 1
                }
            };
        }

        public int GetLastUserId()
        {
            var userId = _Context.Users.Last().Id;
            return userId;
        }

        public void CreateNewUserWithId(bool isAdmin)
        {

            User user;
            if (isAdmin)
            {
                user = new User
                {
                    FirstName = "Test",
                    LastName = "TestUser",
                    Job = "Full-Stack Web Developer",
                    Email = "TestUser@mail.com",
                    Login = "coiner@mail.com",
                    Password = "azerty",
                    PhoneNumber = "07779167967",
                    Gender = GenderEnum.Male,
                    BirthDay = new DateTime(1992, 10, 18)
                };
            }
            else
            {
                user = new User
                {
                    FirstName = "Test",
                    LastName = "TestUser",
                    Job = "Full-Stack Web Developer",
                    Email = "TestUser@mail.com",
                    Login = "TestUser@mail.com",
                    Password = "Beitcan2022",
                    PhoneNumber = "07779167967",
                    Gender = GenderEnum.Male,
                    BirthDay = new DateTime(1992, 10, 18)
                };
            }
            _Context.Users.Add(user);
            _Context.SaveChanges();
        }

        //var mock = new Mock<IProjectService>();
        //mock.Setup(p => p.GetUserProjects(1, 0, 6)).Returns(new List<Project>
        //{
        //   new Project
        //   {
        //            ProjectName = "Project",
        //            ProjectDescription = "This my test project",
        //            FundingGoal = 5000,
        //            ProjectStatus = ProjectStatusEnum.EndSurvey,
        //            ProjectType = ProjectTypeEnum.Society,
        //            FundraisingPeriod = 5,
        //            ReceivedFunding = 0,
        //            User = CreateNewUser(),
        //            ActivityType = "Car",
        //            Coins = CreateNewCoins()
        //   }
        //});
        //var result = _projectService.CreateProject(mock.Object);

        [TestMethod]
        public void GetLatestProjects_testMock()
        {
            var mockSet = new Mock<DbSet<Project>>();

            var mockContext = new Mock<CoinerContext>();
            mockContext.Setup(m => m.Projects).Returns(mockSet.Object);

            //var service = new UserService(mockContext.Object);
            //service.CreateUser(new User
            //{
            //    FirstName = "mustafa",
            //    LastName = "derouaz",
            //    Email = "aaa@gmail.com",
            //    Password = "azerty",
            //    Login = "aaa@gmail.com"
            //});
        }
            
    }
}
