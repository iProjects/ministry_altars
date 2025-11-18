using ministry_altars.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ministry_altars.UI.Web.Models
{
    public class users_view_model
    {
        public IEnumerable<user_dto> lst_dto { get; set; }
        public user_dto dto { get; set; }
    }
}