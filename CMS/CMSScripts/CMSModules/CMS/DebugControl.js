cmsdefine(['jQuery'], function ($) {

	var Module = function (containerId) {
		var $container = $('#' + containerId),
		$win = $(window),

		setWidth = function () {
			$container.css("width", $win.width());
		},

		init = function () {
			$win.resize(setWidth);

			setWidth();
		};

		init();
	};

	return Module;
});
