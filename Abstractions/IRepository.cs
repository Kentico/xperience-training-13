using System.Collections.Generic;

namespace Abstractions
{
    public interface IRepository<TDto>
    {
        IEnumerable<TDto> GetAll();
    }
}
