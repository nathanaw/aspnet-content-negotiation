# Sqlite Data Repository

This repository demonstrates how to use codecs when saving data
to a database. In this case, the database is Sqlite, which is 
a relational database.

Other document databases (aka NoSql databases) have native 
abilities to persist a resource as json, bson, or another 
document format. However, even these database options require
encoding and decoding the document, and could benefit from 
the codec patterns.

Sqlite is simple, in-process, and uses a single file which
can be created on demand. It serves the demonstration purposes
well and highlights important concepts

## Concepts

### Encode / Decode Database Records

In nearly all database persistence scenarios, the resource instance
must be "mapped" to the needs of the database. For a relational
database, the fields are often mapped to columsn. For document 
databases, the instance is often serialized into a JSON or BSON
format. Both of these are examples of converting the resource
to a new structure. This is also what the Transcoder does, and 
why the Transcoder and codecs can be useful in the database
repository layers.


### Persist the MediaType in the Database

Systems evolve over time. Fields are added, changed, or removed.

For a relational database, the changes span the code and the 
database table structures.

For a document database, the changes can be less costly, but 
the data in the database will still evolve.

This example persists the selected MediaType next to the encoded
representation in the database row. This is important for 
evolving the system forward. As long as the codebase retains the 
older codecs, it will be able to read the database data even
if it is still in an older format.

Typically, the codebase will add a new codec version and then 
use that when saving new records. But existing records can 
remain in the database as is. No migration is strictly required,
but a migration can be done on existing data if needed.

### Promoted Fields

The rows only need a few core fields:
- ID
- MediaType
- Content

For queries that refer to fields other than the ID, the row
includes a copy of that field so that SQL queries can filter
rows efficiently. The value is set by the repository when
saving the row, and should always be up to date as long as
all database updates go through the service.

### Coarse Grained Repository Design

The data repository reads and writes the whole resource.

