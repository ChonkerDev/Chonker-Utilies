using Unity.Collections;
using Unity.Netcode;

namespace Chonker.Common
{
    public struct NetworkString128 :  INetworkSerializeByMemcpy
    {
        private ForceNetworkSerializeByMemcpy<FixedString128Bytes> _info;
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
 
        public static implicit operator string(NetworkString128 s) => s.ToString();
        public static implicit operator NetworkString128(string s) => new NetworkString128() { _info = new FixedString128Bytes(s) };
    }
}
