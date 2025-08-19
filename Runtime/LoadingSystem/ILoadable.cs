namespace RedsUtils.LoadingSystem
{
    public interface ILoadable
    {
        void BeginLoad();         // Start loading
        float Progress { get; }   // Current progress (0 -> 1)
        bool IsDone { get; }      // Whether finished
    }
}