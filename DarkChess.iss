; -- Example1.iess --
; Demonstrates copying 3 files and creating an icon.

; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

[Setup]
AppName=Basic Blind Chess
AppVersion=0.8.0
DefaultDirName={pf}\Basic Blind Chess
DefaultGroupName=Basic Blind Chess
UninstallDisplayIcon={app}\darkchess.exe
Compression=lzma2
SolidCompression=yes
OutputDir=D:\darkchess

[Files]
Source: "D:\darkchess\darkchess_080\*.*"; DestDir: "{app}"
Source: "D:\darkchess\darkchess_080\readme.txt"; DestDir: "{app}"; Flags: isreadme
Source: "D:\darkchess\darkchess_080\Image\*.*"; DestDir: "{app}\Image"; Flags: ignoreversion recursesubdirs
Source: "D:\darkchess\darkchess_080\Sound\*.*"; DestDir: "{app}\Sound"; Flags: ignoreversion recursesubdirs  

[Icons]
Name: "{group}\Basic Blind Chess"; Filename: "{app}\darkchess.exe"; IconFilename: "{app}\Image\darkchess_default.ico"
Name: "{group}\Software Requirements Specification"; Filename: "{app}\Software Requirements Specification.pdf";
Name: "{group}\Uninstall Basic Blind Chess"; Filename: "{uninstallexe}";
