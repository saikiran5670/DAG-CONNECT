using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.notificationengine.entity;
using net.atos.daf.ct2.pushnotificationservice;

namespace net.atos.daf.ct2.notificationservice.Entity
{
    public class Mapper
    {
        public AlertMessageAndAccountClientEntity GetAlertMessageAndAccountEntity(AccountClientMapping accountClientMapping)
        {
            AlertMessageAndAccountClientEntity alertMessageAndAccountClientEntity = new AlertMessageAndAccountClientEntity();
            foreach (var item in accountClientMapping.AccountClient)
            {
                AccountClientEntity accountClientEntity = new AccountClientEntity();
                accountClientEntity.AccountId = item.AccountId;
                accountClientEntity.OrganizationId = item.OrgId;
                accountClientEntity.HubClientId = item.HubClientId;
                alertMessageAndAccountClientEntity.AccountClientEntity.Add(accountClientEntity);
            }

            alertMessageAndAccountClientEntity.TripAlert.Alertid = accountClientMapping.AlertMessageData.Alertid;
            alertMessageAndAccountClientEntity.TripAlert.UrgencyLevelType = accountClientMapping.AlertMessageData.UrgencyLevelType;
            alertMessageAndAccountClientEntity.TripAlert.CategoryType = accountClientMapping.AlertMessageData.CategoryType;
            alertMessageAndAccountClientEntity.TripAlert.Type = accountClientMapping.AlertMessageData.Type;
            alertMessageAndAccountClientEntity.TripAlert.ValueAtAlertTime = accountClientMapping.AlertMessageData.ValueAtAlertTime;
            alertMessageAndAccountClientEntity.TripAlert.Vin = accountClientMapping.AlertMessageData.Vin;

            return alertMessageAndAccountClientEntity;
        }

        public AccountClientMapping GetAlertMessageAndAccountEntity(AlertMessageAndAccountClientEntity alertMessageAndAccountClientEntity)
        {
            AccountClientMapping alertMessageAndAccount = new AccountClientMapping();
            foreach (var item in alertMessageAndAccountClientEntity.AccountClientEntity)
            {
                AccountClientEntity accountClientEntity = new AccountClientEntity();
                accountClientEntity.AccountId = item.AccountId;
                accountClientEntity.OrganizationId = item.OrganizationId;
                accountClientEntity.HubClientId = item.HubClientId;
                alertMessageAndAccountClientEntity.AccountClientEntity.Add(accountClientEntity);
            }

            alertMessageAndAccount.AlertMessageData.Alertid = alertMessageAndAccountClientEntity.TripAlert.Alertid;
            alertMessageAndAccount.AlertMessageData.UrgencyLevelType = alertMessageAndAccountClientEntity.TripAlert.UrgencyLevelType;
            alertMessageAndAccount.AlertMessageData.CategoryType = alertMessageAndAccountClientEntity.TripAlert.CategoryType;
            alertMessageAndAccount.AlertMessageData.Type = alertMessageAndAccountClientEntity.TripAlert.Type;
            alertMessageAndAccount.AlertMessageData.ValueAtAlertTime = alertMessageAndAccountClientEntity.TripAlert.ValueAtAlertTime;
            alertMessageAndAccountClientEntity.TripAlert.Vin = alertMessageAndAccount.AlertMessageData.Vin;

            return alertMessageAndAccount;
        }

        public AlertMessageData GetAlertMessageData(TripAlert tripAlert)
        {
            AlertMessageData alertMessageData = new AlertMessageData();
            alertMessageData.Alertid = tripAlert.Alertid;
            alertMessageData.UrgencyLevelType = tripAlert.UrgencyLevelType;
            alertMessageData.UrgencyLevelType = tripAlert.UrgencyLevelType;
            alertMessageData.Type = tripAlert.Type;
            return alertMessageData;
        }
    }
}
