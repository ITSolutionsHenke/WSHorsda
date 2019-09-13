using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using ZPF;
using ZPF.SQL;

namespace wsStockAPPro.Controllers
{
   public class ToolsController : Controller
   {
      [AllowCrossSiteJson]
      [Route("~/HorsDA/tools/now")]
      [HttpGet]
      public string GetNow()
      {
         string lang = "";

         return GetNow(lang);
      }

      [AllowCrossSiteJson]
      [Route("~/HorsDA/tools/now/{lang}")]
      [HttpGet]
      public string GetNow([FromRoute] string lang = "")
      {
         var userLangs = Request.Headers["Accept-Language"].ToString();

         var firstLang = userLangs.Split(',').FirstOrDefault();
         firstLang = firstLang.Split('-').FirstOrDefault().ToLower();

         var defaultLang = string.IsNullOrEmpty(firstLang) ? "en" : firstLang;
         if (("*en*de*fr*").IndexOf(lang.ToLower()) > 0) defaultLang = lang.ToLower();

         switch (defaultLang)
         {
            case "de": return DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"); break;
            case "fr": return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); break;

            default:
            case "en": return DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"); break;
         };
      }

      [AllowCrossSiteJson]
      [Route("~/HorsDA/Horse/GetList/{OwnerID}")]
      [HttpGet]
      public string GetListHorse([FromHeader]string authorization, [FromRoute] string OwnerID = "")
      {
         MainViewModel.Current.Prologue(Request);

         if (!string.IsNullOrEmpty(authorization) && MainViewModel.Current.CheckAuthorization(authorization))
         {
            MainViewModel.Current.TestUnitaires = false;

                //var list = DB_SQL.Query<ItsHenke.HorsDaMessages.Bewegungsdaten.Horse.Horse.HorseMessage>
                //   ($"select * from Horse where OwnerID = '{OwnerID}'");

                //return Newtonsoft.Json.JsonConvert.SerializeObject(list);
                return "todo";
         }
         else
         {
            return null;
         };
      }

        //[AllowCrossSiteJson]
        [Route("~/HorsDA/contact/insert")]
        [HttpPost]
        public string PostContact([FromHeader]string authorization)
        {
            if (!string.IsNullOrEmpty(authorization) && MainViewModel.Current.CheckAuthorization(authorization))
            {
                string json = MainViewModel.Current.GetContent(HttpContext.Request);
                //return wsViewModel.Current.PostContact(json);

                return "holla";
            }
            else
            {
                return null;
            };
        }

        [AllowCrossSiteJson]
      [Route("~/HorsDA/tools/TestDB")]
      [HttpGet]
      public string GetTestDB()
      {
         string Result = "";

         MainViewModel.Current.Prologue(Request);

         try
         {
            if (!string.IsNullOrEmpty(MainViewModel.Current.Connection.LastError))
            {
               return MainViewModel.Current.Connection.LastError;
            };
         }
         catch (Exception ex)
         {
            return "(1) " + ex.Message;
         };

         if (MainViewModel.Current.Connection.DbConnection.State != System.Data.ConnectionState.Open)
         {
            MainViewModel.Current.Connection.CheckConnection();
         };

         Result = DB_SQL.QuickQuery("SELECT @@VERSION ;");

         if (!string.IsNullOrEmpty(MainViewModel.Current.Connection.LastError))
         {
            return MainViewModel.Current.Connection.LastError;
         };

         return Result;
      }

      [Route("~/HorsDA/tools/Config")]
      [HttpGet]
      public string GetConfig([FromHeader]string authorization)
      {
         MainViewModel.Current.Prologue(Request);

         if (!string.IsNullOrEmpty(authorization) && MainViewModel.Current.CheckAuthorization(authorization))
         {
            MainViewModel.Current.TestUnitaires = false;

            TStrings config = DB_SQL.QuickQueryList("select DBList.Param as Name, DBList.Value from DBList where DBList.List='Config'");

            return config.Text;
         }
         else
         {
            return null;
         };
      }

   }
}
