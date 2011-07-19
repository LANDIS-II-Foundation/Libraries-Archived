#! /bin/sh

#  Make a configuration file for NUnit test assembly, by copying a
#  template configuration file, and substituting the text "TEST_DATA_DIR"
#  in the template with the actual path to the test data directory.
#
#  Usage: {this-script} {path-to-config-file} {path-to-data-dir}
#
#  {path-to-data-dir} is the relative path to the data directory from
#  the directory containing {this script}.

configFile=$1
if [ -f $configFile ] ; then
  printf "Configuration file already exists: $configFile\n"
  exit 0
fi

scriptDir=`dirname $0`
#  Get canonical form of script path (remove ".." components)
scriptDir=`cd $scriptDir ; pwd`

configTemplate=$scriptDir/template.config

dataDir=$2
dataDirPath=$scriptDir/$dataDir

if sed "s=TEST_DATA_DIR=$dataDirPath=" $configTemplate > $configFile ; then
  printf "Created configuration file: $configFile\n"
  exit 0
else
  printf "Error creating configuration file: $configFile\n"
  exit 1
fi
