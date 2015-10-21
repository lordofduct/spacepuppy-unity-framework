

namespace com.spacepuppy.Collections
{
    public interface ICachePool<T> where T : class
    {

        T GetInstance();

        void Release(T obj);

    }
}
