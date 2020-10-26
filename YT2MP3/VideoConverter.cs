using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YT2MP3
{
    class VideoConverter
    {
        SaveFileDialog save = new SaveFileDialog();
        FolderBrowserDialog fbg = new FolderBrowserDialog();
        

        public async Task<int> Conv()
        {
            save.Filter = "mp3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            save.ShowDialog();
            var title = save.FileName;
            var path = fbg.SelectedPath;

            try
            {
                await Task.Run(() =>
                {
                    var startInfo = new ProcessStartInfo
                    {

                        FileName = $"{Environment.CurrentDirectory}/ffmpeg.exe",
                        Arguments = $"-i video.webm -vn {title}.mp3",
                        WorkingDirectory = path, // Enviroment.CurrentDirectory
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };

                    using (var process = new Process { StartInfo = startInfo })
                    {
                        process.Start();
                        process.WaitForExit();                       
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return 0;
        }

        public async Task<int> Conv2()
        {

            try
            {
                await Task.Run(() =>
                {
                    var _startInfo = new ProcessStartInfo
                    {

                        FileName = $"{Environment.CurrentDirectory}/ffmpeg.exe",
                        Arguments = $"-i video.webm -vn audio.mp3",                       
                        WorkingDirectory = Environment.CurrentDirectory, // Enviroment.CurrentDirectory
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };
                    
                    using (var process = new Process { StartInfo = _startInfo })
                    {
                        process.Start();
                        System.Console.Beep();
                        process.WaitForExit();
                        process.Close();
                        File.Delete($"{Environment.CurrentDirectory}/video.webm");
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return 0;
        }
    }
}
