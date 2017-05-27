﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

namespace KouXiaGu.Resources
{


    public class FileXmlSerializer<T> : IReader<T>
    {
        public FileXmlSerializer(IFilePath file)
        {
            File = file;
            serializer = new XmlSerializer(typeof(T));
        }

        public IFilePath File { get; private set; }
        protected XmlSerializer serializer { get; private set; }

        public virtual T Read()
        {
            string filePath = File.GetMainPath();
            return Read(filePath);
        }

        public T Read(string filePath)
        {
            T item = (T)serializer.DeserializeXiaGu(filePath);
            return item;
        }

        public void Write(T item)
        {
            string filePath = File.GetMainPath();
            Write(item, filePath);
        }

        public void Write(T item, string filePath)
        {
            serializer.SerializeXiaGu(filePath, item);
        }
    }


    public abstract class BasicFilesXmlSerializer<T>
    {
        public BasicFilesXmlSerializer(IFilePath file)
        {
            File = file;
            serializer = new XmlSerializer(typeof(T));
        }

        public IFilePath File { get; private set; }
        protected XmlSerializer serializer { get; private set; }

        public List<T> ReadAll()
        {
            List<T> completed = new List<T>();
            IEnumerable<string> filePaths = File.FindFiles();
            foreach (var path in filePaths)
            {
                try
                {
                    T item = Read(path);
                    completed.Add(item);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return completed;
        }

        public T Read(string filePath)
        {
            T item = (T)serializer.DeserializeXiaGu(filePath);
            return item;
        }

        public void Write(T item)
        {
            string filePath = File.GetMainPath();
            Write(item, filePath);
        }

        public void Write(T item, string filePath)
        {
            serializer.SerializeXiaGu(filePath, item);
        }
    }

    /// <summary>
    /// 使多个相同类型结合方法;
    /// </summary>
    public interface ICombiner<T>
    {
        T Combine(IEnumerable<T> items);
    }

    /// <summary>
    /// 多个文件读取;
    /// </summary>
    public class FilesXmlSerializer<T> : BasicFilesXmlSerializer<T>, IReader<T>
    {
        public FilesXmlSerializer(IFilePath multipleFile, ICombiner<T> combiner) : base(multipleFile)
        {
            Combiner = combiner;
        }

        public ICombiner<T> Combiner { get; private set; }

        public T Read()
        {
            List<T> items = ReadAll();
            T item = Combiner.Combine(items);
            return item;
        }
    }


    public interface ICombiner<TSource, TResult>
    {
        TResult Combine(IEnumerable<TSource> items);
    }

    /// <summary>
    /// 读取多个
    /// </summary>
    /// <typeparam name="TSource">从文件读取到的内容;</typeparam>
    /// <typeparam name="TResult">转换后的内容;</typeparam>
    public class FilesXmlSerializer<TSource, TResult> : BasicFilesXmlSerializer<TSource>, IReader<TResult>
    {
        public FilesXmlSerializer(IFilePath multipleFile, ICombiner<TSource, TResult> combiner) : base(multipleFile)
        {
            Combiner = combiner;
        }

        public ICombiner<TSource, TResult> Combiner { get; private set; }

        public TResult Read()
        {
            List<TSource> items = ReadAll();
            TResult item = Combiner.Combine(items);
            return item;
        }
    }
}
