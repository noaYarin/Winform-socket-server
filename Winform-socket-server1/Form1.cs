using System;
using System.Drawing;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.IO;

namespace Winform_socket_server1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Server is starting...");
            byte[] data = new byte[1024];
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 9050);

            Socket newsock = new Socket(AddressFamily.InterNetwork,
                            SocketType.Stream, ProtocolType.Tcp);

            newsock.Bind(ip);
            newsock.Listen(10);
            Console.WriteLine("Waiting for a client...");

            Socket client = newsock.Accept();
            IPEndPoint newclient = (IPEndPoint)client.RemoteEndPoint;
            Console.WriteLine("Connected with {0} at port {1}",
                            newclient.Address, newclient.Port);

            while (true)
            {
                data = ReceiveData(client);
                MemoryStream memoryStream = new MemoryStream(data);
                
                try
                {
                    Image bmp = Image.FromStream(memoryStream);
                    Console.WriteLine(bmp);
                }
                catch (ArgumentException )
                {
                    Console.WriteLine("something broke");
                }
                if (data.Length == 0)
                    newsock.Listen(10);

                client.Close();
                newsock.Close();
            }

        }

        private static byte[] ReceiveData(Socket socket)
        {
            Console.WriteLine("Im building the data");

            int total = 0;
            int imageRecived;
            byte[] datasize = new byte[1024];

            imageRecived = socket.Receive(datasize, 0, 1024, 0);
            int size = BitConverter.ToInt32(datasize, 0);
            int dataleft = size;
            byte[] data = new byte[size];


            while (total < size)
            {
                imageRecived = socket.Receive(data, total, dataleft, 0);
                if (imageRecived == 0)
                {
                    break;
                }
                total += imageRecived;
                dataleft -= imageRecived;
            }
            Console.WriteLine("I finished the build the data");

            return data;

        }

    }
}
