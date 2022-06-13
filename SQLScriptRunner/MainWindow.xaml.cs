using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace SQLScriptRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _script = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = Application.Current.Resources["OpenFilter"].ToString();
            ofd.Title = Application.Current.Resources["OpenTitle"].ToString();

            if (ofd.ShowDialog() == true)
            {
                FileLabel.Text = ofd.SafeFileName;
                _script = File.ReadAllText(ofd.FileName);
            }
        }

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            ResultTextBlock.Text = Application.Current.Resources["TestProcess"].ToString();
            try
            {
                using (var conn = new MySqlConnection(GetConnectionString()))
                {
                   await conn.OpenAsync();
                }
                ResultTextBlock.Text = Application.Current.Resources["TestSuccess"].ToString();
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = ex.Message;
            }
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            ResultTextBlock.Text = Application.Current.Resources["RunProcess"].ToString();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                await conn.OpenAsync();
                MySqlCommand createCmd = new MySqlCommand(_script, conn);
                try
                {
                    await createCmd.ExecuteNonQueryAsync();
                    ResultTextBlock.Text = Application.Current.Resources["RunSuccess"].ToString();
                }
                catch (Exception ex)
                {
                    ResultTextBlock.Text = ex.Message;
                }
            }
        }

        private string GetConnectionString()
        {
            return "server=" + ServerTextBox.Text + ";" +
                    "userid=" + UsernameTextBox.Text + ";" +
                    "password=" + PasswordPasswordBox.Password + ";" +
                    "persistsecurityinfo=True;" +
                    "database=" + DatabaseTextBox.Text;
        }
    }
}
