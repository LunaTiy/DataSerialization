using SerializationDeserialization.Serialization;

namespace SerializationDeserialization;

public static class Program
{
    public static void Main(string[] args)
    {
        ListNode listNode = new()
        {
            data = "current node",
            next = new ListNode
            {
                data = "next node"
            }
        };

        string serializedData = Serializer.Serialize(listNode);
        ListNode deserializeData = Serializer.Deserialize<ListNode>(serializedData);
    }
}