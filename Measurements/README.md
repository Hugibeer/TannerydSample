# Measurements
Here are shown the measurements for SQL procedure and bulk operation variant of bulk inserting records.
It is obvious that Tannyryd is much slower than plain old SQL insert procedure, and especially with multiple threads accessing the database.
Measurements were taken for 1 thread, 5 threads and 10 threads.
Each thread would insert 1 000 000 records in bulks of 4000 at a time.
Measurements are below:
- 1 thread
  - SQL procedure - around 6 seconds
  - Tannyryd bulk library - around 11 seconds
  - SQL BulkCopy - around 5 seconds
- 5 thread
  - SQL procedure - around 15-20 seconds
  - Tannyryd bulk library - around 76 seconds
  - SQL BulkCopy - around 28 seconds
- 10 thread
  - SQL procedure - around 34-50 seconds
  - Tannyryd bulk library - around 235 seconds
  - SQL BulkCopy - around 60 seconds
