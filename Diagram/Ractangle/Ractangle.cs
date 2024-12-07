using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Ractangle
{
    public partial class Ractangle : UserControl
    {
        public Ractangle()
        {
            InitializeComponent();
            this.BackColor = Color.Orange;
            this.Size = new Size(100, 50);
        }

        private void Ractangle_Load(object sender, EventArgs e)
        {

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 클라이언트 영역 가져오기
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            // 배경 그리기
            using (Brush brush = new SolidBrush(this.BackColor))
            {
                g.FillRectangle(brush, rect);
            }

            // 테두리 그리기
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawRectangle(pen, rect);
            }

            // 텍스트 중앙에 그리기
            using (Brush textBrush = new SolidBrush(this.ForeColor))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(this.Text, this.Font, textBrush, rect, sf);
            }
        }
    }
}
