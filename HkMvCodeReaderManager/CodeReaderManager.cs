using MvCodeReaderSDKNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HkMvCodeReaderManager
{
    public class CodeReaderManager
    {
        /// <summary>
        /// 读码器容器，存储当前可连接的读码器
        /// </summary>
        public ObservableCollection<CoderReader> coderReaders = new ObservableCollection<CoderReader>();
        /// <summary>
        /// 构造方法
        /// </summary>
        public CodeReaderManager() {

            int nRet = MvCodeReader.MV_CODEREADER_OK;           
            MvCodeReader.MV_CODEREADER_DEVICE_INFO_LIST stDevList = new MvCodeReader.MV_CODEREADER_DEVICE_INFO_LIST();
            nRet = MvCodeReader.MV_CODEREADER_EnumDevices_NET(ref stDevList, MvCodeReader.MV_CODEREADER_GIGE_DEVICE);
            if (MvCodeReader.MV_CODEREADER_OK != nRet)
            {                
                throw new Exception(string.Format("搜索扫码器异常:{0:x8}", nRet));                
            }

            Console.WriteLine("已连接的设备数量: " + Convert.ToString(stDevList.nDeviceNum));
            if (0 == stDevList.nDeviceNum)
            {
                
            }

            MvCodeReader.MV_CODEREADER_DEVICE_INFO stDevInfo;                            // 通用设备信息

            // 根据设备信息创建一个CoderReader 实例，并将其放在coderReaders容器
            for (Int32 i = 0; i < stDevList.nDeviceNum; i++)
            {
                stDevInfo = (MvCodeReader.MV_CODEREADER_DEVICE_INFO)Marshal.PtrToStructure(stDevList.pDeviceInfo[i], typeof(MvCodeReader.MV_CODEREADER_DEVICE_INFO));
                CoderReader reader = new CoderReader(stDevInfo);
                coderReaders.Add(reader);
            }

            

        }

        /// <summary>
        /// 获取通过IP获取一个读写器
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public CoderReader GetCoderReaderByIp(string ip) 
        {
            foreach (var d in coderReaders ) 
            {
                if (d.Ip.Equals(ip)) 
                {
                    return d;                
                }                           
            }

            return null;
          
        }


    }
}
