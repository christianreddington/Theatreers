namespace Theatreers.Show.Models
{
    class EventGridSchema
    {
        public string topic { get; set; }
        public string subject { get; set; }
        public string eventType { get; set; }
        public string eventTime { get; set; }
        public string id { get; set; }
        public EventGridSchemaDataObject data { get; set; }
        public string dataVersion { get; set; }
        public string metadataVersion { get; set; }
    }

    class EventGridSchemaDataObject
    {

        public string api { get; set; }
        public string clientRequestId { get; set; }
        public string requestId { get; set; }
        public string eTag { get; set; }
        public string contentType { get; set; }
        public string contentLength { get; set; }
        public string blobType { get; set; }
        public string url { get; set; }
        public string sequencer { get; set; }
        public EventGridSchemaDataStorageDiagnosticsObject storageDiagnostics { get; set; }
    }

    class EventGridSchemaDataStorageDiagnosticsObject
    {

        public string batchId { get; set; }
    }
}
