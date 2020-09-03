# OutputModusActualPoints
Macro/Program to output the actual points for a modus program.

## How to Use
There are 2 parts to this project:
1. The OutputPointsToFile macro
2. The GetFeaturesForOutput Executable.

### OutputPointsToFile
This macro, for modus, is responsible for calling the executable, reading the file it generates, and outputting the points to a text file
The macro can be located here:

### GetFeaturesForOutput
The exeuctable connects to the modus database, gets the all of the measured features, orders them by the date/time they were inspected, and writes out a text file for modus to read.

### Making Changes to GetFeaturesForOutput
Upon making changes, publish the project as a single file executable and place the resulting exe in the same folder as the macro.
