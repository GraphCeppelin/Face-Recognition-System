<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Log On by Photo
</asp:Content>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Log On</h2>
    <p>
        Please upload your photo.
        <%= Html.ActionLink("Register", "Register") %>
        if you don't have an account.
    </p>
    <%= Html.ValidationSummary("Login was unsuccessful. Please correct the errors and try again.") %>
    <% using (Html.BeginForm())
       { %>
    <div>
        <fieldset>
            <legend>Log on by photo</legend>
            <p>
                <input type="file" name="Photo" id="Photo" style="width: 100%;" accept="image/*" onchange="ChangeImage('Photo','#PhotoThumbnail')" />
                <br />
                <label>
                    Photo:</label><br />
                <img src="" alt="" id="PhotoThumbnail" width="100%"  style="visibility: hidden;"/>
            </p>
                <fieldset>
                    <label>
                        Recognition algorithm:</label>
                    <select id="Algorithm" name="Algorithm" style="width: 100%;" >
                        <option value="0">"Eigen Faces"</option>
                        <option value="1">"Fisher Faces"</option>
                    </select>
                </fieldset>
            </p>
            <p>
                <%= Html.CheckBox("rememberMe") %>
                <label class="inline" for="rememberMe">
                    Remember me?</label>
            </p>
            <p>
                <input type="submit" value="Log On" style="width: 100%;" />
            </p>
        </fieldset>
    </div>
    <% } %>
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
