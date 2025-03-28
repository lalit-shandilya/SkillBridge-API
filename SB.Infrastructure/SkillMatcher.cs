using System;
using System.Collections.Generic;
using System.Linq;
namespace SB.Infrastructure
{
    public class SkillMatcher
    {
        public static double CalculateSkillMatchScore(List<string> userSkills, List<string> jobSkills)
        {
            if (jobSkills == null || jobSkills.Count == 0)
                return 0;

            int totalRequiredSkills = jobSkills.Count;
            double matchedSkillScore = 0;

            foreach (var jobSkill in jobSkills)
            {
                if (userSkills.Contains(jobSkill, StringComparer.OrdinalIgnoreCase))
                {
                    matchedSkillScore += 1; // Exact match gets full point
                }
                else if (userSkills.Any(us => IsPartialMatch(us, jobSkill)))
                {
                    matchedSkillScore += 0.5; // Partial match gets 0.5 points
                }
            }

            return (matchedSkillScore / totalRequiredSkills) * 100; // Normalize to percentage
        }

        private static bool IsPartialMatch(string userSkill, string jobSkill)
        {
            return GetLevenshteinDistance(userSkill.ToLower(), jobSkill.ToLower()) <= 2; // Allow minor spelling differences
        }

        private static int GetLevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source)) return target.Length;
            if (string.IsNullOrEmpty(target)) return source.Length;

            int[,] matrix = new int[source.Length + 1, target.Length + 1];

            for (int i = 0; i <= source.Length; i++) matrix[i, 0] = i;
            for (int j = 0; j <= target.Length; j++) matrix[0, j] = j;

            for (int i = 1; i <= source.Length; i++)
            {
                for (int j = 1; j <= target.Length; j++)
                {
                    int cost = (source[i - 1] == target[j - 1]) ? 0 : 1;
                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }

            return matrix[source.Length, target.Length];
        }
    }

}
