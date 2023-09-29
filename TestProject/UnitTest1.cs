using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using ComplaintManagementSystem.Controllers;
//using ComplaintManagementSystem.Data;
using ComplaintManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NUnit.Framework;


namespace ComplaintManagementSystem.Tests
{
    [TestFixture]
    public class ComplaintManagementSystemTest
    {
        private DbContextOptions<ComplaintDbContext> _dbContextOptions;

        [SetUp]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ComplaintDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var dbContext = new ComplaintDbContext(_dbContextOptions))
            {

                var teamData1 = new Dictionary<string, object>
                {
                    { "RepresentativeID", 1 },
                    { "FirstName", "Demo" },
                    { "LastName", "sdfghj44" },
                    { "Email", "demo@gmail.com" }
                };
                var student1 = CreateRepFromDictionary(teamData1);
                var teamData = new Dictionary<string, object>
                {
                    { "CustomerName", "Demo1" },
                    { "ContactNumber", 100 },
                    { "AccountNumber", "sdfghj44" },
                    { "Description", "aswqde" },
                    { "Status", "pending" },
                    { "RepresentativeID", 1 }
                };
                var student = CreateTeamFromDictionary(teamData);

                dbContext.CustomerServiceRepresentatives.Add(student1);
                dbContext.Complaints.Add(student);
                dbContext.SaveChanges();
            }
        }

        [TearDown]
        public void TearDown()
        {
            using (var dbContext = new ComplaintDbContext(_dbContextOptions))
            {
                // Clear the in-memory database after each test
                dbContext.Database.EnsureDeleted();
            }
        }


        [Test]
        public void SubmitNewComplaint_ValidDetails_AddSuccessfully_ComplaintsTable()
        {
            using (var dbContext = new ComplaintDbContext(_dbContextOptions))
            {
                // Arrange
                var controllerType = typeof(ComplaintController);
                var controller = Activator.CreateInstance(controllerType, dbContext);

                // Specify the method signature (parameter types)
                MethodInfo method = controllerType.GetMethod("Submit", new[] { typeof(Complaint) });
                var teamData = new Dictionary<string, object>
                {
                    { "CustomerName", "Demo1" },
                    { "ContactNumber", 100 },
                    { "AccountNumber", "sdfghj44" },
                    { "Description", "aswqde" },
                    { "Status", "pending" },
                    { "RepresentativeID", 1 }
                };
                var student = CreateTeamFromDictionary(teamData);
                
                var result = method.Invoke(controller, new object[] { student });

                Assert.IsNotNull(result);

                var ride = dbContext.Complaints.FirstOrDefault(r => r.ComplaintID == 2);
                Console.WriteLine(ride.ComplaintID);
                Assert.AreEqual(100, ride.ContactNumber);
            }
        }

        [Test]
        public void CreateNewCustomerServiceRepresentative_ValidDetails_AddSuccessfully_CustomerServiceRepresentative()
        {
            using (var dbContext = new ComplaintDbContext(_dbContextOptions))
            {
                // Arrange
                var controllerType = typeof(ComplaintController);
                var controller = Activator.CreateInstance(controllerType, dbContext);

                // Specify the method signature (parameter types)
                MethodInfo method = controllerType.GetMethod("UpdateStatus", new[] { typeof(int), typeof(string) });

                var result = method.Invoke(controller, new object[] { 1, "Resolved"});

                // Assert
                Assert.IsNotNull(result);

                var ride = dbContext.Complaints.FirstOrDefault(r => r.ComplaintID == 1);
                //Assert.IsNotNull(ride);
                Console.WriteLine(ride.Status);
                Assert.AreEqual("Resolved", ride.Status);
            }
        }


        [Test]
        public void Submit_Method_Exists_ComplaintController_with_Complaint_Parameter()
        {
            using (var dbContext = new ComplaintDbContext(_dbContextOptions))
            {
                // Arrange
                var controllerType = typeof(ComplaintController);
                var controller = Activator.CreateInstance(controllerType, dbContext);

                // Specify the method signature (parameter types)
                MethodInfo method = controllerType.GetMethod("Submit", new[] { typeof(Complaint) });
                Assert.IsNotNull(method);
            }
        }

        [Test]
        public void Submit_Method_Exists_ComplaintController_parameter()
        {
            using (var dbContext = new ComplaintDbContext(_dbContextOptions))
            {
                // Arrange
                var controllerType = typeof(ComplaintController);
                var controller = Activator.CreateInstance(controllerType, dbContext);

                // Specify the method signature (parameter types)
                MethodInfo method = controllerType.GetMethod("Submit", new Type[] { });
                Assert.IsNotNull(method);
            }
        }

        [Test]
        public void Index_Method_Exists_CustomerServiceRepresentativeController()
        {
            using (var dbContext = new ComplaintDbContext(_dbContextOptions))
            {
                // Arrange
                var controllerType = typeof(CustomerServiceRepresentativeController);
                var controller = Activator.CreateInstance(controllerType, dbContext);

                // Specify the method signature (parameter types)
                MethodInfo method = controllerType.GetMethod("Index");
                Assert.IsNotNull(method);
            }
        }


        [Test]
        public void CustomerServiceRepresentative_ClassExists()
        {
            string assemblyName = "ComplaintManagementSystem";
            string typeName = "ComplaintManagementSystem.Models.CustomerServiceRepresentative";
            Assembly assembly = Assembly.Load(assemblyName);
            Type rideType = assembly.GetType(typeName);
            Assert.IsNotNull(rideType);
            var ride = Activator.CreateInstance(rideType);
            Assert.IsNotNull(ride);
        }

        [Test]
        public void Complaint_ClassExists()
        {
            string assemblyName = "ComplaintManagementSystem";
            string typeName = "ComplaintManagementSystem.Models.Complaint";
            Assembly assembly = Assembly.Load(assemblyName);
            Type rideType = assembly.GetType(typeName);
            Assert.IsNotNull(rideType);
            var ride = Activator.CreateInstance(rideType);
            Assert.IsNotNull(ride);
        }
        [Test]
        public void ComplaintException_ClassExists()
        {
            string assemblyName = "ComplaintManagementSystem";
            string typeName = "ComplaintManagementSystem.Models.ComplaintException";
            Assembly assembly = Assembly.Load(assemblyName);
            Type rideType = assembly.GetType(typeName);
            Assert.IsNotNull(rideType);
        }



        [Test]
        public void ComplaintDbContextContainsDbSetCustomerServiceRepresentativesProperty()
        {
            // var context = new ApplicationDbContext();
            using (var dbContext = new ComplaintDbContext(_dbContextOptions))
            {
                var propertyInfo = dbContext.GetType().GetProperty("CustomerServiceRepresentatives");

                Assert.IsNotNull(propertyInfo);
                Assert.AreEqual(typeof(DbSet<CustomerServiceRepresentative>), propertyInfo.PropertyType);
            }
        }

        [Test]
        public void ComplaintDbContextContainsDbSetComplaintsProperty()
        {
            // var context = new ApplicationDbContext();
            using (var dbContext = new ComplaintDbContext(_dbContextOptions))
            {
                var propertyInfo = dbContext.GetType().GetProperty("Complaints");
                Assert.IsNotNull(propertyInfo);
                Assert.AreEqual(typeof(DbSet<Complaint>), propertyInfo.PropertyType);
            }
        }


        [Test]
        public void CustomerServiceRepresentative_Properties_RepresentativeID_ReturnExpectedDataTypes_int()
        {
            string assemblyName = "ComplaintManagementSystem";
            string typeName = "ComplaintManagementSystem.Models.CustomerServiceRepresentative";
            Assembly assembly = Assembly.Load(assemblyName);
            Type commuterType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = commuterType.GetProperty("RepresentativeID");
            Assert.IsNotNull(propertyInfo, "The property 'SpaceID' was not found on the CustomerServiceRepresentative class.");
            Type propertyType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(int), propertyType, "The data type of 'SpaceID' property is not as expected (int).");
        }

        [Test]
        public void CustomerServiceRepresentatives_Properties_Name_ReturnExpectedDataTypes_string()
        {
            string assemblyName = "ComplaintManagementSystem";
            string typeName = "ComplaintManagementSystem.Models.CustomerServiceRepresentative";
            Assembly assembly = Assembly.Load(assemblyName);
            Type commuterType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = commuterType.GetProperty("FirstName");
            Assert.IsNotNull(propertyInfo, "The property 'Name' was not found on the CustomerServiceRepresentative class.");
            Type propertyType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(string), propertyType, "The data type of 'Name' property is not as expected (string).");
        }
        [Test]
        public void CustomerServiceRepresentatives_Properties_Email_ReturnExpectedDataTypes_string()
        {
            string assemblyName = "ComplaintManagementSystem";
            string typeName = "ComplaintManagementSystem.Models.CustomerServiceRepresentative";
            Assembly assembly = Assembly.Load(assemblyName);
            Type commuterType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = commuterType.GetProperty("Email");
            Assert.IsNotNull(propertyInfo, "The property 'Capacity' was not found on the CustomerServiceRepresentatives class.");
            Type propertyType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(string), propertyType, "The data type of 'Capacity' property is not as expected (int).");
        }
        [Test]
        public void CustomerServiceRepresentatives_Properties_LastName_ReturnExpectedDataTypes_Bool()
        {
            string assemblyName = "ComplaintManagementSystem";
            string typeName = "ComplaintManagementSystem.Models.CustomerServiceRepresentative";
            Assembly assembly = Assembly.Load(assemblyName);
            Type commuterType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = commuterType.GetProperty("LastName");
            Assert.IsNotNull(propertyInfo, "The property 'Availability' was not found on the CustomerServiceRepresentative class.");
            Type propertyType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(string), propertyType, "The data type of 'Availability' property is not as expected (Student).");
        }
        [Test]
        public void Complaints_Properties_ComplaintID_ReturnExpectedDataTypes_int()
        {
            string assemblyName = "ComplaintManagementSystem";
            string typeName = "ComplaintManagementSystem.Models.Complaint";
            Assembly assembly = Assembly.Load(assemblyName);
            Type commuterType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = commuterType.GetProperty("ComplaintID");
            Assert.IsNotNull(propertyInfo, "The property 'Availability' was not found on the CustomerServiceRepresentative class.");
            Type propertyType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(int), propertyType, "The data type of 'Complaints' property is as expected (ICollection_Complaint).");
        }

        [Test]
        public void Complaint_Properties_CustomerName_ReturnExpectedDataTypes_string()
        {
            string assemblyName = "ComplaintManagementSystem";
            string typeName = "ComplaintManagementSystem.Models.Complaint";
            Assembly assembly = Assembly.Load(assemblyName);
            Type commuterType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = commuterType.GetProperty("CustomerName");
            Assert.IsNotNull(propertyInfo, "The property 'ComplaintID' was not found on the Commuter class.");
            Type propertyType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(string), propertyType, "The data type of 'ComplaintID' property is not as expected (int).");
        }
        [Test]
        public void Complaint_Properties_ContactNumber_ReturnExpectedDataTypes_int()
        {
            string assemblyName = "ComplaintManagementSystem";
            string typeName = "ComplaintManagementSystem.Models.Complaint";
            Assembly assembly = Assembly.Load(assemblyName);
            Type commuterType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = commuterType.GetProperty("ContactNumber");
            Assert.IsNotNull(propertyInfo, "The property 'Name' was not found on the Commuter class.");
            Type propertyType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(long), propertyType, "The data type of 'Name' property is not as expected (string).");
        }
        [Test]
        public void Complaint_Properties_AccountNumber_ReturnExpectedDataTypes_int()
        {
            string assemblyName = "ComplaintManagementSystem";
            string typeName = "ComplaintManagementSystem.Models.Complaint";
            Assembly assembly = Assembly.Load(assemblyName);
            Type commuterType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = commuterType.GetProperty("AccountNumber");
            Assert.IsNotNull(propertyInfo, "The property 'AccountNumber' was not found on the Commuter class.");
            Type propertyType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(string), propertyType, "The data type of 'AccountNumber' property is not as expected (AccountNumber).");
        }



        [Test]
        public void Test_SubmitViewFile_Exists_complaint()
        {
            string indexPath = Path.Combine(@"/home/coder/project/workspace/ComplaintManagementSystem/Views/Complain/", "Submit.cshtml");
            bool indexViewExists = File.Exists(indexPath);

            Assert.IsTrue(indexViewExists, "Submit.cshtml view file does not exist.");
        }
        [Test]
        public void Test_DashboardViewFile_Exists_Complaint()
        {
            string indexPath = Path.Combine(@"/home/coder/project/workspace/ComplaintManagementSystem/Views/Complaint/", "Dashboard.cshtml");
            bool indexViewExists = File.Exists(indexPath);

            Assert.IsTrue(indexViewExists, "Dashboard.cshtml view file does not exist.");
        }

        public CustomerServiceRepresentative CreateRepFromDictionary(Dictionary<string, object> data)
        {
            var team = new CustomerServiceRepresentative();
            foreach (var kvp in data)
            {
                var propertyInfo = typeof(CustomerServiceRepresentative).GetProperty(kvp.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(team, kvp.Value);
                }
            }
            return team;
        }

        public Complaint CreateTeamFromDictionary(Dictionary<string, object> data)
        {
            var team = new Complaint();
            foreach (var kvp in data)
            {
                var propertyInfo = typeof(Complaint).GetProperty(kvp.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(team, kvp.Value);
                }
            }
            return team;
        }
    }

}