using System;
using System.Configuration;

namespace KarmaHangfire
{
    public static class CbrServices
    {
        public static void DownloadForeignExchange()
        {
            string connection = ConfigurationManager.AppSettings["karma_admin"];
            //KarmaSchedulerFunctions.CreateCbrForeignExchangeDownload(connection, new CbrForeignParam { DateTime = DateTime.Now });
        }
    }
}
