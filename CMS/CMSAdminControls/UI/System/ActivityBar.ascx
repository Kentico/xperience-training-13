<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_System_ActivityBar"  Codebehind="ActivityBar.ascx.cs" %>
<div class="ActivityBar" style="overflow: hidden;" id="activityBar">
    &nbsp;        
</div>

<script type="text/javascript">
//<![CDATA[
    var activityElem = document.getElementById('activityBar');
    var pos = 0;

    function Activity()
    {
        pos++;
        activityElem.style.backgroundPosition = pos + 'px 0px';
    }
    
    function ShowActivity()
    {
        activityElem.display = '';
    }
    
    function HideActivity()
    {
        activityElem.display = 'none';
    }
//]]>
</script>