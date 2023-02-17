# C#通用工具类说明
1、工具的每个部分，都发布了独立的包，可以按需引入(LgyUtil、LgyUtil.Cache、LgyUtil.Compress、LgyUtil.ModelCheck、LgyUtil.Net、LgyUtil.SSH)
2、下面每个说明最后括号内标记的内容，为工具所在的包，如 (LgyUtil),说明在LgyUtil包下
## 一、ArrayUtil，数组帮助类(LgyUtil)
    1、ForEach，Exists，Find，FindAll，FindIndex，Sort，ConvertAll，普通数组[]增加扩展功能，都是List存在的功能
        用法：(以ForEach为例)
            string[] str={"a","b","c"};
            str.ForEach(s=>s+"1");
    2、JoinToString，将数组内容拼接成字符串，普通数组[]和List数组扩展功能
        用法：
        1)用英文逗号拼接数组所有内容
            string[] str={"a","b","c"};
            Console.WriteLine(str.JoinToString()) //JoinToString不填写参数，默认用英文逗号分隔
            输出结果 a,b,c
        2)自定义拼接内容
            public Class Test { 
                public int a;
                public string b;
            }
            List<Test> tests=new List<Test>();
            //选择属性b，并且用+来拼接成字符串
            tests.JoinToString(t=>t.b,"+");
    3、ContainsAny，包含参数内多个内容的某一个即可返回ture，普通数组[]和List数组扩展功能
        用法：(解决了Contains方法只能输入一个参数进行判断)
            string[] str={"a","b","c","d"};
            str.ContainsAny("b","c");//数组某项 包含b 或者 包含c,就能返回true
    4、Slice，数组切片，普通数组[]和List数组扩展功能
        用法：(.net6 出现了新的切片语法糖[1..2]，可以替代这个方法)
            string[] str={"a","b","c","d"};
            str.Slice(1,2);//返回结果{"b","c"}，参数从0开始
    5、HaveContent，数组是否有内容，普通数组[]和List数组扩展功能
        用法：
            string[] str={"a","b","c","d"};  str.HaveContent()---->true
            string[] str={};                 str.HaveContent()---->false
            string[] str=null;               str.HaveContent()---->false
    6、IsNullOrEmpty，数组是否为空，普通数组[]和List数组扩展功能
        用法：
            string[] str={"a","b","c","d"};  str.IsNullOrEmpty()---->false
            string[] str={};                 str.IsNullOrEmpty()---->true
            string[] str=null;               str.IsNullOrEmpty()---->true
## 二、CronUtil，定时帮助类，对Quartz进行了封装(LgyUtil)
    1、开始必须初始化定时对象，执行:CronUtil.InitScheduler()
    2、添加定时任务
        1)AddCronJob，按照cron表达式定时
        2)AddSecondJob，按照秒定时
        3)AddMinuteJob，按照分钟定时
        4)AddHourJob，按照小时定时
        5)AddJob，自定义定时方式(TriggerBuilder对象)
    3、停止方法StopJob
## 三、DataTable帮助类，效率不高，程序中避免使用DataTable(LgyUtil)
    1、ToList<T>，将表转成数组
    2、ToDictionary<string,T>，将表转成字典
    3、ToDataTable，将DataRow[]转成DateTable，一般在表的Select以后使用
    4、ToModel<T>，将DataRow转成模型
## 四、DateTimeUtil，日期扩展方法(LgyUtil)
    1、ToyyyyMMdd，new DateTime(2000,5,2).ToyyyyMMdd() 输出：2000-05-02
    2、ToyyyyMMddCH，new DateTime(2000,5,2).ToyyyyMMddCH() 输出：2000年05月02日
    3、ToyyyyMMddHHmmss，new DateTime(2000,5,2,10,8,20).ToyyyyMMddHHmmss() 输出：2000-05-02 10:08:20
    4、ToyyyyMMddHHmmssCH，new DateTime(2000,5,2,10,8,20).ToyyyyMMddHHmmssCH() 输出：2000年05月02日 10:08:20
    5、GetTimestamp，获取当前unix时间戳(13位)
    6、GetDateTimeByTimestamp，js时间戳转本地日期
    7、AddQuarter，给传入日期 添加季度
    8、GetQuarter，获取传入日期的 季度
    9、GetMonthBetween，获取月份差，例如：5月和3月的月份差为2
    10、GetYearsStart，获取传入日期 年的第一天
    11、GetYearsEnd，获取传入日期 年的最后一天
    12、GetQuarterStart，获取传入日期 季度的第一天
    13、GetQuarterEnd，获取传入日期 季度最后一天
    14、GetMonthStart，获取传入日期 月的第一天
    15、GetMonthEnd，获取传入日期 月最后一天
    16、GetDaysStart，获取传入日期 0点0分0秒
    17、GetDaysEnd，获取传入日期  23点59分59秒
    18、GetHourStart，获取传入日期 0分0秒
    19、GetHourEnd，获取传入日期 59分59秒
    20、GetMinuteStart，获取传入日期 0秒
    21、GetMinuteEnd，获取传入日期 59秒
## 五、EncryptUtil，加密算法扩展(LgyUtil)
    1、AES加密解密(对称加密)：
        用法：
            string key="1234567890123456";//秘钥必须16位
            string vector = "1234567890123456";//偏移向量也必须是16位，可以不填写，有默认向量
            //加密
            string miwen = EncryptUtil.AESEncrypt("加密内容",key,vector);
            //解密
            string mingwen = EncryptUtil.AESDecrypt(miwen,key,vector);
    2、DES加密解密(对称加密)：与上面AES使用方法相同， 只是秘钥和向量都必须是8位长度
        EncryptUtil.EncryptDES
        EncryptUtil.DecryptDES
    3、RSA加密（非对称加密）：
        1、先获取公钥和私钥
            (string publicKey,string privateKey) = EncryptUtil.GetRSAKey();
        2、公钥加密：
            EncryptUtil.RSAPublicEncrypt("数据","公钥");
        3、私钥解密：
            EncryptUtil.RSAPrivateDecrypt("加密数据","私钥");
    4、GetMd5，获取字符串的md5
## 六、

