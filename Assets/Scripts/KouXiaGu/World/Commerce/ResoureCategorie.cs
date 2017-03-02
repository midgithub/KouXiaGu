﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KouXiaGu.World.Commerce
{

    /// <summary>
    /// 资源类别;
    /// </summary>
    public class ResoureCategorie : IEquatable<ResoureCategorie>
    {

        /// <summary>
        /// 编号;
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// 是否为可存储的;
        /// </summary>
        public bool Storable { get; private set; }


        public bool Equals(ResoureCategorie other)
        {
            return other.ID == this.ID;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ResoureCategorie))
                return false;

            return Equals((ResoureCategorie)obj);
        }

        public override int GetHashCode()
        {
            return ID;
        }

    }

}
