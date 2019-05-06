using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Common;

namespace Agent.Utils
{
    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }

    public interface IHardwareDataProvider : IDisposable
    {
        HardwareTree GetSystemInfo();
    }

    public class HardwareDataProvider : IHardwareDataProvider
    {
        private readonly Computer computer;

        public HardwareDataProvider()
        {
            UpdateVisitor updateVisitor = new UpdateVisitor();
            computer = new Computer();
            computer.Open();
            computer.MainboardEnabled = true;
            computer.CPUEnabled = true;
            computer.GPUEnabled = true;
            computer.FanControllerEnabled = true;
            computer.HDDEnabled = true;
            computer.Accept(updateVisitor);
        }

        public void Dispose()
        {
            computer.Close();
        }

        public HardwareTree GetSystemInfo()
        {
            HardwareTree tree = new HardwareTree();
            tree.DeviceName = "Computer";
            tree.Sensors = new List<SensorHW>();
            tree.Subhardware = new List<HardwareTree>();

            foreach (var hardware in computer.Hardware)
            {
                tree.Subhardware.Add(GetInfoRecursive(hardware));
            }
            return tree;
        }

        private HardwareTree GetInfoRecursive(IHardware hardware)
        {
            HardwareTree tree = new HardwareTree();
            tree.DeviceName = hardware.Name;
            tree.Sensors = new List<SensorHW>();
            tree.Subhardware = new List<HardwareTree>();

            foreach (var sensor in hardware.Sensors)
            {
                    tree.Sensors.Add(new SensorHW(sensor.Identifier.ToString(), 
                        sensor.SensorType.ToString(),
                        (float)sensor.Value));
            }

            foreach(var subhardware in hardware.SubHardware)
            {
                tree.Subhardware.Add(GetInfoRecursive(subhardware));
            }

            return tree;
        }
    }
}