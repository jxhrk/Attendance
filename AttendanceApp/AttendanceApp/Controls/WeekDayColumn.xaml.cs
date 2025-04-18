using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

namespace AttendanceApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для WeekDayColumn.xaml
    /// </summary>
    public partial class WeekDayColumn : UserControl, INotifyPropertyChanged, INotifyCollectionChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        //public ObservableCollection<AttendanceDay> days { get; set; } = new ObservableCollection<AttendanceDay>();

        private ObservableCollection<AttendanceDay> days;
        public ObservableCollection<AttendanceDay> Days
        {
            get { return days; }
            set
            {
                days = value;
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Days)));
            }
        }

        private string day;


        public string Day
        {
            set
            {
                day = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Day)));
            }
            get { return day; }
        }

        private string weekDay;

        public string WeekDay
        {
            set
            {
                weekDay = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WeekDay)));
            }
            get { return weekDay; }
        }

        private string month;

        public string Month
        {
            set
            {
                month = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Month)));
            }
            get { return month; }
        }


        DateTime Date = DateTime.MinValue;

        private object _lock = new object();


        Dictionary<int, List<Truancy>> Data;

        public WeekDayColumn()
        {
            InitializeComponent();

            Days = new ObservableCollection<AttendanceDay>();
            BindingOperations.EnableCollectionSynchronization(Days, _lock);

            //TrGrid.ItemsSource = days;
        }


        public void ChangeDate(DateTime date)
        {
            Day = date.Day.ToString();
            WeekDay = DayOfWeekToString(date.DayOfWeek);
            Month = MonthToString(date.Month);
            Date = date;
        }

        public void UpdateData(Dictionary<int, List<Truancy>> data, List<int> lessons)
        {
            //Days.Clear();

            int i = 0;
            foreach (List<Truancy> row in data.Values)
            {
                bool add = Days.Count <= i;
                AttendanceDay day = add ? new AttendanceDay() : Days[i];//new AttendanceDay();
                day.Lesson1 = CheckLesson(row, lessons, 1);
                day.Lesson2 = CheckLesson(row, lessons, 2);
                day.Lesson3 = CheckLesson(row, lessons, 3);
                day.Lesson4 = CheckLesson(row, lessons, 4);
                day.Lesson5 = CheckLesson(row, lessons, 5);
                if (add)
                {
                    Days.Add(day);
                }
                i++;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Days)));

            Dispatcher.Invoke(() =>
            {
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Days)));
            });
            //Dispatcher.Invoke(() =>
            //{
            //TrGrid.ItemsSource = days;
            //});

            Data = data;
        }

        int CheckLesson(List<Truancy> row, List<int> lessons, int number)
        {
            if (!lessons.Contains(number)) return -1;
            Truancy? t = row.Find(o => o.IdScheduleNavigation.LessonNumber == number);
            if (t == null) return 0;
            return t.IdReason == 0 ? 1 : 2;
        }
        string DayOfWeekToString(DayOfWeek month)
        {
            switch (month)
            {
                case DayOfWeek.Monday:
                    return "пн";
                case DayOfWeek.Tuesday:
                    return "вт";
                case DayOfWeek.Wednesday:
                    return "ср";
                case DayOfWeek.Thursday:
                    return "чт";
                case DayOfWeek.Friday:
                    return "пт";
                case DayOfWeek.Saturday:
                    return "сб";
                case DayOfWeek.Sunday:
                    return "вс";
                default:
                    return "";
            }
        }

        string MonthToString(int month)
        {
            switch (month)
            {
                case 1:
                    return "янв.";
                case 2:
                    return "фев.";
                case 3:
                    return "мар.";
                case 4:
                    return "апр.";
                case 5:
                    return "мая";
                case 6:
                    return "июня";
                case 7:
                    return "июля";
                case 8:
                    return "авг.";
                case 9:
                    return "сент.";
                case 10:
                    return "окт.";
                case 11:
                    return "ноя.";
                case 12:
                    return "дек.";
                default:
                    return null;
            }
        }

        private async void TrGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (TrGrid.SelectedCells.Count == 0) return;
            if (DataBase.CurrentSession.user.IdRole == 0 && !await DataBase.GetIsGroupElder())
            {
                TrGrid.SelectedCells.Clear();
                return;
            }
            DataGridCellInfo info = TrGrid.SelectedCells.First();

            int number = info.Column.DisplayIndex + 1;
            AttendanceDay day = info.Item as AttendanceDay;

            int index = Days.IndexOf(day);
            int stIndex = Data.ElementAt(index).Key;

            int result = -1;

            switch (number)
            {
                case 1:
                    result = GetNewState(day.Lesson1);
                    day.Lesson1 = result;
                    break;
                case 2:
                    result = GetNewState(day.Lesson2);
                    day.Lesson2 = result;
                    break;
                case 3:
                    result = GetNewState(day.Lesson3);
                    day.Lesson3 = result;
                    break;
                case 4:
                    result = GetNewState(day.Lesson4);
                    day.Lesson4 = result;
                    break;
                case 5:
                    result = GetNewState(day.Lesson5);
                    day.Lesson5 = result;
                    break;
            }

            //MessageBox.Show($"{stIndex} {Date} {number} {!result}");
            if (result != -1)
            {
                await DataBase.SetTruancy(stIndex, Date, number, result == 0, result == 2);
            }

            TrGrid.SelectedCells.Clear();
        }

        int GetNewState(int curState)
        {
            if (curState == -1) return -1;
            int result = curState + 1;
            if (result > 2)
            {
                result = 0;
            }
            return result;
        }

        private void TrGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //e.Handled = false;
            ScrollViewer svParent = GetParentOfType<ScrollViewer>(TrGrid);
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
    }
}
