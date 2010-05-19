<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="DIExample.Services.ViewPageBase" %>
<%@ Import Namespace="DIExample.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
    <p>
        To learn more about ASP.NET MVC visit <a href="http://asp.net/mvc" title="ASP.NET MVC Website">http://asp.net/mvc</a>.
        <%= HtmlBuilder.EditLink<HomeController>(c => c.Edit()) %>
    </p>
</asp:Content>
