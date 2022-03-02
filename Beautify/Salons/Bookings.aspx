<%@ Page Title="" Language="C#" MasterPageFile="~/Salons/Salons.Master" AutoEventWireup="true" CodeBehind="Bookings.aspx.cs" Inherits="Beautify.Salons.WebForm4" %>
<%@ Register TagPrefix="CP" TagName="DevasolPager" Src="~/Paging/PagingUserControl.ascx" %>
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

                        <!-- Quick Stats -->
                        <div class="row text-center">
                            <div class="col-sm-6 col-lg-3">
                                <asp:LinkButton runat="server" ID="lnkAttendedBookings" class="widget widget-hover-effect2" OnClick="lnkAttendedBookings_Click">
                                    <div class="widget-extra themed-background-success">
                                        <h4 class="widget-content-light"><strong>Attended</strong> Bookings</h4>
                                    </div>
                                    <div class="widget-extra-full"><span class="h2 text-success animation-expandOpen" runat="server" id="lblAttendedBookingsCount"></span></div>
                                </asp:LinkButton>
                            </div>
                            <div class="col-sm-6 col-lg-3">
                                <asp:LinkButton runat="server" ID="lnkCancelledBookings" class="widget widget-hover-effect2" OnClick="lnkCancelledBookings_Click" >
                                    <div class="widget-extra themed-background-danger">
                                        <h4 class="widget-content-light"><strong>Cancelled</strong> Bookings</h4>
                                    </div>
                                    <div class="widget-extra-full"><span class="h2 text-danger animation-expandOpen" runat="server" id="lblCancelledBookingsCount"></span></div>
                                </asp:LinkButton>
                            </div>
                            <div class="col-sm-6 col-lg-3">
                                <asp:LinkButton runat="server" ID="lnkPendingBookings" class="widget widget-hover-effect2" OnClick="lnkPendingBookings_Click" >
                                    <div class="widget-extra themed-background-default">
                                        <h4 class="widget-content-light"><strong>Pending</strong> Bookings</h4>
                                    </div>
                                    <div class="widget-extra-full"><span class="h2 themed-color-dark animation-expandOpen" runat="server" id="lblPendingBookingsCount"></span></div>
                                </asp:LinkButton>
                            </div>
                            <div class="col-sm-6 col-lg-3">
                                <asp:LinkButton runat="server" ID="lnkUnattendedBookings" class="widget widget-hover-effect2" OnClick="lnkUnattendedBookings_Click" >
                                    <div class="widget-extra themed-background-warning">
                                        <h4 class="widget-content-light"><strong>Unattended</strong> Bookings</h4>
                                    </div>
                                    <div class="widget-extra-full"><span class="h2 themed-color-dark animation-expandOpen" runat="server" id="lblUnattendedBookingsCount"></span></div>
                                </asp:LinkButton>
                            </div>
                            
                        </div>
                        <!-- END Quick Stats -->

                            <!--All Bookins -->
                            <div align="center">
                                <asp:LinkButton runat="server" ID="lnkAllBookings" class="widget widget-hover-effect2" OnClick="lnkAllBookings_Click" >
                                    <div class="widget-extra themed-background-dark">
                                        <h4 class="widget-content-light"><strong>All</strong> Bookings</h4>
                                    </div>
                                    <div class="widget-extra-full"><span class="h2 themed-color-dark animation-expandOpen" runat="server" id="lblAllBookingsCount"></span></div>
                                </asp:LinkButton>
                            </div>
                            <!-- End All Bookins Stat -->

                        <!-- All Services Block -->
                        <div class="block full">
                            <!-- All Services Title -->
                            <div class="block-title">
                                
                                <h2 runat="server" id="lblSearchDescription"><strong>All</strong> Bookings</h2>
                            </div>
                            <!-- END All Services Title -->

                            <!-- FILTER -->
                            <div class="form-inline push-bit clearfix">
                                <select id="selBookingStatus" runat="server" class="form-control pull-left" size="1">
                                    <option value="" disabled>STATUS</option>
                                    <option value="All" selected="selected">All</option>
                                    <option value="ATTENDED">ATTENDED</option>
                                    <option value="CANCELLED">CANCELLED</option>
                                    <option value="PENDING">PENDING</option>
                                    <option value="UNATTENDED">UNATTENDED</option>
                                </select>
                                <select id="selBookingsPageSize" runat="server" class="form-control pull-left" size="1">
                                    <option value="0" disabled>SHOW</option>
                                    <option value="20" selected="selected">20</option>
                                    <option value="40">40</option>
                                    <option value="60">60</option>
                                    <option value="80">80</option>
                                    <option value="100">100</option>
                                </select>
                                
                                <asp:LinkButton runat="server" ID="lnkRefreshBookings" CssClass="btn btn-primary pull-right" ToolTip="Refresh" OnClick="lnkRefreshBookings_Click"><i class="fa fa-refresh fa-2x"></i></asp:LinkButton>
                                
                            </div>
                            <!-- END FILTER -->
                           

                            <!-- All Services Content -->
                            <div runat="server" id="divServices">
                            
                                </div>
                            <!-- END All Services Content -->
                            <CP:DevasolPager ID="uclPagerBookings" runat="server" />
                            <asp:HiddenField ID="hdnCurrentIndexBookings" runat="server" Value="Blank Value" />
                            
                                
                        </div>
                        <!-- END All Services Block -->

    
</asp:Content>
