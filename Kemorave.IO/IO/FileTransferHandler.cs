using System;
using System.Collections.Generic;
using System.Linq;

namespace Kemorave.IO
{
    public enum FileOperation { Copy, Move, Delete }

    public class TransferInfo : IDisposable
    {
        public event EventHandler<string> EntryAdded;
        private Queue<TransferInfo> _NextTransfers;
        private readonly Dictionary<string, string> _TransferDictionary;
        private bool _isReadOnly, _isDisposed;
        public TransferInfo(string destination, FileOperation transfer)
        {
            if (destination == null && transfer != FileOperation.Delete)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            _TransferDictionary = new Dictionary<string, string>();
            Destination = destination;
            Transfer = transfer;
        }
        ~TransferInfo()
        {
            Dispose(true);
        }

        private void Dispose(bool finalizer)
        {
            if (!finalizer)
            {
                System.GC.SuppressFinalize(this);
            }
            Lock();
            _isDisposed = true;
            if (_NextTransfers?.Count > 0)
            {
                foreach (TransferInfo item in _NextTransfers)
                {
                    item.Dispose();
                }
                _NextTransfers.Clear();
            }
            _TransferDictionary.Clear();
        }

        internal void Lock()
        {
            _isReadOnly = true;
            if (_NextTransfers?.Count > 0)
            {
                foreach (TransferInfo item in _NextTransfers)
                {
                    item.Lock();
                }
            }
        }


        public virtual void AddFileToTransfer(string file)
        {
            if (_isReadOnly)
            {
                throw new InvalidOperationException("Tranfer info is in read only mode");
            }
            if (Transfer != FileOperation.Delete)
            {
                _TransferDictionary[file] = System.IO.Path.Combine(Destination, Path.GetFileName(file));
            }
            else
            {
                _TransferDictionary[file] = null;
            }
            EntryAdded?.Invoke(this, file);
            Length += new System.IO.FileInfo(file).Length;
            Count++;

        }
        public virtual void AddDirectoryToTransfer(string dir)
        {
            string nextDes = System.IO.Path.Combine(Destination, Path.GetFileName(dir)); ;
            if (Transfer != FileOperation.Delete)
            {
                System.IO.Directory.CreateDirectory(nextDes);
            }

            TransferInfo nextTransfer = new TransferInfo(nextDes, Transfer);
            foreach (string file in System.IO.Directory.EnumerateFiles(nextDes))
            {
                if (_isDisposed)
                {
                    return;
                }
                nextTransfer.AddFileToTransfer(file);
            }
            foreach (string directory in System.IO.Directory.EnumerateDirectories(nextDes))
            {
                if (_isDisposed)
                {
                    return;
                }
                nextTransfer.AddDirectoryToTransfer(directory);
            }
            if (_NextTransfers == null)
            {
                _NextTransfers = new Queue<TransferInfo>();
            }
            Length += nextTransfer.Length;
            Count += nextTransfer.Count;
            _NextTransfers.Enqueue(nextTransfer);
        }

        public void Dispose()
        {
            Dispose(false);
        }

        public string Destination { get; }
        public FileOperation Transfer { get; }
        public virtual long Length { get; protected set; }
        public IReadOnlyCollection<TransferInfo> NextTransfers => _NextTransfers;
        public IReadOnlyDictionary<string, string> TransferDictionary => _TransferDictionary;
        public int Count { get; private set; }
    }

    public class FileTransferHandler : Progress<double>
    {
        public FileTransferHandler(TransferInfo transferInfo, Action<double> handler) : base(handler)
        {
            TransferInfo = transferInfo;
        }
        public TransferInfo TransferInfo { get; }
        public virtual bool IsPaused { get; protected set; }
        public virtual bool IsCurrentCancelled { get; protected set; }
        public virtual bool IsTransferCancelled { get; protected set; }
        public virtual int BufferSize { get; set; } = 1024;
        public virtual double Progress { get; protected set; }
        public virtual double TotalProgress { get; protected set; }
        public virtual long TotalWrittenBytes { get; protected set; }
        public virtual string CurrentFile { get; protected set; }
        public event EventHandler<Exception> Error;
        public event EventHandler<EventArgs> Interrupt;
        public event EventHandler<double> TotalProgressChanged;
        public event EventHandler<string> CurrentCancelled;
        public event EventHandler<EventArgs> AllCancelled;
        public virtual void StartTransfer()
        {
            try
            {
                TransferInfo.Lock();
                ExecuteTransfer(TransferInfo);
            }
            catch (Exception e)
            {
                RaiseError(e);
                CancelAll();
            }
            finally
            {

            }
        }
        protected virtual void ExecuteTransfer(TransferInfo transferInfo)
        {
            if (transferInfo == null)
            {
                throw new ArgumentNullException(nameof(transferInfo));
            }
            int count = 0;
            foreach (KeyValuePair<string, string> item in transferInfo.TransferDictionary)
            {
                if (IsTransferCancelled)
                {
                    break;
                }
                CurrentFile = item.Key;
                try
                {
                    switch (transferInfo.Transfer)
                    {
                        case FileOperation.Copy:
                            CopyFile(item.Key, item.Value); break;
                        case FileOperation.Move:
                            MoveFile(item.Key, item.Value); break;
                        case FileOperation.Delete:
                            DeleteFile(item.Key); break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    RaiseError(e);
                }
                if (IsCurrentCancelled)
                {
                    IsCurrentCancelled = false;
                }
                count++;
                TotalProgress = ToPercentage(count, TransferInfo.Count);
            }
            if (transferInfo.NextTransfers?.Count > 0)
            {
                foreach (TransferInfo transfer in transferInfo.NextTransfers)
                {
                    ExecuteTransfer(transfer);
                }
            }
            if (transferInfo.Transfer != FileOperation.Copy)
            {
                if (IsTransferCancelled)
                {
                    try
                    {
                        System.IO.Directory.Delete(Path.GetParentPath(transferInfo.TransferDictionary.Keys.FirstOrDefault()));
                    }
                    catch (Exception e)
                    {
                        RaiseError(e);
                    }
                }
            }

        }

        private void DeleteFile(string key)
        {
            OnReport(0);
            System.IO.File.Delete(key);
            OnReport(100);
        }

        private void MoveFile(string filePath, string destinationPath)
        {
            if (Path.GetPathRoot(filePath).Equals(Path.GetPathRoot(destinationPath), StringComparison.Ordinal))
            {
                OnReport(0);
                System.IO.File.Move(filePath, destinationPath);
                OnReport(100);
            }
            else
            {
                CopyFile(filePath, destinationPath);
                System.IO.File.Delete(filePath);
            }
        }

        protected virtual void CopyFile(string FilePath, string Destination)
        {
            int BytesWriten = -1;
            long TotalWriten = 0, TotalToWrite = 0;
            byte[] Bytes = new byte[BufferSize];
            try
            {
                using (System.IO.FileStream NewFile = new System.IO.FileStream(Destination, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
                {
                    using (System.IO.FileStream OldFile = new System.IO.FileStream(FilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        TotalToWrite = OldFile.Length;
                        NewFile.SetLength(TotalToWrite);
                        while (((BytesWriten = OldFile.Read(Bytes, 0, BufferSize)) > 0))
                        {
                            while (IsPaused && !IsCurrentCancelled)
                            {
                                System.Threading.Thread.Sleep(100);
                            }
                            if (IsCurrentCancelled)
                            {
                                break;
                            }

                            TotalWriten += BytesWriten;
                            TotalWrittenBytes += TotalWriten;
                            OnReport(ToPercentage(TotalWriten, TotalToWrite));
                            if (TotalWriten - TotalToWrite < BufferSize)
                            {
                                BufferSize = (int)(TotalWriten - TotalToWrite);
                            }
                            NewFile.Write(Bytes, 0, BytesWriten);
                        }
                    }
                }
            }
            catch
            {
                TotalWrittenBytes += TotalToWrite - TotalWriten;
                OnReport(0);
                throw;
            }
        }
        private double ToPercentage(double val, double max)
        {
            return (val * 100) / max;
        }
        protected override void OnReport(double value)
        {
            Progress = value;
            base.OnReport(value);
        }
        protected virtual void OnTotalReport(double value)
        {
            TotalProgress = value;
            TotalProgressChanged?.Invoke(this, value);
        }
        protected virtual void RaiseError(Exception e)
        {
            Error?.Invoke(this, e);
        }
        protected virtual void Pause()
        {
            this.IsPaused = true;
            Interrupt?.Invoke(this, null);
        }
        protected virtual void Resume()
        {
            this.IsPaused = false;
            Interrupt?.Invoke(this, null);
        }
        protected virtual void CancelAll()
        {
            this.IsTransferCancelled = false;
            AllCancelled?.Invoke(this, null);
            CancelCurrent(CurrentFile);
        }
        protected virtual void CancelCurrent(string cancelledFile)
        {
            this.IsCurrentCancelled = false;
            CurrentCancelled?.Invoke(this, cancelledFile);
        }
    }
}