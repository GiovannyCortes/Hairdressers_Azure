function chargeScheduleData(jsonRows = "") {
    // Elimino los antiguos registros
    $('.schedule_table tbody tr').not(':first-child').remove();
    jsonRows.forEach(function (row) {
        insertRow(false, row.apertura, row.cierre, row.daysText, row.schedule_id, row.schedule_row_id);
    });
}

function insertBBDDRow(apertura, cierre, daysText, schedule_id) {
    return new Promise(function (resolve, reject) {
        $.post("/Hairdresser/AddScheduleRow", { apertura: apertura, cierre: cierre, daysText: daysText, schedule_id: schedule_id })
            .done(function (data) {
                resolve(data);
            })
            .fail(function (error) {
                reject(error);
            });
    });
}

function deleteBBDDRow(schedule_row_id) {
    $.post("/Hairdresser/DeleteScheduleRow", { scheduleRow_id: schedule_row_id });
}

function activateSchedule(hairdresser_id, schedule_id) {
    $.post("/Hairdresser/ActivateSchedule", { hairdresser_id: hairdresser_id, schedule_id: schedule_id }, function () {
        location.reload();
    });
}

function createSchedule(hairdresser_id, name) {
    new swal({
        html: '<h2>¿Crear horario?</h2>' +
              '<p>Esta acción no se puede revertir</p>',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, crear',
        confirmButtonColor: '#3085d6',
        cancelButtonText: 'Cancelar',
    }).then((result) => {
        if (result.isConfirmed) {
            $.post("/Hairdresser/CreateSchedule", { hairdresser_id: hairdresser_id, name: name }, function () {
                location.reload();
            });
        }
    });
}

function deleteSchedule(schedule_id) {
    new swal({
        html: '<h2>¿Eliminar horario?</h2>' +
              '<p>Esta acción no se puede revertir</p>',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        confirmButtonColor: '#9C4851',
        cancelButtonText: 'Cancelar',
    }).then((result) => {
        if (result.isConfirmed) {
            $.post("/Hairdresser/DeleteSchedule", { schedule_id: schedule_id }, function (data) {
                if (data === 0) {
                    location.reload();
                } else {
                    new swal({
                        title: "Eliminación cancelada",
                        text: "No pueden existir peluquerías sin horarios",
                        icon: 'error',
                        showCancelButton: false,
                        confirmButtonText: 'Entendido',
                        confirmButtonColor: '#3085d6',
                    });
                }
            });
        }
    });
}