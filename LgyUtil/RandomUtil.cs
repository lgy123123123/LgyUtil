using System;
using System.Collections.Generic;
using System.Linq;
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
        private bool IsRandomTemplate { get; set; }

        /// <summary>
        /// 解析后的模板集合
        /// </summary>
        private string[] TemplateArr { get; set; }

        /// <summary>
        /// 生成的一个随机码中，没有重复的数字或字母
        /// </summary>
        private bool NotSame { get; set; }

        /// <summary>
        /// 生成的多个随机码，没有重复的
        /// </summary>
        private bool NotSameList { get; set; }

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
        /// 小写字母
        /// </summary>
        private readonly char[] lowerLetters =
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u',
            'v', 'w', 'x', 'y', 'z'
        };

        /// <summary>
        /// 大写字母
        /// </summary>
        private readonly char[] upperLetters =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
            'V', 'W', 'X', 'Y', 'Z'
        };

        /// <summary>
        /// 内部特殊字符
        /// </summary>
        /// <returns></returns>
        private char[] specialCharacters =
        {
            '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '+', '=', '[', ']', '{', '}', '|', '/', '?', ';'
        };

        /// <summary>
        /// 排除的LO字符数组
        /// </summary>
        private readonly char[] excludeSymbolLO = {'l', 'O', 'o', '1', '0'};

        /// <summary>
        /// 排除的字符
        /// </summary>
        private HashSet<char> ExcludeSymbol { get; set; } = new HashSet<char>();

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
            UpperLetter = 2,

            /// <summary>
            /// 特殊字符
            /// </summary>
            SpecialCharacter = 3
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
        /// <para>注意：设置批量不重复后，尽量不要生成超过200个随机码</para>
        /// </summary>
        /// <param name="numCount">生成个数</param>
        /// <returns></returns>
        public string[] GetRandoms(int numCount)
        {
            CheckNotSame();
            string[] randoms = new string[numCount];
            for (int i = 0; i < numCount; i++)
            {
                //尝试重复的次数，初始尝试1次，设置不重复，最多尝试100次
                var tryTimes = NotSameList ? 100 : 1;
                while (tryTimes > 0)
                {
                    var randomOne = GetRandomDetail();
                    if (!randoms.Contains(randomOne))
                    {
                        randoms[i] = randomOne;
                        break;
                    }

                    tryTimes--;
                }
            }

            return randoms;
        }

        /// <summary>
        /// 设置生成的一个随机码中，不会出现重复内容，模板无效
        /// </summary>
        /// <returns></returns>
        public RandomUtil SetNotSame()
        {
            this.NotSame = true;
            return this;
        }

        /// <summary>
        /// 设置批量生成随机码时，每组随机码不会出现重复内容
        /// </summary>
        /// <returns></returns>
        public RandomUtil SetNotSameList()
        {
            this.NotSameList = true;
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
            this.ExcludeSymbol.UnionWith(excludeSymbolLO);
            return this;
        }

        /// <summary>
        /// 设置排除的符号
        /// </summary>
        /// <param name="items">字母或数字，填写所有排除的内容</param>
        /// <returns></returns>
        public RandomUtil SetExcludeSymbol(string items)
        {
            if (items.IsNullOrEmptyTrim())
                throw new LgyUtilException("排除的符号不能为空");
            var excludeItems = items.Distinct().ToArray();
            foreach (var item in excludeItems)
            {
                this.ExcludeSymbol.Add(item);
            }

            return this;
        }

        /// <summary>
        /// 设置特殊字符
        /// </summary>
        /// <param name="items">特殊字符的内容</param>
        /// <returns></returns>
        public RandomUtil SetSpecialCharacters(string items)
        {
            if (items.IsNullOrEmptyTrim())
                throw new LgyUtilException("设置特殊字符不能为空");
            this.specialCharacters = items.Distinct().ToArray();
            return this;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 解析模板字符串
        /// </summary>
        private void AnalysisTemplate()
        {
            var matches = Regex.Matches(this.RandomFormatTemplate, @"\{[nxXs]+}", RegexOptions.Compiled);
            TemplateArr = new string[matches.Count];
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                var m = matches[i];
                //去除两侧大括号，只保留模板
                TemplateArr[i] = m.Value.ReplaceRegex(@"\{|}", "");
                this.RandomFormatTemplate =
                    this.RandomFormatTemplate.ReplaceByIndex(m.Index, m.Length, "{RandomTemplate_" + i + "}");
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
            if (IsRandomTemplate)
            {
                //根据解析的模板，生成的内容
                Dictionary<string, object> dicTemplateValues = new Dictionary<string, object>();
                //循环解析后的模板
                for (int i = 0; i < TemplateArr.Length; i++)
                {
                    StringBuilder randomOne = new StringBuilder();
                    foreach (char item in TemplateArr[i])
                    {
                        int tryTimes = 0;
                        switch (item)
                        {
                            case 'n':
                                randomOne.Append(GetRandomOne(Enum_RandomOneType.Number, ref tryTimes));
                                break;
                            case 'x':
                                randomOne.Append(GetRandomOne(Enum_RandomOneType.LowerLetter, ref tryTimes));
                                break;
                            case 'X':
                                randomOne.Append(GetRandomOne(Enum_RandomOneType.UpperLetter, ref tryTimes));
                                break;
                            case 's':
                                randomOne.Append(GetRandomOne(Enum_RandomOneType.SpecialCharacter, ref tryTimes));
                                break;
                        }
                    }

                    dicTemplateValues.Add("RandomTemplate_" + i, randomOne.ToString());
                }

                RandomStr = RandomFormatTemplate.FormatTemplate(dicTemplateValues, false);
            }
            //按枚举生成
            else
            {
                switch (RandomFormat)
                {
                    case Enum_RandomFormat.OnlyNumber:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            int tryTimes = 0;
                            RandomStr += GetRandomOne(Enum_RandomOneType.Number, ref tryTimes);
                        }

                        break;
                    case Enum_RandomFormat.OnlyLetter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            int tryTimes = 0;
                            bool isBig = randomInstance.Next(0, 2).ToBool();
                            RandomStr +=
                                GetRandomOne(isBig ? Enum_RandomOneType.UpperLetter : Enum_RandomOneType.LowerLetter,
                                    ref tryTimes);
                        }

                        break;
                    case Enum_RandomFormat.OnlyLowerLetter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            int tryTimes = 0;
                            RandomStr += GetRandomOne(Enum_RandomOneType.LowerLetter, ref tryTimes);
                        }

                        break;
                    case Enum_RandomFormat.OnlyUpperLetter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            int tryTimes = 0;
                            RandomStr += GetRandomOne(Enum_RandomOneType.UpperLetter, ref tryTimes);
                        }

                        break;
                    case Enum_RandomFormat.NumberAndLetter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            int tryTimes = 0;
                            Enum_RandomOneType oneType = randomInstance.Next(0, 3).ToEnum<Enum_RandomOneType>();
                            RandomStr += GetRandomOne(oneType, ref tryTimes);
                        }

                        break;
                    case Enum_RandomFormat.NumberAndLowerLetter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            int tryTimes = 0;
                            bool isNumber = randomInstance.Next(0, 2).ToBool();
                            RandomStr +=
                                GetRandomOne(isNumber ? Enum_RandomOneType.Number : Enum_RandomOneType.LowerLetter,
                                    ref tryTimes);
                        }

                        break;
                    case Enum_RandomFormat.NumberAndUpperLetter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            int tryTimes = 0;
                            bool isNumber = randomInstance.Next(0, 2).ToBool();
                            RandomStr +=
                                GetRandomOne(isNumber ? Enum_RandomOneType.Number : Enum_RandomOneType.UpperLetter,
                                    ref tryTimes);
                        }

                        break;
                    case Enum_RandomFormat.NumberAndCharacter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            int tryTimes = 0;
                            bool isNumber = randomInstance.Next(0, 2).ToBool();
                            RandomStr +=
                                GetRandomOne(isNumber ? Enum_RandomOneType.Number : Enum_RandomOneType.SpecialCharacter,
                                    ref tryTimes);
                        }

                        break;
                    case Enum_RandomFormat.LowerLetterAndCharacter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            int tryTimes = 0;
                            bool isLowerLetter = randomInstance.Next(0, 2).ToBool();
                            RandomStr +=
                                GetRandomOne(
                                    isLowerLetter
                                        ? Enum_RandomOneType.LowerLetter
                                        : Enum_RandomOneType.SpecialCharacter, ref tryTimes);
                        }

                        break;
                    case Enum_RandomFormat.UpperLetterAndCharacter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            int tryTimes = 0;
                            bool isUpperLetter = randomInstance.Next(0, 2).ToBool();
                            RandomStr +=
                                GetRandomOne(
                                    isUpperLetter
                                        ? Enum_RandomOneType.UpperLetter
                                        : Enum_RandomOneType.SpecialCharacter, ref tryTimes);
                        }

                        break;
                    case Enum_RandomFormat.NumberAndLowerLetterAndCharacter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            int tryTimes = 0;
                            Enum_RandomOneType oneType = randomInstance.Next(0, 4).ToEnum<Enum_RandomOneType>();
                            // 根据随机数生成对应的枚举值，排除大写字母
                            while (oneType == Enum_RandomOneType.UpperLetter)
                            {
                                oneType = randomInstance.Next(0, 4).ToEnum<Enum_RandomOneType>(); // 0到3
                            }

                            RandomStr += GetRandomOne(oneType, ref tryTimes);
                        }

                        break;
                    case Enum_RandomFormat.NumberAndUpperLetterAndCharacter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            int tryTimes = 0;
                            Enum_RandomOneType oneType = randomInstance.Next(0, 4).ToEnum<Enum_RandomOneType>();
                            // 根据随机数生成对应的枚举值，排除小写字母
                            while (oneType == Enum_RandomOneType.LowerLetter)
                            {
                                oneType = randomInstance.Next(0, 4).ToEnum<Enum_RandomOneType>(); // 0到3
                            }

                            RandomStr += GetRandomOne(oneType, ref tryTimes);
                        }

                        break;
                    case Enum_RandomFormat.LetterAndCharacter:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            int tryTimes = 0;
                            Enum_RandomOneType oneType = randomInstance.Next(1, 4).ToEnum<Enum_RandomOneType>();
                            RandomStr += GetRandomOne(oneType, ref tryTimes);
                        }

                        break;
                    case Enum_RandomFormat.All:
                        for (int i = 0; i < RandomLength; i++)
                        {
                            int tryTimes = 0;
                            Enum_RandomOneType oneType = randomInstance.Next(0, 4).ToEnum<Enum_RandomOneType>();
                            RandomStr += GetRandomOne(oneType, ref tryTimes);
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
        /// <param name="tryTimes">尝试次数</param>
        /// <returns></returns>
        private char GetRandomOne(Enum_RandomOneType oneType, ref int tryTimes)
        {
            char ret = ' ';
            switch (oneType)
            {
                case Enum_RandomOneType.Number:
                    ret = (randomInstance.Next(0, 10).ToString())[0];
                    break;
                case Enum_RandomOneType.LowerLetter:
                    ret = lowerLetters[randomInstance.Next(0, 26)];
                    break;
                case Enum_RandomOneType.UpperLetter:
                    ret = upperLetters[randomInstance.Next(0, 26)];
                    break;
                case Enum_RandomOneType.SpecialCharacter:
                    ret = specialCharacters[randomInstance.Next(0, specialCharacters.Length)];
                    break;
            }

            tryTimes++;
            if (tryTimes == 70) //最多尝试70次，26大写+26小写+10个数字+5个特殊字符=67
                return ret;
            //验证是否重复数字或字母
            if (this.RandomStr.IsNotNullOrEmpty() && this.NotSame && this.RandomStr.Contains(ret))
                ret = GetRandomOne(oneType, ref tryTimes);
            //是否排除 字母l 字母O 字母o 数字1 数字0
            else if (this.ExcludeSymbol.Contains(ret))
                ret = GetRandomOne(oneType, ref tryTimes);
            return ret;
        }

        /// <summary>
        /// 验证不重复生成时，设置的随机码长度
        /// </summary>
        /// <exception cref="LgyUtilException"></exception>
        private void CheckNotSame()
        {
            //按模板生成时，不重复配置无效
            if (IsRandomTemplate)
                NotSame = false;

            //排除多少个数字
            int excludeNumbers = 0;
            //排除多少个大写字母
            int excludeBigLettersNum = 0;
            //排除多少个小写字母
            int excludeSmallLettersNum = 0;
            //是否设置排除字符
            bool isExcludeSymbol = ExcludeSymbol.HaveContent();
            if (isExcludeSymbol)
            {
                excludeNumbers = ExcludeSymbol.Count(s => s >= 48 && s <= 57);
                excludeBigLettersNum = ExcludeSymbol.Count(s => s >= 65 && s <= 90);
                excludeSmallLettersNum = ExcludeSymbol.Count(s => s >= 97 && s <= 122);
            }

            if (NotSame)
            {
                string excludeContent = isExcludeSymbol ? "和排除字符" : "";
                if (RandomFormat == Enum_RandomFormat.OnlyLetter &&
                    RandomLength > 52 - excludeBigLettersNum - excludeSmallLettersNum)
                    throw new LgyUtilException(
                        $"设置不重复{excludeContent}，且只生成字母的时候，随机数长度，不能超过{52 - excludeBigLettersNum - excludeSmallLettersNum}个");
                if (RandomFormat == Enum_RandomFormat.OnlyNumber && RandomLength > 10 - excludeNumbers)
                    throw new LgyUtilException($"设置不重复{excludeContent}，且只生成数字的时候，随机数长度，不能超过{10 - excludeNumbers}个");
                if (RandomFormat == Enum_RandomFormat.OnlyLowerLetter && RandomLength > 26 - excludeSmallLettersNum)
                    throw new LgyUtilException(
                        $"设置不重复{excludeContent}，且只生成小写字母的时候，随机数长度，不能超过{26 - excludeSmallLettersNum}个");
                if (RandomFormat == Enum_RandomFormat.OnlyUpperLetter && RandomLength > 26 - excludeBigLettersNum)
                    throw new LgyUtilException(
                        $"设置不重复{excludeContent}，且只生成大写字母的时候，随机数长度，不能超过{26 - excludeBigLettersNum}个");
                if (RandomFormat == Enum_RandomFormat.NumberAndLetter && RandomLength > 62)
                    throw new LgyUtilException(
                        $"设置不重复{excludeContent}，且生成数字和字母的时候，随机数长度，不能超过{62 - excludeBigLettersNum - excludeSmallLettersNum - excludeNumbers}个");
                if (RandomFormat == Enum_RandomFormat.NumberAndLowerLetter && RandomLength > 36)
                    throw new LgyUtilException(
                        $"设置不重复{excludeContent}，且生成数字和小写字母的时候，随机数长度，不能超过{36 - excludeSmallLettersNum - excludeNumbers}个");
                if (RandomFormat == Enum_RandomFormat.NumberAndUpperLetter && RandomLength > 36)
                    throw new LgyUtilException(
                        $"设置不重复{excludeContent}，且生成数字和大写字母的时候，随机数长度，不能超过{36 - excludeBigLettersNum - excludeNumbers}个");
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
        NumberAndLetter,

        /// <summary>
        ///数字+字符
        /// </summary>
        NumberAndCharacter,

        /// <summary>
        /// 数字+小写字母+字符
        /// </summary>
        NumberAndLowerLetterAndCharacter,

        /// <summary>
        /// 数字+大写字母+字符
        /// </summary>
        NumberAndUpperLetterAndCharacter,

        /// <summary>
        /// 字母+字符
        /// </summary>
        LetterAndCharacter,

        /// <summary>
        /// 小写字母+字符
        /// </summary>
        LowerLetterAndCharacter,

        /// <summary>
        /// 大写字母+字符
        /// </summary>
        UpperLetterAndCharacter,

        /// <summary>
        /// 生成大小写字母+字符+数字
        /// </summary>
        All
    }
}