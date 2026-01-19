using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace EditorModule
{
    public partial class WindowCollectorUtility
    {
        private Dictionary<string, Action> _btnList;
        private Dictionary<string, Action> BtnList => _btnList ??= new Dictionary<string, Action>
        {
            ["打开持久化目录"] = PersistentDataPathOpen,

        };

        private void PersistentDataPathOpen()
        {
            Process.Start(Application.persistentDataPath);
        }
    }
}