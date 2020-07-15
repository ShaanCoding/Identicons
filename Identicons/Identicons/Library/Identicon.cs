using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using System.Drawing;

namespace Identicons.Library
{
    class Identicon
    {
        private string userName;
        private IdenticonUtils.EncryptionTypeEnum encryptionType;
        private int size;
        private int rounds;
        private string salt;
        private int quality;

        private string finalizedHash;
        bool[,] shaMatrix;

        public Identicon(string userName, IdenticonUtils.EncryptionTypeEnum encryptionType, int size, int rounds, string salt, int quality)
        {
            this.userName = userName;
            this.encryptionType = encryptionType;
            this.size = size;
            this.rounds = rounds;
            this.salt = salt;
            this.quality = quality;

            //https://crypto.stackexchange.com/questions/12795/why-do-i-need-to-add-the-original-salt-to-each-hash-iteration-of-a-password Salting is best if done once not during every iteration.

            if (salt != null && salt != "")
            {
                finalizedHash = userName + salt;
            }
            else
            {
                finalizedHash = userName;
            }

            if(rounds > 0)
            {
                for(int i = 0; i < rounds; i++)
                {
                    finalizedHash = GenerateHash(finalizedHash, encryptionType);
                }
            }
            else
            {
                finalizedHash = GenerateHash(finalizedHash, encryptionType);
            }

            shaMatrix = GetBooleanIcon(finalizedHash, size);
        }

        public Bitmap CreateIcon()
        {
            int resolutionInPixels = 180;

            Brush redBrush = Brushes.Red;
            Brush greenBrush = Brushes.LimeGreen;
            Brush cyanBrush= Brushes.LightBlue;
            Brush orangeBrush = Brushes.Orange;
            Brush yellowBrush= Brushes.Yellow;
            Brush pinkBrush = Brushes.HotPink;
            Brush whiteBrush = Brushes.White;

            int red = Convert.ToInt32(finalizedHash.Substring(finalizedHash.Length - 6, 2), 16);
            int green = Convert.ToInt32(finalizedHash.Substring(finalizedHash.Length - 4, 2), 16);
            int blue = Convert.ToInt32(finalizedHash.Substring(finalizedHash.Length - 2, 2), 16);

            Brush tempBrush = new SolidBrush(Color.FromArgb(red, green, blue));

            Bitmap returnBMP = new Bitmap(resolutionInPixels, resolutionInPixels);

           int cellUnitSize = resolutionInPixels / size;

            using (Graphics g = Graphics.FromImage(returnBMP))
            {
                for (int i = 0; i < shaMatrix.GetLength(0); i++)
                {
                    for (int j = 0; j < shaMatrix.GetLength(1); j++)
                    {
                        if (shaMatrix[i, j])
                        {
                            g.FillRectangle(tempBrush, new Rectangle(i * cellUnitSize, j * cellUnitSize, cellUnitSize, cellUnitSize));
                        }
                        else
                        {
                            g.FillRectangle(whiteBrush, new Rectangle(i * cellUnitSize, j * cellUnitSize, cellUnitSize, cellUnitSize));
                        }
                    }
                }
            }

            return returnBMP;
        }

        private bool[,] GetBooleanIcon(string finalizedHash, int size)
        {
            byte[] hashByteArray = GetHashArray(finalizedHash);

            bool[,] splitUpMatrix = new bool[size, size];

            for (int x = 0; x < size; x++)
            {
                int i = x < 3 ? x : size - 1 - x;
                i = i >= hashByteArray.Length ? i % hashByteArray.Length : i;
                for (int y = 0; y < size; y++)
                {
                    if ((hashByteArray[i] >> y & 1) == 1)
                    {
                        splitUpMatrix[x, y] = true;
                    }
                    else
                    {
                        splitUpMatrix[x, y] = false;
                    }
                }
            }


            return splitUpMatrix;
        }

        private byte[] GetHashArray(string finalizedHash)
        {
            string binaryString = string.Join(string.Empty, finalizedHash.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
            return Convert.FromBase64String(binaryString);
        }

        private string GenerateHash(string inputString, IdenticonUtils.EncryptionTypeEnum encryptionType)
        {
            switch(encryptionType)
            {
                case IdenticonUtils.EncryptionTypeEnum.MD5:
                    return GetMD5Hash(inputString);
                case IdenticonUtils.EncryptionTypeEnum.SHA_1:
                    return GetSHA1Hash(inputString);
                case IdenticonUtils.EncryptionTypeEnum.SHA_256:
                    return GetSHA256Hash(inputString);
                case IdenticonUtils.EncryptionTypeEnum.SHA_384:
                    return GetSHA384Hash(inputString);
                case IdenticonUtils.EncryptionTypeEnum.SHA_512:
                    return GetSHA512Hash(inputString);
                default:
                    throw new ArgumentException($"ERROR: Unspecified Hash Algorithm '{encryptionType}'", nameof(encryptionType));
            }
        }

        private string GetSHA512Hash(string inputString)
        {
            using(SHA512Managed sha512 = new SHA512Managed())
            {
                return BitConverter.ToString(sha512.ComputeHash(Encoding.UTF8.GetBytes(inputString))).Replace("-", "");
            }
        }

        private string GetSHA384Hash(string inputString)
        {
            using (SHA384Managed sha384 = new SHA384Managed())
            {
                return BitConverter.ToString(sha384.ComputeHash(Encoding.UTF8.GetBytes(inputString))).Replace("-", "");
            }
        }

        private string GetSHA256Hash(string inputString)
        {
            using (SHA256Managed sha256 = new SHA256Managed())
            {
                return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(inputString))).Replace("-", "");
            }
        }

        private string GetSHA1Hash(string inputString)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                return BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(inputString))).Replace("-", "");
            }
        }

        private string GetMD5Hash(string inputString)
        {
            using(MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(inputString))).Replace("-", "");
            }
        }
    }
}
