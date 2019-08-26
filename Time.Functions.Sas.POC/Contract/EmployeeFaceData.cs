using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Paycor.Time.Functions.Contract.MobileKiosk
{
    public class EmployeeFaceData
    {
        public int ClientId { get; set; }
        public long EmployeeUid { get; set; }
        public string BlobImageUrl { get; set; }
        public string PersonGroupId { get; set; }
        public Guid PersonId { get; set; }
        public Guid PersistedFaceId { get; set; }

    }
}
