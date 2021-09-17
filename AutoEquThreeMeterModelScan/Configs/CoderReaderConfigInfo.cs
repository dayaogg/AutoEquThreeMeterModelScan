using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace AutoEquThreeMeterModelScan.Configs
{
    /// <summary>
    /// 存放读码器的配置信息
    /// 三项扫码绑定的设备上只有两个扫码器，此配置文件保存两个读码器的配置信息包括：
    /// 1、内侧读码器的IP地址
    /// 2、外侧读码器的IP地址
    /// 3、内侧读码器的读取超时时间
    /// 4、外侧读码器的读取超时时间
    /// 5、是否连接内侧读码器
    /// 6、是否连接外侧读码器
    /// </summary>
    public class CoderReaderConfigInfo
    {
        /// <summary>
        /// 内侧读码器IP
        /// </summary>
        public string InsideCodeReaderIp { get; set; }

        /// <summary>
        /// 外侧读码器IP
        /// </summary>
        public string OutsideCoderReaderIp { get; set; }

        /// <summary>
        /// 内侧读码器超时时间
        /// </summary>
        public double InsideCodeReaderOutTime { get; set; }

        /// <summary>
        /// 外侧读码器超时时间
        /// </summary>
        public double OutsideCodeReaderOutTime { get; set; }

        /// <summary>
        ///内侧读码器是否打开
        /// </summary>
        public bool InsideIsOpen { get; set; }

        /// <summary>
        /// 外侧读码器是否打开
        /// </summary>
        public bool OutsideIsOpen { get; set; }

        public CoderReaderConfigInfo() {

            InsideCodeReaderIp = ConfigurationManager.AppSettings["InsideCodeReader"].ToString();

           
            double insideouttime;
            if (double.TryParse(ConfigurationManager.AppSettings["InsideTimeOut"].ToString(), out insideouttime))
            {
                InsideCodeReaderOutTime = insideouttime;
            }
            else {

                InsideCodeReaderOutTime = 1000;
                Console.WriteLine("无法将"+ ConfigurationManager.AppSettings["InsideTimeOut"].ToString()+"转换为内侧读码器超时时间");
            }

            string insideIsOpen = ConfigurationManager.AppSettings["InsideIsOpen"].ToString();

            if (insideIsOpen.Equals("1"))
            {

                InsideIsOpen = true;
            }
            else {

                InsideIsOpen = false;
            }




            OutsideCoderReaderIp = ConfigurationManager.AppSettings["OutsideCodeReader"].ToString();

            double outsideouttime;
            if (double.TryParse(ConfigurationManager.AppSettings["OutsideTimeOut"].ToString(), out outsideouttime))
            {
                OutsideCodeReaderOutTime = outsideouttime;
            }
            else
            {

                OutsideCodeReaderOutTime = 1000;
                Console.WriteLine("无法将" + ConfigurationManager.AppSettings["OutsideTimeOut"].ToString() + "转换为内侧读码器超时时间");
            }

            string outsideIsOpen = ConfigurationManager.AppSettings["OutsideIsOpen"].ToString();

            if (outsideIsOpen.Equals("1"))
            {

                OutsideIsOpen = true;
            }
            else
            {

                OutsideIsOpen = false;
            }

        }


        public void saveConfig() {

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["InsideCodeReader"].Value = InsideCodeReaderIp;
            config.AppSettings.Settings["OutsideCodeReader"].Value = OutsideCoderReaderIp;
            config.AppSettings.Settings["InsideTimeOut"].Value = InsideCodeReaderOutTime.ToString();
            config.AppSettings.Settings["OutsideTimeOut"].Value = OutsideCodeReaderOutTime.ToString();
            config.AppSettings.Settings["InsideIsOpen"].Value = InsideIsOpen.ToString();
            config.AppSettings.Settings["OutsideIsOpen"].Value = OutsideIsOpen.ToString();
            config.Save();

        }
    }
}
