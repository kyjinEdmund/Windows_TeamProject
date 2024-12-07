using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using ScintillaNET;


namespace Generating_flowchart
{
    public partial class Form1 : Form
    {

        private bool onClick;
        private Point startPoint = new Point(0, 0);

        private FlowLayoutPanel mainFlowLayoutPanel; // 수평 정렬을 위한 메인 FlowLayoutPanel
        private Button addButton;
        private int fileCounter = 0;


        int floPanel = 0;
        string activeFileName = null;

        private ContextMenuStrip contextMenu; // ContextMenuStrip
        private ToolStripMenuItem deleteMenuItem; // 삭제 메뉴 아이템

        //json데이터 생성
        private string jsonFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyAppFiles", "json", "list.json");

        //저장할 파일 위치 변수
        string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyAppFiles", "data");

        //타이머
        private System.Windows.Forms.Timer saveTimer;

        public Form1()
        {
            //기초 파일 위치 없으면 생성
            string foldercheck = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyAppFiles", "data");
            Directory.CreateDirectory(foldercheck); // 폴더가 없으면 생성
            foldercheck = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyAppFiles", "json");
            Directory.CreateDirectory(foldercheck); // 폴더가 없으면 생성


            InitializeComponent();
            InitializeCustomComponents();
            //InitializeContextMenu();
            if (!File.Exists(jsonFilePath))
            {
                // JSON 파일이 없으면 빈 JSON 배열을 생성하여 파일로 저장
                File.WriteAllText(jsonFilePath, "[]");  // 시작 시 빈 배열로 만들어서 예외발생 없앰
            }

            //json파일이 있을테니 이것을 추가
            if (File.Exists(jsonFilePath))
            {
                LoadJsonFile();
            }


            //타이머 설정
            saveTimer = new System.Windows.Forms.Timer();
            saveTimer.Interval = 1000; // 1초 후 저장
            saveTimer.Tick += SaveTimer_Tick;
        }



        private void SaveTimer_Tick(object sender, EventArgs e)
        {
            saveTimer.Stop(); // 타이머 중지
            SavingFile(); // 파일 저장

        }

        private void OpenNewFlowchart(object sender, EventArgs e)
        {
            String _tmp = scintilla1.Text;
            Console.WriteLine(_tmp);
            List<ParsedComponent> resList = CCodeAnalyzer.AnalyzeCode(_tmp);
            List<ParsedComponent> parsedBlockRes = CCodeAnalyzer.ParseBlock(resList);

            foreach (ParsedComponent res in resList)
            {
                res.PrintSelf();
            }
            Console.Write("\n\n\n");
            foreach (ParsedComponent res in parsedBlockRes)
            {
                res.PrintSelf();
            }
            GraphBuilder graphBuilder = new GraphBuilder();
            List<Node> GraphList = graphBuilder.BuildGraph(parsedBlockRes);
            graphBuilder.AssignNodeLevels(GraphList[0]);
            Console.WriteLine("\n\n\n");
            foreach (Node res in GraphList)
            {
                Console.WriteLine($"{res.NodeID} {res.Text} {res.NodeLevel} {res.Shape} {res.Cases.Count}");
                if (res.Cases.Count == 1)
                {
                    Node tmp = res.Cases[0];
                    Console.WriteLine($"    {tmp.NodeID} {tmp.Text} {tmp.NodeLevel} {tmp.Shape}");
                }
                if (res.Cases.Count == 2)
                {
                    Node tmp = res.Cases[1];
                    Console.WriteLine($"    {tmp.NodeID} {tmp.Text} {tmp.NodeLevel} {tmp.Shape}");
                    tmp = res.Cases[0];
                    Console.WriteLine($"    {tmp.NodeID} {tmp.Text} {tmp.NodeLevel} {tmp.Shape}");
                }

            }

            FlowchartForm1 _form2 = new FlowchartForm1(GraphList);
            _form2.ShowDialog();
            //FlowchartForm1 _tmp = new FlowchartForm1();
        }

        //프로그램 실행 후 json파일 불러오고 리스트에 항목 추가
        private void LoadJsonFile()
        {
            // JSON 파일 읽기
            string jsonData = File.ReadAllText(jsonFilePath);


            if (jsonData != "")
            {
                // JSON 배열 파싱
                JArray fileArray = JArray.Parse(jsonData);

                // 데이터 출력 (예: ListBox에 추가)
                foreach (JObject file in fileArray)
                {
                    string name = file["name"]?.ToString() ?? "Unknown";
                    string date = file["date"]?.ToString() ?? "????-??-??";
                    date = date.Length > 10 ? date.Substring(0, 10) : date;
                    newList(name, date);
                }
            }
        }

        //json파일 업데이트
        private void UpdateJsonFile(JObject newFile)
        {
            JArray fileArray;

            // 기존 JSON 데이터 읽기
            if (File.Exists(jsonFilePath))
            {
                string jsonData = File.ReadAllText(jsonFilePath);
                fileArray = new JArray();
                if (jsonData != "")
                {
                    fileArray = JArray.Parse(jsonData);
                }
            }
            else
            {
                // 파일이 없으면 새 배열 생성
                fileArray = new JArray();
            }

            // 새 데이터를 배열에 추가
            fileArray.Add(newFile);

            // JSON 데이터를 파일로 저장
            File.WriteAllText(jsonFilePath, fileArray.ToString());

            //MessageBox.Show("JSON 파일이 업데이트되었습니다!");
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
        }

        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            onClick = true;
            startPoint = new Point(e.X, e.Y);
        }

        private void panel3_MouseUp(object sender, MouseEventArgs e)
        {
            onClick = false;
        }

        private void panel3_MouseMove(object sender, MouseEventArgs e)
        {
            if (onClick)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this.startPoint.X, p.Y - this.startPoint.Y);
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            if (activeFileName != null)
            {
                saveTimer.Stop();
                SavingFile();
            }
            this.Close();
        }

        private void minButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void maxButton_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }



        //
        //
        //리스트에 항목을 추가하는 기능
        //
        //
        private void InitializeCustomComponents()
        {
            // 기존 FlowLayoutPanel1을 메인 패널로 사용
            mainFlowLayoutPanel = flowLayoutPanel1;
            mainFlowLayoutPanel.FlowDirection = FlowDirection.TopDown; // 수평 정렬
            mainFlowLayoutPanel.WrapContents = false;
            mainFlowLayoutPanel.AutoScroll = true;

            // TableLayoutPanel의 1행 1열에 버튼 추가
            //Panel panel1 = tableLayoutPanel1.GetControlFromPosition(1, 2) as Panel; // 필요에 따라 위치 변경
            //if (panel1 != null)
            //{
            //    panel1.Controls.Add(addButton);
            //}

            contextMenu = new ContextMenuStrip();
            deleteMenuItem = new ToolStripMenuItem("삭제", null, DeleteMenuItem_Click);

            // 메뉴 아이템 추가
            contextMenu.Items.AddRange(new ToolStripMenuItem[] { deleteMenuItem });


            //scintilla 기본 설정
            scintilla1.Lexer = Lexer.Cpp; // C++ 구문 강조
            scintilla1.Lexer = ScintillaNET.Lexer.Cpp;
            scintilla1.Font = new Font("Consolas", 14, FontStyle.Regular);
            scintilla1.StyleClearAll();

            scintilla1.IndentWidth = 4;
            scintilla1.UseTabs = false;
            scintilla1.Margins[0].Type = MarginType.Number;
            scintilla1.Margins[0].Width = 40;
            scintilla1.CaretLineVisible = true;

            scintilla1.CaretLineBackColor = Color.LightYellow;

            scintilla1.HScrollBar = false;
            scintilla1.Zoom = 3;

            //하이라이트 설정
            scintilla1.Styles[Style.Cpp.Default].ForeColor = Color.Black;
            scintilla1.Styles[Style.Cpp.Comment].ForeColor = Color.Green;
            scintilla1.Styles[Style.Cpp.CommentLine].ForeColor = Color.Green;
            scintilla1.Styles[Style.Cpp.Number].ForeColor = Color.DarkOrange;
            scintilla1.Styles[Style.Cpp.String].ForeColor = Color.Brown;
            scintilla1.Styles[Style.Cpp.Character].ForeColor = Color.DarkRed;
            scintilla1.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
            scintilla1.Styles[5].ForeColor = Color.Blue;  // 키워드 스타일
            scintilla1.Styles[5].Bold = true;

            // 키워드 정의
            scintilla1.SetKeywords(0, "int float double char void bool true false if else while for return");
        }


        //새로 저장 버튼 눌렀을 때
        private void button1_Click(object sender, EventArgs e)
        {
            using (SavingModal modalForm = new SavingModal()) // 모달창 인스턴스 생성
            {
                if (modalForm.ShowDialog() == DialogResult.OK) // 모달창 확인 버튼 클릭 확인
                {
                    string fileName = modalForm.FileName;
                    DateTime today = DateTime.Today;
                    string todayString = today.ToString("yyyy-MM-dd");
                    newList(fileName, todayString);
                    ShowingFile(fileName);

                    //파일로 저장
                    string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyAppFiles", "data");
                    Directory.CreateDirectory(folderPath); // 폴더가 없으면 생성
                    //fileName = $"{fileName}.txt";
                    string filePath = Path.Combine(folderPath, $"{fileName}.txt");

                    File.WriteAllText(filePath, scintilla1.Text); // 텍스트를 파일에 저장


                    //jason생성
                    JObject newFile = new JObject
                    {
                        {"name", fileName },
                        {"date", DateTime.Today},
                    };

                    // JSON 파일 업데이트
                    UpdateJsonFile(newFile);

                }
            }
        }




        //리스트 항목 생성
        private void newList(string fileName, string date)
        {
            FlowLayoutPanel subFlowLayoutPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown, // 세로 정렬
                WrapContents = false,
                AutoScroll = false,
                Width = 240, // 서브 패널 너비
                Height = 70, // 메인 패널 높이 고려
                BorderStyle = BorderStyle.None,
                Margin = new Padding(10, 0, 0, 0), // 서브 패널 간 간격
                Name = "subFlowLayoutPanel" + floPanel,
                Tag = fileName,
            };

            subFlowLayoutPanel.Click += SubFlowLayoutPanel_Click;

            subFlowLayoutPanel.ContextMenuStrip = contextMenu;


            floPanel++;

            // 예시: 서브 패널에 여러 항목 추가
            // 항목 패널 생성
            Panel itemPanel = new Panel
            {
                Width = subFlowLayoutPanel.ClientSize.Width - 20,
                Height = 40,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(5)
            };

            // 이름 라벨 생성
            Label nameLabel = new Label
            {
                Text = fileName,
                Dock = DockStyle.Left,
                Width = 256,
                Font = new Font("맑은 고딕", 14, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Margin = new Padding(5, 11, 0, 0)
            };

            // 날짜 라벨 생성
            Label dateLabel = new Label
            {
                Text = date,
                Dock = DockStyle.Left,
                Width = 120,
                Font = new Font("굴림", 12),
                ForeColor = Color.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Margin = new Padding(10, 1, 0, 0)
            };

            // 실선 라벨 생성
            Label lineLabel = new Label
            {
                Text = "",
                Dock = DockStyle.Left,
                Width = 240,
                Height = 2,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 8, 0, 0)
            };

            // 삭제 버튼 생성
            Button deleteButton = new Button
            {
                Text = "삭제",
                Dock = DockStyle.Right,
                Width = 50,
                Tag = itemPanel // 버튼의 태그에 항목 패널 저장
            };
            deleteButton.Click += (s, ev) =>
            {
                subFlowLayoutPanel.Controls.Remove(subFlowLayoutPanel);
                itemPanel.Dispose();
            };

            // 이름 라벨에 클릭 이벤트 추가
            nameLabel.Click += (s, e) => SubFlowLayoutPanel_Click(subFlowLayoutPanel, EventArgs.Empty);
            // 날짜 라벨에 클릭 이벤트 추가
            dateLabel.Click += (s, e) => SubFlowLayoutPanel_Click(subFlowLayoutPanel, EventArgs.Empty);

            // 서브 패널에 항목 추가
            subFlowLayoutPanel.Controls.Add(nameLabel);
            subFlowLayoutPanel.Controls.Add(dateLabel);
            subFlowLayoutPanel.Controls.Add(lineLabel);

            // 메인 FlowLayoutPanel에 서브 패널 추가
            mainFlowLayoutPanel.Controls.Add(subFlowLayoutPanel);
        }



        private void SubFlowLayoutPanel_Click(object sender, EventArgs e)
        {
            //다른 파일을 연다면 기존 파일을 바로 저장하기
            if (activeFileName != null)
            {
                saveTimer.Stop();
                SavingFile();
            }

            //MessageBox.Show("클릭함.");

            FlowLayoutPanel clickedPanel = (FlowLayoutPanel)sender;

            // 클릭된 패널의 Tag 속성에서 파일 이름 가져오기
            string fileName = clickedPanel.Tag.ToString();

            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyAppFiles", "data");

            string filePath = Path.Combine(folderPath, $"{fileName}.txt");

            if (File.Exists(filePath))
            {
                string fileContent = File.ReadAllText(filePath);
                scintilla1.Text = fileContent;
                ShowingFile(fileName);
            }
            else
            {
                MessageBox.Show("파일을 찾을 수 없습니다.");
            }


        }


        //contextmenu 삭제 버튼 설정
        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            FlowLayoutPanel subFlowLayoutPanel = contextMenu.SourceControl as FlowLayoutPanel;

            if (subFlowLayoutPanel != null)
            {
                // 메인 패널에서 해당 서브 패널을 삭제
                mainFlowLayoutPanel.Controls.Remove(subFlowLayoutPanel);
                subFlowLayoutPanel.Dispose(); // 리소스 해제

                //패널에서 파일 이름 가져오기
                string delFileName = subFlowLayoutPanel.Tag as string;

                //json에서 파일 지우기
                JArray fileArray = JArray.Parse(File.ReadAllText(jsonFilePath));
                for (int i = 0; i < fileArray.Count; i++)
                {
                    if (fileArray[i]["name"].ToString() == delFileName)
                    {
                        fileArray.RemoveAt(i);
                        break;
                    }
                }
                File.WriteAllText(jsonFilePath, fileArray.ToString());

                //text파일 지우기
                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyAppFiles", "data");

                string filePath = Path.Combine(folderPath, $"{delFileName}.txt");

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    if (delFileName == activeFileName)
                    {
                        ShowingFile(null);
                    }
                    //MessageBox.Show("파일이 삭제되었습니다.");
                }
                else
                {
                    MessageBox.Show("파일을 찾을 수 없습니다.");
                }



            }

        }




        //현재 보여줄 파일을 수정하는 함수
        private void ShowingFile(string fileName)
        {
            activeFileName = fileName;
            if (activeFileName != null)
            {
                label1.Text = activeFileName;
                savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyAppFiles", "data");
                savePath = Path.Combine(savePath, $"{fileName}.txt");
            }
            else
            {
                label1.Text = "";
                scintilla1.Text = "";
            }
        }


        //파일에 저장을 하는 함수
        private void SavingFile()
        {
            File.WriteAllText(savePath, scintilla1.Text);
            //MessageBox.Show("저장됨!");
        }



        //창 초기화 버튼 클릭
        private void clearButton_Click(object sender, EventArgs e)
        {
            if (activeFileName != null)
            {
                saveTimer.Stop();
                SavingFile();
            }
            ShowingFile(null);
        }

        //코드텍스트 박스 내용이 수정될 때 이벤트
        private void scintilla1_TextChanged(object sender, EventArgs e)
        {
            if (activeFileName != null)
            {
                saveTimer.Stop();
                saveTimer.Start();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        }



        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void scintilla1_Click(object sender, EventArgs e)
        {

        }
    }
}
