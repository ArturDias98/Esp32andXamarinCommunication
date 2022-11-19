using BluetoothApp.Services.Protocol;
using BluetoothApp.ViewModels;
using Plugin.BluetoothClassic.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BluetoothApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectDeviceView : ContentPage
    {
        private ProtocolDecode _protocolDecode;
        private bool _isConnected;
        private IBluetoothManagedConnection _currentConnection;
        public SelectDeviceView()
        {
            InitializeComponent();

            BindingContext = new SelectDeviceViewModel();
            UpdateUI();

            _protocolDecode = new ProtocolDecode(0xAB, 0xCD, 0xAF, 0xCF);
            _protocolDecode.OnDataFromatedEvent += OnDataFormatted;
        }
        private void UpdateUI()
        {
            var device = BindingContext as SelectDeviceViewModel;
            var adapter = device.BluetoothAdapter;
            if (adapter != null)
            {
                if (!adapter.Enabled)
                {
                    adapter.Enable();
                    device.FindDevices();
                }
            }
        }
        private async void btnConnect_Clicked(object sender, EventArgs e)
        {
            var device = BindingContext as SelectDeviceViewModel;
            var selectedDevice = device.SelectedDevice;
            if (selectedDevice != null)
            {
                if (!_isConnected)
                {
                    var connected = await TryConnect(selectedDevice, device.BluetoothAdapter);
                    if (connected)
                    {
                        btnConnect.Text = "Desconectar";
                        _isConnected = true;
                        lvBondedDevices.IsEnabled = false;
                    }
                }
                else
                {
                    try
                    {
                        _currentConnection.Dispose();
                        btnConnect.Text = "Conectar";
                        _isConnected = false;
                        lvBondedDevices.IsEnabled = true;
                        device.DataReceived = 0;
                    }
                    catch (Exception ex)
                    {

                        await DisplayAlert("Error", ex.Message, "Close");
                    }
                }
            }
        }

        private async Task<bool> TryConnect(BluetoothDeviceModel device, IBluetoothAdapter adapter)
        {
            _currentConnection = adapter.CreateManagedConnection(device);
            try
            {
                _currentConnection.Connect();
                _currentConnection.OnRecived += Connection_OnRecived;
                return true;
            }
            catch (Exception exception)
            {
                await DisplayAlert("Generic error", exception.Message, "Close");
                return false;
            }
        }

        private void Connection_OnRecived(object sender, RecivedEventArgs recivedEventArgs)
        {
            var data = recivedEventArgs.Buffer.ToArray();
            _protocolDecode.Add(data);
        }

        private void OnDataFormatted(IEnumerable<byte> data)
        {
            var model = BindingContext as SelectDeviceViewModel;
            var convert = BitConverter.ToInt32(data.ToArray(), 0);
            model.DataReceived = convert;
        }
    }
}