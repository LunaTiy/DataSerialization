using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace SerializationDeserialization.Serialization;

public static class Serializer
{
    private const string StartBlock = "{";
    private const string EndBlock = "}";
    private const string NameValueSeparator = ":";
    private const char FieldsSeparator = ',';

    private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Instance |
                                              System.Reflection.BindingFlags.Public |
                                              System.Reflection.BindingFlags.Static;

    private static readonly TypeConverter converter = new();

    public static string Serialize<T>(T data)
    {
        Type type = data!.GetType();
        var fieldInfos = GetPublicFieldsInfo(type);

        return ConstructSerializeData(data, fieldInfos);
    }

    public static T Deserialize<T>(string serializedData) where T : new()
    {
        Type type = typeof(T);

        var splitData =
            serializedData.Split(FieldsSeparator, '\"');

        return ConstructDeserializedData<T>(type, splitData);
    }

    private static string ConstructSerializeData<T>(T data, IEnumerable<FieldInfo> fieldInfos)
    {
        StringBuilder serializedData = new();

        serializedData.Append(StartBlock);

        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            var fieldName = fieldInfo.Name;
            var fieldData = string.Empty;

            var value = fieldInfo.GetValue(data);

            if (value != null)
                fieldData = IsBaseType(value) ? value.ToString() : Serialize(value);

            serializedData.Append($"\"{fieldName}\"{NameValueSeparator}\"{fieldData}\"{FieldsSeparator}");
        }

        if (serializedData[^1] == FieldsSeparator)
            serializedData.Remove(serializedData.Length - 1, 1);

        serializedData.Append(EndBlock);
        return serializedData.ToString();
    }

    private static T ConstructDeserializedData<T>(IReflect type, string[] splitData) where T : new()
    {
        T obj = new T();

        var isStartObject = false;
        var isPrevFieldName = false;

        FieldInfo? currentField = null;

        for (var i = 0; i < splitData.Length; i++)
        {
            var data = splitData[i];

            if (data == NameValueSeparator || !isPrevFieldName && string.IsNullOrEmpty(data))
                continue;

            if (data == EndBlock && isStartObject)
                return obj;

            if (data == StartBlock)
            {
                if (isStartObject)
                {
                    SetComplexData(ref splitData, i, currentField, obj);

                    isPrevFieldName = false;
                    continue;
                }

                isStartObject = true;
                continue;
            }

            if (!isPrevFieldName && TryGetFieldInfo(type, data, out FieldInfo? fieldInfo))
            {
                currentField = fieldInfo;
                isPrevFieldName = true;
            }
            else
            {
                SetSimpleData(data, currentField, obj);

                isPrevFieldName = false;
            }
        }

        return obj;
    }

    private static void SetComplexData<T>(ref string[] splitData, int i, FieldInfo? currentField, [DisallowNull] T obj)
        where T : new()
    {
        var newSplitData = GetSubObjectSplitData(ref splitData, i);
        var result = ConstructSubObject(newSplitData, currentField!.FieldType);

        currentField?.SetValue(obj, result);
    }

    private static void SetSimpleData<T>(string data, FieldInfo? currentField, [DisallowNull] T obj) where T : new()
    {
        if (!string.IsNullOrEmpty(data))
            currentField?.SetValue(obj, converter.ConvertTo(data, currentField.FieldType));
    }

    private static IEnumerable<string> GetSubObjectSplitData(ref string[] splitData, int i)
    {
        var endBlockIndex = Array.IndexOf(splitData, EndBlock, i) + 1;
        var newSplitData = splitData[i..endBlockIndex];

        var list = splitData.ToList();
        list.RemoveRange(i, endBlockIndex - i);
        splitData = list.ToArray();

        return newSplitData;
    }

    private static object? ConstructSubObject(IEnumerable splitData, Type type)
    {
        MethodInfo methodInfo = typeof(Serializer).GetMethod(nameof(ConstructDeserializedData),
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;

        var genericMethodInfo = methodInfo.MakeGenericMethod(type);

        var result = genericMethodInfo.Invoke(new object(), new object?[] { type, splitData });

        return result;
    }

    private static bool TryGetFieldInfo(IReflect type, string data, out FieldInfo? fieldInfo)
    {
        fieldInfo = type.GetField(data, BindingFlags);

        return fieldInfo != null;
    }

    private static IEnumerable<FieldInfo> GetPublicFieldsInfo(IReflect type) =>
        type.GetFields(BindingFlags);

    private static bool IsBaseType(object? value) =>
        value is bool or char or float or double or int or string;
}