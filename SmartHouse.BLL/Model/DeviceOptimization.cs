using SmartHouse.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.BLL.Model
{
    [Serializable]
    public class DeviceOptimization
    {
        public Device Device { set; get; }
        public bool IsAvailable { set; get; }

        public DeviceOptimization(Device device, bool isAvailable)
        {
            Device = device;
            IsAvailable = isAvailable;
        }

        public DeviceOptimization(Device device)
        {
            Device = device;
            IsAvailable = false;
        }
    }
}
