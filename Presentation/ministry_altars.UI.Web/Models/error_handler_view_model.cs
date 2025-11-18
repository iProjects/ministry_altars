using ministry_altars.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ministry_altars.UI.Web.Models
{
    public class error_handler_view_model
    {
        public Exception ex { get; set; }
        public string message { get; set; }
        public string stack_trace { get; set; }
    }
}