using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Generating_flowchart
{
    public class FlowChatManager
    {
        private Form _form; // 노드를 배치할 Windows Form
        private List<Node> _nodes; // 전달받은 Node 리스트
        private Dictionary<int, Point> ndPoint; // 노드의 위치 저장  
        private HashSet<(int, int)> drawnConnections = new HashSet<(int, int)>();
        private const int Width = 150; // 가로 간격
        private const int height = 100; // 세로 간격

        private int count = 0;

        public List<Node> Nodes
        {
            get { return _nodes; }
        }

        public Dictionary<int, Point> NodePoint
        {
            get { return ndPoint; }
            set { ndPoint = value; }
        }

        public FlowChatManager(Form form, List<Node> nodes)
        {
            _form = form;
            _nodes = nodes; // 전달받은 Node 리스트를 저장
            ndPoint = new Dictionary<int, Point>();
        }

        public void buildStart()
        {
            if (_nodes == null || !_nodes.Any())    // Any()의 예외에 null인 경우가 있어서 null인 경우 따로 작성  //Any() 는 하나의 요소라도 들어있다면 True
            {
                Console.WriteLine("노드 리스트가 비어있습니다.");
                return;
            }

            // 시작 노드(첫 번째 노드) 찾기
            Node startNode = _nodes.FirstOrDefault();

            if (startNode != null)
            {

                if (_nodes[0].Shape == "Diamond")
                {
                    Diamond.Diamond startLabel = new Diamond.Diamond
                    {
                        Text = _nodes[0].Text,
                        AutoSize = true
                    };
                }
                if (_nodes[0].Shape == "Parallelogram")
                {
                    Parallelogram.Parallelogram startLabel = new Parallelogram.Parallelogram
                    {
                        Text = _nodes[0].Text,
                        AutoSize = true
                    };
                }
                if (_nodes[0].Shape == "Rectangle")
                {
                    Ractangle.Ractangle startLabel = new Ractangle.Ractangle
                    {
                        Text = _nodes[0].Text,
                        AutoSize = true
                    };
                }
                if (_nodes[0].Shape == "Oval")
                {
                    RoundedRactangle.RoundedRactangle startLabel = new RoundedRactangle.RoundedRactangle
                    {
                        Text = _nodes[0].Text,
                        AutoSize = true
                    };
                }
                // 노드 배치 시작
                PlaceNodes(startNode, 0, _form.ClientSize.Width / 2, 50);

            }
        }
        public void linePaint(PaintEventArgs e)
        {
            drawnConnections.Clear();
            foreach (Node node in _nodes)
            {
                if (ndPoint.ContainsKey(node.NodeID))
                {
                    // Graphics 객체를 Paint 이벤트에서 받아옴
                    connect(node, ndPoint[node.NodeID], e.Graphics);
                }
            }
        }
        private void PlaceNodes(Node node, int depth, int x, int y)
        {
            if (ndPoint.ContainsKey(node.NodeID)) return; // 이미 배치된 노드라면 무시
                                                          //Console.WriteLine($"{node.Text} + {x} count : {count}");


            // 현재 노드의 위치 저장
            //Point position = new Point(x, y);
            //ndPoint[node.NodeID] = position;

            // 노드에 해당하는 Label 생성

            dynamic nodeLabel;
            if (node.Shape == "Diamond")
            {
                nodeLabel = new Diamond.Diamond
                {
                    Text = node.Text,
                    AutoSize = true,
                    MaximumSize = new Size(120, 40),
                    Name = node.NodeID.ToString()
                };
            }
            else if (node.Shape == "Parallelogram")
            {
                nodeLabel = new Parallelogram.Parallelogram
                {
                    Text = node.Text,
                    AutoSize = true,
                    MaximumSize = new Size(160, 40),
                    Name = node.NodeID.ToString()
                };
            }
            else if (node.Shape == "Rectangle")
            {
                nodeLabel = new Ractangle.Ractangle
                {
                    Text = node.Text,
                    AutoSize = true,
                    MaximumSize = new Size(120, 40),
                    Name = node.NodeID.ToString()
                };
            }
            else if (node.Shape == "Oval")
            {
                nodeLabel = new RoundedRactangle.RoundedRactangle
                {
                    Text = node.Text,
                    AutoSize = true,
                    MaximumSize = new Size(120, 40),
                    Name = node.NodeID.ToString()
                };
            }
            else
            {
                nodeLabel = new Label
                {
                    Text = node.Text,
                    AutoSize = true,
                    MaximumSize = new Size(120, 0),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Name = node.NodeID.ToString()
                };
            }
            Console.WriteLine($"{node.Text}:{nodeLabel.Name} {node.Shape}");


            // 폼에 Label 추가
            _form.Controls.Add(nodeLabel);

            // 위치 조정 (중앙 정렬 유지)
            nodeLabel.Location = new Point(x - (nodeLabel.Width / 2), y);


            // 노드 위치를 라벨의 중심으로 저장
            Point centerPosition = new Point(
                nodeLabel.Location.X,
                nodeLabel.Location.Y
            );
            ndPoint[node.NodeID] = centerPosition; // 갱신된 중심 좌표 저장

            // 자식 노드 배치
            int childY = y + height;
            int childX = (_form.ClientSize.Width / 2);

            for (int i = 0; i < node.Cases.Count; i++)
            {
                if (i == 0)
                {
                    PlaceNodes(node.Cases[i], depth + 1, childX + (count * Width), childY);
                }
                else if (i == 1)
                {

                    count++;
                    //int count_NO = Count_NoCase1(node, _nodes);
                    PlaceNodes(node.Cases[i], depth + 1, childX + (count * Width), childY - height);
                }
            }
        }

        private void connect(Node node, Point labelpoint, Graphics e)
        {
            foreach (Node child in node.Cases)
            {
                Console.WriteLine($"{child.Text}");
                // 이미 그려진 연결선인지 확인
                var connection = (Math.Min(node.NodeID, child.NodeID), Math.Max(node.NodeID, child.NodeID));
                if (drawnConnections.Contains(connection))
                {
                    continue;
                }
                if (drawnConnections.Contains((node.NodeID, child.NodeID)) ||
                    drawnConnections.Contains((child.NodeID, node.NodeID)))
                {
                    continue;
                }
                Point parentPosition = ndPoint[node.NodeID];
                Point childPosition = ndPoint[child.NodeID];

                Console.WriteLine($"{node.NodeLevel}. {node.Text} : {child.NodeLevel}. {child.Text}");
                if (node.NodeLevel == 0)
                {
                    Console.WriteLine("1번째의 시작입니다다ㅏㅏㅏㅏ");
                    Point startPoint = GetCenter(node.NodeID, 1); // 밑변 중앙
                    Point endPoint = GetCenter(child.NodeID, 0);    // 윗변 중앙
                    DrawLine(e, startPoint, endPoint);

                }
                else if (node.Shape == "Diamond" && node.NodeLevel == child.NodeLevel)
                {
                    Point startPoint = GetCenter(node.NodeID, 3);
                    Point endPoint = GetCenter(child.NodeID, 2);
                    endPoint.Y = startPoint.Y;
                    Point mid1 = new Point(startPoint.X + Width / 2, startPoint.Y);
                    Point mid2 = new Point(mid1.X, startPoint.Y);
                    DrawLine(e, startPoint, endPoint, mid1, mid2);

                }
                else if (node.Shape != "Diamond" && child.NodeLevel - node.NodeLevel > 1)
                {
                    // 자식이 더 높은 레벨일 때: 밑변 센터 -> 좌변 센터, 꺾인 선
                    Point startPoint = GetCenter(node.NodeID, 1); // 밑변 센터
                    Point endPoint = GetCenter(child.NodeID, 3);    // 우변 센터
                    Point angle1 = new Point(GetCenter(node.NodeID, 1).X, GetCenter(child.NodeID, 3).Y); // 꺾이는 지점

                    using (Graphics g = _form.CreateGraphics())
                    {
                        DrawLine(g, startPoint, endPoint, angle1);
                    }
                }
                else if (node.Shape == "Rectangle" && node.NodeLevel > child.NodeLevel)
                {
                    // 부모가 더 높은 레벨일 때: 좌변 센터 -> 좌변 센터, 두 번 꺾인 선
                    Point startPoint = GetCenter(node.NodeID, 2); // 좌변 센터
                    Point endPoint = GetCenter(child.NodeID, 2);    // 좌변 센터
                    Point angle1 = new Point(startPoint.X - (Width / 2), startPoint.Y);
                    Point angle2 = new Point(angle1.X, endPoint.Y);
                    using (Graphics g = _form.CreateGraphics())
                    {
                        DrawLine(g, startPoint, endPoint, angle1, angle2);

                    }
                }
                else if (node.Shape == "Parallelogram" && node.NodeLevel < child.NodeLevel && parentPosition.X > childPosition.X)
                {
                    Point startPoint = GetCenter(node.NodeID, 1);
                    Point endPoint = GetCenter(child.NodeID, 3);
                    Point angle1 = new Point(startPoint.X, endPoint.Y);
                    using (Graphics g = _form.CreateGraphics())
                    {
                        DrawLine(g, startPoint, endPoint, angle1);
                    }
                }
                else
                {
                    // 일반적인 경우: 밑변 센터 -> 윗변 센터로 직선 연결
                    Point startPoint = GetCenter(node.NodeID, 1); // 밑변 센터
                    Point endPoint = GetCenter(child.NodeID, 0);// 윗변 센터
                    endPoint.X = startPoint.X;
                    using (Graphics g = _form.CreateGraphics())
                    {
                        DrawLine(g, startPoint, endPoint);
                    }
                }
                drawnConnections.Add((node.NodeID, child.NodeID));
            }
        }

        // 두 Label 간 연결선 그리기
        void DrawLine(Graphics g, Point start, Point end, Point ang1 = default, Point ang2 = default)
        {
            if (g == null) return;  // null 체크 추가

            using (Pen pen = new Pen(Color.Black, 2))
            {
                pen.CustomEndCap = new AdjustableArrowCap(4, 4);
                pen.LineJoin = LineJoin.Round;  // 선이 만나는 부분을 부드럽게

                if (ang1 == Point.Empty && ang2 == Point.Empty)
                {
                    Console.WriteLine("그려집니다ㅏㅏㅏㅏ111");
                    g.DrawLine(pen, start, end);
                }
                else if (ang2 == Point.Empty)
                {
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddLine(start, ang1);
                        path.AddLine(ang1, end);
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        Console.WriteLine("그려집니다ㅏㅏㅏㅏ");
                        path.AddLine(start, ang1);
                        path.AddLine(ang1, ang2);
                        path.AddLine(ang2, end);
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        // Label의 중심 좌표 계산
        private Point GetCenter(int nodeID, int type = -1)
        {
            Control label = null;
            Point returnP;

            foreach (Control c in _form.Controls)
            {
                if (c.Location.Equals(ndPoint[nodeID]))
                {
                    label = c;
                    //Console.WriteLine($"{c}");
                    break;
                }

            }

            if (label == null)
            {
                Console.WriteLine("null");
            }
            if (type == 0) //윗변의 센터값
            {
                returnP = new Point(label.Location.X + label.Width / 2, label.Location.Y);
                Console.WriteLine($"returnP {returnP}");

            }
            else if (type == 1) //밑변의 센터값
            {
                returnP = new Point(label.Location.X + label.Width / 2, label.Location.Y + label.Height);
            }
            else if (type == 2) //왼변의 센터값
            {
                returnP = new Point(label.Location.X, label.Location.Y + label.Height / 2);
            }
            else if (type == 3) //우변의 센터값
            {
                returnP = new Point(label.Location.X + label.Width, label.Location.Y + label.Height / 2);
            }
            else //미입력시 노드의 중앙값을 리턴.
            {
                returnP = new Point(label.Location.X + label.Width / 2, label.Location.Y + label.Height / 2);
            }
            return returnP;
        }
    }
}
