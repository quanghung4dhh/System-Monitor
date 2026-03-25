using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media; // Thêm dòng này
using System.Windows.Threading;

namespace System_Monitor.Components {
  /// <summary>
  /// Interaction logic for Info.xaml
  /// </summary>
  public partial class Info : UserControl {
    public Info() {
      InitializeComponent();
      this.Loaded += SetInfo;
      this.Unloaded += Info_Unloaded;
    }

    private DispatcherTimer _timer;



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

    public string DeviceName {
      get { return (string)GetValue(DeviceNameProperty); }
      set { SetValue(DeviceNameProperty, value); }
    }

    // Using a DependencyProperty as the backing store for DeviceName.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DeviceNameProperty =
        DependencyProperty.Register(nameof(DeviceName), typeof(string), typeof(Info), new PropertyMetadata("X"));

    public string Card { get; set; }

    private void SetInfo(object sender, RoutedEventArgs e) {
      UpdateData();
      _timer = new DispatcherTimer();
      _timer.Interval = TimeSpan.FromSeconds(1);
      _timer.Tick += (s, ev) => {
        UpdateData(); // Mỗi giây gọi hàm lấy dữ liệu 1 lần
      };
      _timer.Start();
    }

    private void UpdateData() {
      // 1. Kích hoạt Trạm phát Wifi cập nhật phần cứng (đã có chống spam)
      HardwareService.Instance.RefreshData();

      // 2. Lấy gói dữ liệu dựa theo chữ "CPU", "RAM" hoặc "GPU"
      var data = HardwareService.Instance.GetInfo(this.Card);

      // 3. Đổ dữ liệu thật vào các Property để hiển thị lên màn hình
      this.DeviceName = data.Name;
      this.Usage = data.Usage;
      this.Temp = data.Temp;
    }

    private void Info_Unloaded(object sender, RoutedEventArgs e) {
      _timer?.Stop();
    }

  }
  public class UsageToColorConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value is int usage) {
        if (usage >= 80) return Brushes.Red;       // Quá tải -> Báo Đỏ
        if (usage >= 50) return Brushes.DarkOrange;// Bắt đầu nóng -> Báo Cam
        return Brushes.DeepSkyBlue;                // Bình thường -> Xanh dương
      }
      return Brushes.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }
  }
}
