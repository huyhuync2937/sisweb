namespace SisSo.Pages.Input.HDBanle
{
    public class VmgZalo
    {
        public List<Message> messages { get; set; }
        public string account { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string referentId { get; set; }
        public string from { get; set; }
        public int type { get; set; }
        public int serviceType { get; set; }
    }
    public class Message
    {
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string referentId { get; set; }
        public string to { get; set; }
        public string requestId { get; set; }
        public string scheduled { get; set; }
        public string templateId { get; set; }
        public int useUnicode { get; set; }
        public TemplateData templateData { get; set; }
    }

    public class Root
    {
        public List<Message> messages { get; set; }
    }

    public class TemplateData
    {
        public string USER_ID { get; set; }
        public string customer_name { get; set; }
        public string ID_name { get; set; }
    }
}
