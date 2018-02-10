using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.GeoBank.Services
{
    public interface IQueryService<T>
    {
        IQueryable<T> GetQueryable();
    }
}
