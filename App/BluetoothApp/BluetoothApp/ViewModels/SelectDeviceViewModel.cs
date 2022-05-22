
using Plugin.BluetoothClassic.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace BluetoothApp.ViewModels
{
    public class SelectDeviceViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BluetoothDeviceModel> _deviceList;
        private BluetoothDeviceModel _selectedDevice;
        private double _dataReceived;

        public SelectDeviceViewModel()
        {
            BluetoothAdapter = DependencyService.Resolve<IBluetoothAdapter>();

            FindDevices();
        }

        public void FindDevices()
        {
            DeviceList = new ObservableCollection<BluetoothDeviceModel>();
            if (BluetoothAdapter != null)
            {
                foreach (var item in BluetoothAdapter.BondedDevices)
                {
                    DeviceList.Add(item);
                }
            }
        }

        public ObservableCollection<BluetoothDeviceModel> DeviceList
        {
            get
            {
                return _deviceList;
            }
            set
            {
                _deviceList = value;
                OnPropertyChanged(nameof(DeviceList));
            }
        }
        public BluetoothDeviceModel SelectedDevice
        {
            get
            {
                return _selectedDevice;
            }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));
            }
        }
        public double DataReceived
        {
            get
            {
                return _dataReceived;
            }
            set
            {
                _dataReceived = value;
                OnPropertyChanged(nameof(DataReceived));
            }
        }

        public readonly IBluetoothAdapter BluetoothAdapter;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
