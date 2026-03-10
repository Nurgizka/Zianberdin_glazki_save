using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Zianberdin_glazki_save;
using Path = System.IO.Path;

namespace Zianberdin_glazki_save
{
    public partial class VozmiTelefon : Page
    {
        private Agent _currentAgent;

        public VozmiTelefon(Agent selectedAgent = null)
        {
            InitializeComponent();
            LoadAgentTypes();

            if (selectedAgent != null)
            {
                _currentAgent = selectedAgent;
                DeleteButton.Visibility = Visibility.Visible;
                if (TypeComboBox.ItemsSource != null)
                    TypeComboBox.SelectedValue = _currentAgent.AgentTypeID;
            }
            else
            {
                _currentAgent = new Agent();
                DeleteButton.Visibility = Visibility.Collapsed;
            }

            DataContext = _currentAgent;
        }

        private void LoadAgentTypes()
        {
            try
            {
                var types = ZianberdinGlazkiSaveEntities.GetContext().AgentType.ToList();
                TypeComboBox.ItemsSource = types;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentAgent.Title))
                errors.AppendLine("Укажите наименование агента");

            if (TypeComboBox.SelectedItem == null)
                errors.AppendLine("Выберите тип агента");
            else
                _currentAgent.AgentTypeID = (int)TypeComboBox.SelectedValue;

            if (string.IsNullOrWhiteSpace(_currentAgent.Address))
                errors.AppendLine("Укажите адрес");

            if (string.IsNullOrWhiteSpace(_currentAgent.DirectorName))
                errors.AppendLine("Укажите ФИО директора");

            if (_currentAgent.Priority < 0)
                errors.AppendLine("Приоритет должен быть неотрицательным числом");

            if (string.IsNullOrWhiteSpace(_currentAgent.INN))
                errors.AppendLine("Укажите ИНН");

            if (string.IsNullOrWhiteSpace(_currentAgent.KPP))
                errors.AppendLine("Укажите КПП");

            if (string.IsNullOrWhiteSpace(_currentAgent.Phone))
                errors.AppendLine("Укажите телефон");
            else
            {
                string cleanPhone = _currentAgent.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace("+", "").Replace(" ", "");
                if (cleanPhone.Length < 10)
                    errors.AppendLine("Телефон указан неверно");
            }

            if (string.IsNullOrWhiteSpace(_currentAgent.Email))
                errors.AppendLine("Укажите email");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            try
            {
                var context = ZianberdinGlazkiSaveEntities.GetContext();

                if (_currentAgent.ID == 0)
                    context.Agent.Add(_currentAgent);

                context.SaveChanges();
                MessageBox.Show("Информация сохранена");

                NavigationService.Navigate(new ProductPage1());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var context = ZianberdinGlazkiSaveEntities.GetContext();

                    var history = context.AgentPriorityHistory.Where(aph => aph.AgentID == _currentAgent.ID).ToList();
                    context.AgentPriorityHistory.RemoveRange(history);

                    var shops = context.Shop.Where(s => s.AgentID == _currentAgent.ID).ToList();
                    context.Shop.RemoveRange(shops);

                    context.Agent.Remove(_currentAgent);
                    context.SaveChanges();

                    MessageBox.Show("Агент удален");
                    NavigationService.Navigate(new ProductPage1());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            string agentsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agents");
            var dialog = new OpenFileDialog
            {
                Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp",
                InitialDirectory = agentsFolder
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    Directory.CreateDirectory(agentsFolder);

                    string selectedFile = dialog.FileName;
                    string fileName = Path.GetFileName(selectedFile);
                    string destPath = Path.Combine(agentsFolder, fileName);

                    if (!Path.GetDirectoryName(selectedFile).Equals(agentsFolder, StringComparison.OrdinalIgnoreCase))
                    {
                        int i = 1;
                        while (File.Exists(destPath))
                        {
                            string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                            string ext = Path.GetExtension(fileName);
                            destPath = Path.Combine(agentsFolder, $"{nameWithoutExt}_{i++}{ext}");
                        }
                        File.Copy(selectedFile, destPath);
                    }
                    else
                    {
                        destPath = selectedFile;
                    }

                    _currentAgent.Logo = $"agents\\{Path.GetFileName(destPath)}";
                    LogoImage.Source = new BitmapImage(new Uri(destPath));

                    var binding = LogoImage.GetBindingExpression(Image.SourceProperty);
                    binding?.UpdateTarget();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }
    }
}