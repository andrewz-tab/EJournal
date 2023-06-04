using System.Reflection.Metadata.Ecma335;

namespace EJournal
{
    public static class WC
    {
        public static string Cookie { get; set; }
        public static string TypeUser { get { return "TypeUser"; } }
        public const string RequiredChangePassword = "RequiredChangePassword";
        public const string TeacherRole = "Учитель";
        public const string AdminRole = "Администратор";
        public const string HeadTeacherRole = "Завуч";
        public const string EmployeeUser = "Сотрудник";
        public const string StudentUser = "Ученик"; 
        public const string PolicyOnlyForStudent = "PolicyOnlyForStudent";
        public const string PolicyOnlyForTeacher = "PolicyOnlyForTeacher";
        public const string PolicyOnlyForHeadTeacher = "PolicyOnlyForHeadTeacher";
        public const string PolicyOnlyForAdmin = "PolicyOnlyForAdmin";
        public const string PolicyOnlyForEmployee = "PolicyOnlyForEmployee";
        public const string ClassId = "ClassId";
        public const string EmployeeId = "EmployeeId";
        public const string StudentId = "StudentId";
        public const string PolicyOnlyForHeadTeacherOrAdmin = "PolicyOnlyForHeadTeacherOrAdmin";

        public static string TranslateToMark(int? value)
        {
            if (value == null)
                return "";
            if (value > 0 && value <= 5)
                return value.ToString();
            if (value == -1)
                return "Н";
            return "";
        }
    }
}
