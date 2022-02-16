using System.Windows;

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
