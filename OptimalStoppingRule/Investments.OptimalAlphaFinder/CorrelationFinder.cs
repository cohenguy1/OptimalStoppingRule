using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investments.OptimalAlphaFinder
{
    public class CorrelationFinder
    {
        public static double GetCorrelation(IEnumerable<UserResult> userResults)
        {
            var avgAdviserRating = userResults.Average(userResult => userResult.AdviserRating);
            var avgExSmoothing = userResults.Average(userResult => userResult.ExponentialSmoothedValue);

            var nominator = 0.0;
            var denominatorX = 0.0;
            var denominatorY = 0.0;

            foreach (var user in userResults)
            {
                nominator = nominator + (user.AdviserRating - avgAdviserRating) * (user.ExponentialSmoothedValue - avgExSmoothing);
                denominatorX = denominatorX + (user.AdviserRating - avgAdviserRating) * (user.AdviserRating - avgAdviserRating);
                denominatorY = denominatorY + (user.ExponentialSmoothedValue - avgExSmoothing) * (user.ExponentialSmoothedValue - avgExSmoothing);
            }

            var sqrtX = Math.Sqrt(denominatorX);
            var sqrtY = Math.Sqrt(denominatorY);
            return (nominator / (sqrtX * sqrtY));
        }
    }
}
