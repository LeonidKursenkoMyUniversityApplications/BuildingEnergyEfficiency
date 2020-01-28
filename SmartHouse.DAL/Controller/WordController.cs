using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Windows.Forms;
using SmartHouse.DAL.Model.Report;

namespace SmartHouse.DAL.Controller
{
    public class WordController : IDocumentController
    {
        //Create a missing variable for missing value  
        private object missing = Missing.Value;
        private Word.Application _wordApp;
        private Word.Document _document;
        private object _templatePath = Application.StartupPath + @"\data\Звіт.dotx";

        public Report Report { set; get; }

        public void InitDocument()
        {
            try
            {
                _wordApp = new Word.Application();
                _wordApp.Visible = false;
                _document = _wordApp.Documents.Add(ref _templatePath, ref missing, ref missing, ref missing);
            }
            catch (Exception e)
            {
                throw new Exception("Помилка при створенні документа. " + e.Message);
            }
        }

        public void SaveDocument(string fileName)
        {
            object path = fileName;
            try
            {
                _document.SaveAs(ref path);
                _document.Close(ref missing, ref missing, ref missing);
                Marshal.ReleaseComObject(_document);
                _wordApp.Quit(ref missing, ref missing, ref missing);
                Marshal.ReleaseComObject(_wordApp);
                MessageBox.Show("Документ було успішно створено!");
            }
            catch (Exception e)
            {
                throw new Exception("Помилка при збереженні документа. " + e.Message);
            }
        }

        public void DeleteTempFiles()
        {
            DirectoryInfo di = new DirectoryInfo(Report.ImgDirectory);
            foreach (FileInfo file in di.GetFiles())
            {
                if (file.Extension.Equals(".bmp"))
                    file.Delete();
            }
        }

        protected void ReplaceText(string label, object text)
        {
            _document.Content.Find.Execute(FindText: label, ReplaceWith: text);
        }

        protected void ReplaceTextAll(object label, object text)
        {
            object replaceAll = Word.WdReplace.wdReplaceAll;
            _document.Content.Find.Execute(ref label, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref text,
                ref replaceAll, ref missing, ref missing, ref missing, ref missing);
            //_document.Content.Find.Execute(FindText: label, ReplaceWith: text);
        }

        protected void AddChart(int chartNumber, string[,] data)
        {
            Word.InlineShape inlineShape = _document.Content.InlineShapes[chartNumber];

            object oRngTarget = _wordApp.Selection.Range;
            object oOLEClass = "Excel.Sheet.12";
            Word.InlineShape ils = _document.InlineShapes.AddOLEObject(ref oOLEClass, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref oRngTarget);
            Word.OLEFormat olef = ils.OLEFormat;
            
            //if (inlineShape.OLEFormat.ProgID == "Excel.Chart.12")
            //{
                //inlineShape.Activate();
                Excel.Workbook wb = (Excel.Workbook)olef.Object;
                Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];

                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        ((Excel.Range)ws.Cells[i + 1, j + 1]).Value = data[i, j];
                    }
                }
            //}
            //ReplaceText("$picture1.1$", inlineShape);
            Marshal.ReleaseComObject(ils);
        }

        protected string[,] ToData(string xLabel, string yLabel, List<DateTime> xs, List<int> ys)
        {
            string[,] data = new string[xs.Count + 1, 2];
            data[0, 0] = xLabel;
            data[0, 1] = yLabel;
            for (int i = 0; i < xs.Count; i++)
            {
                data[i + 1, 0] = xs[i].ToString(Report.DateTimeFormat);
                data[i + 1, 1] = ys[i].ToString();
            }
            return data;
        }

        protected void PasteImage(string imagePath, string bookmark)
        {
            Word.Range range = _document.Bookmarks.get_Item(bookmark).Range;
            range.Text = "";
            Word.InlineShape inlineShape = _document.InlineShapes.AddPicture(imagePath, Range: range);
            //inlineShape.Width = 400;
            inlineShape.Height = 250;
        }

        protected void PasteTable(List<List<string>> data, string bookmark)
        {
            Word.Range range = _document.Bookmarks.get_Item(bookmark).Range;
            range.Text = "";
            int rowsCount = data.Count;
            int colsCount = data[0].Count;
            Word.Table table = _document.Tables.Add(range, rowsCount, colsCount, ref missing, ref missing);
            table.Borders.Enable = 1;
            table.AllowAutoFit = true;
            table.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent);
            object styleTableText = "Table text";
            for (int r = 0; r < rowsCount; r++)
            {
                Word.Row row = table.Rows[r + 1];
                for (int c = 0; c < colsCount; c++)
                {
                    Word.Cell cell = row.Cells[c + 1];
                    cell.Range.set_Style(ref styleTableText);
                    //Header row  
                    if (cell.RowIndex == 1)
                    {
                        cell.Range.Text = data[r][c];
                        cell.Range.Font.Bold = 1;
                        cell.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        cell.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    }
                    //Data row  
                    else
                    {
                        cell.Range.Text = data[r][c];
                    }
                }
            }
        }

        protected void PasteText(string text, string bookmark)
        {
            Word.Range range = _document.Bookmarks.get_Item(bookmark).Range;
            range.Text = text;
        }
        
    }
}
