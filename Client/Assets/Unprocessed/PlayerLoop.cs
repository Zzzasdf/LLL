#region 定制化 Manager
public interface IEventManager : IManager { }
public interface ITimerManager : IManager, IUpdate { }
public interface IFixedFrequencyManager : IManager, IUpdate { }
public interface IChatManager : IManager { }
public interface IRedPointTreeManager : IManager { }

public interface IConverterCollectorSystemManager: IManager { }

public interface IManager: IInit, IReset, IDestroy { }
#endregion

#region 初始化 + 重置 + 销毁
public interface IInit
{
    void OnInit();
}
public interface IReset
{
    void OnReset();
}
public interface IDestroy
{
    void OnDestroy();
}
#endregion

#region 启用 + 禁用
public interface IEnable
{
    void OnEnable();
}
public interface IDisable
{
    void OnDisable();
}
#endregion

#region 订阅 + 取消
public interface ISubscribe
{
    void OnSubscribe();
}

public interface IUnsubscribe
{
    void OnUnsubscribe();
}
#endregion

#region 更新
public interface IUpdate
{
    void OnUpdate(float deltaTime);
}
public interface ILateUpdate
{
    void OnLateUpdate(float deltaTime);
}
public interface IFixedUpdate
{
    void OnFixedUpdate(float fixedTime);
}
#endregion