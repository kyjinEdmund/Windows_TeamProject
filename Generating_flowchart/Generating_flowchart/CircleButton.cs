using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace Generating_flowchart
{
    public partial class CircleButton : Button
    {
        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics graphics = pevent.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(Color.White);

            GraphicsPath g = new GraphicsPath();





            // 고해상도 DPI 고려
            float dpiX = graphics.DpiX;
            float dpiY = graphics.DpiY;

            // 그래픽스를 고해상도로 설정 (DPI에 맞는 크기 비율)
            graphics.Clear(Color.White);

            // 고해상도 크기에 맞게 조정
            int width = (int)(ClientSize.Width * (dpiX / 96f));
            int height = (int)(ClientSize.Height * (dpiY / 96f));





            g.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
            this.Region = new System.Drawing.Region(g);
            base.OnPaint(pevent);
            this.FlatAppearance.BorderSize = 0;

        }
    }
}
