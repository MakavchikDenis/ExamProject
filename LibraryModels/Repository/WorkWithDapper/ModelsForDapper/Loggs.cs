﻿namespace LibraryModels.Repository
{
    public class Loggs
    {
        public int? Id { get; set; }
        public string? Token { get; set; }
        public DateTime DateAction { get; set; }
        public string Action { get; set; }
        public string ActionResult { get; set; }
        public string? ActionDetails { get; set; }
        public string? ErrorMessage { get; set; }

    }
}
