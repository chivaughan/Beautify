<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PagingUserControl.ascx.cs" Inherits="Beautify.Paging.PagingUserControl" %>
<link type="text/css" href="../content/pagination/css/devasolPager.css" rel="stylesheet" />

<div class="col-xs-12 text-right" id="pagingSection" runat="server">
    <ul class='pagination'>
        <li><asp:LinkButton ID="lnkPrevious" runat="server" CssClass='page-numbers prev' Visible="false" OnClick="LinkButton_Click"><i class='fa fa-arrow-left'></i></asp:LinkButton></li>
<li><asp:LinkButton ID="lnkFirst" runat="server" CssClass='page-numbers' Visible="false"
    OnClick="LinkButton_Click">1</asp:LinkButton></li>
        <li><asp:Label runat="server" ID="lblFirstDots" CssClass="page-numbers prev" Visible="false"
    Text="..."></asp:Label></li>
        <li><asp:PlaceHolder ID="plhDynamicLink" runat="server"></asp:PlaceHolder></li>
        <li><asp:Label runat="server" ID="lblSecondDots" Visible="false" CssClass="page-numbers prev"
    Text="..."></asp:Label></li>
        <li><asp:LinkButton ID="lnkLast" runat="server" CssClass='page-numbers' Visible="false"
    OnClick="LinkButton_Click">Last</asp:LinkButton></li>
        <li><asp:LinkButton ID="lnkNext" runat="server" CssClass='page-numbers next' Visible="false" OnClick="LinkButton_Click"><i class='fa fa-arrow-right'></i></asp:LinkButton></li>

 </ul>
        </div>

