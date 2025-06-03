using SmartDepo_CaseStudy.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartDepo_CaseStudy.Services;

/// <summary>
/// Manages tram depot operations such as initialization, mission planning, and querying.
/// </summary>
public class TramManager
{
    private readonly List<Tram> _trams = new();
    private readonly PriorityQueue<Tram, int> _tramsWithoutMission = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    /// <summary>
    /// Initializes the depot with N trams, and first C trams have missions.
    /// </summary>
    /// <returns>A tuple indicating success status and a message</returns>
    public async Task<(bool Success, string Message)> InitializeAsync(int n, int c)
    {
        if (n < 0 || c < 0 || c > n)
            return (false, "C must be between 0 and N (and N must not be negative).");

        await _lock.WaitAsync();
        try
        {
            _trams.Clear();
            _tramsWithoutMission.Clear();

            for (int i = 0; i <= n; i++)
            {
                var tram = new Tram { Index = i, HasMission = i <= c };
                _trams.Add(tram);
                if (!tram.HasMission)
                    _tramsWithoutMission.Enqueue(tram, tram.Index);
            }
            return (true, "Depot initialized.");
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Assigns a mission to the first available tram.
    /// The lock is held for 5 seconds after the assignment to simulate a long-running operation.
    /// </summary>
    /// <returns>A tuple indicating success and a message</returns>
    public async Task<(bool Success, string Message)> AssignMissionAsync()
    {
        if (!await _lock.WaitAsync(0))
            return (false, "Another client is currently performing planning.");

        if (_tramsWithoutMission.Count == 0)
        {
            _lock.Release();
            return (false, "No available tram.");
        }

        var tram = _tramsWithoutMission.Dequeue();
        tram.HasMission = true;

        // Background task holds the lock for 5 seconds
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(5000); // Simulate long-running planning
            }
            finally
            {
                _lock.Release();
            }
        });

        return (true, $"Mission assigned to tram #{tram.Index}.");
    }

    public IReadOnlyList<Tram> GetTrams() => _trams;
}