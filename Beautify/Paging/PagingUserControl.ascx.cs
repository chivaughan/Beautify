using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Beautify.Paging
{
    public partial class PagingUserControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        public int PreviousIndex { get; set; }
        public int CurrentClickedIndex { get; set; }

        public event EventHandler PaginationLinkClicked;

        protected void LinkButton_Click(object sender, EventArgs e)
        {
            //Assumption: Text of the LinkButton will be same as index
            LinkButton clickedLinkButton = (LinkButton)sender;

            if (String.Equals(clickedLinkButton.Text, "Next") || String.Equals(clickedLinkButton.Text, "<i class='fa fa-arrow-right'></i>"))
            {
                //Next Page index will be one greater than current
                //Note: If the current index is the last page, "Next" control will be in disabled state
                CurrentClickedIndex = PreviousIndex + 1;
            }
            else if (String.Equals(clickedLinkButton.Text, "Prev") || String.Equals(clickedLinkButton.Text,"<i class='fa fa-arrow-left'></i>"))
            {
                //Previous Page index will be one less than current
                //Note: If the current index is the first page, "Prev" control will be in disabled state
                CurrentClickedIndex = PreviousIndex - 1;
            }
            else
            {
                CurrentClickedIndex = Convert.ToInt32(clickedLinkButton.Text, CultureInfo.InvariantCulture);
            }

            //Raise event
            if (this.PaginationLinkClicked != null)
            {
                this.PaginationLinkClicked(clickedLinkButton, e);
            }

        }

        public void PreAddAllLinks(int tableDataCount, int pageSize, int currentIndex)
        {
            if (tableDataCount > 0)
            {
                PagingInfo info = PagingHelper.GetAllLinks(tableDataCount, pageSize, currentIndex);

                //Remove all controls from the placeholder
                plhDynamicLink.Controls.Clear();

                if (info.PaginationLinks != null)
                {
                    foreach (LinkButton link in info.PaginationLinks)
                    {
                        //Adding Event handler must be done inside Page_Laod /Page_Init
                        link.Click += new EventHandler(LinkButton_Click);
                        
                        //Validation controls should be executed before link click.
                        link.ValidationGroup = "Search";
                        this.plhDynamicLink.Controls.Add(link);
                    }
                }

            }
        }

        public void AddPageLinks(int tableDataCount, int pageSize, int index)
        {

            if (tableDataCount > 0)
            {
                pagingSection.Visible = true;
                PagingInfo info = PagingHelper.GetPageLinks(tableDataCount, pageSize, index);

                //Remove all controls from the placeholder
                plhDynamicLink.Controls.Clear();

                if (info.PaginationLinks != null)
                {
                    lnkPrevious.Visible = info.PaginationLinks.Count > 0 ? true : false;
                    lnkNext.Visible = info.PaginationLinks.Count > 0 ? true : false;

                    foreach (LinkButton link in info.PaginationLinks)
                    {
                        //Validation controls should be executed before link click.
                        link.ValidationGroup = "Search";
                        this.plhDynamicLink.Controls.Add(link);
                    }
                }


                //Dots visiblity
                if (info.IsEndDotsVisible != null)
                {
                    lblSecondDots.Visible = Convert.ToBoolean(info.IsEndDotsVisible, CultureInfo.InvariantCulture);
                }
                else
                {
                    lblSecondDots.Visible = false;
                }

                if (info.IsStartDotsVisible != null)
                {
                    lblFirstDots.Visible = Convert.ToBoolean(info.IsStartDotsVisible, CultureInfo.InvariantCulture);
                }
                else
                {
                    lblFirstDots.Visible = false;
                }

                //First and Last Links
                if (info.IsFirstLinkVisible != null)
                {
                    lnkFirst.Visible = Convert.ToBoolean(info.IsFirstLinkVisible, CultureInfo.InvariantCulture);
                }
                else
                {
                    lnkFirst.Visible = false;
                }

                if (info.IsLastLinkVisible != null)
                {
                    lnkLast.Visible = Convert.ToBoolean(info.IsLastLinkVisible, CultureInfo.InvariantCulture);
                    lnkLast.Text = info.NumberOfPagesRequired.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    lnkLast.Visible = false;
                }


                //For first page, there is no previous
                if (index != 1 && info.NumberOfPagesRequired != 1)
                {
                    lnkPrevious.Enabled = true;
                }
                else
                {
                    lnkPrevious.Enabled = false;
                }


                //For last page there is no Next
                if (index != info.NumberOfPagesRequired && info.NumberOfPagesRequired != 1)
                {
                    lnkNext.Enabled = true;
                }
                else
                {
                    lnkNext.Enabled = false;
                }
            }
            else
            {
                pagingSection.Visible = false;
            }

        }
    }
}