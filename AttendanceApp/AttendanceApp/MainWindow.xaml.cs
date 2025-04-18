using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace AttendanceApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.InitTitlebarTheme();

            LoginPanel.Visibility = Visibility.Visible;
            LoginPanel.Main = this;

        }

        public async void Pinging()
        {
            while (true)
            {
                await DataBase.Ping();
                await Task.Delay(10000);
            }
        }

        public async void OnLogin()
        {
            LoginPanel.Visibility = Visibility.Collapsed;

            StFilter.Init();
            StStudent.Init();
            StCalendar.Init();
            if (DataBase.CurrentSession.user.IdRole == 2)
            {
                StAdmin.Init();
            }
            else
            {
                AdminTab.Visibility = Visibility.Collapsed;
            }

            StWeek.Init();

            Pinging();

            FullNameText.Text = await DataBase.GetUserFullName();

            CurrentRoleText.Text = await DataBase.GetUserRole();
        }
    }
}