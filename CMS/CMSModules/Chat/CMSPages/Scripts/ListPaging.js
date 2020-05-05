ListPaging = function(obj) {

    var GroupID = obj.groupID,
        ClientID = obj.clientID,
        WPType = obj.wpType,
        FunctionRenderList = obj.functionRenderList,
        FilterPanel = $cmsj("#" + obj.filterPanelID),
        InfoFilterPanel = $cmsj("#" + obj.filterPanelInfoID),
        FilterTextbox = $cmsj("#" + obj.filterTextboxID),
        FilterButton = $cmsj("#" + obj.filterButtonID),
        FilterNotFoundStr = obj.filterNotFoundStr,
        FilterFoundStr = obj.filterFoundStr,
        FilterEnabled = obj.filterEnabled,
        FilterShowItems = (obj.filterShowItems >= 0) ? obj.filterShowItems : ChatManager.Settings.WPFilterLimit,
        FilterProperty = obj.filterProperty,
        PagingEnabled = obj.pagingEnabled,
        PagingItems = (obj.pagingItems > 0) ? obj.pagingItems : ChatManager.Settings.WPPagingItems,
        PagingGroupPagesBy = (obj.groupPagesBy >= 0) ? obj.groupPagesBy : ChatManager.Settings.WPGroupPagesBy,
        PagingGroupingEnabled = PagingGroupPagesBy > 0,
        PagingContent = $cmsj("#" + obj.pagingContentID),
        
        lastFilter = "",
        DisplayedSubList = null,
        ListOriginal = null,
        DisplayedPage = 1,
        DisplayedGroup = 0,
        List = null,
        FilteredList = null,
        that = this;

    if (FilterEnabled == true) {
        SetFilterKeydownEvent();
        FilterButton.click(function() {
            that.Filter(FilterTextbox.val());
            return false;
        });
    }
    

    this.NextGroup = function () {
        that.SetGroup(DisplayedGroup + 1);
    };


    this.PrevGroup = function () {
        that.SetGroup(DisplayedGroup - 1);
    };
    

    this.SetGroup = function (g) {
        var maxPage = Math.ceil(List.length / PagingItems);
        var maxGroup = Math.ceil(maxPage / PagingGroupPagesBy) - 1;
        if (g < 0) {
            g = maxGroup;
        }
        else if (g > maxGroup) {
            g = 0;
        }
        DisplayedGroup = g;
        DisplayedPage = DisplayedGroup * PagingGroupPagesBy + 1;
        RenderInternal();
    };


    this.SetPage = function(p) {
        var maxPage = Math.ceil(List.length / PagingItems);
        if (p < 1) {
            p = maxPage;
        }
        else if (p > maxPage) {
            p = 1;
        }
        DisplayedPage = p;
        if (PagingGroupingEnabled) {
            DisplayedGroup = Math.ceil(DisplayedPage / PagingGroupPagesBy) - 1;
        }
        RenderInternal();
    };


    this.NextPage = function() {
        that.SetPage(DisplayedPage + 1);
    };


    this.PrevPage = function() {
        that.SetPage(DisplayedPage - 1);
    };


    this.GetList = function() {
        return ListOriginal;
    };


    this.Render = function(lst) {
        ListOriginal = List = lst;
        if (FilterEnabled == true) {
            FilterPanel.show();
        }
        if (FilteredList != null) {
            that.Filter(lastFilter);
            return;
        }
        RenderInternal();
    };


    this.Filter = function(filter) {
        if ((filter == null) || (filter.length == 0)) {
            FilteredList = null;
            InfoFilterPanel.empty();
            lastFilter = "";
        }
        else {
            FilteredList = new Array();
            lastFilter = filter;
            for (var i = 0; i < ListOriginal.length; i++) {
                if (ListOriginal[i][FilterProperty].toLowerCase().indexOf(filter.toLowerCase()) >= 0) {
                    FilteredList.push(ListOriginal[i]);
                }
            }
            if (FilteredList.length == 0) {
                InfoFilterPanel.text(FilterNotFoundStr + " " + lastFilter);
            }
            else {
                InfoFilterPanel.text(FilteredList.length + " " + FilterFoundStr + " " + lastFilter);
            }
        }

        List = (FilteredList == null) ? ListOriginal : FilteredList;
        DisplayedPage = 1;
        DisplayedGroup = 0;
        RenderInternal();
    };


    this.Clear = function() {
        DisplayedPage = 1;
        DisplayedGroup = 0;
        PagingContent.empty();
        InfoFilterPanel.empty();
        lastFilter = "";

        if (FilterEnabled == true) {
            FilterPanel.hide();
            FilterTextbox.val("");
        }

        FilteredList = null;
    };


    function SetFilterKeydownEvent() {
        if (FilterTextbox.length > 0) {
            FilterTextbox.keydown(function(evt) {
                var e = window.event || evt;
                var key = e.keyCode;

                if (key == 13) {
                    if (e.preventDefault) e.preventDefault();
                    if (e.stopPropagation) e.stopPropagation();
                    e.returnValue = false;
                    that.Filter(FilterTextbox.val());
                }
            });
        }
    }


    function RenderInternal() {
        if (FilterEnabled == true) {
            if (FilteredList == null && (!ListOriginal || (ListOriginal.length < FilterShowItems))) {
                FilterPanel.hide();
            }
            else {
                FilterPanel.show();
            }
        }
        
        if (PagingEnabled) {
            DoPaging();
            RenderPaging();
            FunctionRenderList(DisplayedSubList);
        }
        else {
            FunctionRenderList(List);
        }
    }


    function RenderPaging() {
        PagingContent.empty();
        var maxPage = Math.ceil(List.length / PagingItems);
        if (maxPage <= 1) {
            return;
        }
        var groupPages = PagingGroupingEnabled && (maxPage > PagingGroupPagesBy),
            linkStart = '<a href="#"; onclick="var obj = $cmsj.ChatManager.GetWebpart(' + ((GroupID != null) ? ('\'' + GroupID + '\'') : 'null') + ', \'' + ClientID + '\', \'' + WPType + '\');',
            startPage = 1,
            endPage = maxPage,
            html;

        html = '<div class="ChatWebPartPaging"><ul>';
        var firstPageDisplayed = DisplayedPage == 1;
        html += firstPageDisplayed ? '<li class="InactivePagingElem">|&lt;</li>' : ('<li>' + linkStart + ' obj.ListPaging.SetPage(1); return false;">|&lt;</a></li>');
        html += firstPageDisplayed ? '<li class="InactivePagingElem">&lt;</li>' : ('<li>' + linkStart + ' obj.ListPaging.PrevPage(); return false;">&lt;</a></li>');
        
        if (groupPages) {
            html += (DisplayedGroup == 0) ? '<li class="InactivePagingElem">...</li>' : ('<li>' + linkStart + ' obj.ListPaging.PrevGroup(); return false;">...</a></li> ');
            startPage = DisplayedGroup * PagingGroupPagesBy;
            endPage = startPage + PagingGroupPagesBy;
            startPage++;
            if (endPage > maxPage) {
                endPage = maxPage;
            }
        }
        for (var i = startPage; i <= endPage; i++) {
            if (i == DisplayedPage) {
                html += '<li class="ActivePage">' + (i) + '</li>';
            }
            else {
                html += '<li>' + linkStart + ' obj.ListPaging.SetPage(' + i + '); return false;">' + (i) + '</a></li>';
            }
        }
        if (groupPages) {
            var maxGroup = Math.ceil(maxPage / PagingGroupPagesBy) - 1;
            html += (DisplayedGroup == maxGroup) ? '<li class="InactivePagingElem">...</li>' : ('<li>' + linkStart + ' obj.ListPaging.NextGroup(); return false;">...</a></li> ');
        }
        var lastPageDisplayed = DisplayedPage == maxPage;
        html += lastPageDisplayed ? '<li class="InactivePagingElem">&gt;</li>' : ('<li>' + linkStart + ' obj.ListPaging.NextPage(); return false;">&gt;</a></li>');
        html += lastPageDisplayed ? '<li class="InactivePagingElem">&gt;|</li>' : ('<li>' + linkStart + ' obj.ListPaging.SetPage(' + maxPage + '); return false;">&gt;|</a></li>');
        html += '</ul></div>';
        PagingContent.html(html);
    };


    function DoPaging() {
        if (List.length == 0) {
            DisplayedSubList = [];
            DisplayedPage = 1;
            DisplayedGroup = 0;
            return;
        }
        var maxPage = Math.ceil(List.length / PagingItems);
        if (DisplayedPage > maxPage) {
            DisplayedPage = maxPage;
        }
        var start = (DisplayedPage - 1) * PagingItems;
        var end = start + PagingItems;
        if (end > List.length) {
            end = List.length;
        }
        DisplayedSubList = List.slice(start, end);
    };
}