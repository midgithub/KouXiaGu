﻿using ICSharpCode.SharpZipLib.Zip;
using JiongXiaGu.Unity.Resources;
using System;
using System.IO;

namespace JiongXiaGu.Unity.Localizations
{

    /// <summary>
    /// 语言包读写器;
    /// </summary>
    public class LanguagePackSerializer
    {
        private const string descriptionFileName = "Description.xml";
        private const string dictionaryFileExtension = ".xml";
        private readonly Lazy<XmlSerializer<LanguagePackDescription>> descriptionSerializer = new Lazy<XmlSerializer<LanguagePackDescription>>();
        private readonly Lazy<XmlSerializer<LanguageKeyValueList>> languageListSerializer = new Lazy<XmlSerializer<LanguageKeyValueList>>();
        private readonly Lazy<XmlSerializer<LanguageDictionary>> dictionarySerializer = new Lazy<XmlSerializer<LanguageDictionary>>();


        /// <summary>
        /// 反序列化语言包内容;字典结构为 LanguageDictionary;
        /// </summary>
        public LanguagePack DeserializePack(Stream stream)
        {
            LanguageDictionary dictionary;
            LanguagePackDescription description;

            description = Deserialize(stream, out dictionary);
            return new LanguagePack(description, dictionary);
        }

        /// <summary>
        /// 反序列化语言包内容;字典结构为 LanguageSplitDictionary;
        /// </summary>
        public LanguagePack DeserializeSplit(Stream stream)
        {
            LanguageSplitDictionary dictionary;
            LanguagePackDescription description;

            description = Deserialize(stream, out dictionary);
            return new LanguagePack(description, dictionary);
        }

        /// <summary>
        /// 反序列化描述文件;
        /// </summary>
        public LanguagePackDescription Deserialize(Stream stream)
        {
            using (ZipInputStream zipInputStream = new ZipInputStream(stream))
            {
                zipInputStream.IsStreamOwner = false;

                ZipEntry entry;
                while ((entry = zipInputStream.GetNextEntry()) != null)
                {
                    if (entry.IsFile && entry.Name == descriptionFileName)
                    {
                        var description = descriptionSerializer.Value.Deserialize(zipInputStream);
                        return description;
                    }
                }
                throw new FileNotFoundException();
            }
        }

        /// <summary>
        /// 反序列语言包内容;
        /// </summary>
        public LanguagePackDescription Deserialize(Stream stream, out LanguageDictionary dictionary)
        {
            LanguagePackDescription? description = null;
            dictionary = null;

            using (ZipInputStream zipInputStream = new ZipInputStream(stream))
            {
                zipInputStream.IsStreamOwner = false;

                ZipEntry entry;
                while ((entry = zipInputStream.GetNextEntry()) != null)
                {
                    if (entry.IsFile)
                    {
                        if (!description.HasValue && entry.Name == descriptionFileName)
                        {
                            description = descriptionSerializer.Value.Deserialize(zipInputStream);
                        }
                        else if(entry.Name.EndsWith(dictionaryFileExtension))
                        {
                            var value = languageListSerializer.Value.Deserialize(zipInputStream);

                            if (dictionary == null)
                            {
                                dictionary = new LanguageDictionary();
                            }

                            dictionary.Add(value);
                        }
                    }
                }
            }

            if (description.HasValue)
            {
                return description.Value;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// 反序列语言包内容;
        /// </summary>
        public LanguagePackDescription Deserialize(Stream stream, out LanguageSplitDictionary dictionary)
        {
            LanguagePackDescription? description = null;
            dictionary = null;

            using (ZipInputStream zipInputStream = new ZipInputStream(stream))
            {
                zipInputStream.IsStreamOwner = false;

                ZipEntry entry;
                while ((entry = zipInputStream.GetNextEntry()) != null)
                {
                    if (entry.IsFile)
                    {
                        if (!description.HasValue && entry.Name == descriptionFileName)
                        {
                            description = descriptionSerializer.Value.Deserialize(zipInputStream);
                        }
                        else if (entry.Name.EndsWith(dictionaryFileExtension))
                        {
                            var value = languageListSerializer.Value.Deserialize(zipInputStream);

                            if (dictionary == null)
                            {
                                dictionary = new LanguageSplitDictionary();
                            }

                            string tag = Path.GetFileNameWithoutExtension(entry.Name);
                            dictionary.Add(tag, value);
                        }
                    }
                }
            }

            if (description.HasValue)
            {
                return description.Value;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }


        /// <summary>
        /// 序列化语言包内容;
        /// </summary>
        public void Serialize(Stream stream, LanguagePack pack)
        {
            Serialize(stream, pack.Description, pack.LanguageDictionary);
        }

        /// <summary>
        /// 序列化语言包内容;
        /// </summary>
        public void Serialize(Stream stream, LanguagePackDescription description, ILanguageDictionary dictionary)
        {
            if (dictionary == null)
            {
                Serialize(stream, description);
            }
            else if (dictionary is LanguageDictionary)
            {
                Serialize(stream, description, dictionary as LanguageDictionary);
            }
            else if (dictionary is LanguageSplitDictionary)
            {
                Serialize(stream, description, dictionary as LanguageSplitDictionary);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// 仅序列化语言包描述;
        /// </summary>
        public void Serialize(Stream stream, LanguagePackDescription description)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using (ZipOutputStream zipOutputStream = new ZipOutputStream(stream))
            {
                zipOutputStream.IsStreamOwner = false;

                ZipEntry descZipEntry = new ZipEntry(descriptionFileName);
                zipOutputStream.PutNextEntry(descZipEntry);
                descriptionSerializer.Value.Serialize(zipOutputStream, description);
            }
        }

        /// <summary>
        /// 序列化语言包内容;
        /// </summary>
        public void Serialize(Stream stream, LanguagePackDescription description, LanguageDictionary dictionary)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            using (ZipOutputStream zipOutputStream = new ZipOutputStream(stream))
            {
                zipOutputStream.IsStreamOwner = false;

                ZipEntry descZipEntry = new ZipEntry(descriptionFileName);
                zipOutputStream.PutNextEntry(descZipEntry);
                descriptionSerializer.Value.Serialize(zipOutputStream, description);

                const string name = "Main" + dictionaryFileExtension;
                ZipEntry dictionaryZipEntry = new ZipEntry(name);
                zipOutputStream.PutNextEntry(dictionaryZipEntry);
                dictionarySerializer.Value.Serialize(zipOutputStream, dictionary);
            }
        }

        /// <summary>
        /// 输出语言包;
        /// </summary>
        public void Serialize(Stream stream, LanguagePackDescription description, LanguageSplitDictionary dictionary)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            using (ZipOutputStream zipOutputStream = new ZipOutputStream(stream))
            {
                zipOutputStream.IsStreamOwner = false;

                ZipEntry descZipEntry = new ZipEntry(descriptionFileName);
                zipOutputStream.PutNextEntry(descZipEntry);
                descriptionSerializer.Value.Serialize(zipOutputStream, description);

                SplitLanguageCollection collection = dictionary.Split();

                foreach (var value in collection.List)
                {
                    string name = value.Key + dictionaryFileExtension;
                    var keyValueList = value.Value;

                    ZipEntry dictionaryZipEntry = new ZipEntry(name);
                    zipOutputStream.PutNextEntry(dictionaryZipEntry);
                    languageListSerializer.Value.Serialize(zipOutputStream, keyValueList);
                }
            }
        }





        ///// <summary>
        ///// 读取语言包;
        ///// </summary>
        //[Obsolete]
        //public LanguagePack DeserializePack(Stream stream)
        //{
        //    LanguagePackDescription? description = null;
        //    LanguageDictionary dictionary = null;

        //    using (ZipInputStream zipInputStream = new ZipInputStream(stream))
        //    {
        //        zipInputStream.IsStreamOwner = false;

        //        ZipEntry entry;
        //        while ((entry = zipInputStream.GetNextEntry()) != null)
        //        {
        //            if (entry.IsFile)
        //            {
        //                if (!description.HasValue && entry.Name == descriptionFileName)
        //                {
        //                    description = descriptionSerializer.Value.Deserialize(zipInputStream);
        //                }
        //                else if (entry.Name.EndsWith(dictionaryFileExtension))
        //                {
        //                    var value = languageSerializer.Value.Deserialize(zipInputStream);

        //                    if (dictionary == null)
        //                    {
        //                        dictionary = new LanguageDictionary();
        //                    }

        //                    dictionary.Add(value.EnumerateKeyValue());
        //                }
        //                else
        //                {
        //                    continue;
        //                }
        //            }
        //        }

        //        if (description.HasValue && dictionary != null)
        //            return new LanguagePack(description.Value, dictionary);
        //        else
        //            throw new FileNotFoundException();
        //    }
        //}

        ///// <summary>
        ///// 输出语言包;
        ///// </summary>
        //public void SerializePack(Stream stream, LanguagePack pack)
        //{
        //    using (ZipOutputStream zipOutputStream = new ZipOutputStream(stream))
        //    {
        //        zipOutputStream.IsStreamOwner = false;

        //        ZipEntry descZipEntry = new ZipEntry(descriptionFileName);
        //        zipOutputStream.PutNextEntry(descZipEntry);
        //        descriptionSerializer.Value.Serialize(zipOutputStream, pack.Description);

        //        ZipEntry dictionaryZipEntry = new ZipEntry(dictionaryFileExtension);
        //        zipOutputStream.PutNextEntry(dictionaryZipEntry);
        //        dictionarySerializer.Serialize(zipOutputStream, pack.LanguageDictionary);
        //    }
        //}
    }
}
