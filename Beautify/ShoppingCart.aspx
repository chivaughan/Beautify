<%@ Page Title="" Language="C#" MasterPageFile="~/FrontEnd.Master" AutoEventWireup="true" CodeBehind="ShoppingCart.aspx.cs" Inherits="Beautify.WebForm4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPageContent" runat="server">
    <link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.min.css">
    <link rel="stylesheet" href="content/frontend/css/star-rating.css" media="all" type="text/css"/>
       
    <!-- Media Container -->
            <div class="media-container">
                <!-- Intro -->
            <section class="site-section site-section-light site-section-top themed-background-dark">
                <div class="container text-center">
                    <h1 class="animation-slideDown"><strong id="lblSalonName"></strong></h1>
                    <h2 class="animation-slideUp"><i class="fa fa-shopping-cart"></i> Shopping Cart</h2>
                </div>
            </section>
            <!-- END Intro -->

                <!-- For best results use an image with a resolution of 2560x279 pixels -->
                <img src="content/frontend/img/placeholders/headers/store_home.jpg" alt="" class="media-image animation-pulseSlow">
            </div>
            <!-- END Media Container -->

            <!-- Shopping Cart -->
            <section class="site-content site-section">
                <div class="container">
                    <div id="divLoading" align="center">
                                <i class="fa fa-spinner fa-2x fa-spin"></i>
                            </div>
                    <div class="table-responsive" id="divShoppingCart">
                        
                    </div>
                    <div class="row">
                        <div class="col-xs-7 col-md-3" id="divContinueShopping">
                            
                        </div>
                        <div class="col-xs-5 col-md-3 col-md-offset-6">
                            <button type="button" id="btnCheckout" class="btn btn-block btn-danger" onclick="GoToPage('Checkout.aspx');">Checkout <i class="fa fa-arrow-right"></i></button>
                        </div>
                    </div>
                    <br /><br /><br />
                </div>
            </section>
            <!-- END Shopping Cart -->
    
    
    <!--Star Rating-->   
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
    <script src="content/frontend/js/star-rating.js" type="text/javascript"></script>

    <!-- Toast -->    
    <!-- Session-->
        <script type="text/javascript" src="content/js-session/json-serialization.js"></script>
<script type="text/javascript" src="content/js-session/session.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            // Fetch this document's referrer
            var referrer = ParseURL(document.referrer);
            if (referrer != null) {
                if (referrer == "/ChooseServices.aspx") {
                    // The user is coming from ChooseServices.aspx
                    $("#divContinueShopping").html("<button type='button' class='btn btn-block btn-primary' onclick='GoBack();'><i class='fa fa-arrow-left'></i> Continue Shopping</button>")
                }
                else {
                    // The user is coming from another url
                    $("#divContinueShopping").html("<a href='Default.aspx' class='btn btn-block btn-primary'><i class='fa fa-arrow-left'></i> Continue Shopping</a>")
                }
            }
            else {
                // The user typed the address manually in a new tab
                $("#divContinueShopping").html("<a href='Default.aspx' class='btn btn-block btn-primary'><i class='fa fa-arrow-left'></i> Continue Shopping</a>")
            }

            // Load the cart
            LoadCart();
            // Display the salon name
            $("#lblSalonName").html(getCookie("salonName"));
        });

        // Returns the path name of a given URL
        function ParseURL(url) {
            var a = document.createElement('a');
            a.href = url;
            return a.pathname;            
        }

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

        
        function GoBack() {
            window.history.back(1);
        }

        function GoToPage(page) {
            window.open(page, '_parent', null, false);
        }
        
        function serialize(object) {
            return JSON.stringify(object);
        }

        function deserialize(string) {
            var object = JSON.parse(string);
            return object;
        }

        function FormatTextSingularAndPlural(quantity, singularText, leadingText, betweenText) {

            if (quantity <= 1) {
                // The quantity is less than or equal to 1 so the singular form of the text should be returned
                return leadingText + quantity + betweenText + singularText;
            }
            else {
                // The quantity is greater than 1 so the plural form of the text should be returned
                if (singularText.substring(singularText.length - 1) == "y") {
                    // The singular text ends with y, so let us change it to ies
                    // But first, we will remove the last letter which is y
                    singularText = singularText.substring(0, singularText.length - 1);
                    return leadingText + quantity + betweenText + singularText + "ies";
                }
                else {
                    // The singular form does not end with y, so let us append an s to it
                    return leadingText + quantity + betweenText + singularText + "s";
                }
            }
        }

        // Reduce Quantity of a Service
        function ReduceQuantity(serviceID) {
            // Look for the cart item in the array
            var index;
            for (index = 0; index < shoppingCart.length; index++) {
                if (shoppingCart[index].id == serviceID) {

                    // Decrease the quantity by 1
                    var newQuantity = shoppingCart[index].quantity - 1;

                    // Set the new quantity for this cart item
                    shoppingCart[index].quantity = newQuantity;

                    // If the new quantity = 0, we should remove the item from the cart
                    if (newQuantity == 0) {
                        
                        // Remove the item from the cart
                        shoppingCart.splice(index, 1);
                    }
                }
            }

            // store value in session
            Session.set("cart", shoppingCart);

            // Save the updated cart to the cookie
            setCookie('cart', serialize(shoppingCart));

            // Update the cart
            UpdateCart(shoppingCart);

            
        }

        // Increase Quantity of a Service
        function IncreaseQuantity(serviceID) {
            // Look for the cart item in the array
            var index;
            for (index = 0; index < shoppingCart.length; index++) {
                if (shoppingCart[index].id == serviceID) {

                    // Increment the quantity by 1
                    var newQuantity = shoppingCart[index].quantity + 1;

                    // Set the new quantity for this cart item
                    shoppingCart[index].quantity = newQuantity;
                }
            }

            // store value in session
            Session.set("cart", shoppingCart);

            // Save the updated cart to the cookie
            setCookie('cart', serialize(shoppingCart));

            // Update the cart
            UpdateCart(shoppingCart);

            
        }

        

        // the cart through the web service
        function UpdateCart(shoppingCart) {
            $.ajax({
                url: "BeautifyWebService/BeautifyWebService.asmx/UpdateCart",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: $.toJSON({ serializedShoppingCartItems: serialize(shoppingCart) }),
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
                    $.fn.dpToast("Cart Updated");
                }
            });
        }

        // Load the cart through the web service
        function LoadCart() {
            $.ajax({
                url: "BeautifyWebService/BeautifyWebService.asmx/LoadCart",
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

        // onload
        window.onload = function () {
            
            // store value in session
            Session.set("cart", getCookie("cart"));
                        
        };

        // Returns the value of a specified query parameter
        function GetQueryParameterValue(key) {
            var vars = [], hash;
            // Here, we are reading all query values after ?
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0]);
                // Let us check whether there's a # symbol in the value of the current query parameter.
                if (hash[1].indexOf("#") == -1) {
                    // At this point, it means there's no hash symbol in the value of the current query parameter
                    vars[hash[0]] = hash[1];
                }
                else {
                    // At this point, we are reading the value of the query parameter but we are stopping at the # symbol
                    // We are stopping there because the address field contains a # which might have been used to take the user to a particular section of the page
                    vars[hash[0]] = hash[1].substring(0, hash[1].indexOf("#"));
                }
            }
            return vars[key];
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
