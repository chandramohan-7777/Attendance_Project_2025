using System.ComponentModel.DataAnnotations;

namespace iva.Models
{
    public class ResultModel
    {
    }

    public class ScannerResultModel
    {
        public string? message { get; set; }
        public int? status { get; set; }
        public List<scanner>? list { get; set; }
    }

    public class devices
    {
        public string? deviceId { get; set; }

    }

    public class dList
    {
        public DateTime date { get; set; }
        public string dayOfWeek { get; set; }
        public int day { get; set; }
    }

    public class filterDates
    {
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }

    }

    public class attendanceList
    {
        public string? rfid { get; set; }
        public string? employeeCode { get; set; }
        public string? employeeName { get; set; }
        public List<dateList>? dList { get; set; }
    }
    public class reportList
    {
        public string? rfid { get; set; }
        public string? employeeCode { get; set; }
        public string? employeeName { get; set; }
        public List<dateList>? dList { get; set; }


    }


    public class CompanyViewModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
    }

    public class InternreportList
    {
        public string? internCode { get; set; }
        public string? internName { get; set; }
        public List<dateInternList>? dList { get; set; }


    }


    public class dateInternList
    {
        internal string totalBreakingHours;
        public DateTime date { get; set; }
        public string? dType { get; set; }
        public DateTime internIn { get; set; }
        public DateTime internOut { get; set; }
        public bool status { get; set; }
        public string dayOfWeek { get; set; }
        public int day { get; set; }
        public string workingDayType { get; set; }


    }





    public class dateList
    {
        internal string totalBreakingHours;
        public DateTime date { get; set; }
        public string? dType { get; set; }
        public DateTime empIn { get; set; }
        public DateTime empOut { get; set; }
        public bool status { get; set; }
        public string dayOfWeek { get; set; }
        public int day { get; set; }
        public string? workingDayType { get; set; }

    }

    public class EmployeeData
    {

        public string? empId { get; set; }
        public string? empName { get; set; }
        public string? empCode { get; set; }
        public string? empRfid { get; set; }


    }

    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LogListViewModel
    {
        public int? id { get; set; }
        public string? employeeCode { get; set; }
        public string? employeeName { get; set; }
        public string? department { get; set; }
        public DateTime empIn { get; set; }
        public DateTime empOut { get; set; }
        public int? status { get; set; }
    }

    public class deviceList
    {
        public int? id { get; set; }
        public string name { get; set; }
        public bool status { get; set; }
    }

    public class deviceReq
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public int type { get; set; }
    }

    public class dateReq
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }

    public class employeeReqModel
    {
        public int employeeId { get; set; }
        public string? employeeName { get; set; }
        public string? employeeCode { get; set; }
        public string department { get; set; }
        public string position { get; set; }
        public int experience { get; set; }
        public string rfid { get; set; }
        public bool IsActive { get; set; }

        public DateTime from_date { get; set; }

        public DateTime to_date { get; set; }
    }

    public class employeeModel
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





    }
    public class internModel
    {
        public int id { get; set; }
        public string department { get; set; }
        public string internCode { get; set; }
        public string? name { get; set; }
        public string? phoneNo { get; set; }
        public string? purpose { get; set; }
        public string? type { get; set; }
        public string rfid { get; set; }
        public bool IsActive { get; set; }
        public DateTime from_date { get; set; }
        [DataType(DataType.Date)]
        public DateTime to_date { get; set; }

    }


    public class userModel
    {
        public int id { get; set; }
        public string userName { get; set; }
        public string card { get; set; }

        public DateTime ts_add { get; set; }
        public DateTime ts_mod { get; set; }


    }




    public class AllEmployee
    {

        public int? empId { get; set; }
        public string? empName { get; set; }
        public string? empCode { get; set; }
        public string? empRfid { get; set; }


    }

    public class EmployeeInformation
    {
        public int? employeeNewId { get; set; }
        public string? employeeNewName { get; set; }
        public string? employeeNewCode { get; set; }
        public bool employeeNewStatus { get; set; }
        public string? employeeNewRfid { get; set; }


    }






    public class Newemployees
    {
        public string rfid { get; set; }
        // Add other properties as needed
    }



    // Model for request binding
    public class ManualEntryInRequest
    {
        public DateTime InTime { get; set; }
        public string RFID { get; set; }
        public string devCode { get; set; }
    }





    public class ManualEntryOutRequest
    {
        public DateTime OutTime { get; set; }
        public string RFID { get; set; }
        public string devCode { get; set; }
    }




    public class ManualEntryInOutRequest
    {


        public string RFID { get; set; }
        public string devCode { get; set; }

        public InTimeData? INTIME { get; set; }
        public OutTimeData? OUTTIME { get; set; }
        public string reason { get; set; }

    }




    public class InTimeData
    {
        public DateTime ActualInTime { get; set; }
        public DateTime InTime { get; set; }
    }

    public class OutTimeData
    {
        public DateTime ActualOutTime { get; set; }
        public DateTime OuTime { get; set; }
    }


    public class official_holidayList
    {
        public int id { get; set; }
        public string holidayDayOfWeek { get; set; }
        public string holidayName { get; set; }
        public DateTime holidayDate { get; set; }
        public int companyId { get; set; }
    }

    public class departmentNewList
    {
        public int id { get; set; }
        public string departmentName { get; set; }
        public int companyId { get; set; }

    }

    public class scannerViewModel
    {
        public int id { get; set; }
        public string dev_code { get; set; }
        public string room_name { get; set; }
        public int type { get; set; }
        public DateTime? ts_add { get; set; }
        public DateTime? ts_mod { get; set; }
        public DateTime? ts_last_online { get; set; }

        public bool serverStatus { get; set; }

    }



    public class LogRecordsDateList
    {
        public DateTime date { get; set; }
        public string dayOfWeek { get; set; }
        public int day { get; set; }
    }

    public class EmployeeLogRecodsList
    {
        public string rfid { get; set; }
        public string? employeeCode { get; set; }
        public string day { get; set; }
        public string employeeName { get; set; }
        public string dayOfWeek { get; set; }
        public DateTime date { get; set; }  
        public List<employeelogs> records { get; set; }
    }



    public class InternLogRecodsList
    {
        public string rfid { get; set; }
        public string? internCode { get; set; }
        public string day { get; set; }
        public string internName { get; set; }
        public string dayOfWeek { get; set; }
        public DateTime date { get; set; }
        public List<internlogs> records { get; set; }
    }

    public class employeelogs
    {
        public DateTime logs { get; set; }
        public string logType { get; set; }

    }

    public class internlogs
    {
        public DateTime logs { get; set; }
        public string logType { get; set; }

    }


    public class RetriveAllRecords
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string employeeRfid { get; set; }

    }


    public class RetriveInternAllRecords
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string internRfid { get; set; }

    }

    public class LogEmployee
    {
        public string Rfid { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

    }



   public class  RetriveEmployeeAllRecords
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        
    }


    public class RetriveAllInternRecords
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

    }


    public class DashboardViewModel
    {
        public string Username { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalInterns { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public List<DashboardLogListViewModel> AttendanceList { get; set; }
    }

    public class DashboardLogListViewModel
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string Status { get; set; }
        public string StatusClass { get; set; }
    }

}















