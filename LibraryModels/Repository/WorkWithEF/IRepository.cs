namespace LibraryModels.Repository
{
    public interface IRepository
    {
        public List<T> Set<T>() where T : class;
        public void Update<T>(T ob) where T : class;
        public void Add<T>(T ob) where T : class;
        public void Delete<T>(T ob) where T : class;

    }
}
