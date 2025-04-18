using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Microsoft.IdentityModel.Tokens;

namespace AttendanceApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : UserControl
    {
        public MainWindow Main;

        private bool InProgress;

        public LoginPage()
        {
            InitializeComponent();
            ErrorMsg.Visibility = Visibility.Hidden;
        }


        private async void Login()
        {
            if (InProgress)
            {
                return;
            }

            if (LoginText.Text.IsNullOrEmpty() || PasswordText.Password.IsNullOrEmpty())
            {
                ErrorMsg.Text = "Необходимо заполнить поля";
                ErrorMsg.Visibility = Visibility.Visible;
                return;
            }

            InProgress = true;
            LoginButton.IsEnabled = false;
            Session session = await DataBase.Login(LoginText.Text, PasswordText.Password);

            if (session == null)
            {
                LoginButton.IsEnabled = true;
                InProgress = false;
                ErrorMsg.Text = "Неверный логин или пароль";
                ErrorMsg.Visibility = Visibility.Visible;
                return;
            }

            //SessionData.guid = user;
            Main.OnLogin();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void LoginText_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                PasswordText.Focus();
            }
        }

        private void PasswordText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Login();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoginText.Focus();
        }
    }
}
