using System.Windows;
using System.Windows.Forms;
using TranslationHelper.Properties;
using TranslationHelper.ViewModel;

namespace TranslationHelper {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {
    public MainWindow() {
      InitializeComponent();
    }

    public ApplicationModel AppModel {
      get => FindResource("ApplicationModel") as ApplicationModel;
    }
  }
}
