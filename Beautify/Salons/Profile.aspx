<%@ Page Title="" Language="C#" MasterPageFile="~/Salons/Salons.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="Beautify.Salons.WebForm1" %>



<asp:Content ID="Content1" ContentPlaceHolderID="cphPageContent" runat="server">
    <asp:ScriptManager runat="server" ID="ScriptManager1"></asp:ScriptManager>
    <!-- eCommerce Products Header -->
                        <div class="content-header">
                            <ul class="nav-horizontal text-center">
                                <li>
                                    <a href="Default.aspx"><i class="fa fa-bar-chart"></i> Dashboard</a>
                                </li>
                                <li class="active">
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

    
    

                        <!-- Example Block -->
                        <div class="block">
                            <!-- Example Title -->
                            <div class="block-title">
                                <h2>Edit Your Profile</h2>
                            </div>
                            <!-- END Example Title -->
                            <div runat="server" id="divMessage">
                            
                            </div>

                            <!-- Step Info -->
                            <ul class="nav nav-tabs push-bit" data-toggle="tabs">
                                    <li id="tabSalon" class="active"><a href="#salon"><strong>SALON</strong></a></li>
                                    <li id="tabBooking"><a href="#booking"><strong>BOOKING</strong></a></li>
                                <li id="tabBank"><a href="#bank"><strong>BANK</strong></a></li>
                                </ul>
                                <!-- END Step Info -->

                            <!-- TAB CONTAINER-->
                            <div id="tabContainer">
                            <!-- Tab Content -->
                            <div class="tab-content">
                            <!-- SALON -->
                            <div class="tab-pane active" id="salon">
                                
                                <div class="alert alert-info">
                        <h4><i class="fa fa-exclamation"></i> Salon Info</h4> Edit details of your salon
                        </div>
                                 
                                    <p align="center">
                                  <img runat="server" id="imgProfile" height="200" width="200" class="img-circle"/>
                                        </p>
                                <div runat="server" id="divUploadMessage">

                                </div>
                                <div align="center">
                                    <asp:FileUpload ID="FileUpload1" runat="server" />
                                    <asp:CustomValidator ID="CustomValidator1" ControlToValidate="FileUpload1" ValidationGroup="Upload" ForeColor="Red"
                                        runat="server" ErrorMessage="Please select a valid file type (*.jpg, *.jpeg, *.png)." 
                                        onservervalidate="CustomValidator1_ServerValidate"></asp:CustomValidator>
                                        <br />
                                    <asp:Button runat="server" ID="btnUpload" Text="Upload" CssClass="btn btn-primary" OnClick="btnUpload_Click" ValidationGroup="Upload"/>
                                </div>
                                
                                  
                                    <div class="form-horizontal form-bordered">
                                        
                                        <div class="form-group">
                                                <label class="col-md-3 control-label" for="lblSalonUsername">Username</label>
                                                <div class="col-md-6">
                                                    <span><asp:Label runat="server" ID="lblSalonUsername"></asp:Label></span>
                                                    </div>
                                                </div>

                                        <div class="form-group">
                                                <label class="col-md-3 control-label" for="lblSalonEmail">Email</label>
                                                <div class="col-md-6">
                                                    <span><asp:Label runat="server" ID="lblSalonEmail"></asp:Label></span>
                                                    </div>
                                                </div>
                                        <div class="form-group">
                                                <label class="col-md-3 control-label" for="lblBookingLink">Booking Link</label>
                                                <div class="col-md-6" runat="server" id="divBookingLink">
                                                    
                                                    </div>
                                                </div>
                                        <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtSalonName">Salon Name</label><label style="color:red;">*</label>
                                                <div class="col-md-6">
                                                    <asp:TextBox MaxLength="50" ID="txtSalonName" runat="server" CssClass="form-control" placeholder="Salon Name"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" ErrorMessage="Your salon's name is required" ForeColor="Red" ControlToValidate="txtSalonName" ValidationGroup="Update" ></asp:RequiredFieldValidator>
                                                    </div>
                                                </div>
                                    <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtSalonPhoneNumber">Phone Number</label><label style="color:red;">*</label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="txtSalonPhoneNumber" MaxLength="11" runat="server" CssClass="form-control" placeholder="Your Phone Number"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ValidationGroup="Update" ErrorMessage="Your phone number is required" ForeColor="Red" ControlToValidate="txtSalonPhoneNumber"></asp:RequiredFieldValidator>
                                                    <div class="alert alert-info">
                        This will not be visible to everyone. It will only be used to contact you.
                        </div>
                                                </div>
                                            </div>
                                        <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtAboutSalon">About Us/Address</label><label style="color:red;">*</label>
                                                <div class="col-md-6">
                                                    <textarea rows="10" runat="server" ID="txtAboutSalon" class="form-control" placeholder="Say something about your salon. Also list all addresses of your salon in each city you operate"></textarea>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ValidationGroup="Update" ErrorMessage="Say something about your salon and list adresses of your salon" ForeColor="Red" ControlToValidate="txtAboutSalon"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                        
                                        </div>
                                
                                    <br /><br />
                            <!-- Form Buttons -->
                            <div class="form-group text-right">                                
                            <button type="button" class="btn btn-primary" onclick="GoToTab('booking')">Next <i class="fa fa-arrow-right"></i></button>
                            </div>
                            <!-- END Form Buttons -->
                                    
                                </div>
                            
                            <!-- END SALON -->

                            <!-- BOOKING -->
                            <div class="tab-pane" id="booking">
                                
                                <div class="alert alert-info">
                        <h4><i class="fa fa-exclamation"></i> Booking Settings</h4> Configure how clients should book your salon on Beautify
                        </div>
                                    
                                    <div class="form-horizontal form-bordered">
                                        
                                    <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtClientName">Booking</label>
                                                <div class="col-md-6">
                                                    OFF <label class="switch switch-primary"><input id="chkBookingStatus" runat="server" checked="" type="checkbox"><span></span></label> ON
                                                    </div>
                                                </div>
                                            
                                        <div class="form-group">
                                                <label class="col-md-3 control-label" for="selSalonLocations">Select Cities where Your Salon is Available</label><label style="color:red;">*</label>
                                               <div class="col-md-6">
                                                    <select id="selSalonLocations" runat="server" name="example-chosen-multiple" class="select-chosen" data-placeholder="Select Cities" style="width: 250px; display: none;" multiple="">
                                                                                                               
                                                    </select>
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ValidationGroup="Update" ErrorMessage="You must select at least 1 city" ForeColor="Red" ControlToValidate="selSalonLocations"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                    
                                        <div class="form-group">
                                                <label class="col-md-3 control-label" for="selSalonLocationsInCities">Select Locations in the Cities where Your Salon is Available</label><label style="color:red;">*</label>
                                               <div class="col-md-6">
                                                    <select id="selSalonLocationsInCities" runat="server" name="example-chosen-multiple" class="select-chosen" data-placeholder="Select Locations" style="width: 250px; display: none;" multiple="">
                                                                                                               
                                                    </select>
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ValidationGroup="Update" ErrorMessage="You must select at least 1 location" ForeColor="Red" ControlToValidate="selSalonLocationsInCities"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>

                                        <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtSalonOpeningTime">Opening Time</label><label style="color:red;">*</label>
                                                <div class="col-md-3">
                                                    <div class="input-group bootstrap-timepicker">
                                            <asp:TextBox onclick="SelectTime('Opening');" MaxLength="7" CssClass="time form-control" Text="7:00am" placeholder="Select Opening Time" runat="server" ID="txtSalonOpeningTime"></asp:TextBox> 
                                            <span class="input-group-btn">
                                                            <button type="button" onclick="SelectTime('Opening');" class="btn btn-primary"><i class="fa fa-clock-o"></i></button>
                                                        </span> 
                                                    </div>  
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ValidationGroup="Update" ErrorMessage="Please select your salon's opening time" ForeColor="Red" ControlToValidate="txtSalonOpeningTime"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>

                                        <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtSalonClosingTime">Closing Time</label><label style="color:red;">*</label>
                                                <div class="col-md-3">
                                                    <div class="input-group bootstrap-timepicker">
                                            <asp:TextBox onclick="SelectTime('Closing');" MaxLength="7" CssClass="time form-control" Text="5:00pm" placeholder="Select Closing Time" runat="server" ID="txtSalonClosingTime"></asp:TextBox> 
                                            <span class="input-group-btn">
                                                            <button type="button" onclick="SelectTime('Closing');" class="btn btn-primary"><i class="fa fa-clock-o"></i></button>
                                                        </span> 
                                                    </div>  
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ValidationGroup="Update" ErrorMessage="Please select your salon's Closing time" ForeColor="Red" ControlToValidate="txtSalonClosingTime"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                            
                                        <div class="form-group">
                                                <label class="col-md-3 control-label" for="example-timepicker">Select the Days that Your Salon Does Not Operate</label>
                                                <div class="col-md-6">
                                                    <select id="selDisabledDays" runat="server" name="example-chosen-multiple" class="select-chosen" data-placeholder="Select Days" style="width: 250px; display: none;" multiple="">
                                                        <option value="Monday">Monday</option>
                                                        <option value="Tuesday">Tuesday</option>
                                                        <option value="Wednesday">Wednesday</option>
                                                        <option value="Thursday">Thursday</option>
                                                        <option value="Friday">Friday</option>
                                                        <option value="Saturday">Saturday</option>
                                                        <option value="Sunday">Sunday</option>                                                        
                                                    </select>
                                                    <div class="alert alert-info">
                        If you leave this blank, it means your salon operates everyday
                        </div>
                                                </div>
                                            </div>
                                        
                                            
                                        
                                        </div>
                                
                                    <br /><br />
                            <!-- Form Buttons -->
                            <div class="form-group text-right">                                
                                <button type="button" class="btn btn-primary" onclick="GoToTab('salon')"><i class="fa fa-arrow-left"></i> Previous</button>
                            <button type="button" class="btn btn-primary" onclick="GoToTab('bank')">Next <i class="fa fa-arrow-right"></i></button>
                            </div>
                            <!-- END Form Buttons -->
                                    
                                </div>
                            
                            <!-- END BOOKING -->

                            <!-- BANK -->
                            <div class="tab-pane" id="bank">
                                
                                <div class="alert alert-info">
                        <h4><i class="fa fa-exclamation"></i> Bank Account Info</h4> Enter the account where your earnings should be deposited
                        </div>
                                    
                                    <div class="form-horizontal form-bordered">

                                        
                                        <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtBankName">Bank</label><label style="color:red;">*</label>
                                                <div class="col-md-6">
                                                    <asp:TextBox MaxLength="50" ID="txtBankName" runat="server" CssClass="form-control" placeholder="Bank"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ValidationGroup="Update" ErrorMessage="Your bank's name is required" ForeColor="Red" ControlToValidate="txtBankName"></asp:RequiredFieldValidator>
                                                    </div>
                                                </div>
                                    <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtBankAccountName">Account Name</label><label style="color:red;">*</label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="txtBankAccountName" MaxLength="100" runat="server" CssClass="form-control" placeholder="Account Name"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ValidationGroup="Update" ErrorMessage="Your account name is required" ForeColor="Red" ControlToValidate="txtBankAccountName"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                        <div class="form-group">
                                                <label class="col-md-3 control-label" for="txtBankAccountNumber">Account Number</label><label style="color:red;">*</label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="txtBankAccountNumber" MaxLength="11" runat="server" CssClass="form-control" placeholder="Account Number"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ValidationGroup="Update" ErrorMessage="Your account number is required" ForeColor="Red" ControlToValidate="txtBankAccountNumber"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>

                                        </div>
                                
                                
                                <br /><br />
                            <!-- Form Buttons -->
                            <div class="form-group text-right">
                                <button type="button" class="btn btn-primary" onclick="GoToTab('booking')"><i class="fa fa-arrow-left"></i> Previous</button>
                            
                            </div>
                            <!-- END Form Buttons -->

                            </div>
                            <!-- END BANK -->
                            </div>
                            <!-- End Tab Content -->
                            </div>
                        <!-- END TAB CONTAINER-->
                        
                            <asp:Button runat="server" ID="btnUpdateProfile" ValidationGroup="Update" Text="Save Changes" CssClass="btn btn-success btn-block btn-lg" OnClientClick="if (!ValidateFormControls()) { return false;};" OnClick="btnUpdateProfile_Click" />
                            <br /><br />
                        </div>
                        <!-- END Example Block -->

    <script type="text/javascript">
        $(document).ready(function () {
            // Set the opening timepicker for the salon
            $('#' + '<%=txtSalonOpeningTime.ClientID%>').timepicker({
                'minTime': '12:30am',
                'maxTime': '12:00am',
                'showDuration': false,
                'disableTextInput': true
            });

            // Set the closing timepicker for the salon
            $('#' + '<%=txtSalonClosingTime.ClientID%>').timepicker({
                'minTime': '12:30am',
                'maxTime': '12:00am',
                'showDuration': false,
                'disableTextInput': true
            });
        });

        function SelectTime(time) {
            switch (time) {
                case "Opening":
                    $('#' + '<%=txtSalonOpeningTime.ClientID%>').timepicker('show');
                    break;
                case "Closing":
                    $('#' + '<%=txtSalonClosingTime.ClientID%>').timepicker('show');
                    break;
            }


        }

        function GoToTab(tab) {
            // Let us switch the tab
            switch (tab) {
                case "salon":
                    $("#" + tab).attr("class", "tab-pane active");
                    $("#booking").attr("class", "tab-pane");
                    $("#bank").attr("class", "tab-pane");
                    $("#tabSalon").attr("class", "active");
                    $("#tabBooking").attr("class", "");
                    $("#tabBank").attr("class", "");
                    window.open('#tabContainer', '_parent', null, false);
                    break;
                case "booking":
                    $("#" + tab).attr("class", "tab-pane active");
                    $("#salon").attr("class", "tab-pane");
                    $("#bank").attr("class", "tab-pane");
                    $("#tabBooking").attr("class", "active");
                    $("#tabSalon").attr("class", "");
                    $("#tabBank").attr("class", "");
                    window.open('#tabContainer', '_parent', null, false);
                    break;
                case "bank":
                    $("#" + tab).attr("class", "tab-pane active");
                    $("#salon").attr("class", "tab-pane");
                    $("#booking").attr("class", "tab-pane");
                    $("#tabBank").attr("class", "active");
                    $("#tabSalon").attr("class", "");
                    $("#tabBooking").attr("class", "");
                    window.open('#tabContainer', '_parent', null, false);
                    break;
            }

        }

        function ValidateFormControls() {
            if (document.getElementById("<%=txtSalonName.ClientID%>").value == "" || document.getElementById("<%=txtSalonPhoneNumber.ClientID%>").value == "" || document.getElementById("<%=txtAboutSalon.ClientID%>").value == "" || document.getElementById("<%=selSalonLocations.ClientID%>").value == "" || document.getElementById("<%=txtSalonOpeningTime.ClientID%>").value == "" || document.getElementById("<%=txtSalonClosingTime.ClientID%>").value == "" || document.getElementById("<%=txtBankName.ClientID%>").value == "" || document.getElementById("<%=txtBankAccountName.ClientID%>").value == "" || document.getElementById("<%=txtBankAccountNumber.ClientID%>").value == "") {
                alert("Please fill all fields marked with *\n\nYou can use the Previous or Next button to move between tabs");
                return false;
            }
            else {
                return true;
            }
        }
    </script>

    <script type="text/javascript">
        function checkFileExtension(elem) {
            var filePath = elem.value;

            if (filePath.indexOf('.') == -1)
                return false;

            var validExtensions = new Array();
            var ext = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();

            validExtensions[0] = 'jpg';
            validExtensions[1] = 'jpeg';
            validExtensions[2] = 'png';

            for (var i = 0; i < validExtensions.length; i++) {
                if (ext == validExtensions[i])
                    return true;
            }

            alert('The file extension ' + ext.toUpperCase() + ' is not allowed!');
            return false;
        }
    </script>
</asp:Content>
