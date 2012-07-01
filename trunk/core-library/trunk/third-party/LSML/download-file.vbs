' Based on the answer to this StackOverflow question:
' http://stackoverflow.com/questions/2973136/download-a-file-with-vbs

If WScript.Arguments.Count <> 2 Then
    WScript.Echo "Usage: " & WScript.ScriptName & " URL localFile"
    WScript.Quit 1
End If

urlToFetch = WScript.Arguments.Item(0)
localFile = WScript.Arguments.Item(1)

dim xHttp: Set xHttp = createobject("Microsoft.XMLHTTP")
xHttp.Open "GET", urlToFetch, False
On Error Resume Next
xHttp.Send
If Err.Number <> 0 Then 
    Set xHttp = Nothing
    WScript.Echo "Error sending URL (error number = " & Err.Number & ")"
    WScript.Quit 1
End If
If xHttp.Status <> "200" Then
    WScript.Echo "Error getting URL (http status = " & xHttp.Status & ")"
    Set xHttp = Nothing 
    WScript.Quit 1
End If

dim bStrm: Set bStrm = createobject("Adodb.Stream")
with bStrm
    .type = 1 '//binary
    .open
    .write xHttp.responseBody
    .savetofile localFile, 2 '//overwrite
end with
