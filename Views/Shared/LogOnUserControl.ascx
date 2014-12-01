<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated) {
%>
        Welcome <b><%= Html.Encode(Page.User.Identity.Name) %></b>!
        [ <%= Html.ActionLink("Log Off", "LogOff", "Account") %> ]
<%
    }
    else {
%> 
        [ <%= Html.ActionLink("Log On by UserName", "LogOnByUserName", "Account")%> ]<br>
        [ <%= Html.ActionLink("Log On by Photo", "LogOnByPhoto", "Account") %> ]
<%
    }
%>
