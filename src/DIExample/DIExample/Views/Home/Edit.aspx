<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<DIExample.Controllers.FooEditModel>" MasterPageFile="~/Views/Shared/Site.Master" %>
<asp:Content runat="server" ID="Content" ContentPlaceHolderID="TitleContent">Edit Foo</asp:Content>
<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="MainContent">
    <h1>Edit Foo</h1>
    <%= Html.ValidationSummary() %>
    <% using (Html.BeginForm()) { %>
        <%= Html.EditorForModel() %>
        <input type="submit" />
    <% } %>
</asp:Content>
