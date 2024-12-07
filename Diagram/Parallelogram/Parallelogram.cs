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

namespace Parallelogram
{
    public partial class Parallelogram : UserControl
    {
        public Parallelogram()
        {
            InitializeComponent();
            this.BackColor = Color.LightGray; // 기본 배경색
            this.Size = new Size(150, 100);   // 기본 크기 설정
        }
        // 평행사변형의 기울기 설정
        private int skewAmount = 20;

        public int SkewAmount
        {
            get => skewAmount;
            set
            {
                skewAmount = value;
                this.Invalidate(); // 기울기 변경 시 다시 그리기
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 평행사변형 경로 생성
            GraphicsPath path = GetParallelogramPath(this.ClientRectangle, skewAmount);

            // 배경색으로 채운 평행사변형 그리기
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

        // 컨트롤을 평행사변형 모양으로 보이게 하기 위해 Region 설정
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GraphicsPath path = GetParallelogramPath(this.ClientRectangle, skewAmount);
            this.Region = new Region(path);
        }

        // 평행사변형 경로 생성 메서드
        private GraphicsPath GetParallelogramPath(Rectangle rect, int skew)
        {
            GraphicsPath path = new GraphicsPath();

            // 평행사변형의 네 꼭짓점 계산
            Point[] points = new Point[]
            {
            new Point(rect.Left + skew, rect.Top), // 왼쪽 위
            new Point(rect.Right, rect.Top),      // 오른쪽 위
            new Point(rect.Right - skew, rect.Bottom), // 오른쪽 아래
            new Point(rect.Left, rect.Bottom)     // 왼쪽 아래
            };

            path.AddPolygon(points);
            return path;
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
