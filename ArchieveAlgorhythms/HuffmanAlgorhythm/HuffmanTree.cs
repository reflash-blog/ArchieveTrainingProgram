using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ArchieveAlgorithms.HuffmanAlgorithm
{
    public class HuffmanTree
    {
        private readonly List<Node> _nodes = new List<Node>();
        public Node Root { get; set; }
        public Dictionary<char, int> Frequencies = new Dictionary<char, int>();

        public void Build(string source)
        {
            foreach (var t in source)
            {
                if (!Frequencies.ContainsKey(t))
                {
                    Frequencies.Add(t, 0);
                }

                Frequencies[t]++;
            }
 

            foreach (var symbol in Frequencies)
            {
                _nodes.Add(new Node { Symbol = symbol.Key, Frequency = symbol.Value });
            }

            while (_nodes.Count > 1)
            {
                var orderedNodes = _nodes.OrderBy(node => node.Frequency).ToList();

                if (orderedNodes.Count >= 2)
                {
                    // Take first two items
                    var taken = orderedNodes.Take(2).ToList();

                    // Create a parent node by combining the frequencies
                    var parent = new Node
                    {
                        Symbol = '*',
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1]
                    };

                    _nodes.Remove(taken[0]);
                    _nodes.Remove(taken[1]);
                    _nodes.Add(parent);
                }

                

            }
            Root = _nodes.FirstOrDefault();

        }

        public BitArray Encode(string source)
        {
            var encodedSource = new List<bool>();

            foreach (var encodedSymbol in source.Select(t => Root.Traverse(t, new List<bool>())))
            {
                encodedSource.AddRange(encodedSymbol);
            }

            var bits = new BitArray(encodedSource.ToArray());

            return bits;
        }

        public string Decode(BitArray bits)
        {
            var current = Root;
            var decoded = "";

            foreach (bool bit in bits)
            {
                if (bit)
                {
                    if (current.Right != null)
                    {
                        current = current.Right;
                    }
                }
                else
                {
                    if (current.Left != null)
                    {
                        current = current.Left;
                    }
                }

                if (!IsLeaf(current)) continue;
                decoded += current.Symbol;
                current = Root;
            }

            return decoded;
        }

        public bool IsLeaf(Node node)
        {
            return (node.Left == null && node.Right == null);
        }

    }
}
