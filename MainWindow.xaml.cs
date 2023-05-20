using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.IO;

namespace Аудиоплеер
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private bool userIsDraggingSlider = false;
        private int seleced_audio_index;
        private int playing_audio_index;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;



            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void player_MediaFailed(object sender, ExceptionEventArgs e)
        {
            MessageBox.Show("Ошибка во время открытия файла.");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string path = @"text.txt";
            if (File.ReadAllText(path) != String.Empty)
            {
                string[] platlist_list = File.ReadAllText(path).Split(';');

                foreach (string it in platlist_list)
                {
                    playlist.Items.Add(it.Substring(it.LastIndexOf(@"\") + 1, it.Substring(it.LastIndexOf(@"\") + 1).Length - 4));
                    playlist_hidden.Items.Add(it);
                }
            }

        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            string path = @"text.txt";
            string allItems = string.Join(";", playlist_hidden.Items.OfType<object>());
            File.WriteAllText(path, allItems);

            mediaPlayer.Close(); // При закрытии окна освобождаем объект MediaPlayer
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((mediaPlayer.Source != null) && (mediaPlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                audio_slider.Minimum = 0;
                audio_slider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                audio_slider.Value = mediaPlayer.Position.TotalSeconds;
            }
        }

        private void OpenAudio_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 файлы (*.mp3)|*.mp3";
            if (openFileDialog.ShowDialog() == true)
            {
                mediaPlayer.Open(new Uri(openFileDialog.FileName));
                playlist.Items.Add(openFileDialog.FileName.Substring(openFileDialog.FileName.LastIndexOf(@"\") + 1, openFileDialog.FileName.Substring(openFileDialog.FileName.LastIndexOf(@"\") + 1).Length - 4));
                playlist_hidden.Items.Add(openFileDialog.FileName);
                playing_audio_index = playlist.Items.Count - 1;
                mediaPlayer.Play();
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (pause_button.Content.ToString() == "II")
            {
                pause_button.Content = "▶";
                mediaPlayer.Pause();
            }
            else
            {
                pause_button.Content = "II";
                mediaPlayer.Play();
            }
        }

        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            mediaPlayer.Position = TimeSpan.FromSeconds(audio_slider.Value);
        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            audio_progress_status.Text = TimeSpan.FromSeconds(audio_slider.Value).ToString(@"hh\:mm\:ss");
        }

        private void volume_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = 1 - (double)volume_slider.Value;

        }

        private void playlist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (playlist.SelectedItem != null)
            {
                seleced_audio_index = playlist.SelectedIndex;
            }
        }

        private void delet_Button_Click(object sender, RoutedEventArgs e)
        {
            if (playlist.SelectedItem != null)
            {
                playlist.Items.RemoveAt(seleced_audio_index);
                playlist_hidden.Items.RemoveAt(seleced_audio_index);
            }
        }

        private void play_Button_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Open(new Uri((playlist_hidden.Items[seleced_audio_index]).ToString()));
            pause_button.Content = "II";
            playing_audio_index = seleced_audio_index;
            mediaPlayer.Play();
        }

        private void next_audio_Click(object sender, RoutedEventArgs e)
        {
            if (playing_audio_index < (playlist.Items.Count - 1))
            {
                playing_audio_index++;
                mediaPlayer.Open(new Uri(playlist_hidden.Items[playing_audio_index].ToString()));
                mediaPlayer.Play();
            }
        }

        private void last_button_Click(object sender, RoutedEventArgs e)
        {
            if (playing_audio_index > 0)
            {
                playing_audio_index--;
                mediaPlayer.Open(new Uri(playlist_hidden.Items[playing_audio_index].ToString()));
                mediaPlayer.Play();
            }
        }
    }
}
