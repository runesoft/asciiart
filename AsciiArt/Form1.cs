using System;
using System.Drawing;
using System.Windows.Forms;
using Plain.Utilities;
using System.Linq;
using System.Text;
using System.IO;

namespace AsciiArt {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }
        private string chars = " ,.-;:_1490!\"#¤%&=?´`'*¨^~}]{€$£@qvbnmWØNMZXCBN;M:ASDHGLJKQWEYOITÅPzxcvm-asdlækgjgaqwepoitu123+4075";
        private void pictureBox1_Click(object sender, EventArgs e) {
            
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    Bitmap img = new Bitmap(dialog.FileName);
                    pictureBox1.Image = img;

                    txtOrigX.Text = img.Width.ToString();
                    txtOrigY.Text = img.Height.ToString();

                    trackBarScale.Maximum = img.Width;
                    trackBarScale.Minimum = 3;

                    if (trackBarScale.Maximum > 80) {
                        trackBarScale.Value = 80;
                    } else {
                        trackBarScale.Value = trackBarScale.Maximum;
                    }

                    // Load image
                }
            
        }

        private void Form1_Load(object sender, EventArgs e) {
            textBox1.Text = chars;
        }

        private void button1_Click(object sender, EventArgs e) {
            try {
                Brush backgroundBrush = Brushes.White;
                Brush foregroundBrush = Brushes.Black;
                Color backgroundColor = Color.White;
                Color foregroundColor = Color.Black;
                if(radioButtonWOB.Checked){
                backgroundBrush = Brushes.Black;
                foregroundBrush = Brushes.White;
                backgroundColor = Color.Black;
                foregroundColor = Color.BlanchedAlmond;
                
                }


                var orig = pictureBox1.Image;
                float scale = (float)trackBarScale.Value / orig.Width;

                float yscale = scale * ((float)trackBarStretch.Value / 100);

                txtOutY.Text = ((int)((float)pictureBox1.Image.Height * yscale)).ToString();
                var resized = orig.GetThumbnailImage((int)(orig.Width * scale), (int)(orig.Height * yscale), null, IntPtr.Zero);
                //pictureBox2.Image = resized;



                Font f = textBox2.Font;


                var imgs = textBox1.Text.ToCharArray().Do(x => {
                    var img = new Bitmap(20, 20);

                    Graphics g = Graphics.FromImage(img);
                    g.FillRectangle(backgroundBrush, 0, 0, 20, 20);
                    g.DrawString(x.ToString(), f, foregroundBrush, 0, 0);
                    g.Flush();
                    return new textpixel() { image = img, Letter = x, intensity = 0 };
                });
                float t = 0.5f;
                imgs.Do(x => {
                    int intens = 0;
                    for (int i = 0; i < x.image.Height; i++) {
                        for (int j = 0; j < x.image.Width; j++) {
                            float fl = x.image.GetPixel(j, i).GetBrightness();
                            if (x.image.GetPixel(j, i).GetBrightness() > t) {
                                intens++;
                            }
                        }
                    }
                    x.intensity = intens;
                });

                int max = imgs.Max(x => x.intensity);
                int min = imgs.Min(x => x.intensity);
                int dif = max - min;
                StringBuilder sb = new StringBuilder();
                Bitmap b = new Bitmap(resized);
                
                imgs = imgs.OrderBy(x => x.intensity);
                for (int i = 0; i < resized.Height; i++) {
                    for (int j = 0; j < resized.Width; j++) {
                        float brightness = b.GetPixel(j, i).GetBrightness();
                        int n = (int)(dif * brightness + min);
                        sb.Append(imgs.Where(x => x.intensity <= n).Last().Letter);
                    }
                    sb.AppendLine();
                }
                //pictureBox2.Image = imgs.Last().image;
                textBox2.Text = sb.ToString();
                textBox2.BackColor = backgroundColor;
                textBox2.ForeColor = foregroundColor;
                tabControl1.SelectedTab = tabPage1;
                
            } catch {
                MessageBox.Show("Could not create ASCII art");
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e) {
           
        }

        private void trackBarScale_Scroll(object sender, EventArgs e) {
            setoutXY();
        }
        private void setoutXY() {
            try {
                txtOutX.Text = trackBarScale.Value.ToString();
                float scale = trackBarScale.Value / (float)pictureBox1.Image.Width;

                float yscale = scale * ((float)trackBarStretch.Value / 100);

                txtOutY.Text = ((int)((float)pictureBox1.Image.Height * yscale)).ToString();
            } catch { 
                
            }
        }

        private void trackBarStretch_Scroll(object sender, EventArgs e) {
            setoutXY();
        }

        private void button2_Click(object sender, EventArgs e) {
            try {
                SaveFileDialog dialog = new SaveFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK) {
                    string html = "<html><head><style>body{font-family: Courier New;font-size:6px;" + ((radioButtonWOB.Checked)?"color: #FFF;background-color: #000;":"")+"}		</style>	</head><body><pre>" + textBox2.Text + "</pre></body></html>";
                    using (Stream f = dialog.OpenFile()) {
                        using (var writer = new StreamWriter(f, Encoding.UTF8)) {

                            writer.Write(html);
                            writer.Flush();
                        }
                    }
                }
            } catch { MessageBox.Show("arghhh"); }
        }


    }
    public class textpixel {
        public char Letter { get; set; }
        public Bitmap image { get; set; }
        public int intensity { get; set; }
    }
}
