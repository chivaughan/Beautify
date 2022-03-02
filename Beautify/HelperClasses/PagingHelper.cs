using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Beautify
{
    public static class PagingHelper
    {
        public static PagingInfo GetAllLinks(int totalRecordsInTable, int pageSize, int previousIndex)
        {

            string LinkButtonIDPrefix = "lnK";
            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.PaginationLinks = new Collection<LinkButton>();

            if (totalRecordsInTable > 0)
            {
                int itemsBeforePage = 4;
                int itemsAfterPage = 2;
                int dynamicDisplayCount = itemsBeforePage + 1 + itemsAfterPage;

                Double numberOfPagesRequired = Convert.ToDouble(totalRecordsInTable / pageSize);
                if (totalRecordsInTable % pageSize != 0)
                {
                    numberOfPagesRequired = numberOfPagesRequired + 1;
                }

                if (numberOfPagesRequired == 0)
                {
                    numberOfPagesRequired = 1;
                }


                //Note: This function adds only the probable Links that the user can click (based on previous click).
                //This is needed sice dynamic controls need to be added while Page_Load itself for event handlers to work
                //In case of any bug, easiest way is add all links from 1 to numberOfPagesRequired
                //Following is an optimized way

                int endOfLeftPart = dynamicDisplayCount;
                //User may click "1". So the first 7 items may be required for display. Hence add them for event handler purpose
                for (int i = 1; i <= endOfLeftPart; i++)
                {
                    //Create dynamic Links 
                    LinkButton lnk = new LinkButton();
                    lnk.ID = LinkButtonIDPrefix + i.ToString(CultureInfo.InvariantCulture);
                    lnk.Text = i.ToString(CultureInfo.InvariantCulture);
                    pagingInfo.PaginationLinks.Add(lnk);
                }


                int startOfRighPart = Convert.ToInt32(numberOfPagesRequired) - dynamicDisplayCount + 1;

                //User may click the last link. So the last 7 items may be required for display. Hence add them for event handler purpose
                for (int i = startOfRighPart; i <= Convert.ToInt32(numberOfPagesRequired); i++)
                {
                    //Links already added should not be added again
                    if (i > endOfLeftPart)
                    {
                        //Create dynamic Links 
                        LinkButton lnk = new LinkButton();
                        lnk.ID = LinkButtonIDPrefix + i.ToString(CultureInfo.InvariantCulture);
                        lnk.Text = i.ToString(CultureInfo.InvariantCulture);
                        pagingInfo.PaginationLinks.Add(lnk);
                    }
                }

                //User may click on 4 items before current index as well as 2 items after current index
                for (int i = (previousIndex - itemsBeforePage); i <= (previousIndex + itemsAfterPage); i++)
                {
                    //Links already added should not be added again
                    if (i > endOfLeftPart && i < startOfRighPart)
                    {
                        //Create dynamic Links 
                        LinkButton lnk = new LinkButton();
                        lnk.ID = LinkButtonIDPrefix + i.ToString(CultureInfo.InvariantCulture);
                        lnk.Text = i.ToString(CultureInfo.InvariantCulture);
                        pagingInfo.PaginationLinks.Add(lnk);
                    }
                }



            }
            return pagingInfo;
        }
        public static PagingInfo GetPageLinks(int totalRecordsInTable, int pageSize, int currentIndex)
        {
            string LinkButtonIDPrefix = "lnK";
            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.PaginationLinks = new Collection<LinkButton>();

            if (totalRecordsInTable > 0)
            {

                int itemsBeforePage = 4;
                int itemsAfterPage = 2;
                int dynamicDisplayCount = itemsBeforePage + 1 + itemsAfterPage;

                Double numberOfPagesRequired = Convert.ToDouble(totalRecordsInTable / pageSize);
                if (totalRecordsInTable % pageSize != 0)
                {
                    numberOfPagesRequired = numberOfPagesRequired + 1;
                }

                if (numberOfPagesRequired == 0)
                {
                    numberOfPagesRequired = 1;
                }

                //Generate dynamic paging 
                int start;
                if (currentIndex <= (itemsBeforePage + 1))
                {
                    start = 1;
                }
                else
                {
                    start = currentIndex - itemsBeforePage;
                }

                int lastAddedLinkIndex = 0;
                int? firtsAddedLinkIndex = null;

                for (int i = start; i < start + dynamicDisplayCount; i++)
                {

                    if (i > numberOfPagesRequired)
                    {
                        break;
                    }

                    //Create dynamic Links 
                    LinkButton lnk = new LinkButton();
                    lnk.ID = LinkButtonIDPrefix + i.ToString(CultureInfo.InvariantCulture);
                    lnk.Text = i.ToString(CultureInfo.InvariantCulture);
                    lastAddedLinkIndex = i;

                    if (firtsAddedLinkIndex == null)
                    {
                        firtsAddedLinkIndex = i;
                    }


                    //Check whetehr current page
                    if (i == currentIndex)
                    {
                        lnk.CssClass = "page-numbers current";
                    }
                    else
                    {
                        lnk.CssClass = "page-numbers";
                    }

                    pagingInfo.PaginationLinks.Add(lnk);
                }

                if (numberOfPagesRequired > dynamicDisplayCount)
                {
                    //Set dots (ellipse) visibility
                    pagingInfo.IsEndDotsVisible = lastAddedLinkIndex == numberOfPagesRequired ? false : true;
                    pagingInfo.IsStartDotsVisible = firtsAddedLinkIndex <= 2 ? false : true;

                    //First and Last Page Links
                    pagingInfo.IsLastLinkVisible = lastAddedLinkIndex == numberOfPagesRequired ? false : true;
                    pagingInfo.IsFirstLinkVisible = firtsAddedLinkIndex == 1 ? false : true;

                }

                pagingInfo.NumberOfPagesRequired = Convert.ToInt32(numberOfPagesRequired);

            }
            return pagingInfo;

        }
    }
}