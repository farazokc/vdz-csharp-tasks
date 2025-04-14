using Onboarding.Interfaces;
using System;

namespace Onboarding.Helpers
{
    public class ConsoleProgressNotifier : IProgressNotifier
    {
        private readonly int _progressBarWidth;
        
        public ConsoleProgressNotifier(int progressBarWidth = 50)
        {
            _progressBarWidth = progressBarWidth;
        }
        
        public void NotifyProgress(double progress)
        {
            Console.Write("\r[");
            
            int filledWidth = (int)Math.Floor(progress / 100 * _progressBarWidth);
            
            for (int i = 0; i < _progressBarWidth; i++)
            {
                if (i < filledWidth)
                    Console.Write("█");
                else
                    Console.Write(" ");
            }
            
            Console.Write($"] {progress:F2}%");
        }
    }
}