using System.ComponentModel.DataAnnotations;

namespace iva.Models
{
    public class DbModel

    {
    }

    public class dbModels
    {
        public int id { get; set; }
        public string card { get; set; }
        public DateTime ts { get; set; }
        public string dev { get; set; }
        public int idx { get; set; }
    }



    public class login
    {
        public int loginId { get; set; }
        public string? userName { get; set; }
        public string? password { get; set; }
        public int companyId { get; set; }
        public int role { get; set; }
    }
    public class company
    {
        public int id { get; set; }
        public string companyName { get; set; }
        public DateTime ts_add { get; set; }
        public DateTime ts_mod { get; set; }

    }



    public class log
    {
        public int id { get; set; }
        public string card { get; set; }
        public DateTime ts { get; set; }
        public string dev { get; set; }
        public int idx { get; set; }
    }
    public class log_stat
    {
        public int id { get; set; }
        public string devName { get; set; }
        public int logStart { get; set; }
        public DateTime ts_add { get; set; }
        public DateTime ts_upd { get; set; }
    }

    public class office
    {
        public int id { get; set; }
        public string office_name { get; set; }
        public DateTime ts_add { get; set; }
        public DateTime ts_mod { get; set; }
    }

    public class office_scanner
    {
        public int id { get; set; }
        public int id_office { get; set; }
        public int id_scanner { get; set; }

    }

    public class scanner
    {
        public int id { get; set; }
        public string dev_code { get; set; }
        public string room_name { get; set; }
        public int type { get; set; }
        public DateTime? ts_add { get; set; }
        public DateTime? ts_mod { get; set; }
        public DateTime? ts_last_online { get; set; }
    }

    public class user
    {
        public int id { get; set; }
        public string userName { get; set; }
        public string card { get; set; }
        public string state { get; set; }

        public DateTime ts_add { get; set; }
        public DateTime ts_mod { get; set; }
    }

    public class emp_mapping_scanner
    {
        public int id { get; set; }
        public int employeeId { get; set; }
        public int scannerId { get; set; }
        public int mapping_by { get; set; }
        public DateTime mapping_dt { get; set; }
        public string type { get; set; }
    }

    public class employee
    {
        public int employeeId { get; set; }
        public string? employeeName { get; set; }
        public string? employeeCode { get; set; }
        public string department { get; set; }
        public string position { get; set; }
        public int experience { get; set; }
        public string rfid { get; set; }

        public int created_by { get; set; }
        public DateTime created_dt { get; set; }
        public int updated_by { get; set; }
        public DateTime updated_dt { get; set; }


        public bool IsActive { get; set; }

        [DataType(DataType.Date)]
        public DateTime from_date { get; set; }
        [DataType(DataType.Date)]
        public DateTime to_date { get; set; }
        public int companyId { get; set; }



    }


    public class role
    {
        public int id { get; set; }
        public string name { get; set; }
        public int type { get; set; }
    }



        public class intern
    {
        public int id { get; set; }
        public string name { get; set; }
        public string department { get; set; }
        public string internCode { get; set; }
        public string phoneNo { get; set; }
        public string purpose { get; set; }
        public string type { get; set; }
        public string rfid { get; set; }
        public bool IsActive { get; set; }
        [DataType(DataType.Date)]
        public DateTime from_date { get; set; }
        [DataType(DataType.Date)]
        public DateTime to_date { get; set; }
        public int companyId { get; set; }


    }


    public class company_device
    {
        public int id { get; set; }
        public string device_code { get; set; }
        public int companyId { get; set; }

    }

    public class department
    {
        public int id { get; set; }
        public string departmentName { get; set; }
        public int companyId { get; set; }

    }
    public class official_holiday
    {
        public int id { get; set; }
        public string holiday_name { get; set; }
        [DataType(DataType.Date)]
        public DateTime holiday_date { get; set; }
        public int companyId { get; set; }
    }


    public class manualEntryDetail
    {
        public int Id { get; set; }
        public int user_id { get; set; }
        public int employee_Id { get; set; }
        public string employee_rfid { get; set; }
        public DateTime current_datetime { get; set; }
        public DateTime actual_datetime { get; set; }
        public DateTime update_datetime { get; set; }
        public string status { get; set; }
        public string reason { get; set; }
    }


    public class specialuser
    {
        public int Id { get; set; }
        public string userName { get; set; }
        public string card { get; set; }
        public string Type { get; set; }
        public DateTime ts_add { get; set; }
        public DateTime ts_mod { get; set; }
       
    }

}