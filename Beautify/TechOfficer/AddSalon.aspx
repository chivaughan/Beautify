<%@ Page Title="" Language="C#" MasterPageFile="~/TechOfficer/TechOfficer.Master" AutoEventWireup="true" CodeBehind="AddSalon.aspx.cs" Inherits="Beautify.Admin.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPageContent" runat="server">
    <!-- Horizontal Menu Header -->
                        <div class="content-header">
                            <div class="header-section">
                                <h1>
                                    <i class="gi gi-lock"></i>Add Salon<br><small>Create Account for a New Salon!</small>
                                </h1>
                            </div>
                        </div>                        
    <!-- END Horizontal Menu Header -->

    <!-- Horizontal Menu Block -->
                        <div class="block">
                            <!-- Horizontal Menu Title -->
                            <div class="block-title">
                                <h2>Add Salon</h2>
                            </div>
                            <!-- END Horizontal Menu Title -->

                            <!-- Horizontal Menu Content -->
                            <p align="center">
                                <asp:CreateUserWizard align="center" runat="server" ID="CreateUserWizard1" TextBoxStyle-CssClass="form-control" CreateUserButtonStyle-CssClass="btn btn-primary" ValidatorTextStyle-ForeColor="Red"
                                     ContinueDestinationPageUrl="AddSalon.aspx" FinishCompleteButtonStyle-CssClass="btn btn-primary" CancelButtonStyle-CssClass="btn btn-danger" ContinueButtonStyle-CssClass="btn btn-primary" FinishPreviousButtonStyle-CssClass="btn btn-primary" CompleteSuccessText="The salon's account has been successfully created." LoginCreatedUser="False" OnCreatedUser="CreateUserWizard1_CreatedUser" UnknownErrorMessage="The salon's account was not created. Please try again."></asp:CreateUserWizard>
                            </p>
                            <!-- END Horizontal Menu Content -->
                        </div>
                        <!-- END Horizontal Menu Block -->
</asp:Content>
