<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
    }
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Manager of photos
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Manager of photos</h2>
    <div>
        <fieldset>
            <legend>User details</legend>
            <div id="Div2" style="width: 100%">
                <div style="position: relative; width: 100%;">
                    <fieldset>
                        <legend>Person info</legend>
                        <div id="Div5" align="left">
                            <label for="name">
                                Name:
                                <%= Session["PersonForReview"] != null ? (Session["PersonForReview"] as FaceRecognitionSystem.Infrastructure.MembershipPerson).person.Name : "" %></label>
                        </div>
                        <div id="Div6" align="left">
                            <label for="gender">
                                Gender:
                                <%= Session["PersonForReview"] != null ? (Session["PersonForReview"] as FaceRecognitionSystem.Infrastructure.MembershipPerson).person.Gender : ""%></label>
                        </div>
                    </fieldset>
                    <div style="display: inline-block; width: 100%;" align="center">
                        <%= Html.ActionLink("Back", "InformationReview", "Home")%></div>
                    <div style="display: inline-block; width: 100%;" align="center">
                        <%= Html.ActionLink("Exit", "LogOff", "Account")%></div>
                </div>
                <div align="center">
                    <fieldset>
                        <legend>Upload photo</legend>
                        <p>
                            <input type="file" name="Photo" id="Photo" style="width: 100%;" accept="image/*"
                                onchange="ChangeImage('Photo','#CurrentPhotoThumbnail')" />
                            <p />
                            <img alt="" id="CurrentPhotoThumbnail" width="50%" /><br>
                            <div style="display: inline-block; width: 100%;" align="center">
                                <% using (Html.BeginForm("AddPhoto", "Home"))
                                   { %>
                                <input type="submit" value="Add" style="width: 100%;" />
                                <% } %>
                            </div>
                    </fieldset>
                </div>
                <div id="PhotoContainer" style="position: relative; overflow: auto; max-width: 100%;
                    max-height: 100%;">
                    <fieldset>
                        <legend>Photos</legend>
                        <%
                            foreach (FaceRecognitionSystem.Entities.Photo p in (Session["PersonForReview"] as
                                FaceRecognitionSystem.Infrastructure.MembershipPerson).person.Photos.OrderByDescending(x => x.PhotoID))
                            {   
                        %>
                        <div id="div_<%=p.PhotoID%>" align="center">
                            <img alt="" id="img_<%=p.PhotoID%>" src="/Home/ImageLoader?ImageID=<%=p.PhotoID%>"
                                width="50%" /><br>
                            <a style="display: inline-block; width: 100%;" onclick="DeletePhoto('<%=p.PhotoID%>')">
                                Remove</a>
                        </div>
                        <%}%>
                    </fieldset>
                </div>
            </div>
        </fieldset>
    </div>
    <script type="text/javascript">
        function DeletePhoto(imageId) {
            if (!confirm('Are you sure you want to remove this image?')) return
            var myform = document.createElement("form");
            myform.style.display = "none";
            myform.action = "/Home/RemoveImage?imageid=" + imageId;
            myform.enctype = "multipart/form-data";
            myform.method = "post";
            var imageLoad = document.getElementById("img_" + imageId);

            if (imageLoad.Value == '') return;
            $(myform).ajaxSubmit
        ({ success:
            function (responseText) {
                imageLoad.parentNode.parentNode.removeChild(imageLoad.parentNode);
            },
            error: function () {
                alert("Unknown error during operation.");
            }
        });
        }
    </script>
    <script type="text/javascript">
        function ChangeImage(fileId, imageId) {
            var myform = document.createElement("form");
            myform.style.display = "none";
            myform.action = "/Account/UploadFace";
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
                alert("Face could not be recognized. Please try again.");
            } else {
                $(imageId)[0].style.visibility = 'visible';
                $(imageId)[0].src = "/Account/ImageLoad?a=" + d.getMilliseconds();
            }
            imageLoadParent.appendChild(myform.firstChild);
        },
                error: function () {
                    alert("Face could not be recognized. Please try again.");
                    $(imageId)[0].style.visibility = 'hidden';
                    imageLoadParent.appendChild(myform.firstChild);
                }
            });
        }
    </script>
</asp:Content>
