﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace KouXiaGu.Resources
{

    /// <summary>
    /// 从文件读取资源方式;
    /// </summary>
    public interface IOFileSerializer<T>
    {
        /// <summary>
        /// 拓展名;
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// 读取到;
        /// </summary>
        T Read(string filePath);

        /// <summary>
        /// 输出到;
        /// </summary>
        void Write(T item, string filePath, FileMode fileMode);
    }


    /// <summary>
    /// 从文件读取资源方式;
    /// </summary>
    public interface ISerializer<T>
    {
        /// <summary>
        /// 拓展名;
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// 读取到;
        /// </summary>
        T Read(Stream stream);

        /// <summary>
        /// 输出到;
        /// </summary>
        void Write(T item, Stream stream);
    }


    public sealed class ProtoFileSerializer<T> : IOFileSerializer<T>, ISerializer<T>
    {
        public static readonly ProtoFileSerializer<T> Default = new ProtoFileSerializer<T>();

        public string Extension
        {
            get { return ".xml"; }
        }

        public T Read(Stream stream)
        {
            return ProtoBuf.Serializer.Deserialize<T>(stream);
        }

        public void Write(T item, Stream stream)
        {
            ProtoBuf.Serializer.Serialize(stream, item);
        }


        public T Read(string filePath)
        {
            using (Stream fStream = new FileStream(filePath, FileMode.Open))
            {
                return ProtoBuf.Serializer.Deserialize<T>(fStream);
            }
        }

        public void Write(T item, string filePath, FileMode fileMode)
        {
            using (Stream fStream = new FileStream(filePath, fileMode))
            {
                ProtoBuf.Serializer.Serialize(fStream, item);
            }
        }
    }


    public sealed class XmlFileSerializer<T> : IOFileSerializer<T>, ISerializer<T>
    {
        public XmlFileSerializer()
        {
            Serializer = new XmlSerializer(typeof(T));
        }

        public XmlSerializer Serializer { get; private set; }

        public string Extension
        {
            get { return ".data"; }
        }

        public T Read(Stream stream)
        {
            T item = (T)Serializer.Deserialize(stream);
            return item;
        }

        public void Write(T item, Stream stream)
        {
            Serializer.SerializeXiaGu(item, stream);
        }


        public T Read(string filePath)
        {
            T item = (T)Serializer.DeserializeXiaGu(filePath);
            return item;
        }

        public void Write(T item, string filePath, FileMode fileMode)
        {
            Serializer.SerializeXiaGu(item, filePath, fileMode);
        }
    }
}
