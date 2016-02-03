using System;

namespace NBU_Mailer_2016
{
    struct Envelope
    {
        public string envelopePath { get; set; }
        public string envelopeName { get; set; }
        public string sendFromAddress { get; set; }
        public string recieveAddress { get; set; }
        public string fileName { get; set; }
        public int fileSize { get; set; }
        public DateTime fileModified { get; set; }
        public DateTime fileSent { get; set; }
        public DateTime fileDelivered { get; set; }
        public string fileLocation { get; set; }
        public string fileCategory { get; set; }
    }
}
