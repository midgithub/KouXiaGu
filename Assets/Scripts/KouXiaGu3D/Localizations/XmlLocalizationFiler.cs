﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using KouXiaGu.Collections;
using UnityEngine;

namespace KouXiaGu.Localizations
{


    public class XmlLocalizationFiler : ILocalizationReader
    {

        const string ROOT_ELEMENT_NAME = "Localization";
        const string TEXT_ELEMENT_NAME = "Text";
        const string KEY_ATTRIBUTE_NAME = "key";
        const string VALUE_ATTRIBUTE_NAME = "value";
        const string UPDATE_MARK_ATTRIBUTE_NAME = "update";

        const bool DEFAULT_UPDATE_MARK = false;

        XmlLocalizationFiler(string filePath)
        {
            this.FilePath = filePath;
        }

        public string FilePath { get; private set; }

        static readonly XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
        {
            Indent = true,
            NewLineChars = Environment.NewLine,
            NewLineOnAttributes = false,
            Encoding = Encoding.UTF8,
        };

        static readonly XmlReaderSettings xmlReaderSettings = new XmlReaderSettings()
        {
            IgnoreWhitespace = true,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
        };


        public IEnumerable<TextPack> ReadTexts()
        {
            using (XmlReader reader = XmlReader.Create(FilePath, xmlReaderSettings))
            {
                return ReadTexts(reader);
            }
        }

        public static IEnumerable<TextPack> ReadTexts(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.IsStartElement(TEXT_ELEMENT_NAME))
                {
                    string key = null;
                    string value = null;
                    bool updateMark = DEFAULT_UPDATE_MARK;

                    while (reader.MoveToNextAttribute())
                    {
                        switch (reader.Name)
                        {
                            case KEY_ATTRIBUTE_NAME:
                                key = reader.Value;
                                break;
                            case VALUE_ATTRIBUTE_NAME:
                                value = reader.Value;
                                break;
                            case UPDATE_MARK_ATTRIBUTE_NAME:
                                updateMark = reader.ReadContentAsBoolean();
                                break;
                        }
                    }

                    if (key != null && value != null)
                        yield return new TextPack(key, value, updateMark);
                }
            }
        }


        /// <summary>
        /// 保存所有文字结构到XML;
        /// </summary>
        public static void WriteTexts(XmlWriter writer, IEnumerable<TextPack> texts)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement(ROOT_ELEMENT_NAME);

            foreach (var pair in texts)
            {
                string key = pair.Key;
                string value = pair.Value;
                bool updateMark = pair.IsUpdate;

                writer.WriteStartElement(TEXT_ELEMENT_NAME);

                writer.WriteStartAttribute(KEY_ATTRIBUTE_NAME);
                writer.WriteString(key);

                writer.WriteStartAttribute(VALUE_ATTRIBUTE_NAME);
                writer.WriteString(value);

                if (updateMark != DEFAULT_UPDATE_MARK)
                {
                    writer.WriteStartAttribute(UPDATE_MARK_ATTRIBUTE_NAME);
                    writer.WriteValue(updateMark);
                }

                writer.WriteEndElement();
            }

            writer.WriteEndDocument();
        }


    }

}