namespace SB.Application
{
    public class UploadResumeResponse
    {
        public string ResumeUrl { get; set; } = string.Empty;
        public List<string> ExtractedSkills { get; set; } = new();
    }
}
