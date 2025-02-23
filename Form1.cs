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
        private ProgressBar? progress;
        private Button? btnSelectImages;
        private Button? btnConvert;
        private OpenFileDialog? openFileDialog;
        private SaveFileDialog? saveFileDialog;
        private OpenFileDialog? openAudioDialog;
        private string? selectedAudio;
        private NumericUpDown? durationInput;
        private ToolStripMenuItem? resolutionMenuItem;
        private ToolStripMenuItem? formatMenuItem;
        private ToolStripMenuItem? frameRateMenuItem;
        private ToolStripMenuItem? qualityMenuItem;
        private ToolStripMenuItem? transitionMenuItem;
        private TextBox? filePathTextBox;
        private Button? btnPreview;

        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
            selectedImages = new string[0];
            tempDir = Path.Combine(Path.GetTempPath(), "ConverterToVideo");
            Directory.CreateDirectory(tempDir);
            logFile = new StreamWriter(Path.Combine(tempDir, "log.txt"));
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Audio To Video Converter";
            this.Size = new Size(600, 400);

            progress = new ProgressBar();
            progress.Location = new Point(10, 10);
            progress.Size = new Size(560, 20);
            this.Controls.Add(progress);

            btnSelectImages = new Button();
            btnSelectImages.Text = "اختر الصور (CTRL+I)";
            btnSelectImages.Location = new Point(10, 50);
            btnSelectImages.Click += BtnSelectImages_Click;
            btnSelectImages.AccessibleName = "اختر الصور";
            btnSelectImages.AccessibleDescription = "استخدم الاختصار CTRL+I لاختيار الصور";
            this.Controls.Add(btnSelectImages);

            btnConvert = new Button();
            btnConvert.Text = "تحويل إلى فيديو (CTRL+S)";
            btnConvert.Location = new Point(150, 50);
            btnConvert.Click += BtnConvert_Click;
            btnConvert.AccessibleName = "تحويل إلى فيديو";
            btnConvert.AccessibleDescription = "استخدم الاختصار CTRL+S لتحويل الصور إلى فيديو";
            this.Controls.Add(btnConvert);

            Button btnSelectAudio = new Button();
            btnSelectAudio.Text = "اختر الصوت (CTRL+O)";
            btnSelectAudio.Location = new Point(10, 90);
            btnSelectAudio.Click += BtnSelectAudio_Click;
            btnSelectAudio.AccessibleName = "اختر الصوت";
            btnSelectAudio.AccessibleDescription = "استخدم الاختصار CTRL+O لاختيار الصوت";
            this.Controls.Add(btnSelectAudio);

            durationInput = new NumericUpDown();
            durationInput.Location = new Point(150, 90);
            durationInput.Minimum = 1;
            durationInput.Maximum = 10;
            durationInput.Value = 2;
            durationInput.Width = 100;
            this.Controls.Add(durationInput);

            filePathTextBox = new TextBox();
            filePathTextBox.Location = new Point(10, 130);
            filePathTextBox.Size = new Size(560, 20);
            filePathTextBox.ReadOnly = true;
            this.Controls.Add(filePathTextBox);

            btnPreview = new Button();
            btnPreview.Text = "معاينة (CTRL+P)";
            btnPreview.Location = new Point(10, 170);
            btnPreview.Click += BtnPreview_Click;
            btnPreview.AccessibleName = "معاينة";
            btnPreview.AccessibleDescription = "استخدم الاختصار CTRL+P لمعاينة الفيديو";
            this.Controls.Add(btnPreview);

            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem fileMenuItem = new ToolStripMenuItem("ملف");
            ToolStripMenuItem editMenuItem = new ToolStripMenuItem("تحرير");
            ToolStripMenuItem viewMenuItem = new ToolStripMenuItem("عرض");
            ToolStripMenuItem helpMenuItem = new ToolStripMenuItem("مساعدة");

            // قائمة ملف
            ToolStripMenuItem newProjectMenuItem = new ToolStripMenuItem("مشروع جديد");
            ToolStripMenuItem saveProjectMenuItem = new ToolStripMenuItem("حفظ المشروع");
            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("خروج");
            exitMenuItem.Click += (s, e) => this.Close();
            fileMenuItem.DropDownItems.Add(newProjectMenuItem);
            fileMenuItem.DropDownItems.Add(saveProjectMenuItem);
            fileMenuItem.DropDownItems.Add(exitMenuItem);

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
            ToolStripMenuItem facebookMenuItem = new ToolStripMenuItem("فيسبوك");
            facebookMenuItem.Click += (s, e) => OpenLink("https://www.facebook.com/blindtech22/");
            ToolStripMenuItem telegramMenuItem = new ToolStripMenuItem("تليجرام");
            telegramMenuItem.Click += (s, e) => OpenLink("https://t.me/+WbWEsjp_pDg3YTBk");
            ToolStripMenuItem usingGuideMenuItem = new ToolStripMenuItem("دليل الاستخدام");
            usingGuideMenuItem.Click += (s, e) => OpenLink(Path.Combine(Application.StartupPath, "Using guide.html"));
            helpMenuItem.DropDownItems.Add(facebookMenuItem);
            helpMenuItem.DropDownItems.Add(telegramMenuItem);
            helpMenuItem.DropDownItems.Add(usingGuideMenuItem);

            menuStrip.Items.Add(fileMenuItem);
            menuStrip.Items.Add(editMenuItem);
            menuStrip.Items.Add(viewMenuItem);
            menuStrip.Items.Add(helpMenuItem);

            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;

            openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Images|*.bmp;*.jpg;*.jpeg;*.png";

            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Video Files|*.mp4;*.avi;*.mkv";

            openAudioDialog = new OpenFileDialog();
            openAudioDialog.Filter = "Audio Files|*.mp3;*.wav";

            menuStrip.Location = new Point(0, 0);
            menuStrip.Size = new Size(600, 24);

            // إضافة اختصارات لوحة المفاتيح
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
    }
}