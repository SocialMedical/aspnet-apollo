$(function () {
    var evolutionDate = new Date(@Model.EvolutionDate.Year,@(Model.EvolutionDate.Month - 1),@Model.EvolutionDate.Day);
    var currentDate = new Date(@currentDate.Year,@(currentDate.Month - 1),@currentDate.Day);
    $("#medicalCareDateInput").datepicker({ dateFormat: SIC_DATE_DISPLAY_FORMAT, maxDate: currentDate,
        defaultDate: evolutionDate});

    $("#medicalCareDateInput").datepicker("setDate", evolutionDate)

    $("#medicalCareDateInput").change(function()
    {
        $("#medicalCareDateLabel").text($("#medicalCareDateInput").val());
    });

    $("#medicalCareDateLabel").click(function()
    {
        $("#medicalCareDateInput").focus();
    });

    $(".vademecumSearch").autocomplete({ source: '@Url.Action("VademecumsAutocomplete", "Medical")',
        minLength: 3,
        open: function () {
            $(this).autocomplete('widget').css('z-index', 100000000);
            return false;
        },
        change: function (event, ui) {
            if ($(this).val() == '') {
                $(this).parent('[medicineitem]').attr('pvid', '0');
                $(this).parent('[medicineitem]').attr('gid', '0');
            }
        },
        search: function (event, ui) {
            if ($(this).val() != '') {
                $(this).parent('[medicineitem]').attr('pvid', '0');
                $(this).parent('[medicineitem]').attr('gid', '0');
            }
        },
        select: function (event, ui) {
            if (ui.item.label.substring(0, 1) == '*')
                $(this).val(ui.item.label.substring(1));
            else
                $(this).val(ui.item.label);

            var pos = $(this).parent('[medicineitem]').find("[name='posology']")
            var quantity = $(this).parent('[medicineitem]').find("[name='quantity']")
            $(pos).val(ui.item.pos);

            var pvid = ui.item.id;
            var gid = ui.item.gid;
            if (pvid === undefined)
                pvid = '0';
            if (gid === undefined)
                gid = '0';
            $(this).parent('[medicineitem]').attr('pvid', pvid);
            $(this).parent('[medicineitem]').attr('gid', gid);

            $(quantity).val(1);
            $(pos).focus();
            $(pos).select();
            return false;
        }
    });
});