namespace LibraryModels.Repository
{
    public class ModelForDetailsVacancy
    {


        public class ModelForRemoteApi
        {
            public string id { get; set; }
            public bool premium { get; set; }
            public Billing_Type? billing_type { get; set; }
            public object[] relations { get; set; }
            public string name { get; set; }
            public object? insider_interview { get; set; }
            public bool response_letter_required { get; set; }
            public Area? area { get; set; }
            public Salary? salary { get; set; }
            public Type? type { get; set; }
            public Address? address { get; set; }
            public bool allow_messages { get; set; }
            public Experience? experience { get; set; }
            public Schedule? schedule { get; set; }
            public Employment? employment { get; set; }
            public object department { get; set; }
            public object contacts { get; set; }
            public string description { get; set; }
            public object branded_description { get; set; }
            public object vacancy_constructor_template { get; set; }
            public object[]? key_skills { get; set; }
            public bool accept_handicapped { get; set; }
            public bool accept_kids { get; set; }
            public bool archived { get; set; }
            public object? response_url { get; set; }
            public object[]? specializations { get; set; }
            public Professional_Roles[]? professional_roles { get; set; }
            public object? code { get; set; }
            public bool hidden { get; set; }
            public bool quick_responses_allowed { get; set; }
            public object[]? driver_license_types { get; set; }
            public bool accept_incomplete_resumes { get; set; }
            public Employer? employer { get; set; }
            //public DateTime? published_at { get; set; }
            //public DateTime? created_at { get; set; }
            //public DateTime? initial_created_at { get; set; }
            public object? negotiations_url { get; set; }
            public object? suitable_resumes_url { get; set; }
            public string? apply_alternate_url { get; set; }
            public bool has_test { get; set; }
            public object? test { get; set; }
            public string alternate_url { get; set; }
            public object[]? working_days { get; set; }
            public object[]? working_time_intervals { get; set; }
            public object[]? working_time_modes { get; set; }
            public bool accept_temporary { get; set; }
            public Language[]? languages { get; set; }
        }

        public class Billing_Type
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
            public float? from { get; set; }
            public float? to { get; set; }
            public string? currency { get; set; }
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
            public float lat { get; set; }
            public float lng { get; set; }
            public object description { get; set; }
            public string raw { get; set; }
            public object metro { get; set; }
            public object[] metro_stations { get; set; }
        }

        public class Experience
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Schedule
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Employment
        {
            public string id { get; set; }
            public string name { get; set; }
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
            public string original { get; set; }
            public string _240 { get; set; }
            public string _90 { get; set; }
        }

        public class Professional_Roles
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Language
        {
            public string id { get; set; }
            public string name { get; set; }
            public Level level { get; set; }
        }

        public class Level
        {
            public string id { get; set; }
            public string name { get; set; }
        }



    }
}
