; -- Example1.iess --
; Demonstrates copying 3 files and creating an icon.

; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

[Setup]
AppName=Basic Blind Chess
AppVersion=0.7.1
DefaultDirName={pf}\Basic Blind Chess
DefaultGroupName=Basic Blind Chess
UninstallDisplayIcon={app}\darkchess.exe
Compression=lzma2
SolidCompression=yes
OutputDir=E:\darkchess

[Files]
Source: "E:\darkchess\darkchess_071\darkchess.exe"; DestDir: "{app}"
Source: "E:\darkchess\darkchess_071\wqy-zenhei.ttf"; DestDir: "{app}"
Source: "E:\darkchess\darkchess_071\Software Requirements Specification.pdf"; DestDir: "{app}"
Source: "E:\darkchess\darkchess_071\readme.txt"; DestDir: "{app}"; Flags: isreadme
Source: "E:\darkchess\darkchess_071\Image\*.*"; DestDir: "{app}\Image"; Flags: ignoreversion recursesubdirs
Source: "E:\darkchess\darkchess_071\Sound\*.*"; DestDir: "{app}\Sound"; Flags: ignoreversion recursesubdirs  

[Icons]
Name: "{group}\Basic Blind Chess"; Filename: "{app}\darkchess.exe"; IconFilename: "{app}\Image\darkchess_default.ico"
Name: "{group}\Software Requirements Specification"; Filename: "{app}\Software Requirements Specification.pdf";
Name: "{group}\Uninstall Basic Blind Chess"; Filename: "{uninstallexe}";
