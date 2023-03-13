using System.Text;
using SerializationDeserialization.Extensions;
using SerializationDeserialization.Serialization;

namespace SerializationDeserialization.LinkedList;

public class ListRand : ISerializable
{
    public ListNode head = null!;
    public ListNode tail = null!;
    public int count;

    public ListRand()
    {
    }

    public ListRand(ListNode head, ListNode tail, int count)
    {
        this.head = head;
        this.tail = tail;
        this.count = count;
    }

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
    }

    private void WriteDataToFile(Stream fileStream, IReadOnlyDictionary<ListNode, int> map)
    {
        fileStream.Write($"{count}\n".ToBytes());

        for (ListNode currentNode = head; currentNode != null; currentNode = currentNode.next)
        {
            fileStream.Write(currentNode.data.ToBytes());

            if (currentNode.rand != null)
            {
                var randomId = map[currentNode.rand];
                fileStream.Write($" {randomId}".ToBytes());
            }

            fileStream.Write("\n".ToBytes());
        }
    }

    private Dictionary<ListNode, int> GetNodeIdMap()
    {
        Dictionary<ListNode, int> dictionary = new();

        var id = 0;

        for (ListNode currentNode = head; currentNode != null; currentNode = currentNode.next)
        {
            dictionary.Add(currentNode, id);
            id++;
        }

        return dictionary;
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