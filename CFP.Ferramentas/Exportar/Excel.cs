using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CFP.Ferramentas.Exportar
{
    public class Excel<T>
    {
        public string ExcelExport(System.Data.DataTable DT, string title)
        {
            try
            {
                // Create Excel
                Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                Workbook ExcelBook = ExcelApp.Workbooks.Add(Type.Missing);
                // Create a worksheet (that is, a subsheet in Excel) 1 to represent data export in the subsheet 1
                Worksheet ExcelSheet = (Worksheet)ExcelBook.Worksheets[1];
                // If there is a numeric type in the data, it can be displayed in a different text format.
                ExcelSheet.Cells.NumberFormat = "@";
                // Setting the name of the worksheet
                ExcelSheet.Name = title.ToString();
                // Setting Sheet Title
                string start = "A1";
                string end = ChangeASC(DT.Columns.Count) + "1";
                Range _Range = (Range)ExcelSheet.get_Range(start, end);
                _Range.Merge(0); // cell merge action (designed with get_Range() above)
                _Range = ExcelSheet.get_Range(start, end);
                _Range.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                _Range.Font.Size = 22; // Set font size
               // _Range.Font.Name = Song Style; // Types of fonts set 
                ExcelSheet.Cells[1, 1] = title; //Excel cell assignment
                _Range.EntireColumn.AutoFit(); // Auto-adjust column widths
                // write the header.
                for (int m = 1; m <= DT.Columns.Count; m++)
                {
                    ExcelSheet.Cells[2, m] = DT.Columns[m - 1].ColumnName.ToString();
                    start = "A2";
                    end = ChangeASC(DT.Columns.Count) + "2";
                    _Range = ExcelSheet.get_Range(start, end);
                    _Range.Font.Size = 14; // Set font size
                    //_Range.Font.Name = Song Style; // Types of fonts set  
                    _Range.EntireColumn.AutoFit(); // Auto-adjust column widths 
                    _Range.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                }
                // write data
                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    for (int j = 1; j <= DT.Columns.Count; j++)
                    {
                        // Excel cells begin with index 1
                        // if (j == 0) j = 1;
                        ExcelSheet.Cells[i + 3, j] = DT.Rows[i][j - 1].ToString();
                    }
                }
                // Table property settings
                for (int n = 0; n < DT.Rows.Count + 1; n++)
                {
                    start = "A" + (n + 3).ToString();
                    end = ChangeASC(DT.Columns.Count) + (n + 3).ToString();
                    // Get Excel multiple cell areas
                    _Range = ExcelSheet.get_Range(start, end);
                    _Range.Font.Size = 12; // Set font size
                    //_Range.Font.Name = Song Style; // Types of fonts set
                    _Range.EntireColumn.AutoFit(); // Auto-adjust column widths
                    _Range.HorizontalAlignment = XlHAlign.xlHAlignCenter; //设置字体在单元格内的对其方式 _ Range. EntireColumn. AutoFit (); // Auto-adjust column widths 
                }
                ExcelApp.DisplayAlerts = false; // Save Excel without popping up a window to save it directly 
                //// Pop up the save dialog box and save the file
                Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                sfd.DefaultExt = ".xlsx";
                sfd.Filter = "Documento Excel |*.xlsx | Office 2000-2003 File |*.xls | All Files |*.*";
                if (sfd.ShowDialog() == true)
                {
                    if (sfd.FileName != "")
                    {
                        ExcelBook.SaveAs(sfd.FileName); // Save it to the specified path
                        MessageBox.Show("Arquivo exportado para:" + sfd.FileName, "Sucesso", MessageBoxButton.OK, MessageBoxImage.None);
                    }
                }
                // The process of release that may not have been released
                ExcelBook.Close();
                ExcelApp.Quit();
                return sfd.FileName;
            }
            catch
            {
                MessageBox.Show("Falha na exportação! Por favor entre em contato com o Suporte Técnico.");
                return null;
            }
        }

        private string ChangeASC(int count)
        {
            string ascstr = "";
            switch (count)
            {
                case 1:
                    ascstr = "A";
                    break;
                case 2:
                    ascstr = "B";
                    break;
                case 3:
                    ascstr = "C";
                    break;
                case 4:
                    ascstr = "D";
                    break;
                case 5:
                    ascstr = "E";
                    break;
                case 6:
                    ascstr = "F";
                    break;
                case 7:
                    ascstr = "G";
                    break;
                case 8:
                    ascstr = "H";
                    break;
                case 9:
                    ascstr = "I";
                    break;
                case 10:
                    ascstr = "J";
                    break;
                case 11:
                    ascstr = "K";
                    break;
                case 12:
                    ascstr = "L";
                    break;
                case 13:
                    ascstr = "M";
                    break;
                case 14:
                    ascstr = "N";
                    break;
                case 15:
                    ascstr = "O";
                    break;
                case 16:
                    ascstr = "P";
                    break;
                case 17:
                    ascstr = "Q";
                    break;
                case 18:
                    ascstr = "R";
                    break;
                case 19:
                    ascstr = "S";
                    break;
                case 20:
                    ascstr = "T";
                    break;
                default:
                    ascstr = "U";
                    break;
            }
            return ascstr;
        }
        public System.Data.DataTable ConvertToDataTable(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            System.Data.DataTable table = new System.Data.DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }

        public System.Data.DataTable ToDataTable(IList<T> list)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            System.Data.DataTable table = new System.Data.DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in list)
            {
                for (int i = 0; i < values.Length; i++)
                    values[i] = props[i].GetValue(item) ?? DBNull.Value;
                table.Rows.Add(values);
            }
            return table;
        }
    }
}
