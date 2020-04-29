# AutomatedMeasurements
Here are shown the measurements for SQL procedure and SQL bulk copy. Since Tanneryd ef6 bulk operations failed in comparison to SQL bulk copy and SQL procedure approach, I didn't measure its performances.

Measurements were taken for 1 thread, 5 threads and 10 threads.
Each thread would insert 1 000 000 records in bulks of 4000 at a time.
This operaiton ran for 1000 iterations.
For ef6 bulk operations, I ran only 10 iterations
Measurements are below:
- 1 thread
  - SQL procedure - average 4.59 seconds, max time 9.44 seconds
  - SQL BulkCopy - average 4.87 seconds, max time 10.61
  - ef6 bulk operations - average 12.64 seconds, max time 12.97 seconds
- 5 thread
  - SQL procedure - average 13.15 seconds, max time 56.61
  - SQL BulkCopy - average 15.11 seconds, max time 24.57
  - ef6 bulk operations - average 43.18 seconds, max time 46.73 seconds
- 10 thread
  - SQL procedure - average 30.21 seconds, max time 133.11 seconds
  - SQL BulkCopy - average 30.87 seconds, max time 41.56 seconds
  - ef6 bulk operations - average 113.21 seconds, max time 117.42 seconds

Around 6 percent of the entries deviated more than 20 percent from the average on SQL procedure, but those deviations were massive, in range of 125 seconds or so. More than twenty percent from the average deviate around 6 percent SQL procedure calls on 10 thread parallel calls, and on SQL bulk copy, the number is only 0.85.

Due to the fact that bulk copy is more stable option than using SQL procedure, meaning that those extremes are much closer to the average execution time, my conclussion here is that SQL bulk copy should be prefered among these three tools, SQL procedure, SQL bulk copy and Tanneryd ef6 bulk operaitons