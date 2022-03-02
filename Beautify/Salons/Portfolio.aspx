<%@ Page Title="" Language="C#" MasterPageFile="~/Salons/Salons.Master" AutoEventWireup="true" CodeBehind="Portfolio.aspx.cs" Inherits="Beautify.Salons.WebForm7" %>
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
                                <li class="active">
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
                                <a href="EditService.aspx" class="widget widget-hover-effect2">
                                    <div class="widget-extra themed-background-success">
                                        <h4 class="widget-content-light"><strong>Add New</strong> Item</h4>
                                    </div>
                                    <div class="widget-extra-full"><span class="h2 text-success animation-expandOpen"><i class="fa fa-plus"></i></span></div>
                                </a>
                            </div>
                            
                            <div class="col-sm-6 col-lg-6">
                                <asp:LinkButton runat="server" ID="lnkAllItems" class="widget widget-hover-effect2" OnClick="lnkAllPortfolio_Click">
                                    <div class="widget-extra themed-background-dark">
                                        <h4 class="widget-content-light"><strong>All</strong> Items</h4>
                                    </div>
                                    <div class="widget-extra-full"><span class="h2 themed-color-dark animation-expandOpen" runat="server" id="lblAllPortfolioCount"></span></div>
                                </asp:LinkButton>
                            </div>
                        </div>
                        <!-- END Quick Stats -->

                        <!-- All Services Block -->
                        <div class="block full">
                            <!-- All Services Title -->
                            <div class="block-title">
                                
                                <h2 runat="server" id="lblSearchDescription"><strong>All</strong> Portfolios</h2>
                            </div>
                            <!-- END All Services Title -->

                            <!-- FILTER -->
                            <div class="form-inline push-bit clearfix">
                                <select runat="server" id="selServiceCategory" name="results-show" class="form-control pull-left" size="1">
                                    
                                </select>
                                <select id="selServicePageSize" runat="server" class="form-control pull-left" size="1">
                                    <option value="0" disabled>SHOW</option>
                                    <option value="20" selected="selected">20</option>
                                    <option value="40">40</option>
                                    <option value="60">60</option>
                                    <option value="80">80</option>
                                    <option value="100">100</option>
                                </select>
                                
                                <asp:LinkButton runat="server" ID="lnkRefreshPortfolio" CssClass="btn btn-primary pull-right" ToolTip="Refresh" OnClick="lnkRefreshPortfolio_Click"><i class="fa fa-refresh fa-2x"></i></asp:LinkButton>
                                
                            </div>
                            <!-- END FILTER -->
                           

                            <!-- All Portfolio Content -->
                            <div runat="server" id="divPortfolio">
                            
                                </div>
                            <!-- END All Portfolio Content -->
                            <CP:DevasolPager ID="uclPagerPortfolio" runat="server" />
                            <asp:HiddenField ID="hdnCurrentIndexPortfolio" runat="server" Value="Blank Value" />
                            
                                
                        </div>
                        <!-- END All Services Block -->

    
</asp:Content>
