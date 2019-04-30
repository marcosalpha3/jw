using Dapper;

namespace SystemNet.Practice.Common.Interface
{
    public interface IContext
    {
        void ExecuteNonQuery(string instanceDb, string procedureName, ref object returnCommandValue, ref DynamicParameters parameters);
    }
}
