using System.Text;
using SerializationDeserialization.Extensions;
using SerializationDeserialization.Serialization;

namespace SerializationDeserialization.LinkedList;

public class ListRand : ISerializable
{
    public ListNode head = null!;
    public ListNode tail = null!;
    public int count;

    public void Serialize(FileStream fileStream)
    {
        var map = GetNodeIdMap();

        WriteDataToFile(fileStream, map);
    }

    public void Deserialize(FileStream fileStream)
    {
        string readingData = ReadData(fileStream);

        if (string.IsNullOrEmpty(readingData))
            return;

        string[] rows = readingData.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (!int.TryParse(rows[0], out count))
            throw new Exception("Can't parse count nodes");

        var nodes = InitializeNodes(rows);
        SetLinkedListDependencies(nodes);
    }

    private Dictionary<ListNode, int> GetNodeIdMap()
    {
        Dictionary<ListNode, int> dictionary = new();

        int id = 0;

        for (ListNode currentNode = head; currentNode != null; currentNode = currentNode.next)
        {
            dictionary.Add(currentNode, id);
            id++;
        }

        return dictionary;
    }

    private void WriteDataToFile(Stream fileStream, IReadOnlyDictionary<ListNode, int> map)
    {
        fileStream.Write($"{count}\n".ToBytes());

        for (ListNode currentNode = head; currentNode != null; currentNode = currentNode.next)
        {
            fileStream.Write(currentNode.data.ToBytes());

            if (currentNode.rand != null)
            {
                int randomId = map[currentNode.rand];
                fileStream.Write($" <{randomId}>".ToBytes());
            }

            fileStream.Write("\n".ToBytes());
        }
    }

    private static List<(ListNode node, int randomIndex)> InitializeNodes(IReadOnlyList<string> rows)
    {
        List<(ListNode node, int randomIndex)> nodes = new();

        for (int i = 1; i < rows.Count; i++)
        {
            string row = rows[i];
            GetDataForNode(row, out string data, out int randomIndex);

            nodes.Add((new ListNode(data), randomIndex));
        }

        return nodes;
    }

    private static void GetDataForNode(string row, out string data, out int randomIndex)
    {
        var splitRow = row
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        randomIndex = GetRandomIndex(splitRow);
        data = GetNodeData(randomIndex, row, splitRow);
    }

    private static string GetNodeData(int randomIndex, string row, IList<string> splitRow)
    {
        if (randomIndex == -1)
            return row;

        splitRow.RemoveAt(splitRow.Count - 1);
        return string.Join(" ", splitRow);
    }

    private static int GetRandomIndex(List<string> splitRow)
    {
        string lastValue = splitRow[^1];

        int randomIndex = -1;

        if (lastValue[0] != '<' || lastValue[^1] != '>')
            return randomIndex;
        
        lastValue = lastValue.Remove(0, 1);
        lastValue = lastValue.Remove(lastValue.Length - 1, 1);

        if (!int.TryParse(lastValue, out randomIndex))
            throw new Exception("Can't parse random index from string");

        return randomIndex;
    }

    private void SetLinkedListDependencies(List<(ListNode node, int randomIndex)> nodes)
    {
        if (nodes.Count < 1)
            return;

        head = nodes[0].node;

        if (nodes[0].randomIndex != -1)
            nodes[0].node.rand = nodes[nodes[0].randomIndex].node;

        for (int i = 1; i < nodes.Count; i++)
        {
            nodes[i - 1].node.next = nodes[i].node;
            nodes[i].node.prev = nodes[i - 1].node;

            if (nodes[i].randomIndex != -1)
                nodes[i].node.rand = nodes[nodes[i].randomIndex].node;
        }

        tail = nodes[^1].node;
    }

    private static string ReadData(Stream fileStream)
    {
        var b = new byte[1024];
        int readLen;

        UTF8Encoding encoding = new UTF8Encoding(true);
        var result = string.Empty;

        while ((readLen = fileStream.Read(b, 0, b.Length)) > 0)
            result = encoding.GetString(b, 0, readLen);

        return result;
    }
}