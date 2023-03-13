using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryModels.Repository
{
    public class ModelVacancies
    {
        public class ListVacancies
        {
            public Item[] items { get; set; }
            public int found { get; set; }
            public int pages { get; set; }
            public int per_page { get; set; }
            public int page { get; set; }
            public object clusters { get; set; }
            public object arguments { get; set; }
            public string alternate_url { get; set; }
        }

        public class Item
        {
            public string id { get; set; }
            public bool premium { get; set; }
            public string name { get; set; }
            public Department department { get; set; }
            public bool has_test { get; set; }
            public bool response_letter_required { get; set; }
            public Area area { get; set; }
            public Salary salary { get; set; }
            public Type type { get; set; }
            public Address address { get; set; }
            public object response_url { get; set; }
            public object sort_point_distance { get; set; }
            public DateTime published_at { get; set; }
            public DateTime created_at { get; set; }
            public bool archived { get; set; }
            public string apply_alternate_url { get; set; }
            public object insider_interview { get; set; }
            public string url { get; set; }
            public object adv_response_url { get; set; }
            public string alternate_url { get; set; }
            public object[] relations { get; set; }
            public Employer employer { get; set; }
            public Snippet snippet { get; set; }
            public object contacts { get; set; }
            public object schedule { get; set; }
            public object[] working_days { get; set; }
            public Working_Time_Intervals[] working_time_intervals { get; set; }
            public Working_Time_Modes[] working_time_modes { get; set; }
            public bool accept_temporary { get; set; }
            public Professional_Roles[] professional_roles { get; set; }
            public bool accept_incomplete_resumes { get; set; }
        }

        public class Department
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Area
        {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }

        public class Salary
        {
            public int from { get; set; }
            public int? to { get; set; }
            public string currency { get; set; }
            public bool gross { get; set; }
        }

        public class Type
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Address
        {
            public string city { get; set; }
            public string street { get; set; }
            public string building { get; set; }
            public float? lat { get; set; }
            public float? lng { get; set; }
            public object description { get; set; }
            public string raw { get; set; }
            public Metro metro { get; set; }
            public Metro_Stations[] metro_stations { get; set; }
            public string id { get; set; }
        }

        public class Metro
        {
            public string station_name { get; set; }
            public string line_name { get; set; }
            public string station_id { get; set; }
            public string line_id { get; set; }
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Metro_Stations
        {
            public string station_name { get; set; }
            public string line_name { get; set; }
            public string station_id { get; set; }
            public string line_id { get; set; }
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Employer
        {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string alternate_url { get; set; }
            public Logo_Urls logo_urls { get; set; }
            public string vacancies_url { get; set; }
            public bool trusted { get; set; }
        }

        public class Logo_Urls
        {
            public string _90 { get; set; }
            public string _240 { get; set; }
            public string original { get; set; }
        }

        public class Snippet
        {
            public string requirement { get; set; }
            public string responsibility { get; set; }
        }

        public class Working_Time_Intervals
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Working_Time_Modes
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Professional_Roles
        {
            public string id { get; set; }
            public string name { get; set; }
        }

    }
}
