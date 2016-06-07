using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DIT.Activist.Webservice.Helpers
{
    public class FileHelpers
    {
        public static bool TryGetExtension(string fileName, out string extension)
        {
            extension = "";
            if (String.IsNullOrEmpty(fileName))
            {
                return false;
            }

            if (fileName.Contains(".") == false)
            {
                return false;
            }

            var parts = fileName.Split('.');
            extension = parts[parts.Length - 1];
            return true;
        }

        public static Filetypes GetFileType(string extension)
        {
            switch (extension.ToLower())
            {
                case "csv":
                    return Filetypes.CSV;
                case "idx1":
                    return Filetypes.IDX1;
                case "idx3":
                    return Filetypes.IDX3;
                case "bin":
                    return Filetypes.CSV;
                default:
                    throw new ArgumentException("Unrecognized extension: " + extension);
            }
        }

        public enum Filetypes
        {
            CSV,
            IDX3,
            IDX1
        }
    }
}