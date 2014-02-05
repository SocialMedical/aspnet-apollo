namespace Rp3.Security.Encryptor
{
    partial class FormEncryptor
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
            this.radioButtonEsquema1 = new System.Windows.Forms.RadioButton();
            this.radioButtonEsquema2 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxPlainText = new System.Windows.Forms.TextBox();
            this.textBoxEncryptedText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonEncrypt = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonGenerateKeyFile = new System.Windows.Forms.Button();
            this.textBoxGenerate = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButtonEsquema1
            // 
            this.radioButtonEsquema1.AutoSize = true;
            this.radioButtonEsquema1.Checked = true;
            this.radioButtonEsquema1.Location = new System.Drawing.Point(12, 12);
            this.radioButtonEsquema1.Name = "radioButtonEsquema1";
            this.radioButtonEsquema1.Size = new System.Drawing.Size(78, 17);
            this.radioButtonEsquema1.TabIndex = 0;
            this.radioButtonEsquema1.TabStop = true;
            this.radioButtonEsquema1.Text = "Esquema 1";
            this.radioButtonEsquema1.UseVisualStyleBackColor = true;
            // 
            // radioButtonEsquema2
            // 
            this.radioButtonEsquema2.AutoSize = true;
            this.radioButtonEsquema2.Location = new System.Drawing.Point(96, 12);
            this.radioButtonEsquema2.Name = "radioButtonEsquema2";
            this.radioButtonEsquema2.Size = new System.Drawing.Size(78, 17);
            this.radioButtonEsquema2.TabIndex = 1;
            this.radioButtonEsquema2.Text = "Esquema 2";
            this.radioButtonEsquema2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Texto a Encriptar";
            // 
            // textBoxPlainText
            // 
            this.textBoxPlainText.Location = new System.Drawing.Point(106, 38);
            this.textBoxPlainText.Name = "textBoxPlainText";
            this.textBoxPlainText.Size = new System.Drawing.Size(437, 20);
            this.textBoxPlainText.TabIndex = 3;
            // 
            // textBoxEncryptedText
            // 
            this.textBoxEncryptedText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxEncryptedText.Location = new System.Drawing.Point(106, 74);
            this.textBoxEncryptedText.Name = "textBoxEncryptedText";
            this.textBoxEncryptedText.ReadOnly = true;
            this.textBoxEncryptedText.Size = new System.Drawing.Size(437, 20);
            this.textBoxEncryptedText.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Resultado";
            // 
            // buttonEncrypt
            // 
            this.buttonEncrypt.Location = new System.Drawing.Point(549, 38);
            this.buttonEncrypt.Name = "buttonEncrypt";
            this.buttonEncrypt.Size = new System.Drawing.Size(75, 23);
            this.buttonEncrypt.TabIndex = 4;
            this.buttonEncrypt.Text = "Encyptar";
            this.buttonEncrypt.UseVisualStyleBackColor = true;
            this.buttonEncrypt.Click += new System.EventHandler(this.buttonEncrypt_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxGenerate);
            this.groupBox1.Controls.Add(this.buttonGenerateKeyFile);
            this.groupBox1.Location = new System.Drawing.Point(12, 110);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(531, 53);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Generate Key File";
            // 
            // buttonGenerateKeyFile
            // 
            this.buttonGenerateKeyFile.Location = new System.Drawing.Point(168, 23);
            this.buttonGenerateKeyFile.Name = "buttonGenerateKeyFile";
            this.buttonGenerateKeyFile.Size = new System.Drawing.Size(75, 23);
            this.buttonGenerateKeyFile.TabIndex = 0;
            this.buttonGenerateKeyFile.Text = "Generate";
            this.buttonGenerateKeyFile.UseVisualStyleBackColor = true;
            this.buttonGenerateKeyFile.Click += new System.EventHandler(this.buttonGenerateKeyFile_Click);
            // 
            // textBoxGenerate
            // 
            this.textBoxGenerate.Location = new System.Drawing.Point(11, 24);
            this.textBoxGenerate.Name = "textBoxGenerate";
            this.textBoxGenerate.Size = new System.Drawing.Size(151, 20);
            this.textBoxGenerate.TabIndex = 1;
            // 
            // FormEncryptor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(639, 175);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonEncrypt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxEncryptedText);
            this.Controls.Add(this.textBoxPlainText);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radioButtonEsquema2);
            this.Controls.Add(this.radioButtonEsquema1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormEncryptor";
            this.Text = "Encryptor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonEsquema1;
        private System.Windows.Forms.RadioButton radioButtonEsquema2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPlainText;
        private System.Windows.Forms.TextBox textBoxEncryptedText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonEncrypt;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxGenerate;
        private System.Windows.Forms.Button buttonGenerateKeyFile;
    }
}

