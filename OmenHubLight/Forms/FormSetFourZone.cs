using System;
using System.Windows.Forms;
using Hp.Omen.FourZoneModule.Models;
using Hp.Omen.OmenCommonLib;
using OmenHubLight.Properties;

namespace OmenHubLight.Forms
{
    public partial class FormSetFourZone : Form
    {
        private static OmenHsaClient _hsaClient;
        private byte[] _colorBytes;

        public FormSetFourZone()
        {
            InitializeComponent();
            _hsaClient ??= new OmenHsaClient();
        }

        private void FormSetFourZone_Load(object sender, EventArgs e)
        {
            checkBoxEnable.Checked = _hsaClient.GetKeyboardBrightness() > 0;
            _colorBytes = _hsaClient.BiosWmiCmd_GetColor();
            if (checkBoxEnable.Checked)
            {
                var colors = FourZoneHelper.ParseColorArray(_colorBytes);
                colorBox1.BackColor = colors[0];
                colorBox2.BackColor = colors[1];
                colorBox3.BackColor = colors[2];
                colorBox4.BackColor = colors[3];
            }
            else
            {
                var colors = Settings.Default.FourZoneColorArray;
                colors ??= FourZoneHelper.ParseColorArray(_hsaClient.BiosWmiCmd_GetColor());
                colorBox1.BackColor = colors[0];
                colorBox2.BackColor = colors[1];
                colorBox3.BackColor = colors[2];
                colorBox4.BackColor = colors[3];
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            _hsaClient.SetKeyboardBrightness((byte) (checkBoxEnable.Checked ? 228 : 100));
            FourZoneHelper.SetColorArray(new[]
            {
                colorBox1.BackColor,
                colorBox2.BackColor,
                colorBox3.BackColor,
                colorBox4.BackColor,
            }, ref _colorBytes);

            _hsaClient.BiosWmiCmd_SetColor(_colorBytes);

            if (checkBoxEnable.Checked)
            {
                Settings.Default.FourZoneColorArray = new[]
                {
                    colorBox1.BackColor,
                    colorBox2.BackColor,
                    colorBox3.BackColor,
                    colorBox4.BackColor,
                };
                Settings.Default.Save();
            }

            Close();
        }

        private void colorBox_Click(object sender, EventArgs e)
        {
            var result = colorDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                ((PictureBox) sender).BackColor = colorDialog.Color;
            }
        }
    }
}