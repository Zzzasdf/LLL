using System;
using System.Collections.Generic;

namespace ConditionModule.Integration
{
    public interface ICondition<T, TConditionType>
        where TConditionType: struct
    {
        void GetConditionTypes(ICollection<TConditionType> types);
        bool CheckCondition(T t);
    }
    public interface IResponder<T, TConditionType>: ICondition<T, TConditionType>
        where TConditionType: struct
    {
        void Reset();
        void Destroy();
        
        void Fire();
        void Fire(TConditionType type);
        void FireNow();
        void FireNow(TConditionType type);

        void Subscribe(T t, Action<T, bool> callback);
        void Unsubscribe(T t, Action<T, bool> callback);
        bool Status(T t);
    }
    /// 条件响应器
    /// CPU 优化点：1、永久性条件自动取消订阅
    public abstract class ResponderBase<T, TConditionType>: IResponder<T, TConditionType>
        where TConditionType: struct
    {
        /// kv 映射 配置
        private Dictionary<T, ConditionDataWrapper<T, TConditionType>> types2ConfigDict;
        /// 条件类型 映射 配置集 （检测）
        private Dictionary<TConditionType, PooledHashSet<ConditionDataWrapper<T, TConditionType>>> type2ConfigsDict;
        // 单线程仅需一个缓冲器
        private DelayBuffer<TConditionType> delayBuffer;
        
        private Action<ICollection<TConditionType>> getConditionTypesFunc;
        private Func<T, bool> checkConditionFunc;
        private IResponder<T, TConditionType> responderImplementation;

        public ResponderBase()
        {
            types2ConfigDict = new Dictionary<T, ConditionDataWrapper<T, TConditionType>>();
            type2ConfigsDict = new Dictionary<TConditionType, PooledHashSet<ConditionDataWrapper<T, TConditionType>>>();
            delayBuffer = new DelayBuffer<TConditionType>(10, Fire, Fire, Fire);
            getConditionTypesFunc = GetConditionTypes;
            checkConditionFunc = CheckCondition;
        }
        void IResponder<T, TConditionType>.Reset()
        {
            delayBuffer.Reset();
            Clear();
        }
        void IResponder<T, TConditionType>.Destroy()
        {
            delayBuffer.Destroy();
            delayBuffer = null;
            Clear();
        }
        private void Clear()
        {
            foreach (var pair in type2ConfigsDict)
            {
                pair.Value.Clear();
                ((IDisposable)pair.Value).Dispose();
            }
            type2ConfigsDict.Clear();

            foreach (var pair in types2ConfigDict)
            {
                ((IDisposable)pair.Value).Dispose();
            }
            types2ConfigDict.Clear();
        }

        void IResponder<T, TConditionType>.Fire() => delayBuffer.FireAllType();
        void IResponder<T, TConditionType>.Fire(TConditionType type) => delayBuffer.FireType(type);
        void IResponder<T, TConditionType>.FireNow() => delayBuffer.FireAllTypeNow();
        void IResponder<T, TConditionType>.FireNow(TConditionType type) => delayBuffer.FireTypeNow(type);
        private void Fire()
        {
            foreach (var pair in types2ConfigDict)
            {
                ConditionDataWrapper<T, TConditionType> wrapper = pair.Value;
                wrapper.Fire();
            }
        }
        private void Fire(HashSet<TConditionType> types)
        {
            using (var wrappers = PooledHashSet<ConditionDataWrapper<T, TConditionType>>.Get())
            {
                foreach (var type in types)
                {
                    if (!type2ConfigsDict.TryGetValue(type, out PooledHashSet<ConditionDataWrapper<T, TConditionType>> hashSet))
                    {
                        continue;
                    }
                    wrappers.AddRange(hashSet);
                }
                foreach (var wrapper in wrappers)
                {
                    wrapper.Fire();
                }
            }
        }
        private void Fire(TConditionType type)
        {
            if (!type2ConfigsDict.TryGetValue(type, out PooledHashSet<ConditionDataWrapper<T, TConditionType>> wrappers))
            {
                return;
            }
            foreach (var wrapper in wrappers)
            {
                wrapper.Fire();
            }
        }

        // 订阅
        void IResponder<T, TConditionType>.Subscribe(T t, Action<T, bool> callback)
        {
            if (!types2ConfigDict.TryGetValue(t, out ConditionDataWrapper<T, TConditionType> wrapper))
            {
                wrapper = ConditionDataWrapper<T, TConditionType>.Get(t, getConditionTypesFunc, checkConditionFunc);
                AddData(wrapper, t);
            }
            wrapper.Subscribe(callback);
            // 订阅时触发一次检测
            callback.Invoke(wrapper.UnWrapper(), wrapper.Status());
        }
        /// 取消订阅
        void IResponder<T, TConditionType>.Unsubscribe(T t, Action<T, bool> callback)
        {
            if (!types2ConfigDict.TryGetValue(t, out ConditionDataWrapper<T, TConditionType> wrapper))
            {
                return;
            }
            wrapper.Unsubscribe(callback);
            if (!wrapper.CheckNull()) return;
            RemoveData(wrapper);
        }
        /// 状态
        bool IResponder<T, TConditionType>.Status(T t)
        {
            if (!types2ConfigDict.TryGetValue(t, out ConditionDataWrapper<T, TConditionType> wrapper))
            {
                AddData(wrapper, t);
            }
            return wrapper.Status();
        }

        private void AddData(in ConditionDataWrapper<T, TConditionType> wrapper, T t)
        {
            // 加入 kv 映射
            types2ConfigDict.Add(t, wrapper);
            // 归入 条件类型检测字典
            using (var conditionTypes = PooledHashSet<TConditionType>.Get())
            {
                wrapper.GetTypes(conditionTypes);
                foreach (var eConditionType in conditionTypes)
                {
                    if (!type2ConfigsDict.TryGetValue(eConditionType, out PooledHashSet<ConditionDataWrapper<T, TConditionType>> wrappers))
                    {
                        type2ConfigsDict.Add(eConditionType, wrappers = PooledHashSet<ConditionDataWrapper<T, TConditionType>>.Get());
                    }
                    wrappers.Add(wrapper);
                }
            }
        }
        private void RemoveData(ConditionDataWrapper<T, TConditionType> wrapper)
        {
            // 从 条件类型字典 中删除
            using (var conditionTypes = PooledHashSet<TConditionType>.Get())
            {
                wrapper.GetTypes(conditionTypes);
                foreach (var eConditionType in conditionTypes)
                {
                    type2ConfigsDict[eConditionType].Remove(wrapper);
                }
            }
            // 从 kv 字典中 删除
            types2ConfigDict.Remove(wrapper.UnWrapper());
            ((IDisposable)wrapper).Dispose();
        }
        
        private class ConditionDataWrapper<T, TConditionType>: IDisposable
            where TConditionType: struct
        {
            private static readonly MonitoredObjectPool.ObjectPool<ConditionDataWrapper<T, TConditionType>, T> s_Pool = 
                new("ConditionDataWrapper", () => new ConditionDataWrapper<T, TConditionType>(), 
                null,
                x =>
                {
                    x.callbacks.Clear();
                    x.status = false;
                    x.isDirty = true;
                });
            
            public static ConditionDataWrapper<T, TConditionType> Get(T t, 
                Action<ICollection<TConditionType>> getConditionTypesFunc, 
                Func<T, bool> checkConditionFunc)
            {
                ConditionDataWrapper<T, TConditionType> result = s_Pool.Get();
                result.t = t;
                result.getConditionTypesFunc = getConditionTypesFunc;
                result.checkConditionFunc = checkConditionFunc;
                return result;
            }

            private ConditionDataWrapper()
            {
                callbacks = new HashSet<Action<T, bool>>();
            }
            void IDisposable.Dispose() => s_Pool.Release(this);
            
#if POOLED_EXCEPTION
            ~ConditionDataWrapper() => s_Pool.FinalizeDebug();
#endif

            private T t;
            private Action<ICollection<TConditionType>> getConditionTypesFunc;
            private Func<T, bool> checkConditionFunc;
            
            private bool isDirty = true;
            private bool status;
            private HashSet<Action<T, bool>> callbacks;
            
            public void GetTypes<TCollection>(TCollection eConditionTypes)
                where TCollection: ICollection<TConditionType>
            {
                getConditionTypesFunc.Invoke(eConditionTypes);
            }

            public void Subscribe(Action<T, bool> callback)
            {
                callbacks.Add(callback);
            }
            public void Unsubscribe(Action<T, bool> callback)
            {
                callbacks.Remove(callback);
            }
            public void Fire()
            {
                isDirty = true;
                if (callbacks.Count == 0) return;
                bool lastStatus = status;
                if (lastStatus == Status()) return;
                foreach (var callback in callbacks)
                {
                    callback.Invoke(t, status);
                }
            }
            public bool Status()
            {
                if (isDirty)
                {
                    isDirty = !isDirty;
                    status = checkConditionFunc.Invoke(t);
                }
                return status;
            }
            // 检空
            public bool CheckNull() => callbacks.Count == 0;
            
            public T UnWrapper() => t;
        }

        public abstract void GetConditionTypes(ICollection<TConditionType> types);
        public abstract bool CheckCondition(T t);
    }
}
