using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemarkableSync.OnenoteAddin
{
    public partial class PreviewImage : UserControl
    {
        private int _pageNumber;
        public bool IsChecked
        {
            get { return chbSelected.Checked; }
        }

        public int PageNumber
        {
            get { return _pageNumber; }
        }
        public PreviewImage(Bitmap image, int pageNumber)
        {
            InitializeComponent();
            _pageNumber = pageNumber;
            lblDescription.Text = $"page {pageNumber+1}";
            pbPreview.Image = image;

        }

        

        private void ResizeImage()
        {
            //pbPreview.Image. = pbPreview.Width;
        }

        private void pbPreview_Resize(object sender, EventArgs e)
        {

        }

        private void PreviewImage_Click(object sender, EventArgs e)
        {
            ToggleChecked();
        }

        private void pbPreview_Click(object sender, EventArgs e)
        {
            ToggleChecked();
        }

        public void ToggleChecked()
        {
            SetCheckedState(!chbSelected.Checked);
        }

        public void SetCheckedState(bool checkState)
        {
            chbSelected.Checked = checkState;
            lblDescription.ForeColor = checkState ? Color.Black : Color.Red;
        }
    }
}
