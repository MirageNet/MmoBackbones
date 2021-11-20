#if UNITY_SERVER || UNITY_EDITOR || DEVELOPMENT_BUILD

namespace LUD.DataStructures
{
    public struct ServerDataInfo
    {
        public string ServerName;
        public byte ServerId;
    }
}

#endif
