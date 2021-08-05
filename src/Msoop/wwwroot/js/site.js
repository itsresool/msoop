// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(document).ready(() => {
    $("#edit-sheet-form select").change((event) => {
        const shouldHideCustomInput = $(event.target).val() !== $("#edit-sheet-form select > option").last().val();
        $("#custom-age-input").attr("hidden", shouldHideCustomInput);
    });

});
