using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3rd
{


    public class DiaryExcelHelper : IDisposable
    {
        const string _sheetName = "个人工作日志";

        public int RowToInsert { get; set; }
        public string DatePosition { get; set; }
        public string NumberPosition { get; set; }
        public string DepartPosition { get; set; }
        public string NamePosition { get; set; }

        ExcelPackage _excelPackage;

        ExcelWorksheet _workSheet;

        public DiaryExcelHelper(string filename)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(filename);
            _excelPackage = new ExcelPackage(fi);
            _workSheet = _excelPackage.Workbook.Worksheets[_sheetName];
        }


        private void SetStaffName(string name)
        {
            _workSheet.SetValue(NamePosition, name);
        }

        private void SetDepart(string depart)
        {
            _workSheet.SetValue(DepartPosition, depart);
        }

        private void SetNumber(string staffNumber)
        {
            _workSheet.SetValue(NumberPosition, staffNumber);
        }

        private void SetDate(string date)
        {
            _workSheet.SetValue(DatePosition, date);
        }

        public void InsertItem(DBModel.vDiarys diary)
        {
            ExcelRange range;

            //项目
            _workSheet.InsertRow(RowToInsert, 4);
            _workSheet.Cells[RowToInsert, 2].Value = diary.item;
            range = _workSheet.Cells[RowToInsert, 2, RowToInsert + 4 - 1, 2];
            range.Merge = true;
            range.Style.WrapText = true;
            range.Style.ShrinkToFit = true;
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            var border = range.Style.Border;
            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

            //项目内容
            _workSheet.Cells[RowToInsert, 3].Value = diary.dtext;
            range = _workSheet.Cells[RowToInsert, 3, RowToInsert + 4 - 1, 5];
            range.Style.WrapText = true;
            range.Merge = true;
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            border = range.Style.Border;
            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;


            //完成状态
            _workSheet.Cells[RowToInsert, 6].Value = diary.status;
            range = _workSheet.Cells[RowToInsert, 6, RowToInsert + 4 - 1, 8];
            range.Merge = true;
            range.Style.WrapText = true;
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            border = range.Style.Border;
            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;


            //预计增时
            _workSheet.Cells[RowToInsert, 9].Value = diary.increaseTime;
            range = _workSheet.Cells[RowToInsert, 9, RowToInsert + 4 - 1, 9];
            range.Merge = true;
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            border = range.Style.Border;
            border.Bottom.Style = border.Top.Style = border.Left.Style = ExcelBorderStyle.Thin;
            border.Right.Style = ExcelBorderStyle.Thick;

            //左边线
            range = _workSheet.Cells[RowToInsert, 1, RowToInsert + 4 - 1, 1];
             border = range.Style.Border;
            border.Left.Style = ExcelBorderStyle.Thick;

            SetDate(diary.date.ToShortDateString());
            SetDepart(diary.depart);
            SetNumber("N" + diary.number);
            SetStaffName(diary.name);
        }

        public void Save()
        {
            _excelPackage.Save();
        }

        public void SaveAs(string filename)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(filename);
            _excelPackage.SaveAs(fi);
        }

        public void Dispose()
        {
            _excelPackage.Dispose();
        }
    }
}
