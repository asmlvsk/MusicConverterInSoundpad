using SoundpadConnector;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;

/*
 * TODO:
 * 1. Saving file name with video.Title               +
 * 2. if(link != true)   							?
 * 3. Adding progressBar                 			-
 * 4. Deleting file after downloading                  +
 * 5. New UI                                      	-
*/
namespace YT2MP3
{
    public partial class Form1 : Form
    {
        public static Soundpad Soundpad;     

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            Soundpad = new Soundpad();
            await Soundpad.ConnectAsync();
            try
            {
                if (String.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Empty field!");
                    return;
                }
                else if(Soundpad.ConnectionStatus != ConnectionStatus.Connected)
                {
                    MessageBox.Show("Please, turn ON your Sounpad");
                    return;
                }
                else
                {
                    Task<string> yt = YouTubeConv();
                    string result = await yt;

                    var ex = File.Exists($"{Environment.CurrentDirectory}/{result}.mp3");
                    while (!ex) { }
                    if (ex == true) { await Sound(result); }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            progressBar1.Value = 0;
        }

        public async Task<string> YouTubeConv()
        {
            YoutubeClient youtube = new YoutubeClient();
            Form1 f1 = new Form1();
            var ytconv = new YoutubeConverter(youtube);

            var video = await youtube.Videos.GetAsync(textBox1.Text);
            var id = video.Id;
            string title = video.Title;
            string newtitle = title
                .Replace(':', '-')
                .Replace('"', '_')
                .Replace(' ', '_')
                .Replace('|', '-');
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(id);

            var audioStreamInfo = streamManifest.GetAudio().WithHighestBitrate();
            var streamInfos = new IStreamInfo[] { audioStreamInfo };
            await ytconv.DownloadAndProcessMediaStreamsAsync(streamInfos, $"{newtitle}.mp3", "mp3");

            return newtitle;
        }

        public async Task Sound(string _result)
        {
            Soundpad = new Soundpad();
            await Soundpad.ConnectAsync();
            if (Soundpad.ConnectionStatus == ConnectionStatus.Connected)
            {
                string result = _result;
                await Soundpad.AddSound($"{Environment.CurrentDirectory}/{result}.mp3");
                File.Delete($"{Environment.CurrentDirectory}/{result}.mp3");
                MessageBox.Show("Трек добавлен в Sounpad");
            }
            else
            {
                MessageBox.Show("Soundpad is not ON");
                return;
            }
        }
    }
}
