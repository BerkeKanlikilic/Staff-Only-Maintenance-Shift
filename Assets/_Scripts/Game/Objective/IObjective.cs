using System.Collections.Generic;
using UnityEngine;

public interface IObjective
{
    string Id { get; }
    string Title { get; }
    string Description { get; }
    bool IsCompleted { get; }
    
    void Initialize();
    void CheckStatus(); // Called on server periodically or from triggers
    void OnProgress();  // Called by external triggers like cleaning or grabbing
    
    HashSet<GameObject> GetHighlightTargets();
}