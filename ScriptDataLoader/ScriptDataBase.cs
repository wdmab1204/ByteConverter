using ByteConverter;

namespace ScriptDataLoader
{
    public abstract class ScriptDataBase<TData> where TData : struct
    {
        public virtual TData Data { get; set; }

        public abstract int GetKey();

        public override int GetHashCode()
        {
            return GetKey();
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }
    }

    public class ItemScriptData : ScriptDataBase<ItemData>
    {
        public override int GetKey()
        {
            return Data.timeStamp;
        }
    }
}
