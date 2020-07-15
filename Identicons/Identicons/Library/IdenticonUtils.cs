using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Identicons.Library
{
    public static class IdenticonUtils
    {
        public enum EncryptionTypeEnum
        {
            MD5,
            SHA_1,
            SHA_256,
            SHA_384,
            SHA_512
        };
    }
}
