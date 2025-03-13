﻿using ClickFlow.DAL.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs
{
    public class PagingDTO<T> where T : class
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
        public PaginatedList<T> Datas { get; set; }

        public PagingDTO(PaginatedList<T> pagedEntity)
        {
            PageIndex = pagedEntity.PageIndex;
            TotalPages = pagedEntity.TotalPages;
            TotalItems = pagedEntity.TotalItems;
            Datas = pagedEntity;
        }
    }
}
