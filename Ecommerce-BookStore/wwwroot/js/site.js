// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function () {
    const sidebarToggleBtn = document.querySelector('.nav-link[data-lte-toggle="sidebar"]');
    const body = document.body;

    sidebarToggleBtn.addEventListener('click', function (e) {
        e.preventDefault();
        // Toggle the class 'sidebar-open' on the body element
        body.classList.toggle('sidebar-open');
    });
});