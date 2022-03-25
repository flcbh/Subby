using System;
using System.Collections.Generic;

namespace Subby.Utilities.Pagination
{
    public abstract class PagedResultBase
    {
        public int CurrentPage { get; set; }
 
        public int PageCount { get; set; }
 
        public int PageSize { get; set; }
 
        public int RowCount { get; set; }
        public string LinkTemplate { get; set; }
 
        public int FirstRowOnPage
        {
 
            get { return (CurrentPage - 1) * PageSize + 1; }
        }
 
        public int LastRowOnPage
        {
            get { return Math.Min(CurrentPage * PageSize, RowCount); }
        }
    }
 
    public class PagedResult<T> : PagedResultBase
    {
        public IList<T> Results { get; set; }
 
        public PagedResult()
        {
            Results = new List<T>();
        }
    }
}