﻿using API.Models;
using LibraryModels.Repository.WorkWithEF;
using Microsoft.EntityFrameworkCore;

namespace LibraryModels.Repository
{
    public class Repository : IRepository, IRepositoryExtra
    {
        private Context _context;

        public Repository(string ConnectToDb) => _context = new Context(ConnectToDb);

        /// <summary>
        /// Возвращает все=> в зивисимости от типа передаемого парамтра при вызове метода 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> Set<T>() where T : class => _context.Set<T>().ToList();


        /// <summary>
        /// Поиск сущности по параметру (по Токену)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public Session? Find(string id) => _context.Sessions.Where(x => x.Acces_token == id).FirstOrDefault();

        /// <summary>
        /// Обновляем данные
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ob"></param>
        public void Update<T>(T ob) where T : class
        {
            _context.Update<T>(ob).State = EntityState.Modified;
            _context.SaveChanges();
        }


        /// <summary>
        /// Добавление
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ob"></param>
        public void Add<T>(T ob) where T : class
        {
            _context.Add<T>(ob);
            _context.SaveChanges();

        }

        /// <summary>
        /// Удаление 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ob"></param>
        public void Delete<T>(T ob) where T : class
        {
            _context.Remove<T>(ob);
            _context.SaveChanges();

        }

        /// <summary>
        /// из View вытягиваем данные по отслеживаемым вакансиям пользователя
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="textVacancie"></param>
        /// <returns></returns>
        public IQueryable<ViewVacancies> FindVacanciesForUser(string? idUser, string? textVacancie) =>
            _context.ViewVacancies.Where(x => x.User == Int32.Parse(idUser) && x.Vacancy == textVacancie);


        /// <summary>
        /// Получаем данные по вакансиям сохраненным для пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<DetailsVacanciesForUser> FindDetailsVacancies(string? user) => _context.DetailsVacanciesForUsers.ToList();
        //Where(x => x.idUser == Int32.Parse(user)).ToList();

    }
}
