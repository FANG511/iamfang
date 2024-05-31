using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

/// <summary>
/// 設備資料查詢，合併身體監測裝置及定位裝置
/// 設備綁定，綁定人員
/// </summary>
namespace FTC_MES_MVC.Models.PerosonLocation.ViewModels
{
	/// <summary>
	/// 設備基本資訊
	/// </summary>
	public class DeviceInfo
	{
		public string DeviceID { get; set; }
		public string DeviceName { get; set; }
		public string UUID { get; set; }
		public string BindPerson {get; set;}
		public string Enable { get; set; }
	}
	
	/// <summary>
	/// 設備
	/// </summary>
	public class Device
    {
		public string DeviceID { get; set; }
		public string DeviceName { get; set; }
		public string Status { get; set; }
		public string PersonID { get; set; }
		public string GPSLocation { get; set; }
		public float Altitude { get; set; }
		public float HeartRate { get; set; }
		public float BloodPressure { get; set; }
		public float Temperature { get; set; }
		public string SignalStrength { get; set; }
		public string CreateTime { get; set; }
		public string BatteryInfo { get; set; }
		public string AlarmCheckFlag { get; set; }
	}

	/// <summary>
	/// 上傳的設備數值
	/// </summary>
	public class UpdDeviceValue
	{
		public string UUID { get; set; }
		public string GPSLocation { get; set; }
		public float Altitude { get; set; }
		public float HeartRate { get; set; }
		public float BloodPressure { get; set; }
		public float Temperature { get; set; }
		public string SignalStrength { get; set; }
		public string CreateTime { get; set; }
		public string BatteryInfo { get; set; }
	}

	/// <summary>
	/// 查詢設備
	/// </summary>
	public class QryDevice
	{
		public string DeviceID { get; set;}
		public string DeviceName { get; set; }
		public string Status { get; set; }
		public string PersonID{ get; set; }
	}

	/// <summary>
	/// 查詢設備歷史值
	/// </summary>
	public class QryDeviceHis
	{
		public string DeviceID { get; set; }
		public string DeviceName { get; set; }
		public string Status { get; set; }
		public string PersonID { get; set; }
		public string StartTime { get; set; }
		public string EndTime { get; set; }
	}

	/// <summary>
	/// 更新裝置
	/// </summary>
	public class UpdateDevice
	{
		public List<Device> Devices { get; set; }
	}

	/// <summary>
	/// 綁定設備
	/// </summary>
	public class BindDevcie
	{
		public int ID { get; set; }
		public string DeviceID { get; set; }
		public string PersonID { get; set; }
		public string Status { get; set; }
		public string CreateTime { get; set; }
		public string EndTime { get; set; }
	}

	public class BindDevciePerson
	{
		public string DeviceID { get; set; }
		public string DeviceName { get; set; }
		public string PersonID { get; set; }
		public string PersonName { get; set; }
	}

	/// <summary>
	/// 查詢綁定設備
	/// </summary>
	public class QryBindDevice
	{
		public string DeviceID { get; set; }
		public string PersonID { get; set; }
		public string Status { get; set; }
		public string StartTime { get; set; }
		public string EndTime { get; set; }
	}
}