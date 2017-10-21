Public Class libman
    'holy fucking shit im getting a lil bit sober from last night and im already forcing myself to write this.
    Dim locallib As String = My.Application.Info.DirectoryPath + "\aics_files\Include"
    Dim instlib As String = "C:\Program Files (x86)\AutoIt3\Include"
    Dim count_local, count_installed, count_new, count_compared, count_sync As Integer
    Dim sync_new(800) As String

    Private Sub libman_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If FileIO.FileSystem.DirectoryExists(instlib) = False Then
            instlib = ""
            Dim b = MsgBox("Default intallation of AutoIt Include libraries can't be found. Would you like to browse for it?", MsgBoxStyle.YesNo)
            If b = vbYes Then
                fbd_instlib.ShowDialog()
                instlib = fbd_instlib.SelectedPath
                TextBox2.Text = instlib
            ElseIf b = vbNo Then
            End If
        Else
            TextBox2.Text = instlib
        End If

        If FileIO.FileSystem.DirectoryExists(locallib) = False Then
            Dim a = MsgBox("Local Library path was not included or is missing!" + vbNewLine + locallib + vbNewLine + vbNewLine + "Would you to synchronize all of your installed library to the local library?", MsgBoxStyle.YesNo, "Local Library Includes Missing.")
            If a = vbYes Then
                If FileIO.FileSystem.DirectoryExists(instlib) = False Then
                    MsgBox("AutoIt Installation directory for Include libraries cannot be found.")
                Else
                    FileIO.FileSystem.CopyDirectory(instlib, locallib)
                    MsgBox("Copy finished!")
                End If
            ElseIf a = vbNo Then
                MsgBox("i have no idea what you're doing but im pretty sure you'll have a hard time compiling things.", 0, "Good Luck.")
            End If
        Else
            TextBox1.Text = locallib
        End If
        Call rfrsh_i()
        Call rfrsh_l()
    End Sub

    Private Sub rfrsh_l()
        count_local = 0
        ListBox1.Items.Clear()
        If FileIO.FileSystem.DirectoryExists(locallib) Then
            For Each a In FileIO.FileSystem.GetFiles(locallib, FileIO.SearchOption.SearchTopLevelOnly, "*.au3")
                ListBox1.Items.Add(Replace(a, locallib + "\", ""))
                count_local += 1
                Call synctxt()
            Next
        Else
            MsgBox("Local Library Directory Un-Identifiable.")
        End If
    End Sub

    Private Sub rfrsh_i()
        count_installed = 0
        ListBox2.Items.Clear()
        If FileIO.FileSystem.DirectoryExists(instlib) Then
            For Each a In FileIO.FileSystem.GetFiles(instlib, FileIO.SearchOption.SearchTopLevelOnly, "*.au3")
                ListBox2.Items.Add(Replace(a, instlib + "\", ""))
                count_installed += 1
                Call synctxt()
            Next
        Else
            MsgBox("Installed Library Directory Un-Identifiable.")
        End If
    End Sub

    Private Sub chksync()
        count_compared = 0
        count_new = 0
        For Each a In ListBox2.Items
            count_compared += 1
            Call synctxt()
            If FileIO.FileSystem.FileExists(locallib + "\" + a) = False Then
                FileIO.FileSystem.CopyFile(instlib + "\" + a, locallib + "\" + a)
                ListBox3.Items.Add(a)
                count_new += 1
                Call synctxt()
            End If
        Next
        MsgBox("Synchronize is done!")
        Call rfrsh_i()
        Call rfrsh_l()
    End Sub

    Private Sub synctxt()
        Label4.Text = "New UDF: " + count_new.ToString
        Label5.Text = "Compared UDF: " + count_compared.ToString
        Label6.Text = "Local UDF: " + count_local.ToString
        Label7.Text = "Installed UDF: " + count_installed.ToString
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Call chksync()
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
        Me.Hide()
        Form1.CheckBox6.Checked = False
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Call rfrsh_i()
        Call rfrsh_l()
    End Sub

    Private Sub TextBox1_Click(sender As Object, e As EventArgs) Handles TextBox1.Click
        If FileIO.FileSystem.DirectoryExists(TextBox1.Text) Then
            Process.Start(TextBox1.Text)
        End If
    End Sub

    Private Sub TextBox2_Click(sender As Object, e As EventArgs) Handles TextBox2.Click
        If FileIO.FileSystem.DirectoryExists(TextBox2.Text) Then
            Process.Start(TextBox2.Text)
        End If
    End Sub
End Class