using Data;
using System.Windows;
using System.Windows.Controls;

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
            ExpiryTextBox.TextChanged += ExpiryTextBox_TextChanged;


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

        private void ExpiryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isFormatting) return;
            _isFormatting = true;

            string text = ExpiryTextBox.Text;

            string digits = new string(text.Where(char.IsDigit).ToArray());

            if (digits.Length > 4)
                digits = digits.Substring(0, 4);

            string formatted = "";
            for (int i = 0; i < digits.Length; i++)
            {
                if (i == 2)
                    formatted += "/";
                formatted += digits[i];
            }

            if (digits.Length >= 1)
            {
                int firstDigit = digits[0] - '0';
                if (firstDigit > 1)
                {
                    formatted = "0" + firstDigit;
                    if (digits.Length > 1)
                        formatted += "/" + digits.Substring(1);
                }
            }

            if (digits.Length >= 2)
            {
                int month = int.Parse(digits.Substring(0, 2));
                if (month > 12)
                {
                    formatted = "12";
                    if (digits.Length > 2)
                        formatted += "/" + digits[2];
                    if (digits.Length > 3)
                        formatted += digits[3];
                }
            }

            ExpiryTextBox.Text = formatted;

            ExpiryTextBox.CaretIndex = formatted.Length;

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

