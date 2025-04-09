using CurrentGame.GameFlow;
using CurrentGame.Gameplay.Models;
using System.Linq;
using UnityEngine;

namespace CurrentGame.Gameplay.Controllers
{
    public class LevelLoader
    {
        private readonly LevelsDataManager levelsDataManager;

        public LevelLoader(LevelsDataManager levelsDataManager)
        {
            this.levelsDataManager = levelsDataManager;
        }

        public LevelData LoadLevelByIndex(int index)
        {
            return levelsDataManager.GetLevelByIndex(index);
        }
        
        public LevelModel CreateModel(LevelData levelData)
        {
            var model = new LevelModel();
            
            model.words = levelData.words.Select(wordStr => new Word(wordStr)).ToList();
            model.paletteClusters = levelData.clusters.Select(clusterStr => new Cluster(clusterStr)).ToList();

            return model;
        }
    }
}