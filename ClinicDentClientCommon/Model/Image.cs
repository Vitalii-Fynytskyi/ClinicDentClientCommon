﻿using ClinicDentClientCommon.Services;

namespace ClinicDentClientCommon.Model
{
    public enum ImageType
    {
        Undefined = 0,
        Regular = 1,
        XRay = 2,
        All = 3,

    }
    public class Image
    {
        public int Id { get; set; }
        public byte[] OriginalBytes
        {
            get
            {
                return originalBytes;
            }
            set
            {
                originalBytes = value;
                if (originalBytes != null)
                    CompressedBytes = ImageCompressor.CompressImage(originalBytes, new System.Drawing.Size(200, 150));
            }
        }
        private byte[] originalBytes;
        public int DoctorId { get; set; }

        public byte[] CompressedBytes { get; set; }
        public bool? IsXRay { get; set; }
        public string FileName { get; set; }
    }
    public class ImagesToClient
    {
        public Image[] Images;
        public int CountPages { get; set; }
    }
}
