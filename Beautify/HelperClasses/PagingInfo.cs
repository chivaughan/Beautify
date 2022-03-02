using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Beautify
{
    public class PagingInfo
    {
        public Collection<LinkButton> PaginationLinks { get; set; }
        public bool? IsEndDotsVisible { get; set; }
        public bool? IsStartDotsVisible { get; set; }
        public bool? IsFirstLinkVisible { get; set; }
        public bool? IsLastLinkVisible { get; set; }
        public int NumberOfPagesRequired { get; set; }
    }
}