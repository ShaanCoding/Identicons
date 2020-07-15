﻿using Identicons.Library;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Identicons
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isLoaded = false;

        private IdenticonUtils.EncryptionTypeEnum encryptionType = IdenticonUtils.EncryptionTypeEnum.MD5;
        private string userName = "";
        private string salt = "";
        private int size = 6;
        private int rounds = 0;
        private int quality = 600;

        private Identicon identicon;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateIdenticon()
        {
            if(userName != "" && isLoaded)
            {
                identicon = new Identicon(userName, encryptionType, size, rounds, salt, quality);
                IdenticonCanvas.Source = IdenticonUtils.Convert(identicon.CreateIcon());
            }
        }

        private void UserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            userName = UserNameTextBox.Text;
            UpdateIdenticon();
        }

        private void MD5_Checked(object sender, RoutedEventArgs e)
        {
            encryptionType = IdenticonUtils.EncryptionTypeEnum.MD5;
            UpdateIdenticon();
        }

        private void SHA1_Checked(object sender, RoutedEventArgs e)
        {
            encryptionType = IdenticonUtils.EncryptionTypeEnum.SHA_1;
            UpdateIdenticon();
        }

        private void SHA256_Checked(object sender, RoutedEventArgs e)
        {
            encryptionType = IdenticonUtils.EncryptionTypeEnum.SHA_256;
            UpdateIdenticon();
        }

        private void SHA384_Checked(object sender, RoutedEventArgs e)
        {
            encryptionType = IdenticonUtils.EncryptionTypeEnum.SHA_384;
            UpdateIdenticon();
        }

        private void SHA512_Checked(object sender, RoutedEventArgs e)
        {
            encryptionType = IdenticonUtils.EncryptionTypeEnum.SHA_512;
            UpdateIdenticon();
        }

        private void SizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(SizeTextBox.Text, out int size);
            SetSize(size);

            UpdateIdenticon();
        }

        private void SetSize(int size)
        {
            if(size > 0)
            {
                this.size = size;
            }
        }

        private void SaltTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            salt = SaltTextBox.Text;

            UpdateIdenticon();
        }

        private void RoundsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(RoundsTextBox.Text, out rounds);

            UpdateIdenticon();
        }

        private void QualityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(QualityTextBox.Text, out int quality);
            SetQuality(quality);

            UpdateIdenticon();
        }

        private void SetQuality(int quality)
        {
            if(quality > 0)
            {
                this.quality = quality;
            }
        }

        private void Identicon_ContentRendered(object sender, EventArgs e)
        {
            isLoaded = true;
            UpdateIdenticon();
        }

        private void IdenticonCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                using (CommonSaveFileDialog saveFileDialog = new CommonSaveFileDialog())
                {
                    saveFileDialog.AlwaysAppendDefaultExtension = true;
                    saveFileDialog.Filters.Add(new CommonFileDialogFilter("PNG Files", "*.png"));
                    saveFileDialog.DefaultExtension = "png";

                    if (saveFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        identicon.CreateIcon().Save(saveFileDialog.FileName);
                    }
                }
            }
            catch(IOException ex)
            {
                MessageBox.Show("An exception has occured, the image was unable to be saved.");
            }
        }
    }
}
