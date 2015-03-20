using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelTest
{
  

    class Program
    {
        static void Main(string[] args)
        {
            #region excel file test
            //string fname = @"e:\RG03_042_20140625.xlsx";

            //string nfile = @"E:\RG03_042_20150101.xlsx";

            //using (_3rd.DiaryExcelHelper de = new _3rd.DiaryExcelHelper(fname))
            //{
            //    de.RowToInsert = 9;

            //    de.NamePosition = "B3";
            //    de.DepartPosition = "C5";
            //    de.DatePosition = "G3";
            //    de.NumberPosition = "E3";

            //    DBModel.vDiarys diary = new DBModel.vDiarys();
            //    diary.name = "张鹏";
            //    diary.number = "042";
            //    diary.depart = "测试部门";
            //    diary.status = "完成";
            //    diary.increaseTime = "1工作日";
            //    diary.dtext = "今天啥也没干今天啥也没干今天啥也没干今天啥也没干今天啥也没干今天啥也没干今天啥也没干今天啥也没干";
            //    diary.item = "狗屁项目今天啥也没干今天啥也没干今天啥也没干今天啥也没干";
            //    diary.date = DateTime.Now;
            //    de.InsertItem(diary);

            //    de.SaveAs(nfile);
            //} 
            #endregion

            Console.WriteLine(DateTime.Now.ToString("yyyyMMdd"));

            

            string code1 = _3rd.Security.Encode("10463082");
            string code2 = _3rd.Security.Decode(code1);

            Console.WriteLine("10463082->{0}->{1}", code1, code2);

            Console.WriteLine("press any key to continue...");
            Console.ReadKey();
        }
    }
}
