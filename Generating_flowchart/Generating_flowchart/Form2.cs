using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Generating_flowchart
{
    public partial class SavingModal : Form
    {
        public string FileName { get; private set; }

        private string jsonFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyAppFiles", "json", "list.json");

        public SavingModal()
        {
            InitializeComponent();
        }

        private void SavingModal_Shown(object sender, EventArgs e)
        {
            nameTextBox.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            FileName = nameTextBox.Text.Trim();

            JArray fileArray = JArray.Parse(File.ReadAllText(jsonFilePath));

            if (IsValidFileName(FileName))
            {
                if (string.IsNullOrEmpty(FileName))
                {
                    MessageBox.Show("파일 이름을 입력하세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {

                    for (int i = 0; i < fileArray.Count; i++)
                    {
                        if (fileArray[i]["name"].ToString() == FileName)
                        {
                            MessageBox.Show("기존 파일과 이름이 겹칩니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }


                }
            }
            else
            {
                MessageBox.Show("파일 이름 형식이 잘못되었습니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void nameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Enter 키를 눌렀을 때 확인 버튼의 클릭 이벤트 호출
                button1.PerformClick();

                // 기본 Enter 키 동작 방지 (필수)
                //e.Handled = true;
                //e.SuppressKeyPress = true;
            }

        }








        //
        //
        //파일 이름 검사
        //
        //
        private bool IsValidFileName(string fileName)
        {
            // 예약된 문자 검사
            char[] invalidChars = Path.GetInvalidFileNameChars();
            if (fileName.IndexOfAny(invalidChars) >= 0)
                return false;

            // 예약된 이름 검사
            string[] reservedNames = { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
                                "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };
            if (Array.Exists(reservedNames, name => fileName.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return false;

            // 길이 검사
            if (fileName.Length > 255) // Windows에서 파일 이름의 최대 길이 제한
                return false;

            return true;
        }

        private void SavingModal_Load(object sender, EventArgs e)
        {

        }
    }
}
