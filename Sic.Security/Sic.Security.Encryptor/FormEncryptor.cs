using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Rp3.Security.Encryptor
{
    public partial class FormEncryptor : Form
    {
        public FormEncryptor()
        {
            InitializeComponent();
            Rp3.Security.Cryptography.KeyFileName = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "key");
        }

        private void buttonEncrypt_Click(object sender, EventArgs e)
        {            
            string plainText = this.textBoxPlainText.Text;
            string encryptedText = string.Empty;
            try
            {
                if (this.radioButtonEsquema1.Checked)
                {
                    encryptedText = Rp3.Security.Cryptography.Encrypt(plainText);
                }
                else
                {
                    encryptedText = Rp3.Security.Cryptography.EncodePassword(plainText);
                }                
            }catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            this.textBoxEncryptedText.Text = encryptedText;
        }

        private void buttonGenerateKeyFile_Click(object sender, EventArgs e)
        {
            string fileName = this.textBoxGenerate.Text;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                System.Windows.Forms.MessageBox.Show("Empty File Name");
                return;
            }

            try
            {
                string newKey = Rp3.Security.Cryptography.GenerateKey();
                Rp3.Security.Cryptography.EncryptToFile(newKey, fileName);
                System.Windows.Forms.MessageBox.Show("Success");
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
    }
}
