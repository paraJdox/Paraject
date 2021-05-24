﻿using Paraject.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paraject.Core.Services
{
    class UserCrudService<TEntity> : IMainCrudOperations<TEntity>
    {
        private readonly SqlConnection _sqlCon;
        private readonly SqlCommand _sqlCmd;

        public UserCrudService()
        {
            _sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["ParajectDbTest"].ConnectionString);
            _sqlCmd = new SqlCommand
            {
                Connection = _sqlCon,
                CommandType = CommandType.StoredProcedure
            };
        }

        public bool Add(TEntity entity)
        {




            return true;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public TEntity Get(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
