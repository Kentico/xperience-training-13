<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_Group_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Group list"
     Codebehind="Group_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/Groups/Controls/GroupList.ascx" TagName="GroupList"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">

    <script type="text/javascript">
    //<![CDATA[
        function editGroup(groupId) {
            location.replace('Group_Edit.aspx?groupId=' + groupId);   
        }
    //]]>
    </script>

    <cms:GroupList ID="groupListElem" runat="server" IsLiveSite="false" />
</asp:Content>
