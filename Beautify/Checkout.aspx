<%@ Page Title="" Language="C#" MasterPageFile="~/FrontEnd.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="Beautify.WebForm5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPageContent" runat="server">
    <link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.min.css">
    <link rel="stylesheet" href="content/frontend/css/star-rating.css" media="all" type="text/css"/>

    <link rel="stylesheet" href="content/zebra_datepicker/css/default.css" type="text/css">
    <link rel="stylesheet" href="content/timepicker/css/jquery.timepicker.css" type="text/css">

    <!-- Media Container -->
            <div class="media-container">
                <!-- Intro -->
            <section class="site-section site-section-light site-section-top themed-background-dark">
                <div class="container text-center">
                    <h1 class="animation-slideDown"><strong id="lblSalonName"></strong></h1>
                    <h2 class="animation-slideUp"><i class="fa fa-shopping-cart"></i> Checkout</h2>
                    <input type="hidden" runat="server" id="hdnSalonName" />
                </div>
            </section>
            <!-- END Intro -->

                <!-- For best results use an image with a resolution of 2560x279 pixels -->
                <img src="content/frontend/img/placeholders/headers/store_home.jpg" alt="" class="media-image animation-pulseSlow">
            </div>
            <!-- END Media Container -->

            <!-- Checkout Process -->
            <section class="site-content site-section">
                <div class="container">
                    <div class="row">
                        <!-- Salon Info Sidebar -->
                        <div class="col-md-4 col-lg-3">
                            <aside class="sidebar site-block">
                                
                                <!-- Salon Info -->
                                <div class="sidebar-block">
                                    <div class="row" id="divSalonInfo">
                                       
                                    </div>
                                </div>
                                <!-- END Salon Info -->
                            </aside>
                        </div>
                        <!-- END Salon Info Sidebar -->

                        <!-- Booking Details -->
                        <div class="col-md-8 col-lg-9" id="tabContainer">
                            <!-- Step Info -->
                            <ul class="nav nav-tabs push-bit" data-toggle="tabs">
                                    <li id="tabBookingDetails" class="active"><a href="#booking-details" onclick="GoToTab('booking-details');"><strong>1. BOOKING DETAILS</strong></a></li>
                                    <li id="tabConfirmOrder"><a href="#" onclick="ValidateBookingDetails('tabConfirmOrder');"><strong>2. CONFIRM ORDER</strong></a></li>
                                </ul>
                                <!-- END Step Info -->

                            <!-- Tab Content -->
                            <div class="tab-content">
                            <!-- First Step -->
                            <div class="tab-pane active" id="booking-details">
                                
                                
                                    <div id="divLoading_BookingDetailsTab" align="center">
                                <i class="fa fa-spinner fa-2x fa-spin"></i>
                            </div>
                                    <div class="form-horizontal form-bordered">
                                        
                                    <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtClientName">Name</label>
                                                <div class="col-md-6">
                                                    <asp:TextBox MaxLength="50" ID="txtClientName" runat="server" CssClass="form-control" placeholder="Your Name"></asp:TextBox>
                                                    <asp:RequiredFieldValidator runat="server" ValidationGroup="Checkout" ErrorMessage="Your name is required" ForeColor="Red" ControlToValidate="txtClientName"></asp:RequiredFieldValidator>
                                                    </div>
                                                </div>
                                            
                                        <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtClientEmail">Email</label>
                                                <div class="col-md-6">
                                                    <asp:TextBox MaxLength="50" type="email" ID="txtClientEmail" runat="server" CssClass="form-control" placeholder="Your Email Address"></asp:TextBox>
                                                       <asp:RequiredFieldValidator runat="server" ValidationGroup="Checkout" ErrorMessage="Your email is required" ForeColor="Red" ControlToValidate="txtClientEmail"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ControlToValidate="txtClientEmail" ValidationExpression="[-0-9a-zA-Z.+_]+@[-0-9a-zA-Z.+_]+\.[a-zA-Z]{2,4}" ErrorMessage="Your email is invalid" ForeColor="Red" ValidationGroup="Checkout"></asp:RegularExpressionValidator>
                                                </div>
                                            </div>
                                    <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtClientPhoneNumber">Phone Number</label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="txtClientPhoneNumber" MaxLength="11" runat="server" CssClass="form-control" placeholder="Your Phone Number"></asp:TextBox>
                                                    <asp:RequiredFieldValidator runat="server" ValidationGroup="Checkout" ErrorMessage="Your phone number is required" ForeColor="Red" ControlToValidate="txtClientPhoneNumber"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                        <div class="form-group">
                                                <label class="col-md-3 control-label" for="cldChoiceDate">Choice Date</label>
                                                <div class="col-md-3">
                                                    <div class="input-group bootstrap-timepicker">
                                            <asp:TextBox ID="cldChoiceDate" MaxLength="10" runat="server" placeholder="Select Date" CssClass="form-control"></asp:TextBox>
                                            <span class="input-group-btn">
                                                            <button type="button" onclick="SelectDate();" class="btn btn-primary"><i class="fa fa-calendar-o"></i></button>
                                                        </span> 
                                                    </div>  
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ValidationGroup="Checkout" ErrorMessage="Please select a date" ForeColor="Red" ControlToValidate="cldChoiceDate"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                        <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtChoiceTime">Choice Time</label>
                                                <div class="col-md-3">
                                                    <div class="input-group bootstrap-timepicker">
                                            <asp:TextBox onclick="SelectTime();" MaxLength="7" CssClass="time form-control" placeholder="Select Time" runat="server" ID="txtChoiceTime"></asp:TextBox> 
                                            <span class="input-group-btn">
                                                            <button type="button" onclick="SelectTime();" class="btn btn-primary"><i class="fa fa-clock-o"></i></button>
                                                        </span> 
                                                    </div>  
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ValidationGroup="Checkout" ErrorMessage="Please select a time" ForeColor="Red" ControlToValidate="txtChoiceTime"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                            
                                        <div class="form-group">
                                                <label class="col-md-3 control-label" for="selChoiceCity">City</label>
                                                <div class="col-md-6">
                                                    <select runat="server" id="ddlChoiceCity" onchange="RecordCitySelection();"></select>
                                                    <input type="hidden" runat="server" id="hdnChoiceCity" />
                                                </div>
                                            </div>
                                        <div class="form-group" hidden="hidden">
                                                <label class="col-md-3 control-label" for="txtChoiceLocation">Address</label>
                                                <div class="col-md-6">
                                                    <textarea maxlength="300" rows="3" runat="server" ID="txtChoiceLocation" class="form-control" placeholder="The address in the selected city where you would like to have your beauty service rendered" readonly="readonly">Address</textarea>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ValidationGroup="Checkout" ErrorMessage="Your address is required" ForeColor="Red" ControlToValidate="txtChoiceLocation"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                        <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtOtherNotes">Other Notes (Optional)</label>
                                                <div class="col-md-6">
                                                    <textarea rows="3" maxlength="300" runat="server" ID="txtOtherNotes" name="checkout-choiceLocation" class="form-control" placeholder="Any other thing you would like to tell the salon. E.g Please reserve some weavons. I would like to purchase 1"></textarea>
                                                </div>
                                            </div>
                                            
                                        
                                        </div>
                                
                                    <br /><br />
                            <!-- Form Buttons -->
                            <div class="form-group text-right">                                
                            <asp:Button runat="server" UseSubmitBehavior="false" CausesValidation="true" CssClass="btn btn-primary btn-lg" ValidationGroup="Checkout" OnClientClick="return ValidateBookingDetails('btnNext_BookingDetails');" Text="Next" />
                            </div>
                            <!-- END Form Buttons -->
                                    
                                </div>
                            
                            <!-- END First Step -->

                            
                            <!-- Fourth Step -->
                            <div id="confirm-order" class="tab-pane">
                                <div id="divLoading" align="center">
                                <i class="fa fa-spinner fa-2x fa-spin"></i>
                            </div>
                                <div class="table-responsive" id="divShoppingCart">
                                    
                                </div>
                                <div class="row">
                                    <div class="col-sm-6 push-bit">
                                        <h4 class="page-header"><strong>Personal Details</strong></h4>
                                        <h4><strong id="lblClientName"></strong></h4>
                                        <address>
                                            <i class="fa fa-phone"></i> <label id="lblPhoneNumber"></label><br>
                                            <i class="fa fa-envelope-o"></i> <label id="lblEmail"></label>
                                        </address>
                                    </div>
                                    <div class="col-sm-6 push-bit">
                                        <h4 class="page-header"><strong>Booking Details</strong></h4>
                                        <address>
                                            <i class="fa fa-map-marker"></i> <label id="lblAddress"></label><br>
                                            <label id="lblCity"></label><br />
                                            <i class="fa fa-calendar"></i> <label id="lblChoiceDate"></label><br>
                                            <i class="fa fa-clock-o"></i> <label id="lblChoiceTime"></label>
                                        </address>
                                    </div>
                                </div>

                                <div class="form-horizontal form-borderless">
                                    <div id="faq1" class="panel-group">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                <h4 class="panel-title"><i class="fa fa-angle-right"></i> <a class="accordion-toggle" data-toggle="collapse" data-parent="#faq1" href="#faq1_q1">I Have Outstanding Balance</a></h4>
                                            </div>
                                            <div id="faq1_q1" class="panel-collapse collapse">
                                                <div class="panel-body">
                                                    <div class="alert alert-info">
                                                        <h4><i class="fa fa-exclamation-circle"></i> Details of Outstanding Balance</h4> You should only use this section if you have outstanding balance from a cancelled or unattended booking. Enter the phone number you used for the booking and the booking ID and unlock code that was sent to you
                                                    </div>
                                                    <div id="divLoading_Outstanding" hidden="hidden" align="center">
                                <i class="fa fa-spinner fa-2x fa-spin"></i>
                            </div>
                                                    <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtOutstandingBalance_PhoneNumber">Phone Number</label>
                                                <div class="col-md-6">
                                                    <asp:TextBox runat="server" MaxLength="11" ID="txtOutstandingBalance_PhoneNumber" CssClass="form-control" placeholder="Phone Number"></asp:TextBox>
                                                    </div>
                                                </div>
                                                    <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtOutstandingBalance_BookingID">Booking ID</label>
                                                <div class="col-md-6">
                                                    <asp:TextBox runat="server" MaxLength="50" ID="txtOutstandingBalance_BookingID" CssClass="form-control" placeholder="Booking ID"></asp:TextBox>
                                                    </div>
                                                </div>
                                                    <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtOutstandingBalance_UnlockCode">Unlock Code</label>
                                                <div class="col-md-6">
                                                    <asp:TextBox runat="server" TextMode="Password" MaxLength="50" ID="txtOutstandingBalance_UnlockCode" CssClass="form-control" placeholder="Unlock Code"></asp:TextBox>
                                                    </div>
                                                </div>
                                                    <br />
                                                    <div class="form-group text-right">
                                <button type="button" class="btn btn-primary" id="btnApplyOutstandingBalance" value="" onclick="ApplyOutstandingBalance(shoppingCart);">Apply Outstanding Balance</button>
                            </div>
                                                </div>
                                            </div>
                                        </div>
                                       
                                    </div>
                                </div>
                                <input type="hidden" runat="server" ID="lblOutstandingPhoneNumber" />
                                <input type="hidden" runat="server" ID="lblOutstandingBookingID" />
                                <input type="hidden" runat="server" ID="lblOutstandingUnlockCode" />
                                <input type="hidden" runat="server" ID="lblShoppingCart" />
                                <br /><br />
                            <!-- Form Buttons -->
                            <div class="form-group text-right">
                                <button type="button" class="btn btn-danger btn-lg" onclick="GoToTab('booking-details');" id="btnPrevious"><i class="fa fa-arrow-left"></i> Previous</button>
                                <asp:Button runat="server" CssClass="btn btn-primary btn-lg" ValidationGroup="Checkout" OnClientClick="ValidateBookingDetails('btnMakePayment');" ID="btnMakePayment" Text="Make Payment" OnClick="btnMakePayment_Click"/>
                            </div>
                            <!-- END Form Buttons -->

                            </div>
                            <!-- END Fourth Step -->
                                </div>
                            <!-- End Tab Content -->
                            
                        </div>
                        <!-- END Booking Details -->
                    </div>
                
                </div>
            </section>
            <!-- END Checkout Process -->

            <!-- Services Info -->
            <!--<section class="site-content site-section site-section-light themed-background-coral">
                <div class="container">
                    <div class="row text-center">
                        <div class="col-sm-5 push visibility-none" data-toggle="animation-appear" data-animation-class="animation-fadeInQuick" data-element-offset="-100">
                            <span class="h3"><i class="fa fa-truck"></i> <strong>FREE SHIPPING</strong><br>24h DELIVERY</span>
                        </div>
                        <div class="col-sm-2 push visibility-none" data-toggle="animation-appear" data-animation-class="animation-fadeInQuickInv" data-element-offset="-100">
                            <i class="fa fa-plus fa-4x"></i>
                        </div>
                        <div class="col-sm-5 push visibility-none" data-toggle="animation-appear" data-animation-class="animation-fadeInQuick" data-element-offset="-100">
                            <span class="h3"><i class="fa fa-support"></i> <strong>FREE SUPPORT</strong><br>24/7</span>
                        </div>
                    </div>
                </div>
            </section>-->
            <!-- END Services Info -->
    
    
    
    <!--Star Rating-->   
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
    <!--<script src="content/frontend/js/star-rating.js" type="text/javascript"></script>-->
    <!-- Toast -->    
    <!-- Session-->
        <script type="text/javascript" src="content/js-session/json-serialization.js"></script>
<script type="text/javascript" src="content/js-session/session.js"></script>
    
    <script type="text/javascript">
        $(document).ready(function () {
            
            // Load the salon info
            LoadSalonInfo();

            // Load the cart
            LoadCart();
            
            
            // Display the salon name
            $("#lblSalonName").html(getCookie("salonName"));
            $("#" + '<%=hdnSalonName.ClientID%>').val(getCookie("salonName"));
        }
        );
        

        // initialize application defaults
        var shoppingCart = [];
        if (Session.get("cart") != null) {
            shoppingCart = Session.get("cart");
        }
        if (shoppingCart.length == 0) {
            if (getCookie("cart") != "" && getCookie("cart") != "[]" && getCookie("cart") != null) {
                // This is a fail safe. It is useful for mobile browsers that cannot pass the cookieless sessions between pages
                shoppingCart = getCookie("cart");
            }
            else {
                // Take the user to the home page if the shopping cart is empty in both the session and cookie
                alert("Cart is Empty");
                GoToPage("Default.aspx");
            }
        }

        
        // Save the default outstanding balance details to the session
        Session.set("outstanding_phoneNumber", "");
        Session.set("outstanding_bookingID", "");
        Session.set("outstanding_unlockCode", "");

        function serialize(object) {
            return JSON.stringify(object);
        }

        function deserialize(string) {
            var object = JSON.parse(string);
            return object;
        }

        function RecordCitySelection() {
            // Record the selected city in the hidden field
            var selCity = document.getElementById('<%=ddlChoiceCity.ClientID%>');
            var hdnCity = document.getElementById('<%=hdnChoiceCity.ClientID%>');
            hdnCity.value = selCity.value;
        }

        // Apply the outanding balance through the web service
        function ApplyOutstandingBalance(shoppingCart) {
            // Fetch the booking details
            var booking_name = document.getElementById("<%=txtClientName.ClientID%>").value;
            var booking_phoneNumber = document.getElementById("<%=txtClientPhoneNumber.ClientID%>").value;
            var booking_email = document.getElementById("<%=txtClientEmail.ClientID%>").value;
            var booking_choiceDate = document.getElementById("<%=cldChoiceDate.ClientID%>").value;
            var booking_choiceTime = document.getElementById("<%=txtChoiceTime.ClientID%>").value;
            var booking_city = document.getElementById("<%=ddlChoiceCity.ClientID%>").value;
            var booking_address = document.getElementById("<%=txtChoiceLocation.ClientID%>").value;
            var booking_otherNotes = document.getElementById("<%=txtOtherNotes.ClientID%>").value
            
            // Fetch the oustanding balance details
            var outstanding_phoneNumber = document.getElementById("<%=txtOutstandingBalance_PhoneNumber.ClientID%>").value;
            var outstanding_bookingID = document.getElementById("<%=txtOutstandingBalance_BookingID.ClientID%>").value;
            var outstanding_unlockCode = document.getElementById("<%=txtOutstandingBalance_UnlockCode.ClientID%>").value;

            // Save the outstanding balance details to the session
            Session.set("outstanding_phoneNumber", outstanding_phoneNumber);
            Session.set("outstanding_bookingID", outstanding_bookingID);
            Session.set("outstanding_unlockCode", outstanding_unlockCode);

            // Save the outstanding balance details to the hidden field.
            // The value in the hidden field will be used by the server Make Payment button which is a server control
            document.getElementById("<%=lblOutstandingPhoneNumber.ClientID%>").value = outstanding_phoneNumber;
            document.getElementById("<%=lblOutstandingBookingID.ClientID%>").value = outstanding_bookingID;
            document.getElementById("<%=lblOutstandingUnlockCode.ClientID%>").value = outstanding_unlockCode;

            // Call the web service to apply the outstanding balance
            $.ajax({
                url: "BeautifyWebService/BeautifyWebService.asmx/ApplyOutstandingBalance",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: $.toJSON({
                    serializedShoppingCartItems: serialize(shoppingCart), outstanding_phoneNumber: outstanding_phoneNumber,
                    outstanding_bookingID: outstanding_bookingID, outstanding_unlockCode: outstanding_unlockCode, booking_name: booking_name,
                    booking_phoneNumber: booking_phoneNumber, booking_email: booking_email, booking_choiceDate: booking_choiceDate, 
                    booking_choiceTime: booking_choiceTime, booking_city: booking_city, booking_address: booking_address, booking_otherNotes: booking_otherNotes
                }),
                dataType: "json",
                success: function (data) {
                    if (data != null) {
                        jQuery.each(data.d, function (rec) {
                            if (this.outstandingBalance != 0) {
                                // At this point, we know that the user has an outstanding balance.
                                // So let us decide what action to perform based on the result description
                                switch (this.resultDescription) {
                                    case "Payment Cleared":
                                        $("#divShoppingCart").html(this.cartTable);
                                        // At this point, we know that the user's outstanding balance has cleared his totalAmountDue
                                        // So we clear the cart and the outstanding balance details and then take the user to the success page
                                        Session.set("cart", []);
                                        // Clear the outstanding balance details
                                        Session.set("outstanding_phoneNumber", "");
                                        Session.set("outstanding_bookingID", "");
                                        Session.set("outstanding_unlockCode", "");
                                        // Take the user to the success page
                                        alert("Your payment has been cleared");
                                        GoToPage("BookingSuccessful.aspx");
                                        break;
                                    case "Needs To Make Additional Payment":
                                        // At this point, we know that the user has outstanding balance but still needs to make additional payment
                                        // So we display the updated cart
                                        $("#divShoppingCart").html(this.cartTable);
                                        // Show a success message
                                        alert('Your outstanding balance has been applied to your cart');
                                        break;
                                }
                                         
                            }
                            else {
                                // At this point we know that the user does not have an outstanding balance
                                // So we do not need to update the cart
                                alert('You do not have an outstanding balance. \nKindly click the Make Payment button to pay with your ATM/Debit Card');
                            }
                        });
                    }

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log("Something really bad happened: " + textStatus);
                    alert(jqXHR.responseText);
                },

                beforeSend: function (jqXHR, settings) {
                    $("#divLoading_Outstanding").show();
                },

                complete: function (jqXHR, textStatus) {
                    $("#divLoading_Outstanding").hide();
                }
            });
        }

        // Load the cart through the web service
        function LoadCart() {
            $.ajax({
                url: "BeautifyWebService/BeautifyWebService.asmx/LoadCartForCheckout",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: $.toJSON({ serializedShoppingCartItems: getCookie("cart") }),
                dataType: "json",
                success: function (data) {
                    if (data != null) {
                        jQuery.each(data.d, function (rec) {
                            $("#divShoppingCart").html(this.cartTable);

                            // Hide the Checkout button if the totalDue=0
                            if (this.totalDue == 0) {
                                $("#btnCheckout").hide();
                            }

                        });
                        }

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log("Something really bad happened: " + textStatus);
                    alert(jqXHR.responseText);
                },

                beforeSend: function (jqXHR, settings) {
                    $("#divLoading").show();
                },

                complete: function (jqXHR, textStatus) {
                    $("#divLoading").hide();
                }
            });
        }

        // Load the salon's info through the web service
        function LoadSalonInfo()
        {
            $.ajax({
                url: "BeautifyWebService/BeautifyWebService.asmx/LoadSalonInfoForCheckout",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: $.toJSON({ salon: Session.get("salon") }),
                dataType: "json",
                success: function (data) {
                    if (data != null) {
                        jQuery.each(data.d, function (rec) {
                            $("#divSalonInfo").html(this.salonInfo);
                            // Note that we are referencing the star-rating.js file here to ensure that the star-rating is displayed after the ajax load
                            $.getScript("content/frontend/js/star-rating.js", function () { });

                            // Disable the days in the calendar that this salon has disabled
                            if (this.disabledDays != "") {
                                // The salon has disabled some days. So we disable those days in the calendar as well
                                $('#' + '<%=cldChoiceDate.ClientID%>').Zebra_DatePicker({
                                    direction: 1,    // Allow only future dates to be selected in the calendar
                                    disabled_dates: ['* * * ' + ConvertCsvWeekdayStringToCsvWeekDayNumber(this.disabledDays)]
                                });
                            }
                            else {
                                // No day has been disabled. So we don't have to disable any weekday in the calendar
                                $('#' + '<%=cldChoiceDate.ClientID%>').Zebra_DatePicker({
                                    direction: 1    // Allow only future dates to be selected in the calendar
                                });
                            }


                            // Set the opening and closing time for the salon
                            $('#' + '<%=txtChoiceTime.ClientID%>').timepicker({
                                'minTime': this.openingTime,
                                'maxTime': this.closingTime,
                                'showDuration': false,
                                'disableTextInput': true
                            });

                            var selCity = document.getElementById('<%=ddlChoiceCity.ClientID%>');
                            // Fetch the locations
                            var locations = this.locations;
                            // Break it down to cities
                            var cities = locations.split(",").sort();
                            // Loop through all cities
                            for (var i = 0; i < cities.length; i++) {
                                // Ensure the current city contains a value adding it to the dropdown
                                if (cities[i] != "") {
                                    // Add each city to the city dropdown
                                    var option = document.createElement("option");
                                    option.text = cities[i];
                                    selCity.add(option);
                                    // Select the first city as default
                                    selCity.selectedIndex = 0;
                                }
                            }

                            // Record the selected city in the hidden field
                            var hdnCity = document.getElementById('<%=hdnChoiceCity.ClientID%>');
                            hdnCity.value = selCity.value;
                            
                        });
                    }

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log("Something really bad happened: " + textStatus);
                    alert(jqXHR.responseText);
                },

                beforeSend: function (jqXHR, settings) {
                    $("#divLoading_BookingDetailsTab").show();
                },

                complete: function (jqXHR, textStatus) {
                    $("#divLoading_BookingDetailsTab").hide();
                }
            });
        }

        function SelectTime() {
            $('#' + '<%=txtChoiceTime.ClientID%>').timepicker('show');

        }

        function SelectDate() {
            $('#' + '<%=cldChoiceDate.ClientID%>').click();
        }

        function ConvertCsvWeekdayStringToCsvWeekDayNumber(csvString)
        {
            // Fetch each weekday name in the csv
            var csvItems = csvString.split(",");

            // This variable will hold a new csv with the weekday numbers
            var csvWeekdayNumber = "";

            // Loop through each csv weekday name and convert it each one to its weekday number equivalent and then write to new csv
            for (i=0;i<csvItems.length;i++)
            {
                // Ensure the weekday nameis not empty
                if (csvItems[i] != "") {
                    var weekdayNumber = GetWeekdayNumber(csvItems[i]);
                    // Append the number value of each weekday to the csvWeekdayNumber
                    csvWeekdayNumber = csvWeekdayNumber + weekdayNumber + ",";
                }
            }
            
            // Return the csv of the weekday number
            return csvWeekdayNumber;
        }

        function GetWeekdayNumber(weekdayName) {
            switch (weekdayName) {
                case "Sunday":
                    return "0";
                    break;
                case "Monday":
                    return "1";
                    break;
                case "Tuesday":
                    return "2";
                    break;
                case "Wednesday":
                    return "3";
                    break;
                case "Thursday":
                    return "4";
                    break;
                case "Friday":
                    return "5";
                    break;
                case "Saturday":
                    return "6";
                    break;                
            }

        }

        function GoToTab(tab) {
            switch(tab)
            {
                case "booking-details":
                    $("#" + tab).attr("class", "tab-pane active");
                    $("#confirm-order").attr("class", "tab-pane");
                    $("#tabBookingDetails").attr("class", "active");
                    $("#tabConfirmOrder").attr("class", "");
                    window.open('#tabContainer', '_parent', null, false);
                    break;
                case "confirm-order":
                    $("#" + tab).attr("class", "tab-pane active");
                    $("#booking-details").attr("class", "tab-pane");
                    $("#tabBookingDetails").attr("class", "");
                    $("#tabConfirmOrder").attr("class", "active");
                    window.open('#tabContainer', '_parent', null, false);
                    break;
            }
        }

        function ValidateBookingDetails(caller) {
            var txtClientName = document.getElementById("<%=txtClientName.ClientID%>");
            var txtClientEmail = document.getElementById("<%=txtClientEmail.ClientID%>");
            var txtClientPhoneNumber = document.getElementById("<%=txtClientPhoneNumber.ClientID%>");
            var cldChoiceDate = document.getElementById("<%=cldChoiceDate.ClientID%>");
            var txtChoiceTime = document.getElementById("<%=txtChoiceTime.ClientID%>");
            var selChoiceCity = document.getElementById("<%=ddlChoiceCity.ClientID%>");
            var txtChoiceLocation = document.getElementById("<%=txtChoiceLocation.ClientID%>");

            // The labels on the Confirm Order tab
            var lblClientName = document.getElementById("lblClientName");
            var lblPhoneNumber = document.getElementById("lblPhoneNumber");
            var lblEmail = document.getElementById("lblEmail");
            var lblAddress = document.getElementById("lblAddress");
            var lblCity = document.getElementById("lblCity");
            var lblChoiceDate = document.getElementById("lblChoiceDate");
            var lblChoiceTime = document.getElementById("lblChoiceTime");
            
            // Check if any required textbox is empty
            if ($("#" + txtClientName.id).val() == "" || $("#" + txtClientEmail.id).val() == "" || $("#" + txtClientPhoneNumber.id).val() == "" || $("#" + cldChoiceDate.id).val() == "" || $("#" + txtChoiceTime.id).val() == "" || $("#" + selChoiceCity.id).val() == "" || $("#" + txtChoiceLocation.id).val() == "") {
                alert("Complete your booking details first");
                GoToTab('booking-details');
                return false;
            }
            else {
                if (ValidateEmail($("#" + txtClientEmail.id).val()) == true) {
                    // The email is valid                    
                    GoToTab('confirm-order');

                    // Write the textbox values to the labels
                    $("#" + lblClientName.id).html($("#" + txtClientName.id).val());
                    $("#" + lblPhoneNumber.id).html($("#" + txtClientPhoneNumber.id).val());
                    $("#" + lblEmail.id).html($("#" + txtClientEmail.id).val());
                    $("#" + lblAddress.id).html($("#" + txtChoiceLocation.id).val());
                    $("#" + lblCity.id).html($("#" + selChoiceCity.id).val());
                    $("#" + lblChoiceDate.id).html($("#" + cldChoiceDate.id).val());
                    $("#" + lblChoiceTime.id).html($("#" + txtChoiceTime.id).val());

                    // Write the serialized shopping cart to the hidden label
                    // This will be used by the Make Payment button which is a server control
                    document.getElementById("<%=lblShoppingCart.ClientID%>").value = getCookie("cart");
                    if (caller == 'btnMakePayment') {
                        // If this function has been called by the Make Payment button, we should empty the cart since we are taking the user to the Payment page
                        //Session.set("cart", []);
                    }
                    return false;
                }
                else {
                    // The email is invalid
                    alert("Invalid Email Format");
                    GoToTab('booking-details');
                    return false;
                }
            }
            
        }

        function ValidateEmail(emailToValidate) {
            var mailformat = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
            if (emailToValidate.match(mailformat)) {
                return true;
            }
            else {
                return false;
            }
        }

        function GoToPage(page) {
            window.open(page, '_parent', null, false);
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
