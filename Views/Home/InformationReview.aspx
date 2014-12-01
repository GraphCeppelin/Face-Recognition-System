<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Update user information
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Update user information</h2>
    <div>
        <fieldset>
            <legend>User details</legend>
            <div id="Div2" style="width: 100%">
                <div id="Div7" style="position: relative; width: 100%;" align="center">
                    <fieldset>
                        <legend>Photo</legend>
                        <p>
                            <input type="file" name="Avatar" id="Avatar" style="width: 100%;" accept="image/*"
                                onchange="ChangeImage('Avatar','#AvatarThumbnail')" />
                        </p>
                        <img alt="" id="AvatarThumbnail" src="/Home/ImageLoader?ImageID=Avatar" width="50%" /><br>
                        <div id="Div1" style="display: inline-block; width: 100%;" align="center">
                            <%= Html.ActionLink("Edit photos", "PhotosManager", "Home")%></div>
                    </fieldset>
                </div>
                <div id="Div4" style="position: relative; width: 100%;">
                    <% using (Html.BeginForm("InformationUpdate", "Account"))
                       { %>
                    <fieldset>
                        <legend>Person info</legend>
                        <div id="Div5" align="left">
                            <% if (!string.IsNullOrWhiteSpace((string)Session["LastRecognitionAlgorithm"]) && Session["LastRecognitionTime"] != null && (double)Session["LastRecognitionTime"] > 0)
                               {%>
                               <font size = 6>
                                Last recognition method:
                            <%=Session["LastRecognitionAlgorithm"]%><br>
                                Last recognition time (seconds):
                            <%=Session["LastRecognitionTime"]%>
                               </font>
                            <%}%>
                            <label for="name">
                                Name:</label><%= Html.TextBox("name", Session["PersonForReview"] != null ? (Session["PersonForReview"] as FaceRecognitionSystem.Infrastructure.MembershipPerson).person.Name : "", new { style = "width:100%" })%>
                        </div>
                        <div id="Div6" align="left">
                            <label for="gender">
                                Gender:</label><%= Html.TextBox("gender", Session["PersonForReview"] != null ? (Session["PersonForReview"] as FaceRecognitionSystem.Infrastructure.MembershipPerson).person.Gender : "", new { style = "width:100%" })%>
                        </div>
                        <p>
                            <div id="Div9" style="display: inline-block; width: 100%;" align="center">
                                <input type="submit" value="Save" style="width: 100%;" /></div>
                        </p>
                        <% } %>
                        <div id="Div8" style="display: inline-block; width: 100%;" align="center">
                            <%= Html.ActionLink("Back", "Index", "Home")%></div>
                        <div id="Div3" style="display: inline-block; width: 100%;" align="center">
                            <%= Html.ActionLink("Exit", "LogOff", "Account")%></div>
                    </fieldset>
                </div>
            </div>
            </br>
        </fieldset>
    </div>
    <script type="text/javascript">
        function ChangeImage(fileId, imageId) {
            var myform = document.createElement("form");
            myform.style.display = "none";
            myform.action = "/Account/UpdateAvatar";
            myform.enctype = "multipart/form-data";
            myform.method = "post";
            var imageLoad;
            var imageLoadParent;
            imageLoad = document.getElementById(fileId);
            imageLoadParent = document.getElementById(fileId).parentNode;
            myform.appendChild(imageLoad);
            document.body.appendChild(myform);
            $(myform).ajaxSubmit({ success:
        function (responseText) {
            var d = new Date();
            if (responseText == "error") {
                $(imageId)[0].style.visibility = 'hidden';
            } else {
                $(imageId)[0].style.visibility = 'visible';
                $(imageId)[0].src = "/Account/ImageLoad?a=" + d.getMilliseconds();
            }
            imageLoadParent.appendChild(myform.firstChild);
        },
                error: function () {
                    alert("Unknown error.");
                    $(imageId)[0].style.visibility = 'hidden';
                    imageLoadParent.appendChild(myform.firstChild);
                }
            });
        }
    </script>
</asp:Content>
