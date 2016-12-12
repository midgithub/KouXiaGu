﻿using System;
using System.Collections.Generic;
using System.Linq;
using KouXiaGu.Grids;
using ProtoBuf;
using UnityEngine;

namespace KouXiaGu.Grids
{

    /// <summary>
    /// 矩形网格存在的方向;
    /// </summary>
    [Flags]
    public enum RecDirections
    {
        North = 1,
        Northeast = 2,
        East = 4,
        Southeast = 8,
        South = 16,
        Southwest = 32,
        West = 64,
        Northwest = 128,
        Self = 256,
    }


    /// <summary>
    /// 矩形网格的坐标;
    /// </summary>
    [Serializable, ProtoContract]
    public struct RectCoord : IEquatable<RectCoord>, IGrid, IGrid<RecDirections>, IGridCoord
    {

        /// <summary>
        /// 存在方向数;
        /// </summary>
        public const int DirectionsNumber = 9;

        const int maxDirectionMark = (int)RecDirections.Self;
        const int minDirectionMark = (int)RecDirections.North;

        public static readonly RectCoord North = new RectCoord(0, 1);
        public static readonly RectCoord South = new RectCoord(0, -1);
        public static readonly RectCoord West = new RectCoord(-1, 0);
        public static readonly RectCoord East = new RectCoord(1, 0);
        public static readonly RectCoord Self = new RectCoord(0, 0);

        public static readonly RectCoord Northeast = North + East;
        public static readonly RectCoord Southeast = South + East;
        public static readonly RectCoord Southwest = South + West;
        public static readonly RectCoord Northwest = North + West;

        /// <summary>
        /// 方向偏移量;
        /// </summary>
        static Dictionary<int, RectCoord> directionsVector = new Dictionary<int, RectCoord>()
        {
            { (int)RecDirections.North, North },
            { (int)RecDirections.Northeast, Northeast },
            { (int)RecDirections.East, East},
            { (int)RecDirections.Southeast, Southeast },
            { (int)RecDirections.South, South},
            { (int)RecDirections.Southwest, Southwest },
            { (int)RecDirections.West, West},
            { (int)RecDirections.Northwest, Northwest },
            { (int)RecDirections.Self, Self},

        };

        /// <summary>
        /// 按标记为从 高位到低位 循序排列的数组;不包含本身
        /// </summary>
        static readonly RecDirections[] DirectionsArray = new RecDirections[]
        {
            RecDirections.Northwest,
            RecDirections.West,
            RecDirections.Southwest,
            RecDirections.South,
            RecDirections.Southeast,
            RecDirections.East,
            RecDirections.Northeast,
            RecDirections.North,
        };

        /// <summary>
        /// 按标记为从 高位到低位 循序排列的数组(存在本身方向,且在最高位);
        /// </summary>
        static readonly RecDirections[] DirectionsAndSelfArray = new RecDirections[]
        {
            RecDirections.Self,
            RecDirections.Northwest,
            RecDirections.West,
            RecDirections.Southwest,
            RecDirections.South,
            RecDirections.Southeast,
            RecDirections.East,
            RecDirections.Northeast,
            RecDirections.North,
        };


        [ProtoMember(1)]
        public short X { get; set; }

        [ProtoMember(2)]
        public short Y { get; set; }

        public RectCoord(short x, short y)
        {
            this.X = x;
            this.Y = y;
        }

        public RectCoord(int x, int y)
        {
            this.X = (short)x;
            this.Y = (short)y;
        }

        public void SetValue(short x, short y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return String.Concat("(", X, " , ", Y, ")");
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RectCoord))
                return false;
            return Equals((RectCoord)obj);
        }

        public bool Equals(RectCoord other)
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        /// 根据位置确定哈希值;
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashCode = X << 16;
            hashCode += short.MaxValue + Y;
            return hashCode;
        }


        /// <summary>
        /// 获取到这个方向的坐标;
        /// </summary>
        public RectCoord GetDirection(RecDirections direction)
        {
            return this + GetDirectionOffset(direction);
        }

        /// <summary>
        /// 获取到目标点的邻居节点;
        /// </summary>
        public IEnumerable<CoordPack<RectCoord, RecDirections>> GetNeighbours()
        {
            foreach (var direction in Directions)
            {
                yield return new CoordPack<RectCoord, RecDirections>(this.GetDirection(direction), direction);
            }
        }

        /// <summary>
        /// 获取到目标点的邻居节点;
        /// </summary>
        public IEnumerable<CoordPack<RectCoord, RecDirections>> GetNeighbours(RecDirections directions)
        {
            foreach (var direction in GetDirections(directions))
            {
                yield return new CoordPack<RectCoord, RecDirections>(this.GetDirection(direction), direction);
            }
        }

        /// <summary>
        /// 获取到目标点的邻居节点,但是也返回自己本身;
        /// </summary>
        public IEnumerable<CoordPack<RectCoord, RecDirections>> GetNeighboursAndSelf()
        {
            foreach (var direction in DirectionsAndSelf)
            {
                yield return new CoordPack<RectCoord, RecDirections>(this.GetDirection(direction), direction);
            }
        }


        IEnumerable<IGrid> IGrid.GetNeighbours()
        {
            return GetNeighbours().Select(coord => coord.Point).Cast<IGrid>();
        }

        IEnumerable<IGrid> IGrid.GetNeighboursAndSelf()
        {
            return GetNeighboursAndSelf().Select(coord => coord.Point).Cast<IGrid>();
        }

        IEnumerable<CoordPack<IGrid<RecDirections>, RecDirections>> IGrid<RecDirections>.GetNeighbours()
        {
            return GetNeighbours().Select(coord => new CoordPack<IGrid<RecDirections>, RecDirections>(coord.Point, coord.Item));
        }

        IEnumerable<CoordPack<IGrid<RecDirections>, RecDirections>> IGrid<RecDirections>.GetNeighboursAndSelf()
        {
            return GetNeighboursAndSelf().Select(coord => new CoordPack<IGrid<RecDirections>, RecDirections>(coord.Point, coord.Item));
        }



        /// <summary>
        /// 将哈希值转换成坐标;
        /// </summary>
        public static RectCoord HashCodeToVector(int hashCode)
        {
            short x = (short)(hashCode >> 16);
            short y = (short)((hashCode & 0xFFFF) - short.MaxValue);
            return new RectCoord(x, y);
        }


        /// <summary>
        /// 获取这两个点的距离;
        /// </summary>
        public static float Distance(RectCoord v1, RectCoord v2)
        {
            float distance = (float)Math.Sqrt(Math.Pow((v1.X - v2.X), 2) + Math.Pow((v1.Y - v2.Y), 2));
            return distance;
        }

        /// <summary>
        /// 获取这两个点的曼哈顿距离;
        /// </summary>
        public static int ManhattanDistance(RectCoord v1, RectCoord v2)
        {
            int distance = Math.Abs(v1.X - v2.X) + Math.Abs(v1.Y - v2.Y);
            return distance;
        }


        /// <summary>
        /// 按标记为从 高位到低位 循序返回的迭代结构;不包含本身
        /// </summary>
        public static IEnumerable<RecDirections> Directions
        {
            get { return DirectionsArray; }
        }

        /// <summary>
        /// 获取到从 高位到低位 顺序返回的迭代结构;(存在本身方向,且在最高位);
        /// </summary>
        public static IEnumerable<RecDirections> DirectionsAndSelf
        {
            get { return DirectionsAndSelfArray; }
        }

        /// <summary>
        /// 获取到方向偏移量;
        /// </summary>
        public static RectCoord GetDirectionOffset(RecDirections direction)
        {
            return directionsVector[(int)direction];
        }

        /// <summary>
        /// 获取到方向集表示的所有方向;
        /// </summary>
        public static IEnumerable<RecDirections> GetDirections(RecDirections directions)
        {
            int mask = (int)directions;
            for (int intDirection = minDirectionMark; intDirection <= maxDirectionMark; intDirection <<= 1)
            {
                if ((intDirection & mask) == 1)
                {
                    yield return (RecDirections)intDirection;
                }
            }
        }


        /// <summary>
        /// 获取到这个范围所有的点;
        /// </summary>
        public static IEnumerable<RectCoord> RectRange(RectCoord southwest, RectCoord northeast)
        {
            for (short x = southwest.X; x <= northeast.X; x++)
            {
                for (short y = southwest.Y; y <= northeast.Y; y++)
                {
                    yield return new RectCoord(x, y);
                }
            }
        }

        public static bool operator ==(RectCoord a, RectCoord b)
        {
            return  a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(RectCoord a, RectCoord b)
        {
            return !(a == b);
        }

        public static RectCoord operator -(RectCoord point1, RectCoord point2)
        {
            point1.X -= point2.X;
            point1.Y -= point2.Y;
            return point1;
        }

        public static RectCoord operator +(RectCoord point1, RectCoord point2)
        {
            point1.X += point2.X;
            point1.Y += point2.Y;
            return point1;
        }

        public static RectCoord operator *(RectCoord point1, short n)
        {
            point1.X *= n;
            point1.Y *= n;
            return point1;
        }

        public static RectCoord operator /(RectCoord point1, short n)
        {
            point1.X /= n;
            point1.Y /= n;
            return point1;
        }

        public static RectCoord operator +(RectCoord point1, short n)
        {
            point1.X += n;
            point1.Y += n;
            return point1;
        }

        public static RectCoord operator -(RectCoord point1, short n)
        {
            point1.X -= n;
            point1.Y -= n;
            return point1;
        }

    }
}
