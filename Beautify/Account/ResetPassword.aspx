<%@ Page Title="Reset Password" Language="C#" MasterPageFile="~/Site2.Master" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="Beautify.WebForm11" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPageContent" runat="server">
    <!-- Horizontal Menu Header -->
                        <div class="content-header">
                            <div class="header-section">
                                <h1>
                                    <i class="gi gi-lock"></i>Reset Password<br><small>Reset Your Password!</small>
                                </h1>
                            </div>
                        </div>                        
    <!-- END Horizontal Menu Header -->

    <!-- Horizontal Menu Block -->
                        <div class="block">
                            <!-- Horizontal Menu Title -->
                            <div class="block-title">
                                <h2>Reset Password</h2>
                            </div>
                            <!-- END Horizontal Menu Title -->

                            <!-- Horizontal Menu Content -->
                            <p align="center">
                                <asp:PasswordRecovery align="center" runat="server" ID="PasswordRecovery1" SubmitButtonStyle-CssClass="btn btn-primary" TextBoxStyle-CssClass="form-control"
                                     FailureTextStyle-ForeColor="Red" ValidatorTextStyle-ForeColor="Red"></asp:PasswordRecovery>
                            </p>
                            <!-- END Horizontal Menu Content -->
                        </div>
                        <!-- END Horizontal Menu Block -->

</asp:Content>
