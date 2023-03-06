using ContextMapper;
using CORE_API.Models;
using CORE_BASE.CORE;
using CORE_MODELS;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace CORE_API.General
{
    public static class Common
    {
        public static void DateHandler(ref DateTime? date)
        {
            string dt = date.Value.TimeOfDay.ToString();
            if (dt == "19:00:00")
                date = Convert.ToDateTime(date.Value.AddDays(1).ToShortDateString());//.ToDateTime();
            else
                date = Convert.ToDateTime(date.Value.ToShortDateString());
        }
        public static DataTable ExcelPackageToDataTable(ExcelPackage excelPackage)
        {
            DataTable dt = new DataTable();
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[1];

            //check if the worksheet is completely empty
            if (worksheet.Dimension == null)
            {
                return dt;
            }

            //create a list to hold the column names
            List<string> columnNames = new List<string>();

            //needed to keep track of empty column headers
            int currentColumn = 1;

            //loop all columns in the sheet and add them to the datatable
            foreach (var cell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
            {
                string columnName = cell.Text.Trim();

                //check if the previous header was empty and add it if it was
                if (cell.Start.Column != currentColumn)
                {
                    columnNames.Add("Header_" + currentColumn);
                    dt.Columns.Add("Header_" + currentColumn);
                    currentColumn++;
                }

                //add the column name to the list to count the duplicates
                columnNames.Add(columnName);

                //count the duplicate column names and make them unique to avoid the exception
                //A column named 'Name' already belongs to this DataTable
                int occurrences = columnNames.Count(x => x.Equals(columnName));
                if (occurrences > 1)
                {
                    columnName = columnName + "_" + occurrences;
                }

                //add the column to the datatable
                dt.Columns.Add(columnName);

                currentColumn++;
            }

            //start adding the contents of the excel file to the datatable
            for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
            {
                var row = worksheet.Cells[i, 1, i, worksheet.Dimension.End.Column];
                DataRow newRow = dt.NewRow();

                //loop all cells in the row
                foreach (var cell in row)
                {
                    newRow[cell.Start.Column - 1] = cell.Text;
                }

                dt.Rows.Add(newRow);
            }

            return dt;
        }

        public static string GenerateCifNumber(string previousNumber)
        {
            var lastAddedCifNumber = int.Parse(previousNumber);
            var newCifNumber = Convert.ToString(++lastAddedCifNumber);

            return newCifNumber;
        }


        public static DataTable ExcelSheetsToDataTable(ExcelPackage excelPackage)
        {
            DataTable dt = new DataTable();
            foreach (var worksheet in excelPackage.Workbook.Worksheets)
            {

                //check if the worksheet is completely empty
                if (worksheet.Dimension == null)
                {
                    return dt;
                }

                //create a list to hold the column names
                List<string> columnNames = new List<string>();

                //needed to keep track of empty column headers
                int currentColumn = 1;

                //loop all columns in the sheet and add them to the datatable
                foreach (var cell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    string columnName = cell.Text.Trim();

                    //check if the previous header was empty and add it if it was
                    if (cell.Start.Column != currentColumn)
                    {
                        columnNames.Add("Header_" + currentColumn);
                        dt.Columns.Add("Header_" + currentColumn);
                        currentColumn++;
                    }

                    //add the column name to the list to count the duplicates
                    columnNames.Add(columnName);

                    //count the duplicate column names and make them unique to avoid the exception
                    //A column named 'Name' already belongs to this DataTable
                    int occurrences = columnNames.Count(x => x.Equals(columnName));
                    if (occurrences > 1)
                    {
                        columnName = columnName + "_" + occurrences;
                    }

                    //add the column to the datatable
                    dt.Columns.Add(columnName);

                    currentColumn++;
                }

                //start adding the contents of the excel file to the datatable
                for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                {
                    var row = worksheet.Cells[i, 1, i, worksheet.Dimension.End.Column];
                    DataRow newRow = dt.NewRow();

                    //loop all cells in the row
                    foreach (var cell in row)
                    {
                        newRow[cell.Start.Column - 1] = cell.Text;
                    }

                    dt.Rows.Add(newRow);
                }
            }
            return dt;
        }
        public static async Task<Tuple<bool, T>> PostAsync<T>(Uri baseAddress, string action, object data, int timeoutInSeconds = 40)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(timeoutInSeconds);
                    httpClient.BaseAddress = baseAddress;
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    //Create the request.
                    HttpResponseMessage response;

                    string json = JsonConvert.SerializeObject(data);
                    HttpContent content = new StringContent(json);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    response = await httpClient.PostAsync(action, content);


                    return await ProcessResponse<T>(response);
                }
            }
            catch (HttpRequestException ex)
            {
                return new Tuple<bool, T>(false, default(T));
            }
            catch (TaskCanceledException ex)
            {

                return new Tuple<bool, T>(false, default(T));
            }


        }
        private static async Task<Tuple<bool, T>> ProcessResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode || (response.StatusCode == System.Net.HttpStatusCode.Created))
            {
                return new Tuple<bool, T>(true, await JsonDeserialize<T>(response));
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {

                throw new Exception(await response.Content.ReadAsStringAsync());
            }

            return new Tuple<bool, T>(false, await JsonDeserialize<T>(response));
        }
        public static async Task<T> JsonDeserialize<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<T>(json);

            return deserialized;
        }

        public static Status GetStatus(string status)
        {
            return new BLInfo<Status>().GetQuerable<Status>().Where(a => a.Name.Equals(status.ToString())).FirstOrDefault();
        }

        public static StatusType GetStatusType(string statusType)
        {
            return new BLInfo<StatusType>().GetQuerable<StatusType>().Where(a => a.Name.Equals(statusType.ToString())).FirstOrDefault();
        }

        public static List<Status> GetStatusByTypeId(int typeId)
        {
            return new BLInfo<Status>().GetQuerable<Status>().Where(a => a.TypeId == typeId).ToList();
        }


        public static bool SubmitLog(LogBindingModel Logs)
        {

            dbEngine db = new dbEngine();
            var log = new Log();
            log.Action = Logs.Action;
            log.Error = Logs.Error;
            log.ActionValue = Logs.ActionValue;
            log.Module = Logs.Module;
            log.Date = DateTime.Now;
            log.DeviceName = "Web";

            db._dbContext.Logs.InsertOnSubmit(log);
            db._dbContext.SubmitChanges();

            return true;
        }

    }
}
