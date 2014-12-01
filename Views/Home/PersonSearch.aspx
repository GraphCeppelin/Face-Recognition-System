<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {

    }
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    PersonSearch
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        PersonSearch</h2>
  <!--       <% Response.Write("<script language=\"javascript\">alert('" + System.Web.HttpContext.Current.Request.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath).Replace('\\', '/') + "');</script>"); %> 
  -->
    <br>
    <%= Html.ValidationSummary() %>
    <div style="position: relative; width: 100%;" align="center">
        <%  
            if (Session["ComparedPhoto"] != null)
            {
        %>
        <fieldset>
            <legend>Search results</legend>
            <div align="center">
                <div style="display: inline-block; width: 30%;">
                    <legend>Searched</legend>
                    <img alt="" src="/Home/ImageLoader?ShowComparedPhoto=1" width="100%" />
                </div>
                <div style="display: inline-block; width: 30%;">
                    <legend>Found</legend>
                    <img alt="" src="/Home/ImageLoader?ImageID=Avatar" width="100%" /><br>
                </div>
            </div>
            <a style="display: inline-block;" href="/Home/InformationReview">Person details</a>
        </fieldset>
        <%} %>
        <fieldset>
            <legend>Search</legend>
            <p>
                <p>
                    <% using (Html.BeginForm("SearchByPersonID", "Home"))
                       { %>
                    <fieldset>
                        <legend>By person ID</legend>
                        <%= Html.TextBox("PersonID", "", new { style = "width:100%" })%>
                        <%= Html.ValidationMessage("PersonID")%>
                        <input type="submit" value="Search" style="width: 100%;" />
                        <% } %>
                    </fieldset>
                </p>
            </p>
            <div style="display: inline-block; width: 100%;">
                <% using (Html.BeginForm("SearchByPhoto", "Home"))
                   { %>
                <fieldset>
                    <legend>By photo</legend>
                    <div style="display: inline-block; width: 50%;" align="center">
                        <fieldset>
                            <input type="file" name="Photo" id="Photo" style="width: 100%;" accept="image/*"
                                onchange="ChangeImage('Photo','#CurrentPhotoThumbnail')" />
                            <img alt="" id="CurrentPhotoThumbnail" width="100%" style="visibility: hidden;" /><br>
                        </fieldset>
                    </div>
                    <label>
                        Recognition algorithm:</label>
                    <select id="Algorithm" name="Algorithm" style="width: 100%;">
                        <option value="0">"Eigen Faces"</option>
                        <option value="1">"Fisher Faces"</option>
                    </select>
                    <input type="submit" value="Search" style="width: 100%;" />
                </fieldset>
                <div id="Div8" style="display: inline-block; width: 100%;" align="center">
                    <%= Html.ActionLink("Back", "Index", "Home")%></div>
                <div id="Div3" style="display: inline-block; width: 100%;" align="center">
                    <%= Html.ActionLink("Exit", "LogOff", "Account")%></div>
                <% } %>
        </fieldset>
    </div>
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
