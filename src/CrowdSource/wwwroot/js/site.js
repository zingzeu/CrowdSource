// Write your Javascript code.

$(document).ready(function () {

    if ($('#IsPivotRow').is(":checked")) {
        $("#PivotRowSpecificFields").show();
    } else {
        $("#PivotRowSpecificFields").hide();
    }

    $('#IsPivotRow').click(function () {
        if ($(this).is(':checked')) {
            $("#PivotRowSpecificFields").show();
        } else {
            $("#PivotRowSpecificFields").hide();
        }
    }); 
});

