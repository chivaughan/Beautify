<%@ Page Title="" Language="C#" MasterPageFile="~/Salons/Salons.Master" AutoEventWireup="true" CodeBehind="ViewEarning.aspx.cs" Inherits="Beautify.Salons.WebForm9" %>
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
                                <li>
                                    <a href="Bookings.aspx"><i class="gi gi-shop_window"></i> Bookings</a>
                                </li>
                                <li class="active">
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
                                    <span runat="server" id="lblEarningPaymentStatus"></span>
                                </div>
                                        <h2><i class="fa fa-pencil"></i> <strong runat="server" id="lblBookingID"></strong></h2>
                                    </div>
                                    <!-- END General Data Title -->

                                    

                                    
                                    
                                    <div class="row">
                                    <div class="col-sm-6 push-bit">
                                        <h4 class="page-header"><strong>Beneficiary</strong></h4>
                                        <h4><strong runat="server" id="lblAccountName"></strong></h4>
                                        <address>
                                            <label runat="server" id="lblAccountNumber"></label><br>
                                            <i class="fa fa-bank"></i> <label runat="server" id="lblBankName"></label>
                                        </address>
                                    </div>
                                    <div class="col-sm-6 push-bit">
                                        <h4 class="page-header"><strong>Payment Info</strong></h4>
                                        <address>
                                            <span runat="server" id="lblPaymentStatus"></span><br />
                                            <i class="fa fa-calendar"></i> <label runat="server" id="lblDatePaid"></label><br>
                                        </address>
                                    </div>
                                </div>
                                    <h4>Client: <strong><label runat="server" id="lblClientName"></label></strong></h4>
                                                                            
                                    

                                    <div class="table-responsive" runat="server" id="divBookedServices">
                                    
                                </div>
                                
                                </div>
                                <!-- END General Data Block -->
        </div>                    
     </div>   

    
</asp:Content>
