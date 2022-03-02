<%@ Page Title="" Language="C#" MasterPageFile="~/FrontEnd.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Beautify.WebForm2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPageContent" runat="server">
    <link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.min.css">
    <link rel="stylesheet" href="content/frontend/css/star-rating.css" media="all" type="text/css"/>
    
    
    <!-- Media Container -->
            <div class="media-container">
                <!-- Intro -->
                <section class="site-section site-section-light site-section-top">
                    <div class="container text-center">
                        <h1 class="animation-slideDown"><strong>Online Salon Service for Nigerian Cities!</strong></h1>
                        <h2 class="h3 animation-slideUp hidden-xs" runat="server" id="lblSalonCount"></h2>
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
                        <section id="sectionCities" runat="server">
                        
                    </section>
                            <br /><br />
                        <p align="center"><button type="button" class="btn btn-lg btn-primary" onclick="ValidateCitySelection();">Choose a Salon <i class="fa fa-arrow-right"></i></button> </p>
                        
                    </div>
                    <!-- END Seach Form -->
                    
                       
                   
                    <!-- New Arrivals -->
                    <!--<h2 class="site-heading"><strong>Top Rated</strong> Salons</h2>
                    <hr>
                    <div class="row store-items">
                        <div class="col-md-4 visibility-none" data-toggle="animation-appear" data-animation-class="animation-fadeInQuick" data-element-offset="-100">
                            <div class="store-item">
                                
                                <div class="store-item-image">
                                    <a href="ecom_product.html">
                                        <img src="content/frontend/img/placeholders/photos/photo26.jpg" alt="" class="img-responsive">
                                    </a>
                                </div>
                                <input id="input-2c" class="rating" min="0" max="5" step="0.1" data-size="xs"
           data-symbol="&#xf005;" data-glyphicon="false" data-rating-class="rating-fa" value="5" disabled="disabled"> 
           <label>11 Votes</label>
                                <div class="store-item-info clearfix">
                                    <span class="store-item-price themed-color-dark pull-right" style="font-size:small;">80 Bookings</span>
                                    <a href="ecom_product.html"><strong style="font-size:large;">The House of Tara</strong></a><br>
                                    <small><i class="fa fa-map-marker fa-2x text-muted"></i> <span style="font-size:small">Lagos - NG, Abuja - NG, Ilorin - NG, Owerri - NG</span></small>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 visibility-none" data-toggle="animation-appear" data-animation-class="animation-fadeInQuick" data-element-offset="-100">
                            <div class="store-item">
                                
                                <div class="store-item-image">
                                    <a href="ecom_product.html">
                                        <img src="content/frontend/img/placeholders/photos/photo29.jpg" alt="" class="img-responsive">
                                    </a>
                                </div>
                                <input id="Text1" class="rating" min="0" max="5" step="0.5" data-size="xs"
           data-symbol="&#xf005;" data-glyphicon="false" data-rating-class="rating-fa" value="3.1" disabled="disabled">
                                <label>11 Votes</label>
                                <div class="store-item-info clearfix">
                                    <span class="store-item-price themed-color-dark pull-right" style="font-size:small;">99 Bookings</span>
                                    <a href="ecom_product.html"><strong style="font-size:large;">Mud Salon</strong></a><br>
                                    <small><i class="fa fa-map-marker fa-2x text-muted"></i> <span style="font-size:small">Lagos - NG, Abuja - NG, Ilorin - NG, Owerri - NG</span></small>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 visibility-none" data-toggle="animation-appear" data-animation-class="animation-fadeInQuick" data-element-offset="-100">
                            <div class="store-item">
                                
                                <div class="store-item-image">
                                    <a href="ecom_product.html">
                                        <img src="content/frontend/img/placeholders/photos/photo27.jpg" alt="" class="img-responsive">
                                    </a>
                                </div>
                                <input id="Text2" class="rating" min="0" max="5" step="0.5" data-size="xs"
           data-symbol="&#xf005;" data-glyphicon="false" data-rating-class="rating-fa" value="3.1" disabled="disabled">
                                <label>11 Votes</label>
                                <div class="store-item-info clearfix">
                                    <span class="store-item-price themed-color-dark pull-right" style="font-size:small;">299 Bookings</span>
                                    <a href="ecom_product.html"><strong style="font-size:large;">Bella Makeover Palace</strong></a><br>
                                    <small><i class="fa fa-map-marker fa-2x text-muted"></i> <span style="font-size:small">Lagos - NG, Abuja - NG, Ilorin - NG, Owerri - NG</span></small>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 text-right">
                            <a href="ecom_product_list.html"><strong>View All</strong> <i class="fa fa-arrow-right"></i></a>
                        </div>
                    </div>-->
                    <!-- END New Arrivals -->

                    <!-- Best Sellers -->
                    <!--<h2 class="site-heading"><strong>Best</strong> Sellers</h2>
                    <hr>
                    <div class="row store-items">
                        <div class="col-md-4 visibility-none" data-toggle="animation-appear" data-animation-class="animation-fadeInQuick" data-element-offset="-100">
                            <div class="store-item">
                                
                                <div class="store-item-image">
                                    <a href="ecom_product.html">
                                        <img src="content/frontend/img/placeholders/photos/photo25.jpg" alt="" class="img-responsive">
                                    </a>
                                </div>
                                <input id="Text3" class="rating" min="0" max="5" step="0.5" data-size="xs"
           data-symbol="&#xf005;" data-glyphicon="false" data-rating-class="rating-fa" value="3.1" disabled="disabled">
                                <label>11 Votes</label>
                                <div class="store-item-info clearfix">
                                    <span class="store-item-price themed-color-dark pull-right" style="font-size:small;">832 Bookings</span>
                                    <a href="ecom_product.html"><strong style="font-size:large;">Sunglasses</strong></a><br>
                                    <small><i class="fa fa-map-marker fa-2x text-muted"></i> <span style="font-size:small">Lagos - NG, Abuja - NG, Ilorin - NG, Owerri - NG</span></small>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 visibility-none" data-toggle="animation-appear" data-animation-class="animation-fadeInQuick" data-element-offset="-100">
                            <div class="store-item">
                                
                                <div class="store-item-image">
                                    <a href="ecom_product.html">
                                        <img src="content/frontend/img/placeholders/photos/photo28.jpg" alt="" class="img-responsive">
                                    </a>
                                </div>
                                <input id="Text4" class="rating" min="0" max="5" step="0.5" data-size="xs"
           data-symbol="&#xf005;" data-glyphicon="false" data-rating-class="rating-fa" value="3.1" disabled="disabled">
                                <label>11 Votes</label>
                                <div class="store-item-info clearfix">
                                    <span class="store-item-price themed-color-dark pull-right" style="font-size:small;">592 Bookings</span>
                                    <a href="ecom_product.html"><strong style="font-size:large;">Gloves</strong></a><br>
                                    <small><i class="fa fa-map-marker fa-2x text-muted"></i> <span style="font-size:small">Lagos - NG, Abuja - NG, Ilorin - NG, Owerri - NG</span></small>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 visibility-none" data-toggle="animation-appear" data-animation-class="animation-fadeInQuick" data-element-offset="-100">
                            <div class="store-item">
                                
                                <div class="store-item-image">
                                    <a href="ecom_product.html">
                                        <img src="content/frontend/img/placeholders/photos/photo30.jpg" alt="" class="img-responsive">
                                    </a>
                                </div>
                                <input id="Text5" class="rating" min="0" max="5" step="0.5" data-size="xs"
           data-symbol="&#xf005;" data-glyphicon="false" data-rating-class="rating-fa" value="3.1" disabled="disabled">
                                <label>11 Votes</label>
                                <div class="store-item-info clearfix">
                                    <span class="store-item-price themed-color-dark pull-right" style="font-size:small;">523 Bookings</span>
                                    <a href="ecom_product.html"><strong style="font-size:large;">Jacket</strong></a><br>
                                    <small><i class="fa fa-map-marker fa-2x text-muted"></i> <span style="font-size:small">Lagos - NG, Abuja - NG, Ilorin - NG, Owerri - NG</span></small>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 visibility-none" data-toggle="animation-appear" data-animation-class="animation-fadeInQuick" data-element-offset="-100">
                            <div class="store-item">
                                
                                <div class="store-item-image">
                                    <a href="ecom_product.html">
                                        <img src="content/frontend/img/placeholders/photos/photo32.jpg" alt="" class="img-responsive">
                                    </a>
                                </div>
                                <input id="Text8" class="rating" min="0" max="5" step="0.5" data-size="xs"
           data-symbol="&#xf005;" data-glyphicon="false" data-rating-class="rating-fa" value="3.1" disabled="disabled">
                                <label>11 Votes</label>
                                <div class="store-item-info clearfix">
                                    <span class="store-item-price themed-color-dark pull-right" style="font-size:small;">790 Bookings</span>
                                    <a href="ecom_product.html"><strong style="font-size:large;">Headset</strong></a><br>
                                    <small><i class="fa fa-map-marker fa-2x text-muted"></i> <span style="font-size:small">Lagos - NG, Abuja - NG, Ilorin - NG, Owerri - NG</span></small>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 visibility-none" data-toggle="animation-appear" data-animation-class="animation-fadeInQuick" data-element-offset="-100">
                            <div class="store-item">
                                
                                <div class="store-item-image">
                                    <a href="ecom_product.html">
                                        <img src="content/frontend/img/placeholders/photos/photo35.jpg" alt="" class="img-responsive">
                                    </a>
                                </div>
                                <input id="Text6" class="rating" min="0" max="5" step="0.5" data-size="xs"
           data-symbol="&#xf005;" data-glyphicon="false" data-rating-class="rating-fa" value="3.1" disabled="disabled">
                                <label>11 Votes</label>
                                <div class="store-item-info clearfix">
                                    <span class="store-item-price themed-color-dark pull-right" style="font-size:small;">1,000 Bookings</span>
                                    <a href="ecom_product.html"><strong style="font-size:large;">Laptop</strong></a><br>
                                    <small><i class="fa fa-map-marker fa-2x text-muted"></i> <span style="font-size:small">Lagos - NG, Abuja - NG, Ilorin - NG, Owerri - NG</span></small>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 visibility-none" data-toggle="animation-appear" data-animation-class="animation-fadeInQuick" data-element-offset="-100">
                            <div class="store-item">
                                
                                <div class="store-item-image">
                                    <a href="ecom_product.html">
                                        <img src="content/frontend/img/placeholders/photos/photo33.jpg" alt="" class="img-responsive">
                                    </a>
                                </div>
                                <input id="Text7" class="rating" min="0" max="5" step="0.5" data-size="xs"
           data-symbol="&#xf005;" data-glyphicon="false" data-rating-class="rating-fa" value="3.1" disabled="disabled">
                                <label>11 Votes</label>
                                <div class="store-item-info clearfix">
                                    <span class="store-item-price themed-color-dark pull-right" style="font-size:small;">852 Bookings</span>
                                    <a href="ecom_product.html"><strong style="font-size:large;">Sunglasses</strong></a><br>
                                    <small><i class="fa fa-map-marker fa-2x text-muted"></i> <span style="font-size:small">Lagos - NG, Abuja - NG, Ilorin - NG, Owerri - NG</span></small>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 text-right">
                            <a href="ecom_product_list.html"><strong>View All</strong> <i class="fa fa-arrow-right"></i></a>
                        </div>
                    </div>-->
                    <!-- END Best Sellers -->
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

    <script type="text/javascript">
        function ValidateCitySelection() {
            var selCity = document.getElementById("selCity");
            // Check whether the city selection is still empty
            if (selCity.value == "") {
                alert('Please select a city');
                return false;
            }
            else {
                // Show the salons in the selected city
                ShowSalons(selCity.value);
            }

        }

        function ShowSalons(city) {
            // Take the user to the Salons page.
            // Append the selected city to the URL
            window.open('Salons.aspx?city=' + city, '_parent', null, false);
        }
    </script>

    <!--Star Rating-->   
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
    <script src="content/frontend/js/star-rating.js" type="text/javascript"></script>
    
</asp:Content>
