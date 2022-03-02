<%@ Page Title="" Language="C#" MasterPageFile="~/FrontEnd.Master" AutoEventWireup="true" CodeBehind="BookingSuccessful.aspx.cs" Inherits="Beautify.WebForm6" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPageContent" runat="server">
    <link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.min.css">
    <link rel="stylesheet" href="content/frontend/css/star-rating.css" media="all" type="text/css"/>
    
    
    <!-- Media Container -->
            <div class="media-container">
                <!-- Intro -->
                <section class="site-section site-section-light site-section-top">
                    <div class="container text-center">
                        <h1 class="animation-slideDown"><strong id="lblSalonName"></strong></h1>
                        <h2 class="animation-slideUp"><i class="fa fa-ticket"></i> Booking Successful!</h2>
                    </div>
                </section>
                <!-- END Intro -->

                <!-- For best results use an image with a resolution of 2560x279 pixels -->
                <img src="content/frontend/img/placeholders/headers/store_home.jpg" alt="" class="media-image animation-pulseSlow">
            </div>
            <!-- END Media Container -->

            <!-- Products -->
            <section class="site-content site-section">
                <div class="container">
                    <!-- Seach Form -->
                    
                    <div class="site-block">                        
                        <div class="alert alert-success">
                        <h4><i class="fa fa-check-circle"></i> Booking Successful</h4> Your booking was successful. Details of your booking have been sent to the phone number you supplied. You should only provide the salon with the Service Delivery Code after they have rendered the service. Should you have any questions or complaints, feel free to call our support line on <strong><%=System.Configuration.ConfigurationManager.AppSettings["beautifyPhoneNumber"] %></strong>. Thank you for using Beautify
                        </div>
                    </div>
                    <!-- END Seach Form -->
                    
                       
                   
                    
                </div>
            </section>
            <!-- END Products -->

            <!-- Explore Store Action -->
            <!--<section class="site-content site-section site-section-light themed-background-dark-night">
                <div class="container">
                    <div class="text-center push">
                        <div class="push">
                            <i class="gi gi-shopping_bag fa-5x text-muted"></i>
                        </div>
                        <a href="#" class="btn btn-lg btn-primary">Explore Store <i class="fa fa-arrow-right"></i></a>
                    </div>
                </div>
            </section>-->
            <!-- END Explore Store Action -->

    <!--Star Rating-->   
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
    <script src="content/frontend/js/star-rating.js" type="text/javascript"></script>
    <!-- Session-->
        <script type="text/javascript" src="content/js-session/json-serialization.js"></script>
<script type="text/javascript" src="content/js-session/session.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            // Display the salon name
            $("#lblSalonName").html(Session.get("salonName"));            
        });
    </script>
    
</asp:Content>
