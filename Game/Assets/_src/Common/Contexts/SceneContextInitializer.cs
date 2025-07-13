using UnityEngine;

namespace TFs.Common.Contexts
{
    public abstract class SceneContextInitializer : MonoBehaviour
    {
        public abstract void InstallBindings(IContextBinding contextBinding);
    }
}
