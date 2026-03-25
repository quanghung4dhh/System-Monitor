using LibreHardwareMonitor.Hardware;

namespace System_Monitor {
  public class HardwareService {
    public static HardwareService Instance { get; } = new HardwareService();

    private Computer _computer;
    private DateTime _lastUpdate = DateTime.MinValue;

    public HardwareService() {
      _computer = new Computer {
        IsCpuEnabled = true,
        IsGpuEnabled = true,
        IsMemoryEnabled = true,
        IsMotherboardEnabled = true, // BẮT BUỘC PHẢI THÊM CÁI NÀY ĐỂ ĐỌC TEMP CPU!
        IsControllerEnabled = true
      };
      _computer.Open();
    }

    public void RefreshData() {
      if ((DateTime.Now - _lastUpdate).TotalSeconds < 1) return;
      foreach (var hardware in _computer.Hardware) hardware.Update();
      _lastUpdate = DateTime.Now;
    }

    public (string Name, int Usage, int Temp) GetInfo(string hardwareType) {
      IHardware hw = null;

      if (hardwareType == "CPU")
        hw = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
      else if (hardwareType == "RAM")
        // Đã sửa: Bỏ qua Virtual Memory, chỉ lấy RAM thật
        hw = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Memory && !h.Name.Contains("Virtual"));
      else if (hardwareType == "GPU")
        hw = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.GpuNvidia || h.HardwareType == HardwareType.GpuAmd);

      if (hw == null) return ("Unknown", 0, 0);

      // Đã sửa: Tự động đổi tên RAM cho đẹp, không dùng chữ "Generic Memory" mặc định nữa
      string displayName = hw.Name;
      if (hardwareType == "RAM") displayName = "Physical Memory";

      float usage = 0;
      float temp = 0;

      foreach (var sensor in hw.Sensors) {
        // -- LẤY % SỬ DỤNG --
        if (sensor.SensorType == SensorType.Load) {
          if (hardwareType == "RAM" && sensor.Name == "Memory")
            usage = sensor.Value ?? 0;
          else if (sensor.Name.Contains("Total"))
            usage = sensor.Value ?? 0;
        }
        // -- LẤY NHIỆT ĐỘ --
        else if (sensor.SensorType == SensorType.Temperature) {
          if (hardwareType == "GPU" && sensor.Name == "GPU Core")
            temp = sensor.Value ?? 0;
          else if (hardwareType == "CPU") {
            if (sensor.Name.Contains("Package") || sensor.Name.Contains("Tdie") || sensor.Name.Contains("Core Average"))
              temp = sensor.Value ?? 0;
          }
        }
      }

      return (displayName, (int)usage, (int)temp);
    }
  }
}