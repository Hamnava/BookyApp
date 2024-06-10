using System.Windows;

namespace BookyApp
{
    /// <summary>
    /// Interaction logic for CustomMessageShow.xaml
    /// </summary>
    public partial class CustomMessageShow : Window
    {
        public CustomMessageShow(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static void Show(string message, Window owner)
        {
            var messageBox = new CustomMessageShow(message)
            {
                Owner = owner
            };
            messageBox.ShowDialog();
        }
    }
}
