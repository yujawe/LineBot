using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace LineChatBot
{
    public class editgooglesheet
    {
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
	    static string ApplicationName = "Update Google Sheet Data with Google Sheets API v4";
	    static String spreadsheetId = "1k-XXe15yv0PrSvM3BTCLbe_Ilyz2CPiTLXfswCoyIHo";
	    static string sheetName = "LineBot Data";
        public static SheetsService OpenSheet()
        {
            UserCredential credential;
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");

                //存儲憑證到credPath
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            //建立一個API服務，設定請求參數
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return service;
        }
        public static void AppendRow(SheetsService service)
        {
            String range = sheetName + "!A1";
            List<object> list1 = new List<object>() { "Item", "Cost", "Stocked", "Ship Date" };
            List<object> list2 = new List<object>() { "Wheel", "$20.50", "4", "3/1/2016" };
            List<object> list3 = new List<object>() { "Door", "$15", "2", "3/15/2016" };
            List<object> list4 = new List<object>() { "Engine", "$100", "1", "30/20/2016" };
            List<object> list5 = new List<object>() { "Totals", "=SUM(B2:B4)", "=SUM(C2:C4)", "=MAX(D2:D4)" };
            IList<IList<Object>> list = new List<IList<Object>>() { list1, list2, list3, list4, list5 };

            ValueRange VRange = new ValueRange();
            VRange.Range = range;
            VRange.Values = list;

            SpreadsheetsResource.ValuesResource.AppendRequest upd
                = service.Spreadsheets.Values.Append(VRange, spreadsheetId, range);
            upd.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            AppendValuesResponse responses = upd.Execute();
            Console.WriteLine(responses.Updates.UpdatedRange);
        }
    }
}