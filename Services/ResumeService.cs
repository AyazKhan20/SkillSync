namespace SmartJobRecommender.Services
{
    public interface IResumeService
    {
        Task<List<string>> ExtractSkillsAsync(Stream pdfStream);
        Task<double> CalculateMatchScore(int userId, int jobId);
        Task<List<int>> RecommendJobs(int userId);
    }

    public class ResumeService : IResumeService
    {
        // Placeholder for advanced extraction logic
        public Task<List<string>> ExtractSkillsAsync(Stream pdfStream)
        {
            return Task.FromResult(new List<string> { "C#", ".NET", "SQL" });
        }

        public Task<double> CalculateMatchScore(int userId, int jobId)
        {
            return Task.FromResult(0.0);
        }

        public Task<List<int>> RecommendJobs(int userId)
        {
            return Task.FromResult(new List<int>());
        }
    }
}
