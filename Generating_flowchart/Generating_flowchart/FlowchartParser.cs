using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Generating_flowchart
{
    internal class FlowchartParser
    {
    }

    public class GraphBuilder
    {

        private int currentId = 1;
        private List<Node> result = new List<Node>();
        public List<Node> GetResult()
        {
            return result;
        }
        public void AssignNodeLevels(Node node, int currentLevel = 0, HashSet<int> visited = null)
        {
            if (node == null) return;

            // 방문한 노드 ID를 기록하는 HashSet 초기화
            if (visited == null)
            {
                visited = new HashSet<int>();
            }

            // 이미 방문한 노드라면 순회를 종료
            if (visited.Contains(node.NodeID))
            {
                return;
            }

            // 현재 노드를 방문한 것으로 기록
            visited.Add(node.NodeID);

            // 현재 노드의 레벨 설정
            node.NodeLevel = currentLevel;

            // True Case 순회
            if (node.Cases.Count > 0)
            {
                // 자식 노드의 깊이를 계산 (True Case는 부모보다 1 깊음)
                AssignNodeLevels(node.Cases[0], currentLevel + 1, visited);
            }

            // False Case 순회
            if (node.Cases.Count > 1)
            {
                // False Case는 분기지만 같은 깊이를 유지
                AssignNodeLevels(node.Cases[1], currentLevel, visited);
            }
        }

        public List<Node> BuildGraph(List<ParsedComponent> components)
        {

            for (int idx = 0; idx < components.Count; idx++)
            {
                ParsedComponent component = components[idx];
                if (component.GetCodeType() == CodeType.If || component.GetCodeType() == CodeType.ElseIf)
                {
                    Node res_nd = CreateNode(component);

                    int nextIdx = idx + 1;
                    if (nextIdx < components.Count)
                    {
                        res_nd.AddCases(CreateNode(components[nextIdx]));
                    }
                    nextIdx += 1;
                    if (nextIdx < components.Count)
                    {
                        if (components[nextIdx].GetCodeType() == CodeType.Else) nextIdx += 1;
                        res_nd.AddCases(CreateNode(components[nextIdx]));
                    }

                }
                else if (component.GetCodeType() == CodeType.Else)
                {
                    continue;
                }
                else if (component.GetCodeType() == CodeType.BlockOperator)
                {
                    List<Node> _tmp = BuildGraph(component.GetChildren()); // 재귀하면서 tmp에 문제가 생기는듯?(중복문제)

                    int nextIdx = idx + 1;
                    while (nextIdx < components.Count)  // 블럭의 마지막 노드에 연결해주기... 어캐하지
                    {
                        if (components[nextIdx].GetCodeType() == CodeType.Else || components[nextIdx].GetCodeType() == CodeType.ElseIf)
                        {
                            nextIdx += 2;
                            continue;
                        }
                        List<ParsedComponent> BlockComponts = component.GetChildren();
                        for (int idxx = 0; idxx < BlockComponts.Count; idxx++)  // 이부분 로직 조금 이상하다...
                        {
                            if (BlockComponts[idxx].GetCodeType() == CodeType.Else)
                                continue;
                            Node __tmp = CreateNode(BlockComponts[idxx]);
                            if (__tmp.Cases.Count == 0)// 
                            {
                                __tmp.AddCases(CreateNode(components[nextIdx]));
                                Console.WriteLine($"다음노드 없는놈 {__tmp.ToString()}");
                            }

                        }

                        break;
                    }
                }
                else if (component.GetCodeType() == CodeType.While)
                {
                    Node res_nd = CreateNode(component);
                    int nextIdx = idx + 1;
                    Node tmp = CreateNode(components[nextIdx]);
                    res_nd.AddCases(tmp);
                    if (components[nextIdx].GetCodeType() != CodeType.BlockOperator)
                    {
                        tmp.AddCases(res_nd);
                        if (nextIdx + 1 < components.Count)
                            res_nd.AddCases(CreateNode(components[nextIdx + 1]));

                    }
                    else
                    {
                        List<Node> _tmp = BuildGraph(components[nextIdx].GetChildren());

                        while (nextIdx < components.Count)
                        {
                            List<ParsedComponent> BlockComponts = components[nextIdx].GetChildren();
                            for (int idxx = 0; idxx < BlockComponts.Count; idxx++)  // 이부분 로직 조금 이상하다...
                            {
                                if (BlockComponts[idxx].GetCodeType() == CodeType.Else)
                                    continue;
                                Node __tmp = CreateNode(BlockComponts[idxx]);
                                if (__tmp.Cases.Count == 0)// 블럭 안에서 다음노드를 못가진 친구를 찾아서 저장해야 하는데... 블럭 안에 없는 친구도 다음노드를 만들어주는게 문제네 블럭 안에서 안만들어진걸 어캐알지 진짜
                                {
                                    Console.WriteLine($"while문 안에서 자식 없는놈{__tmp.ToString()}");
                                    __tmp.AddCases(res_nd);
                                }

                            }

                            break;
                        }
                        if (nextIdx + 1 < components.Count)
                            res_nd.AddCases(CreateNode(components[nextIdx + 1]));
                    }
                    idx += 1;
                }
                else
                {
                    Node res_nd = CreateNode(component);

                    int nextIdx = idx + 1;
                    while (nextIdx < components.Count)
                    {
                        if (components[nextIdx].GetCodeType() == CodeType.Else || components[nextIdx].GetCodeType() == CodeType.ElseIf)
                        {
                            nextIdx += 2;
                            continue;
                        }
                        if (nextIdx < components.Count)
                            res_nd.AddCases(CreateNode(components[nextIdx]));
                        break;
                    }



                }
            }
            return result;
        }


        private Node CreateNode(ParsedComponent component)
        {

            string shape;
            Node node;
            foreach (Node _node in result)
            {
                if (_node.FromComp == component)
                {
                    return _node;
                }
            }
            if (component.GetCodeType() == CodeType.BlockOperator)
            {
                node = CreateNode(component.GetChildren()[0]);

            }
            else
            {
                shape = DetermineShape(component.GetCodeType());
                node = new Node(shape, component.GetText() ?? "", currentId++, component);  // FROM 컴포넌트로 중복 제거?
            }
            foreach (Node _node in result)
            {
                if (_node.FromComp == node.FromComp)
                {
                    return _node;
                }
            }
            Console.WriteLine(node.ToString());
            result.Add(node);
            return node;
        }

        private string DetermineShape(CodeType type)
        {
            switch (type)
            {
                case CodeType.If:
                case CodeType.ElseIf:
                case CodeType.While:
                    return "Diamond";
                case CodeType.FunctionDeclaration:
                case CodeType.FunctionReturn:
                    return "Oval";
                case CodeType.Printf:
                case CodeType.Scanf:
                    return "Parallelogram";
                case CodeType.VariableAssignment:
                case CodeType.VariableDeclaration:
                    return "Rectangle";
                default:
                    return "Undefined";
            }
        }


    }

    public enum CodeType
    {
        If, // diamond
        ElseIf,  // diamond
        Else,  // 노드 없음
        VariableAssignment,  // Rectangle
        VariableDeclaration,  // Rectangle
        FunctionDeclaration,// Oval
        FunctionReturn,// Oval
        Continue,// 노드 없음
        Break,// 노드 없음
        BlockOperator,  // 노드 없음
        BlockOperatorOpen,  // 노드 없음
        BlockOperatorClose, // 노드 없음
        While,  // 
        For,
        Printf,
        Scanf
    }


    public class ParsedComponent
    {
        private CodeType Type { get; set; }

        private string Text { get; set; }
        private List<ParsedComponent> Children { get; set; }


        public ParsedComponent(CodeType type, string text)
        {
            Type = type;
            Text = text;
            Children = new List<ParsedComponent>();

        }
        public void AddChildren(ParsedComponent child)
        {
            Children.Add(child);
        }
        public CodeType GetCodeType() { return Type; }
        public string GetText() { return Text; }
        public List<ParsedComponent> GetChildren() { return Children; }
        public void PrintSelf(string beforeText = "")  // 생각처럼 출력이 안됨
        {
            Console.WriteLine($"{beforeText}{Type}, {Text}");
            if (Type == CodeType.BlockOperator)
            {
                foreach (ParsedComponent child in Children)
                {
                    child.PrintSelf($"{beforeText}    ");
                }
            }

        }

    }

    public class CCodeAnalyzer
    {
        private List<Node> Res { get; set; }


        public static List<ParsedComponent> ParseBlock(List<ParsedComponent> children)
        {

            List<ParsedComponent> res = new List<ParsedComponent>();
            Stack<ParsedComponent> block_stack = new Stack<ParsedComponent>();
            foreach (var i in children)
            {
                if (i.GetCodeType() == CodeType.BlockOperatorOpen)  // {
                {
                    block_stack.Push(new ParsedComponent(CodeType.BlockOperator, ""));
                    continue;
                }
                if (i.GetCodeType() == CodeType.BlockOperatorClose)  // }
                {
                    ParsedComponent _tmp = block_stack.Pop();
                    if (block_stack.Count == 0)  // stack is empty
                        res.Add(_tmp);
                    else
                        block_stack.Peek().AddChildren(_tmp);
                    continue;
                }
                if (block_stack.Count == 0)
                {
                    res.Add(i);
                    continue;
                }
                else
                {
                    block_stack.Peek().AddChildren(i);
                }
            }
            return res;
        }

        public static List<ParsedComponent> AnalyzeCode(string code)
        {
            // 함수 정의
            var functionDefPattern = new Regex(@"(\w+)\s+(\w+)\(([^)]*)\)\s*\{?");
            var returnPattern = new Regex(@"\breturn\s*(.*?);");
            // 조건문들
            var ifPattern = new Regex(@"\bif\s*\(([^)]+)\)");
            var elseIfPattern = new Regex(@"\belse\s+if\s*\(([^)]+)\)");
            var elsePattern = new Regex(@"\belse\b");

            // 반복문들
            var whilePattern = new Regex(@"\bwhile\s*\(([^)]+)\)");
            var forPattern = new Regex(@"\bfor\s*\(([^;]+);([^;]+);([^)]+)\)");

            // 입출력문
            var printfPattern = new Regex(@"\bprintf\s*\(([^)]+)\)");
            var scanfPattern = new Regex(@"\bscanf\s*\(([^)]+)\)");

            // 변수 대입
            var assignmentPattern = new Regex(@"(\w+)\s*=\s*([^;]+);");

            // 블록 연산자
            var openBracePattern = new Regex(@"^\s*\{\s*$");
            var closeBracePattern = new Regex(@"^\s*\}\s*$");

            // 각 줄을 분석
            var lines = code.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var res = new List<ParsedComponent>();

            foreach (var line in lines)
            {
                string trimmedLine = line.Trim();

                // 함수 정의 확인
                var funcMatch = functionDefPattern.Match(trimmedLine);
                if (funcMatch.Success)
                {
                    string returnType = funcMatch.Groups[1].Value;
                    string functionName = funcMatch.Groups[2].Value;
                    string parameters = funcMatch.Groups[3].Value;

                    res.Add(new ParsedComponent(
                            CodeType.FunctionDeclaration,
                            $"{functionName}({parameters})"
                        ));
                    continue;
                }
                // else if문 확인
                var elseIfMatch = elseIfPattern.Match(trimmedLine);
                if (elseIfMatch.Success)
                {
                    string condition = elseIfMatch.Groups[1].Value;

                    res.Add(new ParsedComponent(
                            CodeType.ElseIf,
                            $"{condition}"
                        ));

                    continue;
                }
                // if문 확인
                var ifMatch = ifPattern.Match(trimmedLine);
                if (ifMatch.Success)
                {
                    string condition = ifMatch.Groups[1].Value;

                    res.Add(new ParsedComponent(
                            CodeType.If,
                            $"{condition}"
                        ));
                    continue;
                }



                // else문 확인
                if (elsePattern.IsMatch(trimmedLine))
                {

                    res.Add(new ParsedComponent(
                            CodeType.Else,
                            $""
                        ));
                    continue;
                }

                // while문 확인
                var whileMatch = whilePattern.Match(trimmedLine);
                if (whileMatch.Success)
                {
                    string condition = whileMatch.Groups[1].Value;

                    res.Add(new ParsedComponent(
                            CodeType.While,
                            $"{condition}"
                        ));
                    continue;
                }

                // for문 확인
                var forMatch = forPattern.Match(trimmedLine);
                if (forMatch.Success)
                {
                    string init = forMatch.Groups[1].Value;
                    string condition = forMatch.Groups[2].Value;
                    string increment = forMatch.Groups[3].Value;



                    ParsedComponent forComp = new ParsedComponent(
                            CodeType.For,
                            $""
                    );
                    ParsedComponent block = new ParsedComponent(CodeType.BlockOperator, $"");
                    block.AddChildren(
                        new ParsedComponent(CodeType.While, $"{condition}")
                    );
                    block.AddChildren(
                        new ParsedComponent(CodeType.VariableDeclaration, $"{init}")
                    );
                    block.AddChildren(
                        new ParsedComponent(CodeType.VariableDeclaration, $"{increment}")
                    );
                    forComp.AddChildren(block);
                    res.Add(forComp);


                    continue;
                }

                // printf문 확인
                var printfMatch = printfPattern.Match(trimmedLine);
                if (printfMatch.Success)
                {
                    string content = printfMatch.Groups[1].Value;

                    res.Add(new ParsedComponent(
                            CodeType.Printf,
                            $"{content}"
                        ));
                    continue;
                }

                // scanf문 확인
                var scanfMatch = scanfPattern.Match(trimmedLine);
                if (scanfMatch.Success)
                {
                    string content = scanfMatch.Groups[1].Value;

                    res.Add(new ParsedComponent(
                            CodeType.Scanf,
                            $"{content}"
                        ));
                    continue;
                }

                // 변수 대입 확인
                var assignmentMatch = assignmentPattern.Match(trimmedLine);
                if (assignmentMatch.Success)
                {
                    string variable = assignmentMatch.Groups[1].Value;
                    string value = assignmentMatch.Groups[2].Value;

                    res.Add(new ParsedComponent(
                            CodeType.VariableDeclaration,
                            $"{variable} = {value}"
                        ));
                    continue;
                }

                // 블록 여는 문 확인
                if (openBracePattern.IsMatch(trimmedLine))
                {

                    res.Add(new ParsedComponent(
                            CodeType.BlockOperatorOpen,
                            $""
                        ));
                    continue;
                }

                // 블록 닫는 문 확인
                if (closeBracePattern.IsMatch(trimmedLine))
                {

                    res.Add(new ParsedComponent(
                           CodeType.BlockOperatorClose,
                           $""
                       ));

                    continue;
                }
                if (returnPattern.IsMatch(trimmedLine))
                {
                    var match = returnPattern.Match(trimmedLine);
                    string content = match.Groups[1].Value.Trim(); // 반환값 가져오기 (없을 수도 있음)
                    res.Add(new ParsedComponent(
                    CodeType.FunctionReturn,
                           $"{content}"
                       ));

                    continue;
                }
            }
            return res;
        }
    }
}
