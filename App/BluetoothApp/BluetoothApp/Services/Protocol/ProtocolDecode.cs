using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BluetoothApp.Services.Protocol
{
    public class ProtocolDecode
    {
        private object _sync = new object();
        private const int BUFFER_MAX_LENGTH = 500;
        private List<byte> buffer;
        private List<byte> dataFrame;
        private int _header1;
        private int _header2;
        private int _tail1;
        private int _tail2;
        private int _datasize;
        public ProtocolDecode(int header1, int header2, int tail1, int tail2, int datasize)
        {
            _header1 = header1;
            _header2 = header2;
            _tail1 = tail1;
            _tail2 = tail2;

            _datasize = datasize;

            buffer = new List<byte>(BUFFER_MAX_LENGTH);
            dataFrame = new List<byte>();  
        }

        private void Decode()
        {
            lock (_sync)
            {
                if (FindHeader())
                {
                    if (FindTail())
                    {
                        FormatedData();
                    }
                }
            }
        }
        private bool FindHeader()
        {
            var buffersize = buffer.Count;
            if (buffersize < _datasize) return false;

            for (int i = 0; i < buffersize; i++)
            {
                if ((buffer.Count < _datasize)) return false;

                if (buffer[0] == _header1 && buffer[1] == _header2)
                {
                    buffer.RemoveAt(0);
                    buffer.RemoveAt(0);
                    return true;
                }
                else
                {
                    buffer.RemoveAt(0);
                }
            }
            return false;
        }
        private bool FindTail()
        {
            var buffersize = buffer.Count;
            if (buffersize < _datasize) return false;

            for (int i = 0; i < buffersize; i++)
            {
                if (buffer.Count < _datasize)
                {
                    dataFrame.Clear();
                    return false;
                }

                if ((buffer[0] == _tail1 && buffer[1] == _tail2) && dataFrame.Count == _datasize)
                {
                    buffer.RemoveAt(0);
                    buffer.RemoveAt(0);

                    return true;
                }
                else
                {
                    dataFrame.Add(buffer[0]);
                    buffer.RemoveAt(0);
                }
            }
            dataFrame.Clear();
            return false;
        }
        private void FormatedData()
        {

            int dataFormatted = BitConverter.ToInt16(dataFrame.ToArray(), 0);
            dataFrame.Clear();
            buffer.Clear();
            OnDataFromatedEvent?.Invoke(dataFormatted);
        }

        public Action<double> OnDataFromatedEvent;

        public void Add(IEnumerable<byte> data)
        {
            buffer.AddRange(data);
            Decode();
        }
    }
}
