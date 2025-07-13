using System;
using System.Collections.Generic;

namespace TFs.Common.Contexts
{
    public partial class Context: IDisposable
    {
        private readonly Context _parentContext;
        private readonly Dictionary<Type, object> _services = new();
        private readonly SceneLoader _sceneLoader;

        public static Context CreateProjectContext(string sceneName)
        {
            var context = new Context();
            context._sceneLoader.InitializeContext(sceneName);
            return context;
        }

        private Context(Context parentContext = null)
        {
            _parentContext = parentContext;
            _sceneLoader = new SceneLoader(this);
        }

        //public Task LoadScene(SceneType scene, LoadSceneMode loadSceneMode = LoadSceneMode.Single) => _sceneLoader.LoadSceneWithContext(scene.GetSceneName(), loadSceneMode);

        public T Resolve<T>()
        {
            if (_services.TryGetValue(typeof(T), out var service))
                return (T)service;

            if (_parentContext != null)
                return _parentContext.Resolve<T>();

            throw new Exception($"Service of type {typeof(T)} not found");
        }

        private void Register<T>(T service)
        {
            _services[typeof(T)] = service;
        }

        public IEnumerable<object> GetAllServices() => _services.Values;

        public void Dispose()
        {
            foreach (var service in _services.Values)
            {
                if (service is IDisposable disposable)
                    disposable.Dispose();
            }
            _services.Clear();
        }
    }
}
