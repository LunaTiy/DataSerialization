using System.Text.Json;
using SerializationDeserialization.Serialization;

namespace SerializationDeserialization;

public static class Program
{
    public static void Main(string[] args)
    {
        ListNode listNode = new()
        {
            data = "current",
            next = new ListNode
            {
                data = "next"
            }
        };

        Serializer.Serialize(listNode);
    }

    public static void TestSerialize()
    {
        Test next = new Test()
        {
            Name = "Next",
            Age = 10
        };
        
        Test current = new Test()
        {
            Name = "Current",
            Age = 5,
            Next = next
        };
        
        var serialize = JsonSerializer.Serialize(current);
    }
}

public class Test
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Test Next { get; set; }
}