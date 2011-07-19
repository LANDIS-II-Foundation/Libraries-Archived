"""Make a list of locations of true values in a boolean 2-D array.

Usage:
    python {this-script} [ {input-file} ]

This script reads a text file with the data for a 2-dimensional boolean array.
The format of the file is the one used by the Read2DimArray method in the
Bool class of the Landis utility library (Landis.Util.Bool.Read2DimArray).

Blank lines and comment lines are ignored in the file.  A blank line is either
an empty line (has no characters), or has just whitespace.  A comment line is
a line where the first non-whitespace character is "#".  Any other line is
considered a data line.

The first data line in the file specifies the list of characters which will
indicate true elements in the array.  In this example,

    true-chars =Tt1Yy

any of the 5 characters -- upper and lowercase letters "T" and "Y" and the
digit 1 -- may indicate a true element in the array.  All the characters that
follow the "=" represent the list of true-value characters.  This list may be
empty, in which case, all the elements in the array will be false.

The remaining data lines in the file are of the form:

    row {#} ={data for row}

The {#} is an optional row number: one or more digits.  This row number is not
used by this script; it can used a visual aid for users reading the file.  The
data for the row is 0 or more characters, one character per column in the
array.  If the character for a column is in the list of true characters, then
the corresponding element in the array is true.  Othewise, the element is
false.

Each row in the array must have the same number of columns.  It is acceptable
to have 0 columns in each row, or to have no rows (i.e., there are no data
lines with the "row" keyword).

The script writes to standard output a list of locations of the true elements
in the array.  One location is written per line in the format:

    {row} {column}

The locations were written in row-major order.

If no input file is specified, the script reads the file data from standard
input.
"""

#------------------------------------------------------------------------------

import re
import sys

#------------------------------------------------------------------------------

def main():
    try:
        filename = process_cmd_line()
        array = read_array(filename)
        write_true_locations(array)
        return 0
    except ProgramError, err:
        print >>sys.stderr, err.message
        return 1

#------------------------------------------------------------------------------

def process_cmd_line():
    if len(sys.argv) == 1:
        return '-'
    if len(sys.argv) == 2:
        return sys.argv[1]
    raise ProgramError("Usage: python %s [ file ]" % sys.argv[0])

#------------------------------------------------------------------------------

class ProgramError:

    def __init__(self, message):
        self.message = message

    def __str__(self):
        return message

#------------------------------------------------------------------------------

def read_array(filename):
    input_file = InputFile(filename)

    #  Read list of true characters
    line = input_file.read_line()
    pattern = re.compile(r"^\s*true-chars\s*=(.*)")
    match = pattern.match(line)
    if not match:
        raise input_file.error('Expected line starting with "true-chars ="')
    true_chars = match.group(1)

    #  Read row data
    pattern = re.compile(r"^\s*row\s*\d*\s*=(.*)")
    expectedLen = None
    array = []
    while True:
        line = input_file.read_line()
        if line is None:
            break
        match = pattern.match(line)
        if not match:
            raise input_file.error('Expected line starting with "row [#] ="')
        row = match.group(1)
        if expectedLen is None:
            expectedLen = len(row)
        elif expectedLen != len(row):
            raise input_file.error('Expected %d character%s after the "="' %
                                   expectedLen,
                                   (expectedLen != 1 and "s" or ""))
        array.append([ch in true_chars for ch in row])

    return array

#------------------------------------------------------------------------------

class InputFile:

    def __init__(self, filename):
        self.filename = filename
        if filename == "-":
            self.filename = "standard input"
            self.file = sys.stdin
        else:
            self.file = file(filename, 'r')
        self.line_number = 0

    def read_line(self):
        if self.file is None:
            return None

        while True:
            line = self.file.readline()
            if line == "":
                self.file = None
                self.line_number = None
                return None
            self.line_number += 1
            line = re.sub(r'\s+', '', line)
            if line == "":
                continue
            if re.match(r'\s*#', line):
                continue
            return line

    def error(self, message):
        if self.line_number is None:
            location = 'end of'
        else:
            location = 'line %d in' % self.line_number
        return ProgramError('At %s %s:\n  %s' % (location, self.filename,
                                                 message))

#------------------------------------------------------------------------------

def write_true_locations(array):
    for row in range(0, len(array)):
        for column in range(0, len(array[0])):
            if array[row][column]:
                print row+1, column+1

#------------------------------------------------------------------------------

if __name__ == "__main__":
    main()
