using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using SystemNet.Practice.Common.Interface;
using SystemNet.Shared;

namespace SystemNet.Practices.Data
{

    public class SqlFactory : IContext
    {


        public IEnumerable<T> GetListDbWithMapper<T>(string instanceDb, string procedureName, 
            ref object returnCommandValue, ref DynamicParameters parameters)
        {
            return GetListDbGeneric<T>(instanceDb, procedureName, ref returnCommandValue, true, ref parameters);
        }
        
        public IEnumerable<T> GetListDb<T>(string instanceDb, string procedureName,
            ref object returnCommandValue, ref DynamicParameters parameters)
        {
            return GetListDbGeneric<T>(instanceDb, procedureName, ref returnCommandValue, false, ref parameters);
        }


        public T GetObjectDbWithMapper<T>(string instanceDb, string procedureName,
            ref object returnCommandValue, ref DynamicParameters parameters)
        {
            return GetListDbGeneric<T>(instanceDb, procedureName, ref returnCommandValue, true, ref parameters).FirstOrDefault();
        }

        public T GetObjectDb<T>(string instanceDb, string procedureName,
            ref object returnCommandValue, ref DynamicParameters parameters)
        {
            return GetListDbGeneric<T>(instanceDb, procedureName, ref returnCommandValue, false, ref parameters).FirstOrDefault();
        }


        private IEnumerable<T> GetListDbGeneric<T>(string instanceDb, string procedureName,
                ref object returnCommandValue, bool mappingmapper, ref DynamicParameters parameters)
        {
            parameters.Add("@return", 0, DbType.Object, ParameterDirection.ReturnValue);
            using (var conn = new SqlConnection(Runtime.GetConnectionString(instanceDb)))
            {
                if (mappingmapper)  MappingModel<T>();
                conn.Open();
                var list = conn.Query<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);

                returnCommandValue = parameters.Get<object>("@return");

                return list;
            }
        }

        
        public void ExecuteNonQuery(string instanceDb, string procedureName, ref object returnCommandValue, ref DynamicParameters parameters)
        {
            parameters.Add("@return", 0, DbType.Object, ParameterDirection.ReturnValue);
            using (var conn = new SqlConnection(Runtime.GetConnectionString(instanceDb)))
            {
                conn.Open();
                conn.Execute(procedureName, param: parameters, commandType: CommandType.StoredProcedure);
                returnCommandValue = parameters.Get<object>("@return");
                return;
            }
        }


        public virtual string GetDescriptionFromAttribute(MemberInfo member)
        {
            if (member == null) return null;

            var attrib = (DescriptionAttribute)Attribute.GetCustomAttribute(member, typeof(DescriptionAttribute), false);
            return attrib?.Description;
        }

        public virtual void MappingModel<T>()
        {

            var map = new CustomPropertyTypeMap(typeof(T),
                (type, columnName) => type.GetProperties().FirstOrDefault(prop => GetDescriptionFromAttribute(prop) == columnName));
            SqlMapper.SetTypeMap(typeof(T), map);
        }


    }

}
