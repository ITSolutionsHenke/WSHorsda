using System;
using System.Collections.Generic;
using ZPF.AT;
using ZPF.SQL;

namespace ZPF
{
   public partial class wsViewModel : BaseViewModel
   {
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      static wsViewModel _Instance = null;

      public static wsViewModel Current
      {
         get
         {
            if (_Instance == null)
            {
               _Instance = new wsViewModel();
            };

            return _Instance;
         }

         set
         {
            _Instance = value;
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -


      public wsViewModel()
      {
         _Instance = this;

         //Articles = new List<Article_CE>();
         //Contacts = new List<Contact_CE>();
         //Emplacements = new List<Emplacement_CE>();
         //EtatStock = new List<Stock_CE>();
         //Users = new List<User_CE>();
         //Justifications = new List<Justification_CE>();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -
   }
}
