using ministry_altars.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ministry_altars.UI.Web.Models
{
    public class logs_view_model
    {
        public IEnumerable<log_dto> lst_dto { get; set; }
        public log_dto dto { get; set; }
    }
}