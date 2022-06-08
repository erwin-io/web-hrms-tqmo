using System.Collections.Generic;

namespace HRMS.Domain.BindingModel
{
    public class FileBindingModel
    {
        public bool IsDefault { get; set; }
        public bool IsFromStorage { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public byte[] FileContent { get; set; }
        public long FileSize { get; set; }
        public string FileFromBase64String { get; set; }
    }

    public class UpdateFileBindingModel : FileBindingModel
    {
        public string FileId { get; set; }
        public bool HasChange { get; set; }
    }
}
