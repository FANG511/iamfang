using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models.PerosonLocation.ViewModels
{
    /// <summary>
    /// 警報設定
    /// </summary>
    public class AlarmSetting
    {
        public string AlarmID { get; set; }
        public string AlarmName { get; set; }
        public string AlarmField { get; set; }
        public string AlarmRole { get; set; }
        public string AlarmValue { get; set; }
        public string NotificationGroup { get; set; }
        public string Enable { get; set; }
    }

    /// <summary>
    /// 警報紀錄
    /// </summary>
    public class AlarmRecord
    {
        public int ID { get; set; }
        public string DeviceID { get; set; }
        public string PersonID { get; set; }
        public string AlarmID { get; set; }
        public string AlarmValue { get; set; }
        public string CreateTime { get; set; }
    }

    public class QryAlarm
    {
        public string AlarmID { get; set; }
        public string DeviceID { get; set; }
        public string PersonID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Enable { get; set; }
    }

    public class UpdAlarm
    {
        public string AlarmID { get; set; }
        public string DeviceID { get; set; }
        public string PersonID { get; set; }
        public string EditTime { get; set; }
    }

    public class AlarmCheck
    {
        public string AlarmID { get; set; }
        public string TableName { get; set; }
        public string Field { get; set; }
        public string Type { get; set; }
        public string AlarmValue { get; set; }
    }

    public class AlarmMsg
    {
        public string DeviceID { get; set; }
        public string PersonID { get; set; }
        public string AlarmID { get; set; }
        public string Description { get; set; }
        public string IsDupAlarmOn { get; set; }
        public string EventTime { get; set; }
    }
}