using System;

namespace EmailInbox
{
    [Serializable]
    public class EmailInstancePayload
    {
        public int InstanceId;
        public EmailItem EmailData;
    }

    [Serializable]
    public class RemoveEmailInstancePayload
    {
        public EmailInstancePayload EmailInstance;
        public bool CountAsFinishedItem;
    }
}