using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ConverterToVideo
{
    public partial class Form1 : Form
    {
        private string[] selectedImages;
        private string tempDir;
        private StreamWriter logFile;
        private ProgressBar progress;
        private Button btnSelectImages;
        private Button btnConvert;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private OpenFileDialog openAudioDialog;
        private string? selectedAudio;
        private NumericUpDown durationInput;
        private ToolStripMenuItem resolutionMenuItem;
        private ToolStripMenuItem formatMenuItem;
        private ToolStripMenuItem frameRateMenuItem;
        private ToolStripMenuItem qualityMenuItem;
        private ToolStripMenuItem transitionMenuItem;
        private TextBox filePathTextBox;
        private Button btnPreview;
        private string? inputFilePath;
        private string? outputFilePath;
        private TabControl tabControl;
        private TabPage tabPageAudioToVideo;
        private TabPage tabPageVideoToAudio;
        private Button btnSelectVideo;
        private Button btnConvertVideoToAudio;
        private Button btnSaveAudio;
        private ComboBox audioFormatComboBox;
        private Button btnSelectImagesForGif;
        private Button btnConvertToGif;
        private NumericUpDown gifDurationInput;

        // المتغيرات الجديدة
        private TabPage tabPageVideoEditing;
        private TrackBar trackBarFrom;
        private TrackBar trackBarTo;
        private Label labelFrom;
        private Label labelTo;
        private Button btnCutVideo;
        private Button btnCompressVideo;
        private SaveFileDialog saveVideoDialog;
        private SaveFileDialog saveCompressedVideoDialog;

        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
            selectedImages = Array.Empty<string>();
            tempDir = Path.Combine(Path.GetTempPath(), "AudioVideoConverter");
            Directory.CreateDirectory(tempDir);
            logFile = new StreamWriter(Path.Combine(tempDir, "log.txt"));

            // تهيئة المتغيرات الجديدة
            tabPageVideoEditing = new TabPage();
            trackBarFrom = new TrackBar();
            trackBarTo = new TrackBar();
            labelFrom = new Label();
            labelTo = new Label();
            btnCutVideo = new Button();
            btnCompressVideo = new Button();
            saveVideoDialog = new SaveFileDialog();
            saveCompressedVideoDialog = new SaveFileDialog();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Audio/Video Converter";
            this.Size = new Size(800, 600);

            // إنشاء TabControl
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            this.Controls.Add(tabControl);

            // إنشاء التبويبة الأولى (تحويل الصوت إلى فيديو)
            tabPageAudioToVideo = new TabPage("تحويل الصوت إلى فيديو");
            tabControl.Controls.Add(tabPageAudioToVideo);

            progress = new ProgressBar();
            progress.Location = new Point(10, 10);
            progress.Size = new Size(760, 20);
            tabPageAudioToVideo.Controls.Add(progress);

            btnSelectImages = new Button();
            btnSelectImages.Text = "اختر الصور (CTRL+I)";
            btnSelectImages.Location = new Point(10, 50);
            btnSelectImages.Click += BtnSelectImages_Click;
            tabPageAudioToVideo.Controls.Add(btnSelectImages);

            Button btnSelectAudio = new Button();
            btnSelectAudio.Text = "اختر الصوت (CTRL+O)";
            btnSelectAudio.Location = new Point(10, 90);
            btnSelectAudio.Click += BtnSelectAudio_Click;
            tabPageAudioToVideo.Controls.Add(btnSelectAudio);

            btnConvert = new Button();
            btnConvert.Text = "تحويل إلى فيديو (CTRL+S)";
            btnConvert.Location = new Point(150, 50);
            btnConvert.Click += BtnConvert_Click;
            tabPageAudioToVideo.Controls.Add(btnConvert);

            durationInput = new NumericUpDown();
            durationInput.Location = new Point(150, 90);
            durationInput.Minimum = 1;
            durationInput.Maximum = 10;
            durationInput.Value = 2;
            durationInput.Width = 100;
            tabPageAudioToVideo.Controls.Add(durationInput);

            filePathTextBox = new TextBox();
            filePathTextBox.Location = new Point(10, 130);
            filePathTextBox.Size = new Size(760, 20);
            filePathTextBox.ReadOnly = true;
            tabPageAudioToVideo.Controls.Add(filePathTextBox);

            btnPreview = new Button();
            btnPreview.Text = "معاينة (CTRL+P)";
            btnPreview.Location = new Point(10, 170);
            btnPreview.Click += BtnPreview_Click;
            tabPageAudioToVideo.Controls.Add(btnPreview);

            // إنشاء التبويبة الثانية (تحويل الفيديو إلى صوت)
            tabPageVideoToAudio = new TabPage("تحويل الفيديو إلى صوت");
            tabControl.Controls.Add(tabPageVideoToAudio);

            btnSelectVideo = new Button();
            btnSelectVideo.Text = "اختر ملف فيديو (CTRL+Shift+I)";
            btnSelectVideo.Location = new Point(10, 30);
            btnSelectVideo.Click += SelectFileButton_Click;
            tabPageVideoToAudio.Controls.Add(btnSelectVideo);

            btnSaveAudio = new Button();
            btnSaveAudio.Text = "اختر مكان الحفظ (CTRL+Shift+E)";
            btnSaveAudio.Location = new Point(10, 70);
            btnSaveAudio.Click += SaveFileButton_Click;
            tabPageVideoToAudio.Controls.Add(btnSaveAudio);

            btnConvertVideoToAudio = new Button();
            btnConvertVideoToAudio.Text = "تحويل إلى صوت (CTRL+Shift+S)";
            btnConvertVideoToAudio.Location = new Point(10, 110);
            btnConvertVideoToAudio.Click += ConvertButton_Click;
            tabPageVideoToAudio.Controls.Add(btnConvertVideoToAudio);

            audioFormatComboBox = new ComboBox();
            audioFormatComboBox.Items.AddRange(new string[] { "MP3", "WAV", "AAC" });
            audioFormatComboBox.SelectedIndex = 0;
            audioFormatComboBox.Location = new Point(10, 150);
            audioFormatComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            tabPageVideoToAudio.Controls.Add(audioFormatComboBox);

            // إنشاء التبويبة الثالثة (تحويل الصور إلى GIF)
            TabPage tabPageImagesToGif = new TabPage("تحويل الصور إلى GIF");
            tabControl.Controls.Add(tabPageImagesToGif);

            btnSelectImagesForGif = new Button();
            btnSelectImagesForGif.Text = "اختر الصور (CTRL+G)";
            btnSelectImagesForGif.Location = new Point(10, 30);
            btnSelectImagesForGif.Click += BtnSelectImagesForGif_Click;
            tabPageImagesToGif.Controls.Add(btnSelectImagesForGif);

            btnConvertToGif = new Button();
            btnConvertToGif.Text = "تحويل إلى GIF (CTRL+Shift+G)";
            btnConvertToGif.Location = new Point(10, 70);
            btnConvertToGif.Click += BtnConvertToGif_Click;
            tabPageImagesToGif.Controls.Add(btnConvertToGif);

            gifDurationInput = new NumericUpDown();
            gifDurationInput.Location = new Point(10, 110);
            gifDurationInput.Minimum = 1;
            gifDurationInput.Maximum = 10;
            gifDurationInput.Value = 2;
            gifDurationInput.Width = 100;
            tabPageImagesToGif.Controls.Add(gifDurationInput);

            // إنشاء التبويبة الرابعة (تحرير الفيديو)
            tabPageVideoEditing = new TabPage("تحرير الفيديو");
            tabControl.Controls.Add(tabPageVideoEditing);

            // شريط السحب لوقت البداية
            trackBarFrom = new TrackBar();
            trackBarFrom.Location = new Point(10, 30);
            trackBarFrom.Size = new Size(760, 45);
            trackBarFrom.TickStyle = TickStyle.None;
            trackBarFrom.Scroll += TrackBarFrom_Scroll;
            tabPageVideoEditing.Controls.Add(trackBarFrom);

            // شريط السحب لوقت النهاية
            trackBarTo = new TrackBar();
            trackBarTo.Location = new Point(10, 80);
            trackBarTo.Size = new Size(760, 45);
            trackBarTo.TickStyle = TickStyle.None;
            trackBarTo.Scroll += TrackBarTo_Scroll;
            tabPageVideoEditing.Controls.Add(trackBarTo);

            // تسمية لوقت البداية
            labelFrom = new Label();
            labelFrom.Location = new Point(10, 120);
            labelFrom.Text = "من: 0 ثانية";
            tabPageVideoEditing.Controls.Add(labelFrom);

            // تسمية لوقت النهاية
            labelTo = new Label();
            labelTo.Location = new Point(10, 150);
            labelTo.Text = "إلى: 0 ثانية";
            tabPageVideoEditing.Controls.Add(labelTo);

            // زر قص الفيديو
            btnCutVideo = new Button();
            btnCutVideo.Text = "قص الفيديو (CTRL+X)";
            btnCutVideo.Location = new Point(10, 190);
            btnCutVideo.Click += BtnCutVideo_Click;
            tabPageVideoEditing.Controls.Add(btnCutVideo);

            // مربع حوار لحفظ الفيديو المقطوع
            saveVideoDialog = new SaveFileDialog();
            saveVideoDialog.Filter = "Video Files|*.mp4;*.avi;*.mkv";

            // زر ضغط الفيديو
            btnCompressVideo = new Button();
            btnCompressVideo.Text = "ضغط الفيديو (CTRL+C)";
            btnCompressVideo.Location = new Point(150, 190);
            btnCompressVideo.Click += BtnCompressVideo_Click;
            tabPageVideoEditing.Controls.Add(btnCompressVideo);

            // مربع حوار لحفظ الفيديو المضغوط
            saveCompressedVideoDialog = new SaveFileDialog();
            saveCompressedVideoDialog.Filter = "Video Files|*.mp4;*.avi;*.mkv";

            // إنشاء شريط القوائم
            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem viewMenuItem = new ToolStripMenuItem("عرض");
            ToolStripMenuItem helpMenuItem = new ToolStripMenuItem("مساعدة");
            ToolStripMenuItem cloudSaveMenuItem = new ToolStripMenuItem("حفظ على السحابة");

            // قائمة عرض
            resolutionMenuItem = new ToolStripMenuItem("الدقة");
            resolutionMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("640x480"),
                new ToolStripMenuItem("1280x720"),
                new ToolStripMenuItem("1920x1080")
            });
            resolutionMenuItem.DropDownItemClicked += ResolutionMenuItem_DropDownItemClicked;

            formatMenuItem = new ToolStripMenuItem("التنسيق");
            formatMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("MP4"),
                new ToolStripMenuItem("AVI"),
                new ToolStripMenuItem("MKV")
            });
            formatMenuItem.DropDownItemClicked += FormatMenuItem_DropDownItemClicked;

            frameRateMenuItem = new ToolStripMenuItem("معدل الإطارات");
            frameRateMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("24"),
                new ToolStripMenuItem("30"),
                new ToolStripMenuItem("60")
            });
            frameRateMenuItem.DropDownItemClicked += FrameRateMenuItem_DropDownItemClicked;

            qualityMenuItem = new ToolStripMenuItem("الجودة");
            qualityMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("منخفضة"),
                new ToolStripMenuItem("متوسطة"),
                new ToolStripMenuItem("عالية")
            });
            qualityMenuItem.DropDownItemClicked += QualityMenuItem_DropDownItemClicked;

            transitionMenuItem = new ToolStripMenuItem("التأثيرات الانتقالية");
            transitionMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("بدون تأثير"),
                new ToolStripMenuItem("تدرج لوني"),
                new ToolStripMenuItem("انزلاق")
            });
            transitionMenuItem.DropDownItemClicked += TransitionMenuItem_DropDownItemClicked;

            viewMenuItem.DropDownItems.Add(resolutionMenuItem);
            viewMenuItem.DropDownItems.Add(formatMenuItem);
            viewMenuItem.DropDownItems.Add(frameRateMenuItem);
            viewMenuItem.DropDownItems.Add(qualityMenuItem);
            viewMenuItem.DropDownItems.Add(transitionMenuItem);

            // قائمة المساعدة
            ToolStripMenuItem facebookMenuItem = new ToolStripMenuItem("Facebook");
            facebookMenuItem.Click += (s, e) => OpenLink("https://www.facebook.com/blindtech22/");
            ToolStripMenuItem telegramMenuItem = new ToolStripMenuItem("Telegram");
            telegramMenuItem.Click += (s, e) => OpenLink("https://t.me/+WbWEsjp_pDg3YTBk");
            ToolStripMenuItem usingGuideMenuItem = new ToolStripMenuItem("دليل الاستخدام");
            usingGuideMenuItem.Click += (s, e) => OpenLink(Path.Combine(Application.StartupPath, "Using guide.html"));
            ToolStripMenuItem settingsMenuItem = new ToolStripMenuItem("الإعدادات (F3)");
            settingsMenuItem.Click += (s, e) => ShowSettings();

            helpMenuItem.DropDownItems.Add(facebookMenuItem);
            helpMenuItem.DropDownItems.Add(telegramMenuItem);
            helpMenuItem.DropDownItems.Add(usingGuideMenuItem);
            helpMenuItem.DropDownItems.Add(settingsMenuItem);

            ToolStripMenuItem googleDriveMenuItem = new ToolStripMenuItem("Google Drive");
            googleDriveMenuItem.Click += GoogleDriveMenuItem_Click;
            ToolStripMenuItem dropboxMenuItem = new ToolStripMenuItem("Dropbox");
            dropboxMenuItem.Click += DropboxMenuItem_Click;
            cloudSaveMenuItem.DropDownItems.Add(googleDriveMenuItem);
            cloudSaveMenuItem.DropDownItems.Add(dropboxMenuItem);

            menuStrip.Items.Add(viewMenuItem);
            menuStrip.Items.Add(helpMenuItem);
            menuStrip.Items.Add(cloudSaveMenuItem);

            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;

            openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Images|*.bmp;*.jpg;*.jpeg;*.png";

            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Video Files|*.mp4;*.avi;*.mkv";

            openAudioDialog = new OpenFileDialog();
            openAudioDialog.Filter = "Audio Files|*.mp3;*.wav";

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.O)
            {
                BtnSelectAudio_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.I)
            {
                BtnSelectImages_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                BtnConvert_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.P)
            {
                BtnPreview_Click(null, null);
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.I)
            {
                SelectFileButton_Click(null, null);
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.E)
            {
                SaveFileButton_Click(null, null);
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.S)
            {
                ConvertButton_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.G)
            {
                BtnSelectImagesForGif_Click(null, null);
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.G)
            {
                BtnConvertToGif_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.X)
            {
                BtnCutVideo_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                BtnCompressVideo_Click(null, null);
            }
            else if (e.KeyCode == Keys.F3)
            {
                ShowSettings();
            }
        }

        private void ResolutionMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            MessageBox.Show($"تم اختيار الدقة: {e.ClickedItem.Text}", "الدقة", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FormatMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            saveFileDialog!.Filter = $"Video Files|*.{e.ClickedItem.Text.ToLower()}";
            MessageBox.Show($"تم اختيار التنسيق: {e.ClickedItem.Text}", "التنسيق", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FrameRateMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            MessageBox.Show($"تم اختيار معدل الإطارات: {e.ClickedItem.Text}", "معدل الإطارات", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void QualityMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            MessageBox.Show($"تم اختيار الجودة: {e.ClickedItem.Text}", "الجودة", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TransitionMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            MessageBox.Show($"تم اختيار التأثير الانتقالي: {e.ClickedItem.Text}", "التأثيرات الانتقالية", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OpenLink(string url)
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ عند محاولة فتح الرابط: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSelectImages_Click(object? sender, EventArgs e)
        {
            if (openFileDialog?.ShowDialog() == DialogResult.OK)
            {
                selectedImages = openFileDialog.FileNames;
                filePathTextBox!.Text = string.Join(", ", selectedImages);
                MessageBox.Show($"{selectedImages.Length} صور تم اختيارها.", "اختيار الصور", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnSelectAudio_Click(object? sender, EventArgs e)
        {
            if (openAudioDialog?.ShowDialog() == DialogResult.OK)
            {
                selectedAudio = openAudioDialog.FileName;
                MessageBox.Show($"تم اختيار الصوت: {selectedAudio}", "اختيار الصوت", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnConvert_Click(object? sender, EventArgs e)
        {
            if (selectedImages.Length == 0)
            {
                MessageBox.Show("يرجى اختيار الصور أولاً.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (saveFileDialog?.ShowDialog() == DialogResult.OK)
            {
                string outputFile = saveFileDialog.FileName;
                progress!.Value = 0;
                CreateVideo(outputFile);
            }
        }

        private void BtnPreview_Click(object? sender, EventArgs e)
        {
            if (selectedImages.Length == 0)
            {
                MessageBox.Show("يرجى اختيار الصور أولاً.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tempOutputFile = Path.Combine(tempDir, "preview.mp4");
            CreateVideo(tempOutputFile);

            // معاينة الفيديو
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tempOutputFile,
                    UseShellExecute = true
                }
            };
            process.Start();
        }

        private void CreateVideo(string outputFile)
        {
            try
            {
                string ffmpegPath = Path.Combine(Application.StartupPath, "bin", "ffmpeg.exe");
                string imageListFile = Path.Combine(tempDir, "images.txt");

                // إنشاء ملف نصي يحتوي على قائمة الصور
                using (StreamWriter writer = new StreamWriter(imageListFile))
                {
                    foreach (string imagePath in selectedImages)
                    {
                        writer.WriteLine($"file '{imagePath}'");
                        writer.WriteLine($"duration {durationInput!.Value}"); // مدة عرض كل صورة بالثواني
                    }
                }

                string tempOutputFile = Path.Combine(tempDir, "tempOutput.mp4");
                string resolution = resolutionMenuItem!.DropDownItems[0].Text.Split('x')[0] + ":" + resolutionMenuItem.DropDownItems[0].Text.Split('x')[1];

                // إنشاء الفيديو باستخدام FFmpeg
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = ffmpegPath,
                        Arguments = $"-f concat -safe 0 -i \"{imageListFile}\" -vf \"scale={resolution}\" -pix_fmt yuv420p \"{tempOutputFile}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit();

                // دمج الصوت إذا كان محددًا
                if (!string.IsNullOrEmpty(selectedAudio))
                {
                    var audioProcess = new System.Diagnostics.Process
                    {
                        StartInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = ffmpegPath,
                            Arguments = $"-i \"{tempOutputFile}\" -i \"{selectedAudio}\" -c:v copy -c:a aac -strict experimental \"{outputFile}\"",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    audioProcess.Start();
                    audioProcess.WaitForExit();
                }
                else
                {
                    File.Move(tempOutputFile, outputFile);
                }

                this.Invoke((MethodInvoker)delegate
                {
                    progress!.Value = 100;
                    MessageBox.Show("تم تنفيذ التحويل بنجاح مع دمج الصوت", "اكتمال التحويل", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    logFile.Close();
                    if (File.Exists(Path.Combine(tempDir, "log.txt")))
                    {
                        File.Delete(Path.Combine(tempDir, "log.txt"));
                    }
                    Directory.Delete(tempDir, true);
                });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"حدث خطأ أثناء التحويل: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logFile.Close();
                });
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        private void SelectFileButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Video Files|*.mp4;*.avi;*.mov;*.mkv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    inputFilePath = openFileDialog.FileName;
                    MessageBox.Show("تم اختيار الملف: " + inputFilePath);
                }
            }
        }

        private void SaveFileButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = $"Audio Files|*.{audioFormatComboBox.SelectedItem.ToString().ToLower()}";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    outputFilePath = saveFileDialog.FileName;
                    MessageBox.Show("سيتم حفظ الملف في: " + outputFilePath);
                }
            }
        }

        private void ConvertButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(inputFilePath))
            {
                MessageBox.Show("يرجى اختيار ملف فيديو أولاً.");
                return;
            }

            if (string.IsNullOrEmpty(outputFilePath))
            {
                MessageBox.Show("يرجى اختيار مكان لحفظ الملف المحول.");
                return;
            }

            string ffmpegPath = Path.Combine(Application.StartupPath, "bin", "ffmpeg.exe");
            string format = audioFormatComboBox.SelectedItem.ToString().ToLower();

            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = $"-i \"{inputFilePath}\" -vn -acodec {format} \"{outputFilePath}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();

            MessageBox.Show("تم تحويل الفيديو إلى صوت بنجاح!");
        }

        // حدث اختيار الصور
        private void BtnSelectImagesForGif_Click(object sender, EventArgs e)
        {
            if (openFileDialog?.ShowDialog() == DialogResult.OK)
            {
                selectedImages = openFileDialog.FileNames;
                MessageBox.Show($"{selectedImages.Length} صور تم اختيارها.", "اختيار الصور", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // حدث تحويل الصور إلى GIF
        private void BtnConvertToGif_Click(object sender, EventArgs e)
        {
            if (selectedImages.Length == 0)
            {
                MessageBox.Show("يرجى اختيار الصور أولاً.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (saveFileDialog?.ShowDialog() == DialogResult.OK)
            {
                string outputFile = saveFileDialog.FileName;
                CreateGif(outputFile, (int)gifDurationInput.Value);
            }
        }

        // دالة إنشاء GIF
        private void CreateGif(string outputFile, int duration)
        {
            try
            {
                string ffmpegPath = Path.Combine(Application.StartupPath, "bin", "ffmpeg.exe");
                string imageListFile = Path.Combine(tempDir, "images.txt");

                // إنشاء ملف نصي يحتوي على قائمة الصور
                using (StreamWriter writer = new StreamWriter(imageListFile))
                {
                    foreach (string imagePath in selectedImages)
                    {
                        writer.WriteLine($"file '{imagePath}'");
                        writer.WriteLine($"duration {duration}");
                    }
                }

                // إنشاء GIF باستخدام FFmpeg
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = ffmpegPath,
                        Arguments = $"-f concat -safe 0 -i \"{imageListFile}\" -vf \"fps=10,scale=320:-1:flags=lanczos\" \"{outputFile}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit();

                MessageBox.Show("تم تحويل الصور إلى GIF بنجاح!", "اكتمال التحويل", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء التحويل: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // حدث تغيير قيمة شريط السحب "من"
        private void TrackBarFrom_Scroll(object sender, EventArgs e)
        {
            labelFrom.Text = $"من: {trackBarFrom.Value} ثانية";
        }

        // حدث تغيير قيمة شريط السحب "إلى"
        private void TrackBarTo_Scroll(object sender, EventArgs e)
        {
            labelTo.Text = $"إلى: {trackBarTo.Value} ثانية";
        }

        // حدث قص الفيديو
        private void BtnCutVideo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(inputFilePath))
            {
                MessageBox.Show("يرجى اختيار ملف فيديو أولاً.");
                return;
            }

            if (saveVideoDialog.ShowDialog() == DialogResult.OK)
            {
                string outputFile = saveVideoDialog.FileName;
                CutVideo(inputFilePath, outputFile, trackBarFrom.Value, trackBarTo.Value);
            }
        }

        // دالة قص الفيديو
        private void CutVideo(string inputFile, string outputFile, int startTime, int endTime)
        {
            try
            {
                string ffmpegPath = Path.Combine(Application.StartupPath, "bin", "ffmpeg.exe");
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = ffmpegPath,
                        Arguments = $"-i \"{inputFile}\" -ss {startTime} -to {endTime} -c copy \"{outputFile}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit();

                MessageBox.Show("تم قص الفيديو بنجاح!", "اكتمال القص", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء قص الفيديو: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // حدث ضغط الفيديو
        private void BtnCompressVideo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(inputFilePath))
            {
                MessageBox.Show("يرجى اختيار ملف فيديو أولاً.");
                return;
            }

            if (saveCompressedVideoDialog.ShowDialog() == DialogResult.OK)
            {
                string outputFile = saveCompressedVideoDialog.FileName;
                CompressVideo(inputFilePath, outputFile);
            }
        }

        // دالة ضغط الفيديو
        private void CompressVideo(string inputFile, string outputFile)
        {
            try
            {
                string ffmpegPath = Path.Combine(Application.StartupPath, "bin", "ffmpeg.exe");
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = ffmpegPath,
                        Arguments = $"-i \"{inputFile}\" -vf \"scale=640:-1\" -c:v libx264 -crf 28 \"{outputFile}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit();

                MessageBox.Show("تم ضغط الفيديو بنجاح!", "اكتمال الضغط", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء ضغط الفيديو: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // حدث رفع إلى Google Drive
        private void GoogleDriveMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("سيتم رفع الفيديو إلى Google Drive.", "رفع إلى Google Drive", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // حدث رفع إلى Dropbox
        private void DropboxMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("سيتم رفع الفيديو إلى Dropbox.", "رفع إلى Dropbox", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowSettings()
        {
            string settingsInfo = $"دليل الحفظ الافتراضي: {Settings.DefaultOutputDirectory}\n" +
                                 $"جودة الصورة الافتراضية: {Settings.DefaultImageQuality}%\n" +
                                 $"معدل البت الافتراضي للفيديو: {Settings.DefaultVideoBitrate} كيلوبت/ثانية\n" +
                                 $"معدل الإطارات الافتراضي: {Settings.DefaultFrameRate}";

            MessageBox.Show(settingsInfo, "الإعدادات", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}