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
    /// Simulates a longer processing time (e.g., route calculation or DB write).
    /// </summary>
    /// <returns>A tuple indicating success and a message</returns>
    public async Task<(bool Success, string Message)> AssignMissionAsync()
    {
        if (!await _lock.WaitAsync(0))
            return (false, "Another client is currently performing planning.");

        try
        {
            if (_tramsWithoutMission.Count == 0)
                return (false, "No available tram.");

            // Simulate a long-running operation
            await Task.Delay(2000);

            var tram = _tramsWithoutMission.Dequeue();
            tram.HasMission = true;

            return (true, $"Mission assigned to tram #{tram.Index}.");
        }
        finally
        {
            _lock.Release();
        }
    }

    public IReadOnlyList<Tram> GetTrams() => _trams;
}