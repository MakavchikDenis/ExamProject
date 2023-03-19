namespace LibraryModels.Repository
{
    /// <summary>
    /// Интерфейс CRUD для Dapper
    /// </summary>
    public interface IRepositoryDapper<Entity> where Entity : class
    {
        public void Insert(Entity entity);
    }
}
