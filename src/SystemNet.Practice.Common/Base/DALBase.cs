using Dapper;
using SystemNet.Practice.Common.Interface;

namespace SystemNet.Practice.Common
{
    public abstract class DALBase
    {

        protected enum Operations : byte
        {

            REVOKE = 1,
            GRANT = 2,
            INSERT = 3,
            DELETE = 4,
            UPDATE = 5,
            ASSOCIATESOFTLOCK = 6,
            BLOCKBYPASSWORDMASTER = 7

        }

        /// <summary>
        /// 
        /// </summary>
        protected object _ReturnLastCommandValue = -1;
        /// <summary>
        /// 
        /// </summary>
        public virtual object LastCommandValue { get { return _ReturnLastCommandValue; } }



        protected void InsertHistory(string instance, IContext _context, string procname, ref DynamicParameters parameter)
        {
            _context.ExecuteNonQuery(instance, procname, ref this._ReturnLastCommandValue, ref parameter);
        }



    }
}
