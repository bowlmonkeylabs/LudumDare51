namespace EmailInbox
{
    public struct EmailInstancePayload
    {
        public int InstanceId;
        public EmailItem EmailData;
    }

    public struct RemoveEmailInstancePayload
    {
        public EmailInstancePayload EmailInstance;
        public bool CountAsFinishedItem;
    }
}