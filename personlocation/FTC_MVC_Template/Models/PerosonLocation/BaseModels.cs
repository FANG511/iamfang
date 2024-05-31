using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


/// <summary>
/// 基礎相關資料查詢，包含公司、人員
/// </summary>
namespace FTC_MES_MVC.Models.PerosonLocation.ViewModels
{
    public class PersonData
    {
        
        public string PersonID { get; set; }
        public string Name { get; set; } 
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string CompanyID { get; set; }
    
    }
    
    /// <summary>
    /// 查詢人員
    /// </summary>
    public class QueryPerson
    {
        public string PersonID { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string CompanyID { get; set; }
    }

    /// <summary>
    /// 更新人員, 提供可一次修改多筆資料
    /// </summary>
    public class UpdatePerson
    {
        public List<PersonData> Persons { get; set; }
    }

    /// <summary>
    /// 查詢公司
    /// </summary>
    public class Company
    { 
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
    }

    /// <summary>
    /// 查詢工廠
    /// </summary>
    public class Factory
    {
        public string CompanyID { get; set; }
        public string FactoryID { get; set; }
        public string FactoryName { get; set; }
    }
}