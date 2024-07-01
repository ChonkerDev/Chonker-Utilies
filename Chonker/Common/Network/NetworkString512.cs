using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Chonker.Common
{
    public struct NetworkString512 :  INetworkSerializeByMemcpy
    {
        private ForceNetworkSerializeByMemcpy<FixedString512Bytes> _info;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _info);
       
        }
 
        public override string ToString()
        {
            return _info.Value.ToString();
        }

        public char[] ToCharArray() {
            return _info.Value.ToString().ToCharArray();
        }
 
        public static implicit operator string(NetworkString512 s) => s.ToString();
        public static implicit operator NetworkString512(string s) => new NetworkString512() { _info = new FixedString512Bytes(s) };
    }
}
