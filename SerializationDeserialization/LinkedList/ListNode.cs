namespace SerializationDeserialization.LinkedList;

public class ListNode
{
    public string data = null!;
    public ListNode prev = null!;
    public ListNode next = null!;
    public ListNode rand = null!;

    public ListNode() { }
    
    public ListNode(string data, ListNode prev = null!, ListNode next = null!, ListNode rand = null!)
    {
        this.data = data;
        this.prev = prev;
        this.next = next;
        this.rand = rand;
    }
}