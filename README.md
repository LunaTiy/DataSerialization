<div id="header" align="center">
   <h1>
    JSON Serializer
  </h1>  
Serializer and deserializer for any objects, and linked list serializer (not json)
</div>

## 🤔 About

Serializer save any data to json format, as JsonSerializer. But there is a problem with these serializers not being able to store a linked list, 
due to cycles of objects referencing each other.

Therefore, in order to save the linked list, I wrote a simple and fast implementation of the serializer.

## ⚙️ Technologies

.NET 7

C# 11