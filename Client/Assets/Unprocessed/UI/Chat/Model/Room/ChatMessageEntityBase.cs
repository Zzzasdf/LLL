using System.Collections.Generic;
using UnityEngine;

namespace ChatModule
{
    /// 消息实体
    public abstract class ChatMessageEntityBase : MonoBehaviour
    {
        /// 获取 RectTransform 组件
        public abstract RectTransform RectTransform { get; }

        /// 输入数据，返回高度
        public abstract float Init(ChatMessage chatMessage);

        /// 获取高度
        public abstract float GetHeight();
    }

    public class ChatMessageEntityPool
    {
        private ChatMessageEntityBase templateEntity; // 模板
        private RectTransform rectParent; // 父节点

        private LinkedList<ChatMessageEntityBase> inactivePool;
        private LinkedList<ChatMessageEntityBase> activePool;

        public ChatMessageEntityPool(ChatMessageEntityBase templateEntity, RectTransform rectParent)
        {
            templateEntity.gameObject.SetActive(false);
            this.templateEntity = templateEntity;
            this.rectParent = rectParent;
            inactivePool = new LinkedList<ChatMessageEntityBase>();
            activePool = new LinkedList<ChatMessageEntityBase>();
        }

        public void ReleaseAll()
        {
            while (activePool.First != null)
            {
                LinkedListNode<ChatMessageEntityBase> entity = activePool.First;
                entity.Value.gameObject.SetActive(false);
                activePool.RemoveFirst();
                inactivePool.AddLast(entity);
            }
        }

        public LinkedListNode<ChatMessageEntityBase> Get()
        {
            LinkedListNode<ChatMessageEntityBase> result;
            if (inactivePool.Count == 0)
            {
                ChatMessageEntityBase entity = Object.Instantiate(templateEntity, rectParent);
                // 将 锚点 Y 定在 1，简化计算
                entity.RectTransform.anchorMin = new Vector2(0.5f, 1f);
                entity.RectTransform.anchorMax = new Vector2(0.5f, 1f);
                entity.RectTransform.pivot = new Vector2(0.5f, 1f);
                result = new LinkedListNode<ChatMessageEntityBase>(entity);
            }
            else
            {
                result = inactivePool.First;
                inactivePool.RemoveFirst();
            }

            result.Value.gameObject.SetActive(true);
            return result;
        }

        public void AddFirstByActivePool(LinkedListNode<ChatMessageEntityBase> node)
        {
            activePool.AddFirst(node);
        }

        public void AddLastByActivePool(LinkedListNode<ChatMessageEntityBase> node)
        {
            activePool.AddLast(node);
        }

        public LinkedListNode<ChatMessageEntityBase> GetFirstActivePool()
        {
            return activePool.First;
        }

        public LinkedListNode<ChatMessageEntityBase> GetLastActivePool()
        {
            return activePool.Last;
        }

        public LinkedListNode<ChatMessageEntityBase> RemoveLastByActivePool()
        {
            LinkedListNode<ChatMessageEntityBase> result = activePool.Last;
            activePool.RemoveLast();
            return result;
        }

        public LinkedListNode<ChatMessageEntityBase> RemoveFirstByActivePool()
        {
            LinkedListNode<ChatMessageEntityBase> result = activePool.First;
            activePool.RemoveFirst();
            return result;
        }

        public void SetHeightOffsetByActivePool(float heightOffset)
        {
            foreach (var entity in activePool)
            {
                entity.RectTransform.anchoredPosition += new Vector2(0, heightOffset);
            }
        }
    }
}