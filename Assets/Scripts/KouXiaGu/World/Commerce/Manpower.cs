﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KouXiaGu.World.Commerce
{


    /// <summary>
    /// 人力资源全局影响信息;
    /// </summary>
    public class ManpowerNationalFactor
    {

        public ManpowerNationalFactor(WorldInfo info) : this(info.ProportionOfDailyGrowth)
        {
        }

        public ManpowerNationalFactor(float proportionOfDailyGrowth)
        {
            ProportionOfDailyGrowthItems = new ProportionItems(proportionOfDailyGrowth);
        }

        /// <summary>
        /// 每日成长比例记录;
        /// </summary>
        public ProportionItems ProportionOfDailyGrowthItems { get; private set; }

        /// <summary>
        /// 每日成长比例;
        /// </summary>
        public float ProportionOfDailyGrowth
        {
            get { return ProportionOfDailyGrowthItems.Proportion; }
        }

    }

    /// <summary>
    /// 人力资源增长方式;
    /// </summary>
    public class ManpowerDailyGrowth : National, IRequestor
    {
        public ManpowerDailyGrowth(Country belongToCountry, Manpower manpower) : base(belongToCountry)
        {
            Manpower = manpower;
        }

        public Manpower Manpower { get; private set; }

        public ManpowerNationalFactor Factor
        {
            get { return BelongToCountry.ManpowerFactor; }
        }

        /// <summary>
        /// 更新人口资源数量;
        /// </summary>
        public void Update()
        {
            int increment = (int)(Manpower.Total * Factor.ProportionOfDailyGrowth);
            Manpower.Increase(this, increment);
        }

    }

    /// <summary>
    /// 人力资源;
    /// </summary>
    public class Manpower
    {
        public Manpower() : this(0)
        {
        }

        public Manpower(int number)
        {
            Total = number;
        }

        /// <summary>
        /// 人口总数;
        /// </summary>
        public int Total { get; private set; }

        /// <summary>
        /// 添加人口数量;
        /// </summary>
        public void Increase(IRequestor requestor, int number)
        {
            if (number < 0)
                throw new ArgumentOutOfRangeException();

            Total += number;
        }

        /// <summary>
        /// 移除人口数量;
        /// </summary>
        public bool Reduce(IRequestor requestor, int number)
        {
            if (number > Total)
                return false;

            Total -= number;
            return true;
        }


        /// <summary>
        /// 返回人力资源数目;
        /// </summary>
        public static implicit operator int(Manpower item)
        {
            return item.Total;
        }

    }

}
