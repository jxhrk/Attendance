using System.Windows;
using System.Windows.Controls;
using AttendanceApp.Controls.Selectors;

namespace AttendanceApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для StudentDetailsPage.xaml
    /// </summary>
    public partial class StudentDetailsPage : UserControl
    {
        public StudentDetailsPage()
        {
            InitializeComponent();
        }

        public async Task Init()
        {
            ResultsGrid.Init<string>(0, 40, "Предмет", ["Всего пропусков", "Прогулов", "По справке", "% пропусков"]);
            ResultsGrid2.Init<string>(0, 40, "Преподаватель", ["Всего пропусков", "Прогулов", "По справке", "% пропусков"]);

            await GroupSelect.UpdateItems(false);

            GroupSelect.Bind(GroupsCombo_SelectionChanged);
            StudentSelect.Bind(StudentCombo_SelectionChanged);

            ResultsGrid.StListGrid.MaxColumnWidth = 200;

            await UpdateStudentCombo();
            await UpdateData();
        }

        private async Task UpdateStudentCombo()
        {
            Group group = GroupSelect.GetSelected();

            if (group == null)
            {
                return;
            }
            await StudentSelect.UpdateItems(group, false);
        }

        private async Task UpdateData()
        {
            await Show(GroupSelect.GetSelected(), StudentSelect.GetSelected());
        }

        private async Task Show(Group group, Student st)
        {
            const int MAGIC_TERM = 5;
            List<Discipline> disc = await DataBase.GetDisciplinesForGroupInTerm(group, MAGIC_TERM);
            string[] discsNames = new string[disc.Count];
            for (int i = 0; i < disc.Count; i++)
            {
                discsNames[i] = $"{disc[i].NameId} {disc[i].Name}";
            }

            string[,] data = new string[disc.Count, 4];

            List<Truancy> truancies = await DataBase.GetTruanciesOfStudent(st, DateTime.Now.Year, 1, null, null);

            for (int i = 0; i < disc.Count; i++)
            {
                Discipline discipline = disc[i];

                //int discLessons = DataBase.GetPlanLessonsPlan(group.IdSpec, discipline, MAGIC_TERM);
                int discLessons2 = await DataBase.GetPlanLessons(group.IdGroup, discipline.IdDiscipline, -1);

                List<Truancy> filtered = truancies.FindAll(o => o.IdScheduleNavigation.IdDiscipline == discipline.IdDiscipline);

                data[i, 0] = filtered.Count.ToString();
                int woReason = filtered.Where(o => o.IdReason == 0).Count();
                int wReason = filtered.Where(o => o.IdReason == 1).Count();
                data[i, 1] = woReason.ToString();
                data[i, 2] = wReason.ToString();
                //double percentDiscipline = filtered.Count / Convert.ToDouble(discLessons) * 100;
                double percentDiscipline2 = filtered.Count / Convert.ToDouble(discLessons2) * 100;
                data[i, 3] = $"{Math.Round(percentDiscipline2, 2)}%";

            }

            ResultsGrid.UpdateRowsVisibility(disc.Count);
            ResultsGrid.Set2DArray(data, discsNames);


            List<Teacher> teach = await DataBase.GetTeachers();
            string[] teachNames = new string[teach.Count];
            for (int i = 0; i < teach.Count; i++)
            {
                teachNames[i] = await DataBase.GetTeacherFullName(teach[i]);
            }

            string[,] data2 = new string[teach.Count, 4];

            for (int i = 0; i < teach.Count; i++)
            {
                Teacher teacher = teach[i];

                int teachLessons = await DataBase.GetPlanLessons(group.IdGroup, -1, teacher.IdTeacher);

                List<Truancy> filtered = truancies;// DataBase.GetTruanciesByTeacher(truancies, teacher);

                data2[i, 0] = filtered.Count.ToString();
                int woReason = filtered.Where(o => o.IdReason == 0).Count();
                int wReason = filtered.Where(o => o.IdReason == 1).Count();
                data2[i, 1] = woReason.ToString();
                data2[i, 2] = wReason.ToString();
                double percentTeacher = filtered.Count / Convert.ToDouble(teachLessons) * 100;
                data2[i, 3] = $"{Math.Round(percentTeacher, 2)}%";

            }

            ResultsGrid2.UpdateRowsVisibility(teach.Count);
            ResultsGrid2.Set2DArray(data2, teachNames);
        }


        private async void GroupsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await UpdateStudentCombo();
        }

        private async void StudentCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StudentSelect.GetSelected() != null)
            {
                await UpdateData();
            }
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
