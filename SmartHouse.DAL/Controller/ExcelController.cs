using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using SmartHouse.DAL.Model;
using Excel = Microsoft.Office.Interop.Excel;

namespace SmartHouse.Controller
{
    public class ExcelController
    {
        public List<Weather> GetWeathers(string fileName, int pageIndex = 1)
        {
            string[,] data = Read(fileName, 5, pageIndex);
            Regex pattern = new Regex(@"(?<year>\d+)-(?<month>\d+)");
            Match match = pattern.Match(fileName);
            int year = int.Parse(match.Groups["year"].Value);
            int month = int.Parse(match.Groups["month"].Value);
            pattern = new Regex(@"(?<hour>\d+):(?<minute>\d+)");
            List<Weather> weathers = new List<Weather>();
            for (int i = 1; i < data.GetLength(0); i++)
            {
                int day = int.Parse(data[i, 0]);
                match = pattern.Match(data[i, 1]);
                int hour = int.Parse(match.Groups["hour"].Value);
                int minute = int.Parse(match.Groups["minute"].Value);
                int temperature = 0;
                if(int.TryParse(data[i, 2], out temperature) == false)
                    temperature = weathers[weathers.Count - 1].Temperature;
                string windDirection = string.IsNullOrEmpty(data[i, 3]) ? weathers[weathers.Count - 1].WindDirection : data[i, 3];
                int windPower = 0;
                if (int.TryParse(data[i, 4], out windPower) == false)
                    windPower = weathers[weathers.Count - 1].WindPower;

                weathers.Add(new Weather
                {
                    Date = new DateTime(year, month, day, hour, minute, 0),
                    Temperature = temperature,
                    WindDirection = windDirection,
                    WindPower = windPower
                });
            }
            return weathers;
        }

        public List<Sun> GetSunConditions(string fileName, int pageIndex = 1)
        {
            string[,] data = Read(fileName, 3, pageIndex);
            List<Sun> sunConditions = new List<Sun>();
            for (int i = 2; i < data.GetLength(0); i++)
            {
                if (data[i, 1].Equals("24:00")) data[i, 1] = "00:00";
                string dateString = $"{data[i, 0]} {data[i, 1]}";
                DateTime date;
                try
                {

                    date = DateTime.ParseExact(dateString, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
                }
                catch
                {
                    break;
                }
                int insolation = 0;
                if (int.TryParse(data[i, 2], out insolation) == false)
                    insolation = sunConditions[sunConditions.Count - 1].Insolation;

                sunConditions.Add(new Sun
                {
                    Date = date,
                    Insolation = insolation
                });
            }
            return sunConditions;
        }

        private string[,] Read(string fileName, int cols, int pageIndex = 1)
        {
            string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName +
                                ";Extended Properties=Excel 12.0;";
            System.Data.DataTable dt = null;
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt == null) return null;
                string[] excelSheets = new String[dt.Rows.Count];
                int k = 0;
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[k] = row["TABLE_NAME"].ToString();
                    k++;
                }
                string sheetName = excelSheets[0];
                OleDbDataAdapter objDA = new System.Data.OleDb.OleDbDataAdapter
                    ($"select * from [{sheetName}]", conn);
                DataSet excelDataSet = new DataSet();
                objDA.Fill(excelDataSet);
                System.Data.DataTable table = excelDataSet.Tables[0];
                int columnsCount = cols;
                int rowsCount = table.Rows.Count;
                string[,] data = new string[table.Rows.Count, cols];
                for (int i = 0; i < rowsCount; i++)
                {
                    for (int j = 0; j < columnsCount; j++)
                    {
                        data[i, j] = table.Rows[i][j].ToString();
                    }
                }
                return data;
            }
        }

        public string[,] Read2(string fileName,  int cols, int pageIndex = 1)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(System.IO.Path.GetFullPath(fileName));
            Excel._Worksheet xlWorksheet = (Excel._Worksheet)xlWorkbook.Sheets[pageIndex];
            Excel.Range xlRange = xlWorksheet.UsedRange;
            string[,] data;
            try
            {
                int columnsCount = cols;
                int rowsCount = ((Excel.Range)xlRange.Cells[xlRange.Rows.Count, 1]).get_End(Excel.XlDirection.xlUp).Row; //xlRange.Cells[xlRange.Rows.Count, 1].End(Excel.XlDirection.xlUp).Row;

                if (rowsCount == 1) rowsCount = xlRange.Rows.Count;
                data = new string[rowsCount, columnsCount];

                for (int i = 1; i <= rowsCount; i++)
                {
                    for (int j = 1; j <= columnsCount; j++)
                    {
                        data[i - 1, j - 1] = ((Excel.Range)xlRange.Cells[i, j]).Text.ToString();
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Помилка при зчитуванні даних з файлів. " + exception.Message);
            }
            finally
            {
                //cleanup
                GC.Collect();
                GC.WaitForPendingFinalizers();

                //release com objects to fully kill excel process from running in the background
                Marshal.ReleaseComObject(xlRange);
                Marshal.ReleaseComObject(xlWorksheet);

                //close and release
                xlWorkbook.Close();
                Marshal.ReleaseComObject(xlWorkbook);

                //quit and release
                xlApp.Quit();
                Marshal.ReleaseComObject(xlApp);
            }
            return data;
        }

        public void Write(string[,] data, string fileName, int pageIndex = 1)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook;
            if (File.Exists(Path.GetFullPath(fileName)))
                xlWorkbook = xlApp.Workbooks.Open(Path.GetFullPath(fileName));
            else
            {
                xlWorkbook = xlApp.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
                xlWorkbook.SaveAs(Path.GetFullPath(fileName));
            }
            if (xlWorkbook.Sheets.Count < pageIndex)
            {
                xlWorkbook.Sheets.Add(After: xlWorkbook.Sheets[xlWorkbook.Sheets.Count]);
            }
            Excel._Worksheet xlWorksheet = (Excel._Worksheet)xlWorkbook.Sheets[pageIndex];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int columnsCount = data.GetLength(1);
            int rowsCount = data.GetLength(0);

            for (int i = 1; i <= rowsCount; i++)
            {
                for (int j = 1; j <= columnsCount; j++)
                {
                    double val = 0;
                    if (Double.TryParse(data[i - 1, j - 1], out val) == true)
                    {
                        ((Excel.Range)xlRange.Cells[i, j]).EntireColumn.NumberFormat = "@";
                        if (i > 1 && j > 1)
                            ((Excel.Range)xlRange.Cells[i, j]).Value = Convert.ToDouble(data[i - 1, j - 1]);
                    }
                    else
                    {
                        ((Excel.Range)xlRange.Cells[i, j]).Value = data[i - 1, j - 1];
                    }

                }
            }

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Save();
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
        }
    }
}
