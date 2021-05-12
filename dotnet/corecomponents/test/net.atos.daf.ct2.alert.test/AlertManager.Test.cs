using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.alert.entity;
using net.atos.daf.ct2.alert.ENUM;
using net.atos.daf.ct2.alert.repository;
using net.atos.daf.ct2.data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.alert.test
{
    [TestClass]
    public class AlertManagerTest
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _dataAccess;
        private readonly AlertRepository _alertRepository;
        private readonly IAlertManager _ialertManager;
        public AlertManagerTest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                                      .Build();
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _alertRepository = new AlertRepository(_dataAccess);
            _ialertManager = new AlertManager(_alertRepository);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Activate Alert Success")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void ActivateAlertSuccess()
        {
            //Provide the Alert Id which has suspended State in Database
            var ExcepteId = 1;
            var Id = _ialertManager.ActivateAlert(ExcepteId, ((char)AlertState.Active), ((char)AlertState.Suspend)).Result;
            Assert.AreEqual(ExcepteId, Id);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Suspend Alert Success")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void SuspendAlertSuccess()
        {
            //Provide the Alert Id which has Active State
            var ExcepteId = 1;
            var Id = _ialertManager.SuspendAlert(ExcepteId, ((char)AlertState.Suspend), ((char)AlertState.Active)).Result;
            Assert.AreEqual(ExcepteId, Id);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Delete Alert Success, Provide the Alert Id which has a active notification")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void DeleteAlertSucess()
        {
            //Provide the Alert Id which has a active notification
            var ExcepteId = 8;
            int Id = 0;
            if (_ialertManager.CheckIsNotificationExitForAlert(ExcepteId).Result)
                Id = _ialertManager.DeleteAlert(ExcepteId, ((char)AlertState.Suspend)).Result;
            Assert.AreEqual(ExcepteId, Id);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Delete Alert Falied, Provide the Alert Id which has a no active notification")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void DeleteAlertFailure()
        {
            //Provide the Alert Id which has a no active notification
            var ExcepteId = 0;
            int Id = 0;
            if (_ialertManager.CheckIsNotificationExitForAlert(2).Result)
                Id = _ialertManager.DeleteAlert(2, ((char)AlertState.Suspend)).Result;
            Assert.AreEqual(ExcepteId, Id);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for create Alert")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void CreateAlertTest()
        {
            #region Test Data
            Alert alert = new Alert
            {
                //Id =,
                OrganizationId = 10,
                Name = "TestAlert1",
                Category = "L",
                Type = "N",
                ValidityPeriodType = "A",
                ValidityStartDate = 1620272821,
                ValidityEndDate = 1620272821,
                VehicleGroupId = 12,
                State = "A",
                CreatedAt = 1620272821,
                CreatedBy = 10,
                //ModifiedAt =,
                //ModifiedBy =,
                AlertUrgencyLevelRefs = new List<AlertUrgencyLevelRef>(),
                Notifications = new List<Notification>(),
                AlertLandmarkRefs = new List<AlertLandmarkRef>(),
            };
            Notification notification = new Notification
            {
                //Id =,
                //AlertId =,
                AlertUrgencyLevelType = "C",
                FrequencyType = "O",
                FrequencyThreshholdValue = 1,
                ValidityType = "A",
                State = "A",
                CreatedAt = 1620272821,
                CreatedBy = 10,
                //ModifiedAt =,
                //ModifiedBy =,
                NotificationRecipients = new List<NotificationRecipient>(),
                NotificationLimits = new List<NotificationLimit>(),
                NotificationAvailabilityPeriods = new List<NotificationAvailabilityPeriod>(),
            };
            NotificationRecipient notificationRecipient = new NotificationRecipient
            {
                //Id =,
                //NotificationId =,
                RecipientLabel = "Test Manager",
                AccountGroupId = 12,
                NotificationModeType = "E",
                PhoneNo = string.Empty,
                Sms = string.Empty,
                EmailId = "testmanager@atos.net",
                EmailSub = "Email Notification ",
                EmailText = "Hello Text",
                WsUrl = string.Empty,
                WsType = "X",
                WsText = string.Empty,
                WsLogin = string.Empty,
                WsPassword = string.Empty,
                State = "A",
                CreatedAt = 1620272821,
                //ModifiedAt =,
            };
            NotificationLimit notificationLimit = new NotificationLimit
            {
                //Id =,
                //NotificationId =,
                NotificationModeType = "E",
                MaxLimit = 100,
                NotificationPeriodType = "D",
                PeriodLimit = 50,
                State = "A",
                CreatedAt = 1620272821,
                //ModifiedAt =,
            };
            NotificationAvailabilityPeriod notificationAvailabilityPeriod = new NotificationAvailabilityPeriod
            {
                //Id =,
                //NotificationId =,
                AvailabilityPeriodType = "A",
                PeriodType = "D",
                StartTime = 1620272821,
                EndTime = 1620272821,
                State = "A",
                CreatedAt = 1620272821,
                //ModifiedAt =,
            };
            AlertUrgencyLevelRef alertUrgencyLevelRef = new AlertUrgencyLevelRef
            {
                //Id =,
                //AlertId =,
                UrgencyLevelType = "C",
                ThresholdValue = 2,
                UnitType = "C",
                DayType = new bool[7] { true, true, true, true, true, true, true },
                PeriodType = "D",
                UrgencylevelStartDate = 1620272821,
                UrgencylevelEndDate = 1620272821,
                State = "A",
                CreatedAt = 1620272821,
                //ModifiedAt =,
                AlertFilterRefs = new List<AlertFilterRef>()

            };
            AlertLandmarkRef alertLandmarkRef = new AlertLandmarkRef
            {
                //Id =,
                //AlertId =,
                LandmarkType = "P",
                RefId = 170,
                Distance = 20,
                UnitType = "X",
                State = "A",
                CreatedAt = 1620272821,
                //ModifiedAt =,
            };
            AlertFilterRef alertFilterRef = new AlertFilterRef
            {
                //Id =,
                //AlertId =,
                //AlertUrgencyLevelId =,
                FilterType = "O",
                ThresholdValue = 20,
                UnitType = "X",
                LandmarkType = "P",
                RefId = 170,
                PositionType = "X",
                DayType = new bool[7] { true, true, true, true, true, true, true },
                PeriodType = "D",
                FilterStartDate = 1620272821,
                FilterEndDate = 1620272821,
                State = "A",
                CreatedAt = 1620272821,
                //ModifiedAt =,
            };
            #endregion

            notification.NotificationAvailabilityPeriods.Add(notificationAvailabilityPeriod);
            notification.NotificationLimits.Add(notificationLimit);
            notification.NotificationRecipients.Add(notificationRecipient);

            alertUrgencyLevelRef.AlertFilterRefs.Add(alertFilterRef);

            alert.AlertLandmarkRefs.Add(alertLandmarkRef);
            alert.AlertUrgencyLevelRefs.Add(alertUrgencyLevelRef);
            alert.Notifications.Add(notification);
            var result = _ialertManager.CreateAlert(alert).Result;
            Assert.IsTrue(result.Id > 0);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get Alert List")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void GetAlertListTest()
        {
            Alert alert = new Alert
            {
                //Id =,
                OrganizationId = 10,
                Name = "TestAlert1",
                Category = "L",
                Type = "N",
                ValidityPeriodType = "A",
                ValidityStartDate = 1620272821,
                ValidityEndDate = 1620272821,
                VehicleGroupId = 12,
                State = "A",
                CreatedAt = 1620272821,
                CreatedBy = 10,
                //ModifiedAt =,
                //ModifiedBy =,
                AlertUrgencyLevelRefs = new List<AlertUrgencyLevelRef>(),
                Notifications = new List<Notification>(),
                AlertLandmarkRefs = new List<AlertLandmarkRef>(),
            };
            var result = _ialertManager.GetAlertList(alert.CreatedBy,alert.OrganizationId).Result;
            Assert.IsNotNull(result);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get Alert Category")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void UnT_Alert_GetAlertCategoryTest()
        {
            var result = _ialertManager.GetAlertCategory();
            Assert.IsNotNull(result);
            Assert.IsTrue(result != null);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for update Alert")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void UpdateAlertTest()
        {
            #region Test Data
            Alert alert = new Alert
            {
                Id = 14,
                OrganizationId = 10,
                Name = "TestAlertUpdate1",
                Category = "L",
                Type = "N",
                ValidityPeriodType = "A",
                ValidityStartDate = 1620272821,
                ValidityEndDate = 1620272821,
                VehicleGroupId = 12,
                State = "A",
                //CreatedAt = 1620272821,
                //CreatedBy = 10,
                ModifiedAt = 1620272821,
                ModifiedBy = 5,
                AlertUrgencyLevelRefs = new List<AlertUrgencyLevelRef>(),
                Notifications = new List<Notification>(),
                AlertLandmarkRefs = new List<AlertLandmarkRef>(),
            };
            Notification notification = new Notification
            {
                Id = 10,
                AlertId = 14,
                AlertUrgencyLevelType = "W",
                FrequencyType = "O",
                FrequencyThreshholdValue = 5,
                ValidityType = "A",
                State = "A",
                //CreatedAt = 1620272821,
                //CreatedBy = 10,
                ModifiedAt = 1620272821,
                ModifiedBy = 10,
                NotificationRecipients = new List<NotificationRecipient>(),
                NotificationLimits = new List<NotificationLimit>(),
                NotificationAvailabilityPeriods = new List<NotificationAvailabilityPeriod>(),
            };
            NotificationRecipient notificationRecipient = new NotificationRecipient
            {
                Id = 3,
                NotificationId = 10,
                RecipientLabel = "Test notification 01",
                AccountGroupId = 12,
                NotificationModeType = "E",
                PhoneNo = string.Empty,
                Sms = string.Empty,
                EmailId = "testmanage01r@atos.net",
                EmailSub = "Email Notification ",
                EmailText = "Hello Text update",
                WsUrl = string.Empty,
                WsType = "X",
                WsText = string.Empty,
                WsLogin = string.Empty,
                WsPassword = string.Empty,
                State = "A",
                //CreatedAt = 1620272821,
                ModifiedAt = 1620272821
            };
            NotificationLimit notificationLimit = new NotificationLimit
            {
                Id = 6,
                NotificationId = 10,
                NotificationModeType = "E",
                MaxLimit = 100,
                NotificationPeriodType = "D",
                PeriodLimit = 50,
                State = "A",
                //CreatedAt = 1620272821,
                ModifiedAt = 1620272821
            };
            NotificationAvailabilityPeriod notificationAvailabilityPeriod = new NotificationAvailabilityPeriod
            {
                Id = 6,
                NotificationId = 10,
                AvailabilityPeriodType = "A",
                PeriodType = "D",
                StartTime = 1620272821,
                EndTime = 1620272821,
                State = "A",
                //CreatedAt = 1620272821,
                ModifiedAt = 1620272821
            };
            AlertUrgencyLevelRef alertUrgencyLevelRef = new AlertUrgencyLevelRef
            {
                Id = 14,
                AlertId = 14,
                UrgencyLevelType = "C",
                ThresholdValue = 2,
                UnitType = "C",
                DayType = new bool[7] { true, true, true, true, true, true, true },
                PeriodType = "D",
                UrgencylevelStartDate = 1620272821,
                UrgencylevelEndDate = 1620272821,
                State = "A",
                //CreatedAt = 1620272821,
                ModifiedAt = 1620272821,
                AlertFilterRefs = new List<AlertFilterRef>()

            };
            AlertLandmarkRef alertLandmarkRef = new AlertLandmarkRef
            {
                Id = 13,
                AlertId = 14,
                LandmarkType = "P",
                RefId = 170,
                Distance = 20,
                UnitType = "X",
                State = "A",
                //CreatedAt = 1620272821,
                ModifiedAt = 1620272821
            };
            AlertFilterRef alertFilterRef = new AlertFilterRef
            {
                Id = 10,
                AlertId = 14,
                //AlertUrgencyLevelId =,
                FilterType = "O",
                ThresholdValue = 50,
                UnitType = "M",
                LandmarkType = "G",
                RefId = 171,
                PositionType = "E",
                DayType = new bool[7] { true, true, true, true, true, true, true },
                PeriodType = "D",
                FilterStartDate = 1620272821,
                FilterEndDate = 1620272821,
                State = "A",
                //CreatedAt = 1620272821,
                ModifiedAt = 1620272821
            };
            #endregion

            notification.NotificationAvailabilityPeriods.Add(notificationAvailabilityPeriod);
            notification.NotificationLimits.Add(notificationLimit);
            notification.NotificationRecipients.Add(notificationRecipient);

            alertUrgencyLevelRef.AlertFilterRefs.Add(alertFilterRef);

            alert.AlertLandmarkRefs.Add(alertLandmarkRef);
            alert.AlertUrgencyLevelRefs.Add(alertUrgencyLevelRef);
            alert.Notifications.Add(notification);
            var result = _ialertManager.UpdateAlert(alert).Result;
            Assert.IsTrue(result.Id > 0);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Duplicate Alert Type")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task DuplicateAlertType()
        {
            var alert = await _ialertManager.DuplicateAlertType(1);
            Assert.IsTrue(alert.Id > 0);
        }
    }
}
