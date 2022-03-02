<%@ Page Title="" Language="C#" MasterPageFile="~/Salons/Salons.Master" AutoEventWireup="true" CodeBehind="ViewBooking.aspx.cs" Inherits="Beautify.Salons.WebForm5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPageContent" runat="server">
    <!-- eCommerce Products Header -->
                        <div class="content-header">
                            <ul class="nav-horizontal text-center">
                                <li>
                                    <a href="Default.aspx"><i class="fa fa-bar-chart"></i> Dashboard</a>
                                </li>
                                <li>
                                    <a href="Profile.aspx"><i class="gi gi-user"></i> Profile</a>
                                </li>
                                <li>
                                    <a href="Portfolio.aspx"><i class="gi gi-briefcase"></i> Portfolio</a>
                                </li>
                                <li>
                                    <a href="Services.aspx"><i class="gi gi-shopping_bag"></i> Services</a>
                                </li>
                                <li class="active">
                                    <a href="Bookings.aspx"><i class="gi gi-shop_window"></i> Bookings</a>
                                </li>
                                <li>
                                    <a href="Earnings.aspx"><i><%=Beautify.AppHelper.GetCurrencySymbol() %></i> Earnings</a>
                                </li>                                   
                            </ul>
                        </div>
                        <!-- END eCommerce Products Header -->

    <div class="row">
        <div class="col-lg-12">
                                <!-- General Data Block -->
                                <div class="block">
                                    <!-- General Data Title -->
                                    <div class="block-title">
                                        <div class="block-options pull-right">
                                    <span runat="server" id="lblBookingStatus"></span>
                                </div>
                                        <h2><i class="fa fa-pencil"></i> <strong runat="server" id="lblBookingID"></strong></h2>
                                    </div>
                                    <!-- END General Data Title -->

                                    <div class="form-horizontal form-borderless" runat="server" id="divServiceDeliveryCode">
                                    <div id="faq1" class="panel-group">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                <h4 class="panel-title"><i class="fa fa-angle-right"></i> <a class="accordion-toggle" data-toggle="collapse" data-parent="#faq1" href="#faq1_q1">I Have Attended this Booking</a></h4>
                                            </div>
                                            <div id="faq1_q1" class="panel-collapse collapse">
                                                <div class="panel-body">
                                                    <div class="alert alert-info">
                                                        <h4><i class="fa fa-exclamation-circle"></i> Service Delivery Code</h4> Enter the service delivery code given to you by the client
                                                    </div>
                                                    <div id="divServiceDeliveryCode_Loading" hidden="hidden" align="center">
                                <i class="fa fa-spinner fa-2x fa-spin"></i>
                            </div>
                                                    <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtServiceDeliveryCode">Service Delivery Code</label>
                                                <div class="col-md-6">
                                                    <asp:TextBox runat="server" MaxLength="50" ID="txtServiceDeliveryCode" CssClass="form-control" placeholder="Service Delivery Code"></asp:TextBox>
                                                    </div>
                                                </div>
                                                    
                                                    <br />
                                                    <div class="form-group text-right">
                                <button type="button" class="btn btn-primary" onclick="SubmitServiceDeliveryCode();" id="btnSubmitServiceDeliveryCode" value="">Submit</button>
                            </div>
                                                </div>
                                            </div>
                                        </div>
                                       
                                    </div>
                                </div>

                                    <div runat="server" id="divMessage">

                                    </div>

                                    
                                    <div class="row">
                                    <div class="col-sm-6 push-bit">
                                        <h4 class="page-header"><strong>Client Details</strong></h4>
                                        <h4><strong runat="server" id="lblClientName"></strong></h4>
                                        <address>
                                            <i class="fa fa-phone"></i> <label runat="server" id="lblPhoneNumber"></label><br>
                                            <i class="fa fa-envelope-o"></i> <label runat="server" id="lblEmail"></label>
                                        </address>
                                    </div>
                                    <div class="col-sm-6 push-bit">
                                        <h4 class="page-header"><strong>Booking Details</strong></h4>
                                        <address>
                                            <i class="fa fa-map-marker"></i> <label runat="server" id="lblClientChoiceLocation"></label><br>
                                            <label runat="server" id="lblCity"></label><br />
                                            <i class="fa fa-calendar"></i> <label runat="server" id="lblClientChoiceDate"></label><br>
                                            <i class="fa fa-clock-o"></i> <label runat="server" id="lblClientChoiceTime"></label><br />
                                            <label runat="server" id="lblOtherNotes"></label>
                                        </address>
                                    </div>
                                </div>

                                    <div class="table-responsive" runat="server" id="divBookedServices">
                                    
                                </div>
                                
                                </div>
                                <!-- END General Data Block -->
        </div>                    
     </div>   

    <script type="text/javascript">
        // Submit the service delivery code through the web service
        function SubmitServiceDeliveryCode() {
            // Fetch the service delivery code
            var serviceDeliveryCode = document.getElementById("<%=txtServiceDeliveryCode.ClientID%>").value;
            var bookingID = document.getElementById("<%=lblBookingID.ClientID%>").textContent;

            // Call the web service to submit the service delivery code
            $.ajax({
                url: "../BeautifyWebService/BeautifyWebService.asmx/SubmitServiceDeliveryCode",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: $.toJSON({
                    bookingID: bookingID, serviceDeliveryCode: serviceDeliveryCode}),
                dataType: "json",
                success: function (data) {
                    if (data != null) {
                        jQuery.each(data.d, function (rec) {
                            // At this point, we know that the user has an outstanding balance.
                                // So let us decide what action to perform based on the result description
                            switch (this.result) {
                                case "SUCCESS":
                                    $("#" + "<%=lblBookingStatus.ClientID%>").html("<label class='label label-success'>ATTENDED</label>");
                                    // Hide the service delivery div
                                    $("#" + "<%=divServiceDeliveryCode.ClientID%>").hide();
                                    alert("Thanks for attending this booking. Your payment will be processed and your account credited");
                                    break;
                                case "FAILED":
                                    alert("You have entered an invalid service delivery code");
                                    break;
                                case "ATTENDED":
                                    alert("You have already attended this booking");
                                        break;
                                case "UNATTENDED":
                                    alert("You failed to attend this booking and hence cannot mark it as attended");
                                        break;
                                case "CANCELLED":
                                    alert("This booking has been cancelled by the client. You cannot mark it as attended");
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
                    $("#divServiceDeliveryCode_Loading").show();
                },

                complete: function (jqXHR, textStatus) {
                    $("#divServiceDeliveryCode_Loading").hide();
                }
            });
        }
    </script>

</asp:Content>
