using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Game.Objective
{
    public abstract class ProgressObjective : IObjective
    {
        public string Id { get; protected set; }
        public string Title { get; protected set; }
        public string Description { get; protected set; }
        public bool IsCompleted => _currentAmount >= _targetAmount;

        protected int _targetAmount;
        protected int _currentAmount;
        
        public virtual void Initialize()
        {
            _currentAmount = 0;
        }
        
        public abstract void CheckStatus(); // Optional for live tracking or failure detection
        
        public virtual void OnProgress()
        {
            _currentAmount++;
            if (_currentAmount >= _targetAmount)
            {
                OnComplete();
            }
        }

        protected virtual void OnComplete()
        {
            Debug.Log($"Objective Complete: {Title}");
        }
        
        public float GetProgress01() => Mathf.Clamp01((float)_currentAmount / _targetAmount);
        
        public virtual HashSet<GameObject> GetHighlightTargets() => new();
    }
}
