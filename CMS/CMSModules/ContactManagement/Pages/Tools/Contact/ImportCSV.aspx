<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportCSV.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default"
    Title="Contact list" Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_ImportCSV" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div data-ng-view="view"></div>
    
     <script type="text/ng-template" id="importProcessTemplate.html">
        <div data-cms-import-process-directive></div>
     </script>
    
    <script type="text/ng-template" id="importProcessDirectiveTemplate.html">
        <div class="UIContent scroll-area om-import-csv-process-container">
            <div class="PageContent">
                <div data-cms-messages-placeholder></div>
                <h4 data-ng-show="!finished">{{"om.contact.importcsv.importingstepmessage"|resolve}}</h4>
                <h4 data-ng-show="finished">{{"om.contact.importcsv.finishedstepmessage"|resolve}}</h4>

                <p class="lead">{{"om.contact.importcsv.importing"|resolve|stringFormat:result.processed}}</p>
                    
                <ul class="om-import-csv-list">
                    <li>{{"om.contact.importcsv.importedcount"|resolve|stringFormat:result.imported}}</li>
                    <li>{{"om.contact.importcsv.duplicatescount"|resolve|stringFormat:result.duplicities}}</li>
                    <li>{{"om.contact.importcsv.notimported"|resolve|stringFormat:result.failures}} 
                        <div data-cms-download-data-directive                      
                            data-to-download="result.csvData" 
                            data-file-name="not-imported-contacts.csv" 
                            data-content-type="application/csv"
                            data-ng-show="result.csvData">
                        </div>
                    </li>
                </ul>
                <div data-ng-show="finished">
                    <cms-smart-tip-placeholder smart-tip="continueToSmartTip"> </cms-smart-tip-placeholder>
                </div>
            </div>
        </div>
    </script>

    <script type="text/ng-template" id="fileUploadTemplate.html">
        <div data-cms-file-upload-directive>
        </div>   
    </script>

    <script type="text/ng-template" id="fileUploadDirectiveTemplate.html">
        <div class="UIHeader">
            <div class="header-container">
                <div class="header-actions-container">
                    <div class="header-actions-main">
                        <button data-ng-click="onClick()" type="button" class="btn btn-primary">{{"om.contact.importcsv.selectfilebuttontext"|resolve}}</button>
                    </div>
                </div>
            </div>
        </div>
        <input type='file' class='js-file-input hidden' accept='.csv,.txt'></input>
        <div class="UIContent scroll-area om-import-csv-content-container">
            <div class="PageContent">    
                <div data-cms-messages-placeholder></div> 
                <cms-smart-tip-placeholder smart-tip="howToSmartTip"> </cms-smart-tip-placeholder>
            </div>
        </div>   
    </script>

    <script type="text/ng-template" id="attributeMappingTemplate.html">
        <div data-cms-attribute-mapping-directive></div>
    </script>

    <script type="text/ng-template" id="attributeMappingDirectiveTemplate.html">
        <div class="UIHeader">
            <div class="header-container">
                <div class="header-actions-container">
                    <div class="header-actions-main">
                        <button type="button" data-ng-click="onMappingFinished()" class="btn btn-primary">{{"om.contact.importcsv.importcontactsbuttontext"|resolve}}</button>
                    </div>
                </div>
            </div>
        </div>
        <div class="UIContent scroll-area om-import-csv-content-container">
            <div class="PageContent">
                <div data-cms-messages-placeholder></div>
        
                <h4>{{"om.contact.importcsv.segmentation.title"|resolve}}</h4>
                <p>{{"om.contact.importcsv.segmentation.message"|resolve}}</p>
        
                <div class="form-horizontal">            
                    <div class="form-group">
                        <div class="editing-form-value-cell">
                            <div class="radio-list-vertical">       
                                <span class="radio">
                                    <input id="radNew" type="radio" name="segmentation" value="new" data-ng-model="segmentationType">
                                    <label for="radNew">{{"om.contact.importcsv.segmentation.createnew"|resolve}}</label>
                                </span>
                                <div class="selector-subitem" data-ng-show="segmentationType === 'new'">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <label class="control-label" for="cgSelect">{{"om.contact.importcsv.segmentation.contactgroupname"|resolve}}:</label>
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <input id="txtNewContactGroup" type="text" class="form-control" data-ng-model="contactGroup.name" />
                                        </div>
                                    </div>
                                </div>
                                <span class="radio">
                                    <input id="radExisting" type="radio" name="segmentation" value="existing" data-ng-model="segmentationType">
                                    <label for="radExisting">{{"om.contact.importcsv.segmentation.existing"|resolve}}</label>
                                </span>
                                <div class="selector-subitem" data-ng-show="segmentationType === 'existing'">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <label class="control-label" for="cgSelect">{{"om.contact.importcsv.segmentation.selectcontactgroup"|resolve}}:</label>
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <select id="cgSelect" class="DropDownField form-control" 
                                                        data-ng-model="contactGroup.guid" 
                                                        data-ng-options="cg.contactGroupGUID as cg.contactGroupDisplayName for cg in contactGroups">
                                            </select>
                                        </div>
                                    </div>
                                </div>
                                <span class="radio">
                                    <input id="radNone" type="radio" name="segmentation" value="none" data-ng-model="segmentationType">
                                    <label for="radNone">{{"om.contact.importcsv.segmentation.none"|resolve}}</label>
                                </span>
                            </div>
                        </div>
                     </div>
                </div>        
        
                <h4>{{"om.contact.importcsv.mapstepmessage"|resolve}}</h4>
                <%-- Message text contains HTML so it can't be escaped --%>
                <p data-ng-bind-html="resolveFilter('om.contact.importcsv.messagetext')"></p>
                
            </div>    
            <div class="om-import-csv-mapping-tables-container">
                <div data-ng-repeat="csvColumn in csvColumnNamesWithExamples" class="om-import-csv-mapping-table col-xs-12 col-md-6 col-lg-4">
                    <div data-contact-fields-mapping="processingContactFieldsMapping" data-csv-column="csvColumn" data-cms-attribute-mapping-control-directive></div>
                </div>
            </div>
        </div>
    </script>

    <script type="text/ng-template" id="attributeMappingControlDirectiveTemplate.html">
        <table class="table table-hover _nodivs">
            <thead>
                <tr class="unigrid-head">
                    <th scope="col">{{csvColumn.ColumnName.trim() || "&nbsp;"}}</th>
                </tr>
            </thead>
            <tbody>
                <tr data-ng-repeat="example in csvColumn.ColumnExamples track by $index">
                    <td>{{example.trim() || "&nbsp;"}}</td>
                </tr>
            </tbody>
        </table>
        
        <div class="form-horizontal">            
            <div class="form-group">
                <div class="control-group-inline">         
                    <div class="editing-form-label-cell">
                        <label for="id_{{csvColumn.ColumnName}}" class="control-label">{{"om.contact.importcsv.belongsto"|resolve}}:</label>
                    </div>            
                    <div class="editing-form-value-cell">
                        <select id="id_{{csvColumn.ColumnName}}" data-ng-model="selectedField" data-ng-change="contactFieldUpdate()" class="DropDownField form-control" >
                            <option value="">{{"om.contact.importcsv.donotimport"|resolve}}</option>
                            <optgroup data-ng-repeat="category in contactFieldsMapping" label="{{category.categoryName}}" data-ng-if="category.categoryMembers">
                                <option data-ng-repeat="field in category.categoryMembers" value="{{field.name}}" data-ng-disabled="field.mappedIndex !== -1 && field.mappedIndex !== csvColumn.ColumnIndex">
                                    {{field.displayName}}
                                </option>
                            </optgroup>
                        </select>
                    </div>
                </div>
            </div>  
        </div>           
    </script>

    <script type="text/ng-template" id="downloadDataDirectiveTemplate.html">
        <span>
            <a data-ng-click="offerCSVToDownload()">{{"om.contact.importcsv.notimported.link"|resolve}}</a>
            <span class="info-icon">
                <asp:Label runat="server" ID="labelDownloadErrorCSV" CssClass="sr-only" />
                <cms:CMSIcon runat="server" id="iconDownloadErrorCSV" enableviewstate="false" class="icon-question-circle" aria-hidden="true" />
            </span>
        </span>
    </script>

    <script type="text/ng-template" id="requirementsCheckerTemplate.html">
        <div class="UIContent scroll-area om-import-csv-requirements-checker-container">
            <div class="PageContent">
                <div data-cms-messages-placeholder></div>
            </div>
        </div>
    </script>
</asp:Content>