using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace Projectors {
  /// <summary>
  ///   Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application {
    public ObservableCollection<string> AddressHistory { get; set; }

    public NameValueCollection Settings { get; set; }

    
  }
}
