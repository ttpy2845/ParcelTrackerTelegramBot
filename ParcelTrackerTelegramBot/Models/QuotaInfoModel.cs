using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcelTrackerTelegramBot.Models
{
    public class QuotaInfoModel
    {
        public int code { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public int quota_total { get; set; }
        public int quota_used { get; set; }
        public int quota_remain { get; set; }
        public int today_used { get; set; }
        public int max_track_daily { get; set; }
        public int free_email_quota { get; set; }
        public int free_email_quotaused { get; set; }
    }
}
