using ministry_altars.Business;
using ministry_altars.Data;
using ministry_altars.Entities;
using ministry_altars.UI.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ministry_altars.UI.Web.Controllers
{
    public class PastorsController : Controller
    {
        public string TAG;
        //Event declaration:
        //event for publishing messages to output
        public event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        //list to hold messages
        public List<notificationdto> _lstnotificationdto = new List<notificationdto>();

        public PastorsController()
        {
            TAG = this.GetType().Name;

            AppDomain.CurrentDomain.UnhandledException += new
UnhandledExceptionEventHandler(UnhandledException);
            System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(ThreadException);

            //Subscribing to the event: 
            //Dynamically:
            //EventName += HandlerName;
            _notificationmessageEventname += notificationmessageHandler;

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("finished PastorsController initialization", TAG));

        }

        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            helper_utils.getInstance(_notificationmessageEventname).log_messages(ex.ToString(), TAG);
            //Log.Write_To_Log_File_temp_dir(ex);
            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
        }

        private void ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            helper_utils.getInstance(_notificationmessageEventname).log_messages(ex.ToString(), TAG);
            //Log.Write_To_Log_File_temp_dir(ex);
            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
        }

        //Event handler declaration:
        private void notificationmessageHandler(object sender, notificationmessageEventArgs args)
        {
            try
            {
                /* Handler logic */
                notificationdto _notificationdto = new notificationdto();

                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");

                String _logtext = Environment.NewLine + "[ " + dateTimenow + " ]   " + args.message;

                _notificationdto._notification_message = _logtext;
                _notificationdto._created_datetime = dateTimenow;
                _notificationdto.TAG = args.TAG;

                _lstnotificationdto.Add(_notificationdto);

                Console.WriteLine(args.message);

                //TempData["error_message"] = args.message;

                //Log.WriteToErrorLogFile_and_EventViewer(new Exception(args.message));

                var _lstmsgdto = from msgdto in _lstnotificationdto
                                 orderby msgdto._created_datetime descending
                                 select msgdto._notification_message;

                String[] _logflippedlines = _lstmsgdto.ToArray();

                if (_logflippedlines.Length > 5000)
                {
                    _lstnotificationdto.Clear();
                }

                //txtlog.Lines = _logflippedlines;
                //txtlog.ScrollToCaret();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult Index([Bind] pastor_dto search_model)
        {
            pastor_dto dto = new pastor_dto();
            pastors_view_model model = new pastors_view_model();
            List<pastor_dto> _lst_dtos = new List<pastor_dto>();

            _lst_dtos = populate_dtos(_lst_dtos);

            if (_lst_dtos != null)
            {
                Console.WriteLine("Pastors count: " + _lst_dtos.Count());
                TempData["success_message"] = "Retrieved [ " + _lst_dtos.Count() + " ] records.";
                helper_utils.getInstance(_notificationmessageEventname).log_messages("Retrieved [ " + _lst_dtos.Count() + " ] records.", TAG);
            }
            else
            {
                model.dto = populate_model(dto);
                TempData["error_message"] = "Error retrieving data.";
                helper_utils.getInstance(_notificationmessageEventname).log_messages("Error retrieving data.", TAG);
                return View(model);
            }

            model.lst_dto = _lst_dtos;
            model.dto = populate_model(dto);

            //Display the records
            return View(model);

        }

        private List<pastor_dto> populate_dtos(List<pastor_dto> _pastors)
        {
            DataTable dt = null;
            string query = "";
            bool showinactive = true;
            string server = "sqlite"; //selected_server.Key;

            if (showinactive)
            {
                query = DBContract.pastors_entity_table.SELECT_ALL_QUERY;
            }
            else
            {
                query = DBContract.pastors_entity_table.SELECT_ALL_FILTER_QUERY;
            }

            dt = businesslayerapisingleton.getInstance(_notificationmessageEventname).get_pastors(showinactive, query, DBContract.sqlite);

            if (dt != null)
            {
                _pastors = utilzsingleton.getInstance(_notificationmessageEventname).Convert_DataTable_To_list<pastor_dto>(dt);
                _pastors = _pastors.OrderByDescending(i => i.id).ThenBy(t => t.name).ToList();

                Console.WriteLine("Pastors count: " + _pastors.Count());
            }
            else
            {
                return null;
            }

            return _pastors;
        }

        private pastor_dto populate_model(pastor_dto model)
        {
            List<SelectListItem> years = new List<SelectListItem>();
            List<SelectListItem> months = new List<SelectListItem>();
            List<SelectListItem> days = new List<SelectListItem>();
            List<SelectListItem> altars = new List<SelectListItem>();
            List<SelectListItem> types = new List<SelectListItem>();
            List<SelectListItem> genders = new List<SelectListItem>(); 

            List<string> _years = new List<string>();

            DateTime startyear = DateTime.Now;
            while (startyear.Year >= DateTime.Now.AddYears(-80).Year)
            {
                _years.Add(startyear.Year.ToString());
                startyear = startyear.AddYears(-1);
            }

            SelectListItem sli_yr = new SelectListItem()
            {
                Value = null,
                Text = "--- Select Year ---"
            };

            years.Add(sli_yr);

            foreach (string yr in _years)
            {
                SelectListItem sli = new SelectListItem()
                {
                    Value = yr,
                    Text = yr
                };
                years.Add(sli);
            }

            model.years = years;

            months.Add(new SelectListItem { Value = null, Text = "--- Select Month ---" });
            months.Add(new SelectListItem { Value = "1", Text = "January" });
            months.Add(new SelectListItem { Value = "2", Text = "February" });
            months.Add(new SelectListItem { Value = "3", Text = "March" });
            months.Add(new SelectListItem { Value = "4", Text = "April" });
            months.Add(new SelectListItem { Value = "5", Text = "May" });
            months.Add(new SelectListItem { Value = "6", Text = "June" });
            months.Add(new SelectListItem { Value = "7", Text = "July" });
            months.Add(new SelectListItem { Value = "8", Text = "August" });
            months.Add(new SelectListItem { Value = "9", Text = "September" });
            months.Add(new SelectListItem { Value = "10", Text = "October" });
            months.Add(new SelectListItem { Value = "11", Text = "November" });
            months.Add(new SelectListItem { Value = "12", Text = "December" });

            model.months = months;

            int _year = DateTime.Now.Year;
            int _month = DateTime.Now.Month;

            days.Add(new SelectListItem { Value = null, Text = "--- Select Day ---" });

            for (int i = 1; i <= DateTime.DaysInMonth(_year, _month); i++)
            {
                days.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
            }

            model.days = days;

            Dictionary<string, string> types_dict = new Dictionary<string, string>()
            { 
                {"ArchBishop","ArchBishop"},
                {"Senior Deputy ArchBishop","Senior Deputy ArchBishop"}, 
                {"Deputy ArchBishop","Deputy ArchBishop"},
                {"Senior Pastor","Senior Pastor"},
                {"OverSeer","OverSeer"},
                {"Pastor","Pastor"}, 
            };

            SelectListItem types_slim = new SelectListItem()
            {
                Value = null,
                Text = "--- Select Type ---"
            };

            foreach (KeyValuePair<string, string> type in types_dict)
            {
                SelectListItem sli = new SelectListItem()
                {
                    Value = type.Key,
                    Text = type.Value
                };
                types.Add(sli);
            }

            types.Insert(0, types_slim);

            model.types = types;

            List<altar_dto> _altars = new List<altar_dto>();
            _altars = utilzsingleton.getInstance(_notificationmessageEventname).Convert_DataTable_To_list<altar_dto>(businesslayerapisingleton.getInstance(_notificationmessageEventname).get_altars(true, DBContract.altars_entity_table.SELECT_ALL_QUERY, DBContract.sqlite));

            SelectListItem altars_slim = new SelectListItem()
            {
                Value = null,
                Text = "--- Select Altar ---"
            };

            foreach (altar_dto altar in _altars)
            {
                SelectListItem sli = new SelectListItem()
                {
                    Value = altar.id.ToString(),
                    Text = altar.name
                };
                altars.Add(sli);
            }

            altars.Insert(0, altars_slim);

            model.altars = altars;
            
            Dictionary<string, string> genders_dic = new Dictionary<string, string>()
            { 
                {"Male","Male"},
                {"FeMale","FeMale"},
                {"Prefer Not To Say","Prefer Not To Say"}, 
            };

            SelectListItem gender_item = new SelectListItem()
            {
                Value = null,
                Text = "--- Select Gender ---"
            };
             
            foreach (KeyValuePair<string, string> gender in genders_dic)
            {
                SelectListItem sli = new SelectListItem()
                {
                    Value = gender.Key,
                    Text = gender.Value
                };
                genders.Add(sli);
            }

            genders.Insert(0, gender_item);

            model.genders = genders;

            return model;
        }

        [HttpPost]
        public JsonResult search([Bind] pastor_dto search_model)
        {
            pastor_dto dto = new pastor_dto();
            pastors_view_model model = new pastors_view_model();
            DataTable dt = null;
            List<pastor_dto> _lst_dtos = new List<pastor_dto>();

            dt = businesslayerapisingleton.getInstance(_notificationmessageEventname).search_pastors_in_database(search_model);

            if (dt != null)
            {
                _lst_dtos = utilzsingleton.getInstance(_notificationmessageEventname).Convert_DataTable_To_list<pastor_dto>(dt);
                _lst_dtos = _lst_dtos.OrderByDescending(i => i.id).ThenBy(t => t.name).ToList();

                Console.WriteLine("Pastors count: " + _lst_dtos.Count());
                TempData["success_message"] = "Retrieved [ " + _lst_dtos.Count() + " ] records.";
                helper_utils.getInstance(_notificationmessageEventname).log_messages("Retrieved [ " + _lst_dtos.Count() + " ] records.", TAG);
            }
            else
            {
                model.lst_dto = _lst_dtos;
                model.dto = populate_model(dto);

                TempData["error_message"] = "Error retrieving data.";
                helper_utils.getInstance(_notificationmessageEventname).log_messages("Error retrieving data.", TAG);

                return Json(model);
            }

            model.lst_dto = _lst_dtos;
            model.dto = populate_model(dto);

            //Display the records
            return Json(model);

        }

        // GET: altars/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: altars/Create
        public ActionResult Create()
        {
            pastor_dto model = new pastor_dto();
            model = populate_model(model);

            return View(model);
        }

        // POST: altars/Create
        [HttpPost]
        public ActionResult Create([Bind] pastor_dto model)
        {
            try
            {
                // TODO: Add insert logic here

                if (ModelState.IsValid)
                {
                    pastor_dto dto = new pastor_dto();
                    dto.name = Utils.ConvertFirstLetterToUpper(model.name); ;
                    dto.phone_no = model.phone_no;
                    dto.email = model.email;
                    dto.id_no = model.id_no;
                    dto.dob = model.dob;
                    dto.year = model.year;
                    dto.month = model.month;
                    dto.day = model.day;
                    dto.gender = model.gender;
                    dto.type = model.type;
                    dto.altar_id = model.altar_id;
                    dto.status = "active";
                    dto.created_date = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss tt");

                    //check if record exists.
                    //bool _exists = businesslayerapisingleton.getInstance(_notificationmessageEventname).check_if_altar_exists(model.name);

                    //if (_exists)
                    //{
                    //    TempData["error_message"] = "Pastor with name [ " + model.name + " ] Exists.";
                    //    helper_utils.getInstance(_notificationmessageEventname).log_messages("Pastor with name [ " + model.name + " ] Exists.", TAG);
                    //    model = populate_model(model);
                    //    return View(model);
                    //}

                    //save data in database.
                    List<responsedto> _lstresponse = new List<responsedto>();

                    _lstresponse = businesslayerapisingleton.getInstance(_notificationmessageEventname).create_pastor_in_database(dto);

                    foreach (var response in _lstresponse)
                    {
                        responsedto _responsedto = response;

                        if (!string.IsNullOrEmpty(response.responsesuccessmessage))
                        {
                            Console.WriteLine(response.responsesuccessmessage);
                            TempData["create_message"] = _responsedto.responsesuccessmessage;
                            helper_utils.getInstance(_notificationmessageEventname).log_messages(_responsedto.responsesuccessmessage, TAG);

                        }
                        if (!string.IsNullOrEmpty(response.responseerrormessage))
                        {
                            Console.WriteLine(response.responseerrormessage);
                            TempData["error_message"] = _responsedto.responseerrormessage;
                            helper_utils.getInstance(_notificationmessageEventname).log_messages(_responsedto.responseerrormessage, TAG);
                        }
                    }
                }
                else
                {
                    TempData["error_message"] = "Validation Error.";
                    helper_utils.getInstance(_notificationmessageEventname).log_messages("Validation Error.", TAG);
                    model = populate_model(model);
                    return View(model);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                TempData["error_message"] = ex.ToString();
                helper_utils.getInstance(_notificationmessageEventname).log_messages(ex.ToString(), TAG);
                model = populate_model(model);
                return View(model);
            }
        }

        // GET: altars/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: altars/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: altars/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: altars/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
