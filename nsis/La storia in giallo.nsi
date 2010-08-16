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

Function .onInit
 
	ReadRegStr $R0 HKLM \
		"Software\Microsoft\Windows\CurrentVersion\Uninstall\LaStoriaInGiallo" \
		"UninstallString"
		StrCmp $R0 "" done
 
	MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION \
		"La storia in giallo è già installato. $\n$\nPremi OK per rimuovere la versione precedente o Annulla per annullare l'aggiornamento." \
		IDOK uninst
		Abort
 
uninst:
	ClearErrors
	ExecWait '$R0 _?=$INSTDIR' ;Do not copy the uninstaller to a temp file
 
	IfErrors no_remove_uninstaller done

no_remove_uninstaller:
 
done:
 
FunctionEnd

Section ""
	SetOutPath "$INSTDIR"
	File "..\La storia in giallo.exe"
	File "..\taglib-sharp.dll"
	
	CreateDirectory "$INSTDIR\data"
	SetOutPath "$INSTDIR\data"
	File "..\data\episodes-lstg.txt"
	File "..\data\episodes-cdtd.txt"
	File "..\data\episodes-cdto.txt"
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

	CreateDirectory "$SMPROGRAMS\La storia in giallo"
	CreateShortCut "$SMPROGRAMS\La storia in giallo\Disinstalla.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
	CreateShortCut "$SMPROGRAMS\La storia in giallo\La storia in giallo.lnk" "$INSTDIR\La storia in giallo.exe" "" "$INSTDIR\La storia in giallo.exe" 0
	
	WriteUninstaller "uninstall.exe"
	
	; Change the following values (verions + size) for any new version released
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LaStoriaInGiallo" "DisplayName" "La storia in giallo"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LaStoriaInGiallo" "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LaStoriaInGiallo" "Publisher" "Paolo Bernardi"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LaStoriaInGiallo" "DisplayVersion" "2.0"
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LaStoriaInGiallo" "VersionMajor" "2"
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LaStoriaInGiallo" "VersionMinor" "0"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LaStoriaInGiallo" "DisplayIcon" "$\"$INSTDIR\data\icon.ico$\""
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LaStoriaInGiallo" "EstimatedSize" "14375"
SectionEnd

Section "Uninstall"
	Delete "$INSTDIR\doc\licenza.txt"
	Delete "$INSTDIR\doc\lame-license.txt"
	Delete "$INSTDIR\doc\mplayer-license.txt"
	Delete "$INSTDIR\doc\taglib-sharp-license.txt"
	Delete "$INSTDIR\doc\leggimi.html"
	RMDir "$INSTDIR\doc"

	Delete "$INSTDIR\data\icon.ico"
	Delete "$INSTDIR\data\episodes-lstg.txt"
	Delete "$INSTDIR\data\episodes-cdtd.txt"
	Delete "$INSTDIR\data\episodes-cdto.txt"
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
	
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LaStoriaInGiallo"
SectionEnd