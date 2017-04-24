using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Linq;
namespace LockStep {
    public class FrameCompare {
        private bool IsHaveError = false;
        private int sum = 0;
        private static FrameCompare _instance;
        private FrameCompare() {

        }
        public static FrameCompare getIntance() {
            if(_instance == null) {
                _instance = new FrameCompare();
            }
            return _instance;
        }
        //============================================= 外部调用 =========================================
        //游戏一开始时调用(必须在Check函数之前调用, 为了少做判断所以这样设计)
        public void GameStart(GameObject go) {
            go.AddComponent<GameMono>();
            InitFrameCompare();
            Debug.Log("拥有对比日志 :" + IsHaveCache);
            Debug.Log(Application.persistentDataPath);
        }
        //游戏结束后调用
        public void GameOver() {
            FileEventByGameOver();
        }
        //每帧结束后调用
        public void FrameOver() {

        }
        public void Check(StepVector3 vector3) {
#if CHECKSTEP
            Check(vector3.x);
            Check(vector3.y);
            Check(vector3.z);
# endif
        }
        //    public void Check(StepQuaternion vector4) {
        //#if CHECKSTEP
        //        Check(vector4.x);
        //        Check(vector4.y);
        //        Check(vector4.z);
        //        Check(vector4.w);
        //# endif
        //    }
        public void Check(string str) {
            if(IsHaveError)
                return;
            buf = System.Text.Encoding.Default.GetBytes(str);
            for(int i = 0;i < buf.Length;i++) {
                ByteStartPos++;
                if(ByteStartPos >= MaxByteLen) {
                    ByteStartPos = 0;
                    SaveByte();
                    ReadByte();
                }
                OnBytes[ByteStartPos] = buf[i];
                if(IsCanCompare)
                    if(!CacheBytes[ByteStartPos].Equals(OnBytes[ByteStartPos]))
                        ErrorEvent(str);
            }
        }
        public void Check(int num) {
            if(IsHaveError)
                return;
            ObjectToBytes(num);
            for(int i = 0;i < buf.Length;i++) {
                ByteStartPos++;
                if(ByteStartPos >= MaxByteLen) {
                    ByteStartPos = 0;
                    SaveByte();
                    ReadByte();
                }
                OnBytes[ByteStartPos] = buf[i];
                if(IsCanCompare)
                    if(!CacheBytes[ByteStartPos].Equals(OnBytes[ByteStartPos]))
                        ErrorEvent(num);
            }
        }
        public void Check(bool mybool) {
            if(IsHaveError)
                return;
            ObjectToBytes(mybool);
            for(int i = 0;i < buf.Length;i++) {
                ByteStartPos++;
                if(ByteStartPos >= MaxByteLen) {
                    ByteStartPos = 0;
                    SaveByte();
                    ReadByte();
                }
                OnBytes[ByteStartPos] = buf[i];
                if(IsCanCompare)
                    if(!CacheBytes[ByteStartPos].Equals(OnBytes[ByteStartPos]))
                        ErrorEvent(mybool);
            }
        }
        //============================================= 外部调用 ==========================================
        private void InitFrameCompare() {
            InitStream();
        }
        private byte[] buf;
        //当数据出现不同步的时候调用(取附近与之对应的数据)
        private void ErrorEvent<T>(T t) {
            if(IsHaveError)
                return;
            IsCanCompare = true;
            IsHaveError = true;
            Debug.LogError("帧同步出错");
            Debug.LogError("index: " + (ReadStream.Position + ByteStartPos));
            Debug.LogError(t);
            BackBytes = new byte[MaxByteLen];
            BackStartPos = ByteStartPos + 1;
            ByteStartPos++;
            GameMono.LoopEvent = LoopGetError;
        }
        bool isBack = true;
        int BackErrorReadIndex = 0;
        int ErrorReadIndex = 0;
        private byte[] ErrorBuffCache;
        //直接获取数据太卡(计算量太大), 用update获取附近相同的数据
        private void LoopGetError() {
            for(int i = 0;i < 1000;i++) {
                LoopToGetData();
                if(isBackReadOver && isForwardReadOver) {
                    Debug.LogError("没有找到相同数据");
                    GameMono.LoopEvent = null;
                    return;
                }
            }
        }
        private void LoopToGetData() {
            if(isBack && !isBackReadOver) {
                if(SameBytes(buf,GetBackBuff())) {
                    ReadStream.Position -= (ErrorReadIndex + BackErrorReadIndex) * MaxByteLen;
                    Debug.LogError("同样数据index: " + (ReadStream.Position + BackStartPos));
                    GameMono.LoopEvent = null;
                }
            }
            if(!isBack && !isForwardReadOver) {
                if(SameBytes(buf,GetForwardBuff())) {
                    Debug.LogError("同样数据index: " + (ReadStream.Position + ByteStartPos));
                    GameMono.LoopEvent = null;
                }
            }
            isBack = !isBack;
            if(isForwardReadOver)
                isBack = true;
            if(isBackReadOver)
                isBack = false;
        }
        private bool SameBytes(byte[] a,byte[] b) {
            for(int i = 0;i < a.Length;i++) {
                if(!a[i].Equals(b[i])) {
                    return false;
                }
            }
            return true;
        }
        public void ObjectToBytes(object obj) {
            buf = new byte[System.Runtime.InteropServices.Marshal.SizeOf(obj)];
            IntPtr ptr = System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(buf,0);
            System.Runtime.InteropServices.Marshal.StructureToPtr(obj,ptr,true);
        }
        //=========================================  IO文件操作 ==========================================
        private string SavePath = Application.persistentDataPath + "/" + "CacheFrameCompare.bin";
        private string ReadPath = Application.persistentDataPath + "/" + "FrameCompare.bin";
        private FileStream ReadStream;
        private FileStream SaveStream;
        private int MaxByteLen = 500;
        private byte[] OnBytes;
        private byte[] CacheBytes;
        private int ByteStartPos = 0;
        private bool IsHaveCache;
        private bool IsCanCompare;
        private void InitStream() {
            IsHaveCache = File.Exists(ReadPath);
            IsCanCompare = IsHaveCache;
            if(IsHaveCache) {
                ReadStream = new FileStream(ReadPath,FileMode.Open);
            }
            SaveStream = new FileStream(SavePath,FileMode.Create,FileAccess.Write,FileShare.None);
            OnBytes = new byte[MaxByteLen];
            CacheBytes = new byte[MaxByteLen];
            ReadByte();
        }
        //游戏结束后file运行事件
        private void FileEventByGameOver() {
            if(IsHaveError) {
                if(IsHaveCache) {
                    ReadStream.Flush();
                    ReadStream.Close();
                }
                SaveStream.Flush();
                SaveStream.Close();
                File.Delete(SavePath);
                return;
            }
            SaveByte();
            if(IsHaveCache) {
                ReadStream.Flush();
                ReadStream.Close();
                File.Delete(ReadPath);
            }
            SaveStream.Flush();
            SaveStream.Close();
            File.Move(SavePath,ReadPath);
        }
        //读取byte[]
        private void ReadByte() {
            if(!IsCanCompare)
                return;
            if(isForwardReadOver)
                return;
            if(ReadStream.Position + MaxByteLen < ReadStream.Length) {
                ReadStream.Read(CacheBytes,0,MaxByteLen);
            }
            else {
                ReadStream.Read(CacheBytes,0,(int)(ReadStream.Position + MaxByteLen - ReadStream.Length));
                isForwardReadOver = true;
                IsCanCompare = false;
            }
            ReadStream.Position += MaxByteLen;
        }
        private byte[] BackBytes;
        private bool isBackReadOver = false;
        private bool isForwardReadOver = false;

        private void BackReadByte() {
            long oldpos = ReadStream.Position;
            long pos = oldpos - (ErrorReadIndex + BackErrorReadIndex) * MaxByteLen;
            long dif = 0;
            if(pos >= 0) {
                ReadStream.Position = pos;
                ReadStream.Read(BackBytes,0,MaxByteLen);
            }
            else {
                isBackReadOver = true;
                dif = pos;
                ReadStream.Position = 0;
                try {
                    ReadStream.Read(BackBytes,0,(int)(MaxByteLen + dif));
                }
                catch(System.Exception e) {
                    Debug.LogError((int)(MaxByteLen + dif));
                    GameMono.LoopEvent = null;
                }
            }
            ReadStream.Position = oldpos;
        }
        //保存byte[]
        private void SaveByte() {
            SaveStream.Write(OnBytes,0,MaxByteLen);
            SaveStream.Position += MaxByteLen;
        }
        //正常方向获取byte[]
        private byte[] GetForwardBuff() {
            ErrorBuffCache = CacheBytes;
            byte[] comBuff = new byte[buf.Length];
            for(int i = 0;i < buf.Length;i++) {
                ByteStartPos++;
                if(ByteStartPos >= MaxByteLen) {
                    ByteStartPos = 0;
                    ReadByte();
                    ErrorReadIndex++;
                }
                try {
                    comBuff[i] = CacheBytes[ByteStartPos];
                }
                catch(System.Exception e) {
                    Debug.LogError(i);
                    Debug.LogError(ByteStartPos);
                    GameMono.LoopEvent = null;
                }
            }
            for(int i = 0;i < buf.Length - 1;i++) {
                ByteStartPos--;
                if(ByteStartPos < 0) {
                    ByteStartPos = MaxByteLen - 1;
                    CacheBytes = ErrorBuffCache;
                    ErrorReadIndex--;
                }
            }
            return comBuff;
        }
        private int BackStartPos = 0;
        //向后获取byte[]
        private byte[] GetBackBuff() {
            ErrorBuffCache = BackBytes;
            byte[] comBuff = new byte[buf.Length];
            for(int i = 0;i < buf.Length;i++) {
                BackStartPos--;
                if(BackStartPos <= 0) {
                    BackStartPos = MaxByteLen - 1;
                    BackReadByte();
                    BackErrorReadIndex++;
                }
                try {
                    comBuff[buf.Length - 1 - i] = BackBytes[BackStartPos];
                }
                catch(System.Exception e) {
                    Debug.LogError(buf.Length - 1 - i);
                    Debug.LogError(BackStartPos);
                    GameMono.LoopEvent = null;
                }
            }
            for(int i = 0;i < buf.Length - 1;i++) {
                BackStartPos++;
                if(BackStartPos > MaxByteLen) {
                    BackStartPos = 0;
                    BackBytes = ErrorBuffCache;
                    BackErrorReadIndex--;
                }
            }
            return comBuff;
        }
    }
}
