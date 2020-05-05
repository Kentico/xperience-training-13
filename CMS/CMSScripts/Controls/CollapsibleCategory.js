function ToggleDiv(label) {
    var toggleImage = $cmsj(label).parent().find('.ToggleImage');

    $cmsj(toggleImage).toggleClass("icon-plus-square icon-minus-square")
    $cmsj(label).siblings(":last").toggle();
}