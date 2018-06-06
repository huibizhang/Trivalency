using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            go();
        }

        public void go()
        {
            if (System.IO.File.Exists(openFileDialog1.FileName))
            {
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                Bitmap b = (Bitmap)pictureBox1.Image;
                Bitmap b128 = new Bitmap(b);
                Bitmap bb = new Bitmap(b);

                int len​​gth = b.Height * 3 * b.Width;
                byte[] RGB = new byte[length];
                byte[] binary128 = new byte[length];
                BitmapData data = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                BitmapData data_b128 = b128.LockBits(new Rectangle(0, 0, b.Width, b.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                System.IntPtr Scan0 = data.Scan0;
                System.IntPtr Scan0_b128 = data_b128.Scan0;
                System.Runtime.InteropServices.Marshal.Copy(Scan0, RGB, 0, length);
                System.Runtime.InteropServices.Marshal.Copy(Scan0_b128, binary128, 0, length);
                double gray = 0;

                int[] cc = new int[5];

                int[] C = get_Color();

                for (int i = 0; i < RGB.Length; i += 3)
                {
                    //灰階
                    if (radioButton1.Checked)
                        gray = (RGB[i] + RGB[i + 1] + RGB[i + 2]) / 3;
                    else if (radioButton2.Checked){
                        gray = RGB[ i + C[0]] * 0.2990 + RGB[i + C[1]] * 0.58700 + RGB[i + C[2]] * 0.11400;
                    }
                        

                    RGB[i + 2] = RGB[i + 1] = RGB[i] = (byte)gray;

                    //二值化
                    if (RGB[i] > 127)
                        binary128[i + 2] = binary128[i + 1] = binary128[i] = 255;
                    else
                        binary128[i + 2] = binary128[i + 1] = binary128[i] = 0;

                    //五值化灰階
                    if (RGB[i] >= 204)
                        RGB[i + 2] = RGB[i + 1] = RGB[i] = 255;
                    else if (RGB[i] >= 153 && RGB[i] < 204)
                        RGB[i + 2] = RGB[i + 1] = RGB[i] = 191;
                    else if (RGB[i] >= 102 && RGB[i] < 153)
                        RGB[i + 2] = RGB[i + 1] = RGB[i] = 127;
                    else if (RGB[i] >= 51 && RGB[i] < 102)
                        RGB[i + 2] = RGB[i + 1] = RGB[i] = 63;
                    else
                        RGB[i + 2] = RGB[i + 1] = RGB[i] = 0;

                    if (RGB[i] == 255)
                        cc[4]++;
                    else if (RGB[i] == 191)
                        cc[3]++;
                    else if (RGB[i] == 127)
                        cc[2]++;
                    else if (RGB[i] == 63)
                        cc[1]++;
                    else
                        cc[0]++;
                }

                int[] coo = { 0, 63, 127, 191, 255 };
                int co = 0;
                for (int i = 0; i < cc.Length; i++)
                {
                    if (i > 0 && cc[i] > cc[i - 1])
                        co = coo[i];
                    if (i == 0)
                        co = coo[0];
                }

                for (int i = 0; i < RGB.Length; i += 3)
                {
                    if (co > 64)
                    {
                        //二值化
                        if (RGB[i] >= co)
                            RGB[i + 2] = RGB[i + 1] = RGB[i] = 255;
                        else
                            RGB[i + 2] = RGB[i + 1] = RGB[i] = 0;
                    }
                    if (RGB[i] >= 50)
                        RGB[i + 2] = RGB[i + 1] = RGB[i] = 255;
                    else
                        RGB[i + 2] = RGB[i + 1] = RGB[i] = 0;

                    if (RGB[i] != binary128[i])
                        RGB[i + 2] = RGB[i + 1] = RGB[i] = (byte)((RGB[i] + binary128[i]) / 2);
                }
                System.Runtime.InteropServices.Marshal.Copy(RGB, 0, Scan0, length);
                System.Runtime.InteropServices.Marshal.Copy(binary128, 0, Scan0_b128, length);
                b.UnlockBits(data);
                b128.UnlockBits(data_b128);
                pictureBox1.Image = bb;
                pictureBox2.Image = b;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
        }

        private void pictureBox2_DoubleClick(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null)
            {
                System.Windows.Forms.Clipboard.SetImage(pictureBox2.Image);
                MessageBox.Show("已複製到剪貼簿");
            }
        }
        private void 儲存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null)
            {
                System.Windows.Forms.Clipboard.SetImage(pictureBox2.Image);
                MessageBox.Show("已複製到剪貼簿");
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = radioButton2.Checked;
            go();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            go();
        }

        public int[] get_Color()
        {
            int[] temp = new int[3];
            int i = 0;
            switch (comboBox1.Text)
            {
                case "RGB":
                    temp = new int[] { i + 0, i + 1, i + 2 };
                    break;
                case "RBG":
                    temp = new int[] { i + 0, i + 2, i + 1 };
                    break;
                case "GRB":
                    temp = new int[] { i + 1, i + 0, i + 2 };
                    break;
                case "GBR":
                    temp = new int[] { i + 1, i + 2, i + 0 };
                    break;
                case "BRG":
                    temp = new int[] { i + 2, i + 0, i + 1 };
                    break;
                case "BGR":
                    temp = new int[] { i + 2, i + 1, i + 0 };
                    break;
            }
            return temp;
        }
    }
}
