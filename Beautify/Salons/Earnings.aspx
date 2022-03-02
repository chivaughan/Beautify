<%@ Page Title="" Language="C#" MasterPageFile="~/Salons/Salons.Master" AutoEventWireup="true" CodeBehind="Earnings.aspx.cs" Inherits="Beautify.Salons.WebForm6" %>
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
                                <li>
                                    <a href="Bookings.aspx"><i class="gi gi-shop_window"></i> Bookings</a>
                                </li>
                                <li class="active">
                                    <a href="Earnings.aspx"><i><%=Beautify.AppHelper.GetCurrencySymbol() %></i> Earnings</a>
                                </li>
                                
                            </ul>
                        </div>
                        <!-- END eCommerce Products Header -->

                        <!-- Quick Stats -->
                        <div class="row text-center">
                            <div class="col-sm-6 col-lg-4">
                                <asp:LinkButton runat="server" ID="lnkPaidEarnings" CssClass="widget widget-hover-effect2" OnClick="lnkPaidEarnings_Click">
                                    <div class="widget-extra themed-background-success">
                                        <h4 class="widget-content-light"><strong>Paid</strong> Earnings</h4>
                                    </div>
                                    <div class="widget-extra-full">
                                        <span class="h2 text-success animation-expandOpen" runat="server" id="lblPaidEarningsCount"></span>
                                        <strong><span class="h4 store-item-price themed-color-dark pull-right" runat="server" id="lblTotalValueOfPaidEarnings"></span></strong>
                                    </div>
                                </asp:LinkButton>
                            </div>
                            <div class="col-sm-6 col-lg-4">
                                <asp:LinkButton runat="server" ID="lnkUnpaidEarnings" CssClass="widget widget-hover-effect2" OnClick="lnkUnpaidEarnings_Click" >
                                    <div class="widget-extra themed-background-danger">
                                        <h4 class="widget-content-light"><strong>Unpaid</strong> Earnings</h4>
                                    </div>
                                    <div class="widget-extra-full">
                                        <span class="h2 text-danger animation-expandOpen" runat="server" id="lblUnpaidEarningsCount"></span>
                                        <strong><span class="h4 store-item-price themed-color-dark pull-right" runat="server" id="lblTotalValueOfUnpaidEarnings"></span></strong>
                                    </div>
                                </asp:LinkButton>
                            </div>
                            <div class="col-sm-6 col-lg-4">
                                <asp:LinkButton runat="server" ID="lnkAllEarnings" CssClass="widget widget-hover-effect2" OnClick="lnkAllEarnings_Click" >
                                    <div class="widget-extra themed-background-default">
                                        <h4 class="widget-content-light"><strong>All</strong> Earnings</h4>
                                    </div>
                                    <div class="widget-extra-full">
                                        <span class="h2 themed-color-dark animation-expandOpen" runat="server" id="lblAllEarningsCount"></span>
                                        <strong><span class="h4 store-item-price themed-color-dark pull-right" runat="server" id="lblTotalValueOfAllEarnings"></span></strong>
                                    </div>
                                </asp:LinkButton>
                            </div>
                            
                            
                        </div>
                        <!-- END Quick Stats -->

                            

                        <!-- All Services Block -->
                        <div class="block full">
                            <!-- All Services Title -->
                            <div class="block-title">
                                
                                <h2 runat="server" id="lblSearchDescription"><strong>All</strong> Earnings</h2>
                            </div>
                            <!-- END All Services Title -->

                            <!-- FILTER -->
                            <div class="form-inline push-bit clearfix">
                                <select id="selEarningStatus" runat="server" class="form-control pull-left" size="1">
                                    <option value="" disabled>STATUS</option>
                                    <option value="All" selected="selected">All</option>
                                    <option value="PAID">PAID</option>
                                    <option value="UNPAID">UNPAID</option>
                                </select>
                                <select id="selEarningsPageSize" runat="server" class="form-control pull-left" size="1">
                                    <option value="0" disabled>SHOW</option>
                                    <option value="20" selected="selected">20</option>
                                    <option value="40">40</option>
                                    <option value="60">60</option>
                                    <option value="80">80</option>
                                    <option value="100">100</option>
                                </select>
                                
                                <asp:LinkButton runat="server" ID="lnkRefreshEarnings" CssClass="btn btn-primary pull-right" ToolTip="Refresh" OnClick="lnkRefreshEarnings_Click"><i class="fa fa-refresh fa-2x"></i></asp:LinkButton>
                                
                            </div>
                            <!-- END FILTER -->
                           

                            <!-- All Earnings Content -->
                            <div runat="server" id="divEarnings">
                            
                                </div>
                            <!-- END All Earnings Content -->
                            <CP:DevasolPager ID="uclPagerEarnings" runat="server" />
                            <asp:HiddenField ID="hdnCurrentIndexEarnings" runat="server" Value="Blank Value" />
                            
                                
                        </div>
                        <!-- END All Services Block -->

    
</asp:Content>
