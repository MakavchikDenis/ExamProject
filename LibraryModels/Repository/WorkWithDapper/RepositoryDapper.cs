using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace LibraryModels.Repository
{


    /// <summary>
    ///  Логирование действий 
    /// </summary>
    public class RepositoryDapper:IRepositoryDapper<Loggs>
    {
        private string Connect;

        public RepositoryDapper(string _Connect) => this.Connect = _Connect;

        /// <summary>
        /// Добавление в таблицу LoggsUsers
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Insert(Loggs entity)
        {
            using IDbConnection dbConnection = new SqlConnection(Connect);

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Token", entity.Token);
            parameters.Add("@DateAction", entity.DateAction);
            parameters.Add("@Action", entity.Action);
            parameters.Add("@ActionResult", entity.ActionResult);
            parameters.Add("@ActionDetails", entity.ActionDetails);
            parameters.Add("@ErrorMessage", entity.ErrorMessage);

            string Sqlprocedure = "dbo.InsertLoggsusers";

            dbConnection.Query(Sqlprocedure, parameters, commandType: CommandType.StoredProcedure);


        }
    }
}
