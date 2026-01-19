using UnityEngine;
using UnityEngine.UI;

namespace ChatModule
{
    [RequireComponent(typeof(RectTransform))]
    public class ChatMessageEntity : ChatMessageEntityBase
    {
        [SerializeField] private Text tmpMessage;
        
        private RectTransform _rectTransform;
        public override RectTransform RectTransform => _rectTransform ??= GetComponent<RectTransform>();

        public override float Init(ChatMessage chatMessage)
        {
            tmpMessage.text = chatMessage.GetMessage();
            RectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, GetHeight());
            return GetHeight();
        }

        public override float GetHeight()
        {
            return tmpMessage.preferredHeight;
        }
    }
}