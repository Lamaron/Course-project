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
        private bool _isFormatting = false;

        public PaymentWindow()
        {
            InitializeComponent();
            TariffComboBox.SelectionChanged += TariffComboBox_SelectionChanged;
        }

        private void TariffComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selected = (TariffComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString();

            if (selected == "Месячный")
                AmountTextBlock.Text = "Сумма: 300 Р";
            else if (selected == "Годовой")
                AmountTextBlock.Text = "Сумма: 3000 Р";
        }

        private void CardNumberTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_isFormatting) return;

            _isFormatting = true;

            string digits = new string(CardNumberTextBox.Text.Where(char.IsDigit).ToArray());

            if (digits.Length > 16)
                digits = digits.Substring(0, 16);

            var formatted = string.Join("-",
                Enumerable.Range(0, (digits.Length + 3) / 4)
                          .Select(i => digits.Substring(i * 4, Math.Min(4, digits.Length - i * 4)))
            );

            CardNumberTextBox.Text = formatted;
            CardNumberTextBox.CaretIndex = formatted.Length;

            _isFormatting = false;
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(CardNumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(ExpiryTextBox.Text) ||
                string.IsNullOrWhiteSpace(CvvBox.Password) ||
                TariffComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show(
                $"Спасибо, {FirstNameTextBox.Text}!\n\n" +
                "Платёж успешно выполнен (демо-режим).\nPremium активирован.",
                "Успех",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

}

