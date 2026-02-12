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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ProductEditPage1());
        }
    }
}
