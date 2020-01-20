using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace SATI.Helpers
{
    public class DataImport
    {
        private static string GetExcelFileForArea(string area)
        {
            string uploadFolder = HostingEnvironment.MapPath("~/Uploads/Admin");
            if (string.IsNullOrEmpty(uploadFolder))
                return null;
            string[] files = System.IO.Directory.GetFiles(uploadFolder, "*" + area + ".xls*", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                return null;
            return files[0];
        }

        private static void ImportWorkbook<T>(string workbook)
        {
            //TODO move this method to ImportRepository...
         
            Type impType = typeof(T);
            var impTypeProps = impType.GetProperties().Skip(1); //skip ID column
        }

        public static void ImportFromExcel()
        {

        }

    }
}