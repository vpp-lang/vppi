﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.CancelBtn = New System.Windows.Forms.Button()
        Me.NextBtn = New System.Windows.Forms.Button()
        Me.BackBtn = New System.Windows.Forms.Button()
        Me.MainPanel = New System.Windows.Forms.Panel()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.WelcomePage = New System.Windows.Forms.TabPage()
        Me.VIALabel2 = New System.Windows.Forms.Label()
        Me.VAILabel = New System.Windows.Forms.Label()
        Me.VAICombo = New System.Windows.Forms.ComboBox()
        Me.WelcomePageLabel2 = New System.Windows.Forms.Label()
        Me.WelcomePageLabel1 = New System.Windows.Forms.Label()
        Me.InstallationPage = New System.Windows.Forms.TabPage()
        Me.LegacyPath = New System.Windows.Forms.CheckBox()
        Me.InstallationPageChk3 = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.InstallationPageChk2 = New System.Windows.Forms.CheckBox()
        Me.InstallationPageChk1 = New System.Windows.Forms.CheckBox()
        Me.InstallationPageLabel2 = New System.Windows.Forms.Label()
        Me.InstallationPageLabel1 = New System.Windows.Forms.Label()
        Me.ProgressPage = New System.Windows.Forms.TabPage()
        Me.ProgressPageText = New System.Windows.Forms.Label()
        Me.ProgressPagePB = New System.Windows.Forms.ProgressBar()
        Me.ProgressPageTitle = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.MainPanel.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.WelcomePage.SuspendLayout()
        Me.InstallationPage.SuspendLayout()
        Me.ProgressPage.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'CancelBtn
        '
        Me.CancelBtn.Location = New System.Drawing.Point(609, 469)
        Me.CancelBtn.Name = "CancelBtn"
        Me.CancelBtn.Size = New System.Drawing.Size(75, 23)
        Me.CancelBtn.TabIndex = 0
        Me.CancelBtn.Text = "Cancel"
        Me.CancelBtn.UseVisualStyleBackColor = True
        '
        'NextBtn
        '
        Me.NextBtn.Location = New System.Drawing.Point(499, 469)
        Me.NextBtn.Name = "NextBtn"
        Me.NextBtn.Size = New System.Drawing.Size(75, 23)
        Me.NextBtn.TabIndex = 1
        Me.NextBtn.Text = "Next"
        Me.NextBtn.UseVisualStyleBackColor = True
        '
        'BackBtn
        '
        Me.BackBtn.Location = New System.Drawing.Point(418, 469)
        Me.BackBtn.Name = "BackBtn"
        Me.BackBtn.Size = New System.Drawing.Size(75, 23)
        Me.BackBtn.TabIndex = 2
        Me.BackBtn.Text = "Back"
        Me.BackBtn.UseVisualStyleBackColor = True
        '
        'MainPanel
        '
        Me.MainPanel.Controls.Add(Me.TabControl1)
        Me.MainPanel.Controls.Add(Me.Panel1)
        Me.MainPanel.Location = New System.Drawing.Point(0, 0)
        Me.MainPanel.Name = "MainPanel"
        Me.MainPanel.Size = New System.Drawing.Size(696, 463)
        Me.MainPanel.TabIndex = 3
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.WelcomePage)
        Me.TabControl1.Controls.Add(Me.InstallationPage)
        Me.TabControl1.Controls.Add(Me.ProgressPage)
        Me.TabControl1.Location = New System.Drawing.Point(154, 4)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(539, 456)
        Me.TabControl1.TabIndex = 1
        '
        'WelcomePage
        '
        Me.WelcomePage.Controls.Add(Me.VIALabel2)
        Me.WelcomePage.Controls.Add(Me.VAILabel)
        Me.WelcomePage.Controls.Add(Me.VAICombo)
        Me.WelcomePage.Controls.Add(Me.WelcomePageLabel2)
        Me.WelcomePage.Controls.Add(Me.WelcomePageLabel1)
        Me.WelcomePage.Location = New System.Drawing.Point(4, 22)
        Me.WelcomePage.Name = "WelcomePage"
        Me.WelcomePage.Padding = New System.Windows.Forms.Padding(3)
        Me.WelcomePage.Size = New System.Drawing.Size(531, 430)
        Me.WelcomePage.TabIndex = 0
        Me.WelcomePage.Text = "Welcome page"
        Me.WelcomePage.UseVisualStyleBackColor = True
        '
        'VIALabel2
        '
        Me.VIALabel2.AutoSize = True
        Me.VIALabel2.Location = New System.Drawing.Point(6, 414)
        Me.VIALabel2.Name = "VIALabel2"
        Me.VIALabel2.Size = New System.Drawing.Size(300, 13)
        Me.VIALabel2.TabIndex = 5
        Me.VIALabel2.Text = "This is a prerelease. A few feautures of V++ might be disabled."
        '
        'VAILabel
        '
        Me.VAILabel.AutoSize = True
        Me.VAILabel.Location = New System.Drawing.Point(13, 94)
        Me.VAILabel.Name = "VAILabel"
        Me.VAILabel.Size = New System.Drawing.Size(194, 13)
        Me.VAILabel.TabIndex = 4
        Me.VAILabel.Text = "V++ is already installed. Choose action: "
        '
        'VAICombo
        '
        Me.VAICombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.VAICombo.FormattingEnabled = True
        Me.VAICombo.Items.AddRange(New Object() {"Update", "Uninstall"})
        Me.VAICombo.Location = New System.Drawing.Point(213, 91)
        Me.VAICombo.Name = "VAICombo"
        Me.VAICombo.Size = New System.Drawing.Size(312, 21)
        Me.VAICombo.TabIndex = 3
        '
        'WelcomePageLabel2
        '
        Me.WelcomePageLabel2.AutoSize = True
        Me.WelcomePageLabel2.Location = New System.Drawing.Point(13, 42)
        Me.WelcomePageLabel2.Name = "WelcomePageLabel2"
        Me.WelcomePageLabel2.Size = New System.Drawing.Size(479, 26)
        Me.WelcomePageLabel2.TabIndex = 1
        Me.WelcomePageLabel2.Text = "V++ is a small, open-source, language made for background apps with little to no " &
    "input from the user," & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "but it can be used for other stuff too." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'WelcomePageLabel1
        '
        Me.WelcomePageLabel1.AutoSize = True
        Me.WelcomePageLabel1.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!)
        Me.WelcomePageLabel1.Location = New System.Drawing.Point(7, 7)
        Me.WelcomePageLabel1.Name = "WelcomePageLabel1"
        Me.WelcomePageLabel1.Size = New System.Drawing.Size(470, 31)
        Me.WelcomePageLabel1.TabIndex = 0
        Me.WelcomePageLabel1.Text = "Welcome to the V++ instalation setup!"
        '
        'InstallationPage
        '
        Me.InstallationPage.Controls.Add(Me.LegacyPath)
        Me.InstallationPage.Controls.Add(Me.InstallationPageChk3)
        Me.InstallationPage.Controls.Add(Me.Label1)
        Me.InstallationPage.Controls.Add(Me.InstallationPageChk2)
        Me.InstallationPage.Controls.Add(Me.InstallationPageChk1)
        Me.InstallationPage.Controls.Add(Me.InstallationPageLabel2)
        Me.InstallationPage.Controls.Add(Me.InstallationPageLabel1)
        Me.InstallationPage.Location = New System.Drawing.Point(4, 22)
        Me.InstallationPage.Name = "InstallationPage"
        Me.InstallationPage.Padding = New System.Windows.Forms.Padding(3)
        Me.InstallationPage.Size = New System.Drawing.Size(531, 430)
        Me.InstallationPage.TabIndex = 1
        Me.InstallationPage.Text = "Collecting data"
        Me.InstallationPage.UseVisualStyleBackColor = True
        '
        'LegacyPath
        '
        Me.LegacyPath.AutoSize = True
        Me.LegacyPath.Enabled = False
        Me.LegacyPath.Location = New System.Drawing.Point(12, 137)
        Me.LegacyPath.Name = "LegacyPath"
        Me.LegacyPath.Size = New System.Drawing.Size(137, 17)
        Me.LegacyPath.TabIndex = 8
        Me.LegacyPath.Text = "Legacy installation path"
        Me.LegacyPath.UseVisualStyleBackColor = True
        '
        'InstallationPageChk3
        '
        Me.InstallationPageChk3.AutoSize = True
        Me.InstallationPageChk3.Location = New System.Drawing.Point(12, 114)
        Me.InstallationPageChk3.Name = "InstallationPageChk3"
        Me.InstallationPageChk3.Size = New System.Drawing.Size(246, 17)
        Me.InstallationPageChk3.TabIndex = 7
        Me.InstallationPageChk3.Text = "Create a shortcut for interpreter on the desktop"
        Me.InstallationPageChk3.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 403)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(26, 13)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "V++"
        '
        'InstallationPageChk2
        '
        Me.InstallationPageChk2.AutoSize = True
        Me.InstallationPageChk2.Location = New System.Drawing.Point(12, 91)
        Me.InstallationPageChk2.Name = "InstallationPageChk2"
        Me.InstallationPageChk2.Size = New System.Drawing.Size(281, 17)
        Me.InstallationPageChk2.TabIndex = 5
        Me.InstallationPageChk2.Text = "Create a shortcut for the settings app in the start menu" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.InstallationPageChk2.UseVisualStyleBackColor = True
        '
        'InstallationPageChk1
        '
        Me.InstallationPageChk1.AutoSize = True
        Me.InstallationPageChk1.Location = New System.Drawing.Point(12, 68)
        Me.InstallationPageChk1.Name = "InstallationPageChk1"
        Me.InstallationPageChk1.Size = New System.Drawing.Size(111, 17)
        Me.InstallationPageChk1.TabIndex = 4
        Me.InstallationPageChk1.Text = "Add V++ to PATH"
        Me.InstallationPageChk1.UseVisualStyleBackColor = True
        '
        'InstallationPageLabel2
        '
        Me.InstallationPageLabel2.AutoSize = True
        Me.InstallationPageLabel2.Location = New System.Drawing.Point(12, 38)
        Me.InstallationPageLabel2.Name = "InstallationPageLabel2"
        Me.InstallationPageLabel2.Size = New System.Drawing.Size(442, 13)
        Me.InstallationPageLabel2.TabIndex = 3
        Me.InstallationPageLabel2.Text = "Here you will ""set up"" V++, you can chose different settings. After you decided, " &
    "click ""Next""."
        '
        'InstallationPageLabel1
        '
        Me.InstallationPageLabel1.AutoSize = True
        Me.InstallationPageLabel1.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!)
        Me.InstallationPageLabel1.Location = New System.Drawing.Point(6, 3)
        Me.InstallationPageLabel1.Name = "InstallationPageLabel1"
        Me.InstallationPageLabel1.Size = New System.Drawing.Size(194, 31)
        Me.InstallationPageLabel1.TabIndex = 2
        Me.InstallationPageLabel1.Text = "Collecting data"
        '
        'ProgressPage
        '
        Me.ProgressPage.Controls.Add(Me.ProgressPageText)
        Me.ProgressPage.Controls.Add(Me.ProgressPagePB)
        Me.ProgressPage.Controls.Add(Me.ProgressPageTitle)
        Me.ProgressPage.Location = New System.Drawing.Point(4, 22)
        Me.ProgressPage.Name = "ProgressPage"
        Me.ProgressPage.Size = New System.Drawing.Size(531, 430)
        Me.ProgressPage.TabIndex = 2
        Me.ProgressPage.Text = "Processing"
        Me.ProgressPage.UseVisualStyleBackColor = True
        '
        'ProgressPageText
        '
        Me.ProgressPageText.AutoSize = True
        Me.ProgressPageText.Location = New System.Drawing.Point(6, 35)
        Me.ProgressPageText.Name = "ProgressPageText"
        Me.ProgressPageText.Size = New System.Drawing.Size(61, 13)
        Me.ProgressPageText.TabIndex = 5
        Me.ProgressPageText.Text = "Preparing..."
        '
        'ProgressPagePB
        '
        Me.ProgressPagePB.Location = New System.Drawing.Point(9, 51)
        Me.ProgressPagePB.Name = "ProgressPagePB"
        Me.ProgressPagePB.Size = New System.Drawing.Size(501, 23)
        Me.ProgressPagePB.Style = System.Windows.Forms.ProgressBarStyle.Marquee
        Me.ProgressPagePB.TabIndex = 4
        '
        'ProgressPageTitle
        '
        Me.ProgressPageTitle.AutoSize = True
        Me.ProgressPageTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!)
        Me.ProgressPageTitle.Location = New System.Drawing.Point(3, 0)
        Me.ProgressPageTitle.Name = "ProgressPageTitle"
        Me.ProgressPageTitle.Size = New System.Drawing.Size(177, 31)
        Me.ProgressPageTitle.TabIndex = 3
        Me.ProgressPageTitle.Text = "Please wait..."
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.SteelBlue
        Me.Panel1.Controls.Add(Me.PictureBox2)
        Me.Panel1.Location = New System.Drawing.Point(3, 3)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(145, 457)
        Me.Panel1.TabIndex = 0
        '
        'PictureBox2
        '
        Me.PictureBox2.BackColor = System.Drawing.Color.Transparent
        Me.PictureBox2.Image = Global.vppsetup.My.Resources.Resources.vppicon
        Me.PictureBox2.InitialImage = Nothing
        Me.PictureBox2.Location = New System.Drawing.Point(9, 9)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(125, 125)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox2.TabIndex = 1
        Me.PictureBox2.TabStop = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(696, 507)
        Me.Controls.Add(Me.MainPanel)
        Me.Controls.Add(Me.BackBtn)
        Me.Controls.Add(Me.NextBtn)
        Me.Controls.Add(Me.CancelBtn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "V++ Setup"
        Me.MainPanel.ResumeLayout(False)
        Me.TabControl1.ResumeLayout(False)
        Me.WelcomePage.ResumeLayout(False)
        Me.WelcomePage.PerformLayout()
        Me.InstallationPage.ResumeLayout(False)
        Me.InstallationPage.PerformLayout()
        Me.ProgressPage.ResumeLayout(False)
        Me.ProgressPage.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents CancelBtn As Button
    Friend WithEvents NextBtn As Button
    Friend WithEvents BackBtn As Button
    Friend WithEvents MainPanel As Panel
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents WelcomePage As TabPage
    Friend WithEvents WelcomePageLabel1 As Label
    Friend WithEvents ProgressPage As TabPage
    Friend WithEvents WelcomePageLabel2 As Label
    Friend WithEvents InstallationPageChk2 As CheckBox
    Friend WithEvents InstallationPageChk1 As CheckBox
    Friend WithEvents InstallationPageLabel2 As Label
    Friend WithEvents InstallationPageLabel1 As Label
    Friend WithEvents ProgressPageText As Label
    Friend WithEvents ProgressPagePB As ProgressBar
    Friend WithEvents ProgressPageTitle As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents InstallationPageChk3 As CheckBox
    Friend WithEvents VAILabel As Label
    Friend WithEvents VAICombo As ComboBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents VIALabel2 As Label
    Friend WithEvents InstallationPage As TabPage
    Friend WithEvents LegacyPath As CheckBox
    Friend WithEvents PictureBox2 As PictureBox
End Class
