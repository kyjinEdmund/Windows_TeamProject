using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generating_flowchart
{
    public class Node
    {
        public string Shape { get; set; }
        public string Text { get; set; }
        public int NodeID { get; set; }
        public List<Node> Cases { get; set; }
        public ParsedComponent FromComp { get; set; }
        public int NodeLevel { get; set; }

        // 생성자
        public Node(string shape, string text, int nodeId, ParsedComponent from_comp)
        {
            Shape = shape;
            Text = text;
            NodeID = nodeId;
            Cases = new List<Node>();
            FromComp = from_comp;
        }
        public void AddCases(Node node)
        {
            Cases.Add(node);
        }
        public string ToString()
        {
            return $"{NodeID} {Shape} {Text}";
        }
    }
}
