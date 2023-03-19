using Microsoft.AspNetCore.Mvc;
using LocalApi.Service;
using ActiveApiHH.ru;
using LibraryModels.Repository;
using LibraryModels;
using Microsoft.Extensions.Primitives;
using System.Linq;
using LibraryModels.FluentValidation;


namespace LocalApi.Controllers
{
    public partial class ActionVacanciensController
    {
        /// <summary>
        /// Записываем в БД конкретные вакансии, которые выбрал пользователь для сохранения
        /// с клиентского интерфейса приходит массив id вакансий
        /// после стучимся в сторонний АПИ =>получаем данные по каждой вакансии и все это записываем в БД
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        [HttpPost("SaveVacanciesUser")]
        public IActionResult SaveVacanciesUser([FromHeader] string Token)
        {
            try
            {
                // проверяем наличие токена в запросе
                if (Token is null) { throw new Exception(); }

                // проверяем наличие параметров в запросе
                if (!HttpContext.Request.HasFormContentType)
                {
                    throw new Exception("Отсуствуют параметры.");

                }

                // проверка название параметров и их значений
                // формат KeyValuePair<Vacancy1(2,3), id номер вакансии>
                var requestFromForm = HttpContext.Request.Form.ToList();

                RequestDataFromForm validationFormValues = new RequestDataFromForm();

                var validationResult = validationFormValues.Validate(requestFromForm);

                if (!validationResult.IsValid)
                {

                    throw new Exception("Некоррекные данные", new Exception(validationResult.Errors.First().ErrorMessage));


                }


                Dictionary<string, string> request = new Dictionary<string, string>();

                requestFromForm.ForEach(x => request.Add(x.Key, x.Value));

                string[] arrayRequestIdVacancy = request.Values.ToArray();

                // получаем все данные по каждой вакансии через запрос в сторонний АПИ
                List<string> arrayDetailsForVacancies = activeForApi.SearchDetailsVacanciesAsync(Token, arrayRequestIdVacancy).Result;


                List<ModelForDetailsVacancy.ModelForRemoteApi> arrayDeserializeForModel = new List<ModelForDetailsVacancy.ModelForRemoteApi>();

                foreach (var i in arrayDetailsForVacancies)
                {
                    arrayDeserializeForModel.Add(handler.Reverse<ModelForDetailsVacancy.ModelForRemoteApi>(i));

                }


                List<DetailsVacanciesForUser> vacanciesForUsers = new List<DetailsVacanciesForUser>();

                // получаем данные из нашей БД по user

                int idUser = repositoryExtra.Find(Token).IdUser.Value;

                arrayDeserializeForModel.ToList().ForEach(x => vacanciesForUsers.Add(new DetailsVacanciesForUser(idUser, x.name, x.alternate_url,
                    x.salary?.from, x.salary?.to, x.salary?.currency, x.employer?.name, x.description)));

                // все данные вносим в БД
                foreach (var i in vacanciesForUsers)
                {
                    repository.Add<DetailsVacanciesForUser>(i);

                }


                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                   (string)RouteData.Values["action"]), "Succes", Token);

                repositoryDapper.Insert(loggs);

                return NoContent();




            }
            catch (Exception e) when (Token is null)
            {
                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, "Не передан токен",
                    "Не передан токен.");

                (int level, string description, string message) = error;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Error", _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                return BadRequest(ErrorForDB);


            }
            catch (Exception e) when (e.Message == "Отсуствуют параметры." || e.Message == "Некоррекные данные")
            {

                string errorMessage = e.Message == "Отсуствуют параметры." ? e.Message : String.Join(":", e.Message, e.InnerException.Message);

                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, errorMessage, errorMessage);


                (int level, string description, string message) = error;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Error", _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                return BadRequest(ErrorForDB);



            }
            catch (Exception e)
            {

                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, e.Message, "Системная ошибка.");

                (int level, string description, string message) = error;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Error", _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                return Problem(detail: "Системная ошибка программы.", statusCode: StatusCodes.Status500InternalServerError);

            }

        }


        /// <summary>
        /// Возвращаем вакансии, которые были выбраны пользователем для сохранения
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        [HttpGet("GetVacanciesUser")]
        public IActionResult GetVacanciesUser([FromHeader] string Token)
        {

            try
            {
                // проверяем наличие токена в запросе
                if (Token is null) { throw new Exception(); }

                // проверяем наличие параметров в запросе
                if (HttpContext.Request.Query.Count==default)
                {
                    throw new Exception("Отсуствуют параметры.");

                }

                string idUser = HttpContext.Request.Query["idUser"];

                List<DetailsVacanciesForUser> detailsVacanciesForUsers = repositoryExtra.FindDetailsVacancies(idUser);
               
                return Ok(detailsVacanciesForUsers);

            }
            catch (Exception) when (Token is null)
            {
                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, "Не передан токен",
                    "Не передан токен.");

                (int level, string description, string message) = error;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Error", _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                return BadRequest(ErrorForDB);


            }
            catch (Exception e)
            {

                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, e.Message, "Системная ошибка.");

                (int level, string description, string message) = error;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Error", _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                return Problem(detail: "Системная ошибка программы.", statusCode: StatusCodes.Status500InternalServerError);

            }


        }
    }
}
