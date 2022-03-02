/**************************************************************************
 *                                                                        *
 *  JAVASCRIPT MENU HIGHLIGHTER v.1.5 (080929)                            *
 * --------------------------------------------                           *
 * ©2005 Media Division (www.MediaDivision.com)                           *
 *                                                                        *
 * Written by Marius Smarandoiu & Armand Niculescu                        *
 *                                                                        *
 * You are free to use, modify and distribute this file, but please keep  *
 * this header and credits                                                *
 *                                                                        *
 * Usage:                                                                 *
 * - the script will apply the .current class to the <a> and its parent   *
 *   <li> that is contained in the element with id="primarynav" and points*
 *   to the current URL                                                   *
 * - works in IE6, Firefox and Opera                                      *
 **************************************************************************/
function extractPageName(hrefString)
{
	var arr = hrefString.split('/');
	return  (arr.length<2) ? hrefString : arr[arr.length-2].toLowerCase() + arr[arr.length-1].toLowerCase();               
}

function setActiveMenu(arr, crtPage)
{
	for (var i=0; i<arr.length; i++)
	{
		if(extractPageName(arr[i].href) == crtPage)
		{
            
		    if (arr[i].parentNode.tagName != "DIV" && arr[i].id != "imgUserPhoto")
			{
			    arr[i].className = "active";
			    //arr[i].parentNode.className = "active";

                // We are using this code below to highlight sub menu parent
			    // Check whether the grandparent's class name is sub
			    /*if (arr[i].parentNode.parentNode.className == "sub" && arr[i].parentNode.parentNode.tagName == "UL") {
			        // We are at the main li now
			        var mainLI = arr[i].parentNode.parentNode.parentNode;
			        // Get the a tags in main li
			        var aTagsInMainLI = mainLI.getElementsByTagName("a");
			        //aTagsInMainLI[0].className = "active"
			        // Loop through all a tags in main li
			        for (var j = 0; j < aTagsInMainLI.length; j++) {
			            if (aTagsInMainLI[j].href == "javascript:;") {
			                // Set the a tag to active state
			                aTagsInMainLI[j].className = "active";
			                arr[i].className = "";
			                arr[i].parentNode.className = "";
			            }
			        }
			    }*/
			}

		    
		}
	}
}

function setPage()
{
	hrefString = document.location.href ? document.location.href : document.location;

	if (document.getElementById("sidebar") != null)
	    setActiveMenu(document.getElementById("sidebar").getElementsByTagName("a"), extractPageName(hrefString));
}

function setActiveHorizontalMenu()
{
    hrefString = document.location.href ? document.location.href : document.location;
    if (document.getElementById("horizontal_nav") != null)
        setActiveMenu(document.getElementById("horizontal_nav").getElementsByTagName("a"), extractPageName(hrefString));
}