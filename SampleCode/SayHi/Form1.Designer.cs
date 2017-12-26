namespace SayHi
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
            this.components = new System.ComponentModel.Container();
            this.btn_logIn = new System.Windows.Forms.Button();
            this.grpbx_selector = new System.Windows.Forms.GroupBox();
            this.btn_markAllAsRead = new System.Windows.Forms.Button();
            this.btn_Pause = new System.Windows.Forms.Button();
            this.txb_convId = new System.Windows.Forms.TextBox();
            this.btn_join = new System.Windows.Forms.Button();
            this.btn_create = new System.Windows.Forms.Button();
            this.txb_friend = new System.Windows.Forms.TextBox();
            this.txb_clientId = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_Send = new System.Windows.Forms.Button();
            this.txb_InputMessage = new System.Windows.Forms.TextBox();
            this.lbx_messages = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txb_logs = new System.Windows.Forms.TextBox();
            this.btn_Recall = new System.Windows.Forms.Button();
            this.btn_Modify = new System.Windows.Forms.Button();
            this.aVIMMessageBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.grpbx_selector.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aVIMMessageBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_logIn
            // 
            this.btn_logIn.Location = new System.Drawing.Point(16, 68);
            this.btn_logIn.Name = "btn_logIn";
            this.btn_logIn.Size = new System.Drawing.Size(162, 23);
            this.btn_logIn.TabIndex = 0;
            this.btn_logIn.Text = "LogIn";
            this.btn_logIn.UseVisualStyleBackColor = true;
            this.btn_logIn.Click += new System.EventHandler(this.btn_logIn_Click);
            // 
            // grpbx_selector
            // 
            this.grpbx_selector.Controls.Add(this.btn_markAllAsRead);
            this.grpbx_selector.Controls.Add(this.btn_Pause);
            this.grpbx_selector.Controls.Add(this.txb_convId);
            this.grpbx_selector.Controls.Add(this.btn_join);
            this.grpbx_selector.Controls.Add(this.btn_create);
            this.grpbx_selector.Controls.Add(this.txb_friend);
            this.grpbx_selector.Controls.Add(this.txb_clientId);
            this.grpbx_selector.Controls.Add(this.btn_logIn);
            this.grpbx_selector.Location = new System.Drawing.Point(27, 12);
            this.grpbx_selector.Name = "grpbx_selector";
            this.grpbx_selector.Size = new System.Drawing.Size(204, 418);
            this.grpbx_selector.TabIndex = 1;
            this.grpbx_selector.TabStop = false;
            this.grpbx_selector.Text = "操作框";
            // 
            // btn_markAllAsRead
            // 
            this.btn_markAllAsRead.Location = new System.Drawing.Point(16, 364);
            this.btn_markAllAsRead.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_markAllAsRead.Name = "btn_markAllAsRead";
            this.btn_markAllAsRead.Size = new System.Drawing.Size(161, 27);
            this.btn_markAllAsRead.TabIndex = 8;
            this.btn_markAllAsRead.Text = "MarkAllAsRead";
            this.btn_markAllAsRead.UseVisualStyleBackColor = true;
            this.btn_markAllAsRead.Click += new System.EventHandler(this.btn_markAllAsRead_Click);
            // 
            // btn_Pause
            // 
            this.btn_Pause.Location = new System.Drawing.Point(16, 328);
            this.btn_Pause.Name = "btn_Pause";
            this.btn_Pause.Size = new System.Drawing.Size(162, 23);
            this.btn_Pause.TabIndex = 7;
            this.btn_Pause.Text = "Pause";
            this.btn_Pause.UseVisualStyleBackColor = true;
            this.btn_Pause.Click += new System.EventHandler(this.btn_Pause_Click);
            // 
            // txb_convId
            // 
            this.txb_convId.Location = new System.Drawing.Point(16, 227);
            this.txb_convId.Name = "txb_convId";
            this.txb_convId.Size = new System.Drawing.Size(162, 20);
            this.txb_convId.TabIndex = 5;
            this.txb_convId.Text = "5940e71b8fd9c5cf89fb91b7";
            // 
            // btn_join
            // 
            this.btn_join.Location = new System.Drawing.Point(16, 264);
            this.btn_join.Name = "btn_join";
            this.btn_join.Size = new System.Drawing.Size(162, 23);
            this.btn_join.TabIndex = 4;
            this.btn_join.Text = "Join";
            this.btn_join.UseVisualStyleBackColor = true;
            this.btn_join.Click += new System.EventHandler(this.btn_join_Click);
            // 
            // btn_create
            // 
            this.btn_create.Location = new System.Drawing.Point(16, 159);
            this.btn_create.Name = "btn_create";
            this.btn_create.Size = new System.Drawing.Size(162, 23);
            this.btn_create.TabIndex = 3;
            this.btn_create.Text = "CreateConversation";
            this.btn_create.UseVisualStyleBackColor = true;
            this.btn_create.Click += new System.EventHandler(this.btn_create_Click);
            // 
            // txb_friend
            // 
            this.txb_friend.Location = new System.Drawing.Point(16, 110);
            this.txb_friend.Name = "txb_friend";
            this.txb_friend.Size = new System.Drawing.Size(162, 20);
            this.txb_friend.TabIndex = 2;
            this.txb_friend.Text = "hei";
            // 
            // txb_clientId
            // 
            this.txb_clientId.Location = new System.Drawing.Point(16, 29);
            this.txb_clientId.Name = "txb_clientId";
            this.txb_clientId.Size = new System.Drawing.Size(162, 20);
            this.txb_clientId.TabIndex = 1;
            this.txb_clientId.Text = "junwu";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_Modify);
            this.groupBox2.Controls.Add(this.btn_Recall);
            this.groupBox2.Controls.Add(this.btn_Send);
            this.groupBox2.Controls.Add(this.txb_InputMessage);
            this.groupBox2.Controls.Add(this.lbx_messages);
            this.groupBox2.Location = new System.Drawing.Point(237, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(683, 418);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "聊天记录";
            // 
            // btn_Send
            // 
            this.btn_Send.Location = new System.Drawing.Point(570, 273);
            this.btn_Send.Name = "btn_Send";
            this.btn_Send.Size = new System.Drawing.Size(107, 139);
            this.btn_Send.TabIndex = 2;
            this.btn_Send.Text = "Send";
            this.btn_Send.UseVisualStyleBackColor = true;
            this.btn_Send.Click += new System.EventHandler(this.btn_Send_Click);
            // 
            // txb_InputMessage
            // 
            this.txb_InputMessage.Location = new System.Drawing.Point(24, 328);
            this.txb_InputMessage.Multiline = true;
            this.txb_InputMessage.Name = "txb_InputMessage";
            this.txb_InputMessage.Size = new System.Drawing.Size(540, 84);
            this.txb_InputMessage.TabIndex = 1;
            this.txb_InputMessage.Text = "test";
            // 
            // lbx_messages
            // 
            this.lbx_messages.FormattingEnabled = true;
            this.lbx_messages.Location = new System.Drawing.Point(24, 29);
            this.lbx_messages.Name = "lbx_messages";
            this.lbx_messages.Size = new System.Drawing.Size(653, 238);
            this.lbx_messages.TabIndex = 0;
            this.lbx_messages.SelectedValueChanged += new System.EventHandler(this.lbx_messages_SelectedValueChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txb_logs);
            this.groupBox3.Location = new System.Drawing.Point(27, 450);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(893, 226);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "日志";
            // 
            // txb_logs
            // 
            this.txb_logs.Location = new System.Drawing.Point(16, 19);
            this.txb_logs.Multiline = true;
            this.txb_logs.Name = "txb_logs";
            this.txb_logs.Size = new System.Drawing.Size(871, 201);
            this.txb_logs.TabIndex = 0;
            // 
            // btn_Recall
            // 
            this.btn_Recall.Location = new System.Drawing.Point(24, 273);
            this.btn_Recall.Name = "btn_Recall";
            this.btn_Recall.Size = new System.Drawing.Size(132, 49);
            this.btn_Recall.TabIndex = 3;
            this.btn_Recall.Text = "Recall";
            this.btn_Recall.UseVisualStyleBackColor = true;
            this.btn_Recall.Click += new System.EventHandler(this.btn_Recall_Click);
            // 
            // btn_Modify
            // 
            this.btn_Modify.Location = new System.Drawing.Point(439, 273);
            this.btn_Modify.Name = "btn_Modify";
            this.btn_Modify.Size = new System.Drawing.Size(125, 49);
            this.btn_Modify.TabIndex = 4;
            this.btn_Modify.Text = "Modify";
            this.btn_Modify.UseVisualStyleBackColor = true;
            this.btn_Modify.Click += new System.EventHandler(this.btn_Modify_Click);
            // 
            // aVIMMessageBindingSource
            // 
            this.aVIMMessageBindingSource.DataSource = typeof(LeanCloud.Realtime.AVIMMessage);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 688);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.grpbx_selector);
            this.Name = "Form1";
            this.Text = "Form1";
            this.grpbx_selector.ResumeLayout(false);
            this.grpbx_selector.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aVIMMessageBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_logIn;
        private System.Windows.Forms.GroupBox grpbx_selector;
        private System.Windows.Forms.TextBox txb_clientId;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txb_logs;
        private System.Windows.Forms.ListBox lbx_messages;
        private System.Windows.Forms.BindingSource aVIMMessageBindingSource;
        private System.Windows.Forms.TextBox txb_friend;
        private System.Windows.Forms.Button btn_create;
        private System.Windows.Forms.TextBox txb_convId;
        private System.Windows.Forms.Button btn_join;
        private System.Windows.Forms.Button btn_Pause;
        private System.Windows.Forms.Button btn_Send;
        private System.Windows.Forms.TextBox txb_InputMessage;
        private System.Windows.Forms.Button btn_markAllAsRead;
        private System.Windows.Forms.Button btn_Recall;
        private System.Windows.Forms.Button btn_Modify;
    }
}

