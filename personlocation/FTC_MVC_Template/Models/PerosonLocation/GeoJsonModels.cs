using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


/// <summary>
/// 地理資訊基礎建檔相關
/// 包含處理GeoJSON的格式處理，
/// </summary>
namespace FTC_MES_MVC.Models.PerosonLocation.ViewModels
{
    /// <summary>
    /// GeoJson的格式
    /// </summary>
    public class GeoJsonDTO
    {
        public string type { get; set; }
        public string name { get; set; }
        public List<Feature> features { get; set; }
    }
    public class GeometryProperty
    {
        public string type { get; set; }
        public List<object> coordinates { get; set; }
    }

    /// <summary>
    /// 多個Feature
    /// </summary>
    public class FeatureGroup
    {
        public List<Feature> Features { get; set; }
    }

    /// <summary>
    /// 每一個圖徵的設定
    /// </summary>
    public class Feature
    {
        public string type { get; set; }
        public Property properties { get; set; }
        public GeometryProperty geometry { get; set; }
    }

    public class WKTResult
    {
        public string PolygonID { get; set; }
        public string PolygonName { get; set; }
        public string AreaID { get; set; }
        public string WKTGeometry { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Property
    {
        public string PolygonID { get; set; }
        public string PolygonName { get; set; }
        public string AreaID { get; set; }
    }

    /// <summary>
    /// 地理空間資料表Polygon 
    /// </summary>
    public class GeoTable
    {
        public int PolygonID { get; set; }
        public string PolygonName { get; set; }
        public int AreaID { get; set; }
        public string GeographyDate {get; set;}
    }

    /// <summary>
    /// 區域檔案
    /// </summary>
    public class Area
    {
        public int AreaID { get; set; }
        public string AreaName { get; set; }
        public string Category { get; set; }
        public int AreaCapUp { get; set; }
        public int AreaCapBot { get; set; }
        public string AreaRole { get; set; }
    }

    /// <summary>
    /// 定位位置組
    /// </summary>
    public class LocationPath
    {
        public List<LocationPoint> Locations { get; set; }
    }
    public class LocationPoint
    {
        public string DeviceId { get; set; }
        public string UserId { get; set; }
        public string GPSLocation { get; set; }
        public string CreateTime { get; set; }
    }

    /// <summary>
    /// 進出紀錄
    /// </summary>
    public class EntryRecord
    { 
        public int ID { get; set; }
        public string DeviceID { get; set; }
        public string AreaID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string ModifyTime { get; set; }
    }

    public class PathRecord
    {
        public string DeviceID { get; set; }
        public string DeviceName { get; set; }
        public string PersonID { get; set; }
        public string PersonName { get; set; }
        public string CompanyID { get; set; }
        public List<string> Coords { get; set; } 
        public List<string> TimeSeries { get; set; }
    }

    public class QryGPS
    {
        public string Coord { get; set; }
    }

    public class AreaPeopleCount
    {
        public int AreaID { get; set; }
        public string AreaName { get; set; }
        public string Category { get; set; }
        public int Count { get; set; }
        public List<String> PersonList { get; set; }
    }

    public class PeopleArea
    {
        public string PersonID { get; set; }
        public string PersonName { get; set; }
        public int AreaID { get; set; }
        public string AreaName { get; set; }
    }

    public class DeviceCoordinate
    {
        public string DeviceID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}