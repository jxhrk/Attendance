using System.Windows;
using System.Windows.Controls;

namespace AttendanceApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для FilterPage.xaml
    /// </summary>
    public partial class FilterPage : UserControl
    {

        private int StudyYear;
        private int StudyTerm;

        public FilterPage()
        {
            InitializeComponent();
        }


        public async Task Init()
        {
            ResultsGrid.Init<string>(0, 40, "Студент", ["Всего пропусков", "Прогулов", "По справке", "% пропусков по предмету", "% пропусков по преподавателю"]);

            StudyYear = DateTime.Now.Year;
            StudyTerm = 1;

            await GroupSelect.UpdateItems(false);
            await TeacherSelect.UpdateItems(true);

            Term1.IsChecked = true;

            GroupSelect.Bind(GroupsCombo_SelectionChanged);
            DisciplineSelect.Bind(GroupsCombo_SelectionChanged);
            TeacherSelect.Bind(GroupsCombo_SelectionChanged);
            Term1.Checked += RadioButton_Checked;
            Term2.Checked += RadioButton_Checked;
            TermAll.Checked += RadioButton_Checked;

            await UpdateData();
        }

        private async Task UpdateData()
        {
            YearText.Text = $"{StudyYear}-{StudyYear + 1}";
            
            Group group = GroupSelect.GetSelected();

            int term = ((group.CourseNumber - 1) * 2 + StudyTerm);
            await DisciplineSelect.UpdateItems(group, term, true);

            await Show(group, DisciplineSelect.GetSelected(), TeacherSelect.GetSelected());
        }

        private async Task Show(Group grp, Discipline discipline, Teacher teacher)
        {
            Dictionary<string, List<Truancy>> truanciesAll = await DataBase.GetTruanciesInTerm(grp, StudyYear, StudyTerm, discipline, teacher);

            //const int MAGIC_TERM = 5;
            //int discLessons = DataBase.GetPlanLessonsPlan(grp.IdSpec, discipline, MAGIC_TERM);
            int discLessons2 = discipline == null ? 0 : await DataBase.GetPlanLessons(grp.IdGroup, discipline.IdDiscipline, -1);
            int teachLessons = teacher == null ? 0 : await DataBase.GetPlanLessons(grp.IdGroup, -1, teacher.IdTeacher);

            string[,] data = new string[truanciesAll.Count, 5];
            string[] rowNames = new string[truanciesAll.Count];

            for (int i = 0; i < truanciesAll.Count; i++)
            {
                KeyValuePair<string, List<Truancy>> pair = truanciesAll.ElementAt(i);
                rowNames[i] = pair.Key;
                List<Truancy> truancies = pair.Value;

                //if (discipline != null)
                //{
                //    truancies = truancies.FindAll(o => o.IdDiscipline == discipline.IdDiscipline);
                //}

                //if (teacher != null)
                //{
                //    truancies = DataBase.GetTruanciesByTeacher(truancies, teacher);
                //}

                //double percentDiscipline = truancies.Count / Convert.ToDouble(discLessons) * 100;
                double percentDiscipline2 = truancies.Count / Convert.ToDouble(discLessons2) * 100;
                double percentTeacher = truancies.Count / Convert.ToDouble(teachLessons) * 100;

                data[i, 0] = truancies.Count.ToString();
                data[i, 1] = truancies.Where(o => o.IdReason == 0).Count().ToString();
                data[i, 2] = truancies.Where(o => o.IdReason == 1).Count().ToString();
                //data[i, 3] = $"{Math.Round(percentDiscipline, 2)}% {Math.Round(percentDiscipline2, 2)}%";
                data[i, 3] = discLessons2 == 0 ? "-" : $"{Math.Round(percentDiscipline2, 2)}%";
                data[i, 4] = teachLessons == 0 ? "-" : $"{Math.Round(percentTeacher, 2)}%";

            }

            ResultsGrid.UpdateRowsVisibility(truanciesAll.Count);
            ResultsGrid.Set2DArray(data, rowNames);
        }

        private async void GroupsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await UpdateData();
        }

        private async void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StudyYear -= 1;
            await UpdateData();
        }

        private async void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            StudyYear += 1;
            await UpdateData();
        }

        private async void RadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Term1.IsChecked == true)
            {
                StudyTerm = 1;
            }
            else if (Term2.IsChecked == true)
            {
                StudyTerm = 2;
            }
            else if (TermAll.IsChecked == true)
            {
                StudyTerm = -1;
            }
            await UpdateData();
        }
    }
}
