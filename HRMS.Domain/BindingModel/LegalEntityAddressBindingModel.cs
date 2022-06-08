using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.BindingModel
{
    public class LegalEntityAddressBindingModel
    {
        public string Address { get; set; }
    }
    public class CreateLegalEntityAddressBindingModel : LegalEntityAddressBindingModel
    {
        public string LegalEntityId { get; set; }
    }
    public class UpdateLegalEntityAddressBindingModel : LegalEntityAddressBindingModel
    {
        public string LegalEntityAddressId { get; set; }
    }
}
