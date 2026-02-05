using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Synapse.Presentation.Services
{
    /// <summary>
    /// Struct representing the shared memory layout for a single battery.
    /// This struct must have a fixed size to work with MemoryMappedFiles correctly.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BatterySharedData
    {
        public double Voltage;
        public int StateOfCharge;
        public int PassCount;
        public int LastUpdateTick;
    }

    /// <summary>
    /// Manages Inter-Process Communication (IPC) via Shared Memory (MemoryMappedFile).
    /// </summary>
    public class IpcManager : IDisposable
    {
        private const string MapName = "Synapse_Battery_SharedMemory";
        private const int BatteryCount = 24;
        private readonly int _dataSize;
        private MemoryMappedFile? _mmf;
        private MemoryMappedViewAccessor? _accessor;

        public IpcManager()
        {
            _dataSize = Marshal.SizeOf<BatterySharedData>() * BatteryCount;
            // Create or Open the Shared Memory segment
            // In a real IPC scenario, one process creates, others open.
            try
            {
                _mmf = MemoryMappedFile.CreateOrOpen(MapName, _dataSize);
                _accessor = _mmf.CreateViewAccessor();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"IPC Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Writes battery data to the shared memory at the specified index.
        /// This represents a service thread publishing its state.
        /// </summary>
        public void WriteData(int index, BatterySharedData data)
        {
            if (_accessor == null || index < 0 || index >= BatteryCount) return;
            
            int offset = index * Marshal.SizeOf<BatterySharedData>();
            _accessor.Write(offset, ref data);
        }

        /// <summary>
        /// Reads battery data from the shared memory at the specified index.
        /// This represents the UI thread or another process consuming data.
        /// </summary>
        public BatterySharedData ReadData(int index)
        {
            if (_accessor == null || index < 0 || index >= BatteryCount) return default;

            int offset = index * Marshal.SizeOf<BatterySharedData>();
            BatterySharedData data;
            _accessor.Read(offset, out data);
            return data;
        }

        public void Dispose()
        {
            _accessor?.Dispose();
            _mmf?.Dispose();
        }
    }
}
