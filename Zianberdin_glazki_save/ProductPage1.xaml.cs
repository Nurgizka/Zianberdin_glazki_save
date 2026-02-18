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

namespace Zianberdin_glazki_save
{
    /// <summary>
    /// Логика взаимодействия для ProductPage1.xaml
    /// </summary>
    public partial class ProductPage1 : Page
    {
        public ProductPage1()
        {
            InitializeComponent();
            var currentProducts = Zianberdin_saveEntities.GetContext().Agent.ToList();
            ServiceListView.ItemsSource = currentProducts;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ProductEditPage1());
        }

        private void UpdateAgents()
        {
            var context = Zianberdin_saveEntities.GetContext();
            var currentAgents = context.Agent.ToList();

            
            if (!string.IsNullOrWhiteSpace(TBoxSearch.Text))
            {
                var searchText = TBoxSearch.Text.ToLower();

                var digitsOnlySearch = new string(searchText.Where(char.IsDigit).ToArray());

                currentAgents = currentAgents.Where(a =>
                {
                    
                    if (a.Title != null && a.Title.ToLower().Contains(searchText))
                        return true;

                    
                    if (a.Email != null && a.Email.ToLower().Contains(searchText))
                        return true;

                    
                    if (a.Phone != null)
                    {
                        
                        if (a.Phone.Contains(searchText))
                            return true;

                        
                        if (!string.IsNullOrEmpty(digitsOnlySearch))
                        {
                            var cleanPhone = new string(a.Phone.Where(char.IsDigit).ToArray());

                            if (cleanPhone.Contains(digitsOnlySearch))
                                return true;
                        }
                    }

                    return false;
                }).ToList();
            }

            
            if (ComboType.SelectedIndex > 0) 
            {
                var selectedType = (ComboType.SelectedItem as TextBlock)?.Text;
                if (!string.IsNullOrEmpty(selectedType))
                {
                    currentAgents = currentAgents.Where(a => a.AgentTypeName == selectedType).ToList();
                }
            }

            
            switch (ComboSorting.SelectedIndex)
            {
                case 1: 
                    currentAgents = currentAgents.OrderBy(a => a.Title).ToList();
                    break;
                case 2: 
                    currentAgents = currentAgents.OrderByDescending(a => a.Title).ToList();
                    break;
                case 3: 
                    currentAgents = currentAgents.OrderBy(a => a.Discount).ToList();
                    break;
                case 4: 
                    currentAgents = currentAgents.OrderByDescending(a => a.Discount).ToList();
                    break;
                case 5: 
                    currentAgents = currentAgents.OrderBy(a => a.Priority).ToList();
                    break;
                case 6: 
                    currentAgents = currentAgents.OrderByDescending(a => a.Priority).ToList();
                    break;
                    
            }


            
            ServiceListView.ItemsSource = currentAgents;
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void ComboSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }
    }
}
