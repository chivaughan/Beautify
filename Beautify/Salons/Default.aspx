<%@ Page Title="" Language="C#" MasterPageFile="~/Salons/Salons.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Beautify.Salons.WebForm10" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPageContent" runat="server">
    <!-- eCommerce Products Header -->
                        <div class="content-header">
                            <ul class="nav-horizontal text-center">
                                <li class="active">
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
                                <li>
                                    <a href="Earnings.aspx"><i><%=Beautify.AppHelper.GetCurrencySymbol() %></i> Earnings</a>
                                </li>                                   
                            </ul>
                        </div>
                        <!-- END eCommerce Products Header -->

    
                        <!-- Quick Stats -->
                        <div class="row text-center">
                            <div class="col-sm-6 col-lg-6">
                                <a href="Bookings.aspx" class="widget widget-hover-effect2">
                                    <div class="widget-extra themed-background">
                                        <h4 class="widget-content-light"><strong>Pending</strong> Bookings</h4>
                                    </div>
                                    <div class="widget-extra-full">
                                        <span class="h2 text-danger animation-expandOpen" runat="server" id="lblPendingBookingsCount"></span>
                                    </div>
                                </a>
                            </div>
                            <div class="col-sm-6 col-lg-6">
                                <a href="Bookings.aspx" class="widget widget-hover-effect2">
                                    <div class="widget-extra themed-background-dark">
                                        <h4 class="widget-content-light"><strong>Attended</strong> Bookings</h4>
                                    </div>
                                    <div class="widget-extra-full">
                                        <span class="h2 text-danger animation-expandOpen" runat="server" id="lblAttendedBookingsCount"></span>
                                    </div>
                                </a>
                            </div>
                            <div class="col-sm-6 col-lg-6">
                                <a href="Earnings.aspx" class="widget widget-hover-effect2">
                                    <div class="widget-extra themed-background-success">
                                        <h4 class="widget-content-light"><strong>Paid</strong> Earnings</h4>
                                    </div>
                                    <div class="widget-extra-full">
                                        <span class="h2 text-danger animation-expandOpen" runat="server" id="lblPaidEarningsCount"></span>
                                        <strong><span class="h4 store-item-price themed-color-dark pull-right" runat="server" id="lblTotalValueOfPaidEarnings"></span></strong>
                                    </div>
                                </a>
                            </div>
                            <div class="col-sm-6 col-lg-6">
                                <a href="Earnings.aspx" class="widget widget-hover-effect2">
                                    <div class="widget-extra themed-background-danger">
                                        <h4 class="widget-content-light"><strong>Unpaid</strong> Earnings</h4>
                                    </div>
                                    <div class="widget-extra-full">
                                        <span class="h2 text-danger animation-expandOpen" runat="server" id="lblUnpaidEarningsCount"></span>
                                        <strong><span class="h4 store-item-price themed-color-dark pull-right" runat="server" id="lblTotalValueOfUnpaidEarnings"></span></strong>
                                    </div>
                                </a>
                            </div>
                        </div>
                        <!-- END Quick Stats -->
       
    
</asp:Content>
