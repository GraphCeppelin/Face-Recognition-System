<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">    
    <% if (Request.IsAuthenticated) {%>
    <fieldset>
     <legend>Main menu</legend>
     <p>
     <nav> 
         <ul>
              <li><%= Html.ActionLink("Update my information", "UpdateMyInformation", "Home")%></li>
              <li><%= Html.ActionLink("Search/update a person", "PersonSearch", "Home")%></li>
              <li><%= Html.ActionLink("Add new person", "AddNewPerson", "Home")%></li>
              <li><%= Html.ActionLink("Exit", "LogOff", "Account") %></li>
         </ul>
     </nav>
    </p>
    </fieldset>
  <%  }%>
</asp:Content>
