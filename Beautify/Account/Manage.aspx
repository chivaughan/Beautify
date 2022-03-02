<%@ Page Title="Manage Account" Language="C#" MasterPageFile="~/Site2.Master" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="Beautify.WebForm10" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPageContent" runat="server">
    <!-- Horizontal Menu Header -->
                        <div class="content-header">
                            <div class="header-section">
                                <h1>
                                    <i class="gi gi-lock"></i>Manage Account<br><small>Change Your Password</small>
                                </h1>
                            </div>
                        </div>                        
    <!-- END Horizontal Menu Header -->

    <!-- Horizontal Menu Block -->
                        <div class="block">
                            <!-- Horizontal Menu Title -->
                            <div class="block-title">
                                <h2>Change Password</h2>
                            </div>
                            <!-- END Horizontal Menu Title -->

                            <!-- Horizontal Menu Content -->
                            <p align="center">
                                <asp:ChangePassword align="center" runat="server" ID="ChangePassword1" CancelButtonStyle-CssClass="btn btn-danger" ChangePasswordButtonStyle-CssClass="btn btn-primary" ContinueButtonStyle-CssClass="btn btn-primary"
                                     TextBoxStyle-CssClass="form-control" ValidatorTextStyle-ForeColor="Red" FailureTextStyle-ForeColor="Red" ChangePasswordTitleText=""
                                    ContinueDestinationPageUrl="../Salons/Default.aspx" CancelDestinationPageUrl="../Salons/Default.aspx" ></asp:ChangePassword>
                            </p>
                            <!-- END Horizontal Menu Content -->
                        </div>
                        <!-- END Horizontal Menu Block -->

</asp:Content>
