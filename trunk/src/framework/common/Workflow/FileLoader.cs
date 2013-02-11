using System.IO;

namespace DL.Framework.Common
{
    public abstract class FileLoader : IWorkflow
    {
        protected readonly string _inputFolder;
        protected readonly string _processedFolder;
        protected readonly string _fileFilter;

        protected readonly FileSystemWatcher _fileWatcher;

        protected FileLoader(string inputFolder, string processedFolder, string fileFilter)
        {
            _inputFolder = inputFolder;
            _processedFolder = processedFolder;
            _fileFilter = fileFilter;

            _fileWatcher = new FileSystemWatcher();
        }

        public abstract void FileCreated(object sender, FileSystemEventArgs e);

        public virtual void Start()
        {
            if (!Directory.Exists(_inputFolder))
                Directory.CreateDirectory(_inputFolder);

            if (!Directory.Exists(_processedFolder))
                Directory.CreateDirectory(_processedFolder);

            _fileWatcher.Path = _inputFolder;
            _fileWatcher.Filter = _fileFilter;
            _fileWatcher.Created += FileCreated;
            _fileWatcher.EnableRaisingEvents = true;
        }

        public virtual void Stop()
        {
            _fileWatcher.EnableRaisingEvents = false;
            _fileWatcher.Created -= FileCreated;
        }
    }
}
