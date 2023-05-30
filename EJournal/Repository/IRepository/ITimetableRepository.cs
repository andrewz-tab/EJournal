using EJournal.Data;
using EJournal.Models;

namespace EJournal.Repository.IRepository
{
    public interface ITimetableRepository
    {
        Task<IEnumerable<Class>> GetClassesTimetableByDay(DateTime date);
        Task<IEnumerable<Lesson>> GetTimetalbeByWeek(int? weekId);
        Task SaveAsync();

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
            //dayIdOut = (currentDay - defaultDay).Days + 1;
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
