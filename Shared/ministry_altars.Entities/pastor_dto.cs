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
    public class pastor_dto
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
        [Display(Name = "Phone No")]
        [Required(ErrorMessage = "Phone No cannot be null")]
        public string phone_no { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email cannot be null")]
        public string email { get; set; }
        [Display(Name = "Id No")]
        [Required(ErrorMessage = "Id No is Required")]
        public string id_no { get; set; } 
        public string dob { get; set; }
        [Required(ErrorMessage = "Year of Birth cannot be null")]
        [Display(Name = "Year of Birth")]
        public string year { get; set; }
        [Required(ErrorMessage = "Month of Birth cannot be null")]
        [Display(Name = "Month of Birth")]
        public string month { get; set; }
        [Required(ErrorMessage = "Day of Birth cannot be null")]
        [Display(Name = "Day of Birth")]
        public string day { get; set; }
        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Gender is Required")]
        public string gender { get; set; }
        public string gender_str { get; set; }
        [Display(Name = "Type")]
        [Required(ErrorMessage = "Type cannot be null")]
        public string type { get; set; }
        [Display(Name = "Altar")]
        [Required(ErrorMessage = "Altar cannot be null")]
        public string altar_id { get; set; }
        public string status { get; set; }
        public string created_date { get; set; }


        public IEnumerable<SelectListItem> types { get; set; }
        public IEnumerable<SelectListItem> altars { get; set; }
        public IEnumerable<SelectListItem> genders { get; set; }
        public IEnumerable<SelectListItem> years { get; set; }
        public IEnumerable<SelectListItem> months { get; set; }
        public IEnumerable<SelectListItem> days { get; set; }

    }
}
