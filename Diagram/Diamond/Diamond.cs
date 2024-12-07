using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diamond
{
    public partial class Diamond : UserControl
    {
        public Diamond()
        {
            InitializeComponent();
            this.BackColor = Color.Blue;
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 컨트롤의 크기를 가져옵니다.
            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            // 마름모 좌표 계산
            Point[] diamondPoints = new Point[]
            {
                new Point(width / 2, 0), // 위쪽 꼭짓점
                new Point(width, height / 2), // 오른쪽 꼭짓점
                new Point(width / 2, height), // 아래쪽 꼭짓점
                new Point(0, height / 2), // 왼쪽 꼭짓점
            };

            // 배경색으로 채워진 마름모 그리기
            using (Brush brush = new SolidBrush(this.BackColor))
            {
                g.FillPolygon(brush, diamondPoints);
            }

            // 테두리 그리기
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawPolygon(pen, diamondPoints);
            }

            //텍스트 중앙에 그리기
            using (Brush textBrush = new SolidBrush(this.ForeColor))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(this.Text, this.Font, textBrush, this.ClientRectangle, sf);
            }
        }

        // 컨트롤을 마름모 형태로 보이게 하기 위해 Region 설정
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(new Point[]
                {
                    new Point(this.Width / 2, 0),
                    new Point(this.Width, this.Height / 2),
                    new Point(this.Width / 2, this.Height),
                    new Point(0, this.Height / 2),
                });
                this.Region = new Region(path);
            }
        }
    }
}
