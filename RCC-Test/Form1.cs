using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RCC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void add_Click(object sender, EventArgs e)
        {
            var r = dev.Add(addDev.Text, addCard.Text);
            lastErr.Text = dev.LastErr;
            log.Text = "Add(" + addDev.Text + "," + addCard.Text + ")=" + r
                + (r ? "" : ", " + dev.LastErr)
                + Environment.NewLine +
                log.Text;
        }

        private void remove_Click(object sender, EventArgs e)
        {
            var r = dev.Del(remDev.Text, remCard.Text);
            lastErr.Text = dev.LastErr;
            log.Text = "Del(" + remDev.Text + "," + remCard.Text + ")=" + r
                + (r ? "" : ", " + dev.LastErr)
                + Environment.NewLine +
                log.Text;
        }

        private void save_Click(object sender, EventArgs e)
        {
            var r = dev.Save(saveDev.Text);
            lastErr.Text = dev.LastErr;
            log.Text = "Save(" + saveDev.Text + ")=" + r
                + (r ? "" : ", " + dev.LastErr)
                + Environment.NewLine +
                log.Text;
        }

        private void sync_Click(object sender, EventArgs e)
        {
            var r = dev.Sync(syncDev.Text);
            lastErr.Text = dev.LastErr;
            log.Text = "Sync(" + syncDev.Text + ")=" + r
                + (r ? "" : ", " + dev.LastErr)
                + Environment.NewLine +
                log.Text;
        }

        private void list_Click(object sender, EventArgs e)
        {
            var r = dev.List(listDev.Text);
            lastErr.Text = dev.LastErr;
            log.Text = "List(" + listDev.Text + ")="
                + (r == null ? "null, " + dev.LastErr : string.Join(",", r))
                + Environment.NewLine +
                log.Text;
        }
        RCC.DevController dev;
        private void start_Click(object sender, EventArgs e)
        {
            dev = new RCC.DevController(host.Text);
            start.Enabled = false;
            add.Enabled = true;
            remove.Enabled = true;
            save.Enabled = true;
            sync.Enabled = true;
            list.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            dev?.Dispose();
        }
    }
}
