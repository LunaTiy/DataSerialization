using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace SerializationDeserialization.Serialization;

public static class Serializer
{
    public static string Serialize<T>(T data)
    {
        Type type = data!.GetType();
        var fieldInfos = GetPublicFieldsInfo(type);

        return ConstructSerializeData(data, fieldInfos);
    }

    public static object Deserialize<T>(string serializedData)
    {
        return null!;
    }

    private static string ConstructSerializeData<T>(T data, IEnumerable<FieldInfo> fieldInfos)
    {
        StringBuilder serializedData = new();

        serializedData.Append('{');

        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            var fieldName = fieldInfo.Name;
            var fieldData = string.Empty;

            var value = fieldInfo.GetValue(data);
            
            if (value != null)
                fieldData = IsBaseType(value) ? 
                    value.ToString() : Serialize(value);

            serializedData.Append($"\"{fieldName}\":\"{fieldData}\",");
        }

        if (serializedData[^1] == ',')
            serializedData.Remove(serializedData.Length - 1, 1);

        serializedData.Append('}');
        return serializedData.ToString();
    }

    private static IEnumerable<FieldInfo> GetPublicFieldsInfo(IReflect type) =>
        type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

    private static bool IsBaseType(object? value) => 
        value is bool or char or float or double or int or string;
}