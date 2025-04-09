using UnityEngine;
using System.IO;
using System.Linq;
using WordsTest.Gameplay.Models;

namespace WordsTest.Gameplay
{
    public class LevelLoader
    {
        public LevelData LoadLevel(string levelId)
        {
            string jsonPath = Path.Combine(Application.streamingAssetsPath, $"Levels/{levelId}.json");
            if (File.Exists(jsonPath))
            {
                string jsonContent = File.ReadAllText(jsonPath);
                return JsonUtility.FromJson<LevelData>(jsonContent);
            }
            else
            {
                Debug.LogError($"Level file not found: {jsonPath}");
                return null;
            }
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