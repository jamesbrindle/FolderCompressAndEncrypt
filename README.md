# Folder Compress & Encrypt Utility

## Description

This is a command line utility that given an input directory will iterate through all files recursively (optional) and compress, encrypt and password protect (optional) into individual 7z files.

* The folder structure of the source directory will be maintained.
* The output directory and filenames can also be encrypted so they are masked.
* Upon 'decompression', the folder structure will be replicated and the directory and file names will be 'unmasked' if they were encrypted.
* You can re-run on the same folder to be compressed with the same output. Any files that already exist in the output folder will be skipped (unless the -f or --overwrite flag is used), so on subsequent runs the operation will complete much faster.
* You can also have any remaining files that no longer have a source file be deleted (cleaned / synced). I.e if you delete the source file, this will be reflected in the output archive also being deleted.

## Scenario

Say you have a backup location for your files. This backup location could be a shared or just insecure location (for example, online cloud storage). You want a quick and easy way to backup your files to this location, that can be performed on a scheduled daily, weekly etc and you also don't want to have to maintain these backup files - You want that to be automated.

That's why this utility comes in handy, with the added bonus of reducing the resulting file size in many cases.

## Command Flags

| Option Set				| Description																																																																				|
| -------------------------	| ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|   -i, --input=VALUE		| Input folder to compress.																																																																	|
|   -o, --output=VALUE		| Output folder of archives. The folder structure of input folder will be maintained. Don't worry if the input folder contains the output folder; on subsequant passes the output folder will be skipped.																					|
|   -t, --tempdir=VALUE		| You can optionally set a directory for the tempory files the tool uses when compressing files.																																															|
|   -l, --log\[=VALUE\]		| Enable creating a log and optionally set a log path. If you use the '-l' flag but don't set a path the log file will be placed in location: %LocalAppData%\\Temp\\FCE-Temp\\Logs																											|
|   -d, --decompress		| Decompress all the archives in a folder. If you password protected the archives you will need to provide this (-p=<your password>).																																						|
|   -r, --recursive			| If 'recursive' flag is used, then all files in all subdirectories will be included, otherwise only the top level files in the output folder will be included.																																|
|   -e, --encrypt			| Encrypt directory and filenames. Upon 'decompression' they will be restored.																																																				|
|   -p, --password=VALUE	| Password to open archive (wrap in quotes if it contains spaces or special characters).																																																	|
|   -f, --overwrite			| If the resulting file already exists it will be skipped. Use this option to force overwrite regardless.																																													|
|   -m, --mode=VALUE		| File compression mode to use. If no value, then 'normal' will be used. In 'low' and 'normal' compression modes, the 'Deflate' algorithm will be used, in 'high' and 'ultra', the 'Lzma2' algorithm will be used. Higher compression takes longer.											|
|   -c, --clean				| Remove archives if the resulting files exist in the output folder but not the input folder (i.e. monitor source deletions).																																								|
|       --enable-long-paths | Requires elevated permissions (Run as Admin) - Sets the Windows registry to enable long path support as typically it's limited to 260 characters. If elevated process not detected it will be skipped. This flag can be used on its own or together with a compress /extract operation.	|
|   -v, --version			| Shows the version number.																																																																	|
|   -h, -?, --help			| Show help and available arguments.																																																														|

### Example Commands

#### Backuping Up

##### Most Basic (Top Folder Only)
```
fce -i "C:\Folder to Backup" -o "C:\Backup Location"
```

##### Most Basic (Recursive - Include All Subdirectories)
```
fce -i "C:\Folder to Backup" -o "C:\Backup Location" -r
```

##### Maximum Compression
```
fce -i "C:\Folder to Backup" -o "C:\Backup Location" -r -m ultra
```

##### Maximum Speed
```
fce -i "C:\Folder to Backup" -o C:\Backup Location -r -m none
```

##### Password Protect
```
fce -i "C:\Folder to Backup" -o "C:\Backup Location" -r -p "YOUR PASSWORD"
```

##### Encrypt (Mask) Filenames
```
fce -i "C:\Folder to Backup" -o "C:\Backup Location" -r -e
```

##### Password Protect and Encrypt Filenames
```
fce -i "C:\Folder to Backup" -o "C:\Backup Location" -r -p "YOUR PASSWORD" -e
```

##### You Want a File Log of the Operation
```
fce -i "C:\Folder to Backup" -o "C:\Backup Location" -r -l
```

##### You Want a Specific Place for Your Log File
```
fce -i "C:\Folder to Backup" -o "C:\Backup Location" -r -l"C:\Logs"
```

##### Monitor Deletions (Clean Output Files Without a Source File)
```
fce -i "C:\Folder to Backup" -o "C:\Backup Location" -c
```

##### Enable Windows Long Path Support in the Registry
```
fce --enable-long-paths
```

#### Advised Command For Encrypted, Password Protected, Recursively Scanned Input Folder, Using Medium Compression and Deletions Monitored, With Decent Compression
```
fce -i "C:\Folder to Backup" -o "C:\Backup Location" -m high -r -e -c -p "YOUR PASSWORD"
```

**Note: You can also bundle commands:**

In the above command we have '-r -e -c' boolean flags - which can all be bundle like:
```
fce -i "C:\Folder to Backup" -o "C:\Backup Location" -m high -rec -p "YOUR PASSWORD"
```

## Dependencies 

### 7z

This utility uses the free (but awesome) compression program '7Zip' - Available here: https://www.7-zip.org/download.html which should ideally be installed.

There is also an MSI installer for 7zip in the '7z - **Dependency to Install** folder within this repository.

This utility uses the 64-bit 7z.dll assembly, for the 7z interop to 7z.

### .NET 4.8

Anything equal to or greater than .NET 4.8

## Building

Should be straighforward to build, basic console app, self contained, no nuget packages requires.

**NOTE:** I'd advise you to build in x64 configuration only. This application will select which 7z.dll to use at runtime, but there isn't an 'any-cpu' runtime for it,
so I'd use x64 bit for performance and better memory, needed for large file compression.




