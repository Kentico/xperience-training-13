<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="List.ascx.cs" Inherits="CMSModules_Scoring_Controls_UI_Rule_List" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<script type="text/javascript">
    function RefreshPage() {
        __doPostBack('', '');
    }

    (function () {
        var RequestStatus, RequestStatusUntilReady;
        RequestStatus = function (callback) {
            $cmsj.ajax({
                type: "POST",
                url: window.applicationUrl + "/CMSModules/Scoring/Pages/List.aspx/GetScoreStatus",
                data: "{scoreID:" + "<%= ScoreId %>" + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    callback(msg);
                }
            });
        }

        RequestStatusUntilReady = function() {
            RequestStatus(function(msg) {
                if (msg.d !== "Recalculating") {
                    RefreshPage();
                } else {
                    setTimeout(function() { RequestStatusUntilReady(); }, 2000);
                }
            });
        };

        return function() {
            RequestStatus(function(msg) {
                if (msg.d === "Recalculating") {
                    RequestStatusUntilReady();
                }
            });
        };
    }())();
</script>
<cms:UniGrid runat="server" ID="gridElem" ObjectType="om.rule" Columns="RuleID,RuleDisplayName,RuleValue,RuleValidUntil,RuleValidity,RuleValidFor,RuleIsRecurring,RuleType"
    IsLiveSite="false">
    <GridActions Parameters="RuleID">
        <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="#delete" ExternalSourceName="delete" Caption="$General.Delete$"
            FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
    </GridActions>
    <GridColumns>
        <ug:Column ID="colRuleDisplayName" runat="server" Source="RuleDisplayName" Localize="true" Caption="$om.rule.displayname$" Wrap="false">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column ID="colRuleValue" runat="server" Source="RuleValue" Caption="$om.score.value$" Wrap="false" >
            <Filter Type="integer" />
        </ug:Column>
        <ug:Column ID="colRuleValidity" runat="server" Source="##ALL##" Caption="$om.score.validity$" Wrap="false" ExternalSourceName="Validity" />
        <ug:Column ID="colRuleIsRecurring" runat="server" Source="RuleIsRecurring" Caption="$om.score.isrecurring$" Wrap="false"
            ExternalSourceName="#yesno">
            <Filter Type="bool" />
        </ug:Column>
        <ug:Column ID="colRuleType" runat="server" Source="RuleType" Caption="$om.score.ruletype$" Wrap="false" ExternalSourceName="RuleType" />
        <ug:Column CssClass="filling-column" />
    </GridColumns>
    <GridOptions DisplayFilter="true" />
</cms:UniGrid>