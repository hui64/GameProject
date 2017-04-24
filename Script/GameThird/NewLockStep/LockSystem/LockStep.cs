using UnityEngine;
using System.Collections;
namespace LockStep {
    public class KeyFarame {
        public int index;
        public string msg = "NoAction";

    }
    public class LockStep {
        //最大缓存服务器帧数
        public const int MaxServerFrameLen = 100;
        //缓存的网络帧
        public KeyFarame[] ServerKeyKrames = new KeyFarame[MaxServerFrameLen];
        private int InIndex = 0;
        private int OutIndex = 0;
        //帧同步初始化
        public void Init() {

        }
        //FixUpdate驱动接收服务器消息
        public void LoopByServer() {
            //添加ServerKeyKrames 并且同步更新InIndex的值(ServerKeyKrames.Add)
        }
        //帧同步循环(根据服务器同步)Update驱动(1.数据合并,减少发送次数, 而且可以防止网络抖动)
        public void LoopLockStep() {
            //根据每个Update的时间(Time.deltaTime)计算这一帧需要同步多少关键帧(一个关键帧含有固定次数逻辑帧)(ServerKeyKrames.Remove)
        }
        //断线重连系统(60秒重连, 出现网络断开, 如果是1V1游戏暂停进入等待模式, 如果大于1V1服务器记录断线时间点, 重连后发送断线过程中的操作数据)
        public void ReConnectPlayer(int playerId) {
            //循环连接
        }
        //中途加入系统(要求帧同步的过程中, 固定时间记录一个游戏物体的所有状态,可以不支持)
        public void AddPlayerInPlaying(int playerId) {

        }
    }
}
