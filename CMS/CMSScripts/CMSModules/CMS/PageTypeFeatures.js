cmsdefine([], function () {

	var Module = function (data) {

		var highlightSelection = data.highlightSelection;

		registerOnClickHandlers = function () {
			var features = document.querySelectorAll("div[data-feature]");
			features.forEach(function (element) {
				element.addEventListener("click", this.toggleSelection.bind(null, element));
			});
		};

        toggleSelection = function (element) {
            var checkbox = element.querySelector("input[type='checkbox']");
            if (checkbox.disabled) {
                return;
            }

			checkbox.checked = !checkbox.checked;

			ensureCssClass(element, checkbox);

			handleDependencies(element.dataset.feature, checkbox.checked);
		};

		handleDependencies = function (feature, checked) {
			// Page builder feature is dependent on URL feature. When Page builder feature is selected, URL feature must be selected too.
			if (feature === "pagebuilder" && checked) {
				updateDependentSelection("url", true);
			}

			// When URL feature is unselected, Page builder feature must be unselected too.
			if (feature === "url" && !checked) {
				updateDependentSelection("pagebuilder", false);
			}
		};

		updateDependentSelection = function (feature, selected) {
			var element = document.querySelector("div[data-feature='" + feature + "']");

			var checkbox = element.querySelector("input[type='checkbox']");
			checkbox.checked = selected;
			ensureCssClass(element, checkbox);
		};

		ensureCssClass = function (element, checkbox) {
			if (highlightSelection) {
				if (checkbox.checked) {
					element.classList.add("selected");
				} else {
					element.classList.remove("selected");
				}
			}
		};

		registerOnClickHandlers();
	};

	return Module;
});