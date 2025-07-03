using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using DevExpress.Security;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraPrinting.Native;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SisData.Service;
using SisData.Model;
using Microsoft.AspNetCore.Components;
using DevExpress.CodeParser;
using Blazored.LocalStorage;
using System.Reflection.Metadata;
using SisData.Data;
using System.Text.RegularExpressions;

namespace SISERPSME.Controllers
{
    // Declare a class that stores chunk details.
    public class ChunkMetadata
    {
        public int Index { get; set; }
        public int TotalCount { get; set; }
        public int FileSize { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileGuid { get; set; }
    }

    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        SisGlobals mySisGlobals; 
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IOptionsManager myoptions;
        public UploadController(IWebHostEnvironment hostingEnvironment, IOptionsManager _myoptions, SisGlobals _sisGlobals)
        {
            _hostingEnvironment = hostingEnvironment;
            myoptions = _myoptions;
            mySisGlobals = _sisGlobals;
        }

        [HttpGet("DeleteFile/{lsfile}")]
        public IActionResult DeleteFiles(string lsfile)
        {
            string[] dsfile = lsfile.Split(',');
            string Tmppath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
            var dir = new DirectoryInfo(Tmppath);
            if (dir.Exists)
            {
                foreach (string fname in dsfile)
                {
                    foreach (var file in dir.GetFiles())
                    {
                        if (fname.Equals(file.Name))
                        {
                            file.Delete();
                            break;
                        }
                    }
                }
            }
            return Ok("Ok");
        }

        [HttpGet("SaveFile/{lsfile}")]
        public IActionResult SaveFile(string lsfile)
        {
            string[] dsfile = lsfile.Split(',');
            string Tmppath = Path.Combine(_hostingEnvironment.WebRootPath, "upload_tmp");
            var dir = new DirectoryInfo(Tmppath);
            if (dir.Exists)
            {
                foreach (string fname in dsfile)
                {
                    foreach (var file in dir.GetFiles())
                    {
                        if (fname.EndsWith(file.Name))
                        {
                            SaveUploadedFile(file.FullName, fname);
                            break;
                        }
                    }
                }
            }
            RemoveAllTempFiles(Tmppath);
            return Ok("Ok");
        }

        [HttpGet("SaveFiles/{lsfile}")]
        public IEnumerable<string> SaveFiles(string lsfile)
        {
            List<string> _lsdone = new List<string>();
            string Tmppath = Path.Combine(_hostingEnvironment.WebRootPath, "upload_tmp");
            var dir = new DirectoryInfo(Tmppath);
            if (dir.Exists)
            {
                foreach (var file in dir.GetFiles())
                {
                    if (lsfile.Split(',').Contains(file.Name))
                    {
                        SaveUploadedFiles(file.FullName, file.Name, "uploads", "");
                        _lsdone.Add(file.Name);
                    }
                }
            }
            RemoveAllTempFiles(Tmppath, lsfile);
            return _lsdone;
        }

        [HttpGet("SaveFiles/{subfolder}/{lsfile}")]
        public IEnumerable<string> SaveFiles(string subfolder, string lsfile)
        {
            List<string> _lsdone = new List<string>();
            string Tmppath = Path.Combine(_hostingEnvironment.WebRootPath, "upload_tmp");
            var dir = new DirectoryInfo(Tmppath);
            if (dir.Exists)
            {
                foreach (var file in dir.GetFiles())
                {
                    if (lsfile.Split(',').Contains(file.Name))
                    {
                        SaveUploadedFiles(file.FullName, file.Name, "uploads", subfolder);
                        _lsdone.Add(Path.Combine(subfolder, file.Name));
                    }
                }
            }
            RemoveAllTempFiles(Tmppath, lsfile);
            return _lsdone;
        }

        [HttpGet("SaveImagesFile/{lsfile}")]
        public IActionResult SaveImagesFile(string lsfile)
        {
            string[] dsfile = lsfile.Split(',');
            string Tmppath = Path.Combine(_hostingEnvironment.WebRootPath, "upload_tmp");
            var dir = new DirectoryInfo(Tmppath);
            if (dir.Exists)
            {
                foreach (string fname in dsfile)
                {
                    foreach (var file in dir.GetFiles())
                    {
                        if (fname.EndsWith(file.Name))
                        {
                            SaveUploadedImages(file.FullName, fname);
                            break;
                        }
                    }
                }
            }
            RemoveAllTempFiles(Tmppath);
            return Ok("Ok");
        }

        [HttpGet("SaveImagesFiles/{lsfile}")]
        public IEnumerable<string> SaveImagesFiles(string lsfile)
        {
            List<string> _lsdone = new List<string>();
            string Tmppath = Path.Combine(_hostingEnvironment.WebRootPath, "upload_tmp");
            var dir = new DirectoryInfo(Tmppath);
            if (dir.Exists)
            {
                foreach (var file in dir.GetFiles())
                {
                    if (lsfile.Split(',').Contains(file.Name))
                    {
                        SaveUploadedFiles(file.FullName, file.Name, "images", "");
                        _lsdone.Add(file.Name);
                    }
                }
            }
            RemoveAllTempFiles(Tmppath, lsfile);
            return _lsdone;
        }

        [HttpGet("SaveImagesFiles/{subfolder}/{lsfile}")]
        public IEnumerable<string> SaveImagesFiles(string subfolder, string lsfile)
        {
            List<string> _lsdone = new List<string>();
            string Tmppath = Path.Combine(_hostingEnvironment.WebRootPath, "upload_tmp");
            var dir = new DirectoryInfo(Tmppath);
            if (dir.Exists)
            {
                foreach (var file in dir.GetFiles())
                {
                    if (lsfile.Split(',').Contains(file.Name))
                    {
                        SaveUploadedFiles(file.FullName, file.Name, "images", subfolder);
                        _lsdone.Add(Path.Combine(subfolder, file.Name));
                    }
                }
            }
            RemoveAllTempFiles(Tmppath, lsfile);
            return _lsdone;
        }

        [HttpGet("SaveReportFiles/{lsfile}/{folder0}/{folder1}/{folder2}")]
        public IActionResult SaveReportFiles(string lsfile, string folder0, string folder1, string folder2)
        {
            string[] dsfile = lsfile.Split(',');
            string Tmppath = Path.Combine(_hostingEnvironment.WebRootPath, "upload_tmp");
            var dir = new DirectoryInfo(Tmppath);
            if (dir.Exists)
            {
                foreach (string fname in dsfile)
                {
                    foreach (var file in dir.GetFiles())
                    {
                        if (fname.EndsWith(file.Name))
                        {
                            SaveReportFile(file.FullName, fname, folder0, folder1, folder2);
                            break;
                        }
                    }
                }
            }
            RemoveAllTempFiles(Tmppath);
            return Ok("Ok");
        }

        void SaveReportFile(string tempFilePath, string fileName, string folder0, string folder1, string folder2)
        {
            var path = Path.Combine(_hostingEnvironment.WebRootPath, folder0, folder1, folder2);
            System.IO.File.Copy(tempFilePath, Path.Combine(path, fileName), true);
        }
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadFile")]
        public ActionResult UploadFile(IFormFile myFile)
        {
            try
            {
                // Specify the target location for the uploaded files.
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "upload_tmp");
                // Check whether the target directory exists; otherwise, create it.
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                using (var fileStream = System.IO.File.Create(Path.Combine(path, myFile.FileName)))
                {
                    // Check the file here (its extension, safety, and so on). 
                    // If all checks are passed, save the file.
                    myFile.CopyTo(fileStream);
                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadFiles/{file_guid}")]
        public ActionResult UploadFiles(IFormFile myFile, string file_guid)
        {
            try
            {
                // Specify the target location for the uploaded files.
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "upload_tmp");
                // Check whether the target directory exists; otherwise, create it.
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                using (var fileStream = System.IO.File.Create(Path.Combine(path, file_guid + "_" + myFile.FileName)))
                {
                    // Check the file here (its extension, safety, and so on). 
                    // If all checks are passed, save the file.
                    myFile.CopyTo(fileStream);
                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadImg")]
        public ActionResult UploadImg(IFormFile myFile)
        {
            try
            {
                string user1 = myoptions.GetOptionsValue("M_USER_NAME").Trim().ToUpper();
                string FileName = user1 + ".PNG";
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "Pict");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                using (var fileStream = System.IO.File.Create(Path.Combine(path, FileName)))
                {
                    myFile.CopyTo(fileStream);
                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadChunkFile")]
        public ActionResult UploadChunkFile(IFormFile myFile, string chunkMetadata)
        {
            // Specify the location for temporary files.
            var tempPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
            // Remove temporary files.
            RemoveTempFilesAfterDelay(tempPath, new TimeSpan(0, 5, 0));

            try
            {
                if (!string.IsNullOrEmpty(chunkMetadata))
                {
                    // Get chunk details.
                    var metaDataObject = JsonConvert.DeserializeObject<ChunkMetadata>(chunkMetadata);
                    // Specify the full path for temporary files (inluding the file name).
                    var tempFilePath = Path.Combine(tempPath, metaDataObject.FileGuid + ".tmp");
                    // Check whether the target directory exists; otherwise, create it.
                    if (!Directory.Exists(tempPath))
                        Directory.CreateDirectory(tempPath);
                    // Append the chunk to the file.
                    AppendChunkToFile(tempFilePath, myFile);
                    // Save the file if all chunks are received.
                    if (metaDataObject.Index == (metaDataObject.TotalCount - 1))
                        SaveUploadedFile(tempFilePath, metaDataObject.FileName);
                }
            }
            catch
            {
                return BadRequest();
            }
            return Ok();
        }
        void AppendChunkToFile(string path, IFormFile content)
        {
            using (var stream = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                content.CopyTo(stream);
            }
        }
        void SaveUploadedFile(string tempFilePath, string fileName)
        {
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
            System.IO.File.Copy(tempFilePath, Path.Combine(path, fileName), true);
        }
        void SaveUploadedFiles(string tempFilePath, string fileName, string folder, string subfolder)
        {
            var path = string.IsNullOrEmpty(subfolder) ? Path.Combine(_hostingEnvironment.WebRootPath, folder) : Path.Combine(_hostingEnvironment.WebRootPath, folder, subfolder);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            System.IO.File.Copy(tempFilePath, Path.Combine(path, fileName), true);
        }
        void SaveUploadedImages(string tempFilePath, string fileName)
        {
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "images");
            System.IO.File.Copy(tempFilePath, Path.Combine(path, fileName), true);
        }
        void RemoveAllTempFiles(string path)
        {
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
                foreach (var file in dir.GetFiles())
                    file.Delete();
        }
        void RemoveAllTempFiles(string path, string lsfile)
        {
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                foreach (var file in dir.GetFiles())
                {
                    if (lsfile.Split(',').Contains(file.Name))
                        file.Delete();
                }
            }
        }
        void RemoveTempFilesAfterDelay(string path, TimeSpan delay)
        {
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
                foreach (var file in dir.GetFiles("*.tmp").Where(f => f.LastWriteTimeUtc.Add(delay) < DateTime.UtcNow))
                    file.Delete();
        }

        #region "Cr UploadFileToFolder"
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("CrUploadFileToFolder/{myFolder}")]
        public ActionResult CrUploadFileToFolder(IFormFile myFile, string myFolder)
        {
            try
            {
                // Specify the target location for the uploaded files.
                var path = Path.Combine(_hostingEnvironment.WebRootPath, mySisGlobals.M_Cr_Folder_Document_Temp, myFolder);
                // Check whether the target directory exists; otherwise, create it.
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string file_name_replace = Regex.Replace(myFile.FileName, @"(\s+|@|&|\\|\/|<|>|#|-|\(|\)|%|,|;|:|\ )", "_");
                using (var fileStream = System.IO.File.Create(Path.Combine(path, myFolder + "_" + file_name_replace)))
                {
                    // Check the file here (its extension, safety, and so on). 
                    // If all checks are passed, save the file.
                    myFile.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                WLog(ex.Message);
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("CrUploadFileToFolder")]
        public ActionResult CrUploadFileToFolder(IFormFile myFile)
        {
            try
            {
                // Specify the target location for the uploaded files.
                var path = Path.Combine(_hostingEnvironment.WebRootPath, mySisGlobals.M_Cr_Folder_Document_Temp);
                // Check whether the target directory exists; otherwise, create it.
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string file_name_replace = Regex.Replace(myFile.FileName, @"(\s+|@|&|\\|\/|<|>|#|-|\(|\)|%|,|;|:|\ )", "_");
                using (var fileStream = System.IO.File.Create(Path.Combine(path, Guid.NewGuid().ToString().ToUpper() + "_" + file_name_replace)))
                {
                    // Check the file here (its extension, safety, and so on). 
                    // If all checks are passed, save the file.
                    myFile.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                WLog(ex.Message);
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }
        #endregion

        #region "Cr Save folder"
        [HttpGet("CrSaveFileToFolder/{myFolderTmp}/{myFolder}/{lsfile}")]
        public IActionResult CrSaveFileToFolder(string myFolderTmp, string myFolder, string lsfile)
        {
            string[] dsfile = lsfile.Split(',');
            string Tmppath = Path.Combine(_hostingEnvironment.WebRootPath, mySisGlobals.M_Cr_Folder_Document_Temp, myFolderTmp);
            var dir = new DirectoryInfo(Tmppath);
            if (dir.Exists)
            {
                WLog($"{dir.FullName} already exists.");
                foreach (string fname in dsfile)
                {
                    foreach (var file in dir.GetFiles())
                    {
                        if (fname.EndsWith(file.Name))
                        {
                            WLog($"{dir.FullName} already exists. SAVE");
                            CrSaveUploadedFileToFolder(file.FullName, fname.Replace(myFolderTmp+"_",""), myFolder);
                            break;
                        }
                    }
                }
            }
            CrRemoveTempFilesToFolder(Tmppath, lsfile);
            return Ok("Ok");
        }
        #endregion
        #region "Cr Save file guid folder"
        [HttpGet("CrSaveFileGuidToFolder/{myFolderTmp}/{myFolder}/{lsfile}")]
        public IActionResult CrSaveFileGuidToFolder(string myFolderTmp, string myFolder, string lsfile)
        {
            string[] dsfile = lsfile.Split(',');
            string Tmppath = Path.Combine(_hostingEnvironment.WebRootPath, mySisGlobals.M_Cr_Folder_Document_Temp, myFolderTmp);
            var dir = new DirectoryInfo(Tmppath);
            if (dir.Exists)
            {
                WLog($"{dir.FullName} already exists.");
                foreach (string fname in dsfile)
                {
                    foreach (var file in dir.GetFiles())
                    {
                        if (fname.EndsWith(file.Name))
                        {
                            WLog($"{dir.FullName} already exists. SAVE");
                            CrSaveUploadedFileToFolder(file.FullName, fname, myFolder);
                            break;
                        }
                    }
                }
            }
            CrRemoveTempFilesToFolder(Tmppath, lsfile);
            return Ok("Ok");
        }
        #endregion
        #region "Cr SaveUploadedFileToFolder"
        void CrSaveUploadedFileToFolder(string tempFilePath, string fileName, string myFolder)
        {
            var path = Path.Combine(_hostingEnvironment.WebRootPath, mySisGlobals.M_Cr_Folder_Document, myFolder);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
           
            System.IO.File.Copy(tempFilePath, Path.Combine(path, fileName), true);
        }
        #endregion
        #region "Cr RemoveTempFilesToFolder"
        void CrRemoveTempFilesToFolder(string path, string lsfile)
        {
            string[] dsfile = lsfile.Split(',');
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                foreach (string fname in dsfile)
                {
                    foreach (var file in dir.GetFiles())
                    {
                        if (fname.EndsWith(file.Name))
                        {
                            file.Delete();
                            break;
                        }
                    }
                }
                if (dir.GetFiles().Length == 0 && dir.GetDirectories().Length == 0)
                {
                    Directory.Delete(dir.FullName, true);
                }
            }
        }
        #endregion
        #region "Cr CrDeleteFileToFolder"
        [HttpGet("CrDeleteFileToFolder/{myFolderTmp}/{myFolder}/{lsfile}")]
        public IActionResult CrDeleteFileToFolder(string myFolderTmp, string myFolder, string lsfile)
        {
            try
            {
                string[] dsfile = lsfile.Split(',');
                string Tmppath = Path.Combine(_hostingEnvironment.WebRootPath, mySisGlobals.M_Cr_Folder_Document, myFolder);
                var dir = new DirectoryInfo(Tmppath);
                if (dir.Exists)
                {
                    foreach (string fname in dsfile)
                    {
                        foreach (var file in dir.GetFiles())
                        {
                            if (fname.Equals(file.Name))
                            {
                                WLog(file.FullName);
                                var path = Path.Combine(_hostingEnvironment.WebRootPath, mySisGlobals.M_Cr_Folder_Document_Delete, myFolder);
                                if (!Directory.Exists(path))
                                    Directory.CreateDirectory(path);
                                System.IO.File.Copy(file.FullName, Path.Combine(path, file.Name.Substring(0, file.Name.Length - file.Extension.Length) + "_" + DateTime.Now.ToString("ddMMyyHHmmss") + file.Extension), true);
                                file.Delete();
                                break;
                            }
                        }
                    }
                    if (dir.GetFiles().Length == 0 && dir.GetDirectories().Length == 0)
                    {
                        Directory.Delete(dir.FullName);
                    }
                }
                //string Tmppath1 = Path.Combine(_hostingEnvironment.WebRootPath, mySisGlobals.M_Cr_Folder_Document_Temp, myFolderTmp);
                //CrRemoveTempFilesToFolder(Tmppath1, lsfile);
            }
            catch (Exception ex)
            {
                WLog(ex.Message);
            }
            return Ok("Ok");
        }
        #endregion
        #region "Cr CrDeleteFileLikeToFolder"
        [HttpGet("CrDeleteFileLikeToFolder/{myFolderTmp}/{myFolder}/{lsfile}")]
        public IActionResult CrDeleteFileLikeToFolder(string myFolderTmp, string myFolder, string lsfile)
        {
            try
            {
                string[] dsfile = lsfile.Split(',');
                string Tmppath = Path.Combine(_hostingEnvironment.WebRootPath, mySisGlobals.M_Cr_Folder_Document, myFolder);
                var dir = new DirectoryInfo(Tmppath);
                if (dir.Exists)
                {
                    foreach (string fname in dsfile)
                    {
                        foreach (var file in dir.GetFiles())
                        {
                            if (file.Name.StartsWith(fname.Trim()))
                            {
                                var path = Path.Combine(_hostingEnvironment.WebRootPath, mySisGlobals.M_Cr_Folder_Document_Delete, myFolder);
                                if (!Directory.Exists(path))
                                    Directory.CreateDirectory(path);
                                System.IO.File.Copy(file.FullName, Path.Combine(path, file.Name.Substring(0, file.Name.Length - file.Extension.Length) + "_" + DateTime.Now.ToString("ddMMyyHHmmss") + file.Extension), true);
                                file.Delete();
                            }
                        }
                    }
                    if (dir.GetFiles().Length == 0 && dir.GetDirectories().Length == 0)
                    {
                        Directory.Delete(dir.FullName);
                    }
                }
                //string Tmppath1 = Path.Combine(_hostingEnvironment.WebRootPath, mySisGlobals.M_Cr_Folder_Document_Temp, myFolderTmp);
                //CrRemoveTempFilesToFolder(Tmppath1, lsfile);
            }
            catch (Exception ex)
            {
                WLog(ex.Message);
            }
            return Ok("Ok");
        }
        #endregion
        #region "Cr Delete Folder Temp"
        [HttpGet("CrDeleteFolderTemp/{myFolderTmp}/{tenfile}")]
        public IActionResult CrDeleteFolderTemp(string myFolderTmp, string tenfile)
        {
            try
            {
                string Tmppath = Path.Combine(_hostingEnvironment.WebRootPath, mySisGlobals.M_Cr_Folder_Document_Temp, myFolderTmp);
                var dir = new DirectoryInfo(Tmppath);
                if (dir.Exists)
                {
                    foreach (var file in dir.GetFiles())
                    {
                        if (file.Name.StartsWith(tenfile))
                        {
                            file.Delete();
                        }
                    }
                    if (dir.GetFiles().Length == 0 && dir.GetDirectories().Length == 0)
                    {
                        Directory.Delete(dir.FullName, true);
                    }
                }
            }
            catch (Exception ex)
            {
                WLog(ex.Message);
            }
            return Ok("Ok");
        }
        [HttpGet("CrDeleteFolderTemp/{myFolderTmp}")]
        public IActionResult CrDeleteFolderTemp(string myFolderTmp)
        {
            try
            {
                string Tmppath = Path.Combine(_hostingEnvironment.WebRootPath, mySisGlobals.M_Cr_Folder_Document_Temp, myFolderTmp);
                var dir = new DirectoryInfo(Tmppath);
                if (dir.Exists)
                {
                    Directory.Delete(dir.FullName, true);
                }
            }
            catch (Exception ex)
            {
                WLog(ex.Message);
            }
            return Ok("Ok");
        }
        #endregion

        public static void WLog(string logMessage)
        {
            using (StreamWriter w = System.IO.File.AppendText(@"..\Sqlerr.txt"))
            {
                Log(logMessage.Trim(), w);
            }
        }

        //---------------------------------------------------
        public static void Log(string logMessage, TextWriter w)
        {
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("{0}", logMessage);
            w.WriteLine("-------------------------------");
        }
    }
}