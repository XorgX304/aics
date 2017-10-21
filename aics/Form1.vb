
'08/10/2017 (DD/MM/YYYY)

Public Class Form1

    Dim rnhndlr, scrcomp As New ProcessStartInfo()
    Dim dbghndlr As New Process()
    Dim rndG As New Random()
    Dim cri_running, cri_handler, cri_pid As String
    Dim file_ext, str_inp, str_out As String
    Dim flpth As String = My.Application.Info.DirectoryPath + "\aics_files"
    Dim lol As Integer = 0
    Dim q As String = """"
    Dim dbghndlrnm As String = ""
    Dim warnonce_a As String

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'fun.Start()
        ComboBox1.SelectedItem = ComboBox1.Items(0)
    End Sub

    Private Sub dbgfrcstop()
        Try
            Dim taskkill As New ProcessStartInfo()
            taskkill.FileName = "cmd.exe"
            taskkill.WindowStyle = ProcessWindowStyle.Hidden
            taskkill.Arguments = "/k taskkill /f /pid " + dbghndlr.Id.ToString + "&exit"
            Process.Start(taskkill)
            If CheckBox7.Checked = False Then
                MsgBox("Force kill of Process ID commands were successfuly sent to 'taskkill.exe'!", MsgBoxStyle.Information)
            End If
        Catch
            If CheckBox7.Checked = False Then
                MsgBox("Sending force kill Process ID commands to 'taskkill.exe' failed miserably like my parents when they were raising me.")
            End If
        End Try
    End Sub

    Private Sub dbgstop()
        Try
            If dbghndlr.Id > -1 Then
                dbghndlr.Kill()
                If CheckBox7.Checked = False Then
                    MsgBox("Running script has been stopped.")
                End If
            End If
        Catch
            If CheckBox7.Checked = False Then
                MsgBox("Can't stop running compiled script.")
            End If
        End Try
    End Sub

    Private Sub compilescript()
        Try
            If FileIO.FileSystem.FileExists(flpth + "\$temp.a3x") = True Then
                FileIO.FileSystem.DeleteFile(flpth + "\$temp.a3x")
            End If

            If FileIO.FileSystem.FileExists(flpth + "\$temp.exe") = True Then
                FileIO.FileSystem.DeleteFile(flpth + "\$temp.exe")
            End If
        Catch
            MsgBox("Someone or something is using / accessing the temporary files associatied with this application and is causing a problem that dis-allows deletion / clean up of temporary files.", MsgBoxStyle.Critical, "Clean up error | Exiting application | 0x0")
            Me.Close()
        End Try

        Dim compressratio = Split(ComboBox1.SelectedItem, " ")
        scrcomp.FileName = flpth + "\aic.exe"

        If RadioButton1.Checked = True Then ' Compile as EXE

            scrcomp.Arguments = "/in " + q + ofd_script.FileName + q + " /out " + q + flpth + "\$temp.exe" + q + " /comp " + compressratio(1)
            If CheckBox1.Checked = True Then
                scrcomp.Arguments = scrcomp.Arguments + " /console"
            End If

            If dbgmd.Checked = False Then ' skip icon if in debug mode
                If TextBox3.Text IsNot "" Then
                    scrcomp.Arguments = scrcomp.Arguments + " /icon " + q + ofd_ico.FileName + q
                Else
                    scrcomp.Arguments = scrcomp.Arguments + " /icon " + flpth + "\default.ico"
                End If
            Else
                scrcomp.Arguments = scrcomp.Arguments + " /icon " + flpth + "\default.ico"
            End If

            If RadioButton3.Checked = True Then
                    scrcomp.Arguments = scrcomp.Arguments + " /x86"
                ElseIf RadioButton4.Checked = True Then
                    scrcomp.Arguments = scrcomp.Arguments + " /x64"
                End If

            If dbgmd.Checked = False Then ' skip UPX compression if in debug mode
                If CheckBox8.Checked = True Then
                    scrcomp.Arguments = scrcomp.Arguments + " /pack"
                Else
                    scrcomp.Arguments = scrcomp.Arguments + " /nopack"
                End If
            End If

            Dim procsure As Process = Process.Start(scrcomp)
                procsure.WaitForExit()
                If FileIO.FileSystem.FileExists(flpth + "\$temp.exe") Then
                    Try
                        If FileIO.FileSystem.FileExists(sfd_out.FileName + ".exe") = True Then
                            FileIO.FileSystem.DeleteFile(sfd_out.FileName + ".exe")
                        End If
                        FileIO.FileSystem.MoveFile(flpth + "\$temp.exe", sfd_out.FileName + ".exe")
                    If CheckBox4.Checked = False Then
                        MsgBox("Compile Successful!" + vbNewLine + vbNewLine + sfd_out.FileName + ".exe", MsgBoxStyle.Information)
                    End If

                    If dbgmd.Checked = True Then
                        rnhndlr.FileName = sfd_out.FileName + ".exe"
                        rnhndlr.Arguments = execpar.TextBox1.Text
                        dbghndlr = Process.Start(rnhndlr)
                        procinf.TextBox1.Text = rnhndlr.FileName
                        procinf.Label8.Text = ": " + dbghndlr.Id.ToString
                        runchk.Start()
                    End If

                Catch
                        If FileIO.FileSystem.FileExists(flpth + "\$temp.exe") = True Then
                            FileIO.FileSystem.DeleteFile(flpth + "\$temp.exe")
                        End If
                        MsgBox("Compile was successful but the previous compiled script was unable to be overwritten.", MsgBoxStyle.Critical)
                    End Try
                Else
                    MsgBox("Compile Unsuccessful" + vbNewLine + vbNewLine + "The Auto It compiler failed to compile your script.", MsgBoxStyle.Critical)
                End If


        ElseIf RadioButton2.Checked = True Then ' Compile as A3X
                scrcomp.Arguments = "/in " + q + ofd_script.FileName + q + " /out " + q + flpth + "\$temp.a3x" + q + " /comp " + compressratio(1)
            Dim procsure As Process = Process.Start(scrcomp)
            procsure.WaitForExit()
            If FileIO.FileSystem.FileExists(flpth + "\$temp.a3x") Then
                'THIS WHOLE ALGORITHM IS HERE BECAUSE I THOUGHT THAT A3X ARE TREATED LIKE EXECUTABLES WHICH CANT BE MODIFIED WHILE USED / RUNNING!
                Try
                    If FileIO.FileSystem.FileExists(sfd_out.FileName + ".a3x") = True Then
                        FileIO.FileSystem.DeleteFile(sfd_out.FileName + ".a3x")
                    End If
                    FileIO.FileSystem.MoveFile(flpth + "\$temp.a3x", sfd_out.FileName + ".a3x")
                    If CheckBox4.Checked = False Then
                        MsgBox("Compile Successful!" + vbNewLine + vbNewLine + sfd_out.FileName + ".a3x", MsgBoxStyle.Information)
                    End If

                    If dbgmd.Checked = True Then
                        rnhndlr.FileName = flpth + "\aie.exe"
                        rnhndlr.Arguments = q + sfd_out.FileName + ".a3x" + q
                        dbghndlr = Process.Start(rnhndlr)
                        procinf.TextBox1.Text = rnhndlr.FileName
                        procinf.Label8.Text = ": " + dbghndlr.Id.ToString
                        runchk.Start()
                    End If

                Catch
                    If FileIO.FileSystem.FileExists(flpth + "\$temp.a3x") = True Then
                        FileIO.FileSystem.DeleteFile(flpth + "\$temp.a3x")
                    End If
                    MsgBox("Compile was successful but the previous compiled script was unable to be overwritten.", MsgBoxStyle.Critical)
                End Try
                'END OF DUMB ASS CODE.
            Else
                MsgBox("Compile Unsuccessful" + vbNewLine + vbNewLine + "The Auto It compiler failed to compile your script.", MsgBoxStyle.Critical)
            End If

        End If
    End Sub

    Private Sub fun_Tick(sender As Object, e As EventArgs) Handles fun.Tick
        Label4.ForeColor = Color.FromArgb(rndG.Next(0, 255), rndG.Next(0, 255), rndG.Next(0, 255))
    End Sub

    Private Sub getfn(file_ext, str_inp)
        Dim a = Split(str_inp, ".")
        Dim a2 As String = ""
        For Each a1 In a
            If a1 = file_ext Then
            Else
                If a2 = "" Then
                    a2 = a1
                Else
                    a2 = a2 + "." + a1
                End If
            End If
        Next
        str_out = a2
    End Sub

    Private Sub getfp(str_inp)
        Dim a = Split(str_inp, "\")
        Dim b = Replace(str_inp, "\" + a(a.Count - 1).ToString, "")
        str_out = b
    End Sub

    Private Sub getfnp(str_inp)
        Dim a = Split(str_inp, "\")
        str_out = a(a.Count - 1)
    End Sub

    Private Sub seloutexe()
        Label2.Text = "Output (*.exe):"
        GroupBox4.Enabled = True
        GroupBox6.Enabled = True
        GroupBox7.Enabled = True
    End Sub

    Private Sub selouta3x()
        Label2.Text = "Output (*.a3x):"
        GroupBox4.Enabled = False
        GroupBox6.Enabled = False
        GroupBox7.Enabled = False
    End Sub

    Private Sub FlatClose1_Click(sender As Object, e As EventArgs) Handles FlatClose1.Click
        Me.Close()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ofd_script.ShowDialog()
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        Call seloutexe()
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        Call selouta3x()
        rnhndlr.FileName = flpth + "\aie.exe"
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If TextBox2.Text = "" Or TextBox2.Text = "" Then
            MsgBox("No out/input file configured. Please enter an out/input file.", MsgBoxStyle.Critical)
        Else
            Call compilescript()
        End If
    End Sub

    Private Sub CheckBox5_CheckedChanged(sender As Object, e As EventArgs) Handles dbgmd.CheckedChanged
        If TextBox1.Text IsNot "" And TextBox2.Text IsNot "" Then
            If warnonce_a = "" Then
                MsgBox("Debugger mode has been enabled!" + vbNewLine + vbNewLine + "Please note that when debugger mode is enabled the following things will be skipped when compiling the script:" + vbNewLine + "- UPX Compression" + vbNewLine + "- Icons" + vbNewLine + vbNewLine + "If you want this compiler to synergize with your (Text) Editor of choice just check the 'Simple UI Debugger' on the top left. ", MsgBoxStyle.Information, "Notice")
            End If
            If dbgmd.Checked = True Then
                Button4.Text = "COMPILE AND RUN"
                Button5.Enabled = True
                Button6.Enabled = True
                execpar.Show()
            Else
                Button5.Enabled = False
                Button6.Enabled = False
                Button4.Text = "COMPILE"
                execpar.Hide()
            End If
        Else
            If dbgmd.Checked = True Then
                dbgmd.Checked = False
                MsgBox("Atleast enter something before we do this shall we?")
            End If
        End If
    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click
        Process.Start("https://tragenalpha.github.io")
    End Sub

    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click
        lol += 1
        If lol = 3 Then
            MsgBox("i swear to god stop. no boops.")
        ElseIf lol = 4 Then
            MsgBox("i swear to god stop. NO. BOOPS.")
        ElseIf lol >= 5 Then
            MsgBox("i said no fucking boops you fucking shit for brains did your mom drop you when you were a baby?")
            If lol = 10 Then
                lol = 0
            End If
        Else
            MsgBox("no boops.")
        End If
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.Checked = True Then
            procinf.Show()
        Else
            procinf.Hide()
        End If
    End Sub

    Private Sub sfd_out_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles sfd_out.FileOk
        Call getfnp(sfd_out.FileName)
        Call getfn("au3", str_out)
        Call getfn("exe", str_out)
        Call getfn("a3x", str_out)
        TextBox2.Text = str_out
        Call getfp(sfd_out.FileName.ToString)
        sfd_out.FileName = str_out + "\" + TextBox2.Text
        pathinf.o.Text = sfd_out.FileName
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If Button3.Text = "..." Then
            ofd_ico.ShowDialog()
        Else
            Button3.Text = "..."
            TextBox3.Text = ""
            PictureBox2.ImageLocation = ""
        End If
    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click
        Process.Start("https://www.autoitscript.com/autoit3/docs/intro/compiler.htm")
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Call dbgstop()
    End Sub

    Private Sub ofd_ico_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ofd_ico.FileOk
        TextBox3.Text = ofd_ico.SafeFileName
        PictureBox2.ImageLocation = ofd_ico.FileName
        Button3.Text = "✖"
    End Sub

    Private Sub ofd_script_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ofd_script.FileOk
        TextBox1.Text = ofd_script.SafeFileName
        Call getfn("au3", TextBox1.Text)
        TextBox2.Text = str_out
        Call getfp(ofd_script.FileName)
        sfd_out.FileName = str_out + "\" + TextBox2.Text
        str_out = ""
        pathinf.s.Text = ofd_script.FileName
        pathinf.o.Text = sfd_out.FileName
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Call dbgfrcstop()
    End Sub

    Private Sub runchk_Tick(sender As Object, e As EventArgs) Handles runchk.Tick
        Try
            If dbghndlrnm = "" Then
                dbghndlrnm = Process.GetProcessById(dbghndlr.Id.ToString).ProcessName.ToString
            End If
            If Process.GetProcessesByName(dbghndlrnm).Count > 0 Then
                procinf.Label6.Text = ": Running."
            Else
                procinf.Label6.Text = ": Dead."
                runchk.Stop()
                dbghndlrnm = ""
            End If
        Catch
            procinf.Label6.Text = ": Fetching process state error."
            runchk.Stop()
            dbghndlrnm = ""
        End Try
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        If CheckBox2.Checked = True Then
            Me.TopMost = True
            procinf.TopMost = True
            execpar.TopMost = True
        Else
            Me.TopMost = False
            procinf.TopMost = False
            execpar.TopMost = False
        End If
    End Sub

    Private Sub CheckBox6_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox6.CheckedChanged
        If CheckBox6.Checked = True Then
            libman.Show()
        Else
            libman.Hide()
        End If
    End Sub

    Private Sub CheckBox5_CheckedChanged_1(sender As Object, e As EventArgs) Handles CheckBox5.CheckedChanged
        If TextBox1.Text IsNot "" And TextBox2.Text IsNot "" Then
            If CheckBox5.Checked = True Then
                GroupBox9.Location = New Point(12, 57)
                PictureBox3.Location = New Point(270, 50)
                CheckBox5.Location = New Point(155, 77)
                CheckBox2.Location = New Point(155, 97)
                CheckBox5.ForeColor = Color.FromArgb(0, 0, 0)
                CheckBox2.ForeColor = Color.FromArgb(0, 0, 0)
                dbgmd.Checked = True
                dbgmd.Enabled = False
                CheckBox3.Checked = True
                Button4.Text = "►"
                Button5.Text = "✖"
                Button6.Text = "■"
                Button4.Size = New Size(33, 29)
                Button5.Size = New Size(33, 29)
                Button6.Size = New Size(33, 29)
                Button4.Location = New Point(89, 22)
                Button5.Location = New Point(11, 22)
                Button6.Location = New Point(50, 22)
                GroupBox9.Width = 138
                GroupBox1.Hide()
                GroupBox3.Hide()
                Me.Width = 370
                Me.Height = 126
                Dim a1 As New Point()
                a1 = Me.Location
                procinf.Location = New Point(a1.X + 0, a1.Y + 127)
                execpar.Location = New Point(a1.X + 356, a1.Y + 127)
            Else
                GroupBox9.Location = New Point(12, 302)
                PictureBox3.Location = New Point(564, 297)
                CheckBox5.Location = New Point(416, 32)
                CheckBox2.Location = New Point(523, 32)
                CheckBox5.ForeColor = Color.FromArgb(255, 255, 255)
                CheckBox2.ForeColor = Color.FromArgb(255, 255, 255)
                dbgmd.Enabled = True
                Button4.Text = "COMPILE AND RUN"
                Button5.Text = "KILL (FORCE)"
                Button6.Text = "STOP"
                Button4.Size = New Size(173, 29)
                Button5.Size = New Size(116, 29)
                Button6.Size = New Size(94, 29)
                Button4.Location = New Point(233, 22)
                Button5.Location = New Point(11, 22)
                Button6.Location = New Point(133, 22)
                GroupBox9.Width = 546
                GroupBox1.Show()
                GroupBox3.Show()
                Me.Width = 626
                Me.Height = 384
            End If
        Else
            If CheckBox5.Checked = True Then
                CheckBox5.Checked = False
                MsgBox("Entering Simple UI Debugger enables Debugger mode by default and I see you imported nothing. Atleast enter something before we begin, shall we?", MsgBoxStyle.Information, "Notice")
            End If
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If TextBox1.Text = "" Then
            MsgBox("Please import a script first.")
        Else
            sfd_out.ShowDialog()
        End If
    End Sub

    Private Sub TextBox1_Click(sender As Object, e As EventArgs) Handles TextBox1.Click
        If TextBox1.Text IsNot "" Then
            Try
                Process.Start(Replace(ofd_script.FileName, TextBox1.Text, ""))
            Catch
                MsgBox("Cannot open script directory.")
            End Try
        End If
    End Sub

    Private Sub TextBox2_Click(sender As Object, e As EventArgs) Handles TextBox2.Click
        If TextBox2.Text IsNot "" Then
            Try
                Process.Start(Replace(sfd_out.FileName, TextBox2.Text, ""))
            Catch
                MsgBox("Cannot open output directory.")
            End Try
        End If
    End Sub

    Private Sub TextBox3_Click(sender As Object, e As EventArgs) Handles TextBox3.Click
        If TextBox3.Text IsNot "" Then
            Try
                Process.Start(Replace(ofd_ico.FileName, TextBox3.Text, ""))
            Catch
                MsgBox("Cannot open icon directory.")
            End Try
        End If
    End Sub
End Class
