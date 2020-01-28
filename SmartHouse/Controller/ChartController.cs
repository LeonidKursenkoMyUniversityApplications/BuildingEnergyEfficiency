using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;
using SmartHouse.BLL.Model;
using SmartHouse.DAL.Model;

namespace SmartHouse.Controller
{
    public class ChartController
    {
        public static void Clear(Chart chart)
        {
            chart.Series.Clear();
            chart.ChartAreas.Clear();
            chart.ChartAreas.Add(new ChartArea("Default"));
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gainsboro;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gainsboro;
            chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
        }

        public static void AddSeries(Chart chart, string seriesName, SeriesChartType type, Color color)
        {
            chart.Series.Add(new Series(seriesName));
            chart.Series[seriesName].ChartArea = "Default";
            chart.Series[seriesName].ChartType = type;
            chart.Series[seriesName].Color = color;
        }

        public static void Fill<TX, TY>(Chart chart, string seriesName, List<TX> xs, List<TY> ys)
        {
            chart.Series[seriesName].Points.DataBindXY(xs, ys);
        }

        public static void Fill<TY>(Chart chart, string seriesName, List<DateTime> xs, List<TY> ys)
        {
            //for (int i = 0; i < xs.Count; i++)
            //{
            //    chart.Series[seriesName].Points.AddXY(xs[i].ToOADate(), ys[i]);
            //}
            //chart.Series[seriesName].XValueType = ChartValueType.DateTime;
            chart.Series[seriesName].Points.DataBindXY(xs, ys);
            if (xs.Count <= 48)
            {
                chart.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
            }
            else
            {
                chart.ChartAreas[0].AxisX.LabelStyle.Format = "dd.MM.yyyy HH:mm";
            }
            chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
        }

        public static void Fill(Chart chart, string seriesName, List<Period> xs, double y)
        {
            chart.Series[seriesName].XValueType = ChartValueType.DateTime;
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm:ss";
            chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
            chart.ChartAreas[0].AxisX.Minimum = (new DateTime(2018, 10, 1, 0, 0, 0)).ToOADate();
            chart.ChartAreas[0].AxisX.Maximum = (new DateTime(2018, 10, 2, 0, 0, 0)).ToOADate();
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Maximum = y;
            if (xs.Count == 0)
            {
                chart.Series[seriesName].Points.AddXY(new DateTime(2018, 10, 1, 0, 0, 0), 0);
                return;
            }
            for (int i = 0; i < xs.Count; i++)
            {
                chart.Series[seriesName].Points.AddXY(xs[i].Start, 0);
                chart.Series[seriesName].Points.AddXY(xs[i].Start, y);
                chart.Series[seriesName].Points.AddXY(xs[i].End, y);
                chart.Series[seriesName].Points.AddXY(xs[i].End, 0);
            }
            DateTime start = xs.Select(x => x.Start).Min();
            start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
            chart.ChartAreas[0].AxisX.Minimum = start.ToOADate();

            DateTime end = xs.Select(x => x.End).Max();
            end = new DateTime(end.Year, end.Month, end.Day + 1, 0, 0, 0);
            chart.ChartAreas[0].AxisX.Maximum = end.ToOADate();

            
        }

        public static void Fill(Chart chart, string seriesName, List<ElectricalLoad> loads)
        {
            chart.Series[seriesName].XValueType = ChartValueType.DateTime;
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm:ss";
            chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
            chart.ChartAreas[0].AxisX.Minimum = (new DateTime(2018, 10, 1, 0, 0, 0)).ToOADate();
            chart.ChartAreas[0].AxisX.Maximum = (new DateTime(2018, 10, 2, 0, 0, 0)).ToOADate();
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Maximum = 1;
            if (loads.Count == 0)
            {
                chart.Series[seriesName].Points.AddXY(new DateTime(2018, 10, 1, 0, 0, 0), 0);
                return;
            }
            chart.ChartAreas[0].AxisY.Maximum = loads.Select(x => x.Power).Max();
            for (int i = 0; i < loads.Count; i++)
            {
                chart.Series[seriesName].Points.AddXY(loads[i].Start, 0);
                chart.Series[seriesName].Points.AddXY(loads[i].Start, loads[i].Power);
                chart.Series[seriesName].Points.AddXY(loads[i].End, loads[i].Power);
                chart.Series[seriesName].Points.AddXY(loads[i].End, 0);
            }
            DateTime start = loads.Select(x => x.Start).Min();
            start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
            chart.ChartAreas[0].AxisX.Minimum = start.ToOADate();

            DateTime end = loads.Select(x => x.End).Max();
            end = new DateTime(end.Year, end.Month, end.Day + 1, 0, 0, 0);
            chart.ChartAreas[0].AxisX.Maximum = end.ToOADate();
        }
        
        public static void AxisTitles(Chart chart, string xTitle, string yTitle)
        {
            chart.ChartAreas[0].AxisX.Title = xTitle;
            chart.ChartAreas[0].AxisY.Title = yTitle;
        }

        public static void Сhart_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;

            try
            {
                if (e.Delta < 0) // Scrolled down.
                {
                    xAxis.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0) // Scrolled up.
                {
                    var xMin = xAxis.ScaleView.ViewMinimum;
                    var xMax = xAxis.ScaleView.ViewMaximum;

                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;

                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                }
            }
            catch { }
        }

        public static void SaveImage(Chart chart, string path)
        {
            chart.Width = 900;
            chart.Height = 600;
            chart.SaveImage(path, ChartImageFormat.Bmp);
        }
    }
}
