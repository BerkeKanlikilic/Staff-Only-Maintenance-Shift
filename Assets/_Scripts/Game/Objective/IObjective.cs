using System.Collections.Generic;
using UnityEngine;

// Interface for all objectives in the game (binary or progress-based)
public interface IObjective
{
    string Id { get; }
    string Title { get; }
    string Description { get; }
    bool IsCompleted { get; }
    
    void Initialize();                              // Called when objective starts
    void CheckStatus();                             // Used for polling-based check logic
    void OnProgress();                              // Called by external trigger (e.g. stored item, cleaned puddle)
    
    HashSet<GameObject> GetHighlightTargets();      // Objects to highlight for guidance
}