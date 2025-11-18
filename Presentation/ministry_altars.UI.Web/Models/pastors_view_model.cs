using ministry_altars.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ministry_altars.UI.Web.Models
{
    public class pastors_view_model
    {
        public IEnumerable<pastor_dto> lst_dto { get; set; }
        public pastor_dto dto { get; set; }
    }
}