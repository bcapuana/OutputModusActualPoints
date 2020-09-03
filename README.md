# OutputModusActualPoints
Macro/Program to output the actual points for a modus program.

## How to use
1. Download the Executable from the releases area
2. Copy the macro from [CommonMacros.dmi](https://github.com/bcapuana/OutputModusActualPoints/blob/master/CommonMacros.dmi) to your macro file, or use CommonMacros.dmi as your starting point for a macro file
3. Put the executable from 1 in the same folder as the Macro File
4. Call the macro from your Modus program
```
  CALL/EXTERN,DMIS,M(OutputPointsToFile),PointFileName,0,ProgramName,'.FIRST.','.LAST.','.NONE.','ALN_DAT_A_B_C'
```
**Note:** The arguments are described in the comment block for the macro.
