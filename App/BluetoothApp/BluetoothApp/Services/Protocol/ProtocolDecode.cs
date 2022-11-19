using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BluetoothApp.Services.Protocol
{
    public class ProtocolDecode
    {
        private object _sync = new object();
        private const int BUFFER_MAX_LENGTH = 4096;
        private List<byte> buffer;
        private List<byte> dataFrame;
        private int _header1;
        private int _header2;
        private int _tail1;
        private int _tail2;
        private Protocol_Status state;
        private int index_header;

        enum Protocol_Status
        {
            FIND_HEADER,
            FIND_TAIL,
        }

        public ProtocolDecode(int header1, int header2, int tail1, int tail2)
        {
            _header1 = header1;
            _header2 = header2;
            _tail1 = tail1;
            _tail2 = tail2;

            buffer = new List<byte>();
            dataFrame = new List<byte>();
        }

        private void Decode()
        {
            switch (state)
            {
                case Protocol_Status.FIND_HEADER:
                    dataFrame.Clear();
                    state = FindHeader() ? Protocol_Status.FIND_TAIL : Protocol_Status.FIND_HEADER;
                    break;

                case Protocol_Status.FIND_TAIL:
                    state = FindTail() ? Protocol_Status.FIND_HEADER : Protocol_Status.FIND_TAIL;
                    break;

                default:
                    state = Protocol_Status.FIND_HEADER;
                    break;
            }
        }
        private bool FindHeader()
        {
            var buffersize = buffer.Count;
            if (buffersize < 2) return false; //2 Headers

            if (buffer[buffer.Count - 2] == _header1 && buffer[buffer.Count - 1] == _header2)
            {
                index_header = buffer.Count - 2;
                return true;
            }

            return false;
        }
        private bool FindTail()
        {
            if (buffer[buffer.Count - 2] == _tail1 && buffer[buffer.Count - 1] == _tail2)
            {
                var index_tail = buffer.Count;
                var buffer_size = index_tail - index_header;

                var start_index = index_header + 2; //2 Headers
                var size = buffer_size - 4; //2 Headers and 2 Tails

                for (int i = 0; i < size; i++)
                {
                    dataFrame.Add(buffer[start_index + i]);
                }

                OnDataFromatedEvent?.Invoke(dataFrame);

                return true;
            }
            return false;
        }

        public Action<IEnumerable<byte>> OnDataFromatedEvent;

        public void Add(IEnumerable<byte> data)
        {
            foreach (var item in data)
            {
                buffer.Add(item);

                if (buffer.Count > BUFFER_MAX_LENGTH)
                {
                    buffer.RemoveAt(0);
                }

                Decode();
            }
        }
        public void Add(byte data)
        {
            buffer.Add(data);

            if (buffer.Count > BUFFER_MAX_LENGTH)
            {
                buffer.RemoveAt(0);
            }

            Decode();
        }
    }
}
