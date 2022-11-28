using System.Collections;
using System.Diagnostics;

namespace PrimeTest;
class Program
{

    /// <summary>
    /// Read a number (long) from the console with a default value in case an invalid value is entered
    /// </summary>
    /// <param name="prompt">Message to display to the user</param>
    /// <param name="defaultValue">Default value</param>
    /// <returns>Value entered by the user (or default value if invalid)</returns>
    static long ReadLong(string prompt, long defaultValue)
    {
        Console.Write($"{prompt} (default {defaultValue}):");
        string input = Console.ReadLine().Trim();
        long value = defaultValue;
        if (input != "")
        {
            long.TryParse(input, out value);
        }
        return value;
    }

    static void Main(string[] args)
    {
        // default values
        long min = ReadLong("Minimum value", 0);
        long max = ReadLong("Maximum value", 10000000);
        int threads = (int)ReadLong("How many threads", 5);
        
        // keep track of how many prime numbers have been found
        long primeCount = 0;

        // a task is something that can be executed in parallel
        Task[] tasks = new Task[threads];

        // start performance timing
        Stopwatch timer = new Stopwatch();
        timer.Start();

        // split all numbers into slices
        for (int t = 0; t < threads; t++)
        {
            
            // make a new task
            tasks[t] = Task.Factory.StartNew((arg) =>
            {
                // the "t" variable from the for loop above is passed as a parameter to the task as an object: we need to cast it back to an integer
                int t = (int)arg;

                // work out which numbers this task should process
                long threadPrimeCount = 0;
                long sliceSize = (max - min) / threads;
                long threadMin = min + (sliceSize * t);
                long threadMax = threadMin + sliceSize;
                for (long i = threadMin; i < threadMax; i++)
                {
                    if (isPrime(i))
                    {
                        threadPrimeCount++;
                    }
                }

                // display results for this thread
                Console.WriteLine($"Thread {t} has finished: there are {threadPrimeCount} prime numbers between {threadMin} and {threadMax}");
                primeCount += threadPrimeCount;
            }, t);
        }

        // wait until all tasks have finished
        Task.WaitAll(tasks);
        timer.Stop();
        
        // display overall results
        Console.WriteLine($"There are {primeCount} prime numbers between {min} and {max}");
        Console.WriteLine($"Total time taken: {timer.ElapsedMilliseconds}ms");
    }    

    /// <summary>
    /// Check if n is prime
    /// </summary>
    /// <param name="n">number to text</param>
    /// <returns>True if n is prime</returns>
    static bool isPrime(long n)
    {
        // special cases
        if (n == 1)
            return false;
        if (n == 2)
            return true;
        if (n % 2 == 0)
            return false;

        // check for odd factors from 3 up to the square root of n
        long sqrt = (long)Math.Sqrt(n);
        for(long i = 3; i <= sqrt; i++)
        {
            if (n % i == 0)
                return false;
        }
        return true;
    }
}