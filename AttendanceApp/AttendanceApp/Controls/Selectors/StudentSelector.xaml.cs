using System.Data;
using System.Windows.Controls;

namespace AttendanceApp.Controls.Selectors
{
    /// <summary>
    /// Логика взаимодействия для StudentSelector.xaml
    /// </summary>
    public partial class StudentSelector : UserControl
    {
        private List<Student> Students = new List<Student>();
        private bool AddAll;

        public StudentSelector()
        {
            InitializeComponent();
        }

        public void Bind(SelectionChangedEventHandler handler)
        {
            GroupsCombo.SelectionChanged += handler;
        }

        public async Task UpdateItems(Group group, bool addAll)
        {
            //if (SessionData.IsStudent())
            //{
            //    Student? Student = DataBase.GetStudentFromUser(SessionData.LoggedAs);
            //    if (!DataBase.IsGroupElder(Student))
            //    {
            //        if (Student == null) throw new Exception("Student == null");
            //        Students = new List<Student>() { Student };
            //        GroupsCombo.ItemsSource = new List<string>() { await DataBase.GetStudentFullName(Student) };
            //        GroupsCombo.SelectedIndex = 0;
            //        return;
            //    }
            //}
            
            Students = await DataBase.GetStudentsOfGroup(group);
            AddAll = addAll && Students.Count > 1;

            List<string> items = new List<string>();

            if (addAll)
            {
                items.Add("Все");
            }

            foreach (Student Student in Students)
            {
                items.Add(await DataBase.GetStudentFullName(Student));
            }

            GroupsCombo.ItemsSource = items;
            GroupsCombo.SelectedIndex = 0;
        }

        public Student GetSelected()
        {
            if (GroupsCombo.SelectedIndex == -1) return null;
            if (GroupsCombo.SelectedIndex == 0 && AddAll) return null;
            return Students[GroupsCombo.SelectedIndex - (AddAll ? 1 : 0)];
        }

        public List<Student> GetItems()
        {
            return Students;
        }
    }
}
