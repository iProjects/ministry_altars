using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ministry_altars.Entities
{
    public class altar_dto
    {
        [BsonElement("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Browsable(true)]
        public ObjectId mongodb_id { get; set; }
        public long id { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name cannot be null")]
        public string name { get; set; }
        [Display(Name = "Address")]
        [Required(ErrorMessage = "Address cannot be null")]
        public string address { get; set; }
        [Display(Name = "Country")]
        [Required(ErrorMessage = "Country cannot be null")]
        public string country { get; set; }
        [Display(Name = "County")]
        [Required(ErrorMessage = "County cannot be null")]
        public string county { get; set; }
        [Display(Name = "Type")]
        [Required(ErrorMessage = "Type cannot be null")]
        public string type { get; set; }
        public string status { get; set; }
        public string created_date { get; set; }


        public IEnumerable<SelectListItem> countries { get; set; }
        public IEnumerable<SelectListItem> counties { get; set; }
        public IEnumerable<SelectListItem> types { get; set; }

    }
}
