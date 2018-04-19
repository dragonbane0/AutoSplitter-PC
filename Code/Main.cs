namespace AutoSplitter
{
    using System;
    using System.ComponentModel;
    using System.Net.Sockets;
    using System.Windows.Forms;
    using System.Text;
    using System.Threading;
    using System.IO;

    public class Main : Form
    {
       private static System.Collections.Specialized.StringCollection TPSplits = new System.Collections.Specialized.StringCollection{
            "Slingshot",
            "Cage Break",
            "Ordon Finished (Ilya CS)",
            "Sewers Finished",
            "S+S done (Twilight Entered)",
            "Faron Twilight Cleared",
            "Forest Temple Entered",
            "Gale Boomerang",
            "Forest Temple Beaten",
            "Eldin Vessel",
            "Eldin Twilight Cleared",
            "Lanayru Vessel",
            "Lanayru Twilight Cleared",
            "Epona Clip (Domain Entered)",
            "Bomb Bag",
            "KB2 Beaten",
            "Escort Finished",
            "Zora Armour",
            "Iron Boots",
            "Lakebed Entered",
            "Clawshot",
            "Lakebed Beaten",
            "MDH Finished",
            "Master Sword",
            "Arbiter's Grounds Entered",
            "Spinner",
            "Arbiter's Grounds Beaten",
            "Enter Snowpeak Ruins",
            "Ball and Chain",
            "Leave Snowpeak Ruins",
            "City in the Sky Entered",
            "Double Clawshots",
            "City in the Sky Beaten",
            "Palace of Twilight Entered",
            "Light Sword (on Fadeout)",
            "Light Sword (on Text)",
            "Palace of Twilight Beaten (on Zant Fadeout)",
            "Palace of Twilight Beaten (on Warp)",
            "Hyrule Castle Entered",
            "Hyrule Castle Beaten (on Boss Door Fadeout)",
            "Hyrule Castle Beaten (on Fadeout to Throne Room)",
            "Puppet Zelda Defeated",
             };

        private static System.Collections.Specialized.StringCollection TWWSplits = new System.Collections.Specialized.StringCollection{ };

        private static System.Collections.Specialized.StringCollection DKCSplits = new System.Collections.Specialized.StringCollection{
            "Split for every finished level",
             };

        private static System.Collections.Specialized.StringCollection ActivatedSplits = new System.Collections.Specialized.StringCollection();
        private static System.Collections.Specialized.StringCollection FinishedSplits = new System.Collections.Specialized.StringCollection();

        private static System.Collections.Generic.List<System.Timers.Timer> LiveSplitTimerList = new System.Collections.Generic.List<System.Timers.Timer>();

        private static bool connected = false;
        private static int gameID = -1;

        private static bool isRunInProgress = false;
        private static bool isLoadTimerPaused = false;

        private static long startTimeBase = 0;
        private static UInt32 totalLoadFrames = 0;

        private static UInt32 livesplitDelay = 0;
        private static bool timerLock = false;

        private Thread splittingThread;
        private static TcpClient client;
        private static NetworkStream networkStream;

        private static TcpClient liveSplit;
        private static NetworkStream liveSplitStream;

        private IContainer components;
        private Button Button_Connect;
        public uint diff;
        private TextBox Textbox_IP;
        private CheckedListBox checkedListBox_Splits;
        private ComboBox comboBox_Game;
        private Label label2;
        private Label label3;
        private ContextMenuStrip contextMenuStrip_checkedListBox;
        private ToolStripMenuItem toolStripMenuItem_SelectAll;
        private ToolTip toolTip_contextMenu;
        private ToolStripMenuItem toolStripMenuItem_SelectNone;
        private Label label4;
        private TextBox textBox_Delay;
        private Label Label1;

        public Main()
        {
            this.InitializeComponent(); 
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.Button_Connect = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.checkedListBox_Splits = new System.Windows.Forms.CheckedListBox();
            this.comboBox_Game = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Textbox_IP = new System.Windows.Forms.TextBox();
            this.contextMenuStrip_checkedListBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem_SelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_SelectNone = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip_contextMenu = new System.Windows.Forms.ToolTip(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_Delay = new System.Windows.Forms.TextBox();
            this.contextMenuStrip_checkedListBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // Button_Connect
            // 
            this.Button_Connect.Enabled = false;
            this.Button_Connect.Location = new System.Drawing.Point(92, 484);
            this.Button_Connect.Name = "Button_Connect";
            this.Button_Connect.Size = new System.Drawing.Size(111, 30);
            this.Button_Connect.TabIndex = 3;
            this.Button_Connect.Text = "Connect";
            this.Button_Connect.UseVisualStyleBackColor = true;
            this.Button_Connect.Click += new System.EventHandler(this.Button_Connect_Click);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(71, 18);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(46, 13);
            this.Label1.TabIndex = 200;
            this.Label1.Text = "WiiU IP:";
            // 
            // checkedListBox_Splits
            // 
            this.checkedListBox_Splits.CheckOnClick = true;
            this.checkedListBox_Splits.Enabled = false;
            this.checkedListBox_Splits.FormattingEnabled = true;
            this.checkedListBox_Splits.HorizontalScrollbar = true;
            this.checkedListBox_Splits.Location = new System.Drawing.Point(12, 188);
            this.checkedListBox_Splits.Name = "checkedListBox_Splits";
            this.checkedListBox_Splits.ScrollAlwaysVisible = true;
            this.checkedListBox_Splits.Size = new System.Drawing.Size(283, 244);
            this.checkedListBox_Splits.TabIndex = 2;
            this.toolTip_contextMenu.SetToolTip(this.checkedListBox_Splits, "Use right Mouse Button for quick select/deselect of all splits");
            this.checkedListBox_Splits.SelectedIndexChanged += new System.EventHandler(this.checkedListBox_Splits_SelectedIndexChanged);
            this.checkedListBox_Splits.MouseDown += new System.Windows.Forms.MouseEventHandler(this.checkedListBox_Splits_MouseClick);
            // 
            // comboBox_Game
            // 
            this.comboBox_Game.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Game.FormattingEnabled = true;
            this.comboBox_Game.Items.AddRange(new object[] {
            "The Wind Waker HD",
            "Twilight Princess HD",
            "DKC: Tropical Freeze"});
            this.comboBox_Game.Location = new System.Drawing.Point(63, 93);
            this.comboBox_Game.MaxDropDownItems = 4;
            this.comboBox_Game.Name = "comboBox_Game";
            this.comboBox_Game.Size = new System.Drawing.Size(173, 21);
            this.comboBox_Game.TabIndex = 1;
            this.comboBox_Game.SelectedIndexChanged += new System.EventHandler(this.comboBox_Game_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(91, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 13);
            this.label2.TabIndex = 201;
            this.label2.Text = "Choose your Game:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(74, 157);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(151, 26);
            this.label3.TabIndex = 202;
            this.label3.Text = "Select Splits:\r\n(Start + End is automatic)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Textbox_IP
            // 
            this.Textbox_IP.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Properties.Settings.Default, "ipAddress", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Textbox_IP.Location = new System.Drawing.Point(123, 15);
            this.Textbox_IP.Name = "Textbox_IP";
            this.Textbox_IP.Size = new System.Drawing.Size(105, 20);
            this.Textbox_IP.TabIndex = 0;
            this.Textbox_IP.Text = global::Properties.Settings.Default.ipAddress;
            // 
            // contextMenuStrip_checkedListBox
            // 
            this.contextMenuStrip_checkedListBox.AllowMerge = false;
            this.contextMenuStrip_checkedListBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_SelectAll,
            this.toolStripMenuItem_SelectNone});
            this.contextMenuStrip_checkedListBox.Name = "contextMenuStrip1";
            this.contextMenuStrip_checkedListBox.ShowImageMargin = false;
            this.contextMenuStrip_checkedListBox.Size = new System.Drawing.Size(113, 48);
            // 
            // toolStripMenuItem_SelectAll
            // 
            this.toolStripMenuItem_SelectAll.Checked = true;
            this.toolStripMenuItem_SelectAll.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.toolStripMenuItem_SelectAll.Name = "toolStripMenuItem_SelectAll";
            this.toolStripMenuItem_SelectAll.Size = new System.Drawing.Size(112, 22);
            this.toolStripMenuItem_SelectAll.Text = "Select All";
            this.toolStripMenuItem_SelectAll.Click += new System.EventHandler(this.toolStripMenuItem_SelectAll_Click);
            // 
            // toolStripMenuItem_SelectNone
            // 
            this.toolStripMenuItem_SelectNone.Name = "toolStripMenuItem_SelectNone";
            this.toolStripMenuItem_SelectNone.Size = new System.Drawing.Size(112, 22);
            this.toolStripMenuItem_SelectNone.Text = "Select None";
            this.toolStripMenuItem_SelectNone.Click += new System.EventHandler(this.toolStripMenuItem_SelectNone_Click);
            // 
            // toolTip_contextMenu
            // 
            this.toolTip_contextMenu.AutomaticDelay = 300;
            this.toolTip_contextMenu.AutoPopDelay = 5000;
            this.toolTip_contextMenu.InitialDelay = 200;
            this.toolTip_contextMenu.IsBalloon = true;
            this.toolTip_contextMenu.ReshowDelay = 60;
            this.toolTip_contextMenu.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip_contextMenu_Popup);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(70, 451);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 13);
            this.label4.TabIndex = 204;
            this.label4.Text = "LiveSplit Delay (in ms):";
            // 
            // textBox_Delay
            // 
            this.textBox_Delay.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Properties.Settings.Default, "LSDelay", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox_Delay.Location = new System.Drawing.Point(189, 448);
            this.textBox_Delay.MaxLength = 5;
            this.textBox_Delay.Name = "textBox_Delay";
            this.textBox_Delay.Size = new System.Drawing.Size(38, 20);
            this.textBox_Delay.TabIndex = 203;
            this.textBox_Delay.Text = global::Properties.Settings.Default.LSDelay;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 524);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_Delay);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox_Game);
            this.Controls.Add(this.checkedListBox_Splits);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.Button_Connect);
            this.Controls.Add(this.Textbox_IP);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "WiiU Auto Splitter";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_Close);
            this.Load += new System.EventHandler(this.Main_Load);
            this.Shown += new System.EventHandler(this.Main_Shown);
            this.contextMenuStrip_checkedListBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            toolTip_contextMenu.Active = Properties.Settings.Default.toolTipActive;
            comboBox_Game.SelectedItem = Properties.Settings.Default.selectedGame;
        }

        private void Main_Shown(object sender, EventArgs e)
        {

        }

        private void Main_Close(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (connected)
            {
                splittingThread.Abort();

                //Close Gecko
                try
                {
                    if (client == null)
                    {
                        throw new IOException("Not connected.", new NullReferenceException());
                    }
                    networkStream.Close();
                    client.Close();

                }
                catch (Exception) { }
                finally
                {
                    client = null;
                }

                //Close Livesplit
                try
                {
                    if (liveSplit == null)
                    {
                        throw new IOException("Not connected.", new NullReferenceException());
                    }
                    liveSplitStream.Close();
                    liveSplit.Close();
                }
                catch (Exception) { }
                finally
                {
                    liveSplit = null;
                }

                Button_Connect.Text = "Connect";
                connected = false;
            }

            Properties.Settings.Default.Save();
        }

        static UInt16 ConvertToUInt16(UInt32 Raw)
        {
            byte[] Data = new byte[2];
            for (int i = 0; i < 2; i++)
                Data[i] = (byte)((Raw >> (i * 8)) & 0xFF);
            return BitConverter.ToUInt16(Data, 0);
        }

        static bool IsBitSet(byte b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }

        static bool IsSplitCompleted(int index)
        {
            int storedIndex = -1;

            foreach (string item in FinishedSplits)
            {
                int.TryParse(item, out storedIndex);

                if (storedIndex == index)
                    return true;
            }
            return false;
        }

        static string GetTimeString(long currentTimeBase, long startTimeBase, UInt32 loadFrames, double frameTimeMS)
        {
            double totalLoadTimeMS = Math.Floor((loadFrames * frameTimeMS));
            double totalRunTimeMS = currentTimeBase - startTimeBase;

            double adjustedRunTimeMS = totalRunTimeMS - totalLoadTimeMS;

            double seconds = adjustedRunTimeMS / 1000.0f;

            double minutes = seconds / 60.0f;
            double hours = Math.Floor(minutes / 60.0f);

            minutes = Math.Floor(minutes - (hours * 60.0f));
            seconds = seconds - (((hours * 60.0f) * 60.0f) + (minutes * 60.0f));

            string timeString = hours.ToString() + ":" + minutes.ToString() + ":" + seconds.ToString();

            timeString = timeString.Replace(",", ".");

            if (hours > 24.0f || hours < 0.0f || minutes > 60.0f || minutes < 0.0f || seconds > 60.0f || seconds < 0.0f) //Check to make sure the time is reasonable
                timeString = "";

            return timeString;
        }

        static bool IsTimeStringZero(string timeString)
        {
            timeString = timeString.Replace(".", ",");

            string[] splitTimes = timeString.Split(':');

            foreach (string subString in splitTimes)
            {
                double tempTime = 0.0f;

                if (double.TryParse(subString, out tempTime) == true)
                {
                    if (tempTime > 0.0f)
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        static void HandleTimer(System.Timers.Timer timer, string command, EndianBinaryWriter liveSplitWriter)
        {
            while (timerLock)
            {
                Thread.Sleep(10);
            }

            timerLock = true;

            try
            {
                LiveSplitTimerList.Remove(timer);
                timer.Stop();
                timer.Dispose();

                liveSplitWriter.Write(command, Encoding.UTF8, false);
            }
            catch
            {
            }

            timerLock = false;
        }

        static void DelayTimer(string command, EndianBinaryWriter liveSplitWriter)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = livesplitDelay + (LiveSplitTimerList.Count * 30); //Add additional 30 ms of delay per timer to space them out a bit
            timer.Elapsed += (sender, e) => 
            {
                HandleTimer(timer, command, liveSplitWriter);
            };

            timer.AutoReset = false;
            timer.Enabled = true;

            LiveSplitTimerList.Add(timer);
        }

        private void splitThread(object client_obj)
        {
            NetworkStream stream = (NetworkStream)client_obj;
            EndianBinaryReader reader = new EndianBinaryReader(stream);

            EndianBinaryReader liveSplitReader = new EndianBinaryReader(liveSplitStream);
            EndianBinaryWriter liveSplitWriter = new EndianBinaryWriter(liveSplitStream);

            try
            {
                //TP HD
                string CurrentStage = "";
                byte CurrentRoom = 0;
                byte CurrentSpawn = 0;
                string CurrentState = "";

                string NextStage = "";
                byte NextRoom = 0;
                byte NextSpawn = 0;
                byte NextState = 0;

                byte TriggerLoading = 0;
                UInt32 IsLoading_TP = 0;

                byte eventActiveFlag = 0;

                UInt16 CurrentDialogID = 0;
                UInt16 NextDialogID = 0;

                byte CageBreakFlag = 0;
                byte MSRemovedFlag = 0;
                byte LightSwordCSFlag = 0;

                bool firstStart = true;

                //Global
                byte isNewRun = 0;
                byte hasRunEnded = 0;
                byte makeSplit = 0;

                byte isLoading = 0;
                long timeBase = 0;
                UInt32 loadingFrames = 0;

                while (true)
                {
                    byte cmd_byte = reader.ReadByte();
                    switch (cmd_byte)
                    {
                        case 0x00: //Nothing to receive, skip
                            {
                                continue;
                            }
                        case 0x01: //Relevant Auto Splitter Data
                            {
                                if (gameID == 0) //TWW HD Stuff
                                {

                                }
                                else if (gameID == 1) //TP HD Stuff
                                {
                                    CurrentStage = reader.ReadStringCountOrNT(Encoding.ASCII, 7);
                                    CurrentRoom = reader.ReadByte();
                                    CurrentSpawn = reader.ReadByte();
                                    CurrentState = reader.ReadString(Encoding.ASCII, 1);

                                    NextStage = reader.ReadStringCountOrNT(Encoding.ASCII, 7);
                                    NextRoom = reader.ReadByte();
                                    NextSpawn = reader.ReadByte();
                                    NextState = reader.ReadByte();

                                    TriggerLoading = reader.ReadByte();
                                    IsLoading_TP = reader.ReadUInt32();

                                    eventActiveFlag = reader.ReadByte();

                                    CurrentDialogID = reader.ReadUInt16();
                                    NextDialogID = reader.ReadUInt16();

                                    //1 = 0 bit, 2 = 1 bit, 4 = 2 bit, 8 = 3 bit, 16 = 4 bit, 32 = 5 bit, 64 = 6 bit, 128 = 7 bit
                                    CageBreakFlag = reader.ReadByte();
                                    MSRemovedFlag = reader.ReadByte();
                                    LightSwordCSFlag = reader.ReadByte();

                                    isNewRun = reader.ReadByte();
                                    hasRunEnded = reader.ReadByte();

                                    timeBase = reader.ReadInt64();
                                    loadingFrames = reader.ReadUInt32();

                                    //Make sure on first connection that the user is not inside the file menu already
                                    if (firstStart == true)
                                    {
                                        firstStart = false;

                                        if (CurrentStage == "F_SP102" && eventActiveFlag == 0)
                                        {
                                            MessageBox.Show("You need to first connect while on the Title Screen Animation! Disconnecting...", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            throw new Exception("Not Title Screen Error");
                                        }
                                    }

                                    if (isNewRun == 1) //New file was created
                                    {
                                        liveSplitWriter.Write("getcurrenttime\r\n", Encoding.UTF8, false);

                                        String result = liveSplitReader.ReadStringLine(Encoding.UTF8);

                                        string command = "";

                                        if (IsTimeStringZero(result) == false) //Timer is either running or has finished
                                        {
                                            if (isRunInProgress) //Run is still running internally too, thus Reset timer
                                            {
                                                command = "reset\r\n";
                                            }
                                            else //Timer might be in finished state, don't reset so user doesn't loose data
                                            {
                                                MessageBox.Show("Potential data loss risk, timer is already started/not reset! Time was: " + result, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                isNewRun = 0;
                                            }
                                        }

                                        if (isNewRun == 1)
                                        {
                                            //Start Timer
                                            command += "starttimer\r\n";

                                            if (livesplitDelay > 0)
                                                DelayTimer(command, liveSplitWriter);
                                            else
                                                liveSplitWriter.Write(command, Encoding.UTF8, false);

                                            FinishedSplits.Clear();
                                            isRunInProgress = true;
                                            isLoadTimerPaused = false;

                                            startTimeBase = timeBase;
                                            totalLoadFrames = 0;
                                        }
                                    }

                                    if (isRunInProgress)
                                    {
                                        if (hasRunEnded == 1)
                                        {
                                            string command = "split\r\n";

                                            //Calc final time and set it
                                            string timeString = GetTimeString(timeBase, startTimeBase, totalLoadFrames, 16.6667f); //AutoSplitter Thread runs at 60 FPS

                                            if (timeString != "") //Only set time when it's sane
                                                command = "setgametime " + timeString + "\r\n" + command;

                                            //Make final split
                                            if (livesplitDelay > 0)
                                                DelayTimer(command, liveSplitWriter);
                                            else
                                                liveSplitWriter.Write(command, Encoding.UTF8, false);

                                            isRunInProgress = false;
                                        }

                                        if (isRunInProgress)
                                        {
                                            if (IsLoading_TP == 1 && isLoadTimerPaused == false) //Pause Timer
                                            {
                                                string command = "pausegametime\r\n";

                                                //Calc current time and set it to correct eventual drift due network lag
                                                string timeString = GetTimeString(timeBase, startTimeBase, totalLoadFrames, 16.6667f);

                                                if (timeString != "")
                                                    command += "setgametime " + timeString + "\r\n";

                                                if (livesplitDelay > 0)
                                                    DelayTimer(command, liveSplitWriter);
                                                else
                                                    liveSplitWriter.Write(command, Encoding.UTF8, false);

                                                isLoadTimerPaused = true;
                                            }
                                            else if (IsLoading_TP == 0 && isLoadTimerPaused == true) //Unpause Timer
                                            {
                                                string command = "unpausegametime\r\n";
                                                totalLoadFrames = totalLoadFrames + loadingFrames;

                                                //Calc current time and set it to account for eventual home buffers during loading
                                                string timeString = GetTimeString(timeBase, startTimeBase, totalLoadFrames, 16.6667f);

                                                if (timeString != "")
                                                    command += "setgametime " + timeString + "\r\n";

                                                if (livesplitDelay > 0)
                                                    DelayTimer(command, liveSplitWriter);
                                                else
                                                    liveSplitWriter.Write(command, Encoding.UTF8, false);

                                                isLoadTimerPaused = false;
                                            }

                                            //Check Splits
                                            int index = -1;
                                            foreach (string item in ActivatedSplits)
                                            {
                                                int.TryParse(item, out index);

                                                if (IsSplitCompleted(index) == true)
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    bool doSplit = false;

                                                    switch (index)
                                                    {
                                                        case 0: //Slingshot
                                                            {
                                                                if (CurrentDialogID == 176 && NextDialogID == 176) //Slingshot Dialog
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 1: //Cage Break
                                                            {
                                                                if (IsBitSet(CageBreakFlag, 1) == true) //Bit 2 (2) is switched to ON
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 2: //Ordon Finished
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "F_SP104" && NextStage == "F_SP104" && NextSpawn == 0x16 && NextState == 0x08) //Ilya CS is loaded
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 3: //Sewers Finished
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "R_SP107" && CurrentRoom == 3 && NextStage == "F_SP104" && NextSpawn == 0x03) //Spring gets loaded
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 4: //S+S Done
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "F_SP104" && CurrentState == "4" && NextStage == "F_SP108" && NextRoom == 0 && NextSpawn == 0x17 && NextState == 0x0A) //Faron Woods Twilight
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 5: //Faron Cleared
                                                            {
                                                                if (TriggerLoading == 1 && NextStage == "F_SP108" && NextRoom == 1 && NextSpawn == 0x14 && NextState == 0x0B) //Faron Cleared CS
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 6: //Forest Entered
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "F_SP108" && NextStage == "D_MN05" && NextRoom == 22 && NextSpawn == 0x00) //Forest Entered
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 7: //Gale
                                                            {
                                                                if (CurrentDialogID == 165 && NextDialogID == 165) //Gale Dialog
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 8: //Forest Beaten
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "D_MN05A" && NextStage == "F_SP108" && NextRoom == 1 && NextSpawn == 0x01) //Faron Save Dialog
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 9: //Eldin Vessel
                                                            {
                                                                if (CurrentDialogID == 263 && NextDialogID == 263) //Eldin Vessel Dialog
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 10: //Eldin Cleared
                                                            {
                                                                if (TriggerLoading == 1 && NextStage == "F_SP109" && NextSpawn == 0x1E && NextState == 0x08) //Eldin Cleared CS
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 11: //Lanayru Vessel
                                                            {
                                                                if (CurrentDialogID == 264 && NextDialogID == 264) //Lanayru Vessel Dialog
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 12: //Lanayru Cleared
                                                            {
                                                                if (TriggerLoading == 1 && NextStage == "F_SP115" && NextRoom == 1 && NextSpawn == 0x14 && NextState == 0x08) //Lanayru Cleared CS
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 13: //Epona Clip
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "F_SP121" && NextStage == "F_SP113" && NextRoom == 1 && NextSpawn == 0x0F) //Domain Loaded
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 14: //Bomb Bag
                                                            {
                                                                if (CurrentDialogID == 182 && NextDialogID == 182) //Bomb Bag Dialog
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 15: //KB2 Beaten
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "F_SP123" && NextStage == "F_SP121" && NextRoom == 13 && NextSpawn == 0x63) //Post KB2 Load
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 16: //Escort Finished
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "F_SP121" && CurrentRoom == 3 && NextStage == "R_SP109" && NextRoom == 2 && NextSpawn == 0x03) //Post Escort CS
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 17: //Zora Armour
                                                            {
                                                                if (CurrentDialogID == 150 && NextDialogID == 150) //Zora Armour Dialog
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 18: //Iron Boots
                                                            {
                                                                if (CurrentDialogID == 170 && NextDialogID == 170) //Iron Boots Dialog
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 19: //Lakebed Entered
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "F_SP115" && CurrentRoom == 0 && NextStage == "D_MN01" && NextRoom == 0 && NextSpawn == 0x00) //Lakebed Entered
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 20: //Clawshot
                                                            {
                                                                if (CurrentDialogID == 169 && NextDialogID == 169) //Clawshot Dialog
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 21: //Lakebed Beaten
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "D_MN01A" && NextStage == "F_SP115" && NextRoom == 1 && NextSpawn == 0x16) //Post Lakebed Save Dialog
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 22: //MDH Finished
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "R_SP107" && CurrentRoom == 3 && NextStage == "F_SP122" && NextRoom == 8 && NextSpawn == 0x64) //Hyrule Field post MDH
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 23: //MS
                                                            {
                                                                if (IsBitSet(MSRemovedFlag, 5) == true) //Bit 6 (32) is switched to ON
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 24: //Entered AG
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "F_SP118" && CurrentRoom == 3 && NextStage == "D_MN10" && NextRoom == 0 && NextSpawn == 0x00) //AG Entered
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 25: //Spinner
                                                            {
                                                                if (CurrentDialogID == 166 && NextDialogID == 166) //Spinner Dialog
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 26: //AG Finished
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "D_MN10A" && NextStage == "F_SP125" && NextSpawn == 0x00) //Post Arbiters Save Dialog
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 27: //SPR Entered
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "F_SP114" && CurrentRoom == 1 && NextStage == "D_MN11" && NextRoom == 0) //SPR Entered
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 28: //B&C
                                                            {
                                                                if (CurrentDialogID == 167 && NextDialogID == 167) //B&C Dialog
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 29: //Left SPR
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "D_MN11" && CurrentRoom == 0 && NextStage == "F_SP114" && NextRoom == 1) //SPR Left
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 30: //City Entered
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "F_SP115" && CurrentRoom == 0 && NextStage == "D_MN07" && NextRoom == 0 && NextSpawn == 0x02) //City Entered
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 31: //Double Claw
                                                            {
                                                                if (CurrentDialogID == 172 && NextDialogID == 172) //Double Claw Dialog
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 32: //City Beaten
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "D_MN07A" && NextStage == "D_MN07" && NextRoom == 0 && NextSpawn == 0x04) //Post Argorok Save Dialog
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 33: //PoT Entered
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "F_SP125" && NextStage == "D_MN08" && NextRoom == 0 && NextSpawn == 0x0A) //PoT Entered
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 34: //Light Sword (pre cs)
                                                            {
                                                                if (IsBitSet(LightSwordCSFlag, 6) == true) //Bit 7 (64) is switched to ON
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 35: //Light Sword (on text)
                                                            {
                                                                if (CurrentDialogID == 174 && NextDialogID == 174) //Light Sword Dialog
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 36: //Zant Beaten (on fade)
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "D_MN08D" && CurrentRoom == 60 && NextStage == "D_MN08A" && NextSpawn == 0x19) //Load back to Zant Room
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 37: //PoT Beaten (on warp out)
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "D_MN08A" && NextStage == "D_MN08" && NextRoom == 0 && NextSpawn == 0x04) //Post Palace Save Dialog
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 38: //Hyrule Castle Entered
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "F_SP116" && CurrentRoom == 1 && NextStage == "D_MN09" && NextRoom == 11 && NextSpawn == 0x00) //Hyrule Castle Entered
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 39: //HC Beaten (on boss door fade)
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "D_MN09" && CurrentRoom == 12 && NextStage == "D_MN09A" && NextRoom == 51 && NextSpawn == 0x0A) //Post Boss Door in HC
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 40: //Entered final fight
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "D_MN09A" && CurrentRoom == 51 && NextStage == "D_MN09A" && NextRoom == 50 && NextSpawn == 0x14) //Final Fight Entered
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        case 41: //Puppet Zelda Defeated
                                                            {
                                                                if (TriggerLoading == 1 && CurrentStage == "D_MN09A" && CurrentRoom == 50 && NextStage == "D_MN09A" && NextRoom == 50 && NextSpawn == 0x16 && NextState == 0x0A) //Post Puppet Zelda CS
                                                                {
                                                                    doSplit = true;
                                                                }

                                                                break;
                                                            }
                                                        default:
                                                            throw new InvalidDataException();
                                                    }

                                                    if (doSplit)
                                                    {
                                                        string command = "split\r\n";

                                                        //Calc current time for accurate splits and set it
                                                        string timeString = GetTimeString(timeBase, startTimeBase, totalLoadFrames, 16.6667f);

                                                        if (timeString != "")
                                                            command = "setgametime " + timeString + "\r\n" + command;

                                                        //Perform a split
                                                        if (livesplitDelay > 0)
                                                            DelayTimer(command, liveSplitWriter);
                                                        else
                                                            liveSplitWriter.Write(command, Encoding.UTF8, false);

                                                        FinishedSplits.Add(index.ToString());

                                                        //MessageBox.Show("Split: " + checkedListBox_Splits.Items[index].ToString() + " reached!");
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (gameID == 2) //DKC TF Stuff
                                {
                                    isNewRun = reader.ReadByte();
                                    hasRunEnded = reader.ReadByte();
                                    makeSplit =reader.ReadByte();

                                    isLoading = reader.ReadByte();
                                    timeBase = reader.ReadInt64();
                                    loadingFrames = reader.ReadUInt32();

                                    if (isNewRun == 1) //New run was started on level 1-1
                                    {
                                        liveSplitWriter.Write("getcurrenttime\r\n", Encoding.UTF8, false);

                                        String result = liveSplitReader.ReadStringLine(Encoding.UTF8);

                                        string command = "";

                                        if (IsTimeStringZero(result) == false) //Timer is either running or has finished
                                        {
                                            if (isRunInProgress) //Run is still running internally too, thus Reset timer
                                            {
                                                command = "reset\r\n";
                                            }
                                            else //Timer might be in finished state, don't reset so user doesn't loose data
                                            {
                                                MessageBox.Show("Potential data loss risk, timer is already started/not reset! Time was: " + result, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                isNewRun = 0;
                                            }
                                        }

                                        if (isNewRun == 1)
                                        {
                                            //Start Timer
                                            command += "starttimer\r\n";

                                            if (livesplitDelay > 0)
                                                DelayTimer(command, liveSplitWriter);
                                            else
                                                liveSplitWriter.Write(command, Encoding.UTF8, false);


                                            FinishedSplits.Clear();
                                            isRunInProgress = true;
                                            isLoadTimerPaused = false;

                                            startTimeBase = timeBase;
                                            totalLoadFrames = 0;
                                        }
                                    }

                                    if (isRunInProgress)
                                    {
                                        if (hasRunEnded == 1)
                                        {
                                            string command = "split\r\n";

                                            //Calc final time and set it
                                            string timeString = GetTimeString(timeBase, startTimeBase, totalLoadFrames, 16.6667f); //AutoSplitter Thread runs at 60 FPS

                                            if (timeString != "") //Only set time when it's sane
                                                command = "setgametime " + timeString + "\r\n" + command;

                                            //Make final split
                                            if (livesplitDelay > 0)
                                                DelayTimer(command, liveSplitWriter);
                                            else
                                                liveSplitWriter.Write(command, Encoding.UTF8, false);

                                            isRunInProgress = false;
                                        }

                                        if (isRunInProgress)
                                        {
                                            if (isLoading == 1 && isLoadTimerPaused == false) //Pause Timer
                                            {
                                                string command = "pausegametime\r\n";

                                                //Calc current time and set it to correct eventual drift due network lag
                                                string timeString = GetTimeString(timeBase, startTimeBase, totalLoadFrames, 16.6667f);

                                                if (timeString != "")
                                                    command += "setgametime " + timeString + "\r\n";

                                                if (livesplitDelay > 0)
                                                    DelayTimer(command, liveSplitWriter);
                                                else
                                                    liveSplitWriter.Write(command, Encoding.UTF8, false);

                                                isLoadTimerPaused = true;
                                            }
                                            else if (isLoading == 0 && isLoadTimerPaused == true) //Unpause Timer
                                            {
                                                string command = "unpausegametime\r\n";
                                                totalLoadFrames = totalLoadFrames + loadingFrames;

                                                //Calc current time and set it to account for eventual home buffers during loading
                                                string timeString = GetTimeString(timeBase, startTimeBase, totalLoadFrames, 16.6667f);

                                                if (timeString != "")
                                                    command += "setgametime " + timeString + "\r\n";

                                                if (livesplitDelay > 0)
                                                    DelayTimer(command, liveSplitWriter);
                                                else
                                                    liveSplitWriter.Write(command, Encoding.UTF8, false);

                                                isLoadTimerPaused = false;
                                            }

                                            //Check Splits
                                            int index = -1;
                                            foreach (string item in ActivatedSplits)
                                            {
                                                int.TryParse(item, out index);

                                                switch (index)
                                                {
                                                    case 0: //Handle level end splits
                                                        {
                                                            if (makeSplit == 1)
                                                            {                                                           
                                                                string command = "split\r\n";

                                                                //Calc current time for accurate splits and set it
                                                                string timeString = GetTimeString(timeBase, startTimeBase, totalLoadFrames, 16.6667f);

                                                                if (timeString != "")
                                                                    command = "setgametime " + timeString + "\r\n" + command;

                                                                //Perform a split
                                                                if (livesplitDelay > 0)
                                                                    DelayTimer(command, liveSplitWriter);
                                                                else
                                                                    liveSplitWriter.Write(command, Encoding.UTF8, false);
                                                            }

                                                            break;
                                                        }
                                                    default:
                                                        throw new InvalidDataException();
                                                }
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                        default:
                            throw new InvalidDataException();
                    }
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception)
            {
                this.Invoke(
                (Action)(() =>
                {
                    Button_Connect.PerformClick();
                }));
            }
        }

        private void Button_Connect_Click(object sender, EventArgs e)
        {
            if (!connected)
            {
                try
                {
                    //Verify the delay
                    if (UInt32.TryParse(textBox_Delay.Text, out livesplitDelay) == false)
                    {
                        MessageBox.Show("Invalid Delay!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (livesplitDelay > 10000)
                    {
                        MessageBox.Show("Maximum delay is 10 seconds!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    //Livesplit Connection
                    liveSplit = new TcpClient();
                    liveSplit.NoDelay = true;

                    IAsyncResult ar = liveSplit.BeginConnect("localhost", 16834, null, null); //Livesplit uses 16834
                    System.Threading.WaitHandle wh = ar.AsyncWaitHandle;
                    try
                    {
                        if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3), false))
                        {
                            liveSplit.Close();
                            MessageBox.Show("Livesplit: Connection Timeout!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        liveSplit.EndConnect(ar);
                    }
                    finally
                    {
                        wh.Close();
                    }

                    liveSplitStream = liveSplit.GetStream();
                    //networkStream.ReadTimeout = 10000;
                    //networkStream.WriteTimeout = 10000;
                }
                catch (ETCPGeckoException)
                {
                    MessageBox.Show("Livesplit: Connection Error!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                catch (SocketException)
                {
                    MessageBox.Show("Livesplit: Couldn't connect! Is the Livesplit Server running?", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //Gecko Connection
                try
                {
                    client = new TcpClient();
                    client.NoDelay = true;
                    IAsyncResult ar = client.BeginConnect(Textbox_IP.Text, 7334, null, null); //Auto Splitter uses 7334
                    System.Threading.WaitHandle wh = ar.AsyncWaitHandle;
                    try
                    {
                        if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3), false))
                        {
                            client.Close();

                            liveSplitStream.Close();
                            liveSplit.Close();

                            MessageBox.Show("WiiU: Connection Timeout!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        client.EndConnect(ar);
                    }
                    finally
                    {
                        wh.Close();
                    }

                    networkStream = client.GetStream();
                    //networkStream.ReadTimeout = 10000;
                    //networkStream.WriteTimeout = 10000;
                }
                catch (ETCPGeckoException)
                {
                    liveSplitStream.Close();
                    liveSplit.Close();

                    MessageBox.Show("WiiU: Connection Error!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                catch (SocketException)
                {
                    liveSplitStream.Close();
                    liveSplit.Close();

                    MessageBox.Show("WiiU: Wrong IP Entered!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }       


                //Prepare Run Start
                isRunInProgress = false;
                isLoadTimerPaused = false;
                totalLoadFrames = 0;
                startTimeBase = 0;

                ActivatedSplits.Clear();
                FinishedSplits.Clear();

                //Clear LiveSplit Timers
                foreach (System.Timers.Timer timer in LiveSplitTimerList)
                {
                    timer.Stop();
                    timer.Dispose();
                }
                LiveSplitTimerList.Clear();
                timerLock = false;

                foreach (int index in checkedListBox_Splits.CheckedIndices)
                {
                    ActivatedSplits.Add(index.ToString());
                }
                
                comboBox_Game.Enabled = false;
                checkedListBox_Splits.Enabled = false;

                splittingThread = new Thread(splitThread);
                splittingThread.Name = "Auto Splitter Thread";
                splittingThread.Start(networkStream);

                Button_Connect.Text = "Disconnect";
                connected = true;
            }
            else //Disconnect
            {
                splittingThread.Abort();

                isRunInProgress = false;
                isLoadTimerPaused = false;
                totalLoadFrames = 0;
                startTimeBase = 0;

                ActivatedSplits.Clear();
                FinishedSplits.Clear();

                //Clear LiveSplit Timers
                foreach (System.Timers.Timer timer in LiveSplitTimerList)
                {
                    timer.Stop();
                    timer.Dispose();
                }
                LiveSplitTimerList.Clear();

                comboBox_Game.Enabled = true;
                checkedListBox_Splits.Enabled = true;

                //Close Gecko
                try
                {
                    if (client == null)
                    {
                        throw new IOException("Not connected.", new NullReferenceException());
                    }
                    networkStream.Close();
                    client.Close();

                }
                catch (Exception) { }
                finally
                {
                    client = null;
                }

                //Close Livesplit
                try
                {
                    if (liveSplit == null)
                    {
                        throw new IOException("Not connected.", new NullReferenceException());
                    }
                    liveSplitStream.Close();
                    liveSplit.Close();
                }
                catch (Exception) { }
                finally
                {
                    liveSplit = null;
                }

                Button_Connect.Text = "Connect";
                connected = false;
            }
        }

        private void comboBox_Game_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.selectedGame = comboBox_Game.SelectedItem.ToString();

            if (comboBox_Game.SelectedItem.ToString() == "The Wind Waker HD")
            {
                gameID = 0;

                checkedListBox_Splits.Items.Clear();

                foreach (string item in TWWSplits)
                    checkedListBox_Splits.Items.Add(item, false);

                if (Properties.Settings.Default.checkedSplits_TWW == null)
                {
                    Properties.Settings.Default.checkedSplits_TWW = new System.Collections.Specialized.StringCollection();
                }
                else
                {
                    foreach (string item in Properties.Settings.Default.checkedSplits_TWW)
                    {
                        try
                        {
                            int index = 0;
                            int.TryParse(item, out index);

                            checkedListBox_Splits.SetItemChecked(index, true);
                        }
                        catch (Exception)
                        {
                            Properties.Settings.Default.checkedSplits_TWW.Clear();
                            break;
                        }
                    }
                }

                //FIXED: Disabled for now
                checkedListBox_Splits.Enabled = false;
                Button_Connect.Enabled = false;
            }
            else if (comboBox_Game.SelectedItem.ToString() == "Twilight Princess HD")
            {
                gameID = 1;

                checkedListBox_Splits.Items.Clear();

                foreach (string item in TPSplits)
                    checkedListBox_Splits.Items.Add(item, false);

                if (Properties.Settings.Default.checkedSplits_TP == null)
                {
                    Properties.Settings.Default.checkedSplits_TP = new System.Collections.Specialized.StringCollection();
                }
                else
                {
                    foreach (string item in Properties.Settings.Default.checkedSplits_TP)
                    {
                        try
                        {
                            int index = 0;
                            int.TryParse(item, out index);

                            checkedListBox_Splits.SetItemChecked(index, true);
                        }
                        catch (Exception)
                        {
                            Properties.Settings.Default.checkedSplits_TP.Clear();
                            break;
                        }
                    }
                }

                checkedListBox_Splits.Enabled = true;
                Button_Connect.Enabled = true;
            }
            else if (comboBox_Game.SelectedItem.ToString() == "DKC: Tropical Freeze")
            {
                gameID = 2;

                checkedListBox_Splits.Items.Clear();

                foreach (string item in DKCSplits)
                    checkedListBox_Splits.Items.Add(item, false);

                if (Properties.Settings.Default.checkedSplits_DKC == null)
                {
                    Properties.Settings.Default.checkedSplits_DKC = new System.Collections.Specialized.StringCollection();
                }
                else
                {
                    foreach (string item in Properties.Settings.Default.checkedSplits_DKC)
                    {
                        try
                        {
                            int index = 0;
                            int.TryParse(item, out index);

                            checkedListBox_Splits.SetItemChecked(index, true);
                        }
                        catch (Exception)
                        {
                            Properties.Settings.Default.checkedSplits_DKC.Clear();
                            break;
                        }
                    }
                }

                checkedListBox_Splits.Enabled = true;
                Button_Connect.Enabled = true;
            }
        }

        private void checkedListBox_Splits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gameID == 0) //TWW HD
            {
                Properties.Settings.Default.checkedSplits_TWW.Clear();

                foreach (int item in checkedListBox_Splits.CheckedIndices)
                {
                    Properties.Settings.Default.checkedSplits_TWW.Add(item.ToString());
                }
            }
            else if (gameID == 1) //TP HD
            {
                Properties.Settings.Default.checkedSplits_TP.Clear();

                foreach (int item in checkedListBox_Splits.CheckedIndices)
                {
                    Properties.Settings.Default.checkedSplits_TP.Add(item.ToString());
                }
            }
            else if (gameID == 2) //DKC
            {
                Properties.Settings.Default.checkedSplits_DKC.Clear();

                foreach (int item in checkedListBox_Splits.CheckedIndices)
                {
                    Properties.Settings.Default.checkedSplits_DKC.Add(item.ToString());
                }
            }
        }

        private void checkedListBox_Splits_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuStrip_checkedListBox.Show(checkedListBox_Splits, e.X, e.Y);
        }

        private void toolTip_contextMenu_Popup(object sender, PopupEventArgs e)
        {
            Properties.Settings.Default.toolTipActive = false;

            toolTip_contextMenu.ReshowDelay = 600000;
        }

        private void toolStripMenuItem_SelectAll_Click(object sender, EventArgs e)
        {
            if (gameID == 0) //TWW HD
            {
                Properties.Settings.Default.checkedSplits_TWW.Clear();

                for (int index = 0; index < checkedListBox_Splits.Items.Count; index++)
                {
                    Properties.Settings.Default.checkedSplits_TWW.Add(index.ToString());
                    checkedListBox_Splits.SetItemChecked(index, true);
                }
            }
            else if (gameID == 1) //TP HD
            {
                Properties.Settings.Default.checkedSplits_TP.Clear();

                for (int index = 0; index < checkedListBox_Splits.Items.Count; index++)
                {
                    Properties.Settings.Default.checkedSplits_TP.Add(index.ToString());
                    checkedListBox_Splits.SetItemChecked(index, true);
                }
            }
            else if (gameID == 2) //DKC
            {
                Properties.Settings.Default.checkedSplits_DKC.Clear();

                for (int index = 0; index < checkedListBox_Splits.Items.Count; index++)
                {
                    Properties.Settings.Default.checkedSplits_DKC.Add(index.ToString());
                    checkedListBox_Splits.SetItemChecked(index, true);
                }
            }
        }

        private void toolStripMenuItem_SelectNone_Click(object sender, EventArgs e)
        {
            if (gameID == 0) //TWW HD
            {
                Properties.Settings.Default.checkedSplits_TWW.Clear();
            }
            else if (gameID == 1) //TP HD
            {
                Properties.Settings.Default.checkedSplits_TP.Clear();
            }
            else if (gameID == 2) //DKC
            {
                Properties.Settings.Default.checkedSplits_DKC.Clear();
            }

            for (int index = 0; index < checkedListBox_Splits.Items.Count; index++)
            {
                checkedListBox_Splits.SetItemChecked(index, false);
            }
        }
    }
}