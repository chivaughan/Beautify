<%@ Page Title="" Language="C#" MasterPageFile="~/FrontEnd.Master" AutoEventWireup="true" CodeBehind="ChooseServices.aspx.cs" Inherits="Beautify.WebForm3" %>
<%@ Register TagPrefix="CP" TagName="DevasolPager" Src="Paging/PagingUserControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPageContent" runat="server">
    <link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.min.css">
    <link rel="stylesheet" href="../content/frontend/css/star-rating.css" media="all" type="text/css"/>

       

    <!-- Media Container -->
            <div class="media-container">
                <!-- Intro -->
            <section class="site-section site-section-light site-section-top themed-background-dark">
                <div class="container text-center">
                    <h1 class="animation-slideDown"><strong runat="server" id="lblSalonName"></strong></h1>
                    <h2 class="h3 animation-slideUp">Add services to your cart</h2>
                </div>
            </section>
            <!-- END Intro -->

                <!-- For best results use an image with a resolution of 2560x279 pixels -->
                <img src="../content/frontend/img/placeholders/headers/store_home.jpg" alt="" class="media-image animation-pulseSlow">
            </div>
            <!-- END Media Container -->

            <!-- Product View -->
            <section class="site-content site-section">
                <div class="container">
                    <input type="hidden" runat="server" id="lblSalonUsername" />
                    <div class="row">
                        <!-- Sidebar -->
                        <div class="col-md-4 col-lg-3">
                            <aside class="sidebar site-block">
                                
                                <!-- Shopping Cart -->
                                <div class="sidebar-block">
                                    <div class="row" runat="server" id="divSalonInfo">
                                       
                                       
                                    </div>
                                </div>
                                <!-- END Shopping Cart -->
                            </aside>
                        </div>
                        <!-- END Sidebar -->

                        <!-- Product Details -->
                        <div class="col-md-8 col-lg-9">
                            <div class="row" data-toggle="lightbox-gallery">
                                
                                <!-- More Info Tabs -->
                                <div class="col-xs-12 site-block">
                                    <ul class="nav nav-tabs push-bit" data-toggle="tabs">
                                        <li id="tabServices" class="active"><a href="#salon-services" onclick="SetActiveTab('salon-services');">SERVICES</a></li>
                                        <li id="tabReviews"><a href="#salon-reviews" onclick="SetActiveTab('salon-reviews');">REVIEWS</a></li>
                                        <li id="tabPortfolio"><a href="#salon-portfolio" onclick="SetActiveTab('salon-portfolio');">PORTFOLIO</a></li>
                                        <li id="tabAbout"><a href="#salon-about" onclick="SetActiveTab('salon-about');">ABOUT US/ADDRESS</a></li>
                                    </ul>
                                    

                                    
                                    <div class="tab-content" id="tabContainer">
                                        
                                        
                                        <div class="tab-pane active" id="salon-services">
                                            <!-- Products -->
                        <div class="col-md-8 col-lg-12">
                            <div class="form-inline push-bit clearfix">
                                <select runat="server" id="selServiceCategory" name="results-show" class="form-control pull-left" size="1">
                                    
                                </select>
                                <select id="selServicePageSize" runat="server" class="form-control pull-left" size="1">
                                    <option value="0" disabled>SHOW</option>
                                    <option value="6" selected="selected">6</option>
                                    <option value="12">12</option>
                                    <option value="18">18</option>
                                    <option value="24">24</option>
                                    <option value="30">30</option>
                                </select>
                                
                                <asp:LinkButton runat="server" ID="lnkRefreshServices" CssClass="btn btn-primary pull-right" ToolTip="Refresh" OnClick="lnkRefreshServices_Click"><i class="fa fa-refresh fa-2x"></i></asp:LinkButton>
                                 
                            </div>
                            <div class="row store-items" runat="server" id="divServices">
                                
                            </div>
                            <CP:DevasolPager ID="uclPagerServices" runat="server" />
 <asp:HiddenField ID="hdnCurrentIndexServices" runat="server" Value="Blank Value" />
                        </div>
                        <!-- END Products -->

                                        </div>
                                        <div class="tab-pane" id="salon-reviews">
                                            <div runat="server" id="divReviews">
                                            
                                                </div>
                                            <CP:DevasolPager ID="uclPagerReviews" runat="server" />
 <asp:HiddenField ID="hdnCurrentIndexReviews" runat="server" Value="Blank Value" />
                                            <br /><br /><br /><br />
                                            <div class="form-horizontal form-borderless">
                                    <div id="faq1" class="panel-group">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                <h4 class="panel-title"><i class="fa fa-angle-right"></i> <a class="accordion-toggle" data-toggle="collapse" data-parent="#faq1" href="#faq1_q1">Review this Salon</a></h4>
                                            </div>
                                            <div id="faq1_q1" class="panel-collapse collapse">
                                                <div class="panel-body">
                                                    <div class="alert alert-info">
                                                        <h4><i class="fa fa-exclamation-circle"></i> Review this Salon</h4> You can only review this salon if you have booked it before. Each booking qualifies you for a single review
                                                    </div>
                                                    <div id="divLoading_Review" hidden="hidden" align="center">
                                <i class="fa fa-spinner fa-2x fa-spin"></i>
                            </div>
                                                    <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtReview_BookingID">Booking ID</label>
                                                <div class="col-md-6">
                                                    <input type="text" maxlength="50" ID="txtReview_BookingID" class="form-control" placeholder="Booking ID" />
                                                </div>
                                            </div>
                                                    <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtReview_ReviewCode">Review Code</label>
                                                <div class="col-md-6">
                                                    <input type="text" maxlength="20" ID="txtReview_ReviewCode" class="form-control" placeholder="Review Code" />
                                                </div>
                                            </div>
                                                <div class="form-group">
                                                    <label class="col-md-3 control-label" for="txtReview_Rating">Rating</label>
                                                <div class="col-md-6">
                                                    <input id="txtReview_Rating" class="rating" min="0" max="5" step="0.5" data-size="xs" data-symbol="&#xf005;" data-glyphicon="false" 
                                                    data-rating-class="rating-fa" value="0">  
                                                </div>
                                                </div>
                                                <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtReview_Comment">Comment</label>
                                                <div class="col-md-6">
                                                    <textarea maxlength="300" rows="3" ID="txtReview_Comment" class="form-control" placeholder="Comment"></textarea>
                                                </div>
                                            </div>
                                                    <br />
                                                    <div class="form-group text-right">
                                <button type="button" class="btn btn-primary" id="btnSubmitReview" onclick="SubmitReview();" value="" >Submit Review</button>
                            </div>
                                                </div>
                                            </div>
                                        </div>
                                       
                                    </div>
                                </div>
                                            </div>
                                            
                                        <div class="tab-pane" id="salon-portfolio">
                                            <!-- Products -->
                        <div class="col-md-8 col-lg-12">
                            <div class="form-inline push-bit clearfix">
                                <select runat="server" id="selPortfolioServiceCategory" name="results-show" class="form-control pull-left" size="1">
                                    
                                </select>
                                <select id="selPortfolioPageSize" runat="server" class="form-control pull-left" size="1">
                                    <option value="0" disabled>SHOW</option>
                                    <option value="6" selected="selected">6</option>
                                    <option value="12">12</option>
                                    <option value="18">18</option>
                                    <option value="24">24</option>
                                    <option value="30">30</option>
                                </select>
                                
                                <asp:LinkButton runat="server" ID="lnkRefreshPortfolio" CssClass="btn btn-primary pull-right" ToolTip="Refresh" OnClick="lnkRefreshPortfolio_Click"><i class="fa fa-refresh fa-2x"></i></asp:LinkButton>
                                 
                            </div>
                            <div class="row store-items" runat="server" id="divPortfolio">
                                
                            </div>
                            <CP:DevasolPager ID="uclPagerPortfolio" runat="server" />
 <asp:HiddenField ID="hdnCurrentIndexPortfolio" runat="server" Value="Blank Value" />
                        </div>
                        <!-- END Products -->

                                        </div>
                                            
                                        <div class="tab-pane" id="salon-about">
                                            <div runat="server" id="divAbout">
                                            
                                                </div>
                                            <br /><br />
                                        </div>
                                    </div>
                                       
                                    
                                    
                                    <!-- Shopping Cart -->
                                    <div class="row" id="divShoppingCart">
                                        <div class="col-xs-6 push-bit" id="divCartDetails">
                                            <span class="h3"><%=Beautify.AppHelper.GetCurrencySymbol()%> 0<br><small><em>0 Item</em></small></span>
                                        </div>
                                        <div class="col-xs-6">
                                            <button type="button" onclick = "GoToPage('ShoppingCart.aspx');" class="btn btn-sm btn-block btn-success">VIEW CART</button>
                                            <button type="button" onclick ="GoToPage('Checkout.aspx');" class="btn btn-sm btn-block btn-danger">CHECKOUT</button>
                                        </div>
                                    </div>
                                
                                    <!-- End Shopping Cart -->
                                </div>
                                <!-- END More Info Tabs -->
                            </div>
                        </div>
                        <!-- END Product Details -->
                    </div>
                </div>
            </section>
            <!-- END Product View -->
    

    <!--Star Rating-->   
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
    <script src="../content/frontend/js/star-rating.js" type="text/javascript"></script>

    <!-- Toast -->    
    <!-- Session-->
        <script type="text/javascript" src="../content/js-session/json-serialization.js"></script>
<script type="text/javascript" src="../content/js-session/session.js"></script>
    <script type="text/javascript">
        // initialize application defaults
        var shoppingCart = [];
        if (Session.get("cart") != null) {
            shoppingCart = Session.get("cart");
        }
        
                       
        
        function AddItemToCart(serviceID, serviceCost) {
            var cartItem = { id: serviceID, quantity:1, amount: serviceCost };
            shoppingCart.push(cartItem);

            // Show the Remove button for this cart item and Hide the Add button
            $("#removeItem-" + serviceID).show();
            $("#addItem-" + serviceID).hide();

            // Compute the total amount of Cart items
            ComputeCartTotalAmount();
            $.fn.dpToast("Item Added to Cart");
        }

        function RemoveItemFromCart(serviceID) {
            var index;
            for (index = 0; index < shoppingCart.length; index++) {
                if (shoppingCart[index].id == serviceID) {
                    // Remove the item from the cart
                    shoppingCart.splice(index, 1);
                }                
            }

            // Show the Add button for this cart item and Hide the Remove button
            $("#removeItem-" + serviceID).hide();
            $("#addItem-" + serviceID).show();

            // Compute the total amount of Cart items
            ComputeCartTotalAmount();
            $.fn.dpToast("Item Removed from Cart");
        }

        function ComputeCartTotalAmount() {
            var index;
            var totalAmount = 0;
            for (index = 0; index < shoppingCart.length; index++) {
                // Sum the amount of all items in the cart
                totalAmount = totalAmount + shoppingCart[index].amount;
            }

            // Save the updated cart to the cookie
            setCookie('cart', serialize(shoppingCart));

            $("#divCartDetails").html("<span class='h3'><%=Beautify.AppHelper.GetCurrencySymbol() + " "%>" + totalAmount + "<br><small><em>" + FormatTextSingularAndPlural(shoppingCart.length, "Item", " ", " ") + "</em></small></span>");
        }

        function GoToPage(page) {
            window.open(page, '_parent', null, false);
        }

        function NotAvailableForBooking() {
            // Tell the user the salon is not available for booking
            alert('This salon is currently not available for booking. Kindly Go Back and choose a different salon');
        }

        function GoBack() {
            window.history.back(1);
        }

        function serialize(object) {
            return JSON.stringify(object);
        }

        function deserialize(string) {
            var object = JSON.parse(string);
            return object;
        }

        function FormatTextSingularAndPlural(quantity, singularText, leadingText, betweenText)
        {
            
            if (quantity <= 1)
            {
                // The quantity is less than or equal to 1 so the singular form of the text should be returned
                return leadingText + quantity + betweenText + singularText;
            }
            else
            {
                // The quantity is greater than 1 so the plural form of the text should be returned
                if (singularText.substring(singularText.length - 1) == "y")
                {
                    // The singular text ends with y, so let us change it to ies
                    // But first, we will remove the last letter which is y
                    singularText = singularText.substring(0, singularText.length - 1);
                    return leadingText + quantity + betweenText + singularText + "ies";
                }
                else
                {
                    // The singular form does not end with y, so let us append an s to it
                    return leadingText + quantity + betweenText + singularText + "s";
                }
            }
        }

        // Submit the Review through the web service
        function SubmitReview() {
            // Fetch the review details
            var booking_id = document.getElementById("txtReview_BookingID").value; 
            var reviewCode = document.getElementById("txtReview_ReviewCode").value;
            var rating = document.getElementById("txtReview_Rating").value;
            var comment = document.getElementById("txtReview_Comment").value;

            // Check whether the rating is outside the allowed bound (0 to 5)
            if (rating < 0 || rating > 5) {
                alert('Please select a valid rating from 0 to 5');
                // Stop every further action since the rating is outside the allowed bound
                return false;
            }
            // Fetch the salon from the hidden field
            var lblSalonUsername = document.getElementById('<%=lblSalonUsername.ClientID%>');
            var salon = $("#" + lblSalonUsername.id).val();
            
            // Call the web service to submit the review
            $.ajax({
                url: "BeautifyWebService/BeautifyWebService.asmx/SubmitReview",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: $.toJSON({
                    bookingID: booking_id, reviewCode: reviewCode, rating: rating,
                    comment: comment, UsernameOfSalonCurrentlyBeingViewedByClient: salon
                }),
                dataType: "json",
                success: function (data) {
                    if (data != null) {
                        jQuery.each(data.d, function (rec) {
                            switch (this.status) {
                                case "Success":
                                    // The review was submitted successfully
                                    // So we show a success message
                                    alert("Your review was submitted successfully");
                                    // Clear the controls in the review section
                                    document.getElementById("txtReview_BookingID").value = "";
                                    document.getElementById("txtReview_ReviewCode").value = "";
                                    document.getElementById("txtReview_Rating").value = "";
                                    document.getElementById("txtReview_Comment").value = "";
                                    // Refresh the page by revisiting it
                                    // We are doing this to refresh the reviews
                                    // First, let us check whether the address field contains a # symbol. We are doing this to know what value to pass to the GoToPage(...) function
                                    if (location.href.indexOf("#") == -1) {
                                        // At this point, it means the address field does not contain a # symbol
                                        GoToPage(location.href);
                                    }
                                    else {
                                        // At this point, it means the address field contains a # symbol
                                        // So we need to get rid of the # symbol and everything to the right of it before calling GoToPage(...)
                                        GoToPage(location.href.substring(0, location.href.indexOf("#")));
                                    }
                                    break;
                                case "Failed":
                                    // An error occurred while submitting the review
                                    // So we should inform the user
                                    alert("An error occurred while submitting your review.\n\nPlease note that you can only review this salon if you have booked it before.\n\nEach booking qualifies you for a single review");
                                    break;
                                case "Existing":
                                    // The user has reviewed this salon using this booking ID
                                    alert("You have already reviewed this salon using this Booking ID.\n\nYou cannot review this salon more than once using the same Booking ID.\n\nPlease use a different Booking ID");
                                    break;
                            }
                            
                        });
                    }

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log("Something really bad happened: " + textStatus);
                    alert(jqXHR.responseText);
                },

                beforeSend: function (jqXHR, settings) {
                    $("#divLoading_Review").show();
                },

                complete: function (jqXHR, textStatus) {
                    $("#divLoading_Review").hide();
                }
            });
        }

        // onload
        window.onload = function () {

            // Keep the user on the active tab on every page load
            KeepUserOnActiveTab();

            // Fetch the salon from the hidden field
            var lblSalonUsername = document.getElementById('<%=lblSalonUsername.ClientID%>');
            var salon = $("#" + lblSalonUsername.id).val();

            // If the hidden field salon is not the same with the session salon, clear the shopping cart
            // This will ensure that the shopping cart only contains services from 1 salon at any time
            if (salon != Session.get("salon")) {
                // Set the shopping cart to empty
                shoppingCart = [];
            }


            // Hide the Add to Cart button for all records in the cart and show the Remove button
            var index;
            for (index = 0; index < shoppingCart.length; index++) {
                $("#removeItem-" + shoppingCart[index].id).show();
                $("#addItem-" + shoppingCart[index].id).hide();
            }

            // Compute the total amount of Cart items
            ComputeCartTotalAmount();

            // store value in session
            Session.set("cart", shoppingCart);

            // Store the salon username in the session
            Session.set("salon", salon);

            // Fetch the salon name
            salonName = document.getElementById('<%=lblSalonName.ClientID%>');
            // Store the salon name in the session            
            Session.set("salonName", '<%=lblSalonName.InnerText%>');

            // Set cookies. This will ensure that session values are stored on mobile device browsers
            setCookie('cart', serialize(shoppingCart));
            setCookie('salon', salon);
            setCookie('salonName', '<%=lblSalonName.InnerText%>');


        };

        function SetActiveTab(tab) {
            Session.set("activeTab", tab);
        }

        function KeepUserOnActiveTab() {
            // The below logic will ensure that when a post back occurs from any of the tabs, 
            // the user will still remain on that tab after page load

            // Set the default active tab
            var activeTab = "tabServices";
            // Check whether an active tab has been set in the session
            if (Session.get("activeTab") != null) {
                // If an active tab has been set, let us fetch it
                activeTab = Session.get("activeTab");
                // Let us switch the user's active tab and set it to be visible
                switch (activeTab) {
                    case "salon-services":
                        $("#" + activeTab).attr("class", "tab-pane active");
                        $("#salon-reviews").attr("class", "tab-pane");
                        $("#salon-about").attr("class", "tab-pane");
                        $("#tabServices").attr("class", "active");
                        $("#tabReviews").attr("class", "");
                        $("#tabAbout").attr("class", "");
                        $("#salon-portfolio").attr("class", "tab-pane");
                        $("#tabPortfolio").attr("class", "");
                        window.open('#tabContainer', '_parent', null, false);
                        break;
                    case "salon-reviews":
                        $("#" + activeTab).attr("class", "tab-pane active");
                        $("#salon-services").attr("class", "tab-pane");
                        $("#salon-about").attr("class", "tab-pane");
                        $("#tabReviews").attr("class", "active");
                        $("#tabServices").attr("class", "");
                        $("#tabAbout").attr("class", "");
                        $("#salon-portfolio").attr("class", "tab-pane");
                        $("#tabPortfolio").attr("class", "");
                        window.open('#tabContainer', '_parent', null, false);
                        break;
                    case "salon-about":
                        $("#" + activeTab).attr("class", "tab-pane active");
                        $("#salon-reviews").attr("class", "tab-pane");
                        $("#salon-services").attr("class", "tab-pane");
                        $("#tabAbout").attr("class", "active");
                        $("#tabReviews").attr("class", "");
                        $("#tabServices").attr("class", "");
                        $("#salon-portfolio").attr("class", "tab-pane");
                        $("#tabPortfolio").attr("class", "");
                        window.open('#tabContainer', '_parent', null, false);
                        break;
                    case "salon-portfolio":
                        $("#" + activeTab).attr("class", "tab-pane");
                        $("#salon-reviews").attr("class", "tab-pane");
                        $("#salon-services").attr("class", "tab-pane");
                        $("#tabAbout").attr("class", "");
                        $("#tabReviews").attr("class", "");
                        $("#tabServices").attr("class", "");
                        $("#salon-portfolio").attr("class", "tab-pane active");
                        $("#tabPortfolio").attr("class", "active");
                        window.open('#tabContainer', '_parent', null, false);
                        break;
                }
            }
        }

        

        // Returns the value of a specified query parameter
        function GetQueryParameterValue(key) {
            var vars = [], hash;
            // Here, we are reading all query values after ?
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0]);
                // Let us check whether there's a # symbol in the value of the current query parameter.
                if (hash[1].indexOf("#") == -1) {
                    // At this point, it means there's no hash symbol in the value of the current query parameter
                    vars[hash[0]] = hash[1];
                }
                else {
                    // At this point, we are reading the value of the query parameter but we are stopping at the # symbol
                    // We are stopping there because the address field contains a # which might have been used to take the user to a particular section of the page
                    vars[hash[0]] = hash[1].substring(0, hash[1].indexOf("#"));
                }
            }
            return vars[key];
        }
    </script>

    <script>

        function setCookie(cname, cvalue) {
            var path = "path=/";
            document.cookie = cname + "=" + cvalue + "; " + path;
        }

        function getCookie(cname) {
            var name = cname + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) == ' ') c = c.substring(1);
                if (c.indexOf(name) == 0) {
                    return c.substring(name.length, c.length);
                }
            }
            return "";
        }

        function checkCookie(cookieName) {
            var cookieValue = getCookie(cookieName);
            // Return the cookie value
            return cookieValue;
        }

</script>
</asp:Content>
