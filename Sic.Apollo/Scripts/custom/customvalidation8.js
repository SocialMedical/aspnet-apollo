(function ($) {
    jQuery.validator.addMethod("comparevalues", function (value, element, params) {
        alert("Error");
        return false;
    });
    jQuery.validator.unobtrusive.adapters.addSingleVal("comparevalues", "otherproperty");
    alert("nada7");
} (jQuery));