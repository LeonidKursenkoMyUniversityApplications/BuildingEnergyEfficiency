using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHouse.DAL.Model;

namespace SmartHouse.BLL.Model
{
    public class HeatLosses
    {
        // Heat losses from different areas of house, kWht per hour.
        public double Walls { set; get; }
        public double Windows { set; get; }
        public double Ground { set; get; }
        public double Roof { set; get; }
        public double Total { set; get; }

        public DataTable GetDataTable
        {
            get
            {
                DataTable dt = new DataTable();
                DataColumn column = new DataColumn("Огороджуюча конструкція", typeof(string));
                //column.ReadOnly = true;
                dt.Columns.Add(column);

                column = new DataColumn("Тепловтрати при розрахунковій температурі, кВт·год", typeof(double));
                //column.ReadOnly = true;
                dt.Columns.Add(column);

                DataRow row = dt.NewRow();
                row[0] = Constants.OutsideWalls;
                row[1] = Walls;
                dt.Rows.Add(row);

                row = dt.NewRow();
                row[0] = Constants.Window;
                row[1] = Windows;
                dt.Rows.Add(row);

                row = dt.NewRow();
                row[0] = Constants.Ground;
                row[1] = Ground;
                dt.Rows.Add(row);

                row = dt.NewRow();
                row[0] = Constants.Roof;
                row[1] = Roof;
                dt.Rows.Add(row);

                row = dt.NewRow();
                row[0] = "Qсум";
                row[1] = Total;
                dt.Rows.Add(row);
                return dt;
            }
        }

        public List<CharacteristicHeatLosses> Characteristic { set; get; }
        public List<HeatConsumption> HeatConsumptions { set; get; }
        public double TotalHeatConsumption { set; get; }
        public double TotalHeatHelConsumption { set; get; }

        public List<TypeOfHeat> Heats { set; get; }
    }
}
