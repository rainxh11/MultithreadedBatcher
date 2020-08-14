using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace MultithreadedBatcher
{
	class Program
	{
		public static string currentDir = AppContext.BaseDirectory + @"\";
		public static string cmdLine = null;
		public static string exePath = null;
		public static bool silent = false;
		///----------------------------------------------------------------------------------////
		private static void DrawXH()
		{
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine("\r\n                              .    .     \r\n                              Di   Dt    \r\n                   :KW,      LE#i  E#i   \r\n                    ,#W:   ,KGE#t  E#t   \r\n                     ;#W. jWi E#t  E#t   \r\n                      i#KED.  E########f.\r\n                       L#W.   E#j..K#j...\r\n                     .GKj#K.  E#t  E#t   \r\n                    iWf  i#K. E#t  E#t   \r\n                   LK:    t#E f#t  f#t   \r\n                   i       tDj ii   ii                          \r\n                ");
			Console.ResetColor();
		}
		///------------------------------------------------------------------------///
		private static void ShowUsage()
		{
			Console.WriteLine("\t"+ "mtxh.exe --run=<executable to run> --cmd=<command-line>" +
							"--foreach-extension=[extension1] --foreach-extension=[extension2]..." +
							"<folder1> <folder2> <folder3> <folder4>..." + "\n \t \t --- OR --- \n" +
							"\t" + "mtxh.exe --run=<executable to run> --cmd=<command-line>" +
							"<file1> <file2> <file3> <file4>..." + "\n\n" +
							"OPTIONS: \n" +
							 @"--run=<arg>" +		"\t" + @"1. Executable absolute path: <DRIVE>:\<PATH>" + "\n" + 
													"\t" +"\t" + @"2. Executable (current directory||global) path: .\<EXE>" + "\n" +
							 @"--threads=<int>" +	"\t" + @"Specify the number of threads to be executed simultaneously. (DEFAULT: CPU Thread Count)" + "\n" +
							 @"--removefiles" +		 "\t" + @"Remove processed files after job finished." + "\n" +
							 @"--silent" +			"\t" + @"Silent execution no text output" + "\n" 






											  );
			Console.ReadKey();
		}
		///----------------------------------------------------------------------------------////
		///----------------------------------------------------------------------------------////
		private static void Executer(string currentDir, string exePath, string commandLine, FileInfo filePathInfo)
		{
			FileInfo exeFileInfo;
			bool exeIsGlobalPath = false;
			///----------------------------------------< PARSE Command-line Variables >----------------------------------------////
			string parsedCmdLine = commandLine;
			if (parsedCmdLine.Contains("@@File.FullName"))
			{
				parsedCmdLine = parsedCmdLine.Replace("@@File.FullName", filePathInfo.FullName);
			}
			if (parsedCmdLine.Contains("@@File.Name"))
			{
				parsedCmdLine = parsedCmdLine.Replace("@@File.Name", filePathInfo.Name.Replace(filePathInfo.Extension, "") );
			}
			if (parsedCmdLine.Contains("@@File.Root"))
			{
				parsedCmdLine = parsedCmdLine.Replace("@@File.Root", filePathInfo.DirectoryName + @"\");
			}
			if (parsedCmdLine.Contains("!"))
			{
				parsedCmdLine = parsedCmdLine.Replace("!", @"""");
			}
			if (parsedCmdLine.Contains("@@File.Time"))
			{
				DateTime fileDateTime = File.GetLastAccessTime(filePathInfo.FullName);
				parsedCmdLine = parsedCmdLine.Replace("@@File.Time", fileDateTime.ToString("_yyyy-MM-dd_HH-mm"));
			}
			///-------------------------------------------< Parse Executable Path >---------------------------------------////
			if (!exePath.Contains(@":\") && File.Exists(currentDir + exePath))
			{
				if (exePath.Contains(@".\"))
				{
					exePath = exePath.Replace(@".\", "");
				}
				 exeFileInfo = new FileInfo(currentDir + exePath);
			}
			if (!exePath.Contains(@"\") && !exePath.Contains(@".exe"))
			{
				exeFileInfo = new FileInfo(exePath);
				exeIsGlobalPath = true;
			}
			else
			{
				 exeFileInfo = new FileInfo(exePath);
			}
			///----------------------------------------------------------------------------------////
			Process process = new Process();
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = !silent;
			process.StartInfo.CreateNoWindow = false;
			if (exeIsGlobalPath)
			{
				process.StartInfo.FileName = exePath;
			}
			else
			{
				process.StartInfo.FileName = exeFileInfo.FullName;
			}
			process.StartInfo.Arguments = parsedCmdLine;
			try
			{
				//Console.WriteLine(parsedCmdLine);
				process.Start();
				process.WaitForExit();
			}
			catch
			{
			}
		}
		///----------------------------------------------------------------------------------////
		private static void BatchExecuter(List<DirectoryInfo> foldersList, List<string> extenstionsList, int threads, bool removeFiles)
		{
			foreach (string ext in extenstionsList)
			{
				foreach (DirectoryInfo dirInfo in foldersList)
				{
					Parallel.ForEach(dirInfo.GetFiles("*." + ext, SearchOption.AllDirectories), new ParallelOptions { MaxDegreeOfParallelism = threads }, (FileInfo file) =>
					{
						Console.ForegroundColor = ConsoleColor.Cyan;
						Console.WriteLine($"Processing {file.FullName} on thread {Thread.CurrentThread.ManagedThreadId}");
						Console.ForegroundColor = ConsoleColor.White;

						Executer(currentDir, exePath, cmdLine, file);
						try
						{
							if (removeFiles)
							{
								try
								{
									File.Delete(file.FullName);
								}
								catch { }
							}
						}
						catch (Exception e)
						{
							Console.BackgroundColor = ConsoleColor.Red;
							Console.WriteLine(e.Message);
							Console.ResetColor();
						}
					});
				}
			}
		}
		///----------------------------------------------------------------------------------////
		private static void BatchExecuter(List<FileInfo> filesList, int threads, bool removeFiles)
		{
			Parallel.ForEach(filesList, new ParallelOptions { MaxDegreeOfParallelism = threads }, (FileInfo file) =>
			{
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine($"Processing {file.FullName} on thread {Thread.CurrentThread.ManagedThreadId}");
				Console.ForegroundColor = ConsoleColor.White;

				Executer(currentDir, exePath, cmdLine, file);
				try
				{
					if (removeFiles)
					{
						try
						{
							File.Delete(file.FullName);
						}
						catch { }		
					}
				}
				catch (Exception e)
				{
					Console.BackgroundColor = ConsoleColor.Red;
					Console.WriteLine(e.Message);
					Console.ResetColor();
				}
			});
		}
		///----------------------------------------------------------------------------------////
		static void Main(string[] args)
		{
			Console.Title = "XH Multi-threaded Batcher";
			bool removeFiles = false;
			bool processFolders = false;
			int threads = Environment.ProcessorCount;
			List<string> extensions = new List<string>();
			List<DirectoryInfo> folders = new List<DirectoryInfo>();
			List<FileInfo> files = new List<FileInfo>();
			///------------------------------------< Command-line arguments Parser: >-------------------------------------////
			if (args.Length == 0)
			{
				DrawXH();
				ShowUsage();
			}
			else
			{
				foreach (string arg in args)
				{
					if (arg.Contains("--run="))
					{
						exePath = arg.Replace("--run=", "");
					}
					if (arg.Contains("--cmd="))
					{
						cmdLine = arg.Replace("--cmd=", "");
					}
					if (arg.Contains("--removefiles"))
					{
						removeFiles = true;
					}
					if (arg.Contains("--silent"))
					{
						silent = true;
					}
					if (arg.Contains("--threads="))
					{
						string threadsString = arg.Replace("--threads=", "");
						if (UInt32.TryParse(threadsString, out uint t))
						{
							threads = (int)t;
						}
					}
					if (arg.Contains("--foreach-extension="))
					{
						string extString = arg.Replace("--foreach-extension=", "");
						extensions.Add(extString.ToLower());
						extensions.Add(extString.ToUpper());
						processFolders = true;
					}
					if(!arg.Contains("--") && !arg.Contains("="))
					{
						if (processFolders)
						{
							DirectoryInfo dirInfo = new DirectoryInfo(arg);
							folders.Add(dirInfo);
						}
						else
						{
							FileInfo fileInfo = new FileInfo(arg);
							files.Add(fileInfo);
						}
					}
				}
			}
			///----------------------------------------------------------------------------------////
			if (processFolders)
			{
				BatchExecuter(folders, extensions, threads, removeFiles);
			}
			else
			{
				BatchExecuter(files, threads, removeFiles);
			}
			///----------------------------------------------------------------------------------////
		}
	}
}
