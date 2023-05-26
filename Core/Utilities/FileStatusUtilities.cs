using LibGit2Sharp;

namespace UnityGit.Core.Utilities
{
    public static class FileStatusUtilities
    {
        public static bool IsModified(FileStatus status)
        {
            return (status & (FileStatus.ModifiedInIndex | FileStatus.ModifiedInWorkdir)) != 0;
        }
        
        public static bool IsModified(StatusEntry statusEntry)
        {
            return IsModified(statusEntry.State);
        }
        
        public static bool IsNew(FileStatus status)
        {
            return (status & (FileStatus.NewInIndex | FileStatus.NewInWorkdir)) != 0;
        }
        
        public static bool IsNew(StatusEntry statusEntry)
        {
            return IsNew(statusEntry.State);
        }

        public static bool IsRenamed(FileStatus status)
        {
            return (status & (FileStatus.RenamedInIndex | FileStatus.RenamedInWorkdir)) != 0;
        }

        public static bool IsDeleted(FileStatus status)
        {
            return (status & (FileStatus.DeletedFromIndex | FileStatus.DeletedFromWorkdir)) != 0;
        }

        public static bool Exists(FileStatus status)
        {
            const FileStatus doesNotExistFlags =
                FileStatus.Nonexistent | FileStatus.DeletedFromIndex | FileStatus.DeletedFromWorkdir;
            
            return (status & doesNotExistFlags) == 0;
        }
        
        public static bool Exists(StatusEntry statusEntry)
        {
            return Exists(statusEntry.State);
        }
    }
}