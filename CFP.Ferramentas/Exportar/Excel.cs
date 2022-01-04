using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CFP.Ferramentas.Exportar
{
    public class Excel<T>
    {
        //public static void ExportDataToExcel(List<T> result)
        //{
        //    var excel = new Microsoft.Office.Interop.Excel.Application();
        //    var excelworkBook = excel.Workbooks.Add();
        //    var excelSheet = (Worksheet)excelworkBook.ActiveSheet;
        //    excelSheet.Name = "DataSheet";

        //    try
        //    {
        //        //create the column(s) header(s)
        //        int col = 1;

        //        foreach (var propInfo in result[0].GetType().GetProperties())
        //        {
        //            excelSheet.Cells[1, col] = propInfo.Name;
        //            excelSheet.Cells[1, col].Font.Bold = true;
        //            col++;
        //        }

        //        //put the actual data
        //        int k = 0;

        //        foreach (var item in result)
        //        {
        //            int j = 1;
        //            foreach (var propInfo in item.GetType().GetProperties())
        //            {
        //                try
        //                {
        //                    excelSheet.Cells[k + 2, j].Value = propInfo.GetValue(item);
        //                    j++;
        //                }
        //                catch (System.Runtime.InteropServices.COMException comex)
        //                {
        //                    Console.WriteLine(string.Format("{0},caused for exporting value - {1}",
        //                        comex.Message, propInfo.GetValue(item)));
        //                    excelSheet.Cells[k + 2, j].Value = $"'{propInfo.GetValue(item)}'";
        //                    j++;
        //                    continue;
        //                }
        //            }
        //            k++;
        //        }
        //        var folderPath = @"D:\PastaExcel";
        //        if (!Directory.Exists(folderPath))
        //            Directory.CreateDirectory(folderPath);
        //        var filePath = $"{folderPath}\\Teste.xlsx";
        //        excelworkBook.Close(true, filePath);
        //        MessageBox.Show($"Exported Successfully to {filePath}");
        //    }
        //    catch (Exception ex)
        //    {
        //        excelworkBook.Close(false);
        //        MessageBox.Show(ex.Message);
        //        MessageBox.Show("Export Failed.");
        //    }
        //}
    }
}
