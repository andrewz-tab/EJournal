using EJournal.Data;
using EJournal.Models;
using System.Globalization;

namespace EJournal.Repository.IRepository
{
    public interface ITimetableRepository
    {
        Task<IEnumerable<Class>> GetClassesTimetableByDay(DateTime date);
        Task<Employee> GetTeacher(int teacherId);
		Task<Class> GetClass(int classId);
		Task<IEnumerable<Lesson>> GetTimetalbeDiariesByWeek(int classId, string? weekId, int? studentId);
        Task<IEnumerable<Lesson>> GetTimetableByWeekForTeacher(int teacherId, string? weekId);
        Task SaveAsync();

        public static DateTime GetDateTimeByWeekId(string? weekId)
        {

            DateTime today = DateTime.Today;
            int WeekIdDefault = 1;
            DateTime defaultDay = new DateTime(2022, 12, 26);

            DateTime currentBegin = today;
            if (weekId != null)
            {
                var week = weekId.Split("-W");
                try
                {
                    currentBegin = ISOWeek.ToDateTime(Convert.ToInt32(week[0]), Convert.ToInt32(week[1]), DayOfWeek.Monday);
                    return currentBegin;
                }
                catch
                {

                }
            }
            while (currentBegin.DayOfWeek != DayOfWeek.Monday)
            {
                currentBegin = currentBegin.AddDays(-1);
            }

            return currentBegin;
        }
        

        public static DateTime GetDateTimeByDayId(int? dayId)
        {
            DateTime today = DateTime.Today;
            DateTime defaultDay = new DateTime(2023, 01, 01);
            const int defaultDayId = 1;
            DateTime currentDay = today;
            if (dayId != null && dayId > 0)
            {
                currentDay = defaultDay.AddDays((int)dayId - defaultDayId);
            }
            return currentDay;
        }
        public static int GetDayId(DateTime? date = null)
        {
            DateTime today = DateTime.Today;
            DateTime defaultDay = new DateTime(2023, 01, 01);
            const int defaultDayId = 1;
            DateTime currentDay = today;
            if (date != null)
            {
                currentDay = date.Value;
            }
            int dayId = (currentDay - defaultDay).Days + 1;
            return dayId;
        }

    }
}
