using Microsoft.AspNetCore.Http;
using System;
using System.Data.Common;
using System.IO;
using System.Text;
using ZPF.AT;
using ZPF.SQL;

namespace ZPF
{
   public partial class MainViewModel : BaseViewModel
   {
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      static MainViewModel _Instance = null;

      public bool TestUnitaires { get; set; }

      public static MainViewModel Current
      {
         get
         {
            if (_Instance == null)
            {
               _Instance = new MainViewModel();
            };

            return _Instance;
         }

         set
         {
            _Instance = value;
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -


      public MainViewModel()
      {
         _Instance = this;
         TestUnitaires = false;
         webServer = new WebServer();

         webServer.UserName = "ws";
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public void Prologue(HttpRequest request)
      {
         OpenDB(request);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public void OpenDB(HttpRequest request)
      {
         if (DB_SQL._ViewModel == null || !DB_SQL._ViewModel.CheckConnection())
         {
            string ConnectionString = GetConnectionString("ZeConnectionString");

            //#if DEBUG
            if (string.IsNullOrEmpty(ConnectionString))
            {
               //if (request.Host.ToString().ToUpper().Contains("DEV") || request.Host.ToString().ToUpper().Contains("LOCALHOST"))
               //{
               //   string Server = "sql6005.site4now.net";
               //   string DBase = "DB_A44F11_StockAPPro2dev";
               //   string User = "DB_A44F11_StockAPPro2dev_admin";
               //   string Password = "";

               //   ConnectionString = $"Data Source={Server};Initial Catalog={DBase};Persist Security Info=True;User ID={User};Password={Password};MultipleActiveResultSets=True";
               //}
               //else
               {
                  string Server = "sql6007.site4now.net";
                  string DBase = "DB_A44F11_HorsDA";
                  string User = "DB_A44F11_HorsDA_admin";
                  string Password = "HildeIsTheBoss19";

                  ConnectionString = $"Data Source={Server};Initial Catalog={DBase};Persist Security Info=True;User ID={User};Password={Password};MultipleActiveResultSets=True";
               };
            };
            //#endif

            if (string.IsNullOrEmpty(ConnectionString))
            {
               MainViewModel.Current.Connection.LastError = "No ConnectionString ...";
               return;
            };

            Connection = new DBSQLViewModel(new SQLServerEngine());
            DB_SQL._ViewModel = Connection;
            Log.Write("", $"{ConnectionString} {(Connection.Open(ConnectionString, true) ? "OK" : "KO")}");

            //CleanAuditTrail();
            //ToDo:  UpdateSessions();

            //if (!MainViewModel.Current.CheckDB())
            //{
            //   Log.Write(new AuditTrail()
            //   {
            //      Application = "wsStockAPPro",
            //      Message = "Version de base de données incompatible!",
            //      Level = ErrorLevel.Critical,
            //   });
            //};
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public static string GetConnectionString(string ConnectionStringName)
      {
         //variable to hold our return value
         string Result = string.Empty;

         //check if a value was provided
         if (!string.IsNullOrEmpty(ConnectionStringName))
         {
            //name provided so search for that connection
            try
            {
               Result = System.Configuration.ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            }
            catch
            {
            };
         };

         //return the value
         return Result;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      string _Version = "0.80";

      public string Version
      {
         get { return _Version; }
         set { _Version = value; }
      }

      public string ProgramCaption
      {
         get { return "HorsDA" + " - " + Version; }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      public class WebServer
      {
         public string UserName { get; internal set; }
      }

      public WebServer webServer { get; set; }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      public bool Debug { get; set; }
      public DBSQLViewModel Connection { get; private set; }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      internal void CheckMaxMVT(int v)
      {
         throw new NotImplementedException();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      internal bool CheckAuthorization(string authorization)
      {
         string authenticationToken = authorization.Replace("Basic ", "");
         string decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
         string[] usernamePasswordArray = decodedAuthenticationToken.Split(':');
         string userName = usernamePasswordArray[0];
         string password = usernamePasswordArray[1];

         return (userName == "HorsDA" && password == "ZPF");
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      internal string GetContent(HttpRequest request)
      {
         int l = (int)request.ContentLength;

         var buffer = new byte[l];
         Stream stream = request.Body;

         int i = 0;
         do
         {
            i = stream.Read(buffer, 0, l);
         } while (i > 0);

         return System.Text.Encoding.Unicode.GetString(buffer);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      internal bool SaveContentFile(HttpRequest request)
      {
         int l = (int)request.ContentLength;

         var buffer = new byte[l];
         Stream stream = request.Body;

         int i = 0;
         do
         {
            i = stream.Read(buffer, 0, l);
         } while (i > 0);

         var st = System.Text.Encoding.Unicode.GetString(buffer);

         return true;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  

   }
}
