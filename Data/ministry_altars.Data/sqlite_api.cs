/*
 * Created by: "kevin mutugi, kevinmk30@gmail.com, +254717769329"
 * Date: 01/23/2020
 * Time: 02:56
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms; 
using ministry_altars.Entities;

namespace ministry_altars.Data
{
    public sealed class sqlite_api
    {
        // Because the _instance member is made private, the only way to get the single
        // instance is via the static Instance property below. This can also be similarly
        // achieved with a GetInstance() method instead of the property.
        private static sqlite_api singleInstance;
        public static sqlite_api getInstance(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            // The first call will create the one and only instance.
            if (singleInstance == null)
                singleInstance = new sqlite_api(notificationmessageEventname);
            // Every call afterwards will return the single instance created above.
            return singleInstance;
        }
        public string TAG;

        public event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        public sqlite_api(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {

            TAG = this.GetType().Name;

            _notificationmessageEventname = notificationmessageEventname;

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("initialized sqlite_api...", TAG));
        }

        public responsedto save_altar_in_sqlite(altar_dto _altar_dto)
        {
            responsedto _responsedto = new responsedto();

            bool _exists_in_db = check_if_altar_exists(_altar_dto.name);

            if (_exists_in_db)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responsesuccessmessage = "Altar with name [ " + _altar_dto.name + " ] exists in " + DBContract.sqlite + ".";
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Altar with name [ " + _altar_dto.name + " ] exists in " + DBContract.sqlite + ".", TAG));
            }
            else
            {
                string save_in_db = utilzsingleton.getInstance(_notificationmessageEventname).getappsettinggivenkey("saveinsqlite", "false");

                bool _save_in_db;
                bool _try_save_in_db = bool.TryParse(save_in_db, out _save_in_db);

                if (_save_in_db)
                {
                    _responsedto = sqliteapisingleton.getInstance(_notificationmessageEventname).create_altar_in_database(_altar_dto);

                }
            }
            return _responsedto;
        }
        public bool check_if_altar_exists(string entity_name)
        {
            bool _exists_in_db = sqliteapisingleton.getInstance(_notificationmessageEventname).check_if_altar_exists(entity_name);

            return _exists_in_db;

        }
        public responsedto save_pastor_in_sqlite(pastor_dto _pastor_dto)
        {
            responsedto _responsedto = new responsedto();

            bool _exists_in_db = check_if_pastor_exists(_pastor_dto.name);

            if (_exists_in_db)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responsesuccessmessage = "Pastor with name [ " + _pastor_dto.name + " ] exists in " + DBContract.sqlite + ".";
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Pastor with name [ " + _pastor_dto.name + " ] exists in " + DBContract.sqlite + ".", TAG));
            }
            else
            {
                string save_in_db = utilzsingleton.getInstance(_notificationmessageEventname).getappsettinggivenkey("saveinsqlite", "false");

                bool _save_in_db;
                bool _try_save_in_db = bool.TryParse(save_in_db, out _save_in_db);

                if (_save_in_db)
                {
                    _responsedto = sqliteapisingleton.getInstance(_notificationmessageEventname).create_pastor_in_database(_pastor_dto);

                }
            }
            return _responsedto;
        }
        public bool check_if_pastor_exists(string entity_name)
        {
            bool _exists_in_db = sqliteapisingleton.getInstance(_notificationmessageEventname).check_if_pastor_exists(entity_name);

            return _exists_in_db;

        }
        public responsedto save_user_in_sqlite(user_dto _dto)
        {
            responsedto _responsedto = new responsedto();

            bool _exists_in_db = check_if_user_exists(_dto.email);

            if (_exists_in_db)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responsesuccessmessage = "User with email [ " + _dto.email + " ] exists in " + DBContract.sqlite + ".";
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("User with email [ " + _dto.email + " ] exists in " + DBContract.mssql + ".", TAG));
            }
            else
            {
                string save_in_db = utilzsingleton.getInstance(_notificationmessageEventname).getappsettinggivenkey("saveinsqlite", "false");

                bool _save_in_db;
                bool _try_save_in_db = bool.TryParse(save_in_db, out _save_in_db);

                if (_save_in_db)
                {
                    _responsedto = sqliteapisingleton.getInstance(_notificationmessageEventname).create_user_in_database(_dto);

                }
            }
            return _responsedto;
        }
        public bool check_if_user_exists(string entity_name)
        {
            bool _exists_in_db = sqliteapisingleton.getInstance(_notificationmessageEventname).check_if_user_exists(entity_name);

            return _exists_in_db;

        }




    }
}
