using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Infra.CrossCuting
{
    public class PageRequestBase
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }

}
