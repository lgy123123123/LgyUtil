# C#通用工具类说明
1. 工具的每个部分，都发布了独立的包，可以按需引入(LgyUtil、LgyUtil.Cache、LgyUtil.Compress、LgyUtil.ModelCheck、LgyUtil.Net、LgyUtil.SSH)
2. 下面每个说明最后括号内标记的内容，为工具所在的包，如 (LgyUtil),说明在LgyUtil包下
3. 每个方法若写为EncryptUtil.EncryptDES，则是通过帮助类，静态调用。若只有方法名ContainsAny，则为对象直接点出方法来使用(new List<string>().      ContainsAny("1"))。
1. 如遇到bug，请将问题发送至邮箱565493752@qq.com，我会第一时间回复并解决
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
## 二、TimerUtil，简易定时帮助类，依靠Timer对象(LgyUtil)
1. 添加定时任务

    - 按照cron表达式定时(支持6-7位cron表达式)
    
    ~~~ c#
      AddCronJob(string jobName,string cron,Action<JobExecInfo> doing, JobOption options=null)
    ~~~
    - 按照秒定时
    
    ~~~ c#
      AddSecondJob(string jobName,double second,Action<JobExecInfo> doing,JobOption options = null)
    ~~~
    - 按照分钟定时
    
    ~~~ c#
      AddMinuteJob(string jobName, double minute, Action<JobExecInfo> doing, JobOption options = null)
    ~~~
    - 按照小时定时
    
    ~~~ c#
      AddHourJob(string jobName, double hour, Action<JobExecInfo> doing, JobOption options = null)
    ~~~
    - 按照天定时
    
    ~~~ c#
      AddDayJob(string jobName, double day, Action<JobExecInfo> doing, JobOption options = null)
    ~~~
    - 自定义时间间隔定时
    
    ~~~ c#
      AddCustomJob(string jobName, TimeSpan customTimeSpan, Action<JobExecInfo> doing, JobOption options = null)
    ~~~
2. 定时选项说明(JobOption)

    - StartTime：任务开始执行时间。若设置了RunNow，会先执行一次，再按照StartTime执行
    - EndTime：任务结束执行时间(结束后，自动删除任务)
    - RunNow：立即执行一次，默认false。若设置了StartTime，立即执行后，等到达StartTime后执行第二次
    - ContinueNotFinish:上次执行未结束，到下次触发时间，是否继续触发本次任务，默认false
        - true:不管上次执行是否结束，本次依然执行
        - false:上次未结束，跳过本次任务，等待下次触发(此任务只会有一个线程在执行)
    - MaxExecTimes：最大执行次数，默认无限次数执行，到达最大次数时，停止并删除任务
    - ErrorContinue：发生错误时，是否继续执行，默认false，发生错误，停止job
    - ErrorDoing：发生错误时，执行的方法
    - AfterStopDoing：任务停止之后执行的方法rStopDoing:任务停止之后执行的方法
2. 获取未来5次触发时间，GetNext5TriggerTimes(string jobName)
3. 停止并删除任务，StopJob(string jobName)
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
 1. ToStringExt(),格式化日期扩展，Q代表季度，只解析第一个Q
        
        new DateTime(2010,5,3).ToStringExt("yyyy/Q")
## 五、EncryptUtil，加密算法帮助类(LgyUtil)
1. AES加密解密(对称加密)：

        string key="1234567890123456";//秘钥必须16位
        string vector = "1234567890123456";//偏移向量也必须是16位，有默认值，可以不填写
        var model=CipherMode.CBC;//加密模式，默认是CBC，可以不填写
        padding=PaddingMode.PKCS7;//偏移量,默认是PKCS7，可以不填写
        //加密
        string miwen = EncryptUtil.AESEncrypt("加密内容",key,model,padding);
        //解密
        string mingwen = EncryptUtil.AESDecrypt(miwen,key,vector,model,padding);
2. DES加密解密(对称加密)：与上面AES使用方法相同， 只是秘钥和向量都必须是8位长度

        EncryptUtil.EncryptDES()
        EncryptUtil.DecryptDES()
3. RSA加密（非对称加密）两种格式的秘钥xml、pem：

        1)先获取公钥和私钥
            //xml格式秘钥
            (string publicKey,string privateKey) = EncryptUtil.GetRSAKey_Xml();
            //pem格式秘钥，有两种格式，在获取秘钥时，填写第一个参数
            //1.带标准头尾的  -----BEGIN PUBLIC这种
            //2.无换行的纯Base64字符串的
            (string publicKey,string privateKey) = EncryptUtil.GetRSAKey_Pem();
        2)公钥加密：
            EncryptUtil.RSAPublicEncrypt_Xml("数据","xml公钥");
            EncryptUtil.RSAPublicEncrypt_Pem("数据","pem公钥");
        3)私钥解密：
            EncryptUtil.RSAPrivateDecrypt_Xml("xml加密数据","xml私钥");
            EncryptUtil.RSAPrivateDecrypt_Pem("pem加密数据","pem私钥");

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
1. FileUtil.SortByWindowsFileName(),按照windows文件名排序规则，排序字符串
1. FileUtil.SortByWindowsFileNameDesc()，按照windows文件名排序规则，倒序排序字符串
## 八、ObjectConvertUtil，类型转换方法(LgyUtil)
1. ToInt()，ToDouble()，ToLong()，ToDecimal()，ToFloat()，任何对象都可以调用，原理是Convert.ToXXX，支持转为科学计数法
2. ToBool()，字符串"true"(不区分大小写),"1"，数值1，都会返回true，其余返回false
3. ToBytes()，返回对象的二进制数组，若对象为类，必须加上[Serializable]标签
4. ToObject()，根据二进制数组，返回对象，类必须打上[Serializable]标签
5. ToEnum\<Enum>()，将数值转换为枚举对象
## 九、ObjectUtil，对象扩展类(LgyUtil)
1. CloneNewtonJson()，用NewtonJson，序列化 再 序列化 克隆对象
2. CloneBinary()，用二进制方法克隆对象，类必须加上[Serializable]特性
3. CloneMapster(),使用Mapster克隆，只能克隆普通类，速度快
4. In()，类似sql的in，判断内容是否在数组中出现
5. NotIn()，类似sql的not in
6. SerializeNewtonJson()，将对象用NewtonJson序列化为json字符串

        public class Test { 
            public DateTime a;
            public string b;
        }
        var t=new Test(){ a= DateTime.Now,b="x" };
        t.SerializeNewtonJson();
        //输出为{"a":"2023-02-17 16:31:19","b":"x"}

7.MappingTo()，映射对象，比AutoFac简单，上手快。

        //2个基础类
        public class A { 
            public DateTime x;
            public string y;
        }
        public class B { 
            public string y;
            public int z;
        }

        var a=new A(){ x= DateTime.Now,y="x" };
        var b=new B(){ y="abc",z=1};

        //1、将a的属性，映射到b里
        a.MappingTo(b);//b的内容为 y="x",z=1，覆盖同名同类型内容，其它不动
        //2、将a的属性映射到b，并返回一个新的B实例，覆盖同名同类型内容，其它不动
        B b = a.MappingTo<b>();
        //3、自定义配置映射，具体配置参数，请参考https://github.com/MapsterMapper/Mapster
        B b = a.MappingTo(setter =>
        {
            //null值不映射
            setter.IgnoreNullValues(true);
        });
## 十、RandomUtil，随机数帮助类(LgyUtil)
可以生成7种随机类型码：只有数字、只有大写字母、只有小写字母、大小写字母、小写字母和数字、大写字母和数字、大小写字母和数字。

也可以根据模板生成。随机数内容，用一对大括号包括。 n:数字 x:小写字母 X:大写字母。大括号里的内容，只能包含nXx三种字母，若出现{axnX}，此为错误模板，不会解析。

使用方法：链式调用

    //输出7位只有数字的随机数
    RandomUtil.Init(7,Enum_RandomFormat.OnlyNumber).GetRandom();
    //根据模板，生成随机数。注意：{anXx}为错误模板，不解析
    RandomUtil.Init("1234-{nnXXxx}-{anXx}-567-{xxnnXX}").GetRandom();
    //输出内容1234-62DGxg-{anXx}-567-td67IN
链式调用的所有方法：

    GetRandom()，生成一个随机数
    GetRandoms()，生成多个随机数(结果可能会出现重复的情况，设置SetNotSame后，只是生成的一个随机数里的内容不会重复)
    SetNotSame()，生成的一个随机数中，不出现重复的内容，如生成3个不重复的数字，123，不会出现111。注：按模板生成时，此配置无效
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
    7. RegexUtil.IsPwd_NumberLetter()，密码验证：必须包含数字、字母(不区分大小写)
    8. RegexUtil.IsPwd_NumberLetterSymbols()，密码验证：必须包含数字、字母(不区分大小写)、特殊符号(! @ # $ % ^ & * ( ) _ + - = . , { } [ ] ?)
    9. RegexUtil.IsPwd_NumberLetterBigSmall()，密码验证：必须包含数字、大写字母、小写字母
    10. RegexUtil.IsPwd_NumberLetterBigSmallSymbols()，密码验证：必须包含数字、大写字母、小写字母、特殊符号(! @ # $ % ^ & * ( ) _ + - = . , { } [ ] ?)
## 十二、StringUtil，字符串帮助和扩展类(LgyUtil)
1. IsNullOrEmpty()，字符串是否为空
2. IsNullOrEmptyTrim()，trim后，字符串是否为空
3. IsNotNullOrEmpty()，字符串是否不为空
4. Format()，就是string.Format
5. DeserializeNewtonJson()，使用NewtonJson，反序列化字符串为对象
6. ToDateTime()，转为时间，支持格式化时间，支持季度，Q代表季度，只解析第一个Q

        //默认格式转换
        "2010-05-06".ToDateTime()
        //格式化字符串转换
        "20100506".ToDateTime("yyyyMMdd")
        //季度转日期
        "2010/2".ToDateTime("yyyy/Q")

8. ToByteArr()，转为二进制数组
9. ByteToString()，二进制数组转为字符串
10. ToEnum\<Enum>()，字符串转枚举
11. RegexIsMatch()，匹配正则表达式
12. ReplaceByIndex()，根据下标索引，替换字符串

        //替换手机号 177****7777
        "17777777777".ReplaceByIndex(3,4,"*");

13. ReplaceRegex()，根据正则表达式替换
14. GetStringByRegex()，根据正则表达式，返回匹配的第一组内容
15. Split()，将字符串拆分成数组，返回string[]
16. FormatTemplate(),根据模板格式化字符串，可以使用Dictionary<string, object>、匿名类、普通类
    - Dictionary<string, object>
          
          string str="{a},{b}";
          str.FormatTemplate(new Dictionary<string, object>{{"a",1},{"b","2"}});
          //输出1,2

    - 匿名类

          string str="{a},{b}";
          str.FormatTemplate(new {a=1,b="2"});
          //输出1,2

    - 普通类

          string str="{a},{b}";
          class Temp
          {
              public string a { get; set; }
              public int b { get; set; }
          }
          str.FormatTemplate(new Temp{a="1",b=2});
          //输出1,2
17. Trim(),可以接填填写字符串，若替换一次后，开头或结尾还有要trim的内容，不再进行替换
 1. TrimStart(),可直接填写字符串，若替换一次后，开头还有要trim的内容，不再进行替换
 1. TrimEnd(),可直接填写字符串，若替换一次后，结尾还有要trim的内容，不再进行替换
 1. EndsWith(),可以匹配多个字符串
 1. StartsWith(),可以匹配多个字符串
 1. ContainsAny(),字符串包含任意一个匹配项，就返回true
## 十三、TaskUtil，多线程帮助类(LgyUtil)
1. 执行多线程时，控制并行线程个数

        //最多10个线程并行执行
        using(var taskMaxUtil = TaskUtil.GetTaskMaxUtil(10))
        {
            //执行100次，最大线程数为10
            for (int i = 0; i < 100; i++)
            {
                var tempI = i;
                taskMaxUtil.AddRunTask(() =>
                {
                    Console.WriteLine(tempI);
                    Thread.Sleep(1000);
                });
            }
            if (taskMaxUtil.WaitAll())
                Console.WriteLine("所有线程执行完成");
        }
## 十四、ICache，缓存帮助类(<font color="red">LgyUtil.Cache</font>)
- 缓存分为本地缓存(MemoryCache)、Redis缓存(RedisCache)。两个使用方法一样，构造一个静态变量，全局使用

        // 本地缓存，构造函数中，可以设置清理缓存时间，默认5分钟清理一次，使用TimeSpan.Zero，不进行清理
        // MemoryCache(TimeSpan? clearExpirationTime = null)
        public static ICache cache=new MemoryCache();
        // Redis缓存
        public static ICache cacheRedis=new ReidsCache("localhost");//构造函数，填写redis连接字符串
1. 设置缓存，Set\<T>(string key, T value, TimeSpan? expiresSliding = null,DateTime? expiressAbsoulte = null)，可以设置滑动过期时间和绝对过期时间

        //设置缓存后，每读取一次，缓存有效期延长1小时，缓存最多保持1天，1天后，缓存一定消失
        cache.Set("test",new List<string>(){"1","2"},TimeSpan.FromHours(1),DateTime.Now.AddDays(1));

        //设置缓存后，每读取一次，缓存有效期延长1小时，一直读取，一直延长。1小时内没有读取，缓存过期
        cache.Set("test",new List<string>(){"1","2"},TimeSpan.FromHours(1));

        //设置缓存后，读取也不会延长缓存有效期，缓存最多保持1天，1天后，缓存一定消失
        cache.Set("test",new List<string>(){"1","2"},null,DateTime.Now.AddDays(1));
2. 获取缓存，Get\<T>(string key)
3. 获取字符串缓存，GetString(string key)
4. 验证是否存在缓存，Exists(string key)
5. 删除一个缓存，Remove(string key)
6. 清空缓存，或清空多个缓存，RemoveAll(params string[] keys)
7. 根据key前缀，清空缓存，RemoveAllPrefix(string prefix)
8. 根据key前缀，获取缓存，GetKeysByPrefix(string prefix)
9. 获取所有key,GetAllKeys()
## 十五、网络请求帮助类(<font color="red">LgyUtil.Net</font>)
1. 获取HttpClient对象，使用完，不用释放

        NetUtil.GetHttpClient(string url, SocketsHttpHandler _socketsHttpHandler = null)
    - 静态ip获取一个永久不变的对象
    - 域名则是15分钟刷新一次的对象
2. 发送Post、Get请求
    - NetUtil.Post
    - NetUtil.PostAsync
    - NetUtil.Get
    - NetUtil.GetAsync

            HttpResponseMessage Post(string url, string postData = "", Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
            HttpResponseMessage Get(string url, Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
3. 发送Post、Get请求，返回字符串
    - NetUtil.Post_ReturnString
    - NetUtil.PostAsync_ReturnString
    - NetUtil.Get_ReturnString
    - NetUtil.GetAsync_ReturnString

            string Post_ReturnString(string url, string postData = "", Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
            string Get_ReturnString(string url, Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
4. 发送Post、Get请求，返回模型类
    - NetUtil.Post_ReturnModel
    - NetUtil.PostAsync_ReturnModel
    - NetUtil.Get_ReturnModel
    - NetUtil.GetAsync_ReturnModel

            T Post_ReturnModel<T>(string url, string postData = "", Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
            T Get_ReturnModel<T>(string url, Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
5. 发送Post、Get请求，返回流
    - NetUtil.Post_ReturnStream
    - NetUtil.PostAsync_ReturnStream
    - NetUtil.Get_ReturnStream
    - NetUtil.GetAsync_ReturnStream

            Stream Post_ReturnStream(string url, string postData = "", Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
            Stream Get_ReturnStream(string url, Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
## 十六、AppSettingUtil，配置项帮助类，仅支持json文件(LgyUtil)
1. AppSettingUtil.Init\<T>()，初始化并返回配置项实例，支持配置文件热重载
1. AppSettingUtil.InitStatic\<T>()，初始化静态类，支持配置文件热重载

    ~~~ c#
        T Init<T>(string jsonPath, bool hotReload = false, Action<T> afterInitOrReload = null)
        void InitStatic<T>(string jsonPath, bool hotReload = false, Action<T> afterInitOrReload = null)

        //配置项文件，比如是appSetting.json
        {
            "Conn": {
                "Db1": "abc",
                "Db2": "bcd"
            },
            "Other": 1
        }

        //1、初始化并返回实例，允许热重载
        Settings mySetting = AppSettingUtil.Init<Settings>("appSetting.json",true,(s)=>
        {
            //初始化和热重载执行的方法
            s.Conn.Db1="xyz";
        });
        //普通的配置项类
        public class Settings
        {
            public ConnSetting Conn { get; set; }
            public int Other { get; set; }

            public class ConnSetting
            {
                public string Db1 { get; set; }
                public string Db2 { get; set; }
            }
        }

        2、初始化静态类，允许热重载
        AppSettingUtil.InitStatic<Settings>("appSetting.json",true,(s)=>
        {
            //初始化和热重载执行的方法
            Settings.Conn.Db1="xyz";
        });
        //静态属性的配置项类
        public class Settings
        {
            //静态对象
            public static ConnSetting Conn { get; set; }
            //静态对象
            public static int Other { get; set; }

            public class ConnSetting
            {
                public string Db1 { get; set; }
                public string Db2 { get; set; }
            }
        }
    ~~~
## 十七、CronUtil，定时帮助类，对Quartz进行了封装(<font color="red">LgyUtil.Quartz</font>)
1. 开始必须初始化定时对象，执行:CronUtil.InitScheduler()
2. 添加定时任务
    - AddCronJob()，按照cron表达式定时
    - AddSecondJob()，按照秒定时
    - AddMinuteJob()，按照分钟定时
    - AddHourJob()，按照小时定时
    - AddJob()，自定义定时方式(TriggerBuilder对象)
3. 停止方法StopJob