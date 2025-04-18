using System.Windows.Controls;

namespace AttendanceApp.Controls.Selectors
{
    /// <summary>
    /// Логика взаимодействия для TeacherSelector.xaml
    /// </summary>
    public partial class TeacherSelector : UserControl
    {
        private List<Teacher> Teachers;
        private bool AddAll;

        public TeacherSelector()
        {
            InitializeComponent();
        }

        public void Bind(SelectionChangedEventHandler handler)
        {
            GroupsCombo.SelectionChanged += handler;
        }

        public async Task UpdateItems(bool addAll)
        {
            Teachers = await DataBase.GetTeachers();
            AddAll = addAll;

            List<string> items = new List<string>();

            if (addAll)
            {
                items.Add("Все");
            }

            foreach (Teacher teach in Teachers)
            {
                string name = await DataBase.GetTeacherFullName(teach);
                //Person pers = teach.IdPersonNavigation;
                //items.Add($"{pers.LastName} {pers.FirstName} {pers.MiddleName}");
                items.Add(name);
            }

            GroupsCombo.ItemsSource = items;

            if (!items.Contains(GroupsCombo.SelectedItem))
            {
                GroupsCombo.SelectedIndex = 0;
            }
        }

        public Teacher GetSelected()
        {
            if (GroupsCombo.SelectedIndex == -1) return null;
            if (GroupsCombo.SelectedIndex == 0 && AddAll) return null;
            return Teachers[GroupsCombo.SelectedIndex - (AddAll ? 1 : 0)];
        }

        public List<Teacher> GetItems()
        {
            return Teachers;
        }
    }
}
