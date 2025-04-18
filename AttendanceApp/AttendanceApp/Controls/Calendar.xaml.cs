using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace AttendanceApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для Calendar.xaml
    /// </summary>
    public partial class Calendar : UserControl, INotifyPropertyChanged
    {
        public Calendar()
        {
            InitializeComponent();

            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private int year;

        public int Year
        {
            get { return year; }
            set
            {
                if (value != year)
                {
                    year = value;
                    NotifyPropertyChanged(nameof(Year));
                }
            }
        }
        private int Month;

        private string monthName;

        public string MonthName
        {
            get { return monthName; }
            set
            {
                if (value != monthName)
                {
                    monthName = value;
                    NotifyPropertyChanged(nameof(MonthName));
                }
            }
        }

        public async Task Init()
        {
            Year = DateTime.Now.Year;
            Month = DateTime.Now.Month;

            StGrid.Init<string>(31, 40, "Студент", ["Всего пар", "Всего дней", "Прогулов", "По справке"]);

            await GroupSelect.UpdateItems(false);

            GroupSelect.Bind(GroupCombo_SelectionChanged);

            await Show();
        }

        public async Task Show()
        {
            UpdateDaysCount();
            await ViewStudents();
        }

        private async void GroupCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Show();
        }

        private async Task ViewStudents()
        {
            Group grp = GroupSelect.GetSelected();

            Dictionary<string, List<Truancy>> truanciesAll = await DataBase.GetTruanciesInMonth(grp, Year, Month);

            string[,] data = new string[truanciesAll.Count, 35];
            string[] rowNames = new string[truanciesAll.Count];

            for (int i = 0; i < truanciesAll.Count; i++)
            {
                KeyValuePair<string, List<Truancy>> pair = truanciesAll.ElementAt(i);
                rowNames[i] = pair.Key;
                List<Truancy> thisStudent = pair.Value;

                for (int j = 0; j < 31; j++)
                {
                    int count = thisStudent.Where(o => o.IdScheduleNavigation.LessonDate.Day == j + 1).Count();
                    data[i, j] = count == 0 ? "" : count.ToString();
                }

                data[i, 31] = thisStudent.Count.ToString();
                data[i, 32] = thisStudent.GroupBy(o => o.IdScheduleNavigation.LessonDate).Count().ToString();
                data[i, 33] = thisStudent.FindAll(o => o.IdReason == 0).Count.ToString();
                data[i, 34] = thisStudent.FindAll(o => o.IdReason == 1).Count.ToString();
            }

            StGrid.UpdateRowsVisibility(truanciesAll.Count);
            StGrid.Set2DArray(data, rowNames);

        }

        private void UpdateDaysCount()
        {
            StGrid.UpdateCellsVisibility(DateTime.DaysInMonth(Year, Month));

            MonthName = new DateTime(2015, Month, 1).ToString("MMMM", CultureInfo.CurrentCulture);
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            Month -= 1;
            if (Month < 1)
            {
                Month = 12;
                Year -= 1;
            }
            Show();
        }

        private void ButtonForward_Click(object sender, RoutedEventArgs e)
        {
            Month += 1;
            if (Month > 12)
            {
                Month = 1;
                Year += 1;
            }
            Show();
        }

        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
