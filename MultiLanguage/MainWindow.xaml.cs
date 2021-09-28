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

namespace MultiLanguage {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        /// <summary>
        /// The ViewModel of <see cref="MainWindow"/>.
        /// </summary>
        public MainWindowViewModel ViewModel { get; set; }
        public MainWindow() {
            this.InitializeComponent();
            // Initialize ViewModel and set it as the DataContext of MainWindow.
            this.ViewModel = new MainWindowViewModel();
            this.DataContext = this.ViewModel;
        }
    }
}
