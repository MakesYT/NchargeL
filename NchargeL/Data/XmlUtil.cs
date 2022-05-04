using System;
using System.IO;
using System.Xml.Serialization;

/// <summary>
///     Xml序列化与反序列化
/// </summary>
public class XmlUtil
{
    #region 序列化

    /// <summary>
    ///     序列化
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="obj">对象</param>
    /// <returns></returns>
    public static string Serializer(Type type, object obj)
    {
        var Stream = new MemoryStream();
        var xml = new XmlSerializer(type);
        //序列化对象
        xml.Serialize(Stream, obj);

        Stream.Position = 0;
        var sr = new StreamReader(Stream);
        var str = sr.ReadToEnd();

        sr.Dispose();
        Stream.Dispose();

        return str;
    }

    #endregion

    #region 反序列化

    /// <summary>
    ///     反序列化
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="xml">XML字符串</param>
    /// <returns></returns>
    public static object Deserialize(Type type, string xml)
    {
        try
        {
            using (var sr = new StringReader(xml))
            {
                var xmldes = new XmlSerializer(type);
                return xmldes.Deserialize(sr);
            }
        }
        catch (Exception e)
        {
            return null;
        }
    }

    /// <summary>
    ///     反序列化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static object Deserialize(Type type, Stream stream)
    {
        var xmldes = new XmlSerializer(type);
        return xmldes.Deserialize(stream);
    }

    #endregion
}