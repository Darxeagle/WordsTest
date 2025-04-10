using System;
using System.Collections.Generic;
using System.Linq;
using CurrentGame.GameFlow;
using UnityEngine;
using Zenject;

namespace CurrentGame.Gameplay.Models
{
    [Serializable]
    public class LevelModel
    {
        [Inject] private EventManager eventManager;
        
        [SerializeField] private bool initialized = false;
        [SerializeField] private List<Word> words = new List<Word>();
        [SerializeField] private List<PlacedCluster> placedClusters = new List<PlacedCluster>();
        [SerializeField] private List<Cluster> paletteClusters = new List<Cluster>();
        
        public bool Initialized => initialized;
        public IReadOnlyList<Word> Words => words;
        public IReadOnlyList<PlacedCluster> PlacedClusters => placedClusters;
        public IReadOnlyList<Cluster> PaletteClusters => paletteClusters;

        public void Apply(LevelData data)
        {
            words = data.words.Select(wordStr => new Word(wordStr)).ToList();
            paletteClusters = data.clusters.Select(clusterStr => new Cluster(clusterStr)).ToList();
            placedClusters.Clear();
            
            eventManager.TriggerEvent(EventManager.modelUpdated);
            initialized = true;
        }
        
        public void Apply(LevelModel model)
        {
            words = model.words;
            paletteClusters = model.paletteClusters;
            placedClusters = model.placedClusters;
            initialized = model.initialized;
            
            eventManager.TriggerEvent(EventManager.modelUpdated);
        }
        
        public void Clear()
        {
            words.Clear();
            placedClusters.Clear();
            paletteClusters.Clear();
            initialized = false;
            
            eventManager.TriggerEvent(EventManager.modelUpdated);
        }
        
        public void AddPlacedCluster(PlacedCluster placedCluster)
        {
            placedClusters.Add(placedCluster);
            eventManager.TriggerEvent(EventManager.modelUpdated);
        }
        
        public void AddPaletteCluster(Cluster cluster, int insertIndex)
        {
            paletteClusters.Insert(insertIndex, cluster);
            eventManager.TriggerEvent(EventManager.modelUpdated);
        }
        
        public void RemovePlacedCluster(PlacedCluster placedCluster)
        {
            placedClusters.Remove(placedCluster);
            eventManager.TriggerEvent(EventManager.modelUpdated);
        }
        
        public void RemovePaletteCluster(Cluster cluster)
        {
            paletteClusters.Remove(cluster);
            eventManager.TriggerEvent(EventManager.modelUpdated);
        }
    }
}