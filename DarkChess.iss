; -- Example1.iess --
; Demonstrates copying 3 files and creating an icon.

; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

[Setup]
AppName=Basic Blind Chess
AppVersion=1.1.2
DefaultDirName={pf}\Basic Blind Chess
DefaultGroupName=Basic Blind Chess
UninstallDisplayIcon={app}\dchess.exe
Compression=lzma2
SolidCompression=yes
OutputDir=D:\darkchess
ArchitecturesInstallIn64BitMode=x64

[Files]
Source: "D:\darkchess\darkchess_112\*.*"; DestDir: "{app}"; Check: Is64BitInstallMode
Source: "D:\darkchess\darkchess_112\readme.txt"; DestDir: "{app}"; Flags: isreadme
Source: "D:\darkchess\darkchess_112\Image\*.*"; DestDir: "{app}\Image"; Flags: ignoreversion recursesubdirs
Source: "D:\darkchess\darkchess_112\Sound\*.*"; DestDir: "{app}\Sound"; Flags: ignoreversion recursesubdirs  

[Icons]
Name: "{group}\Basic Blind Chess"; Filename: "{app}\dchess.exe"; IconFilename: "{app}\Image\darkchess_default.ico"
Name: "{group}\Software Requirements Specification"; Filename: "{app}\Software Requirements Specification.pdf";
Name: "{group}\Uninstall Basic Blind Chess"; Filename: "{uninstallexe}";
