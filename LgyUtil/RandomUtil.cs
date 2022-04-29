using System;
using System.Collections.Generic;

namespace LgyUtil
{
    /// <summary>
    /// 随机数帮助类,直接调用Init方法实例化，之后链式调用即可
    /// </summary>
    public class RandomUtil
    {
        #region 私有属性
        /// <summary>
        /// 随机数长度
        /// </summary>
        private int RandomLength { get; set; }
        /// <summary>
        /// 生成随机码的格式
        /// </summary>
        private Enum_RandomFormat RandomFormat { get; set; }

        /// <summary>
        /// 一次生成的随机码中，没有重复的数字或字母
        /// </summary>
        private bool NotSame { get; set; } = false;
        /// <summary>
        /// 系统随机实例
        /// </summary>
        private Random r { get; set; }
        /// <summary>
        /// 生成的随机码
        /// </summary>
        private string RandomStr { get; set; }

        /// <summary>
        /// 字母数组，0：小写字母   1：大写字母
        /// </summary>

        private readonly List<string[]> Letters = new List<string[]>
        {
            new []{
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u",
                "v", "w", "x", "y", "z"
            },
            new []{
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U",
                "V", "W", "X", "Y", "Z"
            }
        };
        #endregion

        #region 公共方法
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="length">随机码长度</param>
        /// <param name="format">生成随机码的格式</param>
        public RandomUtil(int length, Enum_RandomFormat format)
        {
            this.RandomLength = length;
            this.RandomFormat = format;
            r = new Random();
        }
        /// <summary>
        /// 创建RandomUtil实例
        /// </summary>
        /// <param name="length">随机长度</param>
        /// <param name="format">生成的格式</param>
        /// <returns>new RandomUtil</returns>
        public static RandomUtil Init(int length, Enum_RandomFormat format)
        {
            return new RandomUtil(length, format);
        }
        /// <summary>
        /// 获取随机码
        /// </summary>
        /// <returns></returns>
        public string GetRandom()
        {
            switch (RandomFormat)
            {
                case Enum_RandomFormat.OnlyNumber:
                    for (int i = 0; i < RandomLength; i++)
                    {
                        RandomStr += GetRandomOne(false);
                    }
                    break;
                case Enum_RandomFormat.OnlyLetter:
                    for (int i = 0; i < RandomLength; i++)
                    {
                        RandomStr += GetRandomOne(true);
                    }
                    break;
                case Enum_RandomFormat.NumberAndLetter:
                    for (int i = 0; i < RandomLength; i++)
                    {
                        RandomStr += GetRandomOne(r.Next(0, 2).ToBool());
                    }
                    break;
            }

            return RandomStr;
        }
        /// <summary>
        /// 设置本次生成的内容不会出现重复的数字或字母
        /// </summary>
        /// <returns></returns>
        public RandomUtil SetNotSame()
        {
            this.NotSame = true;
            return this;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取一个随机码
        /// </summary>
        /// <param name="isLetter">true:字母  false:数字</param>
        /// <returns></returns>
        private string GetRandomOne(bool isLetter)
        {
            string ret;
            if (isLetter)
                ret = Letters[r.Next(0, 2)][r.Next(0, 26)];
            else
                ret = r.Next(0, 10).ToString();
            if (this.NotSame && this.RandomStr.Contains(ret))
                ret = GetRandomOne(isLetter);
            return ret;
        }
        #endregion
    }
    /// <summary>
    /// 随机数类型
    /// </summary>
    public enum Enum_RandomFormat
    {
        /// <summary>
        /// 只有数字
        /// </summary>
        OnlyNumber,
        /// <summary>
        /// 只有字母(大小写)
        /// </summary>
        OnlyLetter,
        /// <summary>
        /// 字母(大小写)和数字
        /// </summary>
        NumberAndLetter
    }
}
