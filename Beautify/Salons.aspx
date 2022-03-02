<%@ Page Title="" Language="C#" MasterPageFile="~/FrontEnd.Master" AutoEventWireup="true" CodeBehind="Salons.aspx.cs" Inherits="Beautify.WebForm1" %>
<%@ Register TagPrefix="CP" TagName="DevasolPager" Src="Paging/PagingUserControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPageContent" runat="server">
    <link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.min.css">
    <link rel="stylesheet" href="content/frontend/css/star-rating.css" media="all" type="text/css"/>
    
    
    <!-- Media Container -->
            <div class="media-container">
                <!-- Intro -->
            <section class="site-section site-section-light site-section-top themed-background-dark">
                <div class="container text-center">
                    <h1 class="animation-slideDown"><strong>Choose a Salon</strong></h1>
                    <h2 class="h3 animation-slideUp" runat="server" id="lblSearchDecription"></h2>
                </div>
            </section>
            <!-- END Intro -->

                <!-- For best results use an image with a resolution of 2560x279 pixels -->
                <img src="content/frontend/img/placeholders/headers/store_home.jpg" alt="" class="media-image animation-pulseSlow">
            </div>
            <!-- END Media Container -->

            <!-- Search Results -->
            <section class="site-content site-section">
                <div class="container">
                    <div class="row">
                       

                        <!-- Products -->
                        <div class="col-md-4 col-lg-12">
                            <div class="form-inline push-bit clearfix">
                                <select id="selPageSize" runat="server" class="form-control pull-left" size="1">
                                    <option value="0" disabled>SHOW</option>
                                    <option value="9" selected="selected">9</option>
                                    <option value="18">18</option>
                                    <option value="27">27</option>
                                    <option value="36">36</option>
                                    <option value="45">45</option>
                                </select>
                                
                                <select id="selOrderBy" runat="server" class="form-control pull-left" size="1">
                                    <option value="0" disabled >SORT BY</option>
                                    <option value="SalonName ASC" selected="selected">Name (A to Z)</option>
                                    <option value="SalonName DESC">Name (Z to A)</option>
                                    <option value="AverageRating ASC">Rating (Lowest to Highest)</option>
                                    <option value="AverageRating DESC">Rating (Highest to Lowest)</option>
                                    <option value="NumberOfBookings ASC">Bookings (Lowest to Highest)</option>
                                    <option value="NumberOfBookings DESC">Bookings (Highest to Lowest)</option>
                                </select>
                                <select id="selLocationInCity" runat="server" class="form-control pull-left" size="1">
                                    <option value="" disabled >LOCATIONS</option>
                                </select>
                                 <asp:LinkButton runat="server" ID="lnkRefreshSalons" CssClass="btn btn-primary pull-right" ToolTip="Refresh" OnClick="lnkRefreshSalons_Click"><i class="fa fa-refresh fa-2x"></i></asp:LinkButton>
                            </div>
                            
                           
                            <div class="row store-items" id="divSalons" runat="server">
                       </div>
                        <CP:DevasolPager ID="uclPagerSalons" runat="server" />
 <asp:HiddenField ID="hdnCurrentIndexSalons" runat="server" Value="Blank Value" />
                    
                            
                        </div>
                        <!-- END Products -->
                    </div>
                </div>
            </section>
            <!-- END Search Results -->

    <!--Star Rating-->   
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
    <script src="content/frontend/js/star-rating.js" type="text/javascript"></script>

</asp:Content>
