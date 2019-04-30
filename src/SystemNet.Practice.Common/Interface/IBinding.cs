using System.Data;

namespace SystemNet.Practice.Common.Interface
{
    public interface IBinding
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        void Bind(IDataReader dr);
    }
}
