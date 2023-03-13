using SerializationDeserialization.Serialization;

namespace SerializationDeserialization;

public static class Program
{
    public static void Main(string[] args)
    {
        SerializeNode();
        SerializeList();
    }

    private static void SerializeNode()
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

    private static void SerializeList()
    {
        ListRand list = ConfigureList();
    }

    private static ListRand ConfigureList()
    {
        ListRand list = new ListRand();

        ListNode head = new ListNode("It's head");
        ListNode inside1 = new ListNode("Inside 1 node", head);
        ListNode inside2 = new ListNode("Inside 2 node", inside1);
        ListNode inside3 = new ListNode("Inside 3 node", inside2);
        ListNode tail = new ListNode("It's tail", inside3);

        head.next = inside1;
        
        inside1.next = inside2;
        inside1.rand = inside1;

        inside2.next = inside3;
        inside2.rand = tail;

        inside3.next = tail;
        inside3.rand = head;

        list.head = head;
        list.tail = tail;
        list.count = 5;

        return list;
    }
}