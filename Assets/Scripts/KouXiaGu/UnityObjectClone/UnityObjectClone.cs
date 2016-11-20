﻿using UnityEngine;


namespace KouXiaGu
{

    /// <summary>
    /// Unity物体实例化\克隆方法;
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UnityObjectClone : MonoBehaviour, IThreadToClone<XiaGuObject>, IThreadToClone<Component>
    {
        private UnityObjectClone() { }

        [SerializeField, Range(1, 300)]
        private uint times = 120;

        internal static readonly ConcurrentInstantiate concurrentInstantiate = new ConcurrentInstantiate();
        internal static readonly XiaGuObjectPool xiaGuObjectPool = new XiaGuObjectPool();
        internal static UnityObjectClone instance;

        public static UnityObjectClone GetInstance
        {
            get { return instance; }
        }

        [SerializeField]
        private int max = 0;

        [ShowOnlyProperty]
        public static int WaitDestroy0
        {
            get{ return concurrentInstantiate.WaitDestroyCount; }
        }
        [ShowOnlyProperty]
        public static int WaitDestroy1
        {
            get { return xiaGuObjectPool.WaitDestroyCount; }
        }
        [ShowOnlyProperty]
        public static int WaitInstantiate0
        {
            get{ return concurrentInstantiate.WaitInstantiateCount; }
        }
        [ShowOnlyProperty]
        public static int WaitInstantiate1
        {
            get { return xiaGuObjectPool.WaitInstantiateCount; }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogError("重复的实例化的单例!" + this.GetType().FullName);
                Destroy(this);
            }
        }

        /// <summary>
        /// 主线程更新;
        /// </summary>
        private void Update()
        {
            int count = WaitInstantiate0;
            if (max < count)
                max = count;

            concurrentInstantiate.MainThreadUpdate(times);
            xiaGuObjectPool.MainThreadUpdate(times);
        }

        public void InstantiateQueue(IRequestForInstance<XiaGuObject> asyncObject)
        {
            xiaGuObjectPool.InstantiateQueue(asyncObject);
        }

        public void InstantiateQueue(IRequestForInstance<Component> asyncObject)
        {
            concurrentInstantiate.InstantiateQueue(asyncObject);
        }


        //#region Instantiate

        ///// <summary>
        ///// 异步实例化;
        ///// </summary>
        //public static IAsyncState<Component> InstantiateInThread(RequestForInstanceAsync<Component> instantiate)
        //{
        //    return concurrentInstantiate.InstantiateState<>(instantiate);
        //}
        ///// <summary>
        ///// 异步实例化;
        ///// </summary>
        //public static IAsyncState<Component> InstantiateInThread(Component original)
        //{
        //    var instantiate = GetInstantiateRequest<Component>(original);
        //    return concurrentInstantiate.InstantiateQueue(instantiate);
        //}
        ///// <summary>
        ///// 异步实例化;
        ///// </summary>
        //public static IAsyncState<Component> InstantiateInThread(Component original, Vector3 position, Quaternion rotation)
        //{
        //    var instantiate = GetInstantiateRequest<Component>(original, position, rotation);
        //    return concurrentInstantiate.InstantiateQueue(instantiate);
        //}
        ///// <summary>
        ///// 异步实例化;
        ///// </summary>
        //public static IAsyncState<Component> InstantiateInThread(Component original, Transform parent, bool worldPositionStays = true)
        //{
        //    var instantiate = GetInstantiateRequest<Component>(original, parent, worldPositionStays);
        //    return concurrentInstantiate.InstantiateQueue(instantiate);
        //}
        ///// <summary>
        ///// 异步实例化;
        ///// </summary>
        //public static IAsyncState<Component> InstantiateInThread(Component original, Vector3 position, Quaternion rotation, Transform parent)
        //{
        //    var instantiate = GetInstantiateRequest<Component>(original, position, rotation, parent);
        //    return concurrentInstantiate.InstantiateQueue(instantiate);
        //}
        ///// <summary>
        ///// 异步摧毁物体;
        ///// </summary>
        //public static void DestroyInThread(Component instance)
        //{
        //    concurrentInstantiate.DestroyAsync(instance);
        //}

        //#endregion

        //#region Pool

        ///// <summary>
        ///// 从对象池取出物体,若存在物体则初始化返回,否则实例化\克隆物体并返回;
        ///// </summary>
        //public static XiaGuObject PoolInstantiate(XiaGuObject original)
        //{
        //    return xiaGuObjectPool.Instantiate(original);
        //}
        ///// <summary>
        ///// 从对象池取出物体,若存在物体则初始化返回,否则实例化\克隆物体并返回;
        ///// </summary>
        //public static XiaGuObject PoolInstantiate(XiaGuObject original, Vector3 position, Quaternion rotation)
        //{
        //    return xiaGuObjectPool.Instantiate(original, position, rotation);
        //}
        ///// <summary>
        ///// 从对象池取出物体,若存在物体则初始化返回,否则实例化\克隆物体并返回;
        ///// </summary>
        //public static XiaGuObject PoolInstantiate(XiaGuObject original, Transform parent, bool worldPositionStays = true)
        //{
        //    return xiaGuObjectPool.Instantiate(original, parent, worldPositionStays);
        //}
        ///// <summary>
        ///// 从对象池取出物体,若存在物体则初始化返回,否则实例化\克隆物体并返回;
        ///// </summary>
        //public static XiaGuObject PoolInstantiate(XiaGuObject original, Vector3 position, Quaternion rotation, Transform parent)
        //{
        //    return xiaGuObjectPool.Instantiate(original, position, rotation, parent);
        //}
        ///// <summary>
        ///// 尝试保存到对象池,若保存失败则摧毁物体;
        ///// </summary>
        //public static void PoolDestroy(XiaGuObject instance)
        //{
        //    xiaGuObjectPool.Destroy(instance);
        //}

        ///// <summary>
        ///// 异步的实例化,若存在对象池内则从对象池返回,否则创建一个克隆返回;
        ///// </summary>
        //public static IAsyncState<XiaGuObject> InstantiateInThread(RequestForInstanceAsync<XiaGuObject> asyncGameObject)
        //{
        //    return xiaGuObjectPool.InstantiateQueue(asyncGameObject);
        //}
        ///// <summary>
        ///// 异步的实例化,若存在对象池内则从对象池返回,否则创建一个克隆返回;
        ///// </summary>
        //public static IAsyncState<XiaGuObject> InstantiateInThread(XiaGuObject original)
        //{
        //    var instantiate = GetInstantiateRequest<XiaGuObject>(original);
        //    return xiaGuObjectPool.InstantiateQueue(instantiate);
        //}
        ///// <summary>
        ///// 异步的实例化,若存在对象池内则从对象池返回,否则创建一个克隆返回;
        ///// </summary>
        //public static IAsyncState<XiaGuObject> InstantiateInThread(XiaGuObject original, Vector3 position, Quaternion rotation)
        //{
        //    var instantiate = GetInstantiateRequest<XiaGuObject>(original, position, rotation);
        //    return xiaGuObjectPool.InstantiateQueue(instantiate);
        //}
        ///// <summary>
        ///// 异步的实例化,若存在对象池内则从对象池返回,否则创建一个克隆返回;
        ///// </summary>
        //public static IAsyncState<XiaGuObject> InstantiateInThread(XiaGuObject original, Transform parent, bool worldPositionStays = true)
        //{
        //    var instantiate = GetInstantiateRequest<XiaGuObject>(original, parent, worldPositionStays);
        //    return xiaGuObjectPool.InstantiateQueue(instantiate);
        //}
        ///// <summary>
        ///// 异步的实例化,若存在对象池内则从对象池返回,否则创建一个克隆返回;
        ///// </summary>
        //public static IAsyncState<XiaGuObject> InstantiateInThread(XiaGuObject original, Vector3 position, Quaternion rotation, Transform parent)
        //{
        //    var instantiate = GetInstantiateRequest<XiaGuObject>(original, position, rotation, parent);
        //    return xiaGuObjectPool.InstantiateQueue(instantiate);
        //}
        ///// <summary>
        ///// 异步的摧毁物体,或保存到对象池;
        ///// </summary>
        //public static void DestroyInThread(XiaGuObject instance)
        //{
        //    xiaGuObjectPool.DestroyAsync(instance);
        //}

        //#endregion

        //#region 获取到请求类;

        //private static RequestForInstanceAsync<T> GetInstantiateRequest<T>(T original)
        //     where T : Component
        //{
        //    RequestForInstanceAsync<T> instantiate = new RequestForInstanceAsync<T>(original);
        //    return instantiate;
        //}
        //private static RequestForInstanceAsync<T> GetInstantiateRequest<T>(T original, Vector3 position, Quaternion rotation)
        //     where T : Component
        //{
        //    RequestForInstanceAsync<T> instantiate = new RequestForInstanceAsync<T>(original, position, rotation);
        //    return instantiate;
        //}
        //private static RequestForInstanceAsync<T> GetInstantiateRequest<T>(T original, Transform parent, bool worldPositionStays)
        //    where T : Component
        //{
        //    RequestForInstanceAsync<T> instantiate = new RequestForInstanceAsync<T>(original, parent, worldPositionStays);
        //    return instantiate;
        //}
        //private static RequestForInstanceAsync<T> GetInstantiateRequest<T>(T original, Vector3 position, Quaternion rotation, Transform parent)
        //    where T : Component
        //{
        //    RequestForInstanceAsync<T> instantiate = new RequestForInstanceAsync<T>(original, position, rotation, parent);
        //    return instantiate;
        //}

        //#endregion

    }

}