using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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
        /// 最终生成随机码的模板，不能与notsame混用。
        /// n:数字 x:小写字母 X:大写字母。
        /// 使用方法：用大括号包起来，例如{nxx}-{nxx}生成结果1df-6er
        /// 注：{nax}这种不会生成，请修改为{n}a{x}
        /// </summary>
        private string RandomFormatTemplate { get; set; }
        /// <summary>
        /// 是否是按模板生成
        /// </summary>
        private bool IsRandomTemplate { get;set; }
        /// <summary>
        /// 解析后的模板集合
        /// </summary>
        private string[] TemplateArr { get; set; }
        /// <summary>
        /// 生成的一个随机码中，没有重复的数字或字母
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
        /// 生成时，是否移除字母L(大小写)、字母O(大小写)、数字1、数字0
        /// </summary>
        private bool IsRemoveLO { get; set; }
        /// <summary>
        /// 小写字母
        /// </summary>
        private readonly string[] lowerLetters =
            {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u",
             "v", "w", "x", "y", "z"};
        /// <summary>
        /// 大写字母
        /// </summary>
        private readonly string[] upperLetters =
            {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U",
             "V", "W", "X", "Y", "Z"};
        /// <summary>
        /// 生成那种随机数
        /// </summary>
        enum Enum_RandomOneType
        {
            /// <summary>
            /// 数字
            /// </summary>
            Number = 0,
            /// <summary>
            /// 小写字母
            /// </summary>
            LowerLetter = 1,
            /// <summary>
            /// 大写字母
            /// </summary>
            UpperLetter = 2
        }
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
        /// 构造函数
        /// </summary>
        /// <param name="template">生成模板</param>
        public RandomUtil(string template)
        {
            this.RandomFormatTemplate = template;
            this.IsRandomTemplate = true;
            AnalysisTemplate();
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
        /// 创建RandomUtil模板实例，不能使用SetNotSame生成不重复内容。
        /// 使用方法：用大括号包起来，例如{nxx}-{nxx}生成结果1df-6er。
        /// n:数字 x:小写字母 X:大写字母。
        /// </summary>
        /// <param name="template">模板：n:数字 x:小写字母 X:大写字母。</param>
        /// <returns></returns>
        public static RandomUtil Init(string template)
        {
            return new RandomUtil(template);
        }
        /// <summary>
        /// 获取一个随机码
        /// </summary>
        /// <returns></returns>
        public string GetRandom()
        {
            CheckNotSame();
            return GetRandomDetail();
        }
        /// <summary>
        /// 批量获取随机码
        /// </summary>
        /// <param name="numCount">生成个数</param>
        /// <returns></returns>
        public string[] GetRandoms(int numCount)
        {
            this.NotSame = false;
            CheckNotSame();
            string[] randoms = new string[numCount];
            for (int i = 0; i < numCount; i++)
            {
                randoms[i] = GetRandom();
            }
            return randoms;
        }
        /// <summary>
        /// 设置本次生成的内容不会出现重复的数字或字母，注：批量生成、按模板生成，此配置无效
        /// </summary>
        /// <returns></returns>
        public RandomUtil SetNotSame()
        {
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
        /// <summary>
        /// 生成结果不包含 字母L(小写)、字母O(大小写)、数字1、数字0，一般在生成数字和字母组合时才使用
        /// </summary>
        /// <returns></returns>
        public RandomUtil NotContainsLO()
        {
            IsRemoveLO = true;
            return this;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 解析模板字符串
        /// </summary>
        private void AnalysisTemplate()
        {
            var matches= Regex.Matches(this.RandomFormatTemplate, @"\{[nxX]+}");
            TemplateArr = new string[matches.Count];
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                var m = matches[i];
                //去除两侧大括号，只保留模板
                TemplateArr[i]=(m.Value.ReplaceRegex(@"\{|}", ""));
                this.RandomFormatTemplate = this.RandomFormatTemplate.ReplaceByIndex(m.Index, m.Length, "{RandomTemplate_" + i + "}");
            }
        }
        /// <summary>
        /// 获取验证码的具体方法
        /// </summary>
        /// <returns></returns>
        private string GetRandomDetail()
        {
            RandomStr = string.Empty;
            //按模板生成
            if(IsRandomTemplate)
            {
                //根据解析的模板，生成的内容
                Dictionary<string,object> dicTemplateValues= new Dictionary<string, object>();
                //循环解析后的模板
                for (int i = 0; i < TemplateArr.Length; i++)
                {
                    StringBuilder randomOne=new StringBuilder();
                    foreach (char item in TemplateArr[i])
                    {
                        switch (item)
                        {
                            case 'n':
                                randomOne.Append(GetRandomOne(Enum_RandomOneType.Number));
                                break;
                            case 'x':
                                randomOne.Append(GetRandomOne(Enum_RandomOneType.LowerLetter));
                                break;
                            case 'X':
                                randomOne.Append(GetRandomOne(Enum_RandomOneType.UpperLetter));
                                break;
                        }
                    }
                    dicTemplateValues.Add("RandomTemplate_" + i, randomOne.ToString());
                }
                RandomStr = RandomFormatTemplate.FormatTemplate(dicTemplateValues,false);
            }
            //按枚举生成
            else
            {
                switch (RandomFormat)
                {
                    case Enum_RandomFormat.OnlyNumber:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            RandomStr += GetRandomOne(Enum_RandomOneType.Number);
                        }
                        break;
                    case Enum_RandomFormat.OnlyLetter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            bool isBig = randomInstance.Next(0, 2).ToBool();
                            RandomStr += GetRandomOne(isBig ? Enum_RandomOneType.UpperLetter : Enum_RandomOneType.LowerLetter);
                        }
                        break;
                    case Enum_RandomFormat.OnlyLowerLetter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            RandomStr += GetRandomOne(Enum_RandomOneType.LowerLetter);
                        }
                        break;
                    case Enum_RandomFormat.OnlyUpperLetter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            RandomStr += GetRandomOne(Enum_RandomOneType.UpperLetter);
                        }
                        break;
                    case Enum_RandomFormat.NumberAndLetter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            Enum_RandomOneType oneType = randomInstance.Next(0, 3).ToEnum<Enum_RandomOneType>();
                            RandomStr += GetRandomOne(oneType);
                        }
                        break;
                    case Enum_RandomFormat.NumberAndLowerLetter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            bool isNumber = randomInstance.Next(0, 2).ToBool();
                            RandomStr += GetRandomOne(isNumber ? Enum_RandomOneType.Number : Enum_RandomOneType.LowerLetter);
                        }
                        break;
                    case Enum_RandomFormat.NumberAndUpperLetter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            bool isNumber = randomInstance.Next(0, 2).ToBool();
                            RandomStr += GetRandomOne(isNumber ? Enum_RandomOneType.Number : Enum_RandomOneType.UpperLetter);
                        }
                        break;
                }
            }
            return Prefix + RandomStr + Suffix;
        }
        /// <summary>
        /// 获取一个随机码
        /// </summary>
        /// <param name="oneType">生成类型</param>
        /// <returns></returns>
        private string GetRandomOne(Enum_RandomOneType oneType)
        {
            string ret = "";
            switch (oneType)
            {
                case Enum_RandomOneType.Number:
                    ret = randomInstance.Next(0, 10).ToString();
                    break;
                case Enum_RandomOneType.LowerLetter:
                    ret = lowerLetters[randomInstance.Next(0, 26)];
                    break;
                case Enum_RandomOneType.UpperLetter:
                    ret = upperLetters[randomInstance.Next(0, 26)];
                    break;
            }
            //验证是否重复数字或字母
            if (this.RandomStr.IsNotNullOrEmpty() && this.NotSame && this.RandomStr.Contains(ret))
                ret = GetRandomOne(oneType);
            //是否排除 字母l 字母O 字母o 数字1 数字0
            else if (this.IsRemoveLO && (ret.In("l", "O", "o", "1", "0")))
                ret = GetRandomOne(oneType);
            return ret;
        }
        /// <summary>
        /// 验证不重复生成时，设置的随机码长度
        /// </summary>
        /// <exception cref="LgyUtilException"></exception>
        private void CheckNotSame()
        {
            //按模板生成时，不重复配置无效
            if (this.IsRandomTemplate)
                this.NotSame = false;

            if (!this.NotSame)
                return;

            if (RandomFormat == Enum_RandomFormat.OnlyLetter && RandomLength > 52)
                throw new LgyUtilException("设置不重复，且只生成字母的时候，随机数长度，不能超过52个");
            if (RandomFormat == Enum_RandomFormat.OnlyNumber && RandomLength > 10)
                throw new LgyUtilException("设置不重复，且只生成数字的时候，随机数长度，不能超过10个");
            if (RandomFormat == Enum_RandomFormat.OnlyLowerLetter && RandomLength > 26)
                throw new LgyUtilException("设置不重复，且只生成小写字母的时候，随机数长度，不能超过26个");
            if (RandomFormat == Enum_RandomFormat.OnlyUpperLetter && RandomLength > 26)
                throw new LgyUtilException("设置不重复，且只生成大写字母的时候，随机数长度，不能超过26个");
            if (RandomFormat == Enum_RandomFormat.NumberAndLetter && RandomLength > 62)
                throw new LgyUtilException("设置不重复，且生成数字和字母的时候，随机数长度，不能超过62个");
            if (RandomFormat == Enum_RandomFormat.NumberAndLowerLetter && RandomLength > 36)
                throw new LgyUtilException("设置不重复，且生成数字和小写字母的时候，随机数长度，不能超过36个");
            if (RandomFormat == Enum_RandomFormat.NumberAndUpperLetter && RandomLength > 36)
                throw new LgyUtilException("设置不重复，且生成数字和大写字母的时候，随机数长度，不能超过36个");

            //对排除字母的验证，减少验证长度
            if (IsRemoveLO)
            {
                if (RandomFormat == Enum_RandomFormat.OnlyLetter && RandomLength > 49)
                    throw new LgyUtilException("设置不重复和排除字母，且只生成字母的时候，随机数长度，不能超过49个");
                if (RandomFormat == Enum_RandomFormat.OnlyNumber && RandomLength > 8)
                    throw new LgyUtilException("设置不重复和排除数字，且只生成数字的时候，随机数长度，不能超过8个");
                if (RandomFormat == Enum_RandomFormat.OnlyLowerLetter && RandomLength > 24)
                    throw new LgyUtilException("设置不重复和排除字母，且只生成小写字母的时候，随机数长度，不能超过24个");
                if (RandomFormat == Enum_RandomFormat.OnlyUpperLetter && RandomLength > 25)
                    throw new LgyUtilException("设置不重复和排除字母，且只生成大写字母的时候，随机数长度，不能超过25个");
                if (RandomFormat == Enum_RandomFormat.NumberAndLetter && RandomLength > 57)
                    throw new LgyUtilException("设置不重复和排除字母与数字，且生成数字和字母的时候，随机数长度，不能超过57个");
                if (RandomFormat == Enum_RandomFormat.NumberAndLowerLetter && RandomLength > 32)
                    throw new LgyUtilException("设置不重复和排除字母与数字，且生成数字和小写字母的时候，随机数长度，不能超过32个");
                if (RandomFormat == Enum_RandomFormat.NumberAndUpperLetter && RandomLength > 33)
                    throw new LgyUtilException("设置不重复和排除字母与数字，且生成数字和大写字母的时候，随机数长度，不能超过33个");
            }
        }
        #endregion
    }
    /// <summary>
    /// 随机数类型
    /// </summary>
    public enum Enum_RandomFormat
    {
        /// <summary>
        /// 生成结果 只有数字
        /// </summary>
        OnlyNumber,
        /// <summary>
        /// 生成结果包含 大写字母 和 小写字母
        /// </summary>
        OnlyLetter,
        /// <summary>
        /// 生成结果 只有小写字母
        /// </summary>
        OnlyLowerLetter,
        /// <summary>
        /// 生成结果 只有大写字母
        /// </summary>
        OnlyUpperLetter,
        /// <summary>
        /// 生成结果包含 小写字母和数字
        /// </summary>
        NumberAndLowerLetter,
        /// <summary>
        /// 生成结果包含 大写字母和数字
        /// </summary>
        NumberAndUpperLetter,
        /// <summary>
        /// 生成结果包含 大写字母、小写字母、数字
        /// </summary>
        NumberAndLetter
    }
}
