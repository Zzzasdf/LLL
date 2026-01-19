using System.Collections.Generic;
using TMPro;

namespace ChatModule
{
    /// 内容编辑贮藏器
    public class EditContentStorage: IChatRoomUnit
    {
        private string editContent;
        private Dictionary<string, string> maps;
        private TMP_InputField tmpInputField;
        
        public void Clear()
        {
            editContent = string.Empty;
            maps?.Clear();
            if(tmpInputField != null) tmpInputField.SetTextWithoutNotify(editContent);
        }
        
        /// 传入组件，内部控制组件监听
        public void AddTmpInputField(TMP_InputField tmpInputField)
        {
            this.tmpInputField = tmpInputField;
            // 移除 inputField 上所有需要用到的监听
            tmpInputField.onEndEdit.RemoveAllListeners();
            // 重新绑定监听
            tmpInputField.onEndEdit.AddListener(OnEndEdit);
            // 赋值上编辑中的内容
            tmpInputField.SetTextWithoutNotify(editContent);
        }
        private void OnEndEdit(string input)
        {
            editContent = input;
        }

        /// 添加需要映射的内容
        public void AddMapContent(string source, string target)
        {
            editContent = string.Concat(editContent, source);
            maps ??= new Dictionary<string, string>();
            if (!maps.ContainsKey(source)) return;
            maps.Add(source, target);
            if(tmpInputField != null) tmpInputField.SetTextWithoutNotify(editContent);
        }
        /// 获取转换的文本
        public string ConvertMessage()
        {
            string result = editContent;
            if (maps != null)
            {
                foreach (var pair in maps)
                {
                    result = result.Replace(pair.Key, pair.Value);
                }
            }
            // TODO ZZZ Other Operate
            return result;
        }
    }
}