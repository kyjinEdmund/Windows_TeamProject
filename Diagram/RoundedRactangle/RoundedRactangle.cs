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

namespace RoundedRactangle
{
    public partial class RoundedRactangle : UserControl
    {
        // 모서리 반지름 (픽셀 단위)
        private int cornerRadius = 20;

        public int CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = value;
                this.Invalidate(); // 반지름 변경 시 다시 그리기
            }
        }

        public RoundedRactangle()
        {
            InitializeComponent();
            this.BackColor = Color.Yellow;
            this.Size = new Size(150, 100);
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 둥근 사각형 경로 생성
            GraphicsPath path = GetRoundedRectanglePath(this.ClientRectangle, cornerRadius);

            // 배경색으로 채운 둥근 사각형 그리기
            using (Brush brush = new SolidBrush(this.BackColor))
            {
                g.FillPath(brush, path);
            }

            // 테두리 그리기
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawPath(pen, path);
            }

            // 텍스트 중앙에 그리기
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

        // 컨트롤을 둥근 모양으로 보이게 하기 위해 Region 설정
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GraphicsPath path = GetRoundedRectanglePath(this.ClientRectangle, cornerRadius);
            this.Region = new Region(path);
        }

        // 둥근 사각형 경로 생성 메서드
        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            // 둥근 모서리를 가진 사각형 생성
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90); // 왼쪽 위
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90); // 오른쪽 위
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90); // 오른쪽 아래
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90); // 왼쪽 아래
            path.CloseFigure();

            return path;
        }

        private void RoundedRactangle_Load(object sender, EventArgs e)
        {

        }
    }
}
