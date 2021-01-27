﻿'V++ Interpreter
'Made by VMGP Official (2016-2021)
'
'VppInterpreter.vb: Used for interpreting V++ code.


Imports System.Windows.Forms
Imports System.Threading
Imports System.IO
Imports System.Net
Imports System.Web
Imports System.Web.Script.Serialization

''' <summary>
''' The V++ interpreter class. Check https://github.com/VMGP/vppi/wiki.
''' </summary>
Public Class VppInterpreter
    Public threadname
    Public Class DefineObject
        Public value = Nothing
        Public type = Nothing
        Public _constant = False
        Public _temporary = False
        Public _private = False
        Public _fargs As String()

        Sub New(Optional _type As String = Nothing, Optional _value As Object = Nothing)
            type = _type
            value = _value
        End Sub
    End Class

    Private ac As New List(Of String) 'code lines
    Private code As String = "" 'raw code
    Private ticktimer As Threading.Timer 'tick timer
    Public ip = 0 'pointer
    Public ignoreerr = False
    Private tmpip = 0 'temp pointer 1
    Private tmpip1 = 0 'temp pointer 2
    Private tmpip2 = -1 'temp pointer 3
    Private tmpval = Nothing 'temp value
    Private tmpval1 = Nothing 'temp value 1
    Private tmpval2 = Nothing 'temp value 2 
    Private tmpval3 = Nothing 'temp value 3 
    Private tmpval4 = Nothing 'temp value 4
    Private tmpval5 As New List(Of String)
    Private canexec = True 'can execute
    Private state = 0 'in "if" statement
    Private starttimer As Integer 'timer value
    Public objects As New Dictionary(Of String, DefineObject) 'defined objects by script (int, string, function)
    Public events As New Dictionary(Of String, String)
    Public didsetup = False
    Public dependencies As New Dictionary(Of String, VppInterpreter)
    Public nf = "main"
    Public cf = ""
    Public slave = False
    Public logfilename = ""
    Public logfile As StreamWriter
    Dim invalidreserved As String() = {"#", "@", "!", "+", "-", "/", "*", "^", "vppmath", "command", "exit", "wait", "varop", "string", "int", "bool"}

    'Events

    'Variable defining and similar stuff
    Public nv_const = False
    Public nv_private = False
    Public nv_temp = False

    'Web Requests
    Public webheaders As New Dictionary(Of String, String)

    Sub New(fpath As String, Optional _slave As Boolean = False)
        Try
            If Path.GetExtension(fpath) = ".vpp" Then

            Else
                MsgBox("Failed to load script.", MsgBoxStyle.Critical, "V++ Interpreter")
            End If
            code = File.ReadAllText(fpath)
            logfilename = "\log_" + Path.GetFileNameWithoutExtension(fpath) + "_" + DateTime.Now.Hour.ToString + DateTime.Now.Minute.ToString + DateTime.Now.Second.ToString + ".txt"
            slave = _slave
            threadname = Path.GetFileNameWithoutExtension(fpath)
            startinterpret()
        Catch ex As Exception
            MsgBox("Failed to load script.", MsgBoxStyle.Critical, "V++ Interpreter")
        End Try
    End Sub

    ''' <summary>
    ''' Start interpreting the script.
    ''' </summary>
    Sub startinterpret()
        ac.Clear()
        For Each i As String In code.Split(vbNewLine)
            ac.Add(i)
        Next
        If slave = False Then
            logfile = File.CreateText(Module1.getapplogsdir() + logfilename)
            logfile.AutoFlush = True
            starttimer = 100
            ticktimer = New Threading.Timer(AddressOf tick, Nothing, 0, starttimer)
            log("[" + DateTime.Now.ToString + "]: Setting up...")
            While True

            End While
        End If
    End Sub

    Function varnamebad(varname)
        Dim varnamebadt = False
        canexec = False
        For Each i In invalidreserved
            If varname.Contains(i) Then
                If i.Length < 1 Then
                    varnamebadt = True
                End If
            End If
            If varname = i Then
                varnamebadt = True
            End If
        Next
        canexec = True
        Return varnamebadt
    End Function

    Function tick(state As Object) As TimerCallback
        ptick()

        ticktimer.Dispose()
        ticktimer = New Threading.Timer(AddressOf tick, Nothing, 0, starttimer)
    End Function

    Public Sub ptick()
        If didsetup = False Then
            setup()
        ElseIf didsetup = True Then
            interpret()
        End If

        For Each i In dependencies
            i.Value.ptick()
        Next
    End Sub

    ''' <summary>
    ''' Reset the interpreter tempvals.
    ''' </summary>
    Sub reset()
        ip = 0
        tmpip = -1
        tmpip1 = -1
        canexec = True
        state = 0
    End Sub

    ''' <summary>
    ''' Setting up the script. (defining functions, loading dependencies)
    ''' </summary>
    Sub setup()
        If tmpip < ac.Count Then
            Static steststring As String
            Static sinsset() As String

            Try
                steststring = ac.Item(tmpip)
            Catch ex As ArgumentOutOfRangeException

            End Try

            sinsset = Split(steststring)

            sinsset(0) = ""
            For Each ii In Split(steststring)(0)

                If Asc(ii) = 9 Then

                ElseIf Asc(ii) = 10 Then

                Else
                    sinsset(0) = sinsset(0) + ii
                End If
            Next

            If sinsset(0) = "@ScriptName" Then
                threadname = sinsset(1)
            ElseIf sinsset(0) = "@EntryPoint" Then
                nf = sinsset(1)
            ElseIf sinsset(0) = "@Include" Then
                include(sinsset(1))
            ElseIf sinsset(0) = "@IgnoreErrors" Then
                ignoreerr = True
            ElseIf sinsset(0) = "[private]" Then

            ElseIf sinsset(0) = "function" Then
                If varnamebad(sinsset(1)) Then
                    exceptionmsg(Chr(34) + sinsset(1) + Chr(34) + " is an invalid or reserved name.", "", 0, True)
                End If
                If objects.ContainsKey(sinsset(1)) = False Then
                    objects.Add(sinsset(1), New DefineObject("function", tmpip))
                    objects(sinsset(1))._fargs = parsearguments(sinsset, 2)
                    log("[" + DateTime.Now.ToString + "-S]: Defined function " + sinsset(1) + ", ip: " + tmpip.ToString)
                End If

            ElseIf sinsset(0) = "@HandleEvent" Then
                If sinsset(1) = "InputEvent" Then
                    tmpval = "InputEvent"
                End If
            Else

            End If

            tmpip = tmpip + 1
        Else
            reset()
            log("[" + DateTime.Now.ToString + "-S]: Finished setting up.")
            didsetup = True
        End If
    End Sub

    ''' <summary>
    ''' Load a dependency. Function called with the "@Include" keyword, at setup time.
    ''' </summary>
    ''' <param name="fpath">File path to the dependency.</param>
    Sub include(fpath As String)
        If File.Exists(fpath) Then
            dependencies.Add(fpath.Substring(fpath.LastIndexOf("\") + 1).Remove(fpath.Substring(fpath.LastIndexOf("\") + 1).LastIndexOf(".")), New VppInterpreter(fpath, True))
            log("[" + DateTime.Now.ToString + "-S]: Loaded dependency. File path: " + fpath + " as " + fpath.Substring(fpath.LastIndexOf("\") + 1).Remove(fpath.Substring(fpath.LastIndexOf("\") + 1).LastIndexOf(".")))
        ElseIf File.Exists(Module1.getapplibsdir() + fpath) Then
            dependencies.Add(fpath.Substring(fpath.LastIndexOf("\") + 1).Remove(fpath.Substring(fpath.LastIndexOf("\") + 1).LastIndexOf(".")), New VppInterpreter(Module1.getapplibsdir() + fpath, True))
            log("[" + DateTime.Now.ToString + "-S]: Loaded dependency. File path: " + Module1.getapplibsdir() + fpath + " as " + fpath.Substring(fpath.LastIndexOf("\") + 1).Remove(fpath.Substring(fpath.LastIndexOf("\") + 1).LastIndexOf(".")))
        Else
            log("[" + DateTime.Now.ToString + "-S]: Failed to load dependency. Could not find: " + fpath)
            exceptionmsg("[SETUP]: Failed to load dependency. Could not find: " + fpath, "g_0003", 1)
        End If
    End Sub

    Private Sub interpret()
        If ip < ac.Count Then
            Static teststring As String
            Static insset() As String

            If canexec Then
                Try
                    teststring = ac.Item(ip)
                Catch ex As ArgumentOutOfRangeException
                    teststring = ""
                End Try

                insset = Split(teststring)
                execfunction(insset, teststring.Length)

                ip = ip + 1
            End If
        Else

        End If
    End Sub

#Region "Parser Fucntions"
    Sub flushtempvars()
        For Each i As KeyValuePair(Of String, DefineObject) In objects
            If i.Value.type = "function" Then

            Else
                If i.Value._temporary = True Then
                    objects.Remove(i.Key)
                End If
            End If
        Next
    End Sub

    Function removeun(_insset1 As String)
        canexec = False
        tmpval4 = ""

        For Each ii In _insset1
            If Asc(ii) = 9 Then

            ElseIf Asc(ii) = 10 Then

            Else
                tmpval4 = tmpval4 + ii
            End If
        Next

        canexec = True
        Return tmpval4
    End Function

    Function execfunction(_insset() As String, strlen As Integer)
        'Parser
        Static insset() As String
        insset = _insset
        insset(0) = removeun(_insset(0))

        log("[" + DateTime.Now.ToString + "]: " + "Interpreting line " + ip.ToString + ". Contents: " + Chr(34) + stringa_to_string(insset) + Chr(34))

        'Function executing
        If cf = nf Then
            If insset(0) = "find" Then

            ElseIf insset(0) = "command" Then
                instruction_interrupt(insset)
            ElseIf insset(0) = "exit" Then
                If state = 1 Then

                Else
                    End
                End If
            ElseIf insset(0) = "wait" Then
                instruction_wait(insset)
            ElseIf insset(0) = "varop" Then
                instruction_varop(insset)
            ElseIf insset(0) = "define" Then
                instruction_def(insset)
            ElseIf insset(0) = "[private]" Then
                nv_private = True
            ElseIf insset(0) = "[const]" Then
                nv_const = True
            ElseIf insset(0) = "[temp]" Then
                nv_temp = True
            ElseIf insset(0) = "function" Then

            ElseIf insset(0) = "if" Then
                instruction_if(insset)
            ElseIf insset(0) = "vppmath" Then
                instruction_math(insset)
            ElseIf insset(0) = "end" Then
                If insset(1) = "if" Then
                    If state = 1 Then
                        state = 0
                    ElseIf state = 2 Then
                        state = 0
                    End If
                ElseIf insset(1) = "function" Then
                    cf = ""
                    flushtempvars()
                End If
            ElseIf insset(0) = "#" Then
                'Comment
            Else

                If state = 1 Then

                Else
                    If insset(0).Length > 1 Then
                        setexec(insset)
                    End If
                End If
            End If
        Else
            If insset(0) = "function" Then
                cf = insset(1)
            ElseIf insset(0) = "end" Then
                If insset(1) = "function" Then
                    cf = ""
                End If
            ElseIf insset(0) = "define" Then
                instruction_def(insset)
            ElseIf insset(0) = "#" Then
                'Comment
            End If
        End If
    End Function

    ''' <summary>
    ''' Exception message. Check https://github.com/VMGP/vppi/wiki
    ''' </summary>
    ''' <param name="message">Exception message.</param>
    ''' <param name="errcode">Error code.</param>
    ''' <param name="severity"></param>
    Sub exceptionmsg(message As String, errcode As String, Optional severity As Integer = 0, Optional exitsetup As Boolean = False)
        canexec = False
        Console.WriteLine()
        Console.WriteLine()
        If severity = 0 Then
            Console.WriteLine("Error: [" + errcode + "] at " + threadname + " (" + (ip + 1).ToString + "): " + message)
        ElseIf severity = 1 Then
            Console.WriteLine("Warning: [" + errcode + "] at " + threadname + " (" + (ip + 1).ToString + "): " + message)
        End If
        If ignoreerr = True Then
            Console.WriteLine()
            canexec = True
        ElseIf didsetup = False Then
            If exitsetup Then
                Console.WriteLine("Press any key...")
                Console.Read()
                End
            Else
                Console.WriteLine()
                canexec = True
            End If
        Else
            Console.WriteLine("Press any key...")
            Console.Read()
            End
        End If
    End Sub

    Sub execfunc(fname As String, fargs As String())
        canexec = False
        tmpval4 = 0
        tmpval3 = Nothing

        If objects(fname)._fargs.Count >= fargs.Length Then
            If objects(fname)._fargs IsNot Nothing Then
                For Each i In fargs
                    If objects(fname)._fargs(tmpval4) IsNot Nothing Then
                        tmpval3 = objects(fname)._fargs(tmpval4).Split(" ")

                        If tmpval3.Length > 1 Then
                            If objects.ContainsKey(fargs(0)) Then
                                objects.Add(tmpval3(0), New DefineObject(tmpval3(2), objects(fargs(0))))
                            Else
                                objects.Add(tmpval3(0), New DefineObject(tmpval3(2), i))
                            End If
                        End If
                    End If

                    tmpval4 += 1
                Next

            End If
        End If

        tmpip2 = ip - 1
        ip = objects(fname).value - 1
        nf = fname
        tmpval4 = Nothing
        canexec = True
    End Sub

    Function getvar(insset_getvar As String())
        Static gvparameters As String()
        gvparameters = parsearguments(insset_getvar, 1)

        If dependencies.ContainsKey(vppstring_to_string(gvparameters(0))) Then
            tmpval2 = vppstring_to_string(gvparameters(0))
            If objects.ContainsKey(gvparameters(2)) Then
                If dependencies(tmpval2).objects.ContainsKey(gvparameters(1)) Then
                    objects(gvparameters(2)).value = objects(gvparameters(1)).value
                End If
            End If
        End If
    End Function

    Sub setexec(insset As String())
        If objects.ContainsKey(insset(0)) Then
            If insset(1) = "=" Then
                'Set variable value
                If objects.ContainsKey(insset(2)) Then
                    If isexpression(insset, 2) Then

                    Else
                        setvalue(insset(0), getvalue(insset(2)))
                    End If
                ElseIf dependencies.ContainsKey(insset(2)) Then
                    If insset(3) = "::" Then
                        If dependencies(insset(2)).objects.ContainsKey(insset(4)) Then
                            setvalue(insset(0), dependencies(insset(2)).objects(insset(4)))
                        End If
                    End If
                Else
                    If gettypefromval(stringa_to_string(insset, 2)) = "string" Then
                        setvalue(insset(0), stringa_to_string(insset, 2))
                    Else
                        setvalue(insset(0), insset(2))
                    End If
                End If
            Else
                'Execute function
                execfunc(insset(0), parsearguments(insset, 1))
            End If
        Else
            If dependencies.ContainsKey(insset(0)) Then
                If insset(1) = "::" Then
                    If insset(3) = "=" Then
                        'Set variable value
                        If objects.ContainsKey(insset(4)) Then
                            dependencies(insset(0)).setvalue(insset(2), stringa_to_string(insset, 4))
                        Else
                            If gettypefromval(stringa_to_string(insset, 2)) = "string" Then
                                dependencies(insset(0)).setvalue(insset(2), stringa_to_string(insset, 4))
                            Else
                                dependencies(insset(0)).setvalue(insset(2), insset(4))
                            End If
                        End If
                    Else
                        'Execute function
                        dependencies(insset(0)).execfunc(insset(2), parsearguments(insset, 3))
                    End If
                End If
            Else
                canexec = False
                Dim insset_string = ""
                For Each i In insset
                    insset_string = insset_string + i + " "
                Next
                exceptionmsg("Syntax error: " + insset_string, "g_0001")
            End If
        End If
    End Sub

    Sub instruction_wait(ByVal stringval() As String)
        Static wparameters As String()
        wparameters = parsearguments(stringval, 1)
        If Not wparameters Is Nothing Then
            canexec = False
            Thread.Sleep(wparameters(0))
            canexec = True
        End If
    End Sub

    Sub instruction_math(ByVal stringvalmath() As String)
        If state = 1 Then
            Exit Sub
        End If
        If stringvalmath(1) = "::" Then

        Else
            exceptionmsg("Invalid syntax.", "g_0001")
            Exit Sub
        End If
        tmpval2 = Nothing
        tmpval3 = Nothing
        Static mathparameters As String()
        mathparameters = parsearguments(stringvalmath, 3)
        If stringvalmath(2) = "add" Then
            If gettypefromval(mathparameters(1)) = "int" Then
                tmpval2 = mathparameters(1)
            Else
                If objects.ContainsKey(mathparameters(1)) Then
                    tmpval2 = objects(mathparameters(1)).value
                End If
            End If
            If gettypefromval(mathparameters(2)) = "int" Then
                tmpval3 = mathparameters(2)
            Else
                If objects.ContainsKey(mathparameters(2)) Then
                    tmpval3 = objects(mathparameters(2)).value
                End If
            End If
            If objects.ContainsKey(mathparameters(0)) Then
                If objects(mathparameters(0)).type = "int" Then
                    objects(mathparameters(0)).value = (Convert.ToDecimal(tmpval2) + Convert.ToDecimal(tmpval3)).ToString()
                End If
            End If
        ElseIf stringvalmath(2) = "subtract" Then
            If gettypefromval(mathparameters(1)) = "int" Then
                tmpval2 = mathparameters(1)
            Else
                If objects.ContainsKey(mathparameters(1)) Then
                    tmpval2 = objects(mathparameters(1)).value
                End If
            End If
            If gettypefromval(mathparameters(2)) = "int" Then
                tmpval3 = mathparameters(2)
            Else
                If objects.ContainsKey(mathparameters(2)) Then
                    tmpval3 = objects(mathparameters(2)).value
                End If
            End If
            If objects.ContainsKey(mathparameters(0)) Then
                If objects(mathparameters(0)).type = "int" Then
                    objects(mathparameters(0)).value = (Convert.ToDecimal(tmpval2) - Convert.ToDecimal(tmpval3)).ToString()
                End If
            End If
        ElseIf stringvalmath(2) = "multiply" Then
            If gettypefromval(mathparameters(1)) = "int" Then
                tmpval2 = mathparameters(1)
            Else
                If objects.ContainsKey(mathparameters(1)) Then
                    tmpval2 = objects(mathparameters(1)).value
                End If
            End If
            If gettypefromval(mathparameters(2)) = "int" Then
                tmpval3 = mathparameters(2)
            Else
                If objects.ContainsKey(mathparameters(2)) Then
                    tmpval3 = objects(mathparameters(2)).value
                End If
            End If
            If objects.ContainsKey(mathparameters(0)) Then
                If objects(mathparameters(0)).type = "int" Then
                    objects(mathparameters(0)).value = (Convert.ToDecimal(tmpval2) * Convert.ToDecimal(tmpval3)).ToString()
                End If
            End If
        ElseIf stringvalmath(2) = "divide" Then
            If gettypefromval(mathparameters(1)) = "int" Then
                tmpval2 = mathparameters(1)
            Else
                If objects.ContainsKey(mathparameters(1)) Then
                    tmpval2 = objects(mathparameters(1)).value
                End If
            End If
            If gettypefromval(mathparameters(2)) = "int" Then
                tmpval3 = mathparameters(2)
            Else
                If objects.ContainsKey(mathparameters(2)) Then
                    tmpval3 = objects(mathparameters(2)).value
                End If
            End If
            If objects.ContainsKey(mathparameters(0)) Then
                If objects(mathparameters(0)).type = "int" Then
                    objects(mathparameters(0)).value = (Convert.ToDecimal(tmpval2) / Convert.ToDecimal(tmpval3)).ToString()
                End If
            End If
        End If
    End Sub


    Sub instruction_if(ByVal stringvalif() As String)
        If state = 1 Then
            Exit Sub
        End If
        Static ifparameters As String()
        ifparameters = parsearguments(stringvalif, 1)
        If objects.ContainsKey(ifparameters(1)) Then
            tmpval1 = objects(ifparameters(1)).value
        Else
            tmpval1 = ifparameters(1)
        End If

        If objects.ContainsKey(ifparameters(2)) Then
            tmpval2 = objects(ifparameters(2)).value
        Else
            tmpval2 = ifparameters(2)
        End If

        If ifparameters(0) = "0" Then
            If tmpval1 = tmpval2 Then
                state = 2
            Else
                state = 1
            End If
        ElseIf ifparameters(0) = "1" Then
            If tmpval1 > tmpval2 Then
                state = 2
            Else
                state = 1
            End If
        ElseIf ifparameters(0) = "2" Then
            If tmpval1 < tmpval2 Then
                state = 2
            Else
                state = 1
            End If
        ElseIf ifparameters(0) = "3" Then
            If Not tmpval1 = tmpval2 Then
                state = 2
            Else
                state = 1
            End If
        End If
        tmpval1 = Nothing
        tmpval2 = Nothing
    End Sub

    Sub instruction_varop(ByVal stringval() As String)
        If state = 1 Then
            Exit Sub
        End If
        Try
            Static vaparameters As String()
            vaparameters = parsearguments(stringval, 1)
            If vaparameters(0) = "0x0001" Then
                Try

                    If objects.ContainsKey(vaparameters(2)) Then
                        tmpval3 = objects(vppstring_to_string(vaparameters(2))).value.ToString
                        tmpval = tmpval3.Remove(tmpval3.Length - 2, 1)
                        tmpval1 = tmpval.ToString.Remove(0, 1)
                    Else
                        tmpval3 = vaparameters(2)
                        tmpval = tmpval3.Remove(tmpval3.Length - 2, 1)
                        tmpval1 = tmpval.ToString.Remove(0, 1)
                    End If
                    If objects.ContainsKey(vaparameters(3)) Then
                        tmpval4 = objects(vppstring_to_string(vaparameters(3))).value.ToString
                        tmpval = tmpval4.Remove(tmpval4.Length - 2, 1)
                        tmpval2 = tmpval.ToString.Remove(0, 1)
                    Else
                        tmpval4 = vaparameters(3)
                        tmpval = tmpval4.Remove(tmpval4.Length - 2, 1)
                        tmpval2 = tmpval.ToString.Remove(0, 1)
                    End If
                    If objects.ContainsKey(vaparameters(1)) Then
                        objects(vaparameters(1)).value = Chr(34) + tmpval1 + tmpval2 + Chr(34)
                    Else
                        exceptionmsg("Could not find/access " + vaparameters(1), "d_0001", 1)
                    End If
                Catch ex As Exception
                    exceptionmsg("Internal exception: " + ex.Message + ex.StackTrace, "c_0002")
                End Try
            ElseIf vaparameters(0) = "0x0002" Then
                Try
                    If objects.ContainsKey(vaparameters(2)) Then
                        tmpval3 = objects(vppstring_to_string(vaparameters(2))).value
                    Else
                        tmpval3 = vaparameters(2)
                    End If
                    If objects.ContainsKey(vaparameters(1)) Then
                        objects(vaparameters(1)).value = Chr(34) + tmpval1 + Chr(34)
                    Else
                        exceptionmsg("Could not find/access " + vaparameters(1), "d_0001", 1)
                    End If
                Catch ex As Exception
                    exceptionmsg("Internal exception: " + ex.Message + ex.StackTrace, "c_0002")
                End Try
            Else
                exceptionmsg("Invalid parameters given: " + Chr(34) + vaparameters(0) + Chr(34), "c_0001")
            End If
        Catch ex As Exception
            If TypeOf ex Is NullReferenceException Then
                exceptionmsg("Internal exception: " + ex.Message, "c_0002")
            End If
        End Try
    End Sub

    Function isexpression(insset() As String, startip As String)
        tmpval4 = parsearguments(insset, startip)
        tmpval1 = False
        tmpval2 = False
        For Each i In tmpval4(0)
            If i = "+" Then
                If tmpval2 = False Then
                    tmpval1 = True
                End If
            ElseIf i = "-" Then
                If tmpval2 = False Then
                    tmpval1 = True
                End If
            ElseIf i = "*" Then
                If tmpval2 = False Then
                    tmpval1 = True
                End If
            ElseIf i = ":" Then
                If tmpval2 = False Then
                    tmpval1 = True
                End If
            ElseIf i = Chr(34) Then
                If tmpval2 = False Then
                    tmpval2 = True
                Else
                    tmpval2 = False
                End If
            End If
        Next
    End Function

    Sub instruction_def(ByVal stringval() As String)
        If state = 1 Then
            Exit Sub
        End If
        tmpval = Nothing
        If stringval(2) = "as" Then
            If gettypefromval(stringval(1)) = "int" Then
                exceptionmsg("Failed to define variable.", "d_0001")
            ElseIf gettypefromval(stringval(1)) = "string" Then
                exceptionmsg("Failed to define variable.", "d_0001")
            ElseIf gettypefromval(stringval(1)) = "bool" Then
                exceptionmsg("Failed to define variable.", "d_0001")
            Else
                If stringval(3) = "string" Then
                    If stringval(4) = "=" Then
                        Try
                            tmpval = stringa_to_string(stringval, 5)
                            log("[" + DateTime.Now.ToString + "]: Value defined (string). vname: " + Chr(34) + stringval(1) + Chr(34) + ",vval: " + Chr(34) + tmpval + Chr(34))
                            objects.Add(stringval(1), New DefineObject("string", tmpval))
                        Catch ex As Exception
                            exceptionmsg("Failed to define variable.", "d_0001")
                        End Try
                    End If
                ElseIf stringval(3) = "bool" Then
                    If stringval(4) = "=" Then
                        Try
                            log("[" + DateTime.Now.ToString + "]: Value defined (string). vname: " + Chr(34) + stringval(1) + Chr(34) + ",vval: " + Chr(34) + tmpval + Chr(34))
                            objects.Add(stringval(1), New DefineObject("bool", stringval(5)))
                        Catch ex As Exception
                            exceptionmsg("Failed to define variable.", "d_0001")
                        End Try
                    End If
                ElseIf stringval(3) = "int" Then
                    If stringval(4) = "=" Then
                        Try
                            log("[" + DateTime.Now.ToString + "]: Value defined (string). vname: " + Chr(34) + stringval(1) + Chr(34) + ",vval: " + Chr(34) + tmpval + Chr(34))
                            objects.Add(stringval(1), New DefineObject("int", stringval(5)))
                        Catch ex As Exception
                            exceptionmsg("Failed to define variable.", "d_0001")
                        End Try
                    End If
                End If
            End If
        End If
    End Sub

    Sub instruction_interrupt(ByVal stringval() As String)
        If state = 1 Then
            Exit Sub
        End If
        Try
            Static parameters As String()
            parameters = parsearguments(stringval, 1)
            log("[" + DateTime.Now.ToString + "]: Command requested. cmdid: " + Chr(34) + parameters(0) + Chr(34) + " args: " + Chr(34) + stringa_to_string(parameters, 1) + Chr(34))
            If parameters(0) = "0x0001" Then
                If objects.ContainsKey(vppstring_to_string(parameters(1))) Then
                    Console.Title = objects(parameters(1)).value
                Else
                    Console.Title = vppstring_to_string(parameters(1))
                End If
            ElseIf parameters(0) = "0x0002" Then
                Console.SetCursorPosition(Convert.ToDecimal(parameters(1)), Convert.ToDecimal(parameters(2)))
            ElseIf parameters(0) = "0x0003" Then
                Console.Clear()
            ElseIf parameters(0) = "0x0004" Then
                If objects.ContainsKey(vppstring_to_string(parameters(1))) Then
                    Console.Write(vppstring_to_string(objects(vppstring_to_string(parameters(1))).value))
                Else
                    Console.Write(vppstring_to_string(parameters(1)))
                End If
            ElseIf parameters(0) = "0x0005" Then
                If objects.ContainsKey(vppstring_to_string(parameters(1))) Then
                    Console.WriteLine(vppstring_to_string(objects(vppstring_to_string(parameters(1))).value))
                Else
                    Console.WriteLine(vppstring_to_string(parameters(1)))
                End If
            ElseIf parameters(0) = "0x0006" Then
                canexec = False
                Console.Read()
                canexec = True
            ElseIf parameters(0) = "0x0007" Then
                canexec = False
                Try
                    If objects.ContainsKey(parameters(1)) Then
                        objects(parameters(1)).value = Chr(34) + Console.ReadLine() + Chr(34)
                    Else

                    End If
                Catch ex As Exception

                End Try
                canexec = True
            ElseIf parameters(0) = "0x0008" Then
                canexec = False
                MsgBox(vppstring_to_string(parameters(1)))
                canexec = True
            ElseIf parameters(0) = "0x0009" Then
                Console.OpenStandardOutput()
            ElseIf parameters(0) = "0x000A" Then
                If gettypefromval(parameters(1)) = "int" Then
                    Console.ForegroundColor = Convert.ToDecimal(parameters(1))
                Else
                    If objects.ContainsKey(parameters(1)) Then
                        Console.ForegroundColor = Convert.ToDecimal(objects(parameters(1)).value)
                    End If
                End If
            ElseIf parameters(0) = "0x000B" Then
                If gettypefromval(parameters(1)) = "int" Then
                    Console.BackgroundColor = Convert.ToDecimal(parameters(1))
                Else
                    If objects.ContainsKey(parameters(1)) Then
                        Console.BackgroundColor = Convert.ToDecimal(objects(parameters(1)).value)
                    End If
                End If
            ElseIf parameters(0) = "0x000C" Then
                Beep()
            ElseIf parameters(0) = "0x000D" Then
                If gettypefromval(parameters(1)) = "int" Then
                    ip = Convert.ToDecimal(parameters(1))
                Else
                    If objects.ContainsKey(parameters(1)) Then
                        ip = Convert.ToDecimal(objects(parameters(1)).value)
                    End If
                End If
            ElseIf parameters(0) = "0x000F" Then
                If gettypefromval(parameters(1)) = "string" Then
                    canexec = False
                    exceptionmsg(parameters(1), "g_0004")
                Else
                    If objects.ContainsKey(parameters(1)) Then
                        exceptionmsg(parameters(1), vppstring_to_string(objects(vppstring_to_string(parameters(1))).value))
                    End If
                End If
            ElseIf parameters(0) = "0x0010" Then
                If objects.ContainsKey(parameters(1)) Then
                    canexec = False
                    objects(parameters(1)).value = Chr(34) + InputBox(vppstring_to_string(parameters(2))) + Chr(34)
                    canexec = True
                Else

                End If
            ElseIf parameters(0) = "0x0011" Then
                Try
                    If objects.ContainsKey(parameters(1)) Then
                        If File.Exists(vppstring_to_string(parameters(2))) Then
                            objects(parameters(1)).value = Chr(34) + File.ReadAllText(vppstring_to_string(parameters(2))) + Chr(34)
                        End If
                    Else

                    End If
                Catch ex As Exception
                    exceptionmsg("Failed to access file.", "i_0001")
                End Try
            ElseIf parameters(0) = "0x0012" Then
                Try
                    If objects.ContainsKey(parameters(1)) Then
                        My.Computer.FileSystem.WriteAllText(vppstring_to_string(parameters(2)), vppstring_to_string(objects(vppstring_to_string(parameters(1))).value), False)
                    Else
                        My.Computer.FileSystem.WriteAllText(vppstring_to_string(parameters(2)), vppstring_to_string(parameters(1)), False)
                    End If
                Catch ex As Exception
                    exceptionmsg("Failed to access file.", "i_0001")
                End Try
            ElseIf parameters(0) = "0x0013" Then
                Try
                    If objects.ContainsKey(parameters(1)) Then
                        Shell(vppstring_to_string(objects(vppstring_to_string(parameters(1))).value))
                    Else
                        Shell(vppstring_to_string(parameters(1)))
                    End If
                Catch ex As Exception
                    exceptionmsg("General exception.", "g_0003")
                End Try
            ElseIf parameters(0) = "0x0014" Then
                Try
                    If objects.ContainsKey(parameters(1)) Then
                        objects(parameters(1)).value = My.Application.Info.Version.Major.ToString()
                    Else

                    End If
                Catch ex As Exception
                    exceptionmsg("General exception.", "g_0003")
                End Try
            ElseIf parameters(0) = "0x0016" Then
                Try
                    If objects.ContainsKey(parameters(1)) Then
                        objects(parameters(1)).value = My.Application.Info.Version.MajorRevision.ToString()
                    Else

                    End If
                Catch ex As Exception
                    exceptionmsg("General exception.", "g_0003")
                End Try
            ElseIf parameters(0) = "0x0017" Then
                Try
                    If objects.ContainsKey(parameters(1)) Then
                        objects(parameters(1)).value = My.Application.Info.Version.Minor.ToString()
                    Else

                    End If
                Catch ex As Exception
                    exceptionmsg("General exception.", "g_0003")
                End Try
            ElseIf parameters(0) = "0x0018" Then
                Try
                    If objects.ContainsKey(parameters(1)) Then
                        objects(parameters(1)).value = My.Application.Info.Version.MinorRevision.ToString()
                    Else

                    End If
                Catch ex As Exception
                    exceptionmsg("General exception.", "g_0003")
                End Try
            ElseIf parameters(0) = "0x0019" Then
                Try
                    If objects.ContainsKey(parameters(1)) Then
                        objects(parameters(1)).value = My.Application.Info.Version.ToString()
                    Else

                    End If
                Catch ex As Exception
                    exceptionmsg("General exception.", "g_0003")
                End Try
            ElseIf parameters(0) = "0x001A" Then
                exceptionmsg("The command 0x001A has been deprecated, please use the command 0x001B.", "", 1)
                Try
                    canexec = False
                    If objects.ContainsKey(parameters(2)) Then
                        tmpval2 = vppstring_to_string(objects(vppstring_to_string(parameters(2))).value).Replace(" ", "")
                    Else
                        tmpval2 = vppstring_to_string(parameters(2))
                    End If
                    If objects.ContainsKey(parameters(3)) Then
                        tmpval1 = vppstring_to_string(objects(vppstring_to_string(parameters(3))).value)
                    Else
                        tmpval1 = vppstring_to_string(parameters(3))
                    End If
                    If objects.ContainsKey(parameters(1)) Then
                        objects(parameters(1)).value = Chr(34) + webreq(tmpval2, tmpval1, "GET") + Chr(34)
                    Else

                    End If
                    canexec = True
                Catch ex As Exception
                    exceptionmsg("General exception. " + ex.Message + ex.StackTrace, "g_0003")
                End Try
            ElseIf parameters(0) = "0x001B" Then
                Try
                    canexec = False
                    If objects.ContainsKey(parameters(2)) Then
                        tmpval2 = vppstring_to_string(objects(vppstring_to_string(parameters(2))).value).Replace(" ", "")
                    Else
                        tmpval2 = vppstring_to_string(parameters(2))
                    End If
                    If objects.ContainsKey(parameters(3)) Then
                        tmpval1 = vppstring_to_string(objects(vppstring_to_string(parameters(3))).value)
                    Else
                        tmpval1 = vppstring_to_string(parameters(3))
                    End If
                    If objects.ContainsKey(parameters(1)) Then
                        objects(parameters(1)).value = Chr(34) + webreq(tmpval2, tmpval1, parameters(4)) + Chr(34)
                    Else

                    End If
                    canexec = True
                Catch ex As Exception
                    exceptionmsg("General exception. " + ex.Message + ex.StackTrace, "g_0003")
                End Try
            ElseIf parameters(0) = "0x001C" Then
                Try
                    If objects.ContainsKey(parameters(1)) Then
                        tmpval2 = vppstring_to_string(objects(vppstring_to_string(parameters(1))).value)
                    Else
                        tmpval2 = vppstring_to_string(parameters(1))
                    End If
                    If objects.ContainsKey(parameters(2)) Then
                        tmpval1 = vppstring_to_string(objects(vppstring_to_string(parameters(2))).value)
                    Else
                        tmpval1 = vppstring_to_string(parameters(2))
                    End If
                    If webheaders.ContainsKey(tmpval2) Then
                        webheaders(tmpval2) = tmpval1
                    Else
                        webheaders.Add(tmpval2, tmpval1)
                    End If
                Catch ex As Exception
                    exceptionmsg("General exception. " + ex.Message, "g_0003")
                End Try
            ElseIf parameters(0) = "0x001D" Then
                Try
                    webheaders.Clear()
                Catch ex As Exception
                    exceptionmsg("General exception. " + ex.Message, "g_0003")
                End Try
            ElseIf parameters(0) = "0x001F" Then
                Try
                    canexec = False
                    If objects.ContainsKey(parameters(1)) Then
                        My.Computer.Audio.Play(vppstring_to_string(objects(vppstring_to_string(parameters(1))).value), AudioPlayMode.Background)
                    Else
                        My.Computer.Audio.Play(vppstring_to_string(parameters(1)), AudioPlayMode.Background)
                    End If
                    canexec = True
                Catch ex As Exception
                    exceptionmsg("General exception.", "g_0003")
                End Try
            Else
                exceptionmsg("Invalid parameters given: " + Chr(34) + parameters(0) + Chr(34), "c_0001")
            End If
        Catch ex As Exception
            If TypeOf ex Is NullReferenceException Then
                exceptionmsg("Internal exception: " + ex.Message, "c_0002")
            End If
            canexec = False
            MsgBox(ex.Message + vbNewLine + ip.ToString + vbNewLine + ex.StackTrace)
            canexec = True
        End Try
    End Sub

    Public Function getvalue(name As String)
        Try
            Return objects(name).value
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "getvalue")
        End Try
    End Function

    Public Sub setvalue(name As String, value As Object, Optional _slaveint As Boolean = False)
        Try
            If _slaveint Then
                If objects(name)._private = False Then
                    If objects(name)._constant = False Then
                        objects(name).value = value
                    Else
                        exceptionmsg("Variable " + Chr(34) + name + Chr(34) + " is a constant and cannot be changed.", "d_0001")
                    End If
                Else
                    exceptionmsg("Variable " + Chr(34) + name + Chr(34) + " cannot be accessed due to it's protection level.", "d_0001")
                End If
            Else
                If objects(name)._constant = False Then
                    objects(name).value = value
                Else
                    exceptionmsg("Variable " + Chr(34) + name + Chr(34) + " is a constant and cannot be changed.", "d_0001")
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "setvalue")
        End Try
    End Sub

    Public Function getref(name As String)
        Try
            Return objects(name)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "getvalue")
        End Try
    End Function

    Public Sub setref(name As String, value As DefineObject)
        Try
            objects(name) = value
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "setvalue")
        End Try
    End Sub

    Public Function parsearguments(arguments() As String, startip As Integer)
        tmpip = 0
        Dim val As New List(Of String)
        Dim tempip = 0
        Dim tempip1 = 0
        Dim ignore = False
        If arguments.Length < startip + 1 Then
            Throw New Exception
        End If
        For Each i In arguments
            val.Add("")
            If tempip >= startip Then
                For Each i1 In i
                    If i1 = "," Then
                        If ignore = True Then
                            val(tempip1) = val(tempip1) + i1
                        Else
                            tempip1 = tempip1 + 1
                            val.Add("")
                        End If
                    ElseIf i1 = "(" Then
                        If ignore = True Then
                            val(tempip1) = val(tempip1) + i1
                        End If
                    ElseIf i1 = ")" Then
                        If ignore = True Then
                            val(tempip1) = val(tempip1) + i1
                        End If
                    ElseIf i1 = Chr(34) Then
                        If ignore = True Then
                            ignore = False
                        ElseIf ignore = False Then
                            ignore = True
                        End If
                        val(tempip1) = val(tempip1) + i1
                    Else
                        val(tempip1) = val(tempip1) + i1
                    End If
                Next
                val(tempip1) = val(tempip1) + " "
            Else

            End If
            tempip = tempip + 1
        Next
        tmpip = tempip
        Return val.ToArray()
    End Function

    ''' <summary>
    ''' Simple web request.
    ''' </summary>
    ''' <param name="requrl">URL Path</param>
    ''' <returns>Response.</returns>
    Function webreq(requrl As String)
        Dim wc As New WebClient

        For Each i As KeyValuePair(Of String, String) In webheaders
            wc.Headers.Add(i.Key, i.Value)
        Next

        Return wc.DownloadString(requrl)
    End Function

    ''' <summary>
    ''' Web request with POST.
    ''' </summary>
    ''' <param name="requrl">URL Path</param>
    ''' <param name="uploadstr">String for post request.</param>
    ''' <returns>Response.</returns>
    Function webreq(requrl As String, uploadstr As String, method As String)
        Dim wc As New WebClient

        tmpval2 = "null"

        For Each i As KeyValuePair(Of String, String) In webheaders
            wc.Headers.Add(i.Key, i.Value)
        Next

        Try
            tmpval2 = wc.UploadString(requrl, vppstring_to_string(method), uploadstr)
        Catch ex As WebException
            tmpval2 = "null"
        End Try

        Return tmpval2
    End Function

    Function _gettype(value As DefineObject)
        If value.value.Contains(Chr(34)) Then
            Return "string"
        ElseIf value.value = "true" Then
            Return "bool"
        ElseIf value.value = "false" Then
            Return "bool"
        ElseIf value.value.Remove(1) = "0" Then
            Return "int"
        ElseIf value.value.Remove(1) = "1" Then
            Return "int"
        ElseIf value.value.Remove(1) = "2" Then
            Return "int"
        ElseIf value.value.Remove(1) = "3" Then
            Return "int"
        ElseIf value.value.Remove(1) = "4" Then
            Return "int"
        ElseIf value.value.Remove(1) = "5" Then
            Return "int"
        ElseIf value.value.Remove(1) = "6" Then
            Return "int"
        ElseIf value.value.Remove(1) = "7" Then
            Return "int"
        ElseIf value.value.Remove(1) = "8" Then
            Return "int"
        ElseIf value.value.Remove(1) = "9" Then
            Return "int"
        ElseIf value.value = "null" Then
            Return "null"
        ElseIf value.value = "null" Then
            Return "null"
        Else
            Return Nothing
        End If
    End Function

    Function gettypefromval(value As String)
        If value.Contains(Chr(34)) Then
            Return "string"
        ElseIf value = "true" Then
            Return "bool"
        ElseIf value = "false" Then
            Return "bool"
        ElseIf value = "0" Then
            Return "int"
        ElseIf value = "1" Then
            Return "int"
        ElseIf value = "2" Then
            Return "int"
        ElseIf value = "3" Then
            Return "int"
        ElseIf value = "4" Then
            Return "int"
        ElseIf value = "5" Then
            Return "int"
        ElseIf value = "6" Then
            Return "int"
        ElseIf value = "7" Then
            Return "int"
        ElseIf value = "8" Then
            Return "int"
        ElseIf value = "9" Then
            Return "int"
        ElseIf value.Remove(1) = "0" Then
            Return "int"
        ElseIf value.Remove(1) = "1" Then
            Return "int"
        ElseIf value.Remove(1) = "2" Then
            Return "int"
        ElseIf value.Remove(1) = "3" Then
            Return "int"
        ElseIf value.Remove(1) = "4" Then
            Return "int"
        ElseIf value.Remove(1) = "5" Then
            Return "int"
        ElseIf value.Remove(1) = "6" Then
            Return "int"
        ElseIf value.Remove(1) = "7" Then
            Return "int"
        ElseIf value.Remove(1) = "8" Then
            Return "int"
        ElseIf value.Remove(1) = "9" Then
            Return "int"
        ElseIf value = "null" Then
            Return "null"
        Else
            Return Nothing
        End If
    End Function

    Function vppstring_to_string(_value As String, Optional modify As Boolean = True) As String
        Static vpps_tmpval
        vpps_tmpval = ""
        tmpval = ""
        canexec = False
        For Each i In _value
            If i = Chr(34) Then
                If tmpval = "\\" Then
                    If modify = False Then
                        vpps_tmpval = vpps_tmpval + "\\" + Chr(34)
                    ElseIf modify = True Then
                        vpps_tmpval = vpps_tmpval + Chr(34)
                        tmpval = ""
                    End If
                Else

                End If
            ElseIf i = "\" Then
                If modify = False Then
                    vpps_tmpval = vpps_tmpval + "\"
                ElseIf modify = True Then
                    If tmpval = "" Then
                        tmpval = "\"
                    ElseIf tmpval = "\" Then
                        tmpval = "\\"
                    ElseIf tmpval = "\\" Then
                        vpps_tmpval = vpps_tmpval + "\"
                    End If
                End If
            Else
                If i = "n" Then
                    If modify = False Then
                        vpps_tmpval = vpps_tmpval + "n"
                    ElseIf modify = True Then
                        If tmpval = "\\" Then
                            vpps_tmpval = vpps_tmpval + vbNewLine
                            tmpval = ""
                        Else
                            vpps_tmpval = vpps_tmpval + i
                        End If
                    End If
                Else
                    vpps_tmpval = vpps_tmpval + i
                End If
            End If
        Next
        canexec = True
        tmpval = Nothing
        Return vpps_tmpval
    End Function

    Function string_to_bool(_value As String) As Boolean
        If _value = "true" Then
            Return True
        ElseIf _value = "false" Then
            Return False
        End If
    End Function

    Function bool_to_string(_value As Boolean) As String
        If _value = True Then
            Return "true"
        ElseIf _value = False Then
            Return "false"
        End If
    End Function

    Function stringa_to_string(_value As String(), Optional startip As Integer = 0) As String
        Static tmpval_stra
        tmpval_stra = ""
        tmpip = 0
        For Each i In _value
            If tmpip >= startip Then
                tmpval_stra += i + " "
            End If
            tmpip += 1
        Next
        Return tmpval_stra
    End Function

    Function conditionval()

    End Function

    Sub log(logmsg As String)
        If slave = False Then
            logfile.WriteLine(logmsg)
        End If
    End Sub


#End Region

End Class
