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
    public class AltarsController : Controller
    {
        public string TAG;
        //Event declaration:
        //event for publishing messages to output
        public event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        //list to hold messages
        public List<notificationdto> _lstnotificationdto = new List<notificationdto>();

        public AltarsController()
        {
            TAG = this.GetType().Name;

            AppDomain.CurrentDomain.UnhandledException += new
UnhandledExceptionEventHandler(UnhandledException);
            System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(ThreadException);

            //Subscribing to the event: 
            //Dynamically:
            //EventName += HandlerName;
            _notificationmessageEventname += notificationmessageHandler;

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("finished AltarsController initialization", TAG));

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
                 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult Index([Bind] altar_dto search_model)
        {
            try
            {
                altar_dto dto = new altar_dto();
                altars_view_model model = new altars_view_model();
                List<altar_dto> _lst_dtos = new List<altar_dto>();

                _lst_dtos = populate_dtos(_lst_dtos);

                if (_lst_dtos != null)
                {
                    Console.WriteLine("Altars count: " + _lst_dtos.Count());
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
            catch (Exception ex)
            {
                TempData["error_message"] = ex.ToString();
                helper_utils.getInstance(_notificationmessageEventname).log_messages(ex.ToString(), TAG);
                error_handler_view_model error_model = new error_handler_view_model();
                error_model.ex = ex;
                return View("Error_View", error_model);
            }
        }

        private List<altar_dto> populate_dtos(List<altar_dto> _altars)
        {
            DataTable dt = null;
            string query = "";
            bool showinactive = true;
            string server = "sqlite"; //selected_server.Key;

            if (showinactive)
            {
                query = DBContract.altars_entity_table.SELECT_ALL_QUERY;
            }
            else
            {
                query = DBContract.altars_entity_table.SELECT_ALL_FILTER_QUERY;
            }

            dt = businesslayerapisingleton.getInstance(_notificationmessageEventname).get_altars(showinactive, query, DBContract.sqlite);

            if (dt != null)
            {
                _altars = utilzsingleton.getInstance(_notificationmessageEventname).Convert_DataTable_To_list<altar_dto>(dt);
                _altars = _altars.OrderByDescending(i => i.id).ThenBy(t => t.name).ToList();

                Console.WriteLine("Altars count: " + _altars.Count());
            }
            else
            {
                return null;
            }

            return _altars;
        }

        private altar_dto populate_model(altar_dto model)
        {
            List<SelectListItem> countries = new List<SelectListItem>();

            Dictionary<string, string> countries_dict = new Dictionary<string, string>()
            { 
                {"Kenya","Kenya"},
                {"Uganda","Uganda"}, 
                {"Rwanda","Rwanda"}, 
                {"Tanzania","Tanzania"},
            };

            SelectListItem countries_slim = new SelectListItem()
            {
                Value = null,
                Text = "--- Select Country ---"
            };

            foreach (KeyValuePair<string, string> country in countries_dict)
            {
                SelectListItem sli = new SelectListItem()
                {
                    Value = country.Key,
                    Text = country.Value
                };
                countries.Add(sli);
            }

            countries.Insert(0, countries_slim);

            model.countries = countries;

            List<SelectListItem> counties = new List<SelectListItem>();

            Dictionary<string, string> counties_dict = new Dictionary<string, string>()
            { 
                {"Kiambu","Kiambu"},
                {"Mombasa","Mombasa"}, 
                {"Nairobi","Nairobi"}, 
                {"Muranga","Muranga"},
            };

            SelectListItem counties_slim = new SelectListItem()
            {
                Value = null,
                Text = "--- Select County ---"
            };

            foreach (KeyValuePair<string, string> type in counties_dict)
            {
                SelectListItem sli = new SelectListItem()
                {
                    Value = type.Key,
                    Text = type.Value
                };
                counties.Add(sli);
            }

            counties.Insert(0, counties_slim);

            model.counties = counties;

            List<SelectListItem> types = new List<SelectListItem>();

            Dictionary<string, string> types_dict = new Dictionary<string, string>()
            { 
                {"Main Altar","Main Altar"},
                {"Sub Altar","Sub Altar"},  
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

            return model;
        }

        [HttpGet]
        public JsonResult getmonthdays(string year, string month)
        {
            List<SelectListItem> days = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(year))
            {
                int _year = int.Parse(year);
                int _month = int.Parse(month);

                for (int i = 1; i <= DateTime.DaysInMonth(_year, _month); i++)
                {
                    days.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
                }
                return Json(days, JsonRequestBehavior.AllowGet);
            }
            else if (string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(month))
            {
                int _year = DateTime.Now.Year;
                int _month = int.Parse(month);

                for (int i = 1; i <= DateTime.DaysInMonth(_year, _month); i++)
                {
                    days.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
                }
                return Json(days, JsonRequestBehavior.AllowGet);
            }
            else if (string.IsNullOrEmpty(year) && string.IsNullOrEmpty(month))
            {
                int _year = DateTime.Now.Year;
                int _month = DateTime.Now.Month;

                for (int i = 1; i <= DateTime.DaysInMonth(_year, _month); i++)
                {
                    days.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
                }
                return Json(days, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [HttpPost]
        public JsonResult search([Bind] altar_dto search_model)
        {
            altar_dto dto = new altar_dto();
            altars_view_model model = new altars_view_model();
            DataTable dt = null;
            List<altar_dto> _lst_dtos = new List<altar_dto>();

            dt = businesslayerapisingleton.getInstance(_notificationmessageEventname).search_altars_in_database(search_model);

            if (dt != null)
            {
                _lst_dtos = utilzsingleton.getInstance(_notificationmessageEventname).Convert_DataTable_To_list<altar_dto>(dt);
                _lst_dtos = _lst_dtos.OrderByDescending(i => i.id).ThenBy(t => t.name).ToList();

                Console.WriteLine("Altars count: " + _lst_dtos.Count());
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

        // GET: altars/Create
        public ActionResult Create()
        {
            altar_dto model = new altar_dto();
            model = populate_model(model);

            return View(model);
        }

        // POST: altars/Create
        [HttpPost]
        public ActionResult Create([Bind] altar_dto model)
        {
            try
            {
                // TODO: Add insert logic here

                if (ModelState.IsValid)
                {
                    altar_dto dto = new altar_dto();
                    dto.name = Utils.ConvertFirstLetterToUpper(model.name); ;
                    dto.address = Utils.ConvertFirstLetterToUpper(model.address); ;
                    dto.country = model.country;
                    dto.county = model.county;
                    dto.type = model.type;
                    dto.status = "active";
                    dto.created_date = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss tt");

                    //check if record exists.
                    //bool _exists = businesslayerapisingleton.getInstance(_notificationmessageEventname).check_if_altar_exists(model.name);

                    //if (_exists)
                    //{
                    //    TempData["error_message"] = "Altar with name [ " + model.name + " ] Exists.";
                    //    helper_utils.getInstance(_notificationmessageEventname).log_messages("Altar with name [ " + model.name + " ] Exists.", TAG);
                    //    model = populate_model(model);
                    //    return View(model);
                    //}

                    //save data in database.
                    List<responsedto> _lstresponse = new List<responsedto>();

                    _lstresponse = businesslayerapisingleton.getInstance(_notificationmessageEventname).create_altar_in_database(dto);

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

        // GET: altars/Details/5
        public ActionResult Details(int id)
        {
            return View();
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
