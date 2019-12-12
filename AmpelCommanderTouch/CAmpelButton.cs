using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Specialized;
using Newtonsoft.Json;


namespace AmpelCommanderTouch
{
    class CAmpelButton : Button
    {
        private class Parameter
        {
            private bool red;

            private bool yellow;

            private bool green;

            private bool horn;

            private string str;

            private int ampelID = 1;

            public bool Red
            {
                get
                {
                    return this.red;
                }
                set
                {
                    this.red = value;
                }
            }

            public bool Yellow
            {
                get
                {
                    return this.yellow;
                }
                set
                {
                    this.yellow = value;
                }
            }

            public bool Green
            {
                get
                {
                    return this.green;
                }
                set
                {
                    this.green = value;
                }
            }

            public bool Horn
            {
                get
                {
                    return this.horn;
                }
                set
                {
                    this.horn = value;
                }
            }

            public string Str
            {
                get
                {
                    return this.str;
                }
                set
                {
                    this.str = value;
                }
            }

            public int AmpelID
            {
                get
                {
                    return this.ampelID;
                }
                set
                {
                    this.ampelID = value;
                }
            }
        }

        private bool red;

        private bool yellow;

        private bool green;

        private bool horn;

        private string str;

        private IPAddress address;

        private int ups;

        private int upsBuffer;

        private Timer tim = new Timer();

        private BackgroundWorker bg_Worker = new BackgroundWorker();

        public bool Red
        {
            get
            {
                return this.red;
            }
            set
            {
                this.red = value;
            }
        }

        public bool Yellow
        {
            get
            {
                return this.yellow;
            }
            set
            {
                this.yellow = value;
            }
        }

        public bool Green
        {
            get
            {
                return this.green;
            }
            set
            {
                this.green = value;
            }
        }

        public bool Horn
        {
            get
            {
                return this.horn;
            }
            set
            {
                this.horn = value;
            }
        }

        public string Str
        {
            get
            {
                return this.str;
            }
            set
            {
                this.str = value;
            }
        }

        public IPAddress Address
        {
            get
            {
                return this.address;
            }
            set
            {
                this.address = value;
            }
        }

        public int Ups
        {
            get
            {
                return this.ups;
            }
            set
            {
                this.ups = value;
            }
        }

        public CAmpelButton(IPAddress pAddress)
        {
            this.address = pAddress;
            this.red = false;
            this.green = false;
            this.yellow = false;
            this.horn = false;
            this.str = "";
            this.tim.Interval = 1000;
            this.tim.Tick += new EventHandler(this.tim_Tick);
            this.tim.Enabled = true;
            this.bg_Worker.DoWork += new DoWorkEventHandler(this.bg_Worker_DoWork);
            base.Size = new Size(160, 90);
            this.BackColor = Color.DarkGray;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            Graphics graphics = pevent.Graphics;
            graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black)), base.Width / 3 * 2, 2, base.Width / 3 - 2, base.Height / 2 - 2);
            graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black)), base.Width / 3, 2, base.Width / 3, base.Height / 2 - 2);
            graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black)), 2, 2, base.Width / 3 - 2, base.Height / 2 - 2);
            if (this.red)
            {
                graphics.FillRectangle(new SolidBrush(Color.Red), 2, 2, base.Width / 3 - 2, base.Height / 2 - 2);
            }
            if (this.yellow)
            {
                graphics.FillRectangle(new SolidBrush(Color.Yellow), base.Width / 3, 2, base.Width / 3, base.Height / 2 - 2);
            }
            if (this.green)
            {
                graphics.FillRectangle(new SolidBrush(Color.Chartreuse), base.Width / 3 * 2, 2, base.Width / 3 - 2, base.Height / 2 - 2);
            }
            if (this.horn)
            {
                graphics.FillEllipse(new SolidBrush(Color.Red), base.Width / 2 - 10, base.Height / 2 - 10, 20, 20);
            }
            Font font = new Font("Arial", 16f);
            graphics.DrawString(this.str, font, new SolidBrush(Color.Red), 5f, (float)(base.Height / 2));
            graphics.DrawString(this.ups + " | " + this.address.ToString(), font, new SolidBrush(Color.Black), 5f, (float)base.Height - graphics.MeasureString("A", font).Height);
        }

        private void tim_Tick(object sender, EventArgs e)
        {
            this.ups = this.upsBuffer;
            this.upsBuffer = 0;
        }

        public void UpdateAmpel(bool pRed, bool pYellow, bool pGreen, bool pHorn, string pStr)
        {
            if (!this.bg_Worker.IsBusy)
            {
                CAmpelButton.Parameter parameter = new CAmpelButton.Parameter();
                parameter.Red = pRed;
                parameter.Yellow = pYellow;
                parameter.Green = pGreen;
                parameter.Horn = pHorn;
                parameter.Str = pStr;
                this.bg_Worker.RunWorkerAsync(parameter);
                this.upsBuffer++;
                this.Refresh();
            }
        }

        private void bg_Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            CAmpelButton.Parameter parameter = (CAmpelButton.Parameter)e.Argument;
            try
            {
                string response = "";
                using (WebClient webClient = new WebClient())
                {
                    response = Encoding.UTF8.GetString(webClient.UploadValues("http://" + this.address.ToString() + "/ampel.php", new NameValueCollection() {
                        {"act", "set"},
                        {"r" + parameter.AmpelID, parameter.Red.ToString()},
                        {"g" + parameter.AmpelID, parameter.Green.ToString()},
                        {"y" + parameter.AmpelID, parameter.Yellow.ToString()},
                        {"s1", parameter.Str.ToString()},
                        {"h1", parameter.Horn.ToString()},
                    }));
                }

                Dictionary<string, string> resp = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

                if(resp["STATUS"] == "OK")
                {
                    using (WebClient webClient = new WebClient())
                    {
                        response = Encoding.UTF8.GetString(webClient.UploadValues("http://" + this.address.ToString() + "/ampel.php", new NameValueCollection() {
                        {"act", "get"},
                        {"q", "r" + parameter.AmpelID +
                        "g" + parameter.AmpelID +
                        "y" + parameter.AmpelID +
                        "s1" +
                        "h1"},
                    }));
                    }

                    resp = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
                    this.red = Convert.ToBoolean(resp["r" + parameter.AmpelID]);
                    this.green = Convert.ToBoolean(resp["g" + parameter.AmpelID]);
                    this.yellow = Convert.ToBoolean(resp["y" + parameter.AmpelID]);
                    this.str = resp["s1"];
                    this.horn = Convert.ToBoolean(resp["h1"]);
                }
            }
            catch
            {
            }
        }
    }
}
