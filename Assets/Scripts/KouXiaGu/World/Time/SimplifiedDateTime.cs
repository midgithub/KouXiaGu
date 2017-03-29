﻿using System;
using System.Xml.Serialization;
using ProtoBuf;
using UnityEngine;

namespace KouXiaGu.World
{

    /// <summary>
    /// 仅记录年月日;
    /// </summary>
    [Serializable, XmlType("SimplifiedDateTime"), ProtoContract]
    public struct SimplifiedDateTime : IEquatable<SimplifiedDateTime>, IComparable<SimplifiedDateTime>
    {

        /// <summary>
        /// 一年一月一日;
        /// </summary>
        const int DEFAULT_TICKS = 0x0;

        /// <summary>
        /// 一年一月一日,默认的日历;
        /// </summary>
        public static SimplifiedDateTime Default
        {
            get { return new SimplifiedDateTime(DEFAULT_TICKS); }
        }


        public SimplifiedDateTime(DateTime time)
        {
            this.ticks = GetSimplifiedDateTimeTicks(time);
        }

        public SimplifiedDateTime(int ticks)
        {
            this.ticks = ticks;
        }

        public SimplifiedDateTime(short year, byte month, byte day)
        {
            this.ticks = DEFAULT_TICKS;

            this.Year = year;
            this.Month = month;
            this.Day = day;
        }


        [SerializeField, ProtoMember(1)]
        int ticks;

        /// <summary>
        /// 周期数;年占用前两个字节,其后到月占用一字节,日占用一字节;
        /// </summary>
        [XmlAttribute("ticks")]
        public int Ticks
        {
            get { return ticks; }
            private set { ticks = value; }
        }


        /// <summary>
        /// 年; -32,768 到 32,767
        /// </summary>
        public short Year
        {
            get { return (short)(Ticks >> 16); }
            private set { Ticks = (Ticks & 0xFFFF) | ((int)value << 16); }
        }

        /// <summary>
        /// 月; 1 至 n;
        /// </summary>
        public byte Month
        {
            get { return (byte)((Ticks & 0xFF00) >> 8); }
            private set { Ticks = (Ticks & -0xFF01) | (value << 8); }
        }

        /// <summary>
        /// 日; 1 至 n;
        /// </summary>
        public byte Day
        {
            get { return (byte)(Ticks & 0xFF); }
            private set { Ticks = (Ticks & -0x100) | value; }
        }


        public int CompareTo(SimplifiedDateTime other)
        {
            return this.Ticks - other.Ticks;
        }

        public bool Equals(SimplifiedDateTime other)
        {
            return this.Ticks == other.Ticks;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SimplifiedDateTime))
                return false;

            return Equals((SimplifiedDateTime)obj);
        }

        public override string ToString()
        {
            return "[Year:" + Year + ",Month:" + Month + ",Day:" + Day + ",Ticks:" + Ticks + "]";
        }

        public override int GetHashCode()
        {
            return this.Ticks;
        }


        public static bool operator ==(SimplifiedDateTime v1, SimplifiedDateTime v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(SimplifiedDateTime v1, SimplifiedDateTime v2)
        {
            return !v1.Equals(v2);
        }

        public static bool operator >(SimplifiedDateTime v1, SimplifiedDateTime v2)
        {
            return v1.Ticks > v2.Ticks;
        }

        public static bool operator <(SimplifiedDateTime v1, SimplifiedDateTime v2)
        {
            return v1.Ticks < v2.Ticks;
        }


        public static bool operator ==(DateTime v1, SimplifiedDateTime v2)
        {
            return GetSimplifiedDateTimeTicks(v1) == v2.Ticks;
        }

        public static bool operator !=(DateTime v1, SimplifiedDateTime v2)
        {
            return GetSimplifiedDateTimeTicks(v1) != v2.Ticks;
        }

        public static bool operator >(DateTime v1, SimplifiedDateTime v2)
        {
            return GetSimplifiedDateTimeTicks(v1) > v2.Ticks;
        }

        public static bool operator <(DateTime v1, SimplifiedDateTime v2)
        {
            return GetSimplifiedDateTimeTicks(v1) < v2.Ticks;
        }


        /// <summary>
        /// 转换为int类型的周期数,仅有 年月日;
        /// </summary>
        static int GetSimplifiedDateTimeTicks(DateTime v)
        {
            return (int)(v.Ticks >> 32);
        }

        public static implicit operator SimplifiedDateTime(DateTime v)
        {
            return new SimplifiedDateTime(GetSimplifiedDateTimeTicks(v));
        }

    }

}
