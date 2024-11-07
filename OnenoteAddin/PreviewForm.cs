using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RemarkableSync.OnenoteAddin
{
    public partial class PreviewForm : Form
    {
        /*[STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PreviewForm(loadTestData()));
        }*/
        public List<int> SelectedBitmaps
        {
            get {
                List<int> _selectedBitmaps = new List<int>();
                foreach (PreviewImage pi in flpPreviewContainer.Controls)
                {
                    if (pi.IsChecked)
                    {
                        _selectedBitmaps.Add(pi.PageNumber);
                    }
                }
                return _selectedBitmaps;
            }
        }
        public PreviewForm(Dictionary<int, Bitmap> pages)
        {
            InitializeComponent();
            SetImages(pages);
            //loadTestData();
        }

        private static Dictionary<int, Bitmap> loadTestData()
        {
            Dictionary<int, Bitmap> pages = new Dictionary<int, Bitmap>();
            for (int i = 1; i < 20; i++)
            {
                pages.Add(i, new Bitmap($"C:\\dev\\reMarkableSync\\backup\\todo\\todo_ ({i}).png"));
            }
            return pages;
        }

        public void SetImages(Dictionary<int, Bitmap> pages)
        {
            foreach (var page in pages)
            {
                PreviewImage previewImage = new PreviewImage(page.Value, page.Key);
                flpPreviewContainer.Controls.Add(previewImage);
            }
        }

        private void btnOk_Click(object sender, System.EventArgs e)
        {          
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int originalWidth = 138;
            double resize = 1 + (tbResizePreviews.Value/10.0);
            int newWidth = Convert.ToInt32(originalWidth * resize);
            int newHeight = Convert.ToInt32(newWidth / 0.8166);
            foreach (PreviewImage pi in flpPreviewContainer.Controls) {
                pi.Width = newWidth;
                pi.Height = newHeight;
            }
        }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            setCheckedStateAll(true);
        }
        private void btnUncheckAll_Click(object sender, EventArgs e)
        {
            setCheckedStateAll(false);
        }


        private void setCheckedStateAll(bool checkedState)
        {
            foreach (PreviewImage pi in flpPreviewContainer.Controls)
            {
                pi.SetCheckedState(checkedState);
            }
        }
    }
}
