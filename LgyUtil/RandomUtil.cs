using System;
using System.Collections.Generic;

namespace LgyUtil
{
    /// <summary>
    /// 随机数帮助类,直接调用Init方法实例化，之后链式调用即可
    /// </summary>
    public sealed class RandomUtil
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
        private Random randomInstance { get; set; }
        /// <summary>
        /// 生成的随机码
        /// </summary>
        private string RandomStr { get; set; }
        /// <summary>
        /// 随机码前缀
        /// </summary>
        private string Prefix { get; set; }
        /// <summary>
        /// 随机码后缀
        /// </summary>
        private string Suffix { get; set; }
        /// <summary>
        /// 当前是否是批量生成
        /// </summary>
        private bool IsBatchCreate { get; set; }
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
            randomInstance = new Random();
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
        /// 获取一个随机码
        /// </summary>
        /// <returns></returns>
        public string GetRandom()
        {
            IsBatchCreate = false;
            return GetRandomDoing();
        }
        /// <summary>
        /// 批量获取随机码
        /// </summary>
        /// <param name="numCount">生成个数</param>
        /// <returns></returns>
        public string[] GetRandoms(int numCount)
        {
            IsBatchCreate = true;
            string[] randoms = new string[numCount];
            for (int i = 0; i < numCount; i++)
            {
                randoms[i] = GetRandom();
            }
            return randoms;
        }
        /// <summary>
        /// 设置本次生成的内容不会出现重复的数字或字母，批量生成时此配置无效
        /// </summary>
        /// <returns></returns>
        public RandomUtil SetNotSame()
        {
            if (RandomFormat == Enum_RandomFormat.OnlyLetter && RandomLength > 52)
                throw new Exception("设置不重复后，且只生成字母的时候，随机数长度，不能超过52个");
            if (RandomFormat == Enum_RandomFormat.OnlyNumber && RandomLength > 10)
                throw new Exception("设置不重复后，且只生成数字的时候，随机数长度，不能超过10个");
            if (RandomFormat == Enum_RandomFormat.NumberAndLetter && RandomLength > 10)
                throw new Exception("设置不重复后，且生成数字和字母的时候，随机数长度，不能超过62个");
            this.NotSame = true;
            return this;
        }
        /// <summary>
        /// 设置随机码前缀
        /// </summary>
        /// <param name="prefix">前缀字符串</param>
        /// <returns></returns>
        public RandomUtil SetPrefix(string prefix)
        {
            this.Prefix = prefix;
            return this;
        }
        /// <summary>
        /// 设置随机码后缀
        /// </summary>
        /// <param name="suffix">后缀字符串</param>
        /// <returns></returns>
        public RandomUtil SetSuffix(string suffix)
        {
            this.Suffix = suffix;
            return this;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取随机码
        /// </summary>
        /// <returns></returns>
        private string GetRandomDoing()
        {
            RandomStr = string.Empty;
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
                        RandomStr += GetRandomOne(randomInstance.Next(0, 2).ToBool());
                    }
                    break;
            }

            return Prefix + RandomStr + Suffix;
        }
        /// <summary>
        /// 获取一个随机码
        /// </summary>
        /// <param name="isLetter">true:字母  false:数字</param>
        /// <returns></returns>
        private string GetRandomOne(bool isLetter)
        {
            string ret;
            if (isLetter)
                ret = Letters[randomInstance.Next(0, 2)][randomInstance.Next(0, 26)];
            else
                ret = randomInstance.Next(0, 10).ToString();
            //验证是否重复数字或字母
            //批量生成不验证
            if (!this.IsBatchCreate && this.RandomStr.IsNotNullOrEmpty() && this.NotSame && this.RandomStr.Contains(ret))
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
