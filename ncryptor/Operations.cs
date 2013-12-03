using Dokan;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ncryptor
{
    public class Operations : DokanOperations, IDisposable
    {
        public Operations(string root)
        {
            this.Root = root;
        }

        public string Root
        {
            get;
            private set;
        }

        private ulong _count = 1;


        #region DokanOperations Members

        //public int CreateFile(string filename, System.IO.FileAccess access, System.IO.FileShare share, System.IO.FileMode mode, System.IO.FileOptions options, DokanFileInfo info)
        //{
        //    var fullpath = this.GetPath(filename);

        //    if (this.GetFileInformation(filename, new FileInformation(), new DokanFileInfo(0)
        //    {
        //        IsDirectory = System.IO.Directory.Exists(fullpath)
        //    }) == 0)
        //    {
        //        return Dokan.DOKAN_SUCCESS;
        //    }
        //    else
        //    {
        //        return Dokan.ERROR_FILE_NOT_FOUND;
        //    }
        //}

        //public int OpenDirectory(string filename, DokanFileInfo info)
        //{
        //    var fullpath = this.GetPath(filename);

        //    if (System.IO.Directory.Exists(fullpath))
        //    {
        //        info.IsDirectory = true;
        //        return Dokan.DOKAN_SUCCESS;
        //    }

        //    return Dokan.ERROR_PATH_NOT_FOUND;
        //}

        //public int CreateDirectory(string filename, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int Cleanup(string filename, DokanFileInfo info)
        //{
        //    return Dokan.DOKAN_SUCCESS;
        //}

        //public int CloseFile(string filename, DokanFileInfo info)
        //{
        //    return Dokan.DOKAN_SUCCESS;
        //}

        //public int ReadFile(string filename, byte[] buffer, ref uint readBytes, long offset, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int WriteFile(string filename, byte[] buffer, ref uint writtenBytes, long offset, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int FlushFileBuffers(string filename, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int GetFileInformation(string filename, FileInformation fileinfo, DokanFileInfo info)
        //{
        //    var fullpath = this.GetPath(filename);

        //    if (info.IsDirectory && System.IO.Directory.Exists(fullpath))
        //    {
        //        var di = new DirectoryInfo(fullpath);

        //        fileinfo.Attributes = di.Attributes;
        //        fileinfo.CreationTime = di.CreationTime;
        //        fileinfo.FileName = di.Name;
        //        fileinfo.LastAccessTime = di.LastAccessTime;
        //        fileinfo.LastWriteTime = di.LastWriteTime;
        //        fileinfo.Length = 0;

        //        return Dokan.DOKAN_SUCCESS;
        //    }
        //    else if (!info.IsDirectory && System.IO.File.Exists(fullpath))
        //    {
        //        var fi = new FileInfo(fullpath);

        //        fileinfo.Attributes = fi.Attributes;
        //        fileinfo.CreationTime = fi.CreationTime;
        //        fileinfo.FileName = fi.Name;
        //        fileinfo.LastAccessTime = fi.LastAccessTime;
        //        fileinfo.LastWriteTime = fi.LastWriteTime;
        //        fileinfo.Length = fi.Length;

        //        return Dokan.DOKAN_SUCCESS;
        //    }

        //    return Dokan.ERROR_FILE_NOT_FOUND;
        //}

        //public int FindFiles(string filename, System.Collections.ArrayList files, DokanFileInfo info)
        //{
        //    var dir = this.GetPath(filename);

        //    if (System.IO.Directory.Exists(dir))
        //    {
        //        var fileInfos = from f in System.IO.Directory.GetFiles(dir)
        //                        let fi = new FileInfo(f)
        //                        select new FileInformation()
        //                        {
        //                            Attributes = fi.Attributes,
        //                            CreationTime = fi.CreationTime,
        //                            FileName = fi.Name,
        //                            LastAccessTime = fi.LastAccessTime,
        //                            LastWriteTime = fi.LastWriteTime,
        //                            Length = fi.Length
        //                        };

        //        files.AddRange(fileInfos.ToArray());

        //        return Dokan.DOKAN_SUCCESS;
        //    }

        //    return Dokan.ERROR_FILE_NOT_FOUND;
        //}

        //public int SetFileAttributes(string filename, System.IO.FileAttributes attr, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int SetFileTime(string filename, DateTime ctime, DateTime atime, DateTime mtime, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int DeleteFile(string filename, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int DeleteDirectory(string filename, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int MoveFile(string filename, string newname, bool replace, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int SetEndOfFile(string filename, long length, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int SetAllocationSize(string filename, long length, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int LockFile(string filename, long offset, long length, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int UnlockFile(string filename, long offset, long length, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int GetDiskFreeSpace(ref ulong freeBytesAvailable, ref ulong totalBytes, ref ulong totalFreeBytes, DokanFileInfo info)
        //{
        //    var di = new DriveInfo(this.Directory.Take(1).First().ToString());

        //    //TODO
        //    freeBytesAvailable = (ulong)di.AvailableFreeSpace;
        //    totalBytes = (ulong)di.TotalSize;
        //    totalFreeBytes = (ulong)di.TotalFreeSpace;
        //    return Dokan.DOKAN_SUCCESS;
        //}

        //public int Unmount(DokanFileInfo info)
        //{
        //    return Dokan.DOKAN_SUCCESS;
        //}

        public int CreateFile(String filename, FileAccess access, FileShare share,
            FileMode mode, FileOptions options, DokanFileInfo info)
        {
            string path = GetPath(filename);
            info.Context = _count++;
            if (File.Exists(path))
            {
                return DokanNet.DOKAN_SUCCESS;
            }
            else if (Directory.Exists(path))
            {
                info.IsDirectory = true;
                return DokanNet.DOKAN_SUCCESS;
            }
            else
            {
                return -DokanNet.ERROR_FILE_NOT_FOUND;
            }
        }

        public int OpenDirectory(String filename, DokanFileInfo info)
        {
            info.Context = _count++;
            if (Directory.Exists(GetPath(filename)))
                return DokanNet.DOKAN_SUCCESS;
            else
                return -DokanNet.ERROR_PATH_NOT_FOUND;
        }

        public int CreateDirectory(String filename, DokanFileInfo info)
        {
            return DokanNet.DOKAN_ERROR;
        }

        public int Cleanup(String filename, DokanFileInfo info)
        {
            //Console.WriteLine("%%%%%% count = {0}", info.Context);
            return DokanNet.DOKAN_SUCCESS;
        }

        public int CloseFile(String filename, DokanFileInfo info)
        {
            return DokanNet.DOKAN_SUCCESS;
        }

        public int ReadFile(String filename, Byte[] buffer, ref uint readBytes,
            long offset, DokanFileInfo info)
        {
            try
            {
                using(var fs = File.OpenRead(GetPath(filename)))
                {
                    if (Cryptography.IsEncrypted(fs))
                    {
                        var cs = Cryptography.DecryptFile(fs, "test", null);

                        fs.Seek(offset, SeekOrigin.Current);
                        readBytes = (uint)cs.Read(buffer, 0, buffer.Length);
                        return DokanNet.DOKAN_SUCCESS;
                    }
                    else
                    {


                        fs.Seek(offset, SeekOrigin.Begin);
                        readBytes = (uint)fs.Read(buffer, 0, buffer.Length);
                        return DokanNet.DOKAN_SUCCESS;
                    }
                }
            }
            catch (Exception)
            {
                return DokanNet.DOKAN_ERROR;
            }
        }

        public int WriteFile(String filename, Byte[] buffer,
            ref uint writtenBytes, long offset, DokanFileInfo info)
        {
            return DokanNet.DOKAN_ERROR;
        }

        public int FlushFileBuffers(String filename, DokanFileInfo info)
        {
            return DokanNet.DOKAN_ERROR;
        }

        public int GetFileInformation(String filename, FileInformation fileinfo, DokanFileInfo info)
        {
            string path = GetPath(filename);
            if (File.Exists(path))
            {
                FileInfo f = new FileInfo(path);

                fileinfo.Attributes = f.Attributes | FileAttributes.NotContentIndexed;
                fileinfo.CreationTime = f.CreationTime;
                fileinfo.LastAccessTime = f.LastAccessTime;
                fileinfo.LastWriteTime = f.LastWriteTime;

                using (var fs = f.OpenRead())
                {
                    if (Cryptography.IsEncrypted(fs))
                    {
                        fileinfo.Length = Cryptography.GetLength(fs);
                    }
                    else
                    {
                        fileinfo.Length = f.Length;
                    }
                }
                return DokanNet.DOKAN_SUCCESS;
            }
            else if (Directory.Exists(path))
            {
                DirectoryInfo f = new DirectoryInfo(path);

                fileinfo.Attributes = f.Attributes | FileAttributes.NotContentIndexed;
                fileinfo.CreationTime = f.CreationTime;
                fileinfo.LastAccessTime = f.LastAccessTime;
                fileinfo.LastWriteTime = f.LastWriteTime;
                fileinfo.Length = 0;// f.Length;
                return DokanNet.DOKAN_SUCCESS;
            }
            else
            {
                return DokanNet.DOKAN_ERROR;
            }
        }

        public int FindFiles(String filename, ArrayList files, DokanFileInfo info)
        {
            string path = GetPath(filename);
            if (Directory.Exists(path))
            {
                DirectoryInfo d = new DirectoryInfo(path);
                FileSystemInfo[] entries = d.GetFileSystemInfos();
                foreach (FileSystemInfo f in entries)
                {
                    FileInformation fi = new FileInformation();
                    fi.Attributes = f.Attributes | FileAttributes.NotContentIndexed;
                    fi.CreationTime = f.CreationTime;
                    fi.LastAccessTime = f.LastAccessTime;
                    fi.LastWriteTime = f.LastWriteTime;
                    fi.Length = (f is DirectoryInfo) ? 0 : ((FileInfo)f).Length;
                    fi.FileName = f.Name;
                    files.Add(fi);
                }
                return DokanNet.DOKAN_SUCCESS;
            }
            else
            {
                return DokanNet.DOKAN_ERROR;
            }
        }

        public int SetFileAttributes(String filename, FileAttributes attr, DokanFileInfo info)
        {
            return DokanNet.DOKAN_ERROR;
        }

        public int SetFileTime(String filename, DateTime ctime,
                DateTime atime, DateTime mtime, DokanFileInfo info)
        {
            return DokanNet.DOKAN_ERROR;
        }

        public int DeleteFile(String filename, DokanFileInfo info)
        {
            return DokanNet.DOKAN_ERROR;
        }

        public int DeleteDirectory(String filename, DokanFileInfo info)
        {
            return DokanNet.DOKAN_ERROR;
        }

        public int MoveFile(String filename, String newname, bool replace, DokanFileInfo info)
        {
            return DokanNet.DOKAN_ERROR;
        }

        public int SetEndOfFile(String filename, long length, DokanFileInfo info)
        {
            return DokanNet.DOKAN_ERROR;
        }

        public int SetAllocationSize(String filename, long length, DokanFileInfo info)
        {
            return DokanNet.DOKAN_ERROR;
        }

        public int LockFile(String filename, long offset, long length, DokanFileInfo info)
        {
            return DokanNet.DOKAN_SUCCESS;
        }

        public int UnlockFile(String filename, long offset, long length, DokanFileInfo info)
        {
            return DokanNet.DOKAN_SUCCESS;
        }

        public int GetDiskFreeSpace(ref ulong freeBytesAvailable, ref ulong totalBytes,
            ref ulong totalFreeBytes, DokanFileInfo info)
        {
            var di = new DriveInfo(this.Root.Take(1).First().ToString());
            freeBytesAvailable = (ulong)di.AvailableFreeSpace;
            totalBytes = (ulong)di.TotalSize;
            totalFreeBytes = (ulong)di.TotalFreeSpace;
            return DokanNet.DOKAN_SUCCESS;
        }

        public int Unmount(DokanFileInfo info)
        {
            return DokanNet.DOKAN_SUCCESS;
        }

        #endregion

        #region IDokanOperations Members

        //public int CreateFile(string dokanPath, uint rawAccessMode, uint rawShare, uint rawCreationDisposition, uint rawFlagsAndAttributes, DokanFileInfo info)
        //{
        //    var fullpath = this.GetPath(dokanPath);
        //    info.refFileHandleContext = _count++;

        //    var fi = new FileInformation();
        //    if (this.GetFileInformation(dokanPath, ref fi, new DokanFileInfo()
        //    {
        //        IsDirectory = System.IO.Directory.Exists(fullpath)
        //    }) == 0)
        //    {
        //        return Dokan.DOKAN_SUCCESS;
        //    }
        //    else
        //    {
        //        return Dokan.ERROR_FILE_NOT_FOUND;
        //    }
        //}

        //public int OpenDirectory(string dokanPath, DokanFileInfo info)
        //{
        //    var fullpath = this.GetPath(dokanPath);
        //    info.refFileHandleContext = _count++;

        //    if (System.IO.Directory.Exists(fullpath))
        //    {
        //        info.IsDirectory = true;
        //        return Dokan.DOKAN_SUCCESS;
        //    }

        //    return Dokan.ERROR_PATH_NOT_FOUND;
        //}

        //public int CreateDirectory(string dokanPath, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int Cleanup(string dokanPath, DokanFileInfo info)
        //{
        //    return Dokan.DOKAN_SUCCESS;
        //}

        //public int CloseFile(string dokanPath, DokanFileInfo info)
        //{
        //    return Dokan.DOKAN_SUCCESS;
        //}

        //public int ReadFileNative(string file, IntPtr rawBuffer, uint rawBufferLength, ref uint rawReadLength, long rawOffset, DokanFileInfo convertFileInfo)
        //{
        //    throw new NotImplementedException();
        //}

        //public int WriteFileNative(string dokanPath, IntPtr rawBuffer, uint rawNumberOfBytesToWrite, ref uint rawNumberOfBytesWritten, long rawOffset, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int FlushFileBuffers(string dokanPath, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int GetFileInformation(string dokanPath, ref FileInformation fileinfo, DokanFileInfo info)
        //{
        //    var fullpath = this.GetPath(dokanPath);

        //    if (info.IsDirectory && System.IO.Directory.Exists(fullpath))
        //    {
        //        var di = new DirectoryInfo(fullpath);

        //        fileinfo.Attributes = di.Attributes | FileAttributes.NotContentIndexed;
        //        fileinfo.CreationTime = di.CreationTime;
        //        fileinfo.FileName = di.Name;
        //        fileinfo.LastAccessTime = di.LastAccessTime;
        //        fileinfo.LastWriteTime = di.LastWriteTime;
        //        fileinfo.Length = 0;

        //        return Dokan.DOKAN_SUCCESS;
        //    }
        //    else if (!info.IsDirectory && System.IO.File.Exists(fullpath))
        //    {
        //        var fi = new FileInfo(fullpath);

        //        fileinfo.Attributes = fi.Attributes | FileAttributes.NotContentIndexed;
        //        fileinfo.CreationTime = fi.CreationTime;
        //        fileinfo.FileName = fi.Name;
        //        fileinfo.LastAccessTime = fi.LastAccessTime;
        //        fileinfo.LastWriteTime = fi.LastWriteTime;
        //        fileinfo.Length = fi.Length;

        //        return Dokan.DOKAN_SUCCESS;
        //    }

        //    return Dokan.ERROR_FILE_NOT_FOUND;
        //}

        //public int FindFiles(string filename, out FileInformation[] files, DokanFileInfo info)
        //{
        //    return FindFilesWithPattern(filename, "*", out files, info);
        //}

        //public int FindFilesWithPattern(string filename, string pattern, out FileInformation[] files, DokanFileInfo info)
        //{
        //    var directory = this.GetPath(filename);

        //    if (System.IO.Directory.Exists(directory))
        //    {
        //        var fileInfos = from f in System.IO.Directory.GetFiles(directory, pattern)
        //                        let fi = new FileInfo(f)
        //                        select new FileInformation()
        //                        {
        //                            Attributes = fi.Attributes | FileAttributes.NotContentIndexed,
        //                            CreationTime = fi.CreationTime,
        //                            FileName = fi.Name,
        //                            LastAccessTime = fi.LastAccessTime,
        //                            LastWriteTime = fi.LastWriteTime,
        //                            Length = fi.Length
        //                        };

        //        files = fileInfos.ToArray();
        //        return Dokan.DOKAN_SUCCESS;
        //    }

        //    files = new FileInformation[0];
        //    return Dokan.ERROR_FILE_NOT_FOUND;
        //}

        //public int SetFileAttributes(string filename, FileAttributes attr, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int SetFileTimeNative(string filename, ref System.Runtime.InteropServices.ComTypes.FILETIME rawCreationTime, ref System.Runtime.InteropServices.ComTypes.FILETIME rawLastAccessTime, ref System.Runtime.InteropServices.ComTypes.FILETIME rawLastWriteTime, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int DeleteFile(string filename, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int DeleteDirectory(string filename, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int MoveFile(string filename, string newname, bool replace, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int SetEndOfFile(string filename, long length, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int SetAllocationSize(string filename, long length, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int LockFile(string filename, long offset, long length, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int UnlockFile(string filename, long offset, long length, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //public int GetDiskFreeSpace(ref ulong freeBytesAvailable, ref ulong totalBytes, ref ulong totalFreeBytes, DokanFileInfo info)
        //{
        //    var di = new DriveInfo(this.Directory.Take(1).First().ToString());

        //    //TODO
        //    freeBytesAvailable = (ulong)di.AvailableFreeSpace;
        //    totalBytes = (ulong)di.TotalSize;
        //    totalFreeBytes = (ulong)di.TotalFreeSpace;
        //    return Dokan.DOKAN_SUCCESS;
        //}

        //public int Unmount(DokanFileInfo info)
        //{
        //    return Dokan.DOKAN_SUCCESS;
        //}

        //public int GetFileSecurityNative(string file, ref SECURITY_INFORMATION rawRequestedInformation, ref SECURITY_DESCRIPTOR rawSecurityDescriptor, uint rawSecurityDescriptorLength, ref uint rawSecurityDescriptorLengthNeeded, DokanFileInfo info)
        //{
        //    return Dokan.DOKAN_SUCCESS;
        //    var fullpath = this.GetPath(file);
        //    var dokanReturn = Dokan.DOKAN_ERROR;

        //    try
        //    {
        //        if (!GetFileSecurity(fullpath, rawRequestedInformation, ref rawSecurityDescriptor, rawSecurityDescriptorLength, ref rawSecurityDescriptorLengthNeeded))
        //        {
        //            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error(), new IntPtr(-1));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        dokanReturn = BestAttemptToWin32(ex);
        //    }
        //    finally
        //    {
        //    }
        //    return dokanReturn;
        //}

        //public int SetFileSecurityNative(string file, ref SECURITY_INFORMATION rawSecurityInformation, ref SECURITY_DESCRIPTOR rawSecurityDescriptor, uint rawSecurityDescriptorLength, DokanFileInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        #region IDisposible

        public void Dispose()
        {
            this.Unmount(null);
        }

        #endregion

        #region methods

        private string GetPath(string filename)
        {
            return this.Root + filename;
        }

        //public static int HiWord(int number)
        //{
        //    return ((number & 0x80000000) == 0x80000000) ? number >> 16 : (number >> 16) & 0xffff;
        //}

        //public static int LoWord(int number)
        //{
        //    return number & 0xffff;
        //}

        //public static int BestAttemptToWin32(Exception ex)
        //{
        //    /*
        //    System.ArgumentException: path is a zero-length string, contains only white space, or contains one or more invalid characters as defined by System.IO.Path.InvalidPathChars. 

        //    System.ArgumentNullException: path is null. 

        //    System.IO.PathTooLongException: The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. 

        //    System.IO.DirectoryNotFoundException: The specified path is invalid (for example, it is on an unmapped drive). 

        //    System.IO.IOException: An I/O error occurred while opening the file. 

        //    System.UnauthorizedAccessException: This operation is not supported on the current platform.
        //      -or- 
        //     path specified a directory.
        //      -or- 
        //      The caller does not have the required permission. 

        //    System.IO.FileNotFoundException: The file specified in path was not found. 

        //    System.NotSupportedException: path is in an invalid format. 

        //    System.Security.SecurityException: The caller does not have the required permission. 
        //    */
        //    if (ex.InnerException is SocketException)
        //    {
        //        return -((SocketException)ex.InnerException).ErrorCode;
        //    }
        //    else
        //    {
        //        int HrForException = Marshal.GetHRForException(ex);
        //        if (ex is IOException)
        //        {
        //            switch (HrForException)
        //            {
        //                case -2147024784:
        //                    return Dokan.ERROR_DISK_FULL;
        //            }
        //        }
        //        return (HiWord(HrForException) == 0x8007) ? -LoWord(HrForException) : Dokan.ERROR_EXCEPTION_IN_SERVICE;
        //    }
        //}

        #endregion

        #region interop

        //[DllImport("AdvAPI32.DLL", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        //private static extern bool GetFileSecurity(string lpdokanPath, SECURITY_INFORMATION requestedInformation, ref SECURITY_DESCRIPTOR pSecurityDescriptor,
        //   uint length, ref uint lengthNeeded);

        #endregion

    }
}
