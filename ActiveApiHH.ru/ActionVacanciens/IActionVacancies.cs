namespace ActiveApiHH.ru
{
    internal interface IActionVacancies
    {
        string SearchVacancies(string Token, string SearchText);

        Task<List<string>> SearchDetailsVacanciesForUserAsync(string Token, string[] SearchVacancies);
    }
}
