;--------------------------------
; Definitions
;--------------------------------

!define APPNAME "SleepyTunes"
!define COMPANY "addpcs"
!define VERSION "1.0.0"
!define UNINSTALL_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANY} ${APPNAME}"

# OpenCandy config
!define OC_STR_MY_PRODUCT_NAME "${APPNAME}"
!define OC_STR_KEY "748ad6d80864338c9c03b664839d8161"
!define OC_STR_SECRET "dfb3a60d6bfdb55c50e1ef53249f1198"
!define OC_OCSETUPHLP_FILE_PATH ".\OCSetupHlp.dll"


;--------------------------------
; Installer Configuration
;--------------------------------
RequestExecutionLevel admin

# name to display in installer
Name "${APPNAME} ${VERSION}"
BrandingText "Copyright ${COMPANY} 2011"

# name of generated installer exe
OutFile "..\bin\${APPNAME}_Setup_${VERSION}.exe"

# default install location
InstallDir "$PROGRAMFILES\${COMPANY}\${APPNAME}"

# Remember the installation directory for future updates and the uninstaller
InstallDirRegKey HKLM "Software\${COMPANY}\${APPNAME}" "InstallDir"

; Use lzma compression
SetCompressor lzma

; Optimize Data Block
SetDatablockOptimize on

; Restore last write datestamp of files
SetDateSave on

; Show un/installation details
ShowInstDetails   show
ShowUnInstDetails show


;--------------------------------
; Includes
;--------------------------------
!include WinVer.nsh
!include "MUI2.nsh"
!include "Sections.nsh"
!include "OCSetupHlp.nsh"


;--------------------------------
; Reserve files
;--------------------------------
!insertmacro MUI_RESERVEFILE_LANGDLL
!insertmacro OpenCandyReserveFile


;--------------------------------
; Modern UI Configuration
;--------------------------------

; MUI Settings
!define MUI_ABORTWARNING

; MUI Settings / Icons
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\orange-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\orange-uninstall.ico"

; MUI Settings / Header
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_RIGHT
!define MUI_HEADERIMAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Header\orange-r.bmp"
!define MUI_HEADERIMAGE_UNBITMAP "${NSISDIR}\Contrib\Graphics\Header\orange-uninstall-r.bmp"

; MUI Settings / Wizard
!define MUI_WELCOMEFINISHPAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Wizard\orange.bmp"
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Wizard\orange-uninstall.bmp"


;--------------------------------
; Installer pages
;--------------------------------

# Installer pages
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "license.rtf"
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro OpenCandyOfferPage
!insertmacro MUI_PAGE_INSTFILES
!define MUI_FINISHPAGE_RUN "${APPNAME}.exe"
!define MUI_FINISHPAGE_RUN_TEXT "Launch ${APPNAME} now!"
!insertmacro MUI_PAGE_FINISH

# Uninstaller pages
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH


;--------------------------------
; Language support
;--------------------------------

!insertmacro MUI_LANGUAGE "English"
LangString Section_Name_MainProduct    ${LANG_ENGLISH} "${APPNAME} ${VERSION}"
LangString Section_Name_StartMenuIcon  ${LANG_ENGLISH} "Start Menu Shortcut"
LangString Section_Name_DesktopIcon    ${LANG_ENGLISH} "Desktop Shortcut"


;---------------------------
; Install sections
;---------------------------

; This section is hidden. It will always execute during installation
; but it won't appear on the component selection screen.
Section "-OpenCandyEmbedded"
	; Handle any offers the user accepted
	!insertmacro OpenCandyInstallEmbedded
SectionEnd


# Installs the main app
Section "$(Section_Name_MainProduct)" sect0
	SectionIn RO
	Push $OUTDIR

	SetOutPath "$INSTDIR"
	
	# check for and install .Net 4.0 Client profile
	ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client" Install
	IntOp $8 $0 & 1
	${If} $8 != 1
		File ..\prereq\dotNetFx40_Client_setup.exe
		DetailPrint "Installing the .Net 4.0 Framework, this make take several minutes..."
		ExecWait '"$INSTDIR\dotNetFx40_Client_setup.exe"  /q /norestart /ChainingPackage "${APPNAME}"' $0
		Delete "$INSTDIR\dotNetFx40_Client_setup.exe"
		${If} $0 == 3010
			SetRebootFlag true
		${ElseIf} $0 != 0
			Abort "Failed to install .Net Framework: Error $0"
		${EndIf}
	${EndIf}
	
	# install the app
	File ..\bin\${APPNAME}.exe

	# Create an uninstaller and add it to the Add an Add/Remove programs list
	WriteUninstaller Uninstall.exe
	WriteRegStr HKLM "${UNINSTALL_KEY}" DisplayName "${COMPANY} ${APPNAME}"
	WriteRegStr HKLM "${UNINSTALL_KEY}" DisplayIcon '"$OUTDIR\Uninstall.exe"'
	WriteRegStr HKLM "${UNINSTALL_KEY}" UninstallString '"$OUTDIR\Uninstall.exe"'	
	
	Pop $OUTDIR
SectionEnd

# Uninstalls the main app
Section "un.$(Section_Name_MainProduct)"
	Delete /REBOOTOK "$INSTDIR\${APPNAME}.exe"
	
	# Remove the uninstaller and Add/Remove programs information
	Delete /REBOOTOK "$INSTDIR\Uninstall.exe"
	DeleteRegKey HKLM "${UNINSTALL_KEY}"

	# Remove the installation directory if empty
	RMDir /REBOOTOK "$INSTDIR"
SectionEnd

# Install Start Menu shortuct
Section "$(Section_Name_StartMenuIcon)" sect1
	CreateShortCut "$SMPROGRAMS\${COMPANY}\${APPNAME}.lnk" "$INSTDIR\${APPNAME}.exe"
SectionEnd

# Uninstall Start Menu shortuct
Section "un.$(Section_Name_StartMenuIcon)"
	Delete /REBOOTOK "$SMPROGRAMS\${COMPANY}\${APPNAME}.lnk"
	RMDir /REBOOTOK "$SMPROGRAMS\${COMPANY}"
SectionEnd

# Install Desktop Shortcut
Section "$(Section_Name_DesktopIcon)" sect2
	CreateShortCut "$DESKTOP\${APPNAME}.lnk" "$INSTDIR\${APPNAME}.exe"
SectionEnd

# Uninstall Desktop Shortcut
Section "un.$(Section_Name_DesktopIcon)"
	Delete "$DESKTOP\${APPNAME}.lnk"
SectionEnd

;---------------------------
; Localized descriptions
;---------------------------
LangString DESC_sect0 ${LANG_ENGLISH} "Installs the main application on your PC."
LangString DESC_sect1 ${LANG_ENGLISH} "Places a shortcut to the application in your Windows Start Menu."
LangString DESC_sect2 ${LANG_ENGLISH} "Places a shortcut to the application on your Windows desktop."
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${sect0} $(DESC_sect0)
  !insertmacro MUI_DESCRIPTION_TEXT ${sect1} $(DESC_sect1)
  !insertmacro MUI_DESCRIPTION_TEXT ${sect2} $(DESC_sect2)
!insertmacro MUI_FUNCTION_DESCRIPTION_END


;--------------------------------
; .onInit NSIS callback
;--------------------------------

Function .onInit
	${IfNot} ${AtLeastWinVista}
    	MessageBox MB_OK "SleepyTunes requires Windows Vista or newer to install."
    	Quit
	${EndIf}

	!insertmacro MUI_LANGDLL_DISPLAY

	; Initialize OpenCandy, check for offers
	;
	; Note: If you use a language selection system,
	; e.g. MUI_LANGDLL_DISPLAY or calls to LangDLL, you must insert
	; this macro after the language selection code in order for
	; OpenCandy to detect the user-selected language.
	!insertmacro OpenCandyAsyncInit "${OC_STR_MY_PRODUCT_NAME}" "${OC_STR_KEY}" "${OC_STR_SECRET}" ${OC_INIT_MODE_NORMAL}
FunctionEnd

Function .onInstSuccess
	; Signal successful installation, download and install accepted offers
	!insertmacro OpenCandyOnInstSuccess
FunctionEnd

Function .onGUIEnd
	; Inform the OpenCandy API that the installer is about to exit
	!insertmacro OpenCandyOnGuiEnd
FunctionEnd

; Have the compiler perform some basic OpenCandy API implementation checks
!insertmacro OpenCandyAPIDoChecks
