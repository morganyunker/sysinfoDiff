<!-- title: SysinfoDiff -->
# SysinfoDiff

`sysinfodiff` is a utility to analyze mismatched patches installed on ACI Postilion environments.

## What is this and Why was this created?

ACI's Postilion system allows admins to create what is called a `sysinfo` report which includes, among other things, a list of all postilion software installed on a given environment.

This report includes a list of components, and their relevant patches.

Working for multiple banks with Postilion software installed, keeping track of which patches installed in which environments is tricky and error-prone.

I created this utility `sysinfodiff` which:
* Scans the `sysinfos` in multiple environments (dev/UAT/prod) and displays the installed components
* Displays the installed patches for each component
* Lists the missing patches for each environment based on patches installed on the other environments.

## Supported OSs
Currently only supports Windows server (tested on Server 2016+)

Next version will be re(written) in C++ for native support in Linux based Postilion systems.

## Usage

First compile with Visual Studio (tested on 2022) with .NET 4.7.2


```Powershell
C:\sysinfodiff.exe \path_to_sysinfos
```

>Example
```Powershell
C:\sysinfodiff.exe C:\Development\SysinfoDiff\Sysinfos
```
   Where `\path_to_sysinfos` is where the sysinfo folders are stored.

```powershell
├── \
│   ├── DevSysinfo
│   │   ├── SystemInfo.txt
│   ├── UATSysinfo
│   │   ├── SystemInfo.txt
│   ├── ProdSysinfo
│   │   ├── SystemInfo.txt
```


⚠️ Postilion creates many files in the sysinfo folder, but while you can copy the entire sysinfo folder, we are only looking for the `sysinfo.txt` file


## Output

The output file `result.html` will be created in the same directory as the `\path_to_sysinfos`.


## Note
If you find this utility useful, let me know :)
