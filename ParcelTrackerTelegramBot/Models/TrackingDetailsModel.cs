using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcelTrackerTelegramBot.Models
{
    namespace TrackingDetailsModel
    {
        public class TrackingDetailsModel
        {
            public int code { get; set; }
            public Data data { get; set; }
        }

        public class Accepted
        {
            public string number { get; set; }
            public int carrier { get; set; }
            public string param { get; set; }
            public string tag { get; set; }
            public TrackInfo track_info { get; set; }
        }

        public class Address
        {
            public string country { get; set; }
            public string state { get; set; }
            public string city { get; set; }
            public object street { get; set; }
            public string postal_code { get; set; }
            public Coordinates coordinates { get; set; }
        }

        public class Coordinates
        {
            public string longitude { get; set; }
            public string latitude { get; set; }
        }

        public class Data
        {
            public List<Accepted> accepted { get; set; }
            public List<Rejected> rejected { get; set; }
        }

        public class DescriptionTranslation
        {
            public string lang { get; set; }
            public string description { get; set; }
        }

        public class Error
        {
            public int code { get; set; }
            public string message { get; set; }
        }

        public class EstimatedDeliveryDate
        {
            public string source { get; set; }
            public DateTime? from { get; set; }
            public DateTime? to { get; set; }
        }

        public class Event
        {
            public DateTime? time_iso { get; set; }
            public DateTime? time_utc { get; set; }
            public TimeRaw time_raw { get; set; }
            public string description { get; set; }
            public DescriptionTranslation description_translation { get; set; }
            public string location { get; set; }
            public string stage { get; set; }
            public string sub_status { get; set; }
            public Address address { get; set; }
        }

        public class LatestEvent
        {
            public DateTime? time_iso { get; set; }
            public DateTime? time_utc { get; set; }
            public TimeRaw time_raw { get; set; }
            public string description { get; set; }
            public DescriptionTranslation description_translation { get; set; }
            public string location { get; set; }
            public string stage { get; set; }
            public string sub_status { get; set; }
            public Address address { get; set; }
        }

        public class LatestStatus
        {
            public string status { get; set; }
            public string sub_status { get; set; }
            public object sub_status_descr { get; set; }
        }

        public class Milestone
        {
            public string key_stage { get; set; }
            public DateTime? time_iso { get; set; }
            public DateTime? time_utc { get; set; }
            public TimeRaw time_raw { get; set; }
        }

        public class MiscInfo
        {
            public int risk_factor { get; set; }
            public string service_type { get; set; }
            public string weight_raw { get; set; }
            public string weight_kg { get; set; }
            public string pieces { get; set; }
            public string dimensions { get; set; }
            public string customer_number { get; set; }
            public object reference_number { get; set; }
            public string local_number { get; set; }
            public string local_provider { get; set; }
            public int local_key { get; set; }
        }

        public class Provider
        {
            public Provider provider { get; set; }
            public string service_type { get; set; }
            public string latest_sync_status { get; set; }
            public DateTime? latest_sync_time { get; set; }
            public int events_hash { get; set; }
            public List<Event> events { get; set; }
        }

        public class Provider2
        {
            public int key { get; set; }
            public string name { get; set; }
            public string alias { get; set; }
            public object tel { get; set; }
            public string homepage { get; set; }
            public string country { get; set; }
        }

        public class RecipientAddress
        {
            public string country { get; set; }
            public object state { get; set; }
            public string city { get; set; }
            public object street { get; set; }
            public object postal_code { get; set; }
            public Coordinates coordinates { get; set; }
        }

        public class Rejected
        {
            public string number { get; set; }
            public Error error { get; set; }
        }

        public class ShipperAddress
        {
            public string country { get; set; }
            public string state { get; set; }
            public string city { get; set; }
            public string street { get; set; }
            public string postal_code { get; set; }
            public Coordinates coordinates { get; set; }
        }

        public class ShippingInfo
        {
            public ShipperAddress shipper_address { get; set; }
            public RecipientAddress recipient_address { get; set; }
        }

        public class TimeMetrics
        {
            public int days_after_order { get; set; }
            public int days_of_transit { get; set; }
            public int days_of_transit_done { get; set; }
            public int days_after_last_update { get; set; }
            public EstimatedDeliveryDate estimated_delivery_date { get; set; }
        }

        public class TimeRaw
        {
            public string date { get; set; }
            public string time { get; set; }
            public string timezone { get; set; }
        }

        public class TrackInfo
        {
            public ShippingInfo shipping_info { get; set; }
            public LatestStatus latest_status { get; set; }
            public LatestEvent latest_event { get; set; }
            public TimeMetrics time_metrics { get; set; }
            public List<Milestone> milestone { get; set; }
            public MiscInfo misc_info { get; set; }
            public Tracking tracking { get; set; }
        }

        public class Tracking
        {
            public int providers_hash { get; set; }
            public List<Provider> providers { get; set; }
        }
    }
}
