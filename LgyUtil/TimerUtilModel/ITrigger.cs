using System;
using System.Collections.Generic;

namespace LgyUtil.TimerUtilModel
{
    /// <summary>
    /// 触发器
    /// </summary>
    internal interface ITrigger
    {
        /// <summary>
        /// 触发器类型
        /// </summary>
        EnumTriggerType Type { get; }

        /// <summary>
        /// 上次触发时间
        /// </summary>
        DateTime NextTime { get; set; }

        /// <summary>
        /// 获取未来5次触发的时间
        /// </summary>
        /// <returns></returns>
        List<DateTime> ComputedNext5Times();

        /// <summary>
        /// 下次触发时间
        /// </summary>
        /// <returns></returns>
        DateTime GetNextTime();

        /// <summary>
        /// 验证触发器是否合法
        /// </summary>
        void IsValid();
    }
}