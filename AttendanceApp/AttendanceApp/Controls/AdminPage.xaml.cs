using System;
using System.Collections.Generic;
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
using AttendanceApp.Controls.Selectors;

namespace AttendanceApp.Controls
{
    enum CreatingType
    {
        None,
        Student,
        Teacher,
        User,
        Group
    }
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : UserControl
    {
        List<Person> Students;
        List<Person> Teachers;
        List<Person> Users;
        CreatingType Type;
        Person SelectedPerson;
        Person SelectedTeacher;



        public AdminPage()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public async Task Init()
        {
            //FullNameStudent.HideIndicator();
            //FullNameStudent2.BindChanged(UpdateDataAsync);

            await GroupSelect.UpdateItems(true);
            await GroupSelect2.UpdateItems(false);

            GroupSelect.Bind(GroupsCombo_SelectionChanged);

            await UpdateDataSt(true);
            await UpdateDataT(true);
            await UpdateDataU(true);
        }

        private async void GroupsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await UpdateDataSt(true);
        }

        private async Task UpdateDataSt(bool updateList = false)
        {
            if (updateList)
            {
                Students = await DataBase.GetPersonsOfGroupAdmin(GroupSelect.GetSelected());
            }

            FilterPersonsAndUpdateList(FullNameStudent.Text, Students, StudentsList);
        }

        private async Task UpdateDataT(bool updateList = false)
        {
            if (updateList)
            {
                Teachers = await DataBase.GetPersonsOfTeachersAdmin();
            }

            FilterPersonsAndUpdateList(FullNameTeacher.Text, Teachers, TeachersList);
        }

        private async Task UpdateDataU(bool updateList = false)
        {
            if (updateList)
            {
                Users = await DataBase.GetPersonsOfUsersAdmin();
            }

            FilterPersonsAndUpdateList(FullNameUser.Text, Users, UsersList);
        }

        private void FilterPersonsAndUpdateList(string fullName, List<Person> persons, ListBox list)
        {
            List<Person> result = new List<Person>();
            foreach (Person person in persons)
            {
                if (IsMatchFullName(fullName, person.FirstName, person.LastName, person.MiddleName))
                {
                    result.Add(person);
                }
            }

            list.ItemsSource = result;
        }

        private bool IsMatchFullName(string fullName, string firstName, string lastName, string middleName)
        {
            string[] parts = fullName.ToLower().Split(' ');

            firstName = firstName.ToLower();
            lastName = lastName.ToLower();
            middleName = (middleName ?? "").ToLower();


            foreach (string part in parts)
            {
                if (!HasMatch(part, firstName, lastName, middleName))
                {
                    return false;
                }
            }

            return true;

        }

        private bool HasMatch(string part, string firstName, string lastName, string middleName)
        {
            return firstName.Contains(part) || lastName.Contains(part) || middleName.Contains(part);
        }

        private async void FullNameStudent_TextChanged(object sender, TextChangedEventArgs e)
        {
            await UpdateDataSt();
        }

        private async void FullNameTeacher_TextChanged(object sender, TextChangedEventArgs e)
        {
            await UpdateDataT();
        }
        
        private async void FullNameUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            await UpdateDataU();
        }

        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            Type = CreatingType.Student;
            AddPanel.Visibility = Visibility.Visible;
            FullNameInput.SetText(FullNameStudent.Text);
            GroupSelect2.SetSelected(GroupSelect.GetSelected());
            HideAll();
            FullNameLabel.Visibility = Visibility.Visible;
            FullNameInput.Visibility = Visibility.Visible;
            GroupLabel.Visibility = Visibility.Visible;
            GroupSelect2.Visibility = Visibility.Visible;
            
        }

        private void AddTeacher_Click(object sender, RoutedEventArgs e)
        {
            Type = CreatingType.Teacher;
            AddPanel.Visibility = Visibility.Visible;
            FullNameInput.SetText(FullNameTeacher.Text);
            HideAll();
            FullNameLabel.Visibility = Visibility.Visible;
            FullNameInput.Visibility = Visibility.Visible;
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            Type = CreatingType.User;
            AddPanel.Visibility = Visibility.Visible;
            FullNameInput.SetText(FullNameUser.Text);
            HideAll();
            PersonLabel.Visibility = Visibility.Visible;
            PersonBox.Visibility = Visibility.Visible;
            LoginLabel.Visibility = Visibility.Visible;
            LoginBox.Visibility = Visibility.Visible;
            PswLabel.Visibility = Visibility.Visible;
            PswBox.Visibility = Visibility.Visible;
        }

        private void HideAll()
        {
            FullNameLabel.Visibility = Visibility.Collapsed;
            FullNameInput.Visibility = Visibility.Collapsed;
            GroupLabel.Visibility = Visibility.Collapsed;
            GroupSelect2.Visibility = Visibility.Collapsed;
            PersonLabel.Visibility = Visibility.Collapsed;
            PersonBox.Visibility = Visibility.Collapsed;
            LoginLabel.Visibility = Visibility.Collapsed;
            LoginBox.Visibility = Visibility.Collapsed;
            PswLabel.Visibility = Visibility.Collapsed;
            PswBox.Visibility = Visibility.Collapsed;
            GrpNameLabel.Visibility = Visibility.Collapsed;
            GrpNameBox.Visibility = Visibility.Collapsed;
            GrpElderLabel.Visibility = Visibility.Collapsed;
            GrpElderBox.Visibility = Visibility.Collapsed;
            GrpCuratorLabel.Visibility = Visibility.Collapsed;
            GrpCuratorBox.Visibility = Visibility.Collapsed;
        }


        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            Type = CreatingType.Group;
            AddPanel.Visibility = Visibility.Visible;
            HideAll();
            GrpNameLabel.Visibility = Visibility.Visible;
            GrpNameBox.Visibility = Visibility.Visible;
            GrpElderLabel.Visibility = Visibility.Visible;
            GrpElderBox.Visibility = Visibility.Visible;
            GrpCuratorLabel.Visibility = Visibility.Visible;
            GrpCuratorBox.Visibility = Visibility.Visible;
        }

        private void StudentTeachersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Person person = (sender as ListBox).SelectedItem as Person;
            if (person != null)
            {
                SelectedPerson = person;
                PersonBox.Text = $"{SelectedPerson.LastName} {SelectedPerson.FirstName} {SelectedPerson.MiddleName}";

                if (sender == TeachersList)
                {
                    SelectedTeacher = person;
                    GrpCuratorBox.Text = $"{SelectedTeacher.LastName} {SelectedTeacher.FirstName} {SelectedTeacher.MiddleName}";
                }
            }

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string first = FullNameInput.FirstName;
            string last = FullNameInput.LastName;
            string middle = FullNameInput.MiddleName;


            switch (Type)
            {
                case CreatingType.Student:
                    await DataBase.CreateStudent(first, last, middle, GroupSelect2.GetSelected());
                    await UpdateDataSt(true);
                    break;
                case CreatingType.Teacher:
                    await DataBase.CreateTeacher(first, last, middle);
                    await UpdateDataT(true);
                    break;
                case CreatingType.User:
                    await DataBase.CreateUser(SelectedPerson, LoginBox.Text, PswBox.Password);
                    await UpdateDataU(true);
                    break;
                case CreatingType.Group:
                    await DataBase.CreateGroup(GrpNameBox.Text, GrpElderBox.FirstName, GrpElderBox.LastName, GrpElderBox.MiddleName, SelectedTeacher);
                    await UpdateDataU(true);
                    await GroupSelect.UpdateItems(true);
                    await GroupSelect2.UpdateItems(false);
                    break;
            }
        }
    }
}
