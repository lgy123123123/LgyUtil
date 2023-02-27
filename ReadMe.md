# C#通用工具类说明
1. 工具的每个部分，都发布了独立的包，可以按需引入(LgyUtil、LgyUtil.Cache、LgyUtil.Compress、LgyUtil.ModelCheck、LgyUtil.Net、LgyUtil.SSH)
2. 下面每个说明最后括号内标记的内容，为工具所在的包，如 (LgyUtil),说明在LgyUtil包下
3. 每个方法若写为EncryptUtil.EncryptDES，则是通过帮助类，静态调用。若只有方法名ContainsAny，则为对象直接点出方法来使用(new List<string>().      ContainsAny("1"))。
## 一、ArrayUtil，数组扩展类(LgyUtil)
1. ForEach()，Exists()，Find()，FindAll()，FindIndex()，Sort()，ConvertAll()，普通数组[]增加扩展功能，都是List存在的功能

        用法：(以ForEach为例)
            string[] str={"a","b","c"};
            str.ForEach(s=>s+"1");
2. JoinToString()，将数组内容拼接成字符串，普通数组[]和List数组扩展功能

        用法：
        1)用英文逗号拼接数组所有内容
            string[] str={"a","b","c"};
            Console.WriteLine(str.JoinToString()) //JoinToString不填写参数，默认用英文逗号分隔
            输出结果 a,b,c
        2)自定义拼接内容
            public class Test { 
                public int a;
                public string b;
            }
            List<Test> tests=new List<Test>();
            //选择属性b，并且用+来拼接成字符串
            tests.JoinToString(t=>t.b,"+");
3. ContainsAny()，包含参数内多个内容的某一个即可返回ture，普通数组[]和List数组扩展功能

        用法：(解决了Contains方法只能输入一个参数进行判断)
            string[] str={"a","b","c","d"};
            str.ContainsAny("b","c");//数组某项 包含b 或者 包含c,就能返回true
4. Slice()，数组切片，普通数组[]和List数组扩展功能

        用法：(.net6 出现了新的切片语法糖[1..2]，可以替代这个方法)
            string[] str={"a","b","c","d"};
            str.Slice(1,2);//返回结果{"b","c"}，参数从0开始
5. HaveContent()，数组是否有内容，普通数组[]和List数组扩展功能

        用法：
            string[] str={"a","b","c","d"};  str.HaveContent()---->true
            string[] str={};                 str.HaveContent()---->false
            string[] str=null;               str.HaveContent()---->false
6. IsNullOrEmpty()，数组是否为空，普通数组[]和List数组扩展功能

        用法：
            string[] str={"a","b","c","d"};  str.IsNullOrEmpty()---->false
            string[] str={};                 str.IsNullOrEmpty()---->true
            string[] str=null;               str.IsNullOrEmpty()---->true
## 二、CronUtil，定时帮助类，对Quartz进行了封装(LgyUtil)
1. 开始必须初始化定时对象，执行:CronUtil.InitScheduler()
2. 添加定时任务
    - AddCronJob()，按照cron表达式定时
    - AddSecondJob()，按照秒定时
    - AddMinuteJob()，按照分钟定时
    - AddHourJob()，按照小时定时
    - AddJob()，自定义定时方式(TriggerBuilder对象)
3. 停止方法StopJob
## 三、DataTable扩展，效率不高，程序中避免使用DataTable(LgyUtil)
1. ToList\<T>()，将表转成数组
2. ToDictionary<string,T>()，将表转成字典
3. ToDataTable()，将DataRow\[]转成DateTable，一般在表的Select以后使用
4. ToModel\<T>()，将DataRow转成模型
## 四、DateTimeUtil，日期扩展方法(LgyUtil)
1. ToyyyyMMdd()，new DateTime(2000,5,2).ToyyyyMMdd() 输出：2000-05-02
2. ToyyyyMMddCH()，new DateTime(2000,5,2).ToyyyyMMddCH() 输出：2000年05月02日
3. ToyyyyMMddHHmmss()，new DateTime(2000,5,2,10,8,20).ToyyyyMMddHHmmss() 输出：2000-05-02 10:08:20
4. ToyyyyMMddHHmmssCH()，new DateTime(2000,5,2,10,8,20).ToyyyyMMddHHmmssCH() 输出：2000年05月02日 10:08:20
5. GetTimestamp()，获取当前unix时间戳(13位)
6. GetDateTimeByTimestamp()，js时间戳转本地日期
7. AddQuarter()，给传入日期 添加季度
8. GetQuarter()，获取传入日期的 季度
9. GetMonthBetween()，获取月份差，例如：5月和3月的月份差为2
10. GetYearsStart()，获取传入日期 年的第一天
11. GetYearsEnd()，获取传入日期 年的最后一天
12. GetQuarterStart()，获取传入日期 季度的第一天
13. GetQuarterEnd()，获取传入日期 季度最后一天
14. GetMonthStart()，获取传入日期 月的第一天
15. GetMonthEnd()，获取传入日期 月最后一天
16. GetDaysStart()，获取传入日期 0点0分0秒
17. GetDaysEnd()，获取传入日期  23点59分59秒
18. GetHourStart()，获取传入日期 0分0秒
19. GetHourEnd()，获取传入日期 59分59秒
20. GetMinuteStart()，获取传入日期 0秒
21. GetMinuteEnd()，获取传入日期 59秒
## 五、EncryptUtil，加密算法帮助类(LgyUtil)
1. AES加密解密(对称加密)：

        string key="1234567890123456";//秘钥必须16位
        string vector = "1234567890123456";//偏移向量也必须是16位，可以不填写，有默认向量
        //加密
        string miwen = EncryptUtil.AESEncrypt("加密内容",key,vector);
        //解密
        string mingwen = EncryptUtil.AESDecrypt(miwen,key,vector);
2. DES加密解密(对称加密)：与上面AES使用方法相同， 只是秘钥和向量都必须是8位长度

        EncryptUtil.EncryptDES()
        EncryptUtil.DecryptDES()
3. RSA加密（非对称加密）：

        1)先获取公钥和私钥
            (string publicKey,string privateKey) = EncryptUtil.GetRSAKey();
        2)公钥加密：
            EncryptUtil.RSAPublicEncrypt("数据","公钥");
        3)私钥解密：
            EncryptUtil.RSAPrivateDecrypt("加密数据","私钥");
4. GetMd5()，获取字符串的md5
## 六、EnumUtil，枚举帮助和扩展(LgyUtil)
1. GetEnumDescription()，获取枚举[Description]标签的描述
2. EnumUtil.GetEnumDescriptionList()，获取所有枚举的[Description]标签描述
3. EnumUtil.GetEnumDescriptionDic()，获取所有枚举的[Description]标签描述的字典

        返回:Dictionary<string:枚举名, string:描述>
4. EnumUtil.GetEnumNameValueDic，获取所有枚举描述的字典

        返回:Dictionary<string：枚举名, int：枚举数值>
## 七、FileUtil，文件帮助类(LgyUtil)
1. FileUtil.GetMD5()，计算并返回文件的md5，大文件慎用。
2. FileUtil.GetSHA1()，计算并返回文件的哈希值，大文件慎用。
3. FileUtil.WatchFileChanged()，监视文件夹变化
4. FileUtil.Copy()，拷贝文件或文件夹
5. FileUtil.ReadFileShare()，以非独占的方式，读取文件。解决windows下，用程序读取文件时，文件不能被文本编辑器打开。
6. FileUtil.GetAllFiles()，获取文件夹下所有文件（返回文件名数组）
7. FileUtil.GetAllFilesDeep()，深度查找文件或文件夹
## 八、ObjectConvertUtil，类型转换方法(LgyUtil)
1. ToInt()，ToDouble()，ToLong()，ToDecimal()，ToFloat()，任何对象都可以调用，原理是Convert.ToXXX，支持转为科学计数法
2. ToBool()，字符串"true"(不区分大小写),"1"，数值1，都会返回true，其余返回false
3. ToBytes()，返回对象的二进制数组，若对象为类，必须加上[Serializable]标签
4. ToObject()，根据二进制数组，返回对象，类必须打上[Serializable]标签
5. ToEnum\<Enum>()，将数值转换为枚举对象
## 九、ObjectUtil，对象扩展类(LgyUtil)
1. CloneNewtonJson()，用NewtonJson，序列化 再 序列化 克隆对象
2. CloneBinary()，用二进制方法克隆对象，类必须加上[Serializable]特性
3. In()，类似sql的in，判断内容是否在数组中出现
4. NotIn()，类似sql的not in
5. SerializeNewtonJson()，将对象用NewtonJson序列化为json字符串

        public class Test { 
                public DateTime a;
                public string b;
        }
        var t=new Test(){ a= DateTime.Now,b="x" };
        t.SerializeNewtonJson();
        //输出为{"a":"2023-02-17 16:31:19","b":"x"}

6.MappingTo()，映射对象，映射完的对象，是个全新的，不影响源对象

        public class Test { 
                public DateTime a;
                public string b;
        }
        public class Test1 { 
                public string b;
        }
        var t=new Test(){ a= DateTime.Now,b="x" };
        var t1=new Test();
        //将t的属性，映射到t1里
        t.MappingTo(t1);
        //将t的属性映射到Test1，并返回一个新的Test1实例
        var t2 = t.MappingTo<Test1>();
## 十、RandomUtil，随机数帮助类(LgyUtil)
可以生成7种随机类型码：只有数字、只有大写字母、只有小写字母、大小写字母、小写字母和数字、大写字母和数字、大小写字母和数字

使用方法：链式调用

    //输出7位只有数字的随机数
    RandomUtil.Init(7,Enum_RandomFormat.OnlyNumber).GetRandom();
链式调用的所有方法：

    GetRandom()，生成一个随机数
    GetRandoms()，生成多个随机数
    SetNotSame()，设置的一个随机数中，不出现重复的内容，如生成3个不重复的数字，123，不会出现111
    SetPrefix()，设置随机码前缀
    SetSuffix()，设置随机码后缀
## 十一、RegexUtil，正则表达式帮助类(LgyUtil)
只有简单的几个正则判断

    1. RegexUtil.IsPhoneNumerCN()，是否是中国大陆手机号
    2. RegexUtil.IsNumber()，是否是正数
    3. RegexUtil.IsChinese()，是否全是汉字
    4. RegexUtil.IsDomainName()，是否是域名
    5. RegexUtil.IsEmail()，是否是邮件
    6. RegexUtil.IsIP()，是否是IP地址
## 十二、StringUtil，字符串帮助和扩展类(LgyUtil)
1. IsNullOrEmpty()，字符串是否为空
2. IsNullOrEmptyTrim()，trim后，字符串是否为空
3. IsNotNullOrEmpty()，字符串是否不为空
4. Format()，就是string.Format
5. DeserializeNewtonJson()，使用NewtonJson，反序列化字符串为对象
6. ToDateTime()，转为时间，支持格式化时间

        //默认格式转换
        "2010-05-06".ToDateTime()
        //格式化字符串转换
        "20100506".ToDateTime("yyyyMMdd")

7. ToByteArr()，转为二进制数组
8. ByteToString()，二进制数组转为字符串
9. ToEnum\<Enum>()，字符串转枚举
10. RegexIsMatch()，匹配正则表达式
11. ReplaceByIndex()，根据下标索引，替换字符串

        //替换手机号 177****7777
        "17777777777".ReplaceByIndex(3,4,"*");

12. ReplaceRegex()，根据正则表达式替换
13. GetStringByRegex()，根据正则表达式，返回匹配的第一组内容
14. Split()，将字符串拆分成数组，返回string[]
