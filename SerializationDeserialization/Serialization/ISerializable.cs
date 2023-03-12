namespace SerializationDeserialization.Serialization;

public interface ISerializable
{
    void Serialize(FileStream fileStream);
    void Deserialize(FileStream fileStream);
}