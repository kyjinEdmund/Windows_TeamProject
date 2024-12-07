using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}
