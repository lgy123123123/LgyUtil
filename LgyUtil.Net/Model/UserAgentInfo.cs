using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UAParser;

namespace LgyUtil.Net.Model
{
    /// <summary>
    /// 浏览器代理信息
    /// </summary>
    public sealed class UserAgentInfo
    {
        /// <summary>
        /// 操作系统信息
        /// </summary>
        public OS OS { get; }
        /// <summary>
        /// 设备信息
        /// </summary>
        public Device Device { get; }
        /// <summary>
        /// 浏览器信息
        /// </summary>
        public UserAgent UA { get; }

        public bool? _IsMobileDevice;
        /// <summary>
        /// 是否是移动设备
        /// </summary>
        public bool IsMobileDevice
        {
            get
            {
                if (_IsMobileDevice is null)
                {
                    _IsMobileDevice = MobileOSList.Contains(this.OS.Family) ||
                        MobileOSList.Contains(this.UA.Family) ||
                        MobileOSList.Contains(this.Device.Family);
                }
                return _IsMobileDevice.Value;
            }
        }

        public bool? _IsTablet;
        /// <summary>
        /// 是否是平板设备
        /// </summary>
        public bool IsTablet
        {
            get
            {
                if (_IsTablet is null)
                {
                    _IsTablet = Regex.IsMatch(this.Device.Family, "iPad|Kindle Fire|Nexus 10|Xoom|Transformer|MI PAD|IdeaTab", RegexOptions.CultureInvariant) ||
                        this.OS.Family == "BlackBerry Tablet OS";
                }
                return _IsTablet.Value;
            }
        }

        public bool? _IsComputer;
        /// <summary>
        /// 是否是普通电脑
        /// </summary>
        public bool IsComputer { 
            get
            {
                if(_IsComputer is null)
                {
                    _IsComputer = !IsMobileDevice && !IsTablet;
                }
                return _IsComputer.Value;
            }
        }

        public UserAgentInfo(ClientInfo cInfo)
        {
            this.OS = cInfo.OS;
            this.Device = cInfo.Device;
            this.UA = cInfo.UA;
        }

        /// <summary>
        /// 移动端操作系统列表
        /// </summary>
        private static readonly HashSet<string> MobileOSList = new HashSet<string>
        {
            "Android",
            "iOS",
            "Windows Mobile",
            "Windows Phone",
            "Windows CE",
            "Symbian OS",
            "BlackBerry OS",
            "BlackBerry Tablet OS",
            "Firefox OS",
            "Brew MP",
            "webOS",
            "Bada",
            "Kindle",
            "Maemo"
        };
        /// <summary>
        /// 移动端浏览器名称列表
        /// </summary>
        private static readonly HashSet<string> MobileBrowsersList = new HashSet<string>
        {
            "Android",
            "Firefox Mobile",
            "Opera Mobile",
            "Opera Mini",
            "Mobile Safari",
            "Amazon Silk",
            "webOS Browser",
            "MicroB",
            "Ovi Browser",
            "NetFront",
            "NetFront NX",
            "Chrome Mobile",
            "Chrome Mobile iOS",
            "UC Browser",
            "Tizen Browser",
            "Baidu Explorer",
            "QQ Browser Mini",
            "QQ Browser Mobile",
            "IE Mobile",
            "Polaris",
            "ONE Browser",
            "iBrowser Mini",
            "Nokia Services (WAP) Browser",
            "Nokia Browser",
            "Nokia OSS Browser",
            "BlackBerry WebKit",
            "BlackBerry", "Palm",
            "Palm Blazer",
            "Palm Pre",
            "Teleca Browser",
            "SEMC-Browser",
            "PlayStation Portable",
            "Nokia",
            "Maemo Browser",
            "Obigo",
            "Bolt",
            "Iris",
            "UP.Browser",
            "Minimo",
            "Bunjaloo",
            "Jasmine",
            "Dolfin",
            "Polaris",
            "Skyfire"
        };
        /// <summary>
        /// 移动设备列表
        /// </summary>
        private static readonly HashSet<string> MobileDevicesList = new HashSet<string>
        {
            "BlackBerry",
            "MI PAD",
            "iPhone",
            "iPad",
            "iPod",
            "Kindle",
            "Kindle Fire",
            "Nokia",
            "Lumia",
            "Palm",
            "DoCoMo",
            "HP TouchPad",
            "Xoom",
            "Motorola",
            "Generic Feature Phone",
            "Generic Smartphone"
        };
    }
}
