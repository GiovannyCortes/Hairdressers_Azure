namespace Hairdressers_Azure.Helpers {
    public static class HelperCalendar {
        /*
        public static string GetBussinesHours(List<Schedule_Row> schedulesRows) {
            // Se almacenará en el siguiente string, el JSON con las horas de negocio
            string bussinesHoursList = "[";

            // Crear una lista para almacenar temporalmente los días de la semana en los que se aplica cada rango
            List<int> daysOfWeek = new List<int>();

            // Crear variables para almacenar temporalmente el inicio y el final del rango actual
            TimeSpan currentRangeStart = TimeSpan.Zero;
            TimeSpan currentRangeEnd = TimeSpan.Zero;

            // Ordenar la lista de Schedules_Rows por el inicio del rango
            schedulesRows.Sort((a, b) => a.Start.CompareTo(b.Start));

            // Iterar por cada objeto Schedules_Rows en la lista
            foreach (Schedule_Row scheduleRow in schedulesRows) {
                // Si el rango actual no ha sido inicializado, inicializarlo con los valores del objeto actual
                if (currentRangeStart == TimeSpan.Zero && currentRangeEnd == TimeSpan.Zero) {
                    currentRangeStart = scheduleRow.Start;
                    currentRangeEnd = scheduleRow.End;
                    AddDayOfWeekToList(daysOfWeek, scheduleRow);
                } else {
                    // Si el inicio del rango actual es igual al inicio del objeto actual, actualizar el final del rango actual
                    if (currentRangeStart == scheduleRow.Start) {
                        currentRangeEnd = scheduleRow.End;
                        AddDayOfWeekToList(daysOfWeek, scheduleRow);
                    } else {
                        // Si el inicio del rango actual es diferente al inicio del objeto actual, añadimos un nuevo objeto
                        bussinesHoursList += @"{
                                                    daysOfWeek = " + daysOfWeek.ToString() + "," +
                                                    "startTime = " + currentRangeStart.ToString(@"hh\:mm") + "," +
                                                    "endTime = " + currentRangeEnd.ToString(@"hh\:mm") +
                                               "}";

                        // Inicializar el rango actual con los valores del objeto actual
                        currentRangeStart = scheduleRow.Start;
                        currentRangeEnd = scheduleRow.End;

                        // Limpiar la lista de días de la semana y agregar los días del objeto actual
                        daysOfWeek.Clear();
                        AddDayOfWeekToList(daysOfWeek, scheduleRow);
                    }
                }
            }

            // Si todavía hay un rango actual sin procesar, agregarlo a la lista
            if (currentRangeStart != TimeSpan.Zero && currentRangeEnd != TimeSpan.Zero) {
                bussinesHoursList += @"{
                                            daysOfWeek = " + daysOfWeek.ToString() + "," +
                                            "startTime = " + currentRangeStart.ToString(@"hh\:mm") + "," +
                                            "endTime = " + currentRangeEnd.ToString(@"hh\:mm") +
                                     "}";
            }

            return bussinesHoursList + "]";
        }

        // Método auxiliar para agregar los días de la semana del objeto actual a la lista
        private static void AddDayOfWeekToList(List<int> daysOfWeek, Schedule_Row scheduleRow) {
            if (scheduleRow.Monday)     daysOfWeek.Add(1);
            if (scheduleRow.Tuesday)    daysOfWeek.Add(2);
            if (scheduleRow.Wednesday)  daysOfWeek.Add(3);
            if (scheduleRow.Thursday)   daysOfWeek.Add(4);
            if (scheduleRow.Friday)     daysOfWeek.Add(5);
            if (scheduleRow.Saturday)   daysOfWeek.Add(6);
            if (scheduleRow.Sunday)     daysOfWeek.Add(7);
        }
        */
    }
}