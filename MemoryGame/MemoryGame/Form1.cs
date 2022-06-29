using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryGame
{
    [Serializable]
    public partial class Form1 : Form
    {
        string nivo = "";
        bool allowClick=false;
        PictureBox firstGuess;
        Random random=new Random();
        Timer clickTimer=new Timer();
        Timer delay=new Timer() { Interval=1000};
        int delayTicks = 0;
        int time = 60;
        int pointTime = 0;
        string modeType="E";
        int timeMode = 60;
        int game = 1;
        Timer timer=new Timer() { Interval=1000};
        private string FileName;
        int gamePoints = 0;
        int points = 0;
        int totalPoints = 0;
        public Form1()
        {
            InitializeComponent();
            setRandomImages();
            HideImages();
            button2.Enabled=false;
            
        }
        private PictureBox[] pictureBoxes
        {
            get { return Controls.OfType<PictureBox>().ToArray(); }
        }
        private static IEnumerable<Image> images
        {
            get
            {
                return new Image[]
                {
                    Properties.Resources.img1,
                    Properties.Resources.img2,
                    Properties.Resources.img3,
                    Properties.Resources.img4,
                    Properties.Resources.img5,
                    Properties.Resources.img6,
                    Properties.Resources.img7,
                    Properties.Resources.img8
                };
            }
        }
        private void StartTimer()
        {
            time = timeMode;
            timer.Interval = 1000;
            timer.Start();
            timer.Tick += delegate
            {
                time--;
                if(time<58)
                {
                    button2.Enabled = true;
                }
                if (time == 0)
                {
                    disableImage();
                    mode.Enabled = true;
                    showImages();
                    if(time<10)
                    {
                        lblTime.Text = "00 : 0" + time.ToString();
                    }
                    
                    timer.Stop();
                    MessageBox.Show("Времето измина!\n Обидете се повторно!");
                   
                    button1.Enabled = false;
                }
                var ssTime = TimeSpan.FromSeconds(time);
                if(time<10)
                {
                    lblTime.Text = "00 : 0" + time.ToString();
                }
                else
                {
                    lblTime.Text = "00 : " + time.ToString();
                }
                
            };
        }

        private void ResetImages()
        {
            foreach(var img in pictureBoxes)
            {
                img.Tag = null;
                img.Visible = true;
            }
            HideImages();
            setRandomImages();
            time = timeMode;
            timer.Start();
        }
        private void setImage()
        {
            foreach (var img in pictureBoxes)
            {
                img.Tag = null;
                img.Visible = true;
            }
            HideImages();
            setRandomImages();
        }

        private void setRandomImages()
        {
            foreach(var img in images)
            {
                getFreeSlot().Tag = img;
                getFreeSlot().Tag=img;
            }
        }

        private void HideImages()
        {
            foreach(var img in pictureBoxes)
            {
                img.Image = Properties.Resources.question;
            }
        }
        private PictureBox getFreeSlot()
        {
            int num;
            do
            {
                num = random.Next(0, pictureBoxes.Count());
            }
            while (pictureBoxes[num].Tag != null);
            return pictureBoxes[num];
        }
        private void CLICK_TICK(object sender,EventArgs e)
        {
            HideImages();
            allowClick = true;
            clickTimer.Stop();
        }
        public void startGame(object sender, EventArgs e)
        {

            allowClick = true;
            mode.Enabled = false;
            enableImage();
            HideImages();
            StartTimer();
            clickTimer.Interval = 1000;
            clickTimer.Tick += CLICK_TICK;
            button1.Enabled = false;

        }
        private async void clickImage(object sender,EventArgs e)
        {
            if (!allowClick) return;
            var pic =(PictureBox)sender;
            if(firstGuess==null)
            {
                firstGuess = pic;
                pic.Image=(Image)pic.Tag;
                return;
            }
            pic.Image=(Image)pic.Tag;
            if(pic.Image==firstGuess.Image &&pic!=firstGuess)
            {
                pic.Image = (Image)pic.Tag;
                pic.Visible = firstGuess.Visible = true;
                await Task.Delay(600);
                pic.Visible=firstGuess.Visible=false;
                {
                    firstGuess = pic;
                }
                HideImages(); 
                if(modeType=="E")
                {
                    points += 10;
                    lblGamePoints.Text = "GAME POINTS: " + points.ToString();
                }
                else if(modeType=="H")
                {
                    points += 20;
                    lblGamePoints.Text = "GAME POINTS: " + points.ToString();
                }
                else if(modeType=="Ex")
                {
                    points += 30;
                    lblGamePoints.Text = "GAME POINTS: " + points.ToString();
                }
            }
            else
            {
                allowClick = false;
                clickTimer.Start();
            }
            firstGuess = null;
            if (pictureBoxes.Any(p => p.Visible)) return;
            timer.Stop();
            pointTime = time;
            var item = "";
            
            if (modeType == "E") nivo = "Easy";
            else if (modeType == "H") nivo = "Hard";
            else if (modeType == "Ex") nivo = "Expert";
            if (time<10)
            {
                item = "Game: " + game + " | Time: 00 : 0" + pointTime +" | Mode: "+nivo;
            }
            else
            {
                 item = "Game: " + game + " | Time: 00 : " + pointTime+ " | Mode: "+nivo;
            }
           
            listBox1.Items.Add(item);
            game++;
            mode.Enabled = true;
            totalPoints += points;
            lblTotalPoints.Text="TOTAL POINTS: "+totalPoints.ToString();
            lblGamePoints.Text = "GAME POINTS: 0";
            points = 0;
            
            DialogResult dg = MessageBox.Show("","Честитки победивте!\n Дали сакате да продолжите да играте на ова ниво?", MessageBoxButtons.YesNo);

            if(dg == DialogResult.Yes)
            {
                enableImage();
                Restart();
            }
            if(dg==DialogResult.No)
            {
                disableImage();
            }
            setImage();
            

            
        }
        

        private void showImages()
        {
            foreach(var img in pictureBoxes)
            {
                img.Image=(Image)img.Tag;
            }
        }
        private void disableImage()
        {
            foreach(var img in pictureBoxes)
            {
                img.Enabled=false;
            }
        }
        private void enableImage()
        {
            foreach(var img in pictureBoxes)
            {
                img.Enabled = true;
            }
        }
        public void Restart()
        {
            mode.Enabled = false;
            lblGamePoints.Text = "GAME POINTS: 0";
            ResetImages();
            enableImage();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Restart();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            lblGamePoints.Text = "GAME POINTS: 0";
            lblTotalPoints.Text = "TOTAL POINTS: 0";
            game = 1;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
         
        }

        private void easyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timeMode = 60;
            time = timeMode;
            lblTime.Text ="00 : " +time.ToString();
            modeType="E";
        }

        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timeMode = 45;
            time = timeMode;
            lblTime.Text = "00 : "+timeMode.ToString();
            modeType = "H";
        }

        private void expertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timeMode = 30;
            time = timeMode;
            lblTime.Text = "00 : " + time.ToString();
            modeType = "Ex";
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
