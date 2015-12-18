using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Office.Tools.Excel;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using System.Net;

namespace ReadingList
{
    public partial class ThisWorkbook
    {
        const string grKey = "HQ2Bd6VkF1VOJYsYmfyiRA";
        const string grSearchString = "http://goodreads.com/search.xml/key={0}&q={1}&page={2}";
        private const int titleColumn = 1;
        private const int authorColumn = 2;
        private const int ratingColumn = 3;

        private void InitializeButtons(Worksheet sheet)
        {
            var range = sheet.get_Range("G2:G3", Type.Missing);

            // Fake Button
            CreateButton(sheet, "Column1 + 1", range, (o1, e1) =>
            {
                sheet.Rows.ClearFormats();
                var totalRows = sheet.UsedRange.Rows.Count;

                for (int i = 2; i <= totalRows; i++)
                {
                    sheet.Cells[i, 1] = sheet.Cells[i, 1].Value + "1";
                }

            });

            // Add book button
            CreateButton(sheet, "Add Book", sheet.get_Range("G5:G6"), (o1, e1) =>
            {
                sheet.Rows.ClearFormats();
                var totalRows = sheet.UsedRange.Rows.Count;

                using (var form = new Search())
                {
                    form.ShowDialog();
                    if (form.HasResult)
                    {
                        sheet.Cells[totalRows + 1, titleColumn] = form.ResultTitle;
                        sheet.Cells[totalRows + 1, authorColumn] = form.ResultAuthor;
                        sheet.Cells[totalRows + 1, ratingColumn] = form.ResultRating.ToString();
                        sheet.Columns.AutoFit();
                    }
                }
                //var httpRequest = (HttpWebRequest)WebRequest.Create(
            });
        }

        private static void CreateButton(Worksheet sheet, string displayText, Excel.Range location, Action<object, EventArgs> action)
        {
            var button = new Button();
            button.Text = displayText;

            button.Click += new EventHandler(action);

            sheet.Controls.AddControl(button, location, displayText);
        }

        private void ValidateWorksheetFormat(object sender, EventArgs e)
        {
            var sheet = GetActiveSheet();

            // TODO: Validate cell contents and prompt to fix/normalize format of document
        }

        private Worksheet GetActiveSheet()
        {
            var comSheet = Globals.ThisWorkbook.Application.ActiveSheet;
            var sheet = Globals.Factory.GetVstoObject(comSheet);

            return sheet;
        }

        private int GetColumnLength(Worksheet sheet, string column)
        {
            sheet.Columns.ClearFormats();
            sheet.Rows.ClearFormats();
            return sheet.UsedRange.Rows.Count;
        }

        private void ThisWorkbook_Startup(object sender, System.EventArgs e)
        {
            var workbook = (Microsoft.Office.Tools.Excel.Workbook)sender;

            foreach (Microsoft.Office.Interop.Excel.Worksheet comSheet in workbook.Worksheets)
            {
                var sheet = Globals.Factory.GetVstoObject(comSheet);
                InitializeButtons(sheet);
            }
        }

        private void ThisWorkbook_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisWorkbook_Startup);
            this.Shutdown += new System.EventHandler(ThisWorkbook_Shutdown);
        }

        #endregion

    }
}
