using System.Collections.Generic;

namespace OpenHardwareMonitor.Hardware {
  public interface IHasTempSensor : IHardware {
    IEnumerable<ISensor> GetTempSensors();
  }
}
