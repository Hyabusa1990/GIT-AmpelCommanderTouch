using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace AmpelCommanderTouch
{
    public partial class frm_main : Form
    {
        
        bool horn = false;
        int hornCount = 0;

        bool red = true;
        bool yellow = false;
        bool green = false;
        string Str = "";

        int time = 0;

        bool ab = true;
        bool round = true;
        bool running = false;
        bool halt = false;
        bool vorlauf = false;

        public frm_main()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(70, 70, 70);
            tp_ampel.BackColor = Color.FromArgb(70, 70, 70);
            tp_add.BackColor = Color.FromArgb(70, 70, 70);
            tp_settings.BackColor = Color.FromArgb(70, 70, 70);
        }

        private void btn_toAdd_Click(object sender, EventArgs e)
        {
            tc_switch.SelectTab("tp_add");
            tb_ampelIP.Focus();
        }


        private void btn_toAmpel_Click(object sender, EventArgs e)
        {
            tc_switch.SelectTab("tp_Ampel");
        }


        private void btn_toSettings_Click(object sender, EventArgs e)
        {
            tc_switch.SelectTab("tp_Settings");
        }

        private void btn_toAdds_Click(object sender, EventArgs e)
        {
            tc_switch.SelectTab("tp_add");
        }

        public void ampel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Möchten Sie die Ampel entfernen?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.flp_ampeln.Controls.Remove((CAmpelButton)sender);
            }
        }

        private void tim_horn_Tick(object sender, EventArgs e)
        {
            if (this.hornCount > 0 || this.horn)
            {
                if (this.horn)
                {
                    this.horn = false;
                }
                else
                {
                    this.horn = true;
                    this.hornCount--;
                }
            }
        }

        private void tim_update_Tick(object sender, EventArgs e)
        {
            foreach (Control control in this.flp_ampeln.Controls)
            {
                CAmpelButton cAmpelButton = (CAmpelButton)control;
                cAmpelButton.Click -= new EventHandler(this.ampel_Click);
                cAmpelButton.Click += new EventHandler(this.ampel_Click);
                cAmpelButton.UpdateAmpel(this.red, this.yellow, this.green, this.horn, this.Str);
            }
            if (this.red)
            {
                this.pb_red.BackColor = Color.Red;
            }
            else
            {
                this.pb_red.BackColor = Color.Transparent;
            }
            if (this.green)
            {
                this.pb_green.BackColor = Color.Chartreuse;
            }
            else
            {
                this.pb_green.BackColor = Color.Transparent;
            }
            if (this.yellow)
            {
                this.pb_yellow.BackColor = Color.Yellow;
            }
            else
            {
                this.pb_yellow.BackColor = Color.Transparent;
            }
            this.lbl_string.Text = this.Str;
        }

        private void tim_count_Tick(object sender, EventArgs e)
        {
            if (this.cb_string.Checked)
            {
                this.Str = this.tb_string.Text;
            }
            else if (this.cb_abcd.Checked)
            {
                if (this.ab)
                {
                    if (this.cb_sepABCD.Checked)
                    {
                        this.Str = "A/B  ";
                    }
                    else
                    {
                        this.Str = "AB ";
                    }
                }
                else if (this.cb_sepABCD.Checked)
                {
                    this.Str = "C/D  ";
                }
                else
                {
                    this.Str = "CD ";
                }
            }
            else
            {
                this.Str = "";
            }
            if (this.running && !this.halt)
            {
                this.btn_start.Enabled = false;
                this.btn_stop.Enabled = true;
                if (this.time > 31 && !this.vorlauf)
                {
                    this.yellow = false;
                    this.red = false;
                    this.green = true;
                }
                else if (this.time <= 31 && this.time > 1 && !this.vorlauf)
                {
                    this.yellow = true;
                    this.red = false;
                    this.green = false;
                }
                else
                {
                    this.yellow = false;
                    this.red = true;
                    this.green = false;
                }
                if (this.time > 1)
                {
                    this.time--;
                }
                else
                {
                    this.time--;
                    if (this.vorlauf)
                    {
                        this.vorlauf = false;
                        if (this.cb_time120.Checked)
                        {
                            this.time = 120;
                        }
                        else if (this.cb_time240.Checked)
                        {
                            this.time = 240;
                        }
                        else
                        {
                            this.time = (int)Convert.ToInt16(this.nud_timeEigen.Value);
                        }
                        this.hornCount = 1;
                        tim_horn.Start();
                    }
                    else if (this.round)
                    {
                        this.round = false;
                        this.hornCount = 2;
                        tim_horn.Start();
                        this.ab = !this.ab;
                        this.vorlauf = true;
                        if (this.cb_preTime10.Checked)
                        {
                            this.time = 10;
                        }
                        else if (this.cb_preTime20.Checked)
                        {
                            this.time = 20;
                        }
                        else if (this.cb_preTimeEigen.Checked)
                        {
                            this.time = (int)Convert.ToInt16(this.nud_preTimeEigen.Value);
                        }
                    }
                    else
                    {
                        this.hornCount = 3;
                        tim_horn.Start();
                        this.running = false;
                        if (this.cb_time120.Checked)
                        {
                            this.time = 120;
                        }
                        else if (this.cb_time240.Checked)
                        {
                            this.time = 240;
                        }
                        else
                        {
                            this.time = (int)Convert.ToInt16(this.nud_timeEigen.Value);
                        }
                    }
                }
            }
            else if (!this.halt)
            {
                this.btn_start.Enabled = true;
                this.btn_stop.Enabled = false;
            }
            else
            {
                this.yellow = false;
                this.red = true;
                this.green = false;
            }
            if (!this.cb_string.Checked)
            {
                this.Str += this.time;
            }
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            if (this.cb_abcd.Checked)
            {
                this.round = true;
            }
            else
            {
                this.round = false;
            }
            this.cb_string.Checked = false;
            this.vorlauf = true;
            this.hornCount = 2;
            tim_horn.Start();
            this.running = true;
            if (this.cb_preTime10.Checked)
            {
                this.time = 10;
            }
            else if (this.cb_preTime20.Checked)
            {
                this.time = 20;
            }
            else if (this.cb_preTimeEigen.Checked)
            {
                this.time = (int)Convert.ToInt16(this.nud_preTimeEigen.Value);
            }
        }

        private void btn_halt_Click(object sender, EventArgs e)
        {
            if (this.halt)
            {
                this.halt = false;
                this.btn_abcd.Enabled = true;
                this.btn_start.Enabled = true;
                this.btn_stop.Enabled = true;
                this.hornCount = 1;
            }
            else
            {
                this.halt = true;
                this.hornCount = 5;
                tim_horn.Start();
                this.btn_abcd.Enabled = false;
                this.btn_start.Enabled = false;
                this.btn_stop.Enabled = false;
            }
        }

        private void btn_abcd_Click(object sender, EventArgs e)
        {
            this.ab = !this.ab;
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            this.time = 0;
            this.vorlauf = false;
        }

        private void tb_ampelIP_TextChanged(object sender, EventArgs e)
        {
            string[] buff = tb_ampelIP.Text.Split('.');
            if (buff.Length > 0 && buff.Length < 4)
            {
                if (buff[buff.Length - 1].Length == 3)
                {
                    tb_ampelIP.Paste(".");
                }
            }
        }

        private void tb_ampelIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Back)
            {
                tb_ampelIP.Text = "";
            }

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            else
            {
                if(tb_ampelIP.Text.Length > 0)
                {
                    if(tb_ampelIP.Text[tb_ampelIP.Text.Length - 1] == '.' && e.KeyChar == '.')
                    {
                        e.Handled = true;
                    }
                    else if (tb_ampelIP.Text.Split('.').Length >= 4)
                    {
                        if (tb_ampelIP.Text.Split('.')[3].Length >= 3 || e.KeyChar == '.')
                        {
                            e.Handled = true;
                        }
                    }
                }
                else if(e.KeyChar == '.')
                {
                    e.Handled = true;
                }
                
            }
        }

        private void btn_closeCommander_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_add1_Click(object sender, EventArgs e)
        {
            if (tb_ampelIP.Text.Split('.').Length == 4) {
                if (tb_ampelIP.Text.Split('.')[3].Length < 3)
                {
                    tb_ampelIP.Paste("1");
                }
            }
            else
            {
                tb_ampelIP.Paste("1");
            }
        }

        private void btn_add2_Click(object sender, EventArgs e)
        {
            if (tb_ampelIP.Text.Split('.').Length == 4)
            {
                if (tb_ampelIP.Text.Split('.')[3].Length < 3)
                {
                    tb_ampelIP.Paste("2");
                }
            }
            else
            {
                tb_ampelIP.Paste("2");
            }
        }

        private void btn_add3_Click(object sender, EventArgs e)
        {
            if (tb_ampelIP.Text.Split('.').Length == 4)
            {
                if (tb_ampelIP.Text.Split('.')[3].Length < 3)
                {
                    tb_ampelIP.Paste("3");
                }
            }
            else
            {
                tb_ampelIP.Paste("3");
            }
        }

        private void btn_add4_Click(object sender, EventArgs e)
        {
            if (tb_ampelIP.Text.Split('.').Length == 4)
            {
                if (tb_ampelIP.Text.Split('.')[3].Length < 3)
                {
                    tb_ampelIP.Paste("4");
                }
            }
            else
            {
                tb_ampelIP.Paste("4");
            }
        }

        private void btn_add5_Click(object sender, EventArgs e)
        {
            if (tb_ampelIP.Text.Split('.').Length == 4)
            {
                if (tb_ampelIP.Text.Split('.')[3].Length < 3)
                {
                    tb_ampelIP.Paste("5");
                }
            }
            else
            {
                tb_ampelIP.Paste("5");
            }
        }

        private void btn_add6_Click(object sender, EventArgs e)
        {
            if (tb_ampelIP.Text.Split('.').Length == 4)
            {
                if (tb_ampelIP.Text.Split('.')[3].Length < 3)
                {
                    tb_ampelIP.Paste("6");
                }
            }
            else
            {
                tb_ampelIP.Paste("6");
            }
        }

        private void btn_add7_Click(object sender, EventArgs e)
        {
            if (tb_ampelIP.Text.Split('.').Length == 4)
            {
                if (tb_ampelIP.Text.Split('.')[3].Length < 3)
                {
                    tb_ampelIP.Paste("7");
                }
            }
            else
            {
                tb_ampelIP.Paste("7");
            }
        }

        private void btn_add8_Click(object sender, EventArgs e)
        {
            if (tb_ampelIP.Text.Split('.').Length == 4)
            {
                if (tb_ampelIP.Text.Split('.')[3].Length < 3)
                {
                    tb_ampelIP.Paste("8");
                }
            }
            else
            {
                tb_ampelIP.Paste("8");
            }
        }

        private void btn_add9_Click(object sender, EventArgs e)
        {
            if (tb_ampelIP.Text.Split('.').Length == 4)
            {
                if (tb_ampelIP.Text.Split('.')[3].Length < 3)
                {
                    tb_ampelIP.Paste("9");
                }
            }
            else
            {
                tb_ampelIP.Paste("9");
            }
        }

        private void btn_add0_Click(object sender, EventArgs e)
        {
            if (tb_ampelIP.Text.Split('.').Length == 4)
            {
                if (tb_ampelIP.Text.Split('.')[3].Length < 3)
                {
                    tb_ampelIP.Paste("0");
                }
            }
            else
            {
                tb_ampelIP.Paste("0");
            }
        }

        private void btn_addDel_Click(object sender, EventArgs e)
        {
            tb_ampelIP.Text = "";
        }

        private void btn_addDot_Click(object sender, EventArgs e)
        {
            if (tb_ampelIP.Text.Length > 0)
            {
                if (tb_ampelIP.Text[tb_ampelIP.Text.Length - 1] != '.')
                {
                    if (tb_ampelIP.Text.Split('.').Length < 4)
                    {
                        tb_ampelIP.Paste(".");
                    }
                }
            }
        }

        private void btn_addAmpel_Click(object sender, EventArgs e)
        {
            if (this.tb_ampelIP.Text != "")
            {
                try
                {
                    IPAddress iPAddress;
                    if (!IPAddress.TryParse(this.tb_ampelIP.Text, out iPAddress))
                    {
                        IPHostEntry hostEntry = Dns.GetHostEntry(this.tb_ampelIP.Text);
                        int num = 0;
                        while (hostEntry.AddressList[num].ToString().Contains(":"))
                        {
                            num++;
                        }
                        bool flag = IPAddress.TryParse(hostEntry.AddressList[num].ToString(), out iPAddress);
                    }
                    CAmpelButton cAmpelButton = new CAmpelButton(iPAddress);
                    cAmpelButton.Click += new EventHandler(this.ampel_Click);
                    WebRequest webRequest = WebRequest.Create("http://" + iPAddress.ToString() + "/ampel.php");
                    webRequest.Credentials = CredentialCache.DefaultCredentials;
                    HttpWebResponse httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
                    Stream responseStream = httpWebResponse.GetResponseStream();
                    StreamReader streamReader = new StreamReader(responseStream);
                    string a = streamReader.ReadToEnd();
                    streamReader.Close();
                    responseStream.Close();
                    httpWebResponse.Close();
                    if (a != "Ampel")
                    {
                        throw new Exception("Keine API gefunden");
                    }
                    bool flag2 = false;
                    foreach (Control control in this.flp_ampeln.Controls)
                    {
                        CAmpelButton cAmpelButton2 = (CAmpelButton)control;
                        if (cAmpelButton2.Address.ToString().Equals(cAmpelButton.Address.ToString()))
                        {
                            flag2 = true;
                        }
                    }
                    if (flag2)
                    {
                        throw new Exception("Ampel Existiert bereits");
                    }
                    this.flp_ampeln.Controls.Add(cAmpelButton);
                    MessageBox.Show("Ampel wurde hinzugefügt", "Ampel hinzugefügt", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tb_ampelIP.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Es konnte keine Ampel hinzugefügt werden." + Environment.NewLine + "Grund: " + ex.Message, "Keine Ampel", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
        }

        private void btn_minMax_Click(object sender, EventArgs e)
        {
            if(this.FormBorderStyle == FormBorderStyle.None)
            {
                this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                this.WindowState = FormWindowState.Normal;
                btn_minMax.Text = "Vollbild";
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                btn_minMax.Text = "Fenster Modus";
            }
        }
    }
}
