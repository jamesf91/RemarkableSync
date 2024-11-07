namespace RemarkableSync.document
{
    public class DocumentMetadata
    {
        public string visibleName { get; set; }
        public string type { get; set; }
        public string parent { get; set; }
        public string lastModified { get; set; }
        public string lastOpened { get; set; }
        public int lastOpenedPage { get; set; }
        public int version { get; set; }
        public bool pinned { get; set; }
        public bool synced { get; set; }
        public bool modified { get; set; }
        public bool deleted { get; set; }
        public bool metadatamodified { get; set; }
    }
}



