using System;
using System.Windows.Forms;

namespace P2PChat {
    public sealed partial class MainForm : Form {

        public MainForm(int port) {
            InitializeComponent();
            this.Text += port.ToString();
        }

        private void OnSendButtonClick(object sender, EventArgs e) {
            if (null == inputTextBox.Text ||
                inputTextBox.Text.Equals("")) {
                MessageBox.Show(@"Empty input!");
                return;
            }
           
            semaphore.Release();
        }

        private void OnMainFormClosing(object sender, EventArgs e) {
            quit = true;
            semaphore.Release();
        }
    }
}
