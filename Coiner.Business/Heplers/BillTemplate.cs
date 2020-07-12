using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Coiner.Business.Heplers
{
    public class BillTemplate
    {
        public static string CoinerLastName = "{CoinerLastName}";
        public static string CoinerAdress = "{CoinerAdress}";
        public static string BackerLastName = "{BackerLastName}";
        public static string BackerFirstName = "{BackerFirstName}";
        public static string BackerEmail = "{BackerEmail}";
        public static string BillId = "{BillId}";
        public static string BillCreationDate = "{BillCreationDate}";
        public static string BillDescription = "{BillDescription}";
        public static string Acquisition = "{Acquisition}";
        public static string ProductTVA = "{ProductTVA}";
        public static string TVA = "{TVA}";
        public static string HT = "{HT}";
        public static string TTC = "{TTC}";

        public static string BillContent()
        {
            var path = HostingEnvironment.WebRootPath + "/Content/billTemplate/bill-template.html";

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), path.Replace("/", "\\"))
                                  .Replace("\\", "/");

            return File.ReadAllText(filePath);
        }
    }
}
