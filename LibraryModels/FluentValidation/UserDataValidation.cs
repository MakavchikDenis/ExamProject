using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using LibraryModels.Repository;
using System.Text.RegularExpressions;
using System;

namespace LibraryModels.FluentValidation
{
    public class UserDataValidation : AbstractValidator<UsersData>
    {

        Dictionary<string, string> listErrors = new Dictionary<string, string>();

        Func<string?,bool> helperForCheckEmail = (email) => {
            return email switch
            {
                null => true,
                _ => email.Contains("@") ? true : false

            };

        };



        public UserDataValidation() {


            // записываем ошибки
            SetListErrors();

            //Проверка  поля IdUser
            RuleFor(x => x.IdUser).NotNull().WithMessage(ReplaceMessage(listErrors["NotNull"], "IdUser")).
                Must(CheckForIdUser).WithMessage(ReplaceMessage(listErrors["NotZero"], "IdUser"));

            // Проверка поля FirstName
            RuleFor(x => x.FirstName).MinimumLength(2).WithMessage(ReplaceMessage(listErrors["NotRequiredLength"], "FirstName")).
                Must(CheckText).WithMessage(ReplaceMessage(listErrors["NotCorrectText"], "FirstName"));

            //Проверка поля LastName
            RuleFor(x => x.SecondName).
                Must(CheckText).WithMessage(ReplaceMessage(listErrors["NotCorrectText"], "SecondName")).
                MinimumLength(2).WithMessage(ReplaceMessage(listErrors["NotRequiredLength"], "SecondName"));

            // Проверка поля email
            RuleFor(x => x.Email).Must(helperForCheckEmail).WithMessage(ReplaceMessage(listErrors["NotCorrectText"], "Email"));


        }

        public bool CheckForIdUser(int idUser) => idUser == 0 ? false : true;

        public bool CheckText(string checkValue) => Regex.IsMatch(checkValue, new Regex(@"^\D+$", RegexOptions.IgnoreCase).ToString());

        void SetListErrors() {
            listErrors.Add("NotNull", "Значение VAR не должно иметь значение null");
            listErrors.Add("NotZero", "Значение VAR не должно иметь значение - 0");
            listErrors.Add("NotCorrectText", "Некорректный формат VAR");
            listErrors.Add("NotRequiredLength", "Длинна поля VAR не соответствует требованиям.");
            
        }

        string ReplaceMessage(string error, string replaceText) => error.Replace("VAR", replaceText);

        

        
    }
}
