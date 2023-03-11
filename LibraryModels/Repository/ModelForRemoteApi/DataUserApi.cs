using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryModels.Repository
{
    public class DataUserApi
    {
        
        public string auth_type { get; set; }
        public bool is_applicant { get; set; }
        public bool is_employer { get; set; }
        public bool is_admin { get; set; }
        public bool is_application { get; set; }
        public string id { get; set; }
        public bool is_anonymous { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public object middle_name { get; set; }
        public string last_name { get; set; }
        public string resumes_url { get; set; }
        public string negotiations_url { get; set; }
        public bool is_in_search { get; set; }
        public object mid_name { get; set; }
        public object employer { get; set; }
        public object personal_manager { get; set; }
        public object manager { get; set; }
        public string phone { get; set; }
        public Counters counters { get; set; }
        public Profile_Videos profile_videos { get; set; }
    }

    public class Counters
    {
        public int new_resume_views { get; set; }
        public int unread_negotiations { get; set; }
        public int resumes_count { get; set; }
    }

    public class Profile_Videos
    {
        public object[] items { get; set; }
    }

}
