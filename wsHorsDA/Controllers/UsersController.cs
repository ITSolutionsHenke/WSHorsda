using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using ZPF;
using ZPF.SQL;

namespace HorsDA
{
   public class UserController : Controller
   {
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      //[AllowCrossSiteJson]
      [Route("~/HorsDA/User/Profil/Create")]
      [HttpPost]
      public string ProfilCreate([FromHeader]string authorization)
      {
         MainViewModel.Current.Prologue(Request);

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

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      [HttpGet]
      [Route("~/HorsDA/User/Login/{Login}/{Password}")]
      public int LoginRequest([FromHeader]string authorization, [FromRoute] string Login, string Password)
      {
         MainViewModel.Current.Prologue(Request);

         if (!string.IsNullOrEmpty(authorization) && MainViewModel.Current.CheckAuthorization(authorization))
         {
            Login = Login.ToUpper();

            //ToDo: use ZPF_User UserViewModel

            //if (UserViewModel.Current.Login(Login, Password))
            //{
            //   return UserViewModel.Current.CurrentUser.PK;
            //}
            //else
            {
               //ToDo: UserViewModel.Current.ErrorMessage
               return -1;

            };

            {
               //Log.Write(new AuditTrail(ex)
               //{
               //   Application = "ws",
               //   Tag = "LoginRequest",
               //   IsBusiness = true,
               //   DataInType = "SQL",
               //   DataIn = wsMainViewModel.Current.Connection.LastQuery
               //});

               //wsMainViewModel.Current.LastError.Add(wsMainViewModel.Current.Connection.LastError);
               //wsMainViewModel.Current.LastError.Add(ex.Message);
            };
         }
         else
         {
            return -1;
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
