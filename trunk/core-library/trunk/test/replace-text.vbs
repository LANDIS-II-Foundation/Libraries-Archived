'  Usage: {script} findStr replaceStr
'  Replace every occurrence of {findStr} in standard input with {replaceStr}

findStr    = Wscript.Arguments.Item(0)
replaceStr = Wscript.Arguments.Item(1)

inputText  = Wscript.StdIn.ReadAll
Wscript.Echo Replace(inputText, findStr, replaceStr)
