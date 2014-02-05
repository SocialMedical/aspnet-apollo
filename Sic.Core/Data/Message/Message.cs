using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Data
{    
    public enum MessageType 
    {
        Information,
        Error,
        Warning
    }

    public class Message
    {
        public Message()
            :this("")
        {
        }

        public Message(string textMessage)
            :this(textMessage, MessageType.Information)
        {            
        }

        public Message(string textMessage,MessageType messageType)
        {
            this.TextMessage = textMessage;
            this.MessageType = messageType;
        }

        public string TextMessage { get; set; }

        public MessageType MessageType { get; set; }
    }

    public class MessageCollection
    {
        private List<Message> messages;
        public List<Message> Messages
        {
            get
            {
                if (messages == null)
                    messages = new List<Message>();
                return messages;
            }
        }

        public string ToString(bool htmlFormat)
        {
            StringBuilder messageString = new StringBuilder();
            messageString.Append("<ul class=\"sicMessageCollection\">");
            foreach (Message message in Messages)
            {
                messageString.AppendFormat("<li class=\"sicMessage sicMessageType\">{1}</li>",
                    (short)message.MessageType, message.TextMessage);
            }
            messageString.Append("</ul>");

            return messageString.ToString();
        }
    }
}
