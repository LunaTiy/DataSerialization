using SerializationDeserialization.Serialization;

namespace SerializationDeserialization;

public class ListRand : ISerializable
{
    public ListNode head;
    public ListNode tail;
    public int count;

    public ListRand() { }

    public ListRand(ListNode head, ListNode tail, int count)
    {
        this.head = head;
        this.tail = tail;
        this.count = count;
    }

    public void Serialize(FileStream fileStream)
    {
        
    }

    public void Deserialize(FileStream fileStream)
    {
        
    }
}