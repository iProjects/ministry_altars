using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders; 
using ministry_altars.Entities;

namespace ministry_altars.Data
{
    public sealed class mongodbapisingleton
    {
        // Because the _instance member is made private, the only way to get the single
        // instance is via the static Instance property below. This can also be similarly
        // achieved with a GetInstance() method instead of the property.
        private static mongodbapisingleton singleInstance;

        public static mongodbapisingleton getInstance(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            // The first call will create the one and only instance.
            if (singleInstance == null)
                singleInstance = new mongodbapisingleton(notificationmessageEventname);
            // Every call afterwards will return the single instance created above.
            return singleInstance;
        }

        private string CONNECTION_STRING = @"mongodb://localhost:27017";
        // Sets the connection URI
        //string connectionUri = "mongodb://localhost:27017";
        string connectionUri = "mongodb+srv://softwareproviders254_db_user:<db_password>@estatecluster.ch2oagw.mongodb.net/ministry_altars_database?retryWrites=true&w=majority&appName=estateCluster";
        string app_name = ConfigurationManager.AppSettings["APP_NAME"];
        string host = ConfigurationManager.AppSettings["mongodb_datasource"];
        string DB_NAME = ConfigurationManager.AppSettings["mongodb_database"];
        int port = int.Parse(ConfigurationManager.AppSettings["mongodb_port"]);
        private event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        private string TAG;

        MongoServerSettings server_settings;
        MongoServer server;
        MongoDatabase database;

        MongoCollection<altar_dto> altars;
        MongoCollection<pastor_dto> pastors; 

        MongoCollection<log_dto> logs;
        MongoCollection<user_dto> users;

        private mongodbapisingleton(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {

            try
            {
                TAG = this.GetType().Name;
                _notificationmessageEventname = notificationmessageEventname;

                //setconnectionstring();
                //createdatabaseonfirstload();
                //createtablesonfirstload();

                //connect_to_local_database();
                //connect_to_cloud_database();
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        private mongodbapisingleton()
        {

        }
        public responsedto set_up_database_on_load()
        {
            responsedto _responsedto = new responsedto();

            //responsedto _cloud_responsedto = connect_to_cloud_database();
            responsedto _local_responsedto = connect_to_local_database();

            //if (_cloud_responsedto.isresponseresultsuccessful)
            //{
            //    _responsedto.responsesuccessmessage += _cloud_responsedto.responsesuccessmessage + Environment.NewLine;
            //    _responsedto.isresponseresultsuccessful = true;
            //}
            //else
            //{
            //    _responsedto.responseerrormessage += _cloud_responsedto.responseerrormessage + Environment.NewLine;
            //    _responsedto.isresponseresultsuccessful = false;
            //}

            if (_local_responsedto.isresponseresultsuccessful)
            {
                _responsedto.responsesuccessmessage += _local_responsedto.responsesuccessmessage;
                _responsedto.isresponseresultsuccessful = true;
            }
            else
            {
                _responsedto.responseerrormessage += _local_responsedto.responseerrormessage;
                _responsedto.isresponseresultsuccessful = false;
            }

            return _responsedto;
        }

        public responsedto connect_to_cloud_database()
        {
            responsedto _responsedto = new responsedto();
            try
            {
                var client = new MongoClient(@"mongodb://softwareproviders254_db_user:ObraVoke%4027809543@estatecluster.ch2oagw.mongodb.net/ministry_altars_database");
                server = client.GetServer();

                //const string connectionUri = "mongodb+srv://localhost:27017/?connectTimeoutMS=60000&tls=true";
                //var url = new MongoUrl(connectionUri);
                //var settings = MongoClientSettings.FromUrl(url);

                //server_settings = new MongoServerSettings();
                //server_settings.Server = new MongoServerAddress(host, port);
                //server = new MongoServer(server_settings);

                if (!server.DatabaseExists(DB_NAME))
                {
                    MongoDatabase.Create(server_settings, DB_NAME);
                }

                database = server.GetDatabase(DB_NAME);

                altars = database.GetCollection<altar_dto>("altars");
                pastors = database.GetCollection<pastor_dto>("pastors"); 

                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");

                _responsedto.responsesuccessmessage = "Mongo DB Database Setup Successfull.";
                return _responsedto;

            }
            catch (Exception ex)
            {
                _responsedto.responseerrormessage = ex.ToString();
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return _responsedto;
            }
        }
        public responsedto connect_to_local_database()
        {
            responsedto _responsedto = new responsedto();
            try
            {
                server_settings = new MongoServerSettings();
                server_settings.Server = new MongoServerAddress(host, port);
                server = new MongoServer(server_settings);

                if (!server.DatabaseExists(DB_NAME))
                {
                    MongoDatabase.Create(server_settings, DB_NAME);
                }

                database = server.GetDatabase(DB_NAME);

                altars = database.GetCollection<altar_dto>("altars");
                pastors = database.GetCollection<pastor_dto>("pastors"); 

                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");


                _responsedto.responsesuccessmessage = "Mongo DB Database Setup Successfull.";
                return _responsedto;

            }
            catch (Exception ex)
            {
                _responsedto.responseerrormessage = ex.ToString();
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return _responsedto;
            }
        }

        #region "altars"
        public responsedto create_altar_in_database(altar_dto dto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                MongoCollection<altar_dto> records = database.GetCollection<altar_dto>("altars");

                altar_dto _dto = new altar_dto();
                _dto.mongodb_id = ObjectId.GenerateNewId();
                _dto.id = get_next_altar_id();
                _dto.name = dto.name;
                _dto.address = dto.address;
                _dto.country = dto.country;
                _dto.county = dto.county;
                _dto.type = dto.type;
                _dto.status = "active";
                _dto.created_date = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss tt");

                MongoInsertOptions opts = new MongoInsertOptions();
                opts.Flags = InsertFlags.ContinueOnError;

                records.Insert(_dto, opts);

                _responsedto.isresponseresultsuccessful = true;
                _responsedto.responsesuccessmessage = "Record created successfully in " + DBContract.mongodb + ".";

                return _responsedto;
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        private long get_next_altar_id()
        {
            try
            {
                long next_id;
                MongoCollection<altar_dto> records = database.GetCollection<altar_dto>("altars");
                List<altar_dto> lst_dto = records.FindAll().AsQueryable().OrderByDescending(a => a.id).Take(20).ToList();
                var last_doc = lst_dto.FirstOrDefault();
                if (last_doc != null)
                {
                    long previous_id = last_doc.id;
                    next_id = previous_id + 1;
                }
                else
                {
                    next_id = 1;
                }
                return next_id;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return 0;
            }
        }
        public bool check_if_altar_exists(string entity_name)
        {
            bool _exists = false;
            try
            {
                DataTable dt = new DataTable();
                dt = get_all_altars();

                var _recordscount = dt.Rows.Count;

                for (int i = 0; i < _recordscount; i++)
                {
                    var _record_from_server = Convert.ToString(dt.Rows[i]["altar_name"]);

                    if (entity_name == _record_from_server)
                    {
                        _exists = true;
                        return _exists;
                    }
                }

                return _exists;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return _exists;
            }
        }

        public DataTable get_all_altars()
        {
            MongoCollection<altar_dto> records = database.GetCollection<altar_dto>("altars");
            DataTable dt = new DataTable();
            var count = records.FindAll().Count();
            List<altar_dto> lst_dto = new List<altar_dto>();

            foreach (var dto in records.FindAll())
            {
                lst_dto.Add(dto);
            }

            dt = utilzsingleton.getInstance(_notificationmessageEventname).Convert_List_To_Datatable(lst_dto);

            return dt;

        }
        #endregion "altars"

        #region "pastors"
        public responsedto create_pastor_in_database(pastor_dto dto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                MongoCollection<pastor_dto> records = database.GetCollection<pastor_dto>("pastors");

                pastor_dto _dto = new pastor_dto();
                _dto.mongodb_id = ObjectId.GenerateNewId();
                _dto.id = get_next_pastor_id();
                _dto.name = dto.name;
                _dto.phone_no = dto.phone_no;
                _dto.email = dto.email;
                _dto.id_no = dto.id_no;
                _dto.dob = dto.dob;
                _dto.year = dto.year;
                _dto.month = dto.month;
                _dto.day = dto.day;
                _dto.gender = dto.gender;
                _dto.type = dto.type;
                _dto.altar_id = dto.altar_id; 
                _dto.status = "active";
                _dto.created_date = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss tt");

                MongoInsertOptions opts = new MongoInsertOptions();
                opts.Flags = InsertFlags.ContinueOnError;

                records.Insert(_dto, opts);

                _responsedto.isresponseresultsuccessful = true;
                _responsedto.responsesuccessmessage = "Record created successfully in " + DBContract.mongodb + ".";

                return _responsedto;
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        private long get_next_pastor_id()
        {
            try
            {
                long next_id;
                MongoCollection<pastor_dto> records = database.GetCollection<pastor_dto>("pastors");
                List<pastor_dto> lst_dto = records.FindAll().AsQueryable().OrderByDescending(a => a.id).Take(20).ToList();
                var last_doc = lst_dto.FirstOrDefault();
                if (last_doc != null)
                {
                    long previous_id = last_doc.id;
                    next_id = previous_id + 1;
                }
                else
                {
                    next_id = 1;
                }
                return next_id;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return 0;
            }
        }
        public bool check_if_pastor_exists(string entity_name)
        {
            bool _exists = false;
            try
            {
                DataTable dt = new DataTable();
                dt = get_all_pastors();

                var _recordscount = dt.Rows.Count;

                for (int i = 0; i < _recordscount; i++)
                {
                    var _record_from_server = Convert.ToString(dt.Rows[i]["name"]);

                    if (entity_name == _record_from_server)
                    {
                        _exists = true;
                        return _exists;
                    }
                }

                return _exists;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return _exists;
            }
        }

        public DataTable get_all_pastors()
        {
            MongoCollection<pastor_dto> records = database.GetCollection<pastor_dto>("pastors");
            DataTable dt = new DataTable();
            var count = records.FindAll().Count();
            List<pastor_dto> lst_dto = new List<pastor_dto>();

            foreach (var dto in records.FindAll())
            {
                lst_dto.Add(dto);
            }

            dt = utilzsingleton.getInstance(_notificationmessageEventname).Convert_List_To_Datatable(lst_dto);

            return dt;

        }
        #endregion "pastors"

        #region "users"
        public responsedto create_user_in_database(user_dto dto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                MongoCollection<user_dto> records = database.GetCollection<user_dto>("users");

                user_dto _dto = new user_dto();
                _dto.mongodb_id = ObjectId.GenerateNewId();
                _dto.id = get_next_log_id();
                _dto.email = dto.email;
                _dto.password = dto.password;
                _dto.password_salt = dto.password_salt;
                _dto.password_hash = dto.password_hash;
                _dto.fullnames = dto.fullnames;
                _dto.gender = dto.gender;
                _dto.dob = dto.dob;
                _dto.year = dto.year;
                _dto.month = dto.month;
                _dto.day = dto.day;
                _dto.status = "active";
                _dto.created_date = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss tt");

                MongoInsertOptions opts = new MongoInsertOptions();
                opts.Flags = InsertFlags.ContinueOnError;

                records.Insert(_dto, opts);

                _responsedto.isresponseresultsuccessful = true;
                _responsedto.responsesuccessmessage = "Record created successfully in " + DBContract.mongodb + ".";

                return _responsedto;
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        private long get_next_user_id()
        {
            try
            {
                long next_id;
                MongoCollection<user_dto> records = database.GetCollection<user_dto>("users");
                List<user_dto> lst_dto = records.FindAll().AsQueryable().OrderByDescending(a => a.id).Take(20).ToList();
                var last_doc = lst_dto.FirstOrDefault();
                if (last_doc != null)
                {
                    long previous_id = last_doc.id;
                    next_id = previous_id + 1;
                }
                else
                {
                    next_id = 1;
                }
                return next_id;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return 0;
            }
        }
        public bool check_if_user_exists(string entity_name)
        {
            bool _exists = false;
            try
            {
                DataTable dt = new DataTable();

                dt = get_all_users();

                var _recordscount = dt.Rows.Count;

                for (int i = 0; i < _recordscount; i++)
                {
                    var _record_from_server = Convert.ToString(dt.Rows[i]["email"]);

                    if (entity_name == _record_from_server)
                    {
                        _exists = true;
                        return _exists;
                    }
                }

                return _exists;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return _exists;
            }
        }

        public DataTable get_all_users()
        {
            MongoCollection<user_dto> records = database.GetCollection<user_dto>("users");
            DataTable dt = new DataTable();
            var count = records.FindAll().Count();
            List<user_dto> lst_dto = new List<user_dto>();

            foreach (var dto in records.FindAll())
            {
                lst_dto.Add(dto);
            }

            dt = utilzsingleton.getInstance(_notificationmessageEventname).Convert_List_To_Datatable(lst_dto);

            return dt;

        }
        #endregion "users"

        #region "logs"
        public responsedto create_log_in_database(log_dto dto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                MongoCollection<log_dto> records = database.GetCollection<log_dto>("logs");

                log_dto _dto = new log_dto();
                _dto.mongodb_id = ObjectId.GenerateNewId();
                _dto.id = get_next_log_id();
                _dto.message = dto.message;
                _dto.timestamp = dto.timestamp;
                _dto.tag = dto.tag;
                _dto.status = "active";
                _dto.created_date = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss tt");

                MongoInsertOptions opts = new MongoInsertOptions();
                opts.Flags = InsertFlags.ContinueOnError;

                records.Insert(_dto, opts);

                _responsedto.isresponseresultsuccessful = true;
                _responsedto.responsesuccessmessage = "Record created successfully in " + DBContract.mongodb + ".";

                return _responsedto;
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        private long get_next_log_id()
        {
            try
            {
                long next_id;
                MongoCollection<log_dto> records = database.GetCollection<log_dto>("logs");
                List<log_dto> lst_dto = records.FindAll().AsQueryable().OrderByDescending(a => a.id).Take(20).ToList();
                var last_doc = lst_dto.FirstOrDefault();
                if (last_doc != null)
                {
                    long previous_id = last_doc.id;
                    next_id = previous_id + 1;
                }
                else
                {
                    next_id = 1;
                }
                return next_id;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return 0;
            }
        }
        public bool check_if_log_exists(string entity_name)
        {
            bool _exists = false;
            try
            {
                DataTable dt = new DataTable();
                dt = get_all_logs();

                var _recordscount = dt.Rows.Count;

                for (int i = 0; i < _recordscount; i++)
                {
                    var _record_from_server = Convert.ToString(dt.Rows[i]["message"]);

                    if (entity_name == _record_from_server)
                    {
                        _exists = true;
                        return _exists;
                    }
                }

                return _exists;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return _exists;
            }
        }

        public DataTable get_all_logs()
        {
            MongoCollection<log_dto> records = database.GetCollection<log_dto>("logs");
            DataTable dt = new DataTable();
            var count = records.FindAll().Count();
            List<log_dto> lst_dto = new List<log_dto>();

            foreach (var dto in records.FindAll())
            {
                lst_dto.Add(dto);
            }

            dt = utilzsingleton.getInstance(_notificationmessageEventname).Convert_List_To_Datatable(lst_dto);

            return dt;

        }
        #endregion "log"






















    }
}
