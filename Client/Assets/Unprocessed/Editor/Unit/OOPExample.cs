using NUnit.Framework;

[TestFixture]
public class OOPExample
{
    public class View
    {
        private Model model = new Model();
        private void EventHandler(DataWhole data)
        {
            // Condition Update
            model.Update(data);
        }
        private void EventHandler(DataPart dataPart)
        {
            // Condition Update
            model.Update(dataPart);
        }
        private class Model
        {
            private DataWrapper dataWrapper = new DataWrapper();
            public void Update(DataWhole data)
            {
                dataWrapper.Update(data);
            }
            public void Update(DataPart dataPart)
            {
                dataWrapper.Update(dataPart);
            }
            // ... Custom Func Function
        }
    }
    public class Message
    {
        private void ReceiveWholeMessage(DataWhole data)
        {
            // Event.Subscribe(A, data);
        }
        private void ReceivePartMessage(DataPart dataPart)
        {
            // Event.Subscribe(B, dataPart);
        }
    }
    public class DataWrapper // or DataWhole Static Extensions or partial
    {
        // 包装器：优点：限制外部部分修改权限，相对安全
        //          缺点：多占点内存，直接调用字段时也要加方法
        // 静态扩展：优点：调用方便
        //          缺点：外部有直接修改数据的权限
        // 分部类方法：优点：调用方便，相对于包装器
        //          缺点：外部调用方法时要判空
        private DataWhole DataWhole;
        public void Update(DataWhole data)
        {
            DataWhole = data;
        }
        public void Update(DataPart dataPart)
        {
            if (DataWhole == null) return;
            DataWhole.a = dataPart.a;
        }
        // ... Other Common Function
    }
    public class DataWhole
    {
        public int a, b;
    }
    public class DataPart
    {
        public int a;
    }
}
