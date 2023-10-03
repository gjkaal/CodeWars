using System.Text;

namespace AdventOfCode2023;

[TestClass]
public class Day8
{
    private static readonly Action<string> StdOut = System.Console.WriteLine;

    private const string example1 = @"RL

AAA = (BBB, CCC)
BBB = (DDD, EEE)
CCC = (ZZZ, GGG)
DDD = (DDD, DDD)
EEE = (EEE, EEE)
GGG = (GGG, GGG)
ZZZ = (ZZZ, ZZZ)";

    private const string example2 = @"LLR

AAA = (BBB, BBB)
BBB = (AAA, ZZZ)
ZZZ = (ZZZ, ZZZ)";

    private const string ghostMap = @"LR

AAA = (AAB, XXX)
AAB = (XXX, AAZ)
AAZ = (AAB, XXX)
PPA = (PPB, XXX)
PPB = (PPC, PPC)
PPC = (PPZ, PPZ)
PPZ = (PPB, PPB)
XXX = (XXX, XXX)";

    [DataTestMethod]
    [DataRow("AAA = (BBB, CCC)", 10101, 20202, 30303)]
    public void TestParseNode(string value, int expectedNodeId, int expectedLeft, int expectedRight)
    {
        var node = ParseNode(value);
        Assert.AreEqual(expectedNodeId, node.NodeId);
        Assert.AreEqual(expectedLeft, node.Left);
        Assert.AreEqual(expectedRight, node.Right);
    }

    [DataTestMethod]
    [DataRow("AAA", true, false)]
    [DataRow("EDA", true, false)]
    [DataRow("CDA", true, false)]
    [DataRow("BBB", false, false)]
    [DataRow("AAB", false, false)]
    [DataRow("ZZB", false, false)]
    [DataRow("ZZZ", false, true)]
    [DataRow("ABZ", false, true)]
    [DataRow("PAZ", false, true)]
    public void TestNodeValue(string value, bool isANode, bool isZNode)
    {
        var node = NodeValue(value);
        StdOut($"{value} = {node}");
        StdOut($"{value} = {node % 100}");
        Assert.AreEqual(isANode, node % 100 == 1);
        Assert.AreEqual(isZNode, node % 100 == 26);
    }

    [TestMethod]
    public void TestFindPathExample1()
    {
        NetworkMap map;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(example1));
        {
            map = ReadMap(stream);
        }
        Assert.AreEqual(map.Instructions, "RL");
        Assert.AreEqual(7, map.Nodes.Count);
        Assert.AreEqual(2, FindPathSteps(map, NodeValue("AAA"), NodeValue("ZZZ")));       
    }

    [TestMethod]
    public void TestFindPathExample2()
    {
        NetworkMap map;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(example2));
        {
            map = ReadMap(stream);
        }
        Assert.AreEqual(6, FindPathSteps(map, NodeValue("AAA"), NodeValue("ZZZ")));
    }

    [TestMethod]
    public void FindPathStepsFinal()
    {
        using var stream = new FileStream("InputFiles/Day8.txt", FileMode.Open);  
        NetworkMap map = ReadMap(stream);        
        Assert.AreEqual(20777, FindPathSteps(map, NodeValue("AAA"), NodeValue("ZZZ")));
    }

    [TestMethod]
    public void TestFindStartNodesFromMap()
    {
        NetworkMap map;
        using var stream = new FileStream("InputFiles/Day8.txt", FileMode.Open);        
        map = ReadMap(stream);        
        var startNodes = FindStartNodes(map);
        StdOut($"Start nodes: {startNodes.Count()}");
        StdOut($"Start nodes: \n\r {string.Join("\n\r", startNodes.Select(n => n.NodeId))}");
        Assert.AreEqual(6, startNodes.Count());
    }

    [TestMethod]
    public void TestFindStartNodes()
    {
        NetworkMap map;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(ghostMap));
        {
            map = ReadMap(stream);
        }
        var startNodes = FindStartNodes(map);
        Assert.IsNotNull(startNodes.Single(s => s.NodeId == NodeValue("AAA")));
        Assert.IsNotNull(startNodes.Single(s => s.NodeId == NodeValue("PPA")));
    }

    [TestMethod]
    public void TestFindGhostSteps()
    {
        NetworkMap map;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(ghostMap));
        {
            map = ReadMap(stream);
        }
        var startNodes = FindStartNodes(map);
        var steps = FindGhostSteps(map, startNodes.Select(n => n.NodeId).ToArray());
        StdOut($"Steps: {steps}");
        Assert.AreEqual(6, steps);
    }

    [TestMethod]
    public void FindGhostStepsFinal()
    {
        NetworkMap map;
        using var stream = new FileStream("InputFiles/Day8.txt", FileMode.Open);
        {
            map = ReadMap(stream);
        }
        var startNodes = FindStartNodes(map);
        var steps = FindGhostSteps(map, startNodes.Select(n => n.NodeId).ToArray());
        StdOut($"Steps: {steps}");
        Assert.AreEqual(13289612809129, steps);
    }

    private NetworkNode[] FindStartNodes(NetworkMap map) { 
        return map.Nodes.Where(n => n.NodeId % 100 == 1).ToArray();
    }

    private long FindGhostSteps(NetworkMap map, int[] startNodes)
    {
        var nodeCount = startNodes.Length;
        var nodeSteps = new long[nodeCount];
        var totalSteps = new long[nodeCount];
        for (var i = 0; i < startNodes.Length; i++)
        {
            var n = startNodes[i];
            var steps = FindGhostStepsToAnyLastNode(map, n);
            nodeSteps[i]= steps;
            totalSteps[i]=steps;
        }
        var maxSteps = (long)nodeSteps.Max();
        var maxStepId = nodeSteps.ToList().IndexOf(maxSteps);


        while (totalSteps.Count(s => s == maxSteps) != nodeCount)
        {
            for (var i = 0; i < nodeCount; i++)
            {
                if (totalSteps[i] < maxSteps)
                {
                    totalSteps[i] += nodeSteps[i];
                }
                if (totalSteps[i] > maxSteps)
                {
                    totalSteps[maxStepId] += nodeSteps[maxStepId];
                    maxSteps = totalSteps[maxStepId];
                }
            }            
        }
        return maxSteps;
    }


    private int FindGhostStepsToAnyLastNode(NetworkMap map, int startNode)
    {
        var steps = 0;
        var currentNode = map.NodesIndex[startNode];

        var instructions = map.Instructions;
        while (true)
        {
            for (var i = 0; i < instructions.Length; i++)
            {
                steps++;
                var node = map.Nodes[currentNode];
                var nextVal = instructions[i] == 'L' ? node.Left : node.Right;
                if ((nextVal % 100) == 26)
                {
                    return steps;
                }
                currentNode = map.NodesIndex[nextVal];
            }
        }
    }

    private int FindPathSteps(NetworkMap map, int startNode, int findNode)
    {
        var steps = 0;
        var currentNode = map.NodesIndex[startNode];
        var instructions = map.Instructions.ToArray();
        while (true) {
            for(var i = 0; i < instructions.Length; i++)
            {
                steps++;
                var node = map.Nodes[currentNode];
                var nodeId = instructions[i] switch
                {
                    'L' => node.Left,
                    'R' => node.Right,
                    _ => throw new InvalidOperationException($"Invalid instruction: {instructions[i]}")
                };
                if (findNode==nodeId)
                {
                    return steps;
                }
                currentNode = map.NodesIndex[nodeId];                
            }
        }
    }

    private NetworkMap ReadMap(Stream stream)
    {        
        using var reader = new StreamReader(stream, leaveOpen: true);
        var result = new NetworkMap
        {
            Instructions = reader.ReadLine() ?? string.Empty
        };

        var line = reader.ReadLine();
        while (line is not null)
        {
            if (!string.IsNullOrEmpty(line))
            {
                result.Nodes.Add(ParseNode(line));
            }
            line = reader.ReadLine();
        }

        for (var i = 0; i < result.Nodes.Count; i++)
        {
            var node = result.Nodes[i];
            result.NodesIndex.Add(node.NodeId, i);
        }

        return result;
    }

    private NetworkNode ParseNode(string value)
    {
        var parts = value.Split("=", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var nodes = parts[1].Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return new NetworkNode
        {
            NodeId = NodeValue(parts[0]),
            Left = NodeValue(nodes[0][1..]),
            Right = NodeValue(nodes[1][..^1])
        };
    }

    private int NodeValue(string value)
    {
        int nodeId = 0;
        for (var i = 0; i < 3; i++)
        {
            var n = (value[i] - '@');
            if (n<0) throw new InvalidOperationException($"Invalid node value: {value}");
            nodeId = nodeId * 100 + n;
        }
        return nodeId;
    }
}

public class NetworkMap
{
    public string Instructions { get; set; } = string.Empty;
    public List<NetworkNode> Nodes { get; set; } = new List<NetworkNode>();
    public Dictionary<int, int> NodesIndex { get; set; } = new();
}

public struct NetworkNode
{
    public int NodeId { get; set; }
    public int Left { get; set; }
    public int Right { get; set; }    
}