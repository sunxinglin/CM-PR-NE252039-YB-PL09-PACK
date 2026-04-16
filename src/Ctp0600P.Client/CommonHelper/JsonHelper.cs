using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ctp0600P.Client.CommonHelper;

public class JsonHelper
{
    public static string ReadJson(string path)
    {
        using StreamReader file = File.OpenText(path);
        using JsonTextReader reader = new JsonTextReader(file);
        return JToken.ReadFrom(reader).ToString();
    }

    public static void WriteJson(string path, object jObj)
    {
        using StreamWriter file = new StreamWriter(path);
        file.Write(jObj.ToString());
    }

    /// <summary>
    ///  通过序列化与反序列化实现深拷备
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T DeepCopyBySerialize<T>(T obj)
    {
        object returnObj;
        using (MemoryStream ms = new MemoryStream())
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            returnObj = formatter.Deserialize(ms);
            ms.Close();
        }
        return (T)returnObj;
    }

    /// <summary>
    /// 通过 反射实现 深拷备
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T DeepCopyByReflection<T>(T obj)
    {
        if (obj is string || obj.GetType().IsValueType)
            return obj;
        object returnObj = Activator.CreateInstance(obj.GetType());
        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        foreach (FieldInfo item in fields)
        {
            try
            {
                item.SetValue(returnObj, DeepCopyByReflection(item.GetValue(obj)));
            }
            catch { }
        }
        return (T)returnObj;
    }
}