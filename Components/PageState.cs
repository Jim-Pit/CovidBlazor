using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbDesign;
using Microsoft.EntityFrameworkCore;

namespace CoViDAccountant.Components
{
    public class PageState
    {
        public async Task<List<T>> ExecutePagedQuery<T>(IQueryable<T> query)
            where T : class
        {
            var records = await query.CountAsync();

            TotalPages = (int)Math.Ceiling((double)records / PageSize);

            var results = await query.Skip(PageSize * (CurrentPage - 1))
                                     .Take(PageSize)
                                     .AsNoTracking()
                                     .ToListAsync();
            return results;
        }

        #region Filtering
        private string _filter;
        public string Filter
        {
            get => _filter;
            set
            {
                if (EqualityComparer<string>.Default.Equals(_filter, value))
                    return;
                _filter = value;
            }
        }
        #endregion

        #region Paging
        private int _pageSize = 5;
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (EqualityComparer<int>.Default.Equals(_pageSize, value))
                    return;
                _pageSize = value;
            }
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (EqualityComparer<int>.Default.Equals(_currentPage, value))
                    return;
                _currentPage = value;
            }
        }

        private int _totalPages;
        public int TotalPages
        {
            get => _totalPages;
            set
            {
                if (EqualityComparer<int>.Default.Equals(_totalPages, value))
                    return;
                _totalPages = value;

                if (CurrentPage > TotalPages)
                    CurrentPage = TotalPages;
                if (CurrentPage <= 0)
                    CurrentPage = 1;
            }
        }

        public static string[] DateFormats => new[]
        {
            "d/M/yyyy", "d-M-yyyy", "d M yyyy", "d/MMM/yyyy", "d-MMM-yyyy", "d MMM yyyy",
            "d/M/yyy", "d-M-yyy", "d M yyy", "d/MMM/yyy", "d-MMM-yyy", "d MMM yyy",
            "d/M/yy", "d-M-yy", "d M yy", "d/MMM/yy", "d-MMM-yy", "d MMM yy",
            //"dd/M/yyyy", "dd-M-yyyy", "dd M yyyy", "dd/MMM/yyyy", "dd-MMM-yyyy", "dd MMM yyyy",
            //"dd/M/yyy", "dd-M-yyy", "dd M yyy", "dd/MMM/yyy", "dd-MMM-yyy", "dd MMM yyy",
            //"dd/M/yy", "dd-M-yy", "dd M yy", "dd/MMM/yy", "dd-MMM-yy", "dd MMM yy" ,
        };
    #endregion
}
}
