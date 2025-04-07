using System;
using System.IO;
using System.Text;

namespace CustomFileLibrary
{
    public static class CustomFileHandler
    {
        // Beklenen magic header (8 byte)
        private static readonly byte[] MagicHeaderBytes = new byte[]
        {
            0x12, 0x00, 0x05, 0x07, 0x12, 0xA0, 0xFF, 0x00
        };

        /// <summary>
        /// Dosyadan ham byte dizisi olarak veri okur.
        /// </summary>
        public static byte[] Read(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Dosya bulunamadı.", filePath);

            return File.ReadAllBytes(filePath);
        }

        /// <summary>
        /// Dosyadan versiyon (2 byte) bilgisini alır.
        /// Magic header'dan sonraki 2 byte'ı döndürür.
        /// </summary>
        public static byte[] GetFileVersion(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Dosya bulunamadı.", filePath);

            byte[] fileBytes = File.ReadAllBytes(filePath);

            // Dosya minimum: 8 byte header + 2 byte versiyon + 4 byte rezerve = 14 byte
            if (fileBytes.Length < 14)
                throw new InvalidDataException("Dosya biçimi geçersiz veya eksik.");

            // Magic header doğru mu kontrolü
            for (int i = 0; i < MagicHeaderBytes.Length; i++)
            {
                if (fileBytes[i] != MagicHeaderBytes[i])
                    throw new InvalidDataException("Magic header hatalı.");
            }

            // Magic header'dan sonraki ilk 2 byte versiyon olsun
            byte[] versionBytes = new byte[2];
            Array.Copy(fileBytes, 8, versionBytes, 0, 2);

            return versionBytes;
        }

        /// <summary>
        /// Verilen içeriği magic header + versiyon + rezerve + içerik formatında dosyaya yazar.
        /// Versiyon: 2 byte
        /// Rezerve: 4 byte
        /// İçerik: n byte
        /// </summary>
        public static bool Write(string filePath, byte[] content, byte[] version = null)
        {
            try
            {
                using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                // 1) Magic header yaz
                fs.Write(MagicHeaderBytes, 0, MagicHeaderBytes.Length);

                // 2) Versiyon yaz (2 byte) – boş verilirse default 0,0
                byte[] versionBytes = version ?? new byte[2] { 0x00, 0x00 };
                fs.Write(versionBytes, 0, versionBytes.Length);

                // 3) Rezerve alan yaz (4 byte) – şimdilik 0 dolduruyoruz
                fs.Write(new byte[4], 0, 4);

                // 4) İçerik yaz
                if (content != null && content.Length > 0)
                    fs.Write(content, 0, content.Length);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Dosyanın magic header'ına bakarak "CustomFile" olup olmadığını döndürür.
        /// </summary>
        public static bool IsCustomFile(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            byte[] fileBytes = File.ReadAllBytes(filePath);

            if (fileBytes.Length < MagicHeaderBytes.Length)
                return false;

            for (int i = 0; i < MagicHeaderBytes.Length; i++)
            {
                if (fileBytes[i] != MagicHeaderBytes[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Metni (string) içerik olarak yazmak için yardımcı metot.
        /// Varsayılan versiyon: [0x00, 0x01]
        /// </summary>
        public static bool WriteString(string filePath, string text, byte[] version = null)
        {
            if (text == null) text = string.Empty;
            byte[] content = Encoding.UTF8.GetBytes(text);
            return Write(filePath, content, version ?? new byte[2] { 0x00, 0x01 });
        }

        /// <summary>
        /// Dosyadaki içerik kısmını UTF8 string olarak döndürür.
        /// </summary>
        public static string ReadString(string filePath)
        {
            if (!IsCustomFile(filePath))
                throw new InvalidDataException("Bu dosya bir 'CustomFile' formatında değildir.");

            byte[] fileBytes = File.ReadAllBytes(filePath);

            // Minimum uzunluk kontrolü
            // 8 (header) + 2 (versiyon) + 4 (rezerve) = 14 byte
            if (fileBytes.Length <= 14)
                return string.Empty;

            // İçerik, 14. byte’tan itibaren başlıyor.
            int contentStartIndex = 8 + 2 + 4;
            int contentLength = fileBytes.Length - contentStartIndex;

            byte[] contentBytes = new byte[contentLength];
            Array.Copy(fileBytes, contentStartIndex, contentBytes, 0, contentLength);

            return Encoding.UTF8.GetString(contentBytes);
        }
    }
}
