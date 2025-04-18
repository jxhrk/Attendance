using System.Windows.Controls;

namespace AttendanceApp.Controls.Selectors
{
    /// <summary>
    /// Логика взаимодействия для DisciplineSelector.xaml
    /// </summary>
    public partial class DisciplineSelector : UserControl
    {
        private List<Discipline> Disciplines;
        private bool AddAll;

        SelectionChangedEventHandler ChangedHandler;

        public DisciplineSelector()
        {
            InitializeComponent();
        }

        public void Bind(SelectionChangedEventHandler handler)
        {
            GroupsCombo.SelectionChanged += handler;
            ChangedHandler = handler;
        }

        public async Task UpdateItems(Group grp, int term, bool addAll)
        {
            Disciplines = await DataBase.GetDisciplinesForGroupInTerm(grp, term);
            AddAll = addAll;

            List<string> items = new List<string>();

            if (addAll)
            {
                items.Add("Все");
            }

            foreach (Discipline disc in Disciplines)
            {
                items.Add(disc.NameId + " " + disc.Name);
            }

            GroupsCombo.SelectionChanged -= ChangedHandler;

            GroupsCombo.ItemsSource = items;

            if (!items.Contains(GroupsCombo.SelectedItem))
            {
                GroupsCombo.SelectedIndex = 0;
            }

            GroupsCombo.SelectionChanged += ChangedHandler;
        }

        public Discipline GetSelected()
        {
            if (GroupsCombo.SelectedIndex == -1) return null;
            if (GroupsCombo.SelectedIndex == 0 && AddAll) return null;
            return Disciplines[GroupsCombo.SelectedIndex - (AddAll ? 1 : 0)];
        }

        public List<Discipline> GetItems()
        {
            return Disciplines;
        }
    }
}
