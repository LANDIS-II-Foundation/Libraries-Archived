Succession library for LANDIS-II

Prerequisite for Building
-------------------------

This project requires the LANDIS-II SDK be installed in order to build
and stage the library's assembly into the LANDIS-II directory structure.
Release 04 or a later release of SDK version 6.0 is needed.

Packaging for Release
---------------------

The library is distributed in a zip file.  The zip file contains the
library along with its XML documentation file (for Intellisense/auto-
completion in IDEs).  To generate the zip file, run the appropriate
"make-zip.*" script for your operating system in the deploy folder
(*.cmd for Windows, *.sh for others).