using MvCodeReaderSDKNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace HkMvCodeReaderManager
{
    /// <summary>
    /// 
    /// </summary>
    public class CoderReader:INotifyPropertyChanged
    {

        /// <summary>
        /// 读码器的MAC地址
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 读码器的IP地址(扫码器通过网口连接时使用)
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 扫码器的COM口号(扫码器通过USB3.0连接时使用)
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// 已连接的读码器实例
        /// </summary>
        private MvCodeReader device ;

        /// <summary>
        /// 读码器信息
        /// </summary>
        private MvCodeReader.MV_CODEREADER_DEVICE_INFO stDevInfo;

        private bool isopen = false;

        /// <summary>
        /// 读码器的连接状态
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return isopen;
            }

            set
            {
                isopen = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsOpen"));
            }
        } 
        public event PropertyChangedEventHandler PropertyChanged;






        /// <summary>
        /// 
        /// </summary>
        /// <param name="stDevInfo"></param>
        public CoderReader(MvCodeReader.MV_CODEREADER_DEVICE_INFO stDevInfo)
        {
            this.stDevInfo = stDevInfo;

            if (MvCodeReader.MV_CODEREADER_GIGE_DEVICE == stDevInfo.nTLayerType)
            {
                MvCodeReader.MV_CODEREADER_GIGE_DEVICE_INFO stGigEDeviceInfo = (MvCodeReader.MV_CODEREADER_GIGE_DEVICE_INFO)MvCodeReader.ByteToStruct(stDevInfo.SpecialInfo.stGigEInfo, typeof(MvCodeReader.MV_CODEREADER_GIGE_DEVICE_INFO));
                uint nIp1 = ((stGigEDeviceInfo.nCurrentIp & 0xff000000) >> 24);
                uint nIp2 = ((stGigEDeviceInfo.nCurrentIp & 0x00ff0000) >> 16);
                uint nIp3 = ((stGigEDeviceInfo.nCurrentIp & 0x0000ff00) >> 8);
                uint nIp4 = (stGigEDeviceInfo.nCurrentIp & 0x000000ff);                
                Name = stGigEDeviceInfo.chUserDefinedName;
                Ip = nIp1 + "." + nIp2 + "." + nIp3 + "." + nIp4;              
            }
            else if (MvCodeReader.MV_CODEREADER_USB_DEVICE == stDevInfo.nTLayerType)
            {
                MvCodeReader.MV_CODEREADER_USB3_DEVICE_INFO stUsb3DeviceInfo = (MvCodeReader.MV_CODEREADER_USB3_DEVICE_INFO)MvCodeReader.ByteToStruct(stDevInfo.SpecialInfo.stUsb3VInfo, typeof(MvCodeReader.MV_CODEREADER_USB3_DEVICE_INFO));              
                Name = stUsb3DeviceInfo.chUserDefinedName;
                SerialNumber = stUsb3DeviceInfo.chSerialNumber;
            }
        }

        /// <summary>
        /// 连接读码器
        /// </summary>
        public void ConnectReader() {

            Console.WriteLine("正在尝试打开 device：" + Ip);
            CloseCodeReader();
            device = new MvCodeReader();


            int nRet = device.MV_CODEREADER_CreateHandle_NET(ref stDevInfo);
            if (MvCodeReader.MV_CODEREADER_OK != nRet)
            {
                throw new CoderReadException(string.Format("创建 device [ " + Ip + " ] 异常:{0:x8}", nRet));
            }

            // ch:打开设备 | en:Open device
            nRet = device.MV_CODEREADER_OpenDevice_NET();
            if (MvCodeReader.MV_CODEREADER_OK != nRet)
            {

                throw new CoderReadException(string.Format("打开 device [ " + Ip + " ]异常:{0:x8}", nRet));

            }

            // ch:设置触发模式为off || en:set trigger mode as off
            if (MvCodeReader.MV_CODEREADER_OK != device.MV_CODEREADER_SetEnumValue_NET("TriggerMode", 0))
            {
                throw new CoderReadException(string.Format("读码器[ " + Ip + " ]设置触发模式off失败"));

            }
            IsOpen = true;

        }

        public void CloseCodeReader() 
        {

            if (device == null) return;

            int nRet = device.MV_CODEREADER_CloseDevice_NET();
            if (MvCodeReader.MV_CODEREADER_OK != nRet)
            {               
                throw new CoderReadException(string.Format("断开读写器异常：{0:x8}", nRet));
            }

            // ch:销毁设备 | en:Destroy device
            nRet = device.MV_CODEREADER_DestroyHandle_NET();
            if (MvCodeReader.MV_CODEREADER_OK != nRet)
            {
                Console.WriteLine("释放读写器异常:{0:x8}", nRet);
               
            }
            device = null;
            IsOpen = false;
        }


        /// <summary>
        /// 获取一个条码数据
        /// </summary>
        /// <param name="OutTime">等待超时时间 单位ms</param>
        /// <returns> 返回读取到的条码数据</returns>
        public string ReadAnCode(double OutTime) {

            if (device == null) return "";

            string code = "";
            // ch:开启抓图 | en:start grab

             int  nRet = device.MV_CODEREADER_StartGrabbing_NET();
            if (MvCodeReader.MV_CODEREADER_OK != nRet)
            {
                throw new CoderReadException(string.Format("开始抓取图像失败 :{0:x8}", nRet));
                               
            }

            
            IntPtr pBufForDriver = IntPtr.Zero;

            MvCodeReader.MV_CODEREADER_IMAGE_OUT_INFO_EX2 stFrameInfo = new MvCodeReader.MV_CODEREADER_IMAGE_OUT_INFO_EX2();
            IntPtr pstFrameInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MvCodeReader.MV_CODEREADER_IMAGE_OUT_INFO_EX2)));
            Marshal.StructureToPtr(stFrameInfo, pstFrameInfo, false);

            DateTime ScanStart = DateTime.Now;

            while (true) {

                DateTime now = DateTime.Now;

                TimeSpan span = now - ScanStart;

                double span_ms = span.TotalMilliseconds;

                if (span_ms > OutTime) {

                    break;
                }

                nRet = device.MV_CODEREADER_GetOneFrameTimeoutEx2_NET(ref pBufForDriver, pstFrameInfo, 1000);


                if (MvCodeReader.MV_CODEREADER_OK == nRet)
                {
                    stFrameInfo = (MvCodeReader.MV_CODEREADER_IMAGE_OUT_INFO_EX2)Marshal.PtrToStructure(pstFrameInfo, typeof(MvCodeReader.MV_CODEREADER_IMAGE_OUT_INFO_EX2));

                    Console.WriteLine("Get One Frame:" + "nChannelID[" + Convert.ToString(stFrameInfo.nChannelID) + "] , Width[" + Convert.ToString(stFrameInfo.nWidth) + "], Height[" + Convert.ToString(stFrameInfo.nHeight)
                                    + "] , FrameNum[" + Convert.ToString(stFrameInfo.nFrameNum) + "]");

                    // 分配条码内存空间
                    MvCodeReader.MV_CODEREADER_RESULT_BCR_EX stBcrResult = (MvCodeReader.MV_CODEREADER_RESULT_BCR_EX)Marshal.PtrToStructure(stFrameInfo.pstCodeListEx, typeof(MvCodeReader.MV_CODEREADER_RESULT_BCR_EX));

                    Console.WriteLine("Get CodeNum:" + "CodeNum[" + Convert.ToString(stBcrResult.nCodeNum) + "]");

                    if (stBcrResult.nCodeNum>1) {

                        Console.WriteLine("读取到多个条码");

                    }
                   
                    for (int i = 0; i < stBcrResult.nCodeNum; ++i)
                    {
                        bool bIsValidUTF8 = IsTextUTF8(stBcrResult.stBcrInfoEx[i].chCode);
                        if (bIsValidUTF8)
                        {
                            string strCode = Encoding.UTF8.GetString(stBcrResult.stBcrInfoEx[i].chCode);
                            Console.WriteLine("Get CodeNum: " + "CodeNum[" + i.ToString() + "], CodeString[" + strCode.Trim().TrimEnd('\0') + "]");
                            code = strCode.Trim().TrimEnd('\0');
                            break;
                        }
                        else
                        {
                            string strCode = Encoding.GetEncoding("GB2312").GetString(stBcrResult.stBcrInfoEx[i].chCode);
                            Console.WriteLine("Get CodeNum: " + "CodeNum[" + i.ToString() + "], CodeString[" + strCode.Trim().TrimEnd('\0') + "]");
                            code = strCode.Trim().TrimEnd('\0');
                            break;
                        }

                        
                    }

                    if (!string.IsNullOrEmpty(code))
                    {

                        break;
                    }


                }
                else
                {
                    Console.WriteLine("No data:{0:x8}", nRet);
                }





            }


            // ch:停止抓图 | en:Stop grab image
            nRet = device.MV_CODEREADER_StopGrabbing_NET();
            if (MvCodeReader.MV_CODEREADER_OK != nRet)
            {
                throw new CoderReadException(string.Format("停止抓取图像失败 :{0:x8}", nRet));
            }

            return code;
              
        }


        public static bool IsTextUTF8(byte[] inputStream)
        {
            int encodingBytesCount = 0;
            bool allTextsAreASCIIChars = true;

            for (int i = 0; i < inputStream.Length; i++)
            {
                byte current = inputStream[i];

                if ((current & 0x80) == 0x80)
                {
                    allTextsAreASCIIChars = false;
                }
                // First byte
                if (encodingBytesCount == 0)
                {
                    if ((current & 0x80) == 0)
                    {
                        // ASCII chars, from 0x00-0x7F
                        continue;
                    }

                    if ((current & 0xC0) == 0xC0)
                    {
                        encodingBytesCount = 1;
                        current <<= 2;

                        // More than two bytes used to encoding a unicode char.
                        // Calculate the real length.
                        while ((current & 0x80) == 0x80)
                        {
                            current <<= 1;
                            encodingBytesCount++;
                        }
                    }
                    else
                    {
                        // Invalid bits structure for UTF8 encoding rule.
                        return false;
                    }
                }
                else
                {
                    // Following bytes, must start with 10.
                    if ((current & 0xC0) == 0x80)
                    {
                        encodingBytesCount--;
                    }
                    else
                    {
                        // Invalid bits structure for UTF8 encoding rule.
                        return false;
                    }
                }
            }

            if (encodingBytesCount != 0)
            {
                // Invalid bits structure for UTF8 encoding rule.
                // Wrong following bytes count.
                return false;
            }

            // Although UTF8 supports encoding for ASCII chars, we regard as a input stream, whose contents are all ASCII as default encoding.
            return !allTextsAreASCIIChars;
        }



    }

    public class CoderReadException : Exception {


        public CoderReadException(string errMessage):base(errMessage) {

            
        }
    }
}
