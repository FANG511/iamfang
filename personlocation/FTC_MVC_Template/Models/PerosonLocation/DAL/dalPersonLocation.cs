using Dapper;
using FTC_MES_MVC.Models.PerosonLocation.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Device.Location;
using System.Linq;
using FTC_MES_MVC.Controllers;
using FTC_MES_MVC.Models;
using FTC_MES_MVC.Models.ViewModels;
using System.Text;
using Newtonsoft.Json;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace FTC_MES_MVC.Models.DAL.PersonLocaion
{
    public class dalPersonLocation : dalBase
    {
        protected string sSql { get; set; }

        /// <summary>
        /// 釋放所有API的資源
        /// </summary>
        protected void AllDispose()
        {
            Dispose();
        }
        /// 基礎類別 - 人員相關
        
        /// <summary>
        /// 取得公司基本資料
        /// </summary>
        public List<PerosonLocation.ViewModels.Company> QryCompany()
        {
            try
            {
                return cn.Query<PerosonLocation.ViewModels.Company>("SELECT * FROM Company").ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        /// <summary>
        /// 取得人員公司ID
        /// </summary>
        public string GetCompanyID(string p_Query)
        {
            string companyID = "";
            try
            {
                sSql = @"SELECT CompanyID FROM PersonData WHERE PersonID = @PersonID";
                DynamicParameters p = new DynamicParameters();
                p.Add("@PersonID", p_Query);
                var result = cn.Query(sSql, p).ToList();
                companyID = (string)result[0].CompanyID;
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return companyID;
        }

        /// <summary>
        /// 取得工廠基本資料，可依據公司ID進行篩選
        /// </summary>
        public List<PerosonLocation.ViewModels.Factory> QryFactory(QueryPerson p_oQuery)
        {
            List<PerosonLocation.ViewModels.Factory> mFactoryList = new List<PerosonLocation.ViewModels.Factory>();
            StringBuilder sbSql = new StringBuilder();
            DynamicParameters p = new DynamicParameters();
            try
            {
                sbSql.Append(@"SELECT * FROM FACTORY WHERE 1 = 1");
                if (!string.IsNullOrEmpty(p_oQuery.CompanyID))
                {
                    sbSql.Append(@" AND CompanyID = @CompanyID");
                    p.Add("@CompanyID", p_oQuery.CompanyID);
                }
                mFactoryList = cn.Query<PerosonLocation.ViewModels.Factory>(sbSql.ToString(), p).ToList();
                return mFactoryList;
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }


        /// <summary>
        /// 取得人員基本資料
        /// </summary>
        public List<PersonData> QryPersonData(QueryPerson p_oQuery)
        {
            List<PersonData> mPersonDataList = new List<PersonData>();
            StringBuilder sbSql = new StringBuilder();
            DynamicParameters p = new DynamicParameters();
            try
            {
                sbSql.Append(@"SELECT * FROM PersonData WHERE 1 = 1");
                if (p_oQuery != null)
                {
                    if (!string.IsNullOrEmpty(p_oQuery.PersonID))
                    {
                        sbSql.Append(@" AND PersonID = @PersonID");
                        p.Add("@PersonID", p_oQuery.PersonID);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.Name))
                    {
                        sbSql.Append(@" AND Name = @Name");
                        p.Add("@Name", p_oQuery.Name);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.Gender))
                    {
                        sbSql.Append(@" AND Gender = @Gender");
                        p.Add("@Gender", p_oQuery.Gender);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.CompanyID))
                    {
                        sbSql.Append(@" AND CompanyID = @CompanyID");
                        p.Add("@CompanyID", p_oQuery.CompanyID);
                    }
                }
                //執行一個 SQL 查詢，並將查詢結果映射到 PersonData 物件的列表中
                mPersonDataList= cn.Query<PersonData>(sbSql.ToString(), p).ToList();
                return mPersonDataList;
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        /// <summary>
        /// 更新人員資料
        /// </summary>
        public ApiReturnMessage UpdPersonData(PersonData p_oUpdate)
        {
            try
            {
                // 如果查詢不到值的話要提示
                sSql = @"UPDATE PersonData SET PersonID = @PersonID, Name = @Name, Gender = @Gender, Phone = @Phone, CompanyId = @CompanyID WHERE PersonID = @PersonID";
                cn.ExecuteScalar(sSql, p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 新增人員資料
        /// </summary>
        public ApiReturnMessage AddPersonData(PersonData p_oUpdate)
        {
            try
            {
                //INSERT INTO:向PersonData資料表中的(欄位)新增數值
                //VALUES(具體的數值)
                sSql = @"INSERT INTO PersonData(PersonID, Name, Gender, Phone, CompanyId) 
                        VALUES (@PersonID,@Name,@Gender,@Phone,@CompanyID)";
                cn.ExecuteScalar(sSql, p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 刪除人員資料
        /// </summary>
        public ApiReturnMessage DelPersonData(PersonData p_oUpdate)
        {
            try
            {
                sSql = @"DELETE PersonData WHERE PersonID = @PersonID";
                cn.ExecuteScalar(sSql, p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 取得對應的使用者名稱
        /// </summary>
        private string GetPersonName(String DeviceID)
        {
            string PersonName = "";
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT BindPerson FROM DeviceInfo WHERE DeviceID = @DeviceID");
                DynamicParameters p = new DynamicParameters();
                p.Add("@DeviceID", DeviceID);
                PersonName = cn.Query<string>(sb.ToString(), p).FirstOrDefault();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return PersonName;
        }

        /// <summary>
        /// 取得人員姓名
        /// </summary>
        public string GetPersonDataName(string p_Query)
        {
            string name = "";
            try
            {
                sSql = @"SELECT Name FROM PersonData WHERE PersonID = @PersonID";
                DynamicParameters p = new DynamicParameters();
                p.Add("@PersonID", p_Query);
                var result = cn.Query(sSql, p).ToList();
                name = (string)result[0].Name;
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return name;
        }



        /// 基礎類別 - 設備相關

        /// <summary>
        /// 查詢設備基本資料
        /// </summary>
        public List<DeviceInfo> QryDeviceInfo(DeviceInfo p_oQuery)
        {
            List<DeviceInfo> mDeviceInfo = new List<DeviceInfo>();
            StringBuilder sbSql = new StringBuilder();
            DynamicParameters p = new DynamicParameters();
            try
            {
                sbSql.Append(@"SELECT * FROM DeviceInfo WHERE 1 = 1");
                if (p_oQuery != null)
                {
                    if (!string.IsNullOrEmpty(p_oQuery.DeviceID))
                    {
                        sbSql.Append(@" AND DeviceID = @DeivceID ");
                        p.Add("@DeviceID", p_oQuery.DeviceID);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.DeviceName))
                    {
                        sbSql.Append(@" AND DeviceName = @DeviceName");
                        p.Add("@DeviceName", p_oQuery.DeviceName);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.UUID))
                    {
                        sbSql.Append(@" AND UUID = @UUID");
                        p.Add("@UUID", p_oQuery.UUID);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.Enable))
                    {
                        sbSql.Append(@" AND Enable = @Enable");
                        p.Add("@Enable", p_oQuery.Enable);
                    }
                }
                return cn.Query<DeviceInfo>(sbSql.ToString(), p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        /// <summary>
        /// 更新設備基本資料
        /// </summary>
        public ApiReturnMessage UpdateDeviceInfo(DeviceInfo p_oUpdate)
        {
            try
            {
                sSql = @"UPDATE DeviceInfo SET DeviceName = @DeviceName, UUID = @UUID, BindPerson = @BindPerson, Enable = @Enable WHERE DeviceID = @DeviceID";
                cn.ExecuteScalar(sSql, p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 新增設備基本資訊
        /// </summary>
        public ApiReturnMessage AddDeviceInfo(DeviceInfo p_oUpdate)
        {
            try
            {
                sSql = @"INSERT INTO DeviceInfo(DeviceID, DeviceName, UUID, BindPerson, Enable) VALUES (@DeviceID, @DeviceName, @UUID, @BindPerson, @Enable)";
                cn.ExecuteScalar(sSql, p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 刪除設備基本資訊
        /// </summary>
        public ApiReturnMessage DeleteDeviceInfo(DeviceInfo p_oUpdate)
        {
            try
            {
                sSql = @"DELETE DeviceInfo WHERE DeviceID = @DeviceID";
                cn.ExecuteScalar(sSql, p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 查詢設備即時資訊
        /// </summary>
        public List<Device> QryDeviceCurrent(QryDevice p_oQuery)
        {
            StringBuilder sbSql = new StringBuilder();
            DynamicParameters p = new DynamicParameters();
            try
            {
                // TODO: 後續處理 - 要查即時值，針對異常或未啟用的設施就不處理
                sbSql.Append(@"SELECT DeviceID, DeviceName, Status, PersonID, GPSLocation, Altitude, HeartRate, Temperature,
                               SignalStrength, CONVERT(varchar, CreateTime, 120) AS CreateTime ,BatteryInfo, AlarmCheckFlag FROM DeviceCurrent WHERE 1 = 1");
                if (!string.IsNullOrEmpty(p_oQuery.DeviceID))
                {
                    sbSql.Append(@" AND DeviceID = @DeviceID");
                    p.Add("@DeviceID", p_oQuery.DeviceID);
                }
                if (!string.IsNullOrEmpty(p_oQuery.DeviceName))
                {
                    sbSql.Append(@" AND DeviceName = @DeviceName");
                    p.Add("@DeviceName", p_oQuery.DeviceName);
                }
                if (!string.IsNullOrEmpty(p_oQuery.Status))
                {
                    sbSql.Append(@" AND Status = @Status");
                    p.Add("@Status", p_oQuery.Status);
                }
                if (!string.IsNullOrEmpty(p_oQuery.PersonID))
                {
                    sbSql.Append(@" AND PersonID = @PersonID");
                    p.Add("@PersonID", p_oQuery.PersonID);
                }
                return cn.Query<Device>(sbSql.ToString(), p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        /// <summary>
        /// 新增設備即時資料，更新歷史資料
        /// </summary>
        public ApiReturnMessage AddDeviceValue(UpdDeviceValue p_oUpdate)
        {
            DynamicParameters p = new DynamicParameters();
            try
            {
                if (!string.IsNullOrEmpty(p_oUpdate.UUID))
                {
                    // 先查詢裝置基本資料是否有對應的UUID，若有則進行判斷
                    // 有資料就更新，沒資料就新增，並且將資料同步更新在His資料表中
                    // PersonID在綁定資料中設定
                    // 警報設定中做調整
                    // TODO Float的小數點問題，沒有
                    string status = "開機";
                    sSql = @"MERGE INTO DeviceCurrent AS dc
                             USING (
                                   SELECT (SELECT DeviceID FROM DeviceInfo WHERE UUID = @UUID) AS DeviceID,
                                          (SELECT DeviceName From DeviceInfo WHERE UUID = @UUID) AS DeviceName,
                                          (SELECT BindPerson From DeviceInfo WHERE UUID = @UUID) AS PersonID,
                                          @Status AS Status, @GPSLocation AS GPSLocation, @Altitude AS Altitude,
                                          @HeartRate AS HeartRate, @BloodPressure AS BloodPressure, @Temperature AS Temperature,
                                          @SignalStrength AS SignalStrength, @CreateTime AS CreateTime, @BatteryInfo AS BatteryInfo,
                                          @AlarmCheckFlag AS AlarmCheckFlag ) AS di
                                   ON dc.DeviceID = di.DeviceID
                             WHEN MATCHED THEN
                                   UPDATE SET dc.PersonID = di.PersonID, dc.GPSLocation = di.GPSLocation, dc.Altitude = di.Altitude, dc.HeartRate = di.HeartRate, 
                                          dc.BloodPressure = di.BloodPressure, dc.Temperature = di.Temperature, dc.SignalStrength = di.SignalStrength, 
                                          dc.CreateTime = di.CreateTime, dc.BatteryInfo = di.BatteryInfo 
                             WHEN NOT MATCHED THEN
                                   INSERT (DeviceID, DeviceName, Status, PersonID, GPSLocation, Altitude, HeartRate, BloodPressure, Temperature, SignalStrength,
                                          CreateTime, BatteryInfo, AlarmCheckFlag)
                                   VALUES (di.DeviceID, di.DeviceName, di.Status, di.PersonID, di.GPSLocation, di.Altitude, di.HeartRate, di.BloodPressure, 
                                          di.Temperature, di.SignalStrength, di.CreateTime, di.BatteryInfo, di.AlarmCheckFlag)
                             OUTPUT inserted.DeviceID, inserted.DeviceName, inserted.Status, inserted.PersonID, inserted.GPSLocation, inserted.Altitude, inserted.HeartRate,
                                          inserted.BloodPressure, inserted.Temperature, inserted.SignalStrength, inserted.CreateTime, inserted.BatteryInfo,
                                          inserted.AlarmCheckFlag 
                                    INTO DeviceHis (DeviceID, DeviceName, Status, PersonID, GPSLocation, Altitude, HeartRate, BloodPressure,
                                          Temperature, SignalStrength, CreateTime, BatteryInfo, AlarmCheckFlag);";
                    p.Add("@UUID", p_oUpdate.UUID);
                    p.Add("@Status", status);
                    p.Add("@GPSLocation", p_oUpdate.GPSLocation);
                    p.Add("@Altitude", p_oUpdate.Altitude);
                    p.Add("@HeartRate", p_oUpdate.HeartRate);
                    p.Add("@BloodPressure", p_oUpdate.BloodPressure);
                    p.Add("@Temperature", p_oUpdate.Temperature);
                    p.Add("@SignalStrength", p_oUpdate.SignalStrength);
                    p.Add("@CreateTime", p_oUpdate.CreateTime);
                    p.Add("@BatteryInfo", p_oUpdate.BatteryInfo);
                    p.Add("@AlarmCheckFlag", "Y");
                    cn.ExecuteScalar(sSql, p);
                }
                else
                {
                    // 沒有UUID無法更新資料
                    oApiReturnMessage.ReturnCode = (int)ReturnCode.InputParameterIsEmpty;
                    oApiReturnMessage.ReturnMessage = "沒有UUID";
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 刪除設備即時資料，更新歷史資料
        /// </summary>
        public ApiReturnMessage DeleteDeviceValue(Device p_oUpdate)
        {
            try
            {
                sSql = @"DELETE DeviceCurrent WHERE DeviceID = @DeviceID ";
                cn.ExecuteScalar(sSql, p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 修改設備即時資料，更新歷史資料
        /// </summary>
        public ApiReturnMessage UpdateDeviceValue(Device p_oUpdate)
        {
            try
            {
                // 先查詢裝置是否已經綁定，有綁定則加上borrowerID, 進行update, insert
                sSql = @"UPDATE DeviceCurrent SET DeviceName = @DeviceName, Status = @Status, PersonID = @PersonID, GPSLocation = @GPSLocation,
                                Altitude = @Altitude, HeartRate = @HeartRate, BloodPressure = @BloodPressure, Temperature = @Temperature, 
                                SignalStrength = @SignalStrength, CreateTime = @CreateTime, BatteryInfo = @BatteryInfo, AlarmCheckFlag = @AlarmCheckFlag
                         OUTPUT INSERTED.DeviceID, INSERTED.DeviceName, INSERTED.Status, INSERTED.PersonID, INSERTED.GPSLocation, INSERTED.Altitude, 
                                INSERTED.HeartRate, INSERTED.BloodPressure, INSERTED.Temperature, INSERTED.SignalStrength, INSERTED.CreateTime,
                                INSERTED.BatteryInfo, INSERTED.AlarmCheckFlag
                         INTO BodyMonitoringDeviceHis(DeviceID, DeviceName, Status, PersonID, GPSLocation, Altitude, HeartRate, BloodPressure, Temperature, 
                                SignalStrength, CreateTime, BatteryInfo, AlarmCheckFlag);";
                DynamicParameters p = new DynamicParameters();
                p.Add("@DeviceID", p_oUpdate.DeviceID);
                p.Add("@DeviceName", p_oUpdate.DeviceName);
                p.Add("@Status", p_oUpdate.Status);
                p.Add("@PersonID", p_oUpdate.PersonID);
                p.Add("@GPSLocation", p_oUpdate.GPSLocation);
                p.Add("@Altitude", p_oUpdate.Altitude);
                p.Add("@HeartRate", p_oUpdate.HeartRate);
                p.Add("@BloodPressure", p_oUpdate.BloodPressure);
                p.Add("@Temperature", p_oUpdate.Temperature);
                p.Add("@SignalStrength", p_oUpdate.SignalStrength);
                p.Add("@CreateTime", p_oUpdate.CreateTime);
                p.Add("@BatteryInfo", p_oUpdate.BatteryInfo);
                p.Add("@AlarmCheckFlag", p_oUpdate.AlarmCheckFlag);
                cn.ExecuteScalar(sSql, p);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 查詢設備歷史值
        /// </summary>
        public List<Device> QryDeviceHistory(QryDeviceHis p_oQuery)
        {
            StringBuilder sbSql = new StringBuilder();
            DynamicParameters p = new DynamicParameters();
            try
            {
                sbSql.Append(@"SELECT DeviceID, DeviceName ,Status ,PersonID ,CONVERT(varchar, CreateTime, 120) AS CreateTime
                               FROM DeviceHis WHERE 1 = 1");
                if (!string.IsNullOrEmpty(p_oQuery.DeviceID))
                {
                    sbSql.Append(@" AND DeviceID = @DeviceID");
                    p.Add("@DeviceID", p_oQuery.DeviceID);
                }
                if (!string.IsNullOrEmpty(p_oQuery.DeviceName))
                {
                    sbSql.Append(@" AND DeviceName = @DeviceName");
                    p.Add("@DeviceName", p_oQuery.DeviceName);
                }
                if (!string.IsNullOrEmpty(p_oQuery.Status))
                {
                    sbSql.Append(@" AND Status = @Status");
                    p.Add("@Status", p_oQuery.Status);
                }
                if (!string.IsNullOrEmpty(p_oQuery.PersonID))
                {
                    sbSql.Append(@" AND PersonID = @PersonID");
                    p.Add("@PersonID", p_oQuery.PersonID);
                }
                // 如果沒有給定開始時間結束時，可查詢全部
                if (!string.IsNullOrEmpty(p_oQuery.StartTime) && !string.IsNullOrEmpty(p_oQuery.EndTime))
                {
                    // 依據實際給予的時間查詢
                    sbSql.Append(@" AND CreateTime BETWEEN CAST( @StartTime AS DATETIME ) AND CAST( @EndTime AS DATETIME)");
                    p.Add("@StartTime", p_oQuery.StartTime);
                    p.Add("@EndTime", p_oQuery.EndTime);
                }
                return cn.Query<Device>(sbSql.ToString(), p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }


        /// 基礎類別 - 地理空間相關

        /// <summary>
        /// 查詢進出紀錄
        /// </summary>
        public List<EntryRecord> QryEntryRecord(EntryRecord p_oQuery)
        {
            StringBuilder sbSql = new StringBuilder();
            DynamicParameters p = new DynamicParameters();
            try
            {
                sbSql.Append(@"SELECT ID, DeviceID, AreaID, CONVERT(varchar, StartTime, 120) AS StartTime, 
                               CONVERT(varchar, EndTime, 120) AS EndTime, CONVERT(varchar, ModifyTime, 120) AS ModifyTime,
                               FROM EntryRecord WHERE 1 = 1");
                if (!string.IsNullOrEmpty(p_oQuery.DeviceID))
                {
                    sbSql.Append(@" AND DeviceID = @DeviceID");
                    p.Add("@DeviceID", p_oQuery.DeviceID);
                }
                if (!string.IsNullOrEmpty(p_oQuery.AreaID))
                {
                    sbSql.Append(@" AND AreaID = @AreaID");
                    p.Add("@AreaID", p_oQuery.AreaID);
                }
                // 如果沒有給定開始時間結束時，可查詢全部
                if (!string.IsNullOrEmpty(p_oQuery.StartTime) && !string.IsNullOrEmpty(p_oQuery.EndTime))
                {
                    // 依據實際給予的時間查詢
                    sbSql.Append(@" AND CreateTime BETWEEN CAST( @StartTime AS DATETIME ) AND CAST( @EndTime AS DATETIME)");
                    p.Add("@StartTime", p_oQuery.StartTime);
                    p.Add("@EndTime", p_oQuery.EndTime);
                }
                return cn.Query<EntryRecord>(sbSql.ToString(), p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        /// <summary>
        /// 查詢區域
        /// </summary>
        public List<Area> QryArea(Area p_oQuery)
        {
            DynamicParameters p = new DynamicParameters();
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.Append(@"SELECT * FROM Area WHERE 1 = 1");
                if (p_oQuery != null)
                {
                    if (!string.IsNullOrEmpty(p_oQuery.AreaName))
                    {
                        sbSql.Append(@" AND AreaName = @AreaName");
                        p.Add("@AreaName", p_oQuery.AreaName);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.Category))
                    {
                        sbSql.Append(@" AND Category = @Category");
                        p.Add("@CateGory", p_oQuery.Category);
                    }
                    // 新增可以搜尋群組
                    if (!string.IsNullOrEmpty(p_oQuery.AreaRole))
                    {
                        sbSql.Append(@" AND AreaRole = @AreaRole");
                        p.Add("@AreaRole", p_oQuery.AreaRole);
                    }
                }
                return cn.Query<Area>(sbSql.ToString(), p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        /// <summary>
        /// 新增區域
        /// </summary>
        public ApiReturnMessage AddArea(Area p_oUpdate)
        {
            try
            {
                sSql = @"INSERT INTO Area(AreaName, Category, AreaCapUp, AreaCapBot, AreaRole) VALUES (@AreaName, @Category, @AreaCapUp, @AreaCapBot, @AreaRole)";
                cn.ExecuteScalar(sSql, p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 更新區域
        /// </summary>
        public ApiReturnMessage UpdateArea(Area p_oUpdate)
        {
            try
            {
                sSql = @"UPDATE Area SET AreaName = @AreaName, Category = @Category, AreaCapUp = @AreaCapUp, 
                         AreaCapBot = @AreaCapBot,AreaRole = @AreaRole WHERE AreaID = @AreaID ";
                cn.ExecuteScalar(sSql, p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 刪除區域
        /// </summary>
        public ApiReturnMessage DeleteArea(Area p_oUpdate)
        {
            try
            {
                sSql = @"DELETE Area WHERE AreaID = @AreaID ";
                cn.ExecuteScalar(sSql, p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }


        /// <summary>
        /// 查詢圖徵
        /// </summary>
        public List<WKTResult> QryPolygon(Property p_oQuery)
        {
            DynamicParameters p = new DynamicParameters();
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.Append(@"SELECT PolygonID, PolygonName, AreaID, GeographyData.STAsText() AS WKTGeometry FROM GeoTable WHERE 1 = 1");
                if (p_oQuery != null)
                {
                    if (!string.IsNullOrEmpty(p_oQuery.PolygonID))
                    {
                        sbSql.Append(@" AND PolygonID = @PolygonID");
                        p.Add("@PolygonID", p_oQuery.PolygonID);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.PolygonName))
                    {
                        sbSql.Append(@" AND PolygonName = @PolygonName");
                        p.Add("@PolygonName", p_oQuery.PolygonName);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.AreaID))
                    {
                        sbSql.Append(@" AND AreaID = @AreaID");
                        p.Add("@AreaID", p_oQuery.AreaID);
                    }
                }
                return cn.Query<WKTResult>(sbSql.ToString(), p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        /// <summary>
        /// 新增圖徵
        /// </summary>
        public ApiReturnMessage AddPolygon(List<Feature> o)
        {
            try
            {
                foreach (var oFeature in o)
                {
                    Property properity = oFeature.properties;
                    GeometryProperty geometry = oFeature.geometry;
                    WKTResult oneGeometry = new WKTResult();
                    var oneGeometryResult = string.Empty;
                    var coordinatesAll = new List<string>();
                    if (geometry.type.ToUpper() == "POLYGON")
                    {
                        var geometryList = JsonConvert.DeserializeObject<List<List<List<double>>>>(JsonConvert.SerializeObject(geometry.coordinates));
                        foreach (var coordinates in geometryList)
                        {
                            var onePolygon = new List<string>();
                            foreach (var coordinate in coordinates)
                            {
                                var oneCoordinates = string.Join(" ", coordinate);
                                onePolygon.Add(oneCoordinates);
                            }
                            onePolygon.Reverse();
                            var onePolygonString = string.Format("({0})", string.Join(", ", onePolygon));
                            coordinatesAll.Add(onePolygonString);
                        }
                    }

                    oneGeometryResult = string.Format("{0}({1})", geometry.type.ToUpper(), string.Join(", ", coordinatesAll));
                    oneGeometry.WKTGeometry = oneGeometryResult;
                    oneGeometry.PolygonID = properity.PolygonID;
                    oneGeometry.PolygonName = properity.PolygonName;
                    oneGeometry.AreaID = properity.AreaID;
                    try
                    {
                        StringBuilder sbSql = new StringBuilder();
                        DynamicParameters p = new DynamicParameters();
                        sbSql.AppendLine("INSERT INTO GeoTable(PolygonName, AreaID, GeographyData)");
                        sbSql.AppendLine("VALUES(@PolygonName, @AreaID, geography::STGeomFromText(@WKTGeometry, 4326))");
                        p.Add("@PolygonName", oneGeometry.PolygonName);
                        p.Add("@AreaID", oneGeometry.AreaID);
                        p.Add("@WKTGeometry", oneGeometry.WKTGeometry);

                        cn.ExecuteScalar(sbSql.ToString(), p);
                    }
                    catch (Exception ex)
                    {
                        WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                        oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                        oApiReturnMessage.ReturnMessage = ex.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 刪除圖徵
        /// </summary>
        public ApiReturnMessage DeletePolygon(Property p_oUpdate)
        {
            try
            {
                sSql = @"DELETE FROM GeoTable WHERE PolygonID = @PolygonID";
                cn.ExecuteScalar(sSql, p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 修改圖徵 屬性內容
        /// </summary>
        public ApiReturnMessage UpdatePolygon(Property p_oUpdate)
        {
            try
            {
                sSql = @"UPDATE GeoTable SET PolygonName = @PolygonName, AreaID = @AreaID WHERE PolygonID = @PolygonID";
                cn.ExecuteScalar(sSql, p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }


        /// 地理空間查詢

        /// <summary>
        /// 確認一點有沒有在區域裡面
        /// </summary>
        public List<string> CheckPointInArea(string coordX, string coordY)
        {
            List<string> mAreaList = new List<string>();
            try
            {
                // 查詢所有的圖徵
                List<WKTResult> mList = new List<WKTResult>();
                sSql = @"SELECT PolygonID, PolygonName, AreaID, GeographyData.STAsText() AS WKTGeometry FROM GeoTable";
                mList = cn.Query<WKTResult>(sSql).ToList();

                // 查詢完先釋放資源
                AllDispose();

                if (mList.Count > 0)
                {
                    // 有可判斷的圖徵
                    mList.ForEach(item =>
                    {
                        string polygonID = item.PolygonID;
                        string polygonName = item.PolygonName;
                        string areaPolygon = item.WKTGeometry;

                        var reader = new WKTReader();
                        var polygon = reader.Read(areaPolygon) as Polygon;
                        double x = double.Parse(coordX); // 替換為實際的X座標
                        double y = double.Parse(coordY); // 替換為實際的Y座標
                        var point = new Point(x, y);

                        bool result = polygon.Contains(point);

                        mAreaList.Add($"{polygonID}_{result}");
                    });
                }
                else
                {
                    // 沒有可判斷的圖徵
                    mAreaList.Add("no Area");
                }
                return mAreaList;
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        /// <summary>
        /// 查詢一點有沒有在區域裡面
        /// </summary>
        /// <param name="p_sPoint"></param>
        /// <returns></returns>
        public ApiReturnMessage CheckPointArea(string p_sPoint)
        {
            ApiReturnMessage mApiReturnMessage = new ApiReturnMessage();
            try
            {
                string[] mArray = p_sPoint.Split(',');
                double x = double.Parse(mArray[0]); // 替換為實際的X座標
                double y = double.Parse(mArray[1]); // 替換為實際的Y座標
                // 查詢所有的圖徵
                List<WKTResult> mList = new List<WKTResult>();
                sSql = @"SELECT PolygonID, PolygonName, AreaID, GeographyData.STAsText() AS WKTGeometry FROM GeoTable";
                mList = cn.Query<WKTResult>(sSql).ToList();

                // 查詢完先釋放資源
                AllDispose();

                if (mList.Count > 0)
                {
                    // 有可判斷的圖徵
                    string resultText = "結果";
                    mList.ForEach(item =>
                    {
                        string polygonID = item.PolygonID;
                        string polygonName = item.PolygonName;
                        string areaPolygon = item.WKTGeometry;

                        var reader = new WKTReader();
                        var polygon = reader.Read(areaPolygon) as Polygon;
                        var point = new Point(x, y);

                        bool result = polygon.Contains(point);
                        resultText += $"[{polygonID} is {result}],";
                    });
                    mApiReturnMessage.ReturnMessage = resultText;
                }
                else
                {
                    // 沒有可判斷的圖徵
                    mApiReturnMessage.ReturnMessage = "沒有區域可判斷";
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                mApiReturnMessage.ReturnCode = (int)ReturnCode.InputParameterCheckFail;
                mApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return mApiReturnMessage;
        }

        /// <summary>
        /// 查詢人員所在區域 // 待修改
        /// </summary>
        public List<PeopleArea> PeopleArea(PersonData p_oQuery)
        {
            List<PeopleArea> mList = new List<PeopleArea>();
            try
            {
                StringBuilder sbSql = new StringBuilder();
                DynamicParameters p = new DynamicParameters();
                sbSql.Append(@"SELECT P.PersonID AS PersonID, P.Name AS PersonName, D.DeviceID AS DeviceID, E.AreaID AS AreaID, A.AreaName AS AreaName
                         FROM PersonData AS P 
                         LEFT JOIN DeviceInfo AS D ON P.PersonID = D.BindPerson 
                         LEFT JOIN EntryRecord AS E ON D.DeviceID = E.DeviceID
                         LEFT JOIN GeoTable AS G ON E.AreaID = G.PolygonID
                         LEFT JOIN Area AS A ON G.AreaID = A.AreaID
                         WHERE D.DeviceID IS NOT NULL AND E.EndTime IS NULL");
                
                if (p_oQuery != null && !string.IsNullOrEmpty(p_oQuery.PersonID))
                {
                    sbSql.Append(@" AND P.PersonID = @PersonID");
                    p.Add("@PersonID", p_oQuery.PersonID);
                }
                mList = cn.Query<PeopleArea>(sbSql.ToString(), p).ToList();

                // 處理沒有未在任何區域內的人給予預設值
                mList.ForEach(item =>
                {
                    item.AreaName = item.AreaID == 0 ? item.AreaName = "沒有位於任何區域" : item.AreaName;
                });

                return mList;

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        /// <summary>
        /// 取得區域內人員統計 // 待修改
        /// </summary>
        public List<AreaPeopleCount> AreaPeopleCount(Area p_oQuery)
        {
            List<AreaPeopleCount> mList = new List<AreaPeopleCount>();
            try
            {
                sSql = @"SELECT * FROM Area;";
                mList = cn.Query<AreaPeopleCount>(sSql).ToList();
                
                StringBuilder sbSql = new StringBuilder();
                DynamicParameters p = new DynamicParameters();
                sbSql.Append(@"SELECT A.AreaID AS AreaID, A.AreaName AS AreaName, A.Category AS Category , G.PolygonID AS PolygonID, D.BindPerson AS PersonID
                               FROM Area AS A
                               LEFT JOIN GeoTable AS G ON A.AreaID = G.AreaID
                               LEFT JOIN (SELECT DeviceID, AreaID AS PolygonID FROM EntryRecord WHERE EndTime IS NULL) AS E ON E.PolygonID = G.PolygonID
                               LEFT JOIN DeviceInfo AS D ON D.DeviceID = E.DeviceID
                               WHERE A.AreaID IS NOT NULL");
                if (p_oQuery != null && !string.IsNullOrEmpty(p_oQuery.AreaName))
                {
                    sbSql.Append(@" AND A.AreaName = @AreaName");
                    p.Add("@AreaName", p_oQuery.AreaName);
                }
                var areaPeople = cn.Query(sbSql.ToString(), p).ToList();
                if (areaPeople.Count != 0)
                {
                    foreach (var oArea in areaPeople)
                    {
                        int id = oArea.AreaID;
                        mList.ForEach(el => {
                            int areaid = el.AreaID;
                            if (areaid == id)
                            {
                                // 沒有list 新增一個
                                if (el.PersonList == null)
                                {
                                    List<string> personList = new List<string>();
                                    el.PersonList = personList;
                                }
                                if (oArea.PersonID != null)
                                {
                                    el.Count += 1;
                                    el.PersonList.Add(oArea.PersonID);
                                }
                            }
                        });
                        
                    }
                }
                
                return mList;

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        /// <summary>
        /// 取得查詢歷史軌跡紀錄
        /// </summary>
        public List<PathRecord> QryPathRecord(QryDeviceHis p_oQuery)
        {
            List<PathRecord> mPathRecords = new List<PathRecord>();
            try
            {
                // 先取得要查詢的人員資訊Device Current
                StringBuilder sbSql = new StringBuilder();
                DynamicParameters p = new DynamicParameters();
                List<Device> mDeviceList = new List<Device>();
                sbSql.Append(@"SELECT DeviceID, DeviceName, PersonID, GPSLocation, CONVERT(varchar, CreateTime, 120) AS CreateTime FROM DeviceHis WHERE 1 = 1");
                if (p_oQuery != null)
                {
                    if (!string.IsNullOrEmpty(p_oQuery.DeviceID))
                    {
                        sbSql.Append(@" AND DeviceID = @DeviceID");
                        p.Add("@DeviceID", p_oQuery.DeviceID);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.DeviceName))
                    {
                        sbSql.Append(@" AND DeviceName = @DeviceName");
                        p.Add("@DeviceName", p_oQuery.DeviceName);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.Status))
                    {
                        sbSql.Append(@" AND Status = @Status");
                        p.Add("@Status", p_oQuery.Status);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.PersonID))
                    {
                        sbSql.Append(@" AND PersonID = @PersonID");
                        p.Add("@PersonID", p_oQuery.PersonID);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.StartTime) && !string.IsNullOrEmpty(p_oQuery.EndTime))
                    {
                        sbSql.Append(@" AND CreateTime Between @StartTime AND @EndTime");
                        p.Add("@StartTime", p_oQuery.StartTime);
                        p.Add("@EndTime", p_oQuery.EndTime);
                    }
                }
                mDeviceList = cn.Query<Device>(sbSql.ToString(), p).ToList();
                if (mDeviceList.Count > 0)
                {
                    mDeviceList.ForEach(item =>
                    {
                        string itemID = item.DeviceID;
                        bool check = mPathRecords.Any(el => el.DeviceID.Contains(itemID));
                        if (!check)
                        {
                            PathRecord pathItem = new PathRecord();
                            List<string> coord = new List<string>();
                            List<string> timeSerise = new List<string>();
                            pathItem.DeviceID = item.DeviceID;
                            pathItem.DeviceName = item.DeviceName;
                            pathItem.PersonID = item.PersonID;
                            // get personName
                            pathItem.PersonName = GetPersonDataName(item.PersonID);
                            pathItem.CompanyID = GetCompanyID(item.PersonID);
                            coord.Add(item.GPSLocation);
                            pathItem.Coords = coord;
                            timeSerise.Add(item.CreateTime);
                            pathItem.TimeSeries = timeSerise;
                            mPathRecords.Add(pathItem);
                        }
                        else
                        {
                            List<string> coorList;
                            List<string> timeSeriseList;
                            mPathRecords.ForEach(el =>
                            {
                                if (el.DeviceID.Contains(itemID))
                                {
                                    coorList = el.Coords;
                                    coorList.Add(item.GPSLocation);
                                    timeSeriseList = el.TimeSeries;
                                    timeSeriseList.Add(item.CreateTime);
                                }
                            });
                        };
                    });
                }
                return mPathRecords;
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }



        /// 綁定

        /// <summary>
        /// 取得用於綁定的人員
        /// </summary>
        public List<PersonData> QryBindAvailable(PersonData p_oQuery)
        {
            StringBuilder sbSql = new StringBuilder();
            DynamicParameters p = new DynamicParameters();
            try
            {
                // TODO: 後續處理 - 要查即時值，針對異常或未啟用的設施就不處理
                sbSql.Append(@"SELECT P.PersonID, Name, Gender, Phone, CompanyId AS CompanyID, B.PersonID AS BindPerson FROM PersonData AS P");
                sbSql.Append(@" LEFT JOIN BindDevice AS B ON P.PersonID = B.PersonID ");
                sbSql.Append(@" WHERE B.PersonID IS NULL ");
                if (p_oQuery != null && !string.IsNullOrEmpty(p_oQuery.PersonID))
                {
                    sbSql.Append(@" OR P.PersonID = @PersonID;");
                    p.Add("@PersonID", p_oQuery.PersonID);
                }
                return cn.Query<PersonData>(sbSql.ToString(), p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }


        /// <summary>
        /// 取得綁定紀錄
        /// </summary>
        public List<BindDevcie> QryBindDevice(QryBindDevice p_oQuery)
        {
            StringBuilder sbSql = new StringBuilder();
            DynamicParameters p = new DynamicParameters();
            try
            {
                sbSql.Append(@"SELECT * FROM BindDevice WHERE 1 = 1");
                if (!string.IsNullOrEmpty(p_oQuery.DeviceID))
                {
                    sbSql.Append(@" AND DeviceID = @DeviceID");
                    p.Add("@DeviceID", p_oQuery.DeviceID);
                }
                if (!string.IsNullOrEmpty(p_oQuery.PersonID))
                {
                    sbSql.Append(@" AND PersonID = @PersonID");
                    p.Add("@PersonID", p_oQuery.PersonID);
                }
                // 如果沒有給定開始時間結束時，可查詢全部
                if (!string.IsNullOrEmpty(p_oQuery.StartTime) && !string.IsNullOrEmpty(p_oQuery.EndTime))
                {
                    // 依據實際給予的時間查詢
                    sbSql.Append(@" AND CreateTime BETWEEN CAST( @StartTime AS DATETIME ) AND CAST( @EndTime AS DATETIME)");
                    p.Add("@StartTime", p_oQuery.StartTime);
                    p.Add("@EndTime", p_oQuery.EndTime);
                }
                return cn.Query<BindDevcie>(sbSql.ToString(), p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        /// <summary>
        /// 解除綁定設備
        /// </summary>
        public ApiReturnMessage UnBindDevice(BindDevcie p_oUpdate)
        {
            try
            {
                sSql = @"BEGIN TRANSACTION 
                               UPDATE BindDevice 
                               SET EndTime = @EndTime 
                               WHERE DeviceID = @DeviceID 
                                     AND EndTime IS NULL;

                               UPDATE DeviceCurrent 
                               SET PersonID = NULL 
                               WHERE DeviceID = @DeviceID;

                               UPDATE DeviceInfo
                               SET BindPerson = NULL
                               WHERE DeviceID = @DeviceID
 
                        COMMIT TRANSACTION";
                cn.ExecuteScalar(sSql, p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 綁定設備
        /// </summary>
        public ApiReturnMessage BindDevice(BindDevcie p_oUpdate)
        {
            try
            {
                if (!string.IsNullOrEmpty(p_oUpdate.DeviceID) && !string.IsNullOrEmpty(p_oUpdate.PersonID))
                {
                    sSql = @"BEGIN TRANSACTION;
                             BEGIN TRY
                                   INSERT INTO BindDevice (DeviceID, PersonID, Status, CreateTime)
                                   VALUES (@DeviceID, @PersonID, @Status, @CreateTime);
                                   
                                   UPDATE DeviceCurrent
                                   SET PersonID = @PersonID
                                   WHERE DeviceID = @DeviceID;

                                   UPDATE DeviceInfo
                                   SET BindPerson = @PersonID
                                   WHERE DeviceID = @DeviceID
                               
                                   COMMIT;
                             END TRY
                             BEGIN CATCH
                                   ROLLBACK;
                             END CATCH;";
                    cn.ExecuteScalar(sSql, p_oUpdate);
                }
                else
                {
                    // 缺少Device ID 或 PersonID
                    oApiReturnMessage.ReturnCode = (int)ReturnCode.InputParameterIsEmpty;
                    oApiReturnMessage.ReturnMessage = "缺少裝置或人員";
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }


        /// <summary>
        /// 取得正在綁定中的裝置
        /// </summary>
        public List<BindDevcie> QryBindDeviceNow(QryBindDevice p_oQuery)
        {
            StringBuilder sbSql = new StringBuilder();
            DynamicParameters p = new DynamicParameters();
            try
            {
                sbSql.Append(@"SELECT ID, DeviceID, Status, CONVERT(varchar, CreateTime,120) AS CreateTime FROM BindDevice WHERE EndTime IS NULL");
                if (!string.IsNullOrEmpty(p_oQuery.DeviceID))
                {
                    sbSql.Append(@" AND DeviceID = @DeviceID");
                    p.Add("@DeviceID", p_oQuery.DeviceID);
                }
                if (!string.IsNullOrEmpty(p_oQuery.PersonID))
                {
                    sbSql.Append(@" AND PersonID = @PersonID");
                    p.Add("@PersonID", p_oQuery.PersonID);
                }
                return cn.Query<BindDevcie>(sbSql.ToString(), p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        /// <summary>
        /// 取得設備綁定清單
        /// </summary>
        public List<BindDevciePerson> QryBindDevicePerson()
        {
            List<BindDevciePerson> mBindDevcies = new List<BindDevciePerson>();
            try
            {
                sSql = "SELECT D.DeviceID, D.DeviceName, B.PersonID, P.Name AS PersonName FROM DeviceInfo AS D " +
                   "LEFT JOIN BindDevice AS B ON D.DeviceID = B.DeviceID " +
                   "LEFT JOIN PersonData AS P ON B.PersonID = P.PersonID " +
                   "WHERE B.EndTime IS NULL;";
                return cn.Query<BindDevciePerson>(sSql).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }



        /// 警報

        /// <summary>
        /// 取得警報設定
        /// </summary>
        public List<AlarmSetting> QryAlarmSetting(QryAlarm p_oQuery)
        {
            StringBuilder sbSql = new StringBuilder();
            DynamicParameters p = new DynamicParameters();
            try
            {
                sbSql.Append(@"SELECT * FROM AlarmSetting WHERE 1 = 1");
                if (p_oQuery != null)
                {
                    if (!string.IsNullOrEmpty(p_oQuery.AlarmID))
                    {
                        sbSql.Append(@" AND AlarmID = @AlarmID");
                        p.Add("@AlarmID", p_oQuery.AlarmID);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.Enable))
                    {
                        sbSql.Append(@" AND Enable = @Enable");
                        p.Add("@Enable", p_oQuery.Enable);
                    }
                }
                return cn.Query<AlarmSetting>(sbSql.ToString(), p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        /// <summary>
        /// 新增警報設定
        /// </summary>
        public ApiReturnMessage AddAlarmSetting(AlarmSetting p_oUpdate)
        {
            try
            {
                sSql = @"INSERT INTO AlarmSetting(AlarmID, AlarmName, AlarmField, AlarmRole, AlarmValue, 
                        NotificationGroup, Enable) VALUES (@AlarmID, @AlarmName, @AlarmField, @AlarmRole, 
                        @AlarmValue, @NotificationGroup, @Enable )";
                cn.Execute(sSql, p_oUpdate);

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 刪除警報設定
        /// </summary>
        public ApiReturnMessage DelAlarmSetting(AlarmSetting p_oUpdate)
        {
            try
            {
                sSql = @"DELETE AlarmSetting WHERE AlarmID = @AlarmID";
                cn.ExecuteScalar(sSql, p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 更新警報設定
        /// </summary>
        public ApiReturnMessage UpdAlarmSetting(AlarmSetting p_oUpdate)
        {
            try
            {
                StringBuilder sSqlUpd = new StringBuilder();
                DynamicParameters p = new DynamicParameters();
                sSqlUpd.Append(@"UPDATE AlarmSetting SET");
                // 目前設定有無啟用, 後續再增加
                if (!string.IsNullOrEmpty(p_oUpdate.AlarmValue))
                {
                    // AlarmValue 有值
                    sSqlUpd.Append(@" AlarmValue = @AlarmValue");
                    p.Add("@AlarmValue", p_oUpdate.AlarmValue);
                }
                if (!string.IsNullOrEmpty(p_oUpdate.AlarmValue) && !string.IsNullOrEmpty(p_oUpdate.Enable))
                {
                    sSqlUpd.Append(@" , ");
                }
                if (!string.IsNullOrEmpty(p_oUpdate.Enable))
                {
                    sSqlUpd.Append(@" Enable = @Enable");
                    p.Add("@Enable", p_oUpdate.Enable);
                }
                sSqlUpd.Append(@" WHERE AlarmID = @AlarmID");
                p.Add("@AlarmId", p_oUpdate.AlarmID);
                cn.ExecuteScalar(sSqlUpd.ToString(), p);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 取得警報歷史紀錄
        /// </summary>
        public List<AlarmRecord> QryAlarmRecord(QryAlarm p_oQuery)
        {
            List<AlarmRecord> mAlarmRecordList = new List<AlarmRecord>();
            StringBuilder sbSql = new StringBuilder();
            DynamicParameters p = new DynamicParameters();
            try
            {
                sbSql.Append(@"SELECT ID, DeviceID, PersonID, AlarmID, AlarmValue, CONVERT(varchar, CreateTime,120) AS CreateTime FROM AlarmRecord WHERE DeviceID != 'system'");
                if (p_oQuery != null)
                {
                    if (!string.IsNullOrEmpty(p_oQuery.DeviceID))
                    {
                        sbSql.Append(@" AND DeviceID = @DeviceID");
                        p.Add("@DeviceID", p_oQuery.DeviceID);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.PersonID))
                    {
                        sbSql.Append(@" AND PersonID = @PersonID");
                        p.Add("@PersonID", p_oQuery.PersonID);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.AlarmID))
                    {
                        sbSql.Append(@" AND AlarmID = @AlarmID");
                        p.Add("@AlarmID", p_oQuery.AlarmID);
                    }
                    // 如果沒有給定開始時間結束時，可查詢全部
                    if (!string.IsNullOrEmpty(p_oQuery.StartTime) && !string.IsNullOrEmpty(p_oQuery.EndTime))
                    {
                        // 依據實際給予的時間查詢
                        sbSql.Append(@" AND CreateTime BETWEEN CAST( @StartTime AS DATETIME ) AND CAST( @EndTime AS DATETIME)");
                        p.Add("@StartTime", p_oQuery.StartTime);
                        p.Add("@EndTime", p_oQuery.EndTime);
                    }
                }
                return cn.Query<AlarmRecord>(sbSql.ToString(), p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        /// <summary>
        /// 修改警報紀錄
        /// </summary>
        public ApiReturnMessage UpdateAlarmRecord(AlarmRecord p_oUpdate)
        {
            try
            {
                sSql = @"UPDATE AlarmRecord SET DeivceID = @DeviceID, PersonID = @PersonID, AlarmID = @AlarmID, 
                         AlarmValue = @AlarmValue, CreateTime = @CareteTime WHERE ID = @ID ";
                cn.ExecuteScalar(sSql, p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 刪除警報紀錄
        /// </summary>
        public ApiReturnMessage DeleteAlarmRecord(AlarmRecord p_oUpdate)
        {
            try
            {
                sSql = @"DELETE AlarmRecord WHERE ID = @ID ";
                cn.ExecuteScalar(sSql, p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 取得待復歸警報
        /// </summary>
        public List<AlarmMsg> GetAlarmMsg(QryAlarm p_oQuery)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                DynamicParameters p = new DynamicParameters();
                sbSql.Append(@"SELECT DeviceID, PersonID, AlarmID, Description, IsDupAlarmOn, CONVERT(varchar, EventTime, 120) AS EventTime FROM AlarmMsg WHERE IsDupAlarmOn = 'F'");
                if (p_oQuery != null)
                {
                    if (!string.IsNullOrEmpty(p_oQuery.DeviceID))
                    {
                        sbSql.Append(@" AND DeviceID = @DeviceID");
                        p.Add("@DeviceID", p_oQuery.DeviceID);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.PersonID))
                    {
                        sbSql.Append(@" AND PersonID = @PersonID");
                        p.Add("@PersonID", p_oQuery.PersonID);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.AlarmID))
                    {
                        sbSql.Append(" AND AlarmID = AlarmID");
                        p.Add("@AlarmID", p_oQuery.AlarmID);
                    }
                    if (!string.IsNullOrEmpty(p_oQuery.StartTime) && !string.IsNullOrEmpty(p_oQuery.EndTime))
                    {
                        sbSql.Append(@" AND EventTime BETWEEN @StartTime AND @ EndTime");
                        p.Add("@StartTime", p_oQuery.StartTime);
                        p.Add("@EndTime", p_oQuery.EndTime);
                    }
                }
                return cn.Query<AlarmMsg>(sbSql.ToString(), p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        /// <summary>
        /// 復歸警報
        /// </summary>
        public ApiReturnMessage ResetAlarmMsg(UpdAlarm p_oQuery)
        {
            ApiReturnMessage mApiReturnMessage = new ApiReturnMessage();
            try
            {
                if (p_oQuery == null)
                {
                    mApiReturnMessage.ReturnCode = (int)ReturnCode.InputParameterIsNull;
                    mApiReturnMessage.ReturnMessage = "沒有傳入數值";
                }
                else
                {
                    if (!string.IsNullOrEmpty(p_oQuery.DeviceID) && !string.IsNullOrEmpty(p_oQuery.PersonID) && !string.IsNullOrEmpty(p_oQuery.AlarmID))
                    {
                        // 都有數值才可以查詢
                        StringBuilder sbSql = new StringBuilder();
                        DynamicParameters p = new DynamicParameters();
                        sbSql.Append(@"UPDATE AlarmMsg SET IsDupAlarmOn = 'T', LastTxnTime = @EventTime 
                                       WHERE DeviceID = @DeviceID AND PersonID = @PersonID AND AlarmID = @AlarmID;");
                        p.Add("@DeviceID", p_oQuery.DeviceID);
                        p.Add("@PersonID", p_oQuery.PersonID);
                        p.Add("@AlarmID", p_oQuery.AlarmID);
                        p.Add("@EventTime", p_oQuery.EditTime);
                        cn.ExecuteScalar(sbSql.ToString(), p);
                    }
                    else
                    {
                        // 沒有足夠的參數
                        mApiReturnMessage.ReturnCode = (int)ReturnCode.InputParameterIsEmpty;
                        mApiReturnMessage.ReturnMessage = "缺少足夠參數";
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                mApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                mApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return mApiReturnMessage;
        }

        /// 警報判斷

        /// <summary>
        /// 執行警報內容
        /// </summary>
        public ApiReturnMessage runEntryAndAlarm()
        {
            ApiReturnMessage mApiReturnMessage = new ApiReturnMessage();
            try
            {
                string mDeviceID = "";
                // 取得警報列表
                List<AlarmCheck> mAlarm = runAlarmCheck();
                if (mAlarm.Count == 0)
                {
                    // 沒有裝置直接回傳
                    mApiReturnMessage.ReturnCode = (int)ReturnCode.InputParameterCheckFail;
                    mApiReturnMessage.ReturnMessage = "沒有警報";
                    return mApiReturnMessage;
                }
                // 先跑目前即時值位在哪個位置
                string status = "開機";
                sSql = @"SELECT DeviceID, GPSLocation FROM DeviceCurrent WHERE Status = @Status;";
                DynamicParameters p = new DynamicParameters();
                p.Add("@Status", status);
                var mResultLocation = cn.Query(sSql, p).ToList();
                // 定義時間
                string mTime = "2023-05-15 09:00:00";
                if (mResultLocation.Count == 0)
                {
                    // 沒有裝置直接回傳
                    mApiReturnMessage.ReturnCode = (int)ReturnCode.InputParameterCheckFail;
                    mApiReturnMessage.ReturnMessage = "沒有即時裝置";
                    return mApiReturnMessage;
                }
                List<string> mDeviceOnList = new List<string>();

                mResultLocation.ForEach(item =>
                {
                    //定義device id
                    mDeviceID =(string) item.DeviceID;
                    mDeviceOnList.Add(mDeviceID);
                    string mGPS = (string)item.GPSLocation;
                    string[] mCoord = mGPS.TrimStart('[').TrimEnd(']').Split(',');
                    string mCoordX = mCoord[1];
                    string mCoordY = mCoord[0];
                    // 確認點位在區域內的狀況
                    List<string> mArea = CheckPointInArea(mCoordX, mCoordY);
                    if (mArea.Count > 1)
                    {
                        mArea.ForEach(area =>
                        {
                            string mAreaID = area.Split('_')[0];
                            string result = area.Split('_')[1];
                            if (result == "True")
                            {
                                // 有在區域內的
                                sSql = @"MERGE INTO EntryRecord AS Ta
                                         USING (SELECT @DeviceID AS DeviceID, @AreaID AS AreaID ) AS So
                                              ON (Ta.DeviceID = So.DeviceID AND Ta.AreaID = So.AreaID AND Ta.EndTime IS NOT NULL)
                                         WHEN MATCHED THEN
                                              UPDATE SET Ta.ModifyTime = @DateTime
                                         WHEN NOT MATCHED THEN
                                              INSERT (DeviceID, AreaID, StartTime, ModifyTime)
                                              VALUES (@DeviceID, @AreaID, @DateTime, @DateTime);";
                                DynamicParameters pa = new DynamicParameters();
                                pa.Add("@DeviceID", mDeviceID);
                                pa.Add("@AreaID", mAreaID);
                                pa.Add("@DateTime", mTime);
                                cn.ExecuteScalar(sSql, pa);
                            }
                            else {
                                // 沒有在區域內的
                                sSql = @"UPDATE EntryRecord SET EndTime = @DataTime, ModifyTime = @DataTime WHERE DeviceID = @DeviceID AND AreaID = @AreaID AND EndTime IS NULL;";
                                DynamicParameters pa = new DynamicParameters();
                                pa.Add("@DeviceID", mDeviceID);
                                pa.Add("@AreaID", mAreaID);
                                pa.Add("@DataTime", mTime);
                                cn.ExecuteScalar(sSql, pa);
                            }
                        });
                    }
                });
                // 進行即時值判定
                string mStatus = "開機";
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append( @"SELECT * FROM DeviceCurrent WHERE Status = @Status;");
                DynamicParameters p_device = new DynamicParameters();
                p_device.Add("@Status", mStatus);
                List<Device> mDeviceList = new List<Device>();
                mDeviceList = cn.Query<Device>(sbSql.ToString(), p_device).ToList();
                mDeviceList.ForEach(device_c => {
                    string deviceId = device_c.DeviceID;
                    string personId = device_c.PersonID;
                    string location = device_c.GPSLocation;
                    double altitude = Math.Round(device_c.Altitude, 2);
                    double heartRate = Math.Round(device_c.HeartRate,2);
                    double bloodPressure = Math.Round(device_c.BloodPressure,2);
                    double temperature = Math.Round(device_c.Temperature,2);
                    mAlarm.ForEach(alarm => {
                        string target = alarm.TableName;
                        if (target == "DeviceCurrent" && alarm.Type == "max/min") {
                            string field = alarm.Field;
                            switch (field) {
                                case "Altitude":
                                    CheckAlarmInterval(deviceId, personId, altitude, alarm, mTime);
                                    break;
                                case "HeartRate":
                                    CheckAlarmInterval(deviceId, personId, heartRate, alarm, mTime);
                                    break;
                                case "BloodPressure":
                                    CheckAlarmInterval(deviceId, personId, bloodPressure, alarm, mTime);
                                    break;
                                case "Temperature":
                                    CheckAlarmInterval(deviceId, personId, temperature, alarm, mTime);
                                    break;
                            }                                                                              
                        } 
                        else if (target == "DeviceCurrent" && alarm.Type == "out")
                        {
                            CheckAlarmAreaExit(deviceId, personId, location, alarm, mTime);
                        }
                    });
                });
                // 進行歷史值判定
                mAlarm.ForEach(alarm =>
                {
                    string target = alarm.TableName;
                    if (target == "DeviceHis")
                    {
                        CheckAlarmStayOnePlace(alarm, mTime, mDeviceOnList);
                    }
                });
                // 進行進出判定
                mAlarm.ForEach(alarm =>
                {
                    string target = alarm.TableName;
                    if (target == "EntryRecord" && alarm.Type == "period")
                    {
                        // 停留時間過久
                        CheakAlarmStayPeroid(alarm, mTime, mDeviceOnList);
                    }
                    else if (target == "DeviceCurrent" && alarm.Type == "in")
                    {
                        // 進入
                        CheckAlarmAreaEntry(alarm, mTime, mDeviceOnList);
                    }
                });

                UpdateAlarmTime(mTime);
                mApiReturnMessage.ReturnCode = (int)ReturnCode.Success;
                mApiReturnMessage.ReturnMessage = "成功執行完警報";
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                mApiReturnMessage.ReturnCode = (int)ReturnCode.InputParameterCheckFail;
                mApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return mApiReturnMessage;
        }

        /// <summary>
        /// 停留時間過長
        /// </summary>
        private void CheakAlarmStayPeroid(AlarmCheck alarm, string mTime, List<string> mDeviceOnList)
        {
            try
            {
                string AlarmID = alarm.AlarmID;
                int Period = Int32.Parse(alarm.AlarmValue);
                // 取得停留時間的buffer
                // 針對進出列表的時間做查詢
                sSql = @"SELECT DeviceID, AreaID, CONVERT(varchar, StartTime,120) AS StartTime FROM EntryRecord WHERE EndTime IS NOT NULL";
                var mEntryRecordList = cn.Query(sSql).ToList();
                if (mEntryRecordList.Count > 0)
                {
                    // 批次查詢
                    mEntryRecordList.ForEach(item =>
                    {
                        string deviceid = item.DeviceID;
                        string startTime = item.StartTime;
                        string areaid = item.AreaID;
                        if (mDeviceOnList.Contains(deviceid))
                        {
                            // 有在裡面就可以進行
                            DateTime entryTime = Convert.ToDateTime(startTime);
                            DateTime nowTime = Convert.ToDateTime(mTime);
                            TimeSpan ts = nowTime - entryTime;
                            if (ts.TotalMinutes > Period)
                            {
                                // 超過了
                                string total = ts.TotalMinutes.ToString();
                                string alarmValue = areaid + ":停留" + total + "分鐘";
                                AddNewAlarm(deviceid, "", AlarmID, alarmValue, mTime);
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 是否進入區域
        /// </summary>
        private void CheckAlarmAreaEntry(AlarmCheck alarm, string mTime, List<string> mDeviceOnList)
        {
            try
            {
                string AlarmID = alarm.AlarmID;
                // 針對進出列表的時間做查詢
                sSql = @"SELECT DeviceID, AreaID, CONVERT(varchar, StartTime,120) AS StartTime FROM EntryRecord WHERE EndTime IS NOT NULL";
                var mEntryRecordList = cn.Query(sSql).ToList();
                if (mEntryRecordList.Count > 0)
                {
                    // 批次查詢
                    mEntryRecordList.ForEach(item =>
                    {
                        string deviceid = item.DeviceID;
                        string startTime = item.StartTime;
                        string areaid = item.AreaID;
                        if (mDeviceOnList.Contains(deviceid))
                        {
                            // 有在裡面就可以進行

                            string alarmValue = "進入" + areaid + "區域";
                            AddNewAlarm(deviceid, "", AlarmID, alarmValue, mTime);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 停留時間過長
        /// </summary>
        private void CheckAlarmStayOnePlace(AlarmCheck alarm, string mTime, List<string> mDeviceOnList)
        {
            try
            {
                string AlarmID = alarm.AlarmID;
                string str = alarm.AlarmValue;
                int period = Int32.Parse(str.Split(',')[0]);
                int buffer = Int32.Parse(str.Split(',')[1]);
                DateTime newTime = DateTime.Parse(mTime);
                TimeSpan ts = TimeSpan.FromMinutes(period);
                DateTime traceTime = newTime.Subtract(ts);
                string fromTime = traceTime.ToString("yyyy-MM-dd HH:mm:ss");
                sSql = @"SELECT DeviceID, GPSLocation FROM DeviceHis WHERE CreateTime BETWEEN @StartTime AND @EndTime;";
                DynamicParameters p = new DynamicParameters();
                p.Add("@StartTime", fromTime);
                p.Add("@Endtime", mTime);
                var historyList = cn.Query(sSql, p).ToList();
                Dictionary<string, List<DeviceCoordinate>> mDeviceCoordinates = new Dictionary<string, List<DeviceCoordinate>>();
                if (historyList.Count > 0)
                {
                    // 處理設備列表~依照device來排列
                    historyList.ForEach(item =>
                    {
                        DeviceCoordinate device = new DeviceCoordinate();
                        device.DeviceID = item.DeviceID;
                        string strCoord = item.GPSLocation;
                        strCoord = strCoord.TrimStart('[').TrimEnd(']');
                        device.Longitude = Double.Parse(strCoord.Split(',')[1]);
                        device.Latitude = Double.Parse(strCoord.Split(',')[0]);
                        if (!mDeviceCoordinates.ContainsKey(device.DeviceID))
                        {
                            mDeviceCoordinates[device.DeviceID] = new List<DeviceCoordinate>();
                        }

                        mDeviceCoordinates[device.DeviceID].Add(device);
                        
                    });                                     
                }
                mDeviceOnList.ForEach(item =>
                {
                    if (mDeviceCoordinates.ContainsKey(item))
                    {
                        var device = mDeviceCoordinates[item];
                        // 進行判定
                        if (device.Count > 1)
                        {
                            var coord = new GeoCoordinate(device[0].Latitude, device[0].Longitude);
                            Boolean mJudge = true;
                            for (int i = 1; i < device.Count; i++)
                            {                                
                                var distant = coord.GetDistanceTo(new GeoCoordinate(device[i].Latitude, device[i].Longitude));
                                if (distant > buffer)
                                {
                                    mJudge = false;
                                }
                            }
                            if (mJudge)
                            {
                                // 跳警報
                                string alarmValue = "超過" + period + "分鐘沒有移動";
                                string PersonID = GetPersonName(item);
                                AddNewAlarm(item, PersonID, AlarmID, alarmValue, mTime);
                            }
                        }

                    }
                });
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }

        }

        /// <summary>
        /// 確認是否在數值範圍裏面
        /// </summary>
        private void CheckAlarmInterval(string DeviceID, string PersonID, double Value, AlarmCheck o, string DateTime)
        {
            try
            {
                string table = o.TableName;
                string field = o.Field;
                string val_arr = o.AlarmValue;
                double max_value = Math.Round(Double.Parse(val_arr.Split(',')[0]),3);
                double min_value = Math.Round(Double.Parse(val_arr.Split(',')[1]),3);
                if (Value > max_value || Value < min_value)
                {
                    // 進行警報
                    string alarmValue = Value.ToString();
                    AddNewAlarm(DeviceID, PersonID, o.AlarmID, alarmValue, DateTime);
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
        }

        /// <summary>
        /// 確認是否離開區域
        /// </summary>
        private void CheckAlarmAreaExit(string deviceId, string personId, string location, AlarmCheck alarm, string mTime)
        {
            try
            {
                location = location.TrimStart('[').TrimEnd(']');
                double x = double.Parse(location.Split(',')[1]); // 替換為實際的X座標
                double y = double.Parse(location.Split(',')[0]); // 替換為實際的Y座標
                // 查詢所有的圖徵
                List<WKTResult> mList = new List<WKTResult>();
                sSql = @"SELECT PolygonID, PolygonName, AreaID, GeographyData.STAsText() AS WKTGeometry FROM GeoTable";
                mList = cn.Query<WKTResult>(sSql).ToList();

                // 查詢完先釋放資源
                AllDispose();

                if (mList.Count > 0)
                {
                    // 有可判斷的圖徵
                    mList.ForEach(item =>
                    {
                        string polygonID = item.PolygonID;
                        string polygonName = item.PolygonName;
                        string areaPolygon = item.WKTGeometry;

                        var reader = new WKTReader();
                        var polygon = reader.Read(areaPolygon) as Polygon;
                        var point = new Point(x, y);

                        bool result = polygon.Contains(point);

                        // 進行警報
                        string alarmValue = "離開" + polygonID + "區域";
                        AddNewAlarm(deviceId, personId, alarm.AlarmID, alarmValue, mTime);
                    });
                }                
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 更新警報最新最後更新時間
        /// </summary>
        private void UpdateAlarmTime(string mTime)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                DynamicParameters p = new DynamicParameters();
                sbSql.Append(@"UPDATE AlarmRecord SET CreateTime = @DateTime WHERE DeviceID = 'system' AND AlarmID = 'system_alarm_scan_time';");
                p.Add("DateTime", mTime);
                cn.ExecuteScalar(sbSql.ToString(), p);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 查詢有需要處理的警報
        /// </summary>
        public List<AlarmCheck> runAlarmCheck()
        {
            List<AlarmSetting> mAlarmSettingList = new List<AlarmSetting>();
            List<AlarmCheck> mAlarmChecks = new List<AlarmCheck>();
            try
            {
                // 確認那些警報需要被啟動
                sSql = @"SELECT * FROM AlarmSetting WHERE Enable = 'Y';";
                mAlarmSettingList = cn.Query<AlarmSetting>(sSql).ToList();
                if (mAlarmSettingList.Count > 0)
                {
                    mAlarmSettingList.ForEach(item => {
                        AlarmCheck alarm = new AlarmCheck();
                        alarm.AlarmID = item.AlarmID;
                        alarm.AlarmValue = item.AlarmValue;
                        alarm.Type = item.AlarmRole;
                        string target = item.AlarmField.Split('.')[1];
                        switch (target)
                        {
                            case "StayOnePlace":
                                alarm.TableName = "DeviceHis";
                                alarm.Field = "GPSLocation";
                                break;
                            case "StayPeroid":
                                alarm.TableName = "EntryRecord";
                                alarm.Field = "StartTime";
                                break;
                            case "AreaEntry":
                                alarm.TableName = "EntryRecord";
                                alarm.Field = "StartTime";
                                break;
                            case "AreaExit":
                                alarm.TableName = "DeviceCurrent";
                                alarm.Field = "GPSLocation";
                                break;
                            default:
                                alarm.TableName = "DeviceCurrent";
                                alarm.Field = target;
                                break;
                        }
                        mAlarmChecks.Add(alarm);
                        
                    });

      
                }
                return mAlarmChecks;
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return mAlarmChecks;
        }

        /// <summary>
        /// 新增一筆警報紀錄
        /// </summary>
        private void AddNewAlarm(string deviceID, string personID, string alarmID, string value, string dateTime)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                DynamicParameters p = new DynamicParameters();
                AddAlarmMsg(deviceID, personID, alarmID, value, dateTime);
                sbSql.Append(@"INSERT AlarmRecord(DeviceID, PersonID, AlarmID, AlarmValue, CreateTime) 
                               VALUES( @DeviceID, @PersonID, @AlarmID, @AlarmValue, @CreateTime)");
                p.Add("@DeviceID", deviceID);
                p.Add("@PErsonID", personID);
                p.Add("@AlarmID", alarmID);
                p.Add("@AlarmValue", value);
                p.Add("CreateTime", dateTime);
                cn.ExecuteScalar(sbSql.ToString(), p);
                
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 新增一筆待處理警報
        /// </summary>
        private void AddAlarmMsg(string deviceID, string personID, string alarmID, string value, string dateTime)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                DynamicParameters p = new DynamicParameters();
                sbSql.Append(@"INSERT INTO AlarmMsg (DeviceID, PersonID, AlarmID, Description, IsDupAlarmOn, EventTime)
                               SELECT @DeviceID, @PersonID, @AlarmID, @Message, 'F', @EventTime
                               WHERE NOT EXISTS (
                                     SELECT * FROM AlarmMsg WHERE AlarmID = @AlarmID AND DeviceID = @DeviceID AND PersonID = @PersonID AND IsDupAlarmOn = 'F' 
                               );");
                p.Add("@DeviceID", deviceID);
                p.Add("@PersonID", personID);
                p.Add("@AlarmID", alarmID);
                p.Add("@Message", value);
                p.Add("@EventTime", dateTime);
                cn.ExecuteScalar(sbSql.ToString(), p);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
        }
    }
}