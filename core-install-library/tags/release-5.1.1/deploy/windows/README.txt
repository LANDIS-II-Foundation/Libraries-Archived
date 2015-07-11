Building Windows Installer
--------------------------

Tools

  *  Inno Setup
       http://www.jrsoftware.org/isinfo.php


1) Confirm that the documentation has been built (see ../../doc/README.txt).

2) Run NAnt in this folder to create the include file for Inno Setup with the
   project's settings.

3) Open the Inno Setup script (LANDIS-II.iss) with Inno Setup and compile it
   to generate the windows installer.
