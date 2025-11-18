using ministry_altars.Entities; 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ministry_altars.UI.Web.Models
{ 
    public class altars_view_model
    { 
        public IEnumerable<altar_dto> lst_dto { get; set; }
        public altar_dto dto { get; set; }
    }
}