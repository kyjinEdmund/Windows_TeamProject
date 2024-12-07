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
