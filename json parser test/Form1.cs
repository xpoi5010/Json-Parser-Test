using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;

namespace json_parser_test_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON File | *.json";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            FileStream fs = new FileStream(ofd.FileName,FileMode.Open);
            byte[] binData = new byte[fs.Length];
            fs.Read(binData, 0, binData.Length);
            fs.Close();
            string data = Encoding.UTF8.GetString(binData);
            for(int i = 0; i < 5; i++)
            {
                DateTime dt = DateTime.Now;
                TimeSpan ts1 = new TimeSpan(), ts2 = new TimeSpan();
                GC.Collect();
                //Json Parser
                dt = DateTime.Now;
                for (int j = 0; j < 3; j++)
                {
                    object obj = JsonParser.JSONDeserialize(data);
                    obj = null;
                }
                ts1 = DateTime.Now - dt;
                GC.Collect();
                //Json.NET
                dt = DateTime.Now;
                for (int j = 0; j < 3; j++)
                {
                    object obj = JsonConvert.DeserializeObject(data);
                    obj = null;
                }
                ts2 = DateTime.Now - dt;
                GC.Collect();
                Console.WriteLine($"1) JsonParser: {ts1.TotalMilliseconds: 0.000}ms Json.net: {ts2.TotalMilliseconds: 0.000}ms");
            }
            for (int i = 0; i < 5; i++)
            {
                DateTime dt = DateTime.Now;
                TimeSpan ts1 = new TimeSpan(), ts2 = new TimeSpan();
                GC.Collect();
                //Json.NET
                dt = DateTime.Now;
                for (int j = 0; j < 3; j++)
                {
                    object obj = JsonConvert.DeserializeObject(data);
                    obj = null;
                }
                ts2 = DateTime.Now - dt;
                GC.Collect();
                //Json Parser
                dt = DateTime.Now;
                for (int j = 0; j < 3; j++)
                {
                    object obj = JsonParser.JSONDeserialize(data);
                    obj = null;
                }
                ts1 = DateTime.Now - dt;
                GC.Collect();
                Console.WriteLine($"2) JsonParser: {ts1.TotalMilliseconds: 0.000}ms Json.net: {ts2.TotalMilliseconds: 0.000}ms");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON File | *.json";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            FileStream fs = new FileStream(ofd.FileName, FileMode.Open);
            byte[] binData = new byte[fs.Length];
            fs.Read(binData, 0, binData.Length);
            fs.Close();
            string data = Encoding.UTF8.GetString(binData);
            for (int i = 0; i < 5; i++)
            {
                DateTime dt = DateTime.Now;
                TimeSpan ts1 = new TimeSpan(), ts2 = new TimeSpan();
                GC.Collect();
                //Json.NET
                dt = DateTime.Now;
                for (int j = 0; j < 3; j++)
                {
                    object obj = JsonConvert.DeserializeObject(data);
                    obj = null;
                }
                ts2 = DateTime.Now - dt;
                GC.Collect();
                //Json Parser
                dt = DateTime.Now;
                for (int j = 0; j < 3; j++)
                {
                    object obj = JsonParser.JSONDeserialize(data);
                    obj = null;
                }
                ts1 = DateTime.Now - dt;
                GC.Collect();

                Console.WriteLine($"3) JsonParser: {ts1.TotalMilliseconds: 0.000}ms Json.net: {ts2.TotalMilliseconds: 0.000}ms");
            }
            for (int i = 0; i < 5; i++)
            {
                DateTime dt = DateTime.Now;
                TimeSpan ts1 = new TimeSpan(), ts2 = new TimeSpan();
                GC.Collect();
                //Json Parser
                dt = DateTime.Now;
                for (int j = 0; j < 3; j++)
                {
                    object obj = JsonParser.JSONDeserialize(data);
                    obj = null;
                }
                ts1 = DateTime.Now - dt;
                GC.Collect();
                //Json.NET
                dt = DateTime.Now;
                for (int j = 0; j < 3; j++)
                {
                    object obj = JsonConvert.DeserializeObject(data);
                    obj = null;
                }
                ts2 = DateTime.Now - dt;
                GC.Collect();

                Console.WriteLine($"4) JsonParser: {ts1.TotalMilliseconds: 0.000}ms Json.net: {ts2.TotalMilliseconds: 0.000}ms");
            }
            //object obj = JSONParser.JSONDeserialize(textBox1.Text);
            //object obj = Newtonsoft.Json.JsonConvert.DeserializeObject(textBox1.Text);
        }
    }
}