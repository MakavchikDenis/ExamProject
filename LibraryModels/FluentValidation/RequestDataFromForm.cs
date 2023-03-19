using FluentValidation;
using Microsoft.Extensions.Primitives;

namespace LibraryModels.FluentValidation
{
    public class RequestDataFromForm : AbstractValidator<List<KeyValuePair<string, StringValues>>>
    {
        public RequestDataFromForm()
        {

            RuleFor<List<KeyValuePair<string, StringValues>>>(x => x).Must(CheckAllNamesParam).WithMessage("Некорректный формат в названии параматра.");

            RuleFor<List<KeyValuePair<string, StringValues>>>(x => x).Must(CheckAllValuesParam).WithMessage("Некорректный формат в значении параметра.");



        }

        bool CheckAllNamesParam(List<KeyValuePair<string, StringValues>> list) => list.All(x => x.Key.Contains("Vacancy"));

        bool CheckAllValuesParam(List<KeyValuePair<string, StringValues>> list) => list.All(x => Int32.TryParse(x.Value, out int tesult));

    }
}
