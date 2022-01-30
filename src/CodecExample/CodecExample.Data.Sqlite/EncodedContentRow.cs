using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodecExample.Data.Sqlite
{
    /// <summary>
    /// A base class for use by dapper to read and write from the database.
    /// The structure of this class matches the columns in the database.
    /// </summary>
    internal class EncodedContentRow
    {

        /// <summary>
        /// An ID for easy query of the resource.
        /// </summary>
        /// <remarks>
        /// The system only assumes a string value. It could be 
        /// any value.
        /// </remarks>
        public string ID { get; set; }

        /// <summary>
        /// The mediatype used to encode the resource.
        /// </summary>
        /// <remarks>
        /// The database may evolve over time. A record could be
        /// encoded with an older version of a codec. For this
        /// reason, the system can not assume the format.
        /// </remarks>
        public string MediaType { get; set; }

        /// <summary>
        /// The binary bytes of the encoded resource.
        /// </summary>
        /// <remarks>
        /// Using byte[] here so that the system can persist binary representations
        /// like Avro or Protobuf
        /// </remarks>
        public byte[] Content { get; set; }

    }
}
