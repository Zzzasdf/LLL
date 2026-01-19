using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace ChatModule
{
    public class UIChatMessageLoopScrollRect : ScrollRect
    {
        [SerializeField] [Header("item 模板")] private ChatMessageEntityBase templateEntity;
        [SerializeField] [Header("item 间隔")] private float space;

        private ChatMessageEntityPool pool;
        private bool init;
        private float viewTotalHeight; // 显示的总高度

        private ChatRoom chatRoom;
        private LinkedListNode<ChatMessage> displayNodeTop; // 循环列表显示 最顶部 item 的数据节点
        private LinkedListNode<ChatMessage> displayNodeBottom; // 循环列表显示 最底部 item 的数据节点

        public void Init(ChatRoom chatRoom)
        {
            // 初始化
            {
                if (!init)
                {
                    init = !init;
                    RectTransform rectThis = GetComponent<RectTransform>();
                    viewTotalHeight = rectThis.sizeDelta.y;
                    // 将 锚点 Y 定在 1，简化计算
                    content.anchorMin = new Vector2(0.5f, 1f);
                    content.anchorMax = new Vector2(0.5f, 1f);
                    pool = new ChatMessageEntityPool(templateEntity, content);
                    onValueChanged.AddListener(OnValueChanged);
                }
            }
            // 订阅
            {
                // 取消旧订阅
                if (this.chatRoom != null)
                {
                    this.chatRoom.UnsubscribeAfterAddNewMessage(AddNewMessage);
                    this.chatRoom.UnsubscribeAfterReleaseMessage(ReleaseMessage);
                }

                this.chatRoom = chatRoom;
                // 新订阅
                this.chatRoom.SubscribeAfterAddNewMessage(AddNewMessage);
                this.chatRoom.SubscribeAfterReleaseMessage(ReleaseMessage);
            }
            // 重置界面
            {
                pool.ReleaseAll();
                // 从最后一条消息往上计算，返回从底部往上添加的列表 与 content 需要噶偶的
                bool isOverViewHeight = TryCalculateByBottomUntilOverOnePage(out List<LinkedListNode<ChatMessageEntityBase>> entityNodes, out float contentHeight);
                if (isOverViewHeight) // 当累加的 总高度 高于 视图 时，则以 底 为 0 显示
                {
                    content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);
                    content.anchoredPosition = new Vector2(content.anchoredPosition.x, contentHeight - viewTotalHeight);
                }
                else // 当累加的 总高度 低于 视图 时，说明所有消息 一页内 可显示，则以 顶 为 0 显示
                {
                    content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);
                    content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
                }

                float itemPosY = 0;
                for (int i = entityNodes.Count - 1; i >= 0; i--)
                {
                    LinkedListNode<ChatMessageEntityBase> node = entityNodes[i];
                    pool.AddLastByActivePool(node);
                    // 定位
                    node.Value.RectTransform.anchoredPosition = new Vector2(node.Value.RectTransform.anchoredPosition.x, -itemPosY);
                    itemPosY += node.Value.GetHeight();
                    itemPosY += space;
                }
                ListPool<LinkedListNode<ChatMessageEntityBase>>.Release(entityNodes);
            }
            return;

            // 尝试从底部计算，直到超过一页
            bool TryCalculateByBottomUntilOverOnePage(out List<LinkedListNode<ChatMessageEntityBase>> entityNodes, out float contentHeight)
            {
                LinkedList<ChatMessage> chatMessages = chatRoom.AllMessage();
                displayNodeBottom = chatMessages.Last;
                displayNodeTop = displayNodeBottom;
                entityNodes = ListPool<LinkedListNode<ChatMessageEntityBase>>.Get();
                contentHeight = 0;
                if (displayNodeTop == null)
                {
                    return false;
                }

                bool isOverViewHeight = false; // 是否超出显示高度
                contentHeight -= space; // 先减去一个 间隔，循环中会被加上
                while (displayNodeTop != null)
                {
                    // 实例化出一个 实体 并输入数据，获取高度
                    LinkedListNode<ChatMessageEntityBase> node = pool.Get();
                    entityNodes.Add(node);
                    float itemHeight = node.Value.Init(displayNodeTop.Value);
                    contentHeight += itemHeight;
                    contentHeight += space;
                    if (contentHeight < viewTotalHeight)
                    {
                        // 小于当前显示高度，继续
                        displayNodeTop = displayNodeTop.Previous;
                    }
                    else if (contentHeight >= viewTotalHeight)
                    {
                        // 当等于或大于高度的时候，需要额外添加一次，然后 break
                        if (!isOverViewHeight)
                        {
                            displayNodeTop = displayNodeTop.Previous;
                            isOverViewHeight = !isOverViewHeight;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                return isOverViewHeight;
            }
        }

        private void OnValueChanged(Vector2 normalization)
        {
            // 显示范围
            float viewDisplayTopPosY = viewport.position.y;
            float viewDisplayBottomPosY = viewDisplayTopPosY - viewTotalHeight;
            // 判断顶部 item 是否被全部显示
            {
                LinkedListNode<ChatMessageEntityBase> nodeFirstActive = pool.GetFirstActivePool();
                // 顶部 item 全部被显示了
                if (nodeFirstActive.Value.RectTransform.position.y <= viewDisplayTopPosY)
                {
                    // 判断 是否有 Previous 数据
                    if (displayNodeTop is { Previous: not null })
                    {
                        displayNodeTop = displayNodeTop.Previous;
                        LinkedListNode<ChatMessageEntityBase> entityNode;
                        // 底部 item 不在显示区域，回收
                        LinkedListNode<ChatMessageEntityBase> nodeLastActive = pool.GetLastActivePool();
                        var sizeDelta = content.sizeDelta;
                        var originPivotOffsetY = content.pivot.y * sizeDelta.y;
                        if (nodeLastActive.Value.RectTransform.position.y < viewDisplayBottomPosY)
                        {
                            displayNodeBottom = displayNodeBottom.Previous;
                            entityNode = pool.RemoveLastByActivePool();
                            // 移除的底部高度
                            var bottomHeightOffset = space + entityNode.Value.GetHeight();
                            // 从改变原始锚点高度
                            originPivotOffsetY -= bottomHeightOffset;
                            // 改变 content 高度
                            sizeDelta.y -= bottomHeightOffset;
                        }
                        else
                        {
                            entityNode = pool.Get();
                        }
                        pool.AddFirstByActivePool(entityNode);
                        entityNode.Value.Init(displayNodeTop.Value);
                        // 改变 content 高度
                        var topHeightOffset = space + entityNode.Value.GetHeight();
                        sizeDelta.y += topHeightOffset;
                        content.sizeDelta = sizeDelta;
                        // 锚点归位
                        content.pivot = new Vector2(0.5f, originPivotOffsetY / sizeDelta.y);
                        // 设置 items 高度偏移量
                        pool.SetHeightOffsetByActivePool(-topHeightOffset);
                        // 定位 新 item
                        entityNode.Value.RectTransform.anchoredPosition = new Vector2(entityNode.Value.RectTransform.anchoredPosition.x, 0);
                    }
                }
            }
            // 判断底部 item 是否被全部显示
            {
                LinkedListNode<ChatMessageEntityBase> nodeLastActive = pool.GetLastActivePool();
                // 底部 item 全部被显示了
                if (nodeLastActive.Value.RectTransform.position.y - nodeLastActive.Value.RectTransform.sizeDelta.y >= viewDisplayBottomPosY)
                {
                    // 判断 是否有 Next 数据
                    if (displayNodeBottom is { Next: not null })
                    {
                        displayNodeBottom = displayNodeBottom.Next;
                        LinkedListNode<ChatMessageEntityBase> entityNode;
                        // 顶部 item 不在显示区域，回收
                        LinkedListNode<ChatMessageEntityBase> nodeFirstActive = pool.GetFirstActivePool();
                        var sizeDelta = content.sizeDelta;
                        var originPivotOffsetY = content.pivot.y * sizeDelta.y;
                        float topHeightOffset = 0;
                        if (nodeFirstActive.Value.RectTransform.position.y - nodeFirstActive.Value.RectTransform.sizeDelta.y > viewDisplayTopPosY)
                        {
                            displayNodeTop = displayNodeTop.Next;
                            entityNode = pool.RemoveFirstByActivePool();
                            // 移除的顶部高度
                            topHeightOffset = space + entityNode.Value.GetHeight();
                            // 改变 content 高度
                            sizeDelta.y -= topHeightOffset;
                        }
                        else
                        {
                            entityNode = pool.Get();
                        }
                        pool.AddLastByActivePool(entityNode);
                        entityNode.Value.Init(displayNodeBottom.Value);
                        // 改变 content 高度
                        var bottomHeightOffset = space + entityNode.Value.GetHeight();
                        sizeDelta.y += bottomHeightOffset;
                        content.sizeDelta = sizeDelta;
                        // 从改变原始锚点高度
                        originPivotOffsetY += bottomHeightOffset;
                        // 锚点归位
                        content.pivot = new Vector2(0.5f, originPivotOffsetY / sizeDelta.y);
                        // 设置 items 高度偏移量
                        pool.SetHeightOffsetByActivePool(topHeightOffset);
                        // 定位 新 item
                        entityNode.Value.RectTransform.anchoredPosition = new Vector2(entityNode.Value.RectTransform.anchoredPosition.x, -sizeDelta.y + space + entityNode.Value.GetHeight());
                    }
                }
            }
        }

        private void AddNewMessage(ChatMessage chatMessage)
        {
            // 显示范围
            float viewDisplayTopPosY = content.anchoredPosition.y;
            float viewDisplayBottomPosY = viewDisplayTopPosY + viewTotalHeight;
            LinkedListNode<ChatMessageEntityBase> nodeLastActive = pool.GetLastActivePool();
            // 判断是否在显示区域，若不在 无需处理
            if (nodeLastActive.Value.RectTransform.anchoredPosition.y >= viewDisplayBottomPosY)
            {
                return;
            }
            // 若在显示范围，则增加实体，增涨 content height
            // TODO ZZZ
        }

        private void ReleaseMessage()
        {
            // 显示范围
            float viewDisplayTopPosY = content.anchoredPosition.y;
            LinkedListNode<ChatMessageEntityBase> nodeFirstActive = pool.GetFirstActivePool();
            // 判断是否在显示区域，若不在 无需处理
            if (nodeFirstActive.Value.RectTransform.anchoredPosition.y + nodeFirstActive.Value.GetHeight() <= viewDisplayTopPosY)
            {
                return;
            }
            // 若在显示范围，则移除实体，缩减 content height
            // TODO ZZZ
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            // 取消订阅
            if (chatRoom != null)
            {
                chatRoom.UnsubscribeAfterAddNewMessage(AddNewMessage);
                chatRoom.UnsubscribeAfterReleaseMessage(ReleaseMessage);
            }
        }
    }
}
