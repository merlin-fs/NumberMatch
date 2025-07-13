namespace TFs.Common.Contexts
{
    public interface IContextBinding
    {
        Context Context { get; }
        void Bind<T>(T service);
    }
}
