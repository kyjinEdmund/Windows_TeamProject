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
using System.Xml.Linq;

namespace Generating_flowchart
{
    public partial class FlowchartForm1 : Form
    {
        private Point mouseLocation;
        private bool isDrag = false;
        private FlowChatManager layout;
        private int fontSizeChange;
        private Dictionary<int, Point> nodePositions;

        public Dictionary<int, Point> NodePositions
        {
            get { return nodePositions; }
            set { nodePositions = value; }
        }
        public FlowchartForm1(List<Node> _tmp)
        {
            InitializeComponent();
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.FlowchartForm1_MouseWheelEvent);
            layout = new FlowChatManager(this, _tmp);
            layout.buildStart();
            nodePositions = layout.NodePoint;
        }

        private void FlowchartForm_Load(object sender, EventArgs e)
        {


        }

        private void FlowchartForm1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseLocation = e.Location; // 마우스 클릭 위치 저장
                isDrag = true;
            }
        }

        private void FlowchartForm1_MouseMove(object sender, MouseEventArgs e)
        {

            if (isDrag) // 드래그 중일 때만 동작
            {
                // 이동 거리 계산
                int deltaX = e.X - mouseLocation.X;
                int deltaY = e.Y - mouseLocation.Y;

                // 모든 컨트롤의 위치 업데이트
                foreach (Control control in this.Controls)
                {
                    control.Location = new Point(control.Location.X + deltaX, control.Location.Y + deltaY);

                    Point newLocation = control.Location;

                    List<int> keysToUpdate = new List<int>();

                    // 딕셔너리를 순회하며 업데이트할 키를 찾음
                    foreach (KeyValuePair<int, Point> n in layout.NodePoint)
                    {
                        if (n.Key.ToString().Equals(control.Name))
                        {
                            keysToUpdate.Add(n.Key);
                        }
                    }

                    this.Invalidate();
                    // 루프 종료 후 딕셔너리 수정
                    foreach (int key in keysToUpdate)
                    {
                        layout.NodePoint[key] = newLocation;
                    }
                }

                // 현재 마우스 위치를 다음 기준점으로 설정
                mouseLocation = e.Location;
            }
        }

        private void FlowchartForm1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrag = false;
                this.Invalidate();
            }

        }

        private void FlowchartForm1_MouseWheelEvent(object sender, MouseEventArgs e)
        {
            int fontSizeChange = e.Delta / 120; //폰트 크기가 휠 움직임에 따라 1씩 조정되게 하기 위함
            int sizeChange = fontSizeChange * 10;

            foreach (Control control in this.Controls)
            {
                if (control is Label label)
                {
                    if (label.Font.Size + fontSizeChange > 0)       //폰트 사이즈가 0보다 클 경우에만 동작하도록 작성
                    {
                        // 위치 변경
                        int newX = label.Location.X + sizeChange;
                        int newY = label.Location.Y + sizeChange;

                        label.Location = new Point(newX, newY);

                        // 폰트 크기 변경
                        label.Font = new Font(label.Font.FontFamily, label.Font.Size + fontSizeChange);
                    }
                }
                Point newLocation = control.Location;
                List<int> keysToUpdate = new List<int>();

                // 딕셔너리를 순회하며 업데이트할 키를 찾음
                foreach (KeyValuePair<int, Point> n in layout.NodePoint)
                {
                    if (n.Key.ToString().Equals(control.Name))
                    {
                        keysToUpdate.Add(n.Key);
                    }
                }

                this.Invalidate();
                // 루프 종료 후 딕셔너리 수정
                foreach (int key in keysToUpdate)
                {
                    layout.NodePoint[key] = newLocation;
                }
            }
            // 순서도 화살표 갱신을 위해 폼 다시 그리기 요청
            this.Invalidate();
        }


        private void FlowchartForm1_Paint(object sender, PaintEventArgs e)
        {
            layout.linePaint(e);
        }

        private void image로저장ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFullFlowchartAsImage();
        }

        private void FlowchartForm1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int minX = int.MaxValue, minY = int.MaxValue;

                foreach (var pos in layout.NodePoint.Values)
                {
                    minX = Math.Min(minX, pos.X);
                    minY = Math.Min(minY, pos.Y);
                }

                Console.WriteLine($"minX: {minX}, minY: {minY}");

            }
        }
        private void SaveFullFlowchartAsImage()
        {
            // SaveFileDialog 설정
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = @"C:\",
                Filter = "PNG 파일(*.png)|*.png|JPEG 파일(*.jpg)|*.jpg|모든 파일(*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 전체 플로우차트 영역 계산
                    int minX = int.MaxValue, minY = int.MaxValue;
                    int maxX = int.MinValue, maxY = int.MinValue;

                    // NodePositions를 기반으로 영역 계산
                    foreach (var pos in layout.NodePoint.Values)
                    {
                        minX = Math.Min(minX, pos.X);
                        minY = Math.Min(minY, pos.Y);
                        maxX = Math.Max(maxX, pos.X);
                        maxY = Math.Max(maxY, pos.Y);
                    }

                    // 컨트롤 크기와 위치를 고려해 전체 영역 확장
                    foreach (Control control in this.Controls)
                    {
                        if (control is Label label)
                        {
                            int controlRight = control.Location.X + label.Width;
                            int controlBottom = control.Location.Y + label.Height;

                            maxX = Math.Max(maxX, controlRight);
                            maxY = Math.Max(maxY, controlBottom);
                        }
                    }

                    // 비트맵 크기 계산
                    int width = maxX + 50;  // 가장 끝 라벨 위치 + 여백
                    int height = maxY + 50;

                    // Bitmap 생성
                    using (Bitmap bitmap = new Bitmap(width, height))
                    {
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.Clear(Color.White); // 배경색 설정
                                                  // 1. 라벨 복사

                            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBBBBBBBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIITTTTTTTTTTTTTTTTTTTTTTTTTTT");
                            foreach (Control control in this.Controls)
                            {
                                if (control is Label label)
                                {
                                    Console.WriteLine($"{control.Text} : {control.Location.X - control.Width / 2}");
                                    Rectangle rect = new Rectangle(label.Location, label.Size);
                                    label.DrawToBitmap(bitmap, rect);
                                }
                            }
                            PaintEventArgs paintArgs = new PaintEventArgs(g, new Rectangle(0, 0, width, height));
                            layout.linePaint(paintArgs);

                        }

                        // 저장
                        string extension = Path.GetExtension(saveFileDialog.FileName)?.ToLower();
                        switch (extension)
                        {
                            case ".jpg":
                                bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                break;
                            case ".png":
                                bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                                break;
                            default:
                                bitmap.Save(saveFileDialog.FileName);
                                break;
                        }

                        MessageBox.Show("전체 플로우차트가 저장되었습니다!", "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}
