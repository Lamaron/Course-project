using System.Windows;
using Data;

namespace UI
{
    /// <summary>
    /// Логика взаимодействия для PaymentWindow.xaml
    /// </summary>
    public partial class PaymentWindow : Window
    {
        private MusicDbContext _db = new MusicDbContext();

        public PaymentWindow()
        {
            InitializeComponent();
            TariffComboBox.SelectionChanged += TariffComboBox_SelectionChanged;
        }

        private void TariffComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var sel = (TariffComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString();
            if (sel == "Месячный") AmountTextBlock.Text = "Сумма: 4.99";
            else if (sel == "Годовой") AmountTextBlock.Text = "Сумма: 49.99";
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Платеж успешно проведен (эмуляция). Премиум активирован.");
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
