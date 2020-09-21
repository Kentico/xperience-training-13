// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
    var sidenavElements = document.querySelectorAll(".sidenav");
    M.Sidenav.init(sidenavElements);
    var dropdownElements = document.querySelectorAll(".dropdown-trigger");

    M.Dropdown.init(dropdownElements, {
        hover: false
    });

    var selectElements = document.querySelectorAll("select");
    M.FormSelect.init(selectElements);
    var datepickerElements = document.querySelectorAll("datepicker");
    M.Datepicker.init(datepickerElements);
});