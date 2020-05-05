var selectedObjectsID = new Array();
var selectedObjects = new Array();

function CMSTreeView_ItemSelect(obj, treeId)
{
	if ((obj != null) && (treeId != ''))
	{
		if (selectedObjects[treeId] == null)
		{
		    selectedObjects[treeId] = document.getElementById(treeId + '_CMSselectedNode');
		}

		    var styleObj = document.getElementById(treeId + '_originalStyle');
		    var classObj = document.getElementById(treeId + '_originalClass');
	    var selectedStyleObj = document.getElementById(treeId + '_selectedStyle');
	    var selectedClassObj = document.getElementById(treeId + '_selectedClass');
	    if ((selectedObjects[treeId] != null)&&(styleObj != null))
		    {
		    	selectedObjects[treeId].removeAttribute("style");
                selectedObjects[treeId].setAttribute("style", styleObj.value);
                selectedObjects[treeId].style.cssText = styleObj.value;
		    }
			
		    if ((selectedObjects[treeId] != null)&&(classObj != null))
		    {
		    	selectedObjects[treeId].className = classObj.value;
	    	}
			
			if (obj.attributes['style'] == null)
			{
			    obj.style.color = '#ffffff';
			}
			
	        obj.removeAttribute("style");
            obj.setAttribute("style", selectedStyleObj.value);
            obj.style.cssText = selectedStyleObj.value;
	        obj.className = selectedClassObj.value;
		
		    selectedObjects[treeId] = obj;
	    }
}