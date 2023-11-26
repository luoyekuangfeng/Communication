
using static S7Communication.S7Enums;

namespace S7Communication
{
    public class Progarm
    {
       public static void Main(string[] args)
        {
            PLC_CPUType _CPUType = PLC_CPUType.S71200;
            string ip = "192.168.1.32";
            short rack = 0;
            short slot=1;
            S7CommunicationHelp.Instance.PLCConnect(_CPUType,ip, rack,slot);

            if (S7CommunicationHelp.Instance.IsConnected)
            {
                //开启左前方风扇
                FanViewModel.Instance.FanPowerFrontOpen(true,30);

                //关闭左前方风扇
                FanViewModel.Instance.FanPowerFrontOpen(false);


            }
        }
    }

}
