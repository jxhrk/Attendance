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

namespace AttendanceApp.Controls.Selectors
{
    /// <summary>
    /// Логика взаимодействия для FullNameInput.xaml
    /// </summary>
    public partial class FullNameInput : UserControl
    {
        public string FirstName = "";
        public string LastName = "";
        public string MiddleName = "";

        public bool IsValid;

        public FullNameInput()
        {
            InitializeComponent();
        }

        public void HideIndicator()
        {
            Indicator.Visibility = Visibility.Collapsed;
        }

        public void SetText(string text)
        {
            FullNameText.Text = text;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = (sender as TextBox).Text;

            string[] parts = text.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            Brush White = Application.Current.Resources["Foreground"] as Brush;
            Brush Green = Application.Current.Resources["ForegroundGreen"] as Brush;
            Brush BgGreen = Application.Current.Resources["BackgroundGreen"] as Brush;
            Brush BgRed = Application.Current.Resources["BackgroundRed"] as Brush;

            Indicator.Background = BgRed;
            F.Foreground = White;
            I.Foreground = White;
            O.Foreground = White;

            IsValid = false;

            if (parts.Length > 0)
            {
                F.Foreground = Green;
                LastName = parts[0];
                if (parts.Length > 1)
                {
                    I.Foreground = Green;
                    Indicator.Background = BgGreen;
                    FirstName = parts[1];
                    IsValid = true;
                    if (parts.Length > 2)
                    {
                        O.Foreground = Green;
                        MiddleName = parts[2];
                    }
                }
            }
        }
    }
}
