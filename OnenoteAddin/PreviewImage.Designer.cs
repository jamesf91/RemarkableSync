namespace RemarkableSync.OnenoteAddin
{
    partial class PreviewImage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblDescription = new System.Windows.Forms.Label();
            this.pnlScrollContainer = new System.Windows.Forms.Panel();
            this.pbPreview = new System.Windows.Forms.PictureBox();
            this.chbSelected = new System.Windows.Forms.CheckBox();
            this.pnlScrollContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDescription
            // 
            this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(50, 180);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(40, 13);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "page 1";
            // 
            // pnlScrollContainer
            // 
            this.pnlScrollContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlScrollContainer.AutoScroll = true;
            this.pnlScrollContainer.Controls.Add(this.pbPreview);
            this.pnlScrollContainer.Location = new System.Drawing.Point(-1, -1);
            this.pnlScrollContainer.Name = "pnlScrollContainer";
            this.pnlScrollContainer.Size = new System.Drawing.Size(140, 179);
            this.pnlScrollContainer.TabIndex = 2;
            // 
            // pbPreview
            // 
            this.pbPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbPreview.Location = new System.Drawing.Point(0, 0);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.Size = new System.Drawing.Size(140, 179);
            this.pbPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPreview.TabIndex = 1;
            this.pbPreview.TabStop = false;
            this.pbPreview.Click += new System.EventHandler(this.pbPreview_Click);
            this.pbPreview.Resize += new System.EventHandler(this.pbPreview_Resize);
            // 
            // chbSelected
            // 
            this.chbSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chbSelected.AutoSize = true;
            this.chbSelected.Checked = true;
            this.chbSelected.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbSelected.Location = new System.Drawing.Point(3, 181);
            this.chbSelected.Name = "chbSelected";
            this.chbSelected.Size = new System.Drawing.Size(15, 14);
            this.chbSelected.TabIndex = 3;
            this.chbSelected.UseVisualStyleBackColor = true;
            // 
            // PreviewImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.chbSelected);
            this.Controls.Add(this.pnlScrollContainer);
            this.Controls.Add(this.lblDescription);
            this.Name = "PreviewImage";
            this.Size = new System.Drawing.Size(138, 196);
            this.Click += new System.EventHandler(this.PreviewImage_Click);
            this.pnlScrollContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Panel pnlScrollContainer;
        private System.Windows.Forms.PictureBox pbPreview;
        private System.Windows.Forms.CheckBox chbSelected;
    }
}
