using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using CustomFileLibrary;
using System;

namespace CustomFileApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Pencerenin üst kısımdan sürüklenebilmesi için
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        // Pencereyi kapat
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Dosya seçimi
        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                txtFilePath.Text = ofd.FileName;
            }
        }

        // Dosyadan ham veri okuma
        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filePath = txtFilePath.Text;
                var data = CustomFileHandler.Read(filePath);
                lblInfo.Text = $"Dosya başarıyla okundu. Boyut: {data.Length} bayt";
            }
            catch (Exception ex)
            {
                lblInfo.Text = $"Hata: {ex.Message}";
            }
        }

        // Dosyaya örnek bir içerik yazma
        private void WriteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filePath = txtFilePath.Text;
                var content = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }; // Örnek veriler
                bool result = CustomFileHandler.Write(filePath, content, new byte[2] { 0x01, 0x02 });
                lblInfo.Text = result ? "Dosya başarıyla yazıldı." : "Dosya yazma başarısız.";
            }
            catch (Exception ex)
            {
                lblInfo.Text = $"Hata: {ex.Message}";
            }
        }

        // Dosyadan versiyon bilgisi okuma
        private void VersionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filePath = txtFilePath.Text;
                var version = CustomFileHandler.GetFileVersion(filePath);
                lblInfo.Text = $"Dosya versiyonu: {version[0]}.{version[1]}";
            }
            catch (Exception ex)
            {
                lblInfo.Text = $"Hata: {ex.Message}";
            }
        }

        // Metin içeriği yazma
        private void WriteStringButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filePath = txtFilePath.Text;
                // Örnek metin, bunu farklı bir TextBox alandan da verebilirsiniz.
                string exampleText = "Merhaba, dünya!";
                bool result = CustomFileHandler.WriteString(filePath, exampleText);
                lblInfo.Text = result ? "Metin başarıyla yazıldı." : "Metin yazma başarısız.";
            }
            catch (Exception ex)
            {
                lblInfo.Text = $"Hata: {ex.Message}";
            }
        }

        // Metin içeriği okuma
        private void ReadStringButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filePath = txtFilePath.Text;
                string text = CustomFileHandler.ReadString(filePath);
                lblInfo.Text = $"Metin içeriği: {text}";
            }
            catch (Exception ex)
            {
                lblInfo.Text = $"Hata: {ex.Message}";
            }
        }
    }
}
