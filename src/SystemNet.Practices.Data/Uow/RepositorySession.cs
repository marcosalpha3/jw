using System;
using System.Data;
using System.Data.SqlClient;
using SystemNet.Shared;

namespace SystemNet.Practices.Data.Uow
{
    public sealed class RepositorySession : IDisposable
    {
        public RepositorySession(string instanceDb)
        {
            _connection = new SqlConnection(Runtime.GetConnectionString(instanceDb));
            _connection.Open();
            _unitOfWork = new UnitOfWork(_connection);
        }

        IDbConnection _connection = null;
        UnitOfWork _unitOfWork = null;

        public UnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
            _connection.Dispose();
        }
    }
}