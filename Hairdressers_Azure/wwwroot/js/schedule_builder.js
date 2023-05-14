function generateScheduleTable() {
    var header = $("<tr>");
    var th_1 = $("<th>").css("border", "2px solid white").text("Apertura");
    var th_2 = $("<th>").css("border", "2px solid white").text("Cierre");
    var th_3 = $("<th>").css("border", "2px solid white").text("Días").addClass("schedule_td_newDays");
    var th_4 = $("<th>").css("border", "2px solid white");

    th_1.css("width", "calc(100%/4)");
    th_2.css("width", "calc(100%/4)");
    th_3.css("width", "calc(100%/4)");
    th_4.css("width", "calc(100%/4)");

    header.append(th_1, th_2, th_3, th_4);

    var selecter = "<td class='schedule_td_newDays'>" +
        "<button type='button' class='days_button schedule_input'>Seleccione día(s)</button>" +
        "<div class='days_select' style='display: none; z-index: 1;'>" +
            "<label class='days_label' for='day_0'><input type='checkbox' id='day_0' value='L'><span style='margin-left: 5px;'>Lunes</span></label>" +
            "<label class='days_label' for='day_1'><input type='checkbox' id='day_1' value='M'><span style='margin-left: 5px;'>Martes</span></label>" +
            "<label class='days_label' for='day_2'><input type='checkbox' id='day_2' value='X'><span style='margin-left: 5px;'>Miércoles</span></label>" +
            "<label class='days_label' for='day_3'><input type='checkbox' id='day_3' value='J'><span style='margin-left: 5px;'>Jueves</span></label>" +
            "<label class='days_label' for='day_4'><input type='checkbox' id='day_4' value='V'><span style='margin-left: 5px;'>Viernes</span></label>" +
            "<label class='days_label' for='day_5'><input type='checkbox' id='day_5' value='S'><span style='margin-left: 5px;'>Sábado</span></label>" +
            "<label class='days_label' for='day_6'><input type='checkbox' id='day_6' value='D'><span style='margin-left: 5px;'>Domingo</span></label>" +
        "</div>" +
        "</td>";

    $("table.schedule_table tbody tr td input[name='newEnd']").parent().after(selecter);

    $('.days_button').on('click', function (e) {
        $(this).siblings('.days_select').slideToggle();
        $(this).toggleClass('active');
    });

    $(".days_label input[type='checkbox']").change(function () {
        var count = $(".days_label input[type='checkbox']:checked").length;
        var text = (count != 0) ? (count + " día(s) seleccionado(s)") : ("Seleccione día(s)");
        $(".days_button").text(text);
    });

    $(".schedule_table thead").html(header);
}

async function insertRow(actionsAllowed, apertura = "", cierre = "", daysText = "", schedule_id = 0, schedule_row_id = 0) {
    var type = $("select[name='schedule_types']").val();
    var counter = $('[id^="schedule_row_"]').length;

    if ((type !== "3" && counter < 2) || (type === "3" && counter < 14)) {
        while ($('#schedule_row_' + counter).length > 0) {
            counter++;
        }

        apertura = (apertura === "") ? $("input[name='newStart']").val() : apertura;
        cierre = (cierre === "") ? $("input[name='newEnd']").val() : cierre;

        if (daysText === "") {
            switch (type) {
                case "1":
                    daysText = "De Lunes a Domingo";
                    break;
                case "2":
                    daysText = "De Lunes a Viernes";
                    break;
                case "3":
                    let checkedValues = "";
                    $("input[type='checkbox']:checked").each(function () {
                        checkedValues += $(this).val() + " ";
                    });
                    daysText = checkedValues.trim();
                    break;
            }
        }

        var insertConfirmed = false;
        if (daysText.length > 0) {
            if (actionsAllowed) {
                schedule_row_id = await insertBBDDRow(apertura, cierre, daysText, schedule_id);
                insertConfirmed = (schedule_row_id !== 0);
            }

            if (validateHour(apertura) && validateHour(cierre) && insertConfirmed === actionsAllowed) {
                var newRow = $("<tr>").attr('id', 'schedule_row_' + counter);
                var newApertura = $("<td>").text(apertura.substring(0,5)).css({ "color": "white", "text-align": "center" });
                var newCierre = $("<td>").text(cierre.substring(0, 5)).css({ "color": "white", "text-align": "center" });
                var newDelete = $("<button>").addClass("schedule_input schedule_btn_remove").text("Eliminar");
                    newDelete.attr("type", "button");
                    newDelete.click(function () {
                        var id_row = "#schedule_row_" + counter;
                        deleteRow(true, id_row, schedule_row_id);
                    });
                var newAction = $("<td>").append(newDelete);

                var newDays = $("<td>");
                    newDays.text(daysText);

                newDays.addClass("schedule_td_newDays").css({ "color": "white", "text-align": "center" });
                if (type !== "3") {
                    newDays.hide();
                }

                newRow.append(newApertura, newCierre, newDays, newAction);
                $("table.schedule_table tbody").append(newRow);

                if ((type !== "3" && counter == 1) || (type === "3" && counter == 13)) {
                    $("table.schedule_table tbody tr td .schedule_btn_add").prop('disabled', true);
                    $("table.schedule_table tbody tr td .schedule_btn_add").toggleClass("disabled", true);
                }
            }
        }

    }
}

function deleteRow(actionsAllowed, id_row, schedule_row_id) {
    if (actionsAllowed) { // Si las acciones están permitidas, eliminamos
        deleteBBDDRow(schedule_row_id);
    }
    $(id_row).remove(); // Eliminamos la fila
    checkButtonAdd(); // Comprobamos el botón de añadir fila
}

function storeCurrentType() {
    $(".schedule_types").each(function () {
        $(this).data('previous', $(this).val());
    });
}

function checkButtonAdd() {
    var type = $("select[name='schedule_types']").val();
    var counter = $('[id^="schedule_row_"]').length;
    var schedule_btn_add = $("table.schedule_table tbody tr td .schedule_btn_add");

    if ((type !== "3" && counter < 2) || (type === "3" && counter < 14)) {
        schedule_btn_add.prop('disabled', false);
        schedule_btn_add.toggleClass("disabled", false);
    } else {
        schedule_btn_add.prop('disabled', true);
        schedule_btn_add.toggleClass("disabled", true);
    }

    if (type !== "3" && counter >= 2) {
        while (counter > 2) {
            $('.schedule_table tbody tr:last-child').remove();
            counter--;
        }
    }
}

function checkScheduleName(schedule_name, schedules) {
    var exist = false;
    var activated = false;
    var schedule_id = 0;
    var hairdresser_id = (schedules[0].HairdresserId != null)? schedules[0].HairdresserId : 0;

    for (var i = 0; i < schedules.length; i++) {
        if (schedules[i].Name === schedule_name) {
            exist = true;
            activated = schedules[i].Active;
            schedule_id = schedules[i].ScheduleId;
        }
    }

    var buttonAddRows = $('button.schedule_input.schedule_btn_add:first');
    var buttonAddSchedule = $('.schedule_buttons .schedule_btn_add');
    var buttonRemoveSchedule = $('.schedule_buttons .schedule_btn_remove');
    var buttonActivate = $('.schedule_buttons .schedule_btn_activate');

    buttonAddRows.prop('disabled', !exist);
    buttonAddRows.toggleClass("disabled", !exist);
    buttonAddRows.off('click').click(function () {
        insertRow(true, '', '', '', schedule_id, 0);
    });

    buttonAddSchedule.prop('disabled', (schedule_name.length == 0 || (exist && schedule_name.length > 0)));
    buttonAddSchedule.toggleClass("disabled", (schedule_name.length == 0 || (exist && schedule_name.length > 0)));
    buttonAddSchedule.off('click').click(function () {
        createSchedule(hairdresser_id, $(".schedule_name").val());
    });

    buttonRemoveSchedule.prop('disabled', !exist);
    buttonRemoveSchedule.toggleClass("disabled", !exist);
    buttonRemoveSchedule.off('click').click(function () {
        deleteSchedule(schedule_id);
    });

    buttonActivate.prop('disabled', (!exist || (exist && activated)));
    buttonActivate.toggleClass("disabled", (!exist || (exist && activated)));
    buttonActivate.off('click').click(function () {
        activateSchedule(hairdresser_id, schedule_id);
    });
}

function validateHour(hora) {
    var pattern = /^([01]?[0-9]|2[0-3]):[0-5][0-9](:[0-5][0-9])?$/;
    return pattern.test(hora);
}

function toggleTdDays() {
    var typeValue = $(".schedule_types").val();
    if (typeValue === "3") {
        $(".schedule_td_newDays").show();
    } else {
        $(".schedule_td_newDays").hide();
    }
}