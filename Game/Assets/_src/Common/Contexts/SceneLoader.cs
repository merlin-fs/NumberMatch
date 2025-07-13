using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TFs.Common.Contexts
{
    public partial class Context
    {
        private class ContextBinding: IContextBinding
        {
            private readonly Context _context;
            public Context Context => _context;

            public ContextBinding(Context context)
            {
                _context = context;
            }

            public void Bind<T>(T service) => _context.Register(service);
        }

        private class SceneLoader
        {
            private readonly Context _context;

            public SceneLoader(Context context)
            {
                _context = context;
            }

            public async Task LoadSceneWithContext(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
            {
                var loadOp = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                loadOp!.completed += handle =>
                {
                    InitializeContext(sceneName);
                };

                while (!loadOp.isDone)
                    await Task.Yield();
            }

            public void InitializeContext(string sceneName)
            {
                var initializer = Object.FindFirstObjectByType<SceneContextInitializer>();
                var newContext = new Context(_context);

                if (initializer != null)
                    initializer.InstallBindings(new ContextBinding(newContext));
                else
                    Debug.LogError("SceneContextInitializer not found in scene");

                var loadedScene = SceneManager.GetSceneByName(sceneName);
                InitializeGameObjects(loadedScene, newContext);
            }

            private static void InitializeGameObjects(Scene loadedScene, Context context)
            {
                var rootGameObjects = new List<GameObject>();
                loadedScene.GetRootGameObjects(rootGameObjects);

                var initializables = new HashSet<IInitializable>();
                foreach (var init in rootGameObjects.SelectMany(root => root.GetComponentsInChildren<IInitializable>(true)))
                {
                    if (init is Component component && component.gameObject.scene == loadedScene)
                    {
                        initializables.Add(init);
                    }
                }

                foreach (var initializable in initializables)
                {
                    initializable.Initialize(context)
                        .ContinueWith(t => Debug.LogException(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
                }
            }
        }
    }
}

