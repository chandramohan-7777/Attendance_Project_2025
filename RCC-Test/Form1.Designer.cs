namespace RCC
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.start = new System.Windows.Forms.Button();
            this.listDev = new System.Windows.Forms.TextBox();
            this.list = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.host = new System.Windows.Forms.TextBox();
            this.log = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lastErr = new System.Windows.Forms.TextBox();
            this.sync = new System.Windows.Forms.Button();
            this.syncDev = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.save = new System.Windows.Forms.Button();
            this.saveDev = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.remove = new System.Windows.Forms.Button();
            this.remCard = new System.Windows.Forms.TextBox();
            this.remDev = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.addCard = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.addDev = new System.Windows.Forms.TextBox();
            this.add = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.Controls.Add(this.start, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.listDev, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.list, 3, 5);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.host, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.log, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.lastErr, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.sync, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.syncDev, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.save, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.saveDev, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.remove, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.remCard, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.remDev, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.addCard, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.addDev, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.add, 3, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(695, 367);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // start
            // 
            this.start.Dock = System.Windows.Forms.DockStyle.Fill;
            this.start.Location = new System.Drawing.Point(628, 3);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(64, 30);
            this.start.TabIndex = 30;
            this.start.Text = "Start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.start_Click);
            // 
            // listDev
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.listDev, 2);
            this.listDev.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listDev.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listDev.Location = new System.Drawing.Point(142, 183);
            this.listDev.Name = "listDev";
            this.listDev.Size = new System.Drawing.Size(480, 30);
            this.listDev.TabIndex = 29;
            this.listDev.Text = "IvaATFA249E0";
            // 
            // list
            // 
            this.list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.list.Enabled = false;
            this.list.Location = new System.Drawing.Point(628, 183);
            this.list.Name = "list";
            this.list.Size = new System.Drawing.Size(64, 30);
            this.list.TabIndex = 28;
            this.list.Text = "Run";
            this.list.UseVisualStyleBackColor = true;
            this.list.Click += new System.EventHandler(this.list_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 180);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(133, 36);
            this.label7.TabIndex = 27;
            this.label7.Text = "List";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(133, 36);
            this.label6.TabIndex = 26;
            this.label6.Text = "Server";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // host
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.host, 2);
            this.host.Dock = System.Windows.Forms.DockStyle.Fill;
            this.host.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.host.Location = new System.Drawing.Point(142, 3);
            this.host.Name = "host";
            this.host.Size = new System.Drawing.Size(480, 30);
            this.host.TabIndex = 25;
            this.host.Text = "http://it.ivatechnos.com:2888/";
            // 
            // log
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.log, 4);
            this.log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.log.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.log.Location = new System.Drawing.Point(3, 255);
            this.log.Multiline = true;
            this.log.Name = "log";
            this.log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.log.Size = new System.Drawing.Size(689, 109);
            this.log.TabIndex = 24;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 216);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(133, 36);
            this.label4.TabIndex = 23;
            this.label4.Text = "Last Error";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lastErr
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.lastErr, 3);
            this.lastErr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lastErr.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lastErr.Location = new System.Drawing.Point(142, 219);
            this.lastErr.Name = "lastErr";
            this.lastErr.Size = new System.Drawing.Size(550, 30);
            this.lastErr.TabIndex = 22;
            // 
            // sync
            // 
            this.sync.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sync.Enabled = false;
            this.sync.Location = new System.Drawing.Point(628, 147);
            this.sync.Name = "sync";
            this.sync.Size = new System.Drawing.Size(64, 30);
            this.sync.TabIndex = 16;
            this.sync.Text = "Run";
            this.sync.UseVisualStyleBackColor = true;
            this.sync.Click += new System.EventHandler(this.sync_Click);
            // 
            // syncDev
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.syncDev, 2);
            this.syncDev.Dock = System.Windows.Forms.DockStyle.Fill;
            this.syncDev.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.syncDev.Location = new System.Drawing.Point(142, 147);
            this.syncDev.Name = "syncDev";
            this.syncDev.Size = new System.Drawing.Size(480, 30);
            this.syncDev.TabIndex = 14;
            this.syncDev.Text = "IvaATFA249E0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 144);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(133, 36);
            this.label5.TabIndex = 13;
            this.label5.Text = "Sync";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // save
            // 
            this.save.Dock = System.Windows.Forms.DockStyle.Fill;
            this.save.Enabled = false;
            this.save.Location = new System.Drawing.Point(628, 111);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(64, 30);
            this.save.TabIndex = 11;
            this.save.Text = "Run";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // saveDev
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.saveDev, 2);
            this.saveDev.Dock = System.Windows.Forms.DockStyle.Fill;
            this.saveDev.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveDev.Location = new System.Drawing.Point(142, 111);
            this.saveDev.Name = "saveDev";
            this.saveDev.Size = new System.Drawing.Size(480, 30);
            this.saveDev.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 36);
            this.label3.TabIndex = 8;
            this.label3.Text = "Save";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // remove
            // 
            this.remove.Dock = System.Windows.Forms.DockStyle.Fill;
            this.remove.Enabled = false;
            this.remove.Location = new System.Drawing.Point(628, 75);
            this.remove.Name = "remove";
            this.remove.Size = new System.Drawing.Size(64, 30);
            this.remove.TabIndex = 7;
            this.remove.Text = "Run";
            this.remove.UseVisualStyleBackColor = true;
            this.remove.Click += new System.EventHandler(this.remove_Click);
            // 
            // remCard
            // 
            this.remCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.remCard.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.remCard.Location = new System.Drawing.Point(385, 75);
            this.remCard.Name = "remCard";
            this.remCard.Size = new System.Drawing.Size(237, 30);
            this.remCard.TabIndex = 6;
            this.remCard.Text = "1234567890";
            // 
            // remDev
            // 
            this.remDev.Dock = System.Windows.Forms.DockStyle.Fill;
            this.remDev.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.remDev.Location = new System.Drawing.Point(142, 75);
            this.remDev.Name = "remDev";
            this.remDev.Size = new System.Drawing.Size(237, 30);
            this.remDev.TabIndex = 5;
            this.remDev.Text = "IvaATFA249E0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 36);
            this.label2.TabIndex = 4;
            this.label2.Text = "Remove";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // addCard
            // 
            this.addCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addCard.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addCard.Location = new System.Drawing.Point(385, 39);
            this.addCard.Name = "addCard";
            this.addCard.Size = new System.Drawing.Size(237, 30);
            this.addCard.TabIndex = 2;
            this.addCard.Text = "1234567890";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 36);
            this.label1.TabIndex = 0;
            this.label1.Text = "Add";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // addDev
            // 
            this.addDev.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addDev.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addDev.Location = new System.Drawing.Point(142, 39);
            this.addDev.Name = "addDev";
            this.addDev.Size = new System.Drawing.Size(237, 30);
            this.addDev.TabIndex = 1;
            this.addDev.Text = "IvaATFA249E0";
            // 
            // add
            // 
            this.add.Dock = System.Windows.Forms.DockStyle.Fill;
            this.add.Enabled = false;
            this.add.Location = new System.Drawing.Point(628, 39);
            this.add.Name = "add";
            this.add.Size = new System.Drawing.Size(64, 30);
            this.add.TabIndex = 3;
            this.add.Text = "Run";
            this.add.UseVisualStyleBackColor = true;
            this.add.Click += new System.EventHandler(this.add_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 367);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Access Control Tester";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox lastErr;
        private System.Windows.Forms.Button sync;
        private System.Windows.Forms.TextBox syncDev;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.TextBox saveDev;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button remove;
        private System.Windows.Forms.TextBox remCard;
        private System.Windows.Forms.TextBox remDev;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox addCard;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox addDev;
        private System.Windows.Forms.Button add;
        private System.Windows.Forms.TextBox log;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox host;
        private System.Windows.Forms.TextBox listDev;
        private System.Windows.Forms.Button list;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button start;
    }
}

