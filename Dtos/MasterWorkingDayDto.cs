﻿namespace TestApiSalon.Dtos
{
    public class MasterWorkingDayDto
    {
        public DateOnly Date { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
    }
}
