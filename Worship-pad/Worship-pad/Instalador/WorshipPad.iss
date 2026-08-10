[Setup]
AppName=WorshipPad
AppVersion=1.0.0
DefaultDirName={autopf}\WorshipPad
DefaultGroupName=WorshipPad
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=admin
SetupIconFile=..\assets\WorshipPad.ico
UninstallDisplayIcon={app}\WorshipPad.exe
OutputDir=..\bin\Installer
OutputBaseFilename=WorshipPad-Setup

Compression=lzma
SolidCompression=yes


[Files]
Source: "..\bin\Release\net9.0-windows\publish\win-x64\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion
Source: "..\assets\WorshipPad.ico"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\WorshipPad"; Filename: "{app}\Worship-pad.exe"; IconFilename: "{app}\WorshipPad.ico"
Name: "{commondesktop}\WorshipPad"; Filename: "{app}\Worship-pad.exe"; IconFilename: "{app}\WorshipPad.ico"

[Run]
Filename: "{app}\Worship-pad.exe"; Description: "Executar WorshipPad"; Flags: nowait postinstall skipifsilent
