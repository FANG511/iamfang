using Dapper;
using FTC_MES_MVC.Models;
using FTC_MES_MVC.Models.PerosonLocation.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using FTC_MES_MVC.Models.DAL.PersonLocaion;
using FTC_MES_MVC.Models.PerosonLocation;
using FTC_MES_MVC.Models.ViewModels;


namespace FTC_MES_MVC.Controllers
{
    public class apiPersonLocationController : BaseApiController
    {
        dalPersonLocation oDal = new dalPersonLocation();
        protected override void Dispose(bool disposing)
        {
            if (oDal != null)
            {
                oDal.Dispose();
            }
        }

        // 接下來進行各種api

        /// <summary>
        /// 取得公司資料
        /// </summary>
        [HttpPost]
        public List<Models.PerosonLocation.ViewModels.Company> GetCompany()
        {
            List<Models.PerosonLocation.ViewModels.Company> mCompanyList = new List<Models.PerosonLocation.ViewModels.Company>();
            try
            {
                mCompanyList = oDal.QryCompany();
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetCompany Error=" + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mCompanyList;
        }

        /// <summary>
        /// 取得工廠資料
        /// </summary>
        [HttpPost]
        public List<Models.PerosonLocation.ViewModels.Factory> GetFactory(QueryPerson p_oQuery)
        {
            List<Models.PerosonLocation.ViewModels.Factory> mFactoryList = new List<Models.PerosonLocation.ViewModels.Factory>();
            try
            {
                mFactoryList = oDal.QryFactory(p_oQuery);
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetFactory Error=" + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mFactoryList;
        }

        /// <summary>
        /// 取得人員資料
        /// </summary>
        [HttpPost]
        
        public List<PersonData> GetPersonData(QueryPerson p_oQuery)
        {
            List<PersonData> mPersonData = new List<PersonData>();
            try
            {
                mPersonData = oDal.QryPersonData(p_oQuery);
            }
            catch (Exception ex)
            {
                //將異常訊息記錄到日誌中
                WriteLog_Error("GetPersonData Error=" + ex.ToString());
                //other?
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mPersonData;
        }

        /// <summary>
        /// 新增人員資料
        /// </summary>
        [HttpPost]
        public ApiReturnMessage AddPersonData(PersonData p_oUpdate)
        {
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            try
            {
                oApiReturnMessage = oDal.AddPersonData(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("AddPersonData Error" + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 更新人員資料
        /// </summary>
        [HttpPost]
        public ApiReturnMessage UpdatePersonData(PersonData p_oUpdate)
        {
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            try
            {
                oApiReturnMessage = oDal.UpdPersonData(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("UpdatePersonData Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 刪除人員資料
        /// </summary>
        [HttpPost]
        public ApiReturnMessage DeletePersonData(PersonData p_oUpdate)
        {
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            try
            {
                oApiReturnMessage = oDal.DelPersonData(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("DeletePersonData Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }


        /// <summary>
        /// 取得設備基本資訊
        /// </summary>
        [HttpPost]
        public List<DeviceInfo> GetDeviceInfo(DeviceInfo p_oQuery)
        {
            List<DeviceInfo> mDeviceInfoList = new List<DeviceInfo>();
            try
            {
                mDeviceInfoList = oDal.QryDeviceInfo(p_oQuery);
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetDeviceInfo Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mDeviceInfoList;
        }


        /// <summary>
        /// 新增設備基礎資料
        /// </summary>
        [HttpPost]
        public ApiReturnMessage AddDeviceInfo(DeviceInfo p_oUpdate)
        {
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            try
            {
                p_oUpdate.Enable = "N";
                oApiReturnMessage = oDal.AddDeviceInfo(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("AddDeviceInfo Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 更新設備基礎資料
        /// </summary>
        [HttpPost]
        public ApiReturnMessage UpdateDeviceInfo(DeviceInfo p_oUpdate)
        {
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            try
            {
                oApiReturnMessage = oDal.UpdateDeviceInfo(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("UpdateDeviceInfo Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 刪除設備基礎資料
        /// </summary>
        [HttpPost]
        public ApiReturnMessage DeleteDeviceInfo(DeviceInfo p_oUpdate)
        {
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            try
            {
                oApiReturnMessage = oDal.DeleteDeviceInfo(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("DeleteDeviceInfo Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 取得設備即時
        /// </summary>
        [HttpPost]
        public List<Device> GetDeviceCurrent(QryDevice p_oQuery)
        {
            List<Device> mDeviceList = new List<Device>();
            try
            {
                mDeviceList = oDal.QryDeviceCurrent(p_oQuery);
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetDeviceCurrent Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mDeviceList;
        }

        /// <summary>
        /// 新增設備數值，更新在即時值，並新增到歷史值
        /// </summary>
        [HttpPost]
        public ApiReturnMessage AddDeviceValue(UpdDeviceValue p_oUpdate)
        {
            ApiReturnMessage mApiReturnMessage = new ApiReturnMessage();
            try
            {
                oApiReturnMessage = oDal.AddDeviceValue(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("AddDeviceValue Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mApiReturnMessage;
        }

        /// <summary>
        /// 更新設備即時數值，更新部分同步更新歷史值
        /// </summary>
        [HttpPost]
        public ApiReturnMessage UpdateDeviceValue(Device p_oUpdate)
        {
            ApiReturnMessage mApiReturnMessage = new ApiReturnMessage();
            try
            {
                oApiReturnMessage = oDal.UpdateDeviceValue(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("updateDeviceValue Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mApiReturnMessage;
        }


        /// <summary>
        /// 刪除設備即時值
        /// </summary>
        [HttpPost]
        public ApiReturnMessage DeleteDeviceValue(Device p_oUpdate)
        {
            ApiReturnMessage mApiReturnMessage = new ApiReturnMessage();
            try
            {
                oApiReturnMessage = oDal.DeleteDeviceValue(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("DeleteDeviceValue Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mApiReturnMessage;
        }

        /// <summary>
        /// 取得設備歷史值
        /// </summary>
        [HttpPost]
        public List<Device> GetDeviceHistory(QryDeviceHis p_oQuery)
        {
            List<Device> mDeviceList = new List<Device>();
            try
            {
                // 待處理 => 要確定查詢的時間是否合法
                mDeviceList = oDal.QryDeviceHistory(p_oQuery);                
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetDeviceHistory Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mDeviceList;
        }

        /// <summary>
        /// 取得警報紀錄
        /// </summary>
        [HttpPost]
        public List<AlarmRecord> GetAlarmRecord(QryAlarm p_oQuery)
        {
            List<AlarmRecord> mAlarmRecordList = new List<AlarmRecord>();
            try
            {
                mAlarmRecordList = oDal.QryAlarmRecord(p_oQuery);
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetAlarmRecord Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mAlarmRecordList;
        }

        /// <summary>
        /// 修改警報紀錄
        /// </summary>
        [HttpPost]
        public ApiReturnMessage UpdateAlarmRecord(AlarmRecord p_oUpdate)
        {
            ApiReturnMessage mApiReturnMessage = new ApiReturnMessage();
            try
            {
                mApiReturnMessage = oDal.UpdateAlarmRecord(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("UpdateAlarmRecord Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mApiReturnMessage;
        }

        /// <summary>
        /// 刪除警報紀錄
        /// </summary>
        [HttpPost]
        public ApiReturnMessage DeleteAlarmRecord(AlarmRecord p_oUpdate)
        {
            ApiReturnMessage mApiReturnMessage = new ApiReturnMessage();
            try
            {
                mApiReturnMessage = oDal.DeleteAlarmRecord(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("DeleteAlarmRecord Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mApiReturnMessage;
        }


        /// <summary>
        /// 取得警報設定
        /// </summary>
        [HttpPost]
        public List<AlarmSetting> GetAlarmSetting(QryAlarm p_oQuery)
        {
            List<AlarmSetting> mAlarmSettingList = new List<AlarmSetting>();
            try
            {
                mAlarmSettingList = oDal.QryAlarmSetting(p_oQuery);
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetAlarmSetting Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mAlarmSettingList;
        }

        /// <summary>
        /// 新增警報設定
        /// </summary>
        [HttpPost]
        public ApiReturnMessage AddAlarmSetting(AlarmSetting p_oUpdate)
        {
            ApiReturnMessage mApiReturnMessage = new ApiReturnMessage();
            try
            {
                mApiReturnMessage = oDal.AddAlarmSetting(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("AddAlarmSetting Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mApiReturnMessage;
        }

        /// <summary>
        /// 修改警報設定
        /// </summary>
        [HttpPost]
        public ApiReturnMessage UpdateAlarmSetting(AlarmSetting p_oUpdate)
        {
            ApiReturnMessage mApiReturnMessage = new ApiReturnMessage();
            try
            {
                mApiReturnMessage = oDal.UpdAlarmSetting(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("UpdateAlarmSetting Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mApiReturnMessage;
        }

        /// <summary>
        /// 刪除警報設定
        /// </summary>
        [HttpPost]
        public ApiReturnMessage DeleteAlarmSetting(AlarmSetting p_oUpdate)
        {
            ApiReturnMessage mApiReturnMessage = new ApiReturnMessage();
            try
            {
                mApiReturnMessage = oDal.DelAlarmSetting(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("DeleteAlarmSetting Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mApiReturnMessage;
        }

        /// <summary>
        /// 取得綁定中裝置
        /// </summary>
        [HttpPost]
        public List<BindDevcie> GetBindDevice(QryBindDevice p_oQuery)
        {
            List<BindDevcie> mBindDevcieList = new List<BindDevcie>();
            try
            {
                mBindDevcieList = oDal.QryBindDeviceNow(p_oQuery);
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetBindDevice Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mBindDevcieList;
        }

        /// <summary>
        /// 取得設備綁定人員清單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<BindDevciePerson> GetBindPerson()
        {
            List<BindDevciePerson> mBindDevcieList = new List<BindDevciePerson>();
            try
            {
                mBindDevcieList = oDal.QryBindDevicePerson();
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetBindPerson Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mBindDevcieList;
        }


        [HttpPost]
        public List<PersonData> GetBindAvailable(PersonData p_oQuery)
        {
            List<PersonData> mPersonList = new List<PersonData>();
            try
            {
                mPersonList = oDal.QryBindAvailable(p_oQuery);
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetBindAvailable Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mPersonList;

        }



        /// <summary>
        /// 綁定裝置
        /// </summary>
        [HttpPost]
        public ApiReturnMessage BindDevice(BindDevcie p_oUpdate)
        {
            ApiReturnMessage mApiReturnMessage = new ApiReturnMessage();
            try
            {
                mApiReturnMessage = oDal.BindDevice(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("BindDevice Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mApiReturnMessage;
        }

        /// <summary>
        /// 解除綁定設備
        /// </summary>
        [HttpPost]
        public ApiReturnMessage UnBindDevice(BindDevcie p_oUpdate)
        {
            ApiReturnMessage mApiReturnMessage = new ApiReturnMessage();
            try
            {
                mApiReturnMessage = oDal.UnBindDevice(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("UnBindDevice Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mApiReturnMessage;
        }

        /// <summary>
        /// 取得歷史綁定裝置
        /// </summary>
        [HttpPost]
        public List<BindDevcie> GetBindDeviceHis(QryBindDevice p_oQuery)
        {
            List<BindDevcie> mBindDevcieList = new List<BindDevcie>();
            try
            {
                mBindDevcieList = oDal.QryBindDevice(p_oQuery);
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetBindDeviceHis Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mBindDevcieList;
        }

        /// <summary>
        /// 取得圖徵
        /// </summary>
        [HttpPost]
        public List<WKTResult> GetPolygon(Property p_oQuery)
        {
            List<WKTResult> mAreaList = new List<WKTResult>();
            try
            {
                mAreaList = oDal.QryPolygon(p_oQuery);
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetPolygon Error =" + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
                throw ex;
            }
            return mAreaList;
        }

        /// <summary>
        /// 新增圖徵
        /// </summary>
        [HttpPost]
        public ApiReturnMessage AddPolygon(List<Feature> o)
        {
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            try
            {
                oApiReturnMessage = oDal.AddPolygon(o);
            }
            catch (Exception ex)
            {
                WriteLog_Error("AddPolygon Error =" + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }


        /// <summary>
        /// 更新圖徵
        /// </summary>
        [HttpPost]
        public ApiReturnMessage UpdatePolygon(Property p_oUpdate)
        {
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            try
            {
                oApiReturnMessage = oDal.UpdatePolygon(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("UpdatePolygon Error =" + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 刪除圖徵
        /// </summary>
        [HttpPost]
        public ApiReturnMessage DeletePolygon(Property p_oUpdate)
        {
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            try
            {
                oApiReturnMessage = oDal.DeletePolygon(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("DeletePolygon" + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }


        /// <summary>
        /// 取得區域
        /// </summary>
        [HttpPost]
        public List<Area> GetArea(Area p_oQuery)
        {
            List<Area> mAreaList = new List<Area>();
            try
            {
                mAreaList = oDal.QryArea(p_oQuery);
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetArea Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
                throw ex;
            }
            return mAreaList;
        }

        /// <summary>
        /// 新增區域
        /// </summary>
        [HttpPost]
        public ApiReturnMessage AddArea(Area p_oUpdate)
        {
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            try
            {
                // 判定是否有數值
                if (p_oUpdate == null || string.IsNullOrEmpty(p_oUpdate.AreaName)) 
                {
                    // 沒有Name 不能使用
                    oApiReturnMessage.ReturnCode = (int)ReturnCode.InputParameterIsEmpty;
                    oApiReturnMessage.ReturnMessage = "請輸入區域名稱與所屬類別";
                    return oApiReturnMessage;
                }
                // 自動加入數值
                oApiReturnMessage = oDal.AddArea(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("AddArea Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 更新區域
        /// </summary>
        [HttpPost]
        public ApiReturnMessage UpdateArea(Area p_oUpdate)
        {
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            try
            {
                if (p_oUpdate != null && p_oUpdate.AreaID != 0)
                {
                    oApiReturnMessage = oDal.UpdateArea(p_oUpdate);
                }
                else 
                {
                    oApiReturnMessage.ReturnCode = (int)ReturnCode.InputParameterIsEmpty;
                    oApiReturnMessage.ReturnMessage = "請輸入AreaID";
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error("UpdateArea Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 刪除區域
        /// </summary>
        [HttpPost]
        public ApiReturnMessage DeleteArea(Area p_oUpdate)
        {
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            try
            {
                oApiReturnMessage = oDal.DeleteArea(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("DeleteArea Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 取得區域人員統計
        /// </summary>
        /// <param name="p_oQuery"></param>
        /// <returns></returns>
        [HttpPost]
        public List<AreaPeopleCount> getAreaPeopleCount(Area p_oQuery)
        {
            List<AreaPeopleCount> mAreaCountList = new List<AreaPeopleCount>();
            try
            {
                mAreaCountList = oDal.AreaPeopleCount(p_oQuery);
            }
            catch (Exception ex)
            {
                WriteLog_Error("getAreaPeopleCount Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
                throw ex;
            }
            return mAreaCountList;
        }

        [HttpPost]
        public List<PeopleArea> getPeopleArea(PersonData p_oQuery)
        {
            List<PeopleArea> mPeopleAreas = new List<PeopleArea>();
            try
            {
                mPeopleAreas = oDal.PeopleArea(p_oQuery);
            }
            catch (Exception ex)
            {
                WriteLog_Error("getPeopleArea Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
                throw ex;
            }
            return mPeopleAreas;
        }

        /// <summary>
        /// 取得進出紀錄
        /// </summary>
        [HttpPost]
        public List<EntryRecord> GetEntryRecord(EntryRecord p_oQuery)
        {
            List<EntryRecord> mEntryRecordList = new List<EntryRecord>();
            try
            {
                mEntryRecordList = oDal.QryEntryRecord(p_oQuery);
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetEntryRecord Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
                throw ex;
            }
            return mEntryRecordList;
        }

        /// <summary>
        /// 取得設備數值
        /// </summary>
        /// <param name="p_oQuery"></param>
        /// <returns></returns>
        [HttpPost]
        public List<PathRecord> GetPathRecord(QryDeviceHis p_oQuery)
        {
            List<PathRecord> mPathRecords = new List<PathRecord>();
            try
            {
                mPathRecords = oDal.QryPathRecord(p_oQuery);
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetPathRecord Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
                throw ex;
            }
            return mPathRecords;
        }


        /// <summary>
        /// 確認點是否在所有區域裡面
        /// </summary>
        [HttpPost]
        public ApiReturnMessage CheckPointArea(QryGPS p_sPoint)
        {
            ApiReturnMessage mApiReturnMessage = new ApiReturnMessage();
            try
            {
                mApiReturnMessage = oDal.CheckPointArea(p_sPoint.Coord);
            }
            catch (Exception ex)
            {
                WriteLog_Error("CheckPointArea Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
                throw ex;
            }
            return mApiReturnMessage;
        }

        [HttpPost]
        public ApiReturnMessage runEntryAndAlarm()
        {
            ApiReturnMessage apiReturnMessage = new ApiReturnMessage();
            try
            {
                apiReturnMessage = oDal.runEntryAndAlarm();
            }
            catch (Exception ex)
            {
                WriteLog_Error("runEntryAndAlarm Error = " + ex.ToString());
                apiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                apiReturnMessage.ReturnMessage = ex.ToString();
            }
            return apiReturnMessage;
        }


        /// <summary>
        /// 取得要提示的警報列表
        /// </summary>
        [HttpPost]
        public List<AlarmMsg> GetAlarmMsg(QryAlarm p_oQuery)
        {
            List<AlarmMsg> mAlarmMsgs = new List<AlarmMsg>();
            try
            {
                mAlarmMsgs = oDal.GetAlarmMsg(p_oQuery);
                return mAlarmMsgs;
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetAlarmMsg Error = " + ex.ToString());
            }
            return null;
        }


        /// <summary>
        /// 賦歸警報
        /// </summary>
        [HttpPost]
        public ApiReturnMessage ResetAlarmMsg(UpdAlarm p_oQuery)
        {
            ApiReturnMessage mApiReturnMessage = new ApiReturnMessage();
            try
            {
                mApiReturnMessage = oDal.ResetAlarmMsg(p_oQuery);
            }
            catch (Exception ex)
            {
                WriteLog_Error("ResetAlarmMsg Error = " + ex.ToString());
                mApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                mApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mApiReturnMessage;
        }
    }
}