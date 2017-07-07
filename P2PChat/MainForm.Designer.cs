using System;
using System.Threading;

namespace P2PChat {
    sealed partial class MainForm : UIComponent {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.addressTextBox = new System.Windows.Forms.TextBox();
            this.addressLabel = new System.Windows.Forms.Label();
            this.portLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // messageTextBox
            // 
            this.messageTextBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.messageTextBox.Location = new System.Drawing.Point(14, 14);
            this.messageTextBox.Multiline = true;
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.ReadOnly = true;
            this.messageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.messageTextBox.Size = new System.Drawing.Size(459, 263);
            this.messageTextBox.TabIndex = 0;
            // 
            // inputTextBox
            // 
            this.inputTextBox.Location = new System.Drawing.Point(14, 285);
            this.inputTextBox.Multiline = true;
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.inputTextBox.Size = new System.Drawing.Size(459, 91);
            this.inputTextBox.TabIndex = 1;
            // 
            // sendButton
            // 
            this.sendButton.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sendButton.Location = new System.Drawing.Point(386, 386);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(87, 27);
            this.sendButton.TabIndex = 2;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.OnSendButtonClick);
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(310, 386);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(55, 22);
            this.portTextBox.TabIndex = 4;
            this.portTextBox.Text = "23333";
            // 
            // addressTextBox
            // 
            this.addressTextBox.Location = new System.Drawing.Point(91, 386);
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.Size = new System.Drawing.Size(164, 22);
            this.addressTextBox.TabIndex = 5;
            this.addressTextBox.Text = "127.0.0.1";
            // 
            // addressLabel
            // 
            this.addressLabel.AutoSize = true;
            this.addressLabel.Font = new System.Drawing.Font("Consolas", 11F);
            this.addressLabel.Location = new System.Drawing.Point(14, 387);
            this.addressLabel.Name = "addressLabel";
            this.addressLabel.Size = new System.Drawing.Size(64, 18);
            this.addressLabel.TabIndex = 6;
            this.addressLabel.Text = "Address";
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Font = new System.Drawing.Font("Consolas", 11F);
            this.portLabel.Location = new System.Drawing.Point(262, 387);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(40, 18);
            this.portLabel.TabIndex = 7;
            this.portLabel.Text = "Port";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 422);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.addressLabel);
            this.Controls.Add(this.addressTextBox);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.messageTextBox);
            this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chat Local Port: ";
            this.ResumeLayout(false);
            this.PerformLayout();
            this.Closing += OnMainFormClosing;
        }

        /* declare callbacks */
        private delegate void DisplayCallback(string info, bool err);
        private delegate void ClearInputCallback();

        public void Display(string info, bool err) {

            /* if Display() is not invoked at ui thread */
            if (this.messageTextBox.InvokeRequired) {
            
                while (!this.messageTextBox.IsHandleCreated) {
                    /* 
                     * if the messageBox is disposed or disposing, 
                     * cancel the invoking 
                     */
                    if (this.messageTextBox.Disposing ||
                        this.messageTextBox.IsDisposed) {
                        return;
                    }
                }

                DisplayCallback callback = new DisplayCallback(Display);
                try {
                    this.messageTextBox.Invoke(callback, new object[] {info, false});
                }
                catch (Exception) {
                    /*  */
                }
            }

            /* else do it as usual */
            else {
                this.messageTextBox.Text += info;
            }
        }

        public string Input() {
            semaphore.WaitOne();

            if (quit) {
                return "quit";
            }

            /* produce a command and send it to background */
            string command = "to " + addressTextBox.Text +
                             ":" + portTextBox.Text +
                             " " + inputTextBox.Text;
            
            /* clear the inputTextBox */
            ClearInputCallback callback = new ClearInputCallback((() => { inputTextBox.Text = ""; }));

            inputTextBox.Invoke(callback, null);

            return command;
        }

        #endregion

        private System.Windows.Forms.TextBox messageTextBox;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.TextBox addressTextBox;
        private System.Windows.Forms.Label addressLabel;
        private System.Windows.Forms.Label portLabel;

        private Semaphore semaphore = new Semaphore(0, 1);
        private bool quit = false;
    }
}