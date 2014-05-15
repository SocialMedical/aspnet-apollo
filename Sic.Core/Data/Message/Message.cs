using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Data
{
    public enum MessageType
    {
        Information = 0,
        Error = 1,
        Warning = 2,
        Confirmation = 3,
        Success = 4
    }

    public class Message
    {
        public Message()
            : this("")
        {
        }

        public Message(string textMessage)
            : this(textMessage, MessageType.Information, null)
        {
        }

        public Message(string textMessage, MessageType messageType)
            : this(textMessage, messageType, null)
        {
        }

        public Message(string textMessage, MessageType messageType, string title)
        {
            this.Text = textMessage;
            this.MessageType = messageType;
            this.Title = title;
        }

        public string Text { get; set; }

        public string TextMessage
        {
            get
            {
                return Text;
            }
        }

        public MessageType MessageType { get; set; }

        public string Title { get; set; }
    }

    public class MessageCollection : List<Message>
    {
        //private List<Message> messages;
        //public List<Message> Messages
        //{
        //    get
        //    {
        //        if (messages == null)
        //            messages = new List<Message>();
        //        return messages;
        //    }
        //}

        public string ToString(bool htmlFormat)
        {
            StringBuilder messageString = new StringBuilder();
            messageString.Append("<ul class=\"sicMessageCollection\">");
            foreach (Message message in this)
            {
                messageString.AppendFormat("<li class=\"sicMessage sicMessageType\">{1}</li>",
                    (short)message.MessageType, message.Text);
            }
            messageString.Append("</ul>");

            return messageString.ToString();
        }
    }
}
