<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site2.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Beautify.WebForm9" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPageContent" runat="server">
    <!-- Horizontal Menu Header -->
                        <div class="content-header">
                            <div class="header-section">
                                <h1>
                                    <i class="gi gi-lock"></i>Login<br><small>(For Registered Salons Only)!</small>
                                </h1>
                            </div>
                        </div>                        
    <!-- END Horizontal Menu Header -->

    <!-- Horizontal Menu Block -->
                        <div class="block">
                            <!-- Horizontal Menu Title -->
                            <div class="block-title">
                                <h2>Login</h2>
                            </div>
                            <!-- END Horizontal Menu Title -->

                            <!-- Horizontal Menu Content -->
                            <p align="center">
                                <asp:Login align="center" runat="server" ID="Login1" TextBoxStyle-CssClass="form-control" LoginButtonStyle-CssClass="btn btn-primary"
                                    TitleText="" PasswordRecoveryText="Forgot Password?" PasswordRecoveryUrl="ResetPassword.aspx" FailureTextStyle-ForeColor="Red" ValidatorTextStyle-ForeColor="Red" OnLoggedIn="Login1_LoggedIn">

                                </asp:Login>
                            </p>
                            <!-- END Horizontal Menu Content -->
                        </div>
                        <!-- END Horizontal Menu Block -->

</asp:Content>
