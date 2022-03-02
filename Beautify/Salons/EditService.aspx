<%@ Page Title="" Language="C#" MasterPageFile="~/Salons/Salons.Master" AutoEventWireup="true" CodeBehind="EditService.aspx.cs" Inherits="Beautify.Salons.WebForm3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphPageContent" runat="server">
    <asp:ScriptManager runat="server" ID="ScriptManager1"></asp:ScriptManager>
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
                                <li class="active">
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
    <div class="row">
        <div class="col-lg-12">
                                <!-- General Data Block -->
                                <div class="block">
                                    <!-- General Data Title -->
                                    <div class="block-title">
                                        <h2><i class="fa fa-pencil"></i> <strong>Service</strong> Data</h2>
                                    </div>
                                    <!-- END General Data Title -->

                                    <div runat="server" id="divMessage">

                                    </div>

                                    
                                
                                    <p align="center">
                                  <img runat="server" id="imgProfile" src="../content/uploads/images/services/default.jpg" height="200" width="200" class="img-circle"/>
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
                                
                                    <br />
                                    <!-- General Data Content -->
                                    <div class="form-horizontal form-bordered">
                                        <div class="form-group" id="divServiceID" runat="server">
                                            <label class="col-md-3 control-label" for="lblServiceID">Service ID</label>
                                            <div class="col-md-9">
                                                <asp:Label runat="server" ID="lblServiceID"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-3 control-label" for="txtServiceName">Name</label>
                                            <div class="col-md-9">
                                                <asp:TextBox runat="server" MaxLength="50" ID="txtServiceName" class="form-control" placeholder="Enter service name.."></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ValidationGroup="Service" ErrorMessage="The service name is required" ForeColor="Red" ControlToValidate="txtServiceName"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-3 control-label" for="txtShortDescription">Short Description</label>
                                            <div class="col-md-9">
                                                <textarea maxlength="100" rows="3" runat="server" ID="txtShortDescription" class="form-control" placeholder="Short description of the service"></textarea>
                                             <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ValidationGroup="Service" ErrorMessage="The short description is required" ForeColor="Red" ControlToValidate="txtShortDescription"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-3 control-label" for="txtFullDescription">Full Description</label>
                                            <div class="col-md-9">
                                                <textarea maxlength="300" rows="5" runat="server" ID="txtFullDescription" class="form-control" placeholder="Full description of the service"></textarea>
                                               <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ValidationGroup="Service" ErrorMessage="The full description is required" ForeColor="Red" ControlToValidate="txtFullDescription"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-3 control-label" for="selServiceCategory">Category</label>
                                            <div class="col-md-8">
                                                <select id="selServiceCategory" runat="server" class="select-chosen" data-placeholder="Choose Category.." style="width: 250px;">
                                                    
                                                </select>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-3 control-label" for="txtServiceCost">Price</label>
                                            <div class="col-md-8">
                                                <div class="input-group">
                                                    <div class="input-group-addon"><i><%=Beautify.AppHelper.GetCurrencySymbol() %></i></div>
                                                    <asp:TextBox runat="server" MaxLength="7" ID="txtServiceCost" class="form-control" placeholder="0.00"></asp:TextBox>
                                                </div>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ValidationGroup="Service" ErrorMessage="The price is required" ForeColor="Red" ControlToValidate="txtServiceCost"></asp:RequiredFieldValidator>
                                                <asp:RangeValidator runat="server" ID="RangeValidator1" ValidationGroup="Service" ErrorMessage="Minimum price is 2000. Max price is 9999999" ForeColor="Red" ControlToValidate="txtServiceCost" MinimumValue="2000" Type="Integer" MaximumValue="9999999"></asp:RangeValidator>
                                                <div class="alert alert-info" runat="server" id="divPricingInfo">
                        
                        </div>
                                        </div>
                                        </div>
                                        
                                        <div class="form-group">
                                            <label class="col-md-3 control-label">Status</label>
                                            <div class="col-md-9">
                                                DISCONTINUED  
                                                <label class="switch switch-primary">
                                                     <input type="checkbox" runat="server" id="chkServiceStatus" name="product-status" checked> <span></span>
                                                </label>
                                                  AVAILABLE
                                            </div>
                                        </div>
                                        <div class="form-group form-actions">
                                            <div class="col-md-9 col-md-offset-3">
                                                <asp:LinkButton runat="server" ValidationGroup="Service" ID="lnkUpdate" ToolTip="Save" class="btn btn-md btn-success" OnClick="lnkUpdate_Click"><i class="fa fa-floppy-o"></i> Save</asp:LinkButton>
                                                <asp:LinkButton runat="server" ValidationGroup="Service" ID="lnkAdd" ToolTip="Add" class="btn btn-md btn-success" OnClick="lnkAdd_Click"><i class="fa fa-plus"></i> Add</asp:LinkButton>
                                                <button runat="server" id="btnReset" type="reset" class="btn btn-md btn-warning" title="Reset"><i class="fa fa-repeat"></i> Reset</button>
                                            </div>
                                        </div>
                                    </div>
                                    <!-- END General Data Content -->
                                </div>
                                <!-- END General Data Block -->
        </div>                    
     </div>   

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
