!include "MUI2.nsh"

Name "La storia in giallo"
OutFile "La storia in giallo.exe"
InstallDir "$PROGRAMFILES\La storia in giallo"

RequestExecutionLevel admin

!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "banner.bmp"

!insertmacro MUI_PAGE_LICENSE "..\doc\licenza.txt"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "Italian"
  
Section ""
	SetOutPath "$INSTDIR"
	File "..\La storia in giallo.exe"
	File "..\taglib-sharp.dll"
	
	CreateDirectory "$INSTDIR\data"
	SetOutPath "$INSTDIR\data"
	File "..\data\episodes.txt"
	File "..\data\icon.ico"

	CreateDirectory "$INSTDIR\apps"
	SetOutPath "$INSTDIR\apps"
	File "..\apps\lame.exe"
	File "..\apps\mplayer.exe"

	CreateDirectory "$INSTDIR\doc"
	SetOutPath "$INSTDIR\doc"
	File "..\doc\licenza.txt"
	File "..\doc\lame-license.txt"
	File "..\doc\mplayer-license.txt"
	File "..\doc\taglib-sharp-license.txt"
	File "..\doc\leggimi.html"

	CreateDirectory "$SMPROGRAMS\La storia in giallo"
	CreateShortCut "$SMPROGRAMS\La storia in giallo\Disinstalla.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
	CreateShortCut "$SMPROGRAMS\La storia in giallo\La storia in giallo.lnk" "$INSTDIR\La storia in giallo.exe" "" "$INSTDIR\La storia in giallo.exe" 0
	
	WriteUninstaller "uninstall.exe"
SectionEnd

Section "Uninstall"
	Delete "$INSTDIR\doc\licenza.txt"
	Delete "$INSTDIR\doc\lame-license.txt"
	Delete "$INSTDIR\doc\mplayer-license.txt"
	Delete "$INSTDIR\doc\taglib-sharp-license.txt"
	Delete "$INSTDIR\doc\leggimi.html"
	RMDir "$INSTDIR\doc"

	Delete "$INSTDIR\data\icon.ico"
	Delete "$INSTDIR\data\episodes.txt"
	RMDir "$INSTDIR\data"
	
	Delete "$INSTDIR\apps\lame.exe"
	Delete "$INSTDIR\apps\mplayer.exe"
	RMDir "$INSTDIR\apps\mplayer"
	RMDir "$INSTDIR\apps"
	
	Delete "$INSTDIR\La storia in giallo.exe"
	Delete "$INSTDIR\taglib-sharp.dll"
	Delete "$INSTDIR\uninstall.exe"
	RMDir "$INSTDIR"
	
	Delete "$SMPROGRAMS\La storia in giallo\*.*"
	RMDir "$SMPROGRAMS\La storia in giallo"
SectionEnd