using System.Data;
using System.Windows.Controls;

namespace AttendanceApp.Controls.Selectors
{
    /// <summary>
    /// Логика взаимодействия для GroupSelector.xaml
    /// </summary>
    public partial class GroupSelector : UserControl
    {
        private List<Group> Groups = new List<Group>();
        private bool AddAll;

        public GroupSelector()
        {
            InitializeComponent();
        }

        public void Bind(SelectionChangedEventHandler handler)
        {
            GroupsCombo.SelectionChanged += handler;
        }

        public async Task UpdateItems(bool addAll)
        {
            Groups = await DataBase.GetGroups();
            AddAll = addAll && Groups.Count > 1;

            List<string> items = new List<string>();

            if (addAll)
            {
                items.Add("Все");
            }

            foreach (Group group in Groups)
            {
                items.Add(group.Name);
            }

            GroupsCombo.ItemsSource = items;
            GroupsCombo.SelectedIndex = 0;
        }

        public Group GetSelected()
        {
            if (GroupsCombo.SelectedIndex == -1) return null;
            if (GroupsCombo.SelectedIndex == 0 && AddAll) return null;
            return Groups[GroupsCombo.SelectedIndex - (AddAll ? 1 : 0)];
        }

        public void SetSelected(Group g)
        {
            if (g == null)
            {
                GroupsCombo.SelectedIndex = -1;
                return;
            }
            GroupsCombo.SelectedItem = g.Name;
        }

        public List<Group> GetItems()
        {
            return Groups;
        }
    }
}
