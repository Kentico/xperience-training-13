using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions
{
    public interface IRepository<TModel>
    {
        IEnumerable<TModel> GetAll();
    }
}
