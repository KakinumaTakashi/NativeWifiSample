
namespace NativeWifiSample
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.LabelSSID = new System.Windows.Forms.Label();
            this.TextBoxSSID = new System.Windows.Forms.TextBox();
            this.LabelKEY = new System.Windows.Forms.Label();
            this.TextBoxKEY = new System.Windows.Forms.TextBox();
            this.ButtonConnect = new System.Windows.Forms.Button();
            this.ButtonDisconnect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LabelSSID
            // 
            this.LabelSSID.AutoSize = true;
            this.LabelSSID.Location = new System.Drawing.Point(12, 9);
            this.LabelSSID.Name = "LabelSSID";
            this.LabelSSID.Size = new System.Drawing.Size(30, 12);
            this.LabelSSID.TabIndex = 0;
            this.LabelSSID.Text = "SSID";
            // 
            // TextBoxSSID
            // 
            this.TextBoxSSID.Location = new System.Drawing.Point(48, 6);
            this.TextBoxSSID.Name = "TextBoxSSID";
            this.TextBoxSSID.Size = new System.Drawing.Size(362, 19);
            this.TextBoxSSID.TabIndex = 1;
            // 
            // LabelKEY
            // 
            this.LabelKEY.AutoSize = true;
            this.LabelKEY.Location = new System.Drawing.Point(16, 34);
            this.LabelKEY.Name = "LabelKEY";
            this.LabelKEY.Size = new System.Drawing.Size(26, 12);
            this.LabelKEY.TabIndex = 2;
            this.LabelKEY.Text = "KEY";
            // 
            // TextBoxKEY
            // 
            this.TextBoxKEY.Location = new System.Drawing.Point(48, 31);
            this.TextBoxKEY.Name = "TextBoxKEY";
            this.TextBoxKEY.Size = new System.Drawing.Size(362, 19);
            this.TextBoxKEY.TabIndex = 2;
            // 
            // ButtonConnect
            // 
            this.ButtonConnect.Location = new System.Drawing.Point(178, 62);
            this.ButtonConnect.Name = "ButtonConnect";
            this.ButtonConnect.Size = new System.Drawing.Size(113, 31);
            this.ButtonConnect.TabIndex = 3;
            this.ButtonConnect.Text = "接続";
            this.ButtonConnect.UseVisualStyleBackColor = true;
            this.ButtonConnect.Click += new System.EventHandler(this.ButtonConnect_Click);
            // 
            // ButtonDisconnect
            // 
            this.ButtonDisconnect.Location = new System.Drawing.Point(297, 62);
            this.ButtonDisconnect.Name = "ButtonDisconnect";
            this.ButtonDisconnect.Size = new System.Drawing.Size(113, 31);
            this.ButtonDisconnect.TabIndex = 4;
            this.ButtonDisconnect.Text = "切断";
            this.ButtonDisconnect.UseVisualStyleBackColor = true;
            this.ButtonDisconnect.Click += new System.EventHandler(this.ButtonDisconnect_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 105);
            this.Controls.Add(this.ButtonDisconnect);
            this.Controls.Add(this.ButtonConnect);
            this.Controls.Add(this.TextBoxKEY);
            this.Controls.Add(this.LabelKEY);
            this.Controls.Add(this.TextBoxSSID);
            this.Controls.Add(this.LabelSSID);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "NativeFifiSample";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LabelSSID;
        private System.Windows.Forms.TextBox TextBoxSSID;
        private System.Windows.Forms.Label LabelKEY;
        private System.Windows.Forms.TextBox TextBoxKEY;
        private System.Windows.Forms.Button ButtonConnect;
        private System.Windows.Forms.Button ButtonDisconnect;
    }
}

