# Introduction 
This tool allow me to move from the following azure configuration:

- Collection 1 (small, 400 RU)
- Collection 2 (small, 400 RU)
- Collection 3 (huge, 1000 RU & 10 Go)

To the following one

- Collection 4 (small, 400 RU)
- Azure blob storage

# How?

Collection 3 is containing html web page, so this collection size is growing very fast and consumption quickly get out of control.

We choose to externalise the web page content to an azure blob storage, cheeper, and to compress content instead of storing it as we received.

# The software

By default, it does not to anything. It is running as a simulation.
The goal is then to get some stats about the current documents, like size and count.

It allow us to get a good idea of the initial size, the compressed one, the compression rate and the required time to do the migration.

# How to use it

They are many options.
- -p or --process: this option should be used if you want to apply the migration. Be carrefull if used with -d!
- -b or --useBlobStorage: this option move the html content to azure blob storage
- -c or --compress: enable compression on html content
- -d or --drop: remove the source collection once migration is done
- -v or --verbose: set output as verbose
- --cleanupDist: cleanup the destination collection
