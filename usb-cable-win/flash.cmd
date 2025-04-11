echo off
cls
echo !
echo ALL FT232 USB DEVICES EXCEPT USB CABLE MUST BE DISCONNECTED!!!
echo !
pause
"C:\Program Files (x86)\FTDI\FT_Prog\FT_Prog-CmdLine.exe" scan prog 0,1 usb2usb-cable-ftdi.xml cycl 0,1
pause
