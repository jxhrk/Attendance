using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace AttendanceApp.Controls
{

    public class AttendanceDay : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int lesson1;
        public int Lesson1
        {
            set
            {
                lesson1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Lesson1)));
            }
            get { return lesson1; }
        }
        private int lesson2;
        public int Lesson2
        {
            set
            {
                lesson2 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Lesson2)));
            }
            get { return lesson2; }
        }
        private int lesson3;
        public int Lesson3
        {
            set
            {
                lesson3 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Lesson3)));
            }
            get { return lesson3; }
        }
        private int lesson4;
        public int Lesson4
        {
            set
            {
                lesson4 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Lesson4)));
            }
            get { return lesson4; }
        }
        private int lesson5;
        public int Lesson5
        {
            set
            {
                lesson5 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Lesson5)));
            }
            get { return lesson5; }
        }
    }

    public class NameData
    {
        public string Name { get; set; }
        public NameData(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Логика взаимодействия для WeekPage.xaml
    /// </summary>
    public partial class WeekPage : UserControl
    {
        private List<WeekDayColumn> Columns;

        DateTime WeekStart = DateTime.MinValue;

        public WeekPage()
        {
            InitializeComponent();

            Columns = new List<WeekDayColumn>()
            {
                C1, C2, C3, C4, C5, C6
            };

            DateTime now = DateTime.Now;
            WeekStart = StartOfWeek(now, DayOfWeek.Monday);

            

            //UpdateData();

            //AttendanceDay d = new AttendanceDay();
            //days = new AttendanceWeek() { day1 = new List<AttendanceDay>() { d } };

            //dtg.ItemsSource = new List<AttendanceWeek>() { days };

        }

        public async void Init()
        {
            await GroupSelect.UpdateItems(false);
            GroupSelect.Bind(GroupsCombo_SelectionChanged);
            await UpdateData();
        }

        private async void GroupsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await UpdateData();
        }

        public DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        async Task UpdateData()
        {
            Dictionary<int, List<Truancy>> truancies = await DataBase.GetTruanciesInPeriodForGroup(GroupSelect.GetSelected(), WeekStart, WeekStart + TimeSpan.FromDays(6));
            List<List<int>> schedule = await DataBase.GetScheduleInPeriod(GroupSelect.GetSelected(), WeekStart, WeekStart + TimeSpan.FromDays(6));
            List<string> namesStrs = await DataBase.GetGroupStudentsFullNames(GroupSelect.GetSelected());

            for (int i = 0; i < Columns.Count; i++)
            {
                UpdateColumn(truancies, schedule, i);
            }

            List<NameData> names = new List<NameData>();
            foreach (string name in namesStrs)
            {
                names.Add(new NameData(name));
            }

            StudentNames.ItemsSource = names;
        }

        private async Task UpdateColumn(Dictionary<int, List<Truancy>> truancies, List<List<int>> schedule, int i)
        {
            Dictionary<int, List<Truancy>> byDay = new Dictionary<int, List<Truancy>>();

            await Task.Run(() =>
            {
                Columns[i].ChangeDate(WeekStart + TimeSpan.FromDays(i));

                DateOnly currentDay = DateOnly.FromDateTime(WeekStart + TimeSpan.FromDays(i));

                foreach (KeyValuePair<int, List<Truancy>> pair in truancies)
                {
                    byDay.Add(pair.Key, pair.Value.FindAll(o => o.IdScheduleNavigation.LessonDate == currentDay));
                }

                Columns[i].UpdateData(byDay, schedule[i]);
            });

        }

        private void StudentNames_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer svParent = GetParentOfType<ScrollViewer>(StudentNames);
            if (svParent != null) svParent.ScrollToVerticalOffset(svParent.VerticalOffset - e.Delta);
        }

        private T GetParentOfType<T>(DependencyObject control) where T : System.Windows.DependencyObject
        {
            DependencyObject ParentControl = control;

            do
                ParentControl = VisualTreeHelper.GetParent(ParentControl);
            while (ParentControl != null && !(ParentControl is T));

            return ParentControl as T;
        }

        private async void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            WeekStart -= TimeSpan.FromDays(7);
            await UpdateData();
        }

        private async void ButtonForward_Click(object sender, RoutedEventArgs e)
        {
            WeekStart += TimeSpan.FromDays(7);
            await UpdateData();
        }
    }
}
