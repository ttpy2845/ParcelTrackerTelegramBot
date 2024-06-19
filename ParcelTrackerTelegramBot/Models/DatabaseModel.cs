using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcelTrackerTelegramBot.Models
{
    public class DatabaseModel
    {
        public int User_id { get; set; }
        public string Parcel_code { get; set; }
        public string Parcel_tag { get; set; }
        public string Parcel_description { get; set; }
        public DateTime? Registration_time { get; set; }

        public DatabaseModel(int user_id, string parcel_code, string parcel_tag, string parcel_description, DateTime dateTime)
        {
            this.User_id = user_id;
            this.Parcel_code = parcel_code;
            this.Parcel_tag = parcel_tag;
            this.Parcel_description = parcel_description;
            this.Registration_time = dateTime;
        }

    }
}
