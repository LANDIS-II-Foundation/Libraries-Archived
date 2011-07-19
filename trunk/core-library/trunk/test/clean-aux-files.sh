#! /bin/sh

#  Clean auxiliary files from a build directory.  Auxiliary files are
#  NUnit config files (*.config), netmodules and their *.pdb files for
#  FLEL geospatial-modeling library.
#
#  Usage: {this-script} {path-to-build-dir}

buildDir=$1
if [ -d $buildDir ] ; then
  cd $buildDir
  rm *.config
  rm *.netmodule
  rm *.pdb
fi
