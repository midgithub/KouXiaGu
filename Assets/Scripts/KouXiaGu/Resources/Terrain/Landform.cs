﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using KouXiaGu.Terrain3D;
using KouXiaGu.Collections;
using KouXiaGu.Navigation;

namespace KouXiaGu.Resources
{

    /// <summary>
    /// 地形信息;
    /// </summary>
    [XmlType("Landform")]
    public struct LandformInfo : IElement
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlElement("Terrain")]
        public TerrainLandformInfo Terrain { get; set; }

        [XmlElement("Navigation")]
        public NavLandformInfo Navigation { get; set; }
    }

    class LandformFile : MultipleFilePath
    {
        [CustomFilePath("地形资源描述文件;", true)]
        public const string fileName = "World/Terrain/Landform.xml";

        public override string FileName
        {
            get { return fileName; }
        }
    }

    class LandformXmlSerializer : ElementsXmlSerializer<LandformInfo>
    {
        public LandformXmlSerializer() : base(new LandformFile())
        {
        }

        public LandformXmlSerializer(IFilePath file) : base(file)
        {
        }
    }

    ///// <summary>
    ///// 地形信息文件路径;
    ///// </summary>
    //public class LandformInfosFilePath : CustomFilePath
    //{
    //    public LandformInfosFilePath(string fileExtension) : base(fileExtension)
    //    {
    //    }

    //    public override string FileName
    //    {
    //        get { return "World/Landform"; }
    //    }
    //}

    ///// <summary>
    ///// 地形信息读取;
    ///// </summary>
    //public class LandformInfoXmlSerializer : DataDictionaryXmlReader<LandformInfo>
    //{
    //    public LandformInfoXmlSerializer()
    //    {
    //        file = new LandformInfosFilePath(FileExtension);
    //    }

    //    LandformInfosFilePath file;

    //    public override CustomFilePath File
    //    {
    //        get { return file; }
    //    }

    //}

}
