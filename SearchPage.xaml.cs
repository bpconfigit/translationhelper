using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TranslationHelper {
  /// <summary>
  /// Interaction logic for SearchPage.xaml
  /// </summary>
  public partial class SearchPage : Window {
    public SearchPage() {
      InitializeComponent();
    }
  }

  // not the right way, but at least I can have a quick add of the editcommand
  public class ListViewCommand : ListView {
    public ICommand EditCommand {
      get => (ICommand) GetValue( EditingCommandProperty );
      set => SetValue( EditingCommandProperty, value );
    }

    public static readonly DependencyProperty EditingCommandProperty =
      DependencyProperty.Register( "EditCommand", typeof(ICommand), typeof( ListViewCommand ), new PropertyMetadata( null, EditCommandPropertyChanged ) );

    private static void EditCommandPropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs e ) {
      var listView = (ListView)d;
      if ( d == null ) {
        return;
      }
      if ( e.NewValue == null ) {
        listView.MouseDoubleClick -= OnMouseDoubleClickTrigger;
      } else { 
        listView.MouseDoubleClick += OnMouseDoubleClickTrigger;
      }
    }

    private static void OnMouseDoubleClickTrigger( object sender, MouseButtonEventArgs e ) {
      var listView = sender as ListView;
      var command = listView?.GetValue( EditingCommandProperty ) as ICommand;
      if ( command == null ) {
        return;
      }
      if ( listView.SelectedItem == null ) {
        return;
      }
      command.Execute( listView.SelectedItem );
    }
  }
}
