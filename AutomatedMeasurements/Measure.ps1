$exe ="..\TannerydSample\ConsoleApp1\bin\Debug\ConsoleApp1.exe"
$numOfExecutions = 900

for ($i = 0; $i -lt $numOfExecutions; $i++) {
    & $exe 1 proc >> proc_1.txt
    & $exe 5 proc >> proc_5.txt
    & $exe 10 proc >> proc_10.txt

    & $exe 1 bulkcopy >> bulkcopy_1.txt
    & $exe 5 bulkcopy >> bulkcopy_5.txt
    & $exe 10 bulkcopy >> bulkcopy_10.txt
}

& $exe analyze proc_1.txt >> analysis_proc1.txt
& $exe analyze proc_5.txt >> analysis_proc5.txt
& $exe analyze proc_10.txt >> analysis_proc10.txt

& $exe analyze bulkcopy_1.txt >> analysis_bulkcopy1.txt
& $exe analyze bulkcopy_5.txt >> analysis_bulkcopy5.txt
& $exe analyze bulkcopy_10.txt >> analysis_bulkcopy10.txt
