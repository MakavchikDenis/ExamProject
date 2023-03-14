namespace LocalApi.Service
{
    public interface IServiceForHangfire
    {
        public string RunRecurringJob(string Token, string searchVacancie);
    }
}
