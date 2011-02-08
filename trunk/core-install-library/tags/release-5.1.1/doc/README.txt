Building documentation for LANDIS-II main package
-------------------------------------------------

1) Confirm the project's settings are up-to-date in "../settings.build".

2) Generate the PDF files (build/*.pdf) from the document RTF source files
   (*.rtf).  Every RTF file should have a corresponding PDF in the "build"
   folder.  If the PDF file doesn't exist, create it.  If the PDF file is
   out-of-date (i.e., the RTF is newer because it was modified since the
   PDF file was last generated), then regenerate the PDF file.  See the
   notes in the SDK about generating PDFs from RTFs.

3) Run NAnt in this folder to build the rest of the documentation files in
   the build folder.
