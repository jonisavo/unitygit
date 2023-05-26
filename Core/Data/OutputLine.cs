namespace UnityGit.Core.Data
{
    public readonly struct OutputLine
    {
        public readonly string Text;
        public readonly bool IsError;
            
        public OutputLine(string text, bool isError)
        {
            Text = text;
            IsError = isError;
        }
    }
}
