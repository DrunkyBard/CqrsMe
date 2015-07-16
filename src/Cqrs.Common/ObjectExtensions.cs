using System;
using System.Text;
using Newtonsoft.Json;

namespace Cqrs.Common
{
    public static class ObjectExtensions
    {
        public static Guid ToGuid(this object obj)
        {
            var jsonRepresentation = JsonConvert.SerializeObject(obj);
            var inputBytes = Encoding.Unicode.GetBytes(jsonRepresentation);
            var hashBytes = Md5HashProvider.ComputeHash(inputBytes);
            var hashGuid = new Guid(hashBytes);

            return hashGuid;
        }
    }
}
