namespace LibraryModels.Repository
{
    public class ViewVacancies
    {
        public int Id { get; set; }

        public string Vacancy { get; set; }

        public DateTime Date { get; set; }

        public int User { get; set; }

        public byte[] Content { get; set; }
    }
}
