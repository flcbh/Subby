using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace Subby.Utilities.Pagination
{
    public static class PagedResultExtensions
    {
        public static PagedResult<T> Pagination<T>(this IQueryable<T> query, int page, int pageSize)
        {
            var result = new PagedResult<T>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = query.Count()
            };
 
            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
 
            var skip = (page - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();
 
            return result;
        }
    }
}