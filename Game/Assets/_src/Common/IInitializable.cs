using System.Threading.Tasks;

namespace TFs.Common.Contexts
{
    public interface IInitializable
    {
        Task Initialize(Context context);
    }
}
