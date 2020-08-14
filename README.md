# XH Multithreaded Batcher

## USAGE:

        mtxh.exe --run=<executable to run> --cmd=<command-line>--foreach-extension=[extension1] --foreach-extension=[extension2]...<folder1> <folder2> <folder3> <folder4>...
                 --- OR ---
        mtxh.exe --run=<executable to run> --cmd=<command-line><file1> <file2> <file3> <file4>...
	OPTIONS:
	--run=<arg>                     1. Executable absolute path: <DRIVE>:\<PATH>
					2. Executable (current directory||global) path: .\<EXE>
	--threads=<int>                 Specify the number of threads to be executed simultaneously. (DEFAULT: CPU Thread Count)
	--removefiles                   Remove processed files after job finished.
	--silent                        Silent execution no text output
	--foreach-extension=[extension] Specify extension which; files filtered by, can be passed several times for each additional extension filter.
	--cmd="<command-line>"          Command-line to be passed to executable, with wildcard support.

	Wildcards & Command-Line syntax:
					 !<string>!             : Any string between double (!), will be passed as a string between quotation marks.
					 @@File.FullName        :  Curennt processed file full path name.
					 @@File.Root            :  Curennt processed file parent directory path name.
					 @@File.Name            :  Curennt processed file file-name only.
					 @@File.Time            :  Curennt processed file last access time, in format: '_yyyy-MM-dd_HH-mm'

### AUTHOR: Ahmed Chakhoum
