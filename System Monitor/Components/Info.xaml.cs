using System.Windows;
using System.Windows.Controls;

namespace System_Monitor.Components {
  /// <summary>
  /// Interaction logic for Info.xaml
  /// </summary>
  public partial class Info : UserControl {
    public Info() {
      InitializeComponent();
      this.Loaded += SetInfo;
    }





    public int Usage {
      get { return (int)GetValue(UsageProperty); }
      set { SetValue(UsageProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Usage.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty UsageProperty =
        DependencyProperty.Register(nameof(Usage), typeof(int), typeof(Info), new PropertyMetadata(0));



    public int Temp {
      get { return (int)GetValue(TempProperty); }
      set { SetValue(TempProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Temp.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TempProperty =
        DependencyProperty.Register(nameof(Temp), typeof(int), typeof(Info), new PropertyMetadata(0));



    public string Card { get; set; } = "CPU";

    private void SetInfo(object sender, RoutedEventArgs e) {
      switch (this.Card) {
        case "CPU":
          this.Usage = 45;
          this.Temp = 67;
          break;
        case "RAM":
          this.Usage = 55;
          this.Temp = 61;
          break;
        case "GPU":
          this.Usage = 90;
          this.Temp = 95;
          break;
        default:
          this.Usage = 45;
          this.Temp = 67;
          break;
      }
    }

  }
}
